using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using AmoDownloaderCLR.Classes;
using AmoDownloaderCLR.Models;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System.Collections;

namespace AmoDownloaderCLR
{
    public class Funcs
    {
        public struct Auth
        {
            public string SessionId;
        }

        public struct TaskResponse
        {
            public int Id;
            public int AmoTime;
            public DateTime ProgramTime;
        }

        public static bool DebugMode;

        [SqlFunction(FillRowMethodName = "GetAccessToken", TableDefinition = "[SessionId] NVARCHAR(50)")]
        public static SqlString GetAccessToken(SqlString Host, SqlString ClientId, SqlString ClientSecret)
        {
            var provider = new Classes.AmoDownloaderCLR();
            Console.WriteLine("Start GetAccessToken\n");
            var cookies = provider.GetAccessToken(Host, ClientId, ClientSecret);
            Environment.ExitCode = Convert.ToInt16(provider.HadErrors);
            /*Auth newRec = new Auth();
            newRec.SessionId = cookies["session_id"];
            ArrayList list = new ArrayList();
            list.Add(newRec);
            return list;*/
            return cookies;
        }

        [SqlFunction(FillRowMethodName = "AddNewTask", TableDefinition = "[Id] int, [AmoTime] int, [ServerTime] int")]
        public static IEnumerable AddNewTask(SqlString Host, SqlString SessionID, SqlInt32 elementid, SqlInt16 elementtype, SqlInt16 tasktype, SqlInt16 taskstatus, 
                SqlString tasktext, SqlInt32 tilltime, SqlInt16? datecreate = null, SqlInt16? lastmodified = null, SqlInt16? responsibleuserid = null)
        {
            var provider = new Classes.AmoDownloaderCLR();
            Task newTask = new Task();
            newTask.request = new Request();
            newTask.request.tasks = new Tasks();
            newTask.request.tasks.add = new List<TaskAdd>();

            TaskAdd newTaskItem = new TaskAdd();
            newTaskItem.element_id = (Int32)elementid;
            newTaskItem.element_type = (int)elementtype;
            newTaskItem.task_type = (int)tasktype;
            newTaskItem.status = (int)taskstatus;
            newTaskItem.text = (string)tasktext;
            newTaskItem.complete_till = (Int32)tilltime;
            newTaskItem.date_create = (int?)datecreate;
            newTaskItem.last_modified = (int?)lastmodified;
            newTaskItem.responsible_user_id = (int?)responsibleuserid;

            newTask.request.tasks.add.Add(newTaskItem);

            Console.WriteLine("Start AddNewTask\n");
            Console.WriteLine(JsonConvert.SerializeObject(newTask));
            var ids = provider.SendQuery(Host, SessionID, newTask);
            Environment.ExitCode = Convert.ToInt16(provider.HadErrors);

            var i = JsonConvert.DeserializeObject<TaskResponseRoot>(ids);
            ArrayList list = new ArrayList();

            foreach (var newTaskResponse in i.response.tasks.add)
            {
                TaskResponse newRec = new TaskResponse();
                newRec.Id = newTaskResponse.id;
                newRec.AmoTime = i.response.Amo_time;
                newRec.ProgramTime = DateTime.Now;
                list.Add(newRec);
            }
            return list;

        }

        [SqlFunction(FillRowMethodName = "UpdateTask", TableDefinition = "[Id] int, [AmoTime] int, [ServerTime] int")]
        public static IEnumerable UpdateTask(SqlString Host, SqlString SessionID,
            SqlInt32 id, SqlInt32? request_id, SqlInt32 element_id, SqlInt16 element_type,
            SqlInt16 status, SqlInt32 last_modified, SqlInt16 task_type, SqlString text, SqlInt32 complete_till
            )
        {
            var provider = new Classes.AmoDownloaderCLR();
            Task newTask = new Task();
            newTask.request = new Request();
            newTask.request.tasks = new Tasks();
            newTask.request.tasks.update = new List<TaskUpdate>();

            TaskUpdate UpdateTask = new TaskUpdate();
            UpdateTask.id = (int)id;
            UpdateTask.request_id = (int?)request_id;
            UpdateTask.element_id = (Int32)element_id;
            UpdateTask.element_type = (int)element_type;
            UpdateTask.last_modified = (int)last_modified;
            UpdateTask.task_type = (int)task_type;
            UpdateTask.text = (string)text;
            UpdateTask.status = (int)status;
            UpdateTask.complete_till = (Int32)complete_till;

            newTask.request.tasks.update.Add(UpdateTask);

            Console.WriteLine("Start UpdateTask\n");
            Console.WriteLine(JsonConvert.SerializeObject(newTask));
            var ids = provider.SendQuery(Host, SessionID, newTask);
            Environment.ExitCode = Convert.ToInt16(provider.HadErrors);

            var i = JsonConvert.DeserializeObject<TaskResponseRoot>(ids);
            ArrayList list = new ArrayList();

            foreach (var newTaskResponse in i.response.tasks.update)
            {
                TaskResponse newRec = new TaskResponse();
                newRec.Id = newTaskResponse.id;
                newRec.AmoTime = i.response.Amo_time;
                newRec.ProgramTime = DateTime.Now;
                list.Add(newRec);
            }
            return list;

        }

        /*static void Main(SqlString[] args)
        {
            SqlString sessionKey;
            SqlString Host = "https://cherkasov.amocrm.ru";
            SqlString ClientId = "amotime@yandex.ru";
            SqlString ClientSecret = "e931184c08f68070e52701f735b58b63";
            SqlString newIds;
            SqlInt16 parentId = Convert.ToInt16(19456016);

            sessionKey = GetAccessToken(Host, ClientId, ClientSecret);
            newIds = AddNewTask(Host, sessionKey, parentId, 2, 1, 1, "This is test task for contact 19456016", 2000);
            Console.ReadKey();
        }*/
    }
}
