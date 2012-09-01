using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hircine.Core.Connectivity;
using Hircine.Core.Indexes;
using Hircine.Core.Runtime;
using NUnit.Framework;
using Raven.Client.Indexes;

namespace Hircine.Core.Tests.Indexes
{
    [TestFixture(Description = "Test fixture used for verifying that we can successfully create indexes using IndexBuilder")]
    public class IndexCreationTests
    {
        private IRavenInstanceFactory _ravenInstanceFactory;
        private Assembly _indexAssembly;

        private IndexBuilder _indexBuilder;

        #region Setup / Teardown

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _ravenInstanceFactory = new DefaultRavenInstanceFactory();
            _indexAssembly = AssemblyRuntimeLoader.LoadAssembly(TestHelper.ValidTestAssemblyPath);
        }

        #endregion

        #region Test Models

        public class SimpleModel
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Tag { get; set; }
            public DateTimeOffset DateCreated { get; set; }
        }

        public class OtherSimpleModel
        {
            public string Id { get; set; }
            public double Price { get; set; }
            public int NumberOfAlcoholicRabbits { get; set; }
            public string Tag { get; set; }
            public DateTimeOffset DateCreated { get; set; }
        }

        public class TotalDocumentsWithTagPerDay
        {
            public string Tag { get; set; }
            public DateTimeOffset Day { get; set; }
            public int Total { get; set; }
        }

        #endregion

        #region Test Indexes

        //Should be able to create this index, a valid multi-map reduce index
        public class ValidMultiMapReduceIndex : AbstractMultiMapIndexCreationTask<TotalDocumentsWithTagPerDay>
        {
            public ValidMultiMapReduceIndex()
            {
                AddMap<SimpleModel>(models => from model in models
                                                  select new
                                                             {
                                                                 Tag = model.Tag,
                                                                 Day = model.DateCreated.Date,
                                                                 Total = 1
                                                             });

                AddMap<OtherSimpleModel>(models => from model in models
                                                       select new
                                                                  {
                                                                      Tag = model.Tag,
                                                                      Day = model.DateCreated.Date,
                                                                      Total = 1 
                                                                  });

                Reduce = results => from result in results
                                    group result by new {result.Day, result.Tag}
                                    into g
                                    select new
                                               {
                                                   Tag = g.Key.Tag,
                                                   Day = g.Key.Day,
                                                   Total = g.Sum(x => x.Total)
                                               };
            }
        }

        //Should NOT be able to create this index, an INVALID multi-map reduce index
        public class InvalidMultiMapReduceIndex : AbstractMultiMapIndexCreationTask<TotalDocumentsWithTagPerDay>
        {
            public InvalidMultiMapReduceIndex()
            {
                AddMap<SimpleModel>(models => from model in models
                                              select new
                                              {
                                                  Tag = model.Tag,
                                                  Day = model.DateCreated.Date,
                                                  Total = 1
                                              });

                AddMap<OtherSimpleModel>(models => from model in models
                                                   select new
                                                   {
                                                       Tag = model.Tag,
                                                       Day = model.DateCreated.Date,
                                                       Total = 1,
                                                       DrunkAnimals = model.NumberOfAlcoholicRabbits //Can't have an extra field on only one of the mapped documents
                                                   });

                Reduce = results => from result in results
                                    group result by new { result.Day, result.Tag }
                                        into g
                                        select new
                                        {
                                            Tag = g.Key.Tag,
                                            Day = g.Key.Day,
                                            Total = g.Sum(x => x.Total),
                                            DrunkGoats = "LIE" //Extraneous field
                                        };
            }
        }

        #endregion

        #region Tests

        [Test(Description = "Should be able to build our valid RavenDB indexes against an in-memory test database")]
        public void Should_Synchronously_Create_Indexes_Against_Embedded_Database()
        {
            //Assert one pre-condition: must have n > 0 indexes in the assembly before we begin
            var numberOfTargetIndexes = AssemblyRuntimeLoader.GetRavenDbIndexes(_indexAssembly).Count;
            Assert.IsTrue(numberOfTargetIndexes > 0, "Pre-condition failed: must have at least 1 index in the defined assembly");

            var embeddedDb = _ravenInstanceFactory.GetEmbeddedInstance(runInMemory: true);
            embeddedDb.Initialize();

            var indexBuilder = new IndexBuilder(embeddedDb, _indexAssembly);
            try
            {
                var indexBuildResults = indexBuilder.Run(null);
                Assert.IsNotNull(indexBuildResults);
                Assert.IsTrue(indexBuildResults.Completed > 0, "Should have been able to successfully build at least 1 index");
                Assert.AreEqual(numberOfTargetIndexes, indexBuildResults.Completed, "Expected the number of built indexes to match the number of indexes defined in the assembly");
                Assert.IsTrue(indexBuildResults.Cancelled == 0, "Should not have had any index building jobs cancelled");
                Assert.IsTrue(indexBuildResults.Failed == 0, "Should not have had any index building jobs fail");
            }
            catch (InvalidOperationException ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                indexBuilder.Dispose();
            }
        }

        [Test(Description = "We should be able to receive a failure notification back from RavenDb when we try to create an index that is invalid")]
        public void Should_Report_IndexCreationFailure_When_Building_Invalid_Index()
        {
            var invalidMultiMapIndex = new InvalidMultiMapReduceIndex();

            var embeddedDb = _ravenInstanceFactory.GetEmbeddedInstance(runInMemory: true);
            embeddedDb.Initialize();

            var indexBuilder = new IndexBuilder(embeddedDb, _indexAssembly);

            try
            {

                var indexBuildResult = indexBuilder.BuildIndex(invalidMultiMapIndex);

                Assert.IsNotNull(indexBuildResult);
                Assert.AreEqual(invalidMultiMapIndex.IndexName, indexBuildResult.IndexName);
                Assert.AreEqual(BuildResult.Failed, indexBuildResult.Result);
            }
            catch (InvalidOperationException ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                indexBuilder.Dispose();
            }
        }

        [Test(Description = "We should recieve a success notification when we are able to successfully build an index against RavenDB")]
        public void Should_Report_IndexCreationSuccess_When_Building_Valid_Index()
        {
            var validMultiMapIndex = new ValidMultiMapReduceIndex();

            var embeddedDb = _ravenInstanceFactory.GetEmbeddedInstance(runInMemory: true);
            embeddedDb.Initialize();

            var indexBuilder = new IndexBuilder(embeddedDb, _indexAssembly);

            try
            {

                var indexBuildResult = indexBuilder.BuildIndex(validMultiMapIndex);

                Assert.IsNotNull(indexBuildResult);
                Assert.AreEqual(validMultiMapIndex.IndexName, indexBuildResult.IndexName);
                Assert.AreEqual(BuildResult.Success, indexBuildResult.Result);
            }
            catch (InvalidOperationException ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                indexBuilder.Dispose();
            }
        }

        [Test(Description = "Should be able to report progress on batch jobs via the callback we pass in without any issues")]
        public void Should_Report_Progress_on_Batch_Jobs()
        {
            //Assert one pre-condition: must have n > 0 indexes in the assembly before we begin
            var numberOfTargetIndexes = AssemblyRuntimeLoader.GetRavenDbIndexes(_indexAssembly).Count;
            Assert.IsTrue(numberOfTargetIndexes > 0, "Pre-condition failed: must have at least 1 index in the defined assembly");

            var embeddedDb = _ravenInstanceFactory.GetEmbeddedInstance(runInMemory: true);
            embeddedDb.Initialize();

            var listBuildResults = new List<IndexBuildResult>();

            var indexBuilder = new IndexBuilder(embeddedDb, _indexAssembly);
            try
            {
                var indexBuildResults = indexBuilder.Run(x =>
                                                             {
                                                                 //Add the results to the list as the test runs
                                                                 listBuildResults.Add(x);
                                                             });

                //Assert that the job was valid first
                Assert.IsNotNull(indexBuildResults);
                Assert.IsTrue(indexBuildResults.Completed > 0, "Should have been able to successfully build at least 1 index");
                Assert.AreEqual(numberOfTargetIndexes, indexBuildResults.Completed, "Expected the number of built indexes to match the number of indexes defined in the assembly");
                Assert.IsTrue(indexBuildResults.Cancelled == 0, "Should not have had any index building jobs cancelled");
                Assert.IsTrue(indexBuildResults.Failed == 0, "Should not have had any index building jobs fail");

                //Now assert that progress was reported correctly and completely
                Assert.AreEqual(numberOfTargetIndexes, listBuildResults.Count , "Expected the number of calls against the progress method to be equal to the number of indexes in the assembly");
                Assert.AreEqual(indexBuildResults.Completed, listBuildResults.Count, "Expected the number of calls against the progress method to be equal to the number of valid indexes built from the assembly, which should be ALL of them in this case");
            }
            catch (InvalidOperationException ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                indexBuilder.Dispose();
            }
        }

        #endregion
    }
}
