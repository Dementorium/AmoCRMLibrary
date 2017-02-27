using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmoDownloaderCLR;
using Newtonsoft.Json;
using System.Data.SqlTypes;

namespace AmoDownloaderCLR_TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //SqlString sessionKey;
            SqlString Host = "https://cherkasov.amocrm.ru";
            SqlString ClientId = "amotime@yandex.ru";
            SqlString ClientSecret = "e931184c08f68070e52701f735b58b63";
            //SqlString newIds;
            SqlInt32 ParentId = Convert.ToInt32(19456016);
            SqlInt32 TillTime = Convert.ToInt32(235900);

            var session = Funcs.GetAccessToken(Host, ClientId, ClientSecret);
            //var sessionKey = session["Se"];

            var newIds = Funcs.AddNewTask(Host, session, ParentId, 1, 1, 0, "This is test task for contact 19456016", TillTime);
            SqlInt32 newId2 = Convert.ToInt32(10428530);
            SqlInt32 lastModifyDate = Convert.ToInt32(1488054561);

            DateTime baseDate = new DateTime(1970, 1, 1);
            var diff = (DateTime.Now - baseDate).TotalMilliseconds + 2;

            var updatedIds = Funcs.UpdateTask(Host, session,
                newId2, null, ParentId, 1, 2, lastModifyDate, 
                3, "This is new task text", (int)diff);
            Console.WriteLine("End program!");
        }
    }
}
