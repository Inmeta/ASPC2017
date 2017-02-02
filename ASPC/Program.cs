using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASPC
{
    class Program
    {
        static void Main(string[] args)
        {
            //Getting first parameter / url
            string url = args[0];

            //Checking url parameter
            if (args.Length != 4 || !CheckURLValid(url))
            {
                Console.WriteLine("Invalid arguments. 4 arguments required and valid url!");
                return;
            }

            //Getting parameter values
            string username = args[1];
            string pwd = args[2];
            string env = args[3];

            //Running provisioning
            Provisioning.Instance.Provision(url, username, pwd, env);
        }

        /*
         * Method for checking url
         */
        private static bool CheckURLValid(string source)
        {
            Uri uriResult;
            return Uri.TryCreate(source, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
