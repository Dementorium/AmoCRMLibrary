using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmoCRMDownloader.Classes;

namespace AmoCRMDownloader
{
    class Program
    {
        public static string Host = ConfigurationManager.AppSettings["Host"];
        public static string ClientId = ConfigurationManager.AppSettings["ClientId"];
        public static string ClientSecret = ConfigurationManager.AppSettings["ClientSecret"];
        //public static string ListId = ConfigurationManager.AppSettings["ListID"];
        public static string MonitoringMail = ConfigurationManager.AppSettings["MonitoringMail"];
        public static string DebugMail = ConfigurationManager.AppSettings["DebugMail"];
        public static bool DebugMode;

        private static readonly Dictionary<string, Action> _commands = new Dictionary<string, Action>()
        {
            //{"/Load", LoadAll},
            //{"/Merge", MergeNewData},
            //{"/Clear", ClearList},
            //{"/Debug", SetDebugMode},
            //{"/CheckErrors", CheckErrors},

            {"/Auth", GetAccessToken},
            {"/?", ListOfAttributes}
        };

        private static void ProcessKeys(string[] args)
        {
            var actions = (from x in args.AsEnumerable()
                           join y in _commands
                               on x.Trim() equals y.Key
                           select y.Value
            );

            foreach (var action in actions)
            {
                action();
            }
        }

        public static void ListOfAttributes()
        {
            string cmds = "";
            foreach (var command in _commands)
            {
                if (command.Key != "/Debug")
                    cmds = cmds + command.Key + '\n';
            }
            Console.WriteLine(cmds);
        }

        public static void GetAccessToken()
        {
            var provider = new AmoCRMProvider();
            Console.WriteLine("Start GetAccessToken\n");
            var authCookie = provider.GetAccessToken();
            //provider.UpdateErrorState(provider.HadErrors);
            Environment.ExitCode = Convert.ToInt16(provider.HadErrors);
            //provider.UpdateAPICallsCount();
        }

        static void Main(string[] args)
        {
            try
            {
                ProcessKeys(args);
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured. {0}: {1}", e.HResult.ToString(), e.Message);
            }
            GetAccessToken();
        }
    }
}
