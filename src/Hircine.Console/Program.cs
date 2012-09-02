using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Hircine.Core;
using Hircine.Core.Indexes;
using Mono.Options;

namespace Hircine.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var showHelp = false;
            IndexBuildCommand indexCommand;
            try
            {
                indexCommand = IndexCommandParser.ParseIndexBuildCommand(args, out showHelp);
            }
            catch (OptionException e)
            {
                WriteError();
                System.Console.Write("hircine: ");
                System.Console.WriteLine(e.Message);
                WriteStandard();
                System.Console.WriteLine("Try 'hircine --help' for more information.");
                return;
            }

            if (showHelp)
            {
                ShowHelp(IndexCommandParser.Options);
                return;
            }

            System.Console.WriteLine(string.Format("Hircine Version {0}", Assembly.GetExecutingAssembly().FullName));

            //Create our job manager for actually building RavenDB indexes
            var indexJobManager = new IndexJobManager(indexCommand);

            System.Console.WriteLine("Validating assemblies...");

            var assemblyResults = indexJobManager.CanLoadAssemblies();
            if(assemblyResults.Successes == assemblyResults.JobResults.Count)
            {
                WriteSuccess();
                System.Console.WriteLine(string.Format("Loaded all ({0}) assemblies and was able to find indexes inside all of them."));
                WriteStandard();
                System.Console.WriteLine();
            } else
            {
                var errorJobs = assemblyResults.JobResults.Where(x => x.WasFound == false);
                WriteWarning();
                System.Console.WriteLine("Errors loading assemblies...");
                foreach(var errorJob in errorJobs)
                {
                    WriteError();
                    System.Console.WriteLine("Error!");
                    WriteStandard();
                    System.Console.WriteLine("Unable to load assembly {0}", errorJob.ResourceName);
                    System.Console.WriteLine("Error message: {0}", errorJob.JobException.Message);
                    System.Console.WriteLine();
                }
                return;
            }

            //Attempt to connect to our databases
            var connectionResult = indexJobManager.CanConnectToDbs();
            System.Console.WriteLine("Validating RavenDB connections...");
            if(connectionResult.Successes == connectionResult.JobResults.Count)
            {
                WriteSuccess();
                System.Console.WriteLine(string.Format("Was able to connect to all ({0}) RavenDB instances successfully."));
                WriteStandard();
                System.Console.WriteLine();
            } else
            {
                var errorJobs = connectionResult.JobResults.Where(x => x.WasFound == false);
                WriteWarning();
                System.Console.WriteLine("Errors connecting to databases...");
                foreach (var errorJob in errorJobs)
                {
                    WriteError();
                    System.Console.WriteLine("Error!");
                    WriteStandard();
                    System.Console.WriteLine("Unable to connect to RavenDB database {0}", errorJob.ResourceName);
                    System.Console.WriteLine("Error message: {0}", errorJob.JobException.Message);
                    System.Console.WriteLine();
                }
                return;
            }

            //And with all of that out of the way, now it's time to actually run our job
            System.Console.WriteLine("Starting index-building job...");
            System.Console.WriteLine();

            var results = indexJobManager.Build(result =>
                                      {
                                          if(result.Result == BuildResult.Success)
                                          {
                                              WriteSuccess();
                                              System.Console.WriteLine("Able to successfully build index {0} in server {1}", result.IndexName, result.ConnectionString);
                                              System.Console.WriteLine();
                                          }
                                          else
                                          {
                                              WriteError();
                                              System.Console.WriteLine("Error!");
                                              System.Console.WriteLine("Unable to build index {0} in server {1}", result.IndexName, result.ConnectionString);
                                              System.Console.WriteLine("Exception: {0}", result.BuildException);
                                              System.Console.WriteLine();
                                          }
                                      });

            if(results.Sum(x => x.Completed) == results.Sum(x => x.BuildResults.Count))
            {
                WriteSuccess();
                System.Console.WriteLine("Success");
                WriteStandard();
            } else
            {
                WriteError();
                System.Console.WriteLine("Failure");
                WriteStandard();
            }

            //Dispose of the job manager at the end
            indexJobManager.Dispose();
        }

        static void WriteError()
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
        }

        static void WriteStandard()
        {
            System.Console.ForegroundColor = ConsoleColor.White;
        }

        static void WriteSuccess()
        {
            System.Console.ForegroundColor = ConsoleColor.Green;
        }

        static void WriteWarning()
        {
            System.Console.ForegroundColor = ConsoleColor.DarkYellow;
        }

        static void ShowHelp(OptionSet p)
        {
            System.Console.WriteLine("Usage: Hircine [OPTIONS]+");
            System.Console.WriteLine("Greet a list of individuals with an optional message.");
            System.Console.WriteLine("If no message is specified, a generic greeting is used.");
            System.Console.WriteLine();
            System.Console.WriteLine("Options:");
            p.WriteOptionDescriptions(System.Console.Out);
        }
    }
}
