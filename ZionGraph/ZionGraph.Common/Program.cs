#region

using System;
using System.Collections.Generic;
using System.Net;

#endregion

namespace ZionGraph.Common
{
    public class Program
    {

        // Single-Threaded Apartment required for OAuth2 Authz Code flow (User Authn) to execute for this demo app
        [STAThread]
        private static void Main()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            //Console.WriteLine("Run operations for signed-in user, or in app-only mode.\n");
            //Console.WriteLine("[a] - app-only\n[u] - as user\n[b] - both as user first, and then as app.\nPlease enter your choice:\n");

            //ConsoleKeyInfo key = Console.ReadKey();
            Requests.AppMode().Wait();
           

            Console.WriteLine("\nCompleted at {0} \n Press Any Key to Exit.", DateTime.Now.ToUniversalTime());
            Console.ReadKey();
        }

        public static string ExtractErrorMessage(Exception exception)
        {
            List<string> errorMessages = new List<string>();
            string tabs = "\n";
            while (exception != null)
            {
                tabs += "    ";
                errorMessages.Add(tabs + exception.Message);
                exception = exception.InnerException;
            }
            return string.Join("-\n", errorMessages);
        }

        public static void WriteError(string output, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(output, args);
            Console.ResetColor();
        }
      
    }
}
