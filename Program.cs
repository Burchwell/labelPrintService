using LabelService3.Models;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Net.NetworkInformation;

namespace LabelService3
{
    class Program
    {
        static void Main(string[] args)
        {
            Globals.Url = "127.0.0.1";
            Globals.Subdomain = "skar-audio-label-printer";
            Globals.Port = 8665;
            Globals.Printer = "";
            Globals.Mode = "Production";
            Globals.PingInterval = 5;
            Globals.NovaUrl = "https://nova.skaraudio.com/api";
            Globals.CoreUrl = "https://core.skaraudio.com/api/v1";
            Globals.PingUrl = "https://cronitor.link/qPQIfk";
            Globals.PingInterval = 2; // In Minutes

            // Important: License Key For PDFs do not remove;
            Globals.SpirePdfLicense = "RFsfGwEApDiAA0LnWWrwVvR8Z6jdT4CVjXIIXCXLhrFKHramlfqRb9TKDiZWZxoFTObBb6VtT7iyZgTIf31PUup/MTpuJDYVZpTDIryXIldmnsHL0uO4autya6eVMe8CF2XzVWqZtltBo43Cdk+v2Hbc2L+RF5I+TUwYm5xwCwoyrVnwz92jguhRrXN1R53WLo5PDhUJ8XzQHIZMMDad8BIVyJxI1m6EYL5dTy/NAIiLEqA2ckKjmwQgqzxrGNF68p0GkEwhXRJ78nn8mINyAf381H+BlkT1D35RVJZbozobfNpaRG4hfb6GDNI+nZ9D6ektbCq1VbPlYEfpuHgLnG7c5yBPAqqK5lnD/nsjNgiHu7nGwe+ZeZxrPvFrsEi3epFkuhss4B6UJfC2erHN9cIGIQfmDyWkF2+NmiQiGJaFMugl2QDXMrtxYajK9UjQo2GKtm8XPtzR3yiVqjWjDz0E9ilP2MFHbZTPyxin7U8lsp9El3F+jE6kOOaeBIM5HwsCISLm0PDJKA+K0WKOtBLGjWpsaeJjjQZt+KvLIBOVU/3NCaF/uKaafC/GGxbX07QOYSLtD6Jd5+Kp7H5XxzOzUm/PSZNqZefaYwGbn43K0XdkczuClronYxrMLmcNYoMl0Ob9mk97in+3HFWzl06b+cpxo+4XUUvVqGigmKXSrzQublO6gpK1mdVLMO+FjVSqwj5y/eO53hZGY9z0P9SOK/5g4ufnXocXb1Pu6P9f1qP/FMPaCYB2B6QebS90mpoBvwKm3VPlMGAmFF9ZqlkJGm9J/ZGpHUWdfx40xww1Jz/qjim3rfmT855kU+bWfenY2B8bVaTz+2BPTBky4xZfI8VbygK9XI15A++ke+ERC+ciEUN5vCpljwvNTUQ7vW+eLxO1DZhcj9/O1af8fUagGonpIPz2FnBdC6A93U39dKZyV8LzzusSzcejt5R+LgUnMKzsT258qJkG0fwrwk6DeZ3MBAq29/qRaSAiKbHwTTnXUw2awlgtH1JnwWekQEDfQJzKexbZC0GZTvdSKlX0d6aAf4A8vMKo2++xuo8/JTR5hsGyte8UhxZiPNHNnZEQpji+UTrD/B9WuYHIRWlsarQ2p8jLYIi+PpjmrW6e9QthnzUUc7ingfjZn1rvFUFB53yQgiiVhVWKvkgJQTBfu8jwur/t4+BJIkfNgBeru0wQ37vVSUP34eTvsE7/cN/rR562HfB1kT+bzsa5Xs/KSrmGQp+1LrsadrhcUC4hdBBUcWiduU12qRJmzXGvzO8+fasRhoTXp2EDH4m6jtk8/dweXm70Lg/tq3iDf4N0bXUiYg3eNFsA450T5MfjXlKy82SiPZz4KpT2gfSEp8BtGWBtkgQmfIPaYfHyKZHgx7Vh1wypWW651f/a9xFYOj038eF5Dcx74BUT+7u5tklrd9Gk72PyCmCFi4SIXIsbXBaMeHC7UBG4OPCkTCKQEONP5tDcE/HKQ5rG1GTYBApmxrI7Nd4J";
            Spire.License.LicenseProvider.SetLicenseKey(Globals.SpirePdfLicense);

            if (args.Length > 0)
            {
                _processArguments(args);
            }

            //_debugMode();
            if (Globals.Printer.Equals(""))
            {
                _printerSelect();
            }

            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds((Globals.PingInterval * 60));

            var timer = new System.Threading.Timer((e) =>
            {
                Http.PingAlive();
            }, null, startTimeSpan, periodTimeSpan);

            _ngrok();
            _microserver();
            
        }

