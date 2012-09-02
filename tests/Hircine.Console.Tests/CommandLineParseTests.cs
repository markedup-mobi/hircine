using System;
using System.Linq;
using Hircine.Core.Connectivity;
using Hircine.Core.Tests;
using NUnit.Framework;

namespace Hircine.Console.Tests
{
    [TestFixture(Description = "Test fixture for ensuring that our")]
    public class CommandLineParseTests
    {
        #region Setup / Teardown
        #endregion

        #region Tests

        [Test(Description = "A simple commandline call with just an embed flag and a valid assembly name")]
        public void Should_Parse_Successful_Options_for_Just_Assembly_and_EmbeddedDb()
        {
            var args = new[] {"-e", string.Format("-a {0}", TestHelper.ValidTestAssemblyPath)};
            bool showHelp;

            var command = IndexCommandBuilder.ParseIndexBuildCommand(args, out showHelp);

            Assert.IsFalse(showHelp);
            Assert.IsTrue(command.UseEmbedded);
            Assert.AreEqual(0, command.ConnectionStrings.Count());
            Assert.AreEqual(1, command.AssemblyPaths.Count());
            Assert.IsFalse(command.ExecuteJobsSequentially);
            Assert.IsFalse(command.ContinueJobOnFailure);
        }

        [Test(Description = "A simple commandline call with just the help flag set should passs")]
        public void Should_Parse_Successful_Options_for_Just_Show_Help()
        {
            var args = new[] { "-h" };
            bool showHelp;

            var command = IndexCommandBuilder.ParseIndexBuildCommand(args, out showHelp);

            Assert.IsTrue(showHelp);
            Assert.IsFalse(command.UseEmbedded);
            Assert.AreEqual(0, command.ConnectionStrings.Count());
            Assert.AreEqual(0, command.AssemblyPaths.Count());
            Assert.IsFalse(command.ExecuteJobsSequentially);
            Assert.IsFalse(command.ContinueJobOnFailure);
        }

        [Test(Description = "Should be able to handle a commandline that contains a single connection string and an assembly")]
        public void Should_Parse_Connection_String_and_Assembly()
        {
            var args = new[] { string.Format("-a {0}", TestHelper.ValidTestAssemblyPath), string.Format("-c {0}", RavenConnectionStringBuilder.BuildConnectionStringWithApiKey("http://localhost:8080", Guid.NewGuid().ToString())) };
            bool showHelp;

            var command = IndexCommandBuilder.ParseIndexBuildCommand(args, out showHelp);

            Assert.IsFalse(showHelp);
            Assert.IsFalse(command.UseEmbedded);
            Assert.AreEqual(1, command.ConnectionStrings.Count());
            Assert.AreEqual(1, command.AssemblyPaths.Count());
            Assert.IsFalse(command.ExecuteJobsSequentially);
            Assert.IsFalse(command.ContinueJobOnFailure);
        }

        [Test(Description = "Should be able to handle a commandline that contains a single connection string, an assembly, and some build flags")]
        public void Should_Parse_Connection_String_and_Assembly_With_SequentialFlags()
        {
            var args = new[] { string.Format("-a {0}", TestHelper.ValidTestAssemblyPath), 
                string.Format("-c {0}", RavenConnectionStringBuilder.BuildConnectionStringWithApiKey("http://localhost:8080", Guid.NewGuid().ToString())),
            "-s", "-f"};
            bool showHelp;

            var command = IndexCommandBuilder.ParseIndexBuildCommand(args, out showHelp);

            Assert.IsFalse(showHelp);
            Assert.IsFalse(command.UseEmbedded);
            Assert.AreEqual(1, command.ConnectionStrings.Count());
            Assert.AreEqual(1, command.AssemblyPaths.Count());
            Assert.IsTrue(command.ExecuteJobsSequentially);
            Assert.IsTrue(command.ContinueJobOnFailure);
        }

        [Test(Description = "Should be able to handle a commandline that contains multiple connection strings and assemblies")]
        public void Should_Parse_Multiple_Connection_Strings_and_Assemblies()
        {
            var args = new[] { string.Format("-a {0}", TestHelper.ValidTestAssemblyPath), 
                string.Format("-c {0}", RavenConnectionStringBuilder.BuildConnectionStringWithApiKey("http://localhost:8080", Guid.NewGuid().ToString())),
                string.Format("-c {0}", RavenConnectionStringBuilder.BuildConnectionStringWithApiKey("http://localhost:8081", Guid.NewGuid().ToString())),
                string.Format("-c {0}", RavenConnectionStringBuilder.BuildConnectionStringWithApiKey("http://localhost:8082", Guid.NewGuid().ToString())),
                string.Format("-a {0}", TestHelper.InvalidAssemblyPath)
            };
            bool showHelp;

            var command = IndexCommandBuilder.ParseIndexBuildCommand(args, out showHelp);

            Assert.IsFalse(showHelp);
            Assert.IsFalse(command.UseEmbedded);
            Assert.AreEqual(3, command.ConnectionStrings.Count());
            Assert.AreEqual(2, command.AssemblyPaths.Count());
            Assert.IsFalse(command.ExecuteJobsSequentially);
            Assert.IsFalse(command.ContinueJobOnFailure);
        }

        #endregion
    }
}
