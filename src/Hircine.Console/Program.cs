using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hircine.Core;
using Mono.Options;

namespace Hircine.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var showHelp = false;
            try
            {
                var indexCommand = IndexCommandBuilder.ParseIndexBuildCommand(args, out showHelp);
            }
            catch (OptionException e)
            {
                System.Console.Write("hircine: ");
                System.Console.WriteLine(e.Message);
                System.Console.WriteLine("Try 'hircine --help' for more information.");
            }

            if (showHelp)
            {
                ShowHelp(IndexCommandBuilder.Options);
            }
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