        private static void _debugMode()
        {
            Console.WriteLine("Debug Mode? y|N");
            string debug = (Console.ReadLine()).ToLower();
            if (!debug.Equals("y") && !debug.Equals("n"))
            {
                Console.WriteLine("Please answer \"y|Y\" for yes and \"n|N\" for No");
                _debugMode();
            }
            else
            {
                Globals.Mode = !debug.Equals("n") ? "Production" : "Staging";
            }
        }

        private static void _printerSelect()
        {
            Console.WriteLine(String.Format("Enter Printer Name: (Default: {0})", Globals.Printer));

            var printerOptions = new List<string>();
            int i = 1;
            foreach (String prtr in PrinterSettings.InstalledPrinters)
            {
                printerOptions.Add(prtr);
                Console.WriteLine(String.Format("[{0}]. {1}\n", i, prtr));
                i++;
            }

            int printer = Convert.ToInt32(Console.ReadLine());

            Globals.Printer = printerOptions[printer - 1];
        }

        private static void _ngrok()
        {
            Console.WriteLine("Starting MicroServer...");
            Process ngrok = new Process();


            string arguments = "http -subdomain \"" + Globals.Subdomain + "\" " + 8665;
            Console.WriteLine(arguments);
            ngrok.StartInfo.FileName = @"ngrok.exe";
            ngrok.StartInfo.CreateNoWindow = false;
            ngrok.StartInfo.RedirectStandardOutput = true;
            ngrok.StartInfo.RedirectStandardError = true;
            ngrok.StartInfo.UseShellExecute = false;
            ngrok.StartInfo.Arguments = arguments;
            ngrok.Start();
        }

        private static void _microserver()
        {
            Console.WriteLine("Ngrok is running...");
            using (WebApp.Start<Startup>(url: String.Format("http://{0}:{1}", Globals.Url, Globals.Port)))
            {
                Uri Address = new Uri(String.Format("http://{0}:{1}/api/", Globals.Url, Globals.Port));
                Console.WriteLine("Microserver located at " + Address);
                Console.WriteLine(String.Format("Tunnel located at {0}.ngrok.io", Globals.Subdomain));
                System.Timers.Timer timer = new System.Timers.Timer(30000);
                Console.ReadKey();
            }
        }

        private static void _processArguments(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-p":          // Printer Select
                    case "--printer":
                        i++;
                        if (args.Length <= i)
                        {
                            throw new ArgumentException(args[i]);
                        }
                        Globals.Printer = args[i];
                        break;
                    case "-i":  // int argument, allows hex or decimal
                    case "--interval":  // int argument, allows hex or decimal
                        i++;
                        if (args.Length <= i)
                        {
                            throw new ArgumentException(args[i]);
                        }
                        Globals.PingInterval = Convert.ToDouble(args[i]);
                        break;
                    default:
                        break;
                }
                Console.WriteLine(args[i]);
            }
        }
    }
}

