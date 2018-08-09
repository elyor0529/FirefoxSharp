using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Gecko;
using Gecko.Events;

namespace ConsoleApp1
{
    internal class Program
    {
        private static readonly Stopwatch Elapsed = new Stopwatch();

        [STAThread]
        private static void Main(string[] args)
        {
            //timer
            Elapsed.Start();

            //domain
            AppDomain.CurrentDomain.UnhandledException += DomainOnUnhandledException;
            AppDomain.CurrentDomain.ProcessExit += DomainOnProcessExit;
            AppDomain.CurrentDomain.DomainUnload += DomainOnDomainUnload;
             
            //form
            Xpcom.Initialize("Firefox");
            var browser = new GeckoWebBrowser();
            browser.DocumentCompleted += Browser_DocumentCompleted;
            var mainFrm = new Form
            {
                ShowInTaskbar = false,
                WindowState = FormWindowState.Minimized,
                Visible = false
            };
            mainFrm.Controls.Add(browser);
            browser.Navigate("https://www.autoscout24.de/lst?sort=standard&desc=0&ustate=N%2CU&cy=D&atype=C");
            mainFrm.Hide();
            Application.Run(mainFrm);
        }

        private static void DomainOnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            var exception = (Exception)args.ExceptionObject;

            if (exception == null)
                return;

            Console.WriteLine(exception.Message);

            Environment.Exit(Environment.ExitCode);
        }

        private static void DomainOnDomainUnload(object sender, EventArgs args)
        {
            Environment.Exit(Environment.ExitCode);
        }

        private static void DomainOnProcessExit(object sender, EventArgs args)
        {
            //elapsed 
            Elapsed.Stop();
            Console.WriteLine("Elapsed {0:g}", Elapsed.Elapsed); 
        }
         
        private static void Browser_DocumentCompleted(object sender, GeckoDocumentCompletedEventArgs e)
        {
            var geckoDomElement = e.Window.Document.DocumentElement;

            if (geckoDomElement is GeckoHtmlElement element)
            {
                var innerHtml = element.InnerHtml;

                File.WriteAllText("C:\\tmp\\list-fx.html", innerHtml);

                Environment.Exit(Environment.ExitCode);
            }
        }

    }
}