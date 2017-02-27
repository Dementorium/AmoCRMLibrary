using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using AmoCRMDownloader.Classes;
using AmoCRMDownloader.Models;
using Newtonsoft.Json;

namespace AmoCRMDownloader
{
    class Program
    {
        public static string Host = ConfigurationManager.AppSettings["Host"];
        public static string ClientId = ConfigurationManager.AppSettings["ClientId"];
        public static string ClientSecret = ConfigurationManager.AppSettings["ClientSecret"];
        public static string MonitoringMail = ConfigurationManager.AppSettings["MonitoringMail"];
        public static string DebugMail = ConfigurationManager.AppSettings["DebugMail"];
        public static string SessionID;
        public static bool DebugMode;

        /*private static readonly Dictionary<string, Action> _commands = new Dictionary<string, Action>()
        {
            //{"/a", GetAccessToken},
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
        }*/

        public static string GetAccessToken()
        {
            var provider = new AmoCRMProvider();
            Console.WriteLine("Start GetAccessToken\n");
            SessionID = provider.GetAccessToken();
            Environment.ExitCode = Convert.ToInt16(provider.HadErrors);
            return SessionID;
        }

        public static List<TaskAdd> AddNewTask(int elementid, int elementtype, int tasktype, int taskstatus, string tasktext, int tilltime, int? datecreate = null, int? lastmodified = null, int? responsibleuserid = null)
        {
            var provider = new AmoCRMProvider();
            Task newTask = new Task();
            newTask.request = new Request();
            newTask.request.tasks = new Tasks();
            newTask.request.tasks.add = new List<TaskAdd>();

            TaskAdd newTaskItem = new TaskAdd();
            newTaskItem.element_id = elementid;
            newTaskItem.element_type = elementtype;
            newTaskItem.task_type = tasktype;
            newTaskItem.status = taskstatus;
            newTaskItem.text = tasktext;
            newTaskItem.complete_till = tilltime;
            newTaskItem.date_create = datecreate;
            newTaskItem.last_modified = lastmodified;
            //newTaskItem.request_id = //этож output-параметр... зачем он тут? хз 
            newTaskItem.responsible_user_id = responsibleuserid;

            newTask.request.tasks.add.Add(newTaskItem);

            Console.WriteLine("Start AddNewTask\n");
            Console.WriteLine(JsonConvert.SerializeObject(newTask));
            TaskResponse ids = provider.SendNewTask(SessionID, newTask);
            Environment.ExitCode = Convert.ToInt16(provider.HadErrors);
            return ids.tasks.add;
            //todo вернуть текущее время и время сервера. Время сервера есть в ответе Auth. А вот текущее правильно ли будет брать сразу после? не совсем, наверное, все же..
        }

        static void Main(string[] args)
        {
            string sessionKey;
            List<TaskAdd> newIds;

            sessionKey = GetAccessToken();
            newIds = AddNewTask(19456016, 2, 1, 1, "This is test task for contact 19456016", 1375112222);
            Console.ReadKey();
        }
    }
}
