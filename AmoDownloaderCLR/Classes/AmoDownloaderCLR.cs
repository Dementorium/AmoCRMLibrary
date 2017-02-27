using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using AmoDownloaderCLR.Models;
using Newtonsoft.Json;
using System.Data.SqlTypes;

namespace AmoDownloaderCLR.Classes
{
    public static class IEnumerableExtensions
    {
        public static DataTable AsDataTable<T>(this IEnumerable<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
        public static IEnumerable<object> GetValues<T>(IEnumerable<T> items, string propertyName)
        {
            Type type = typeof(T);
            var prop = type.GetProperty(propertyName);
            foreach (var item in items)
                yield return prop.GetValue(item, null);
        }

    }


    public class AmoDownloaderCLR
    {
        public SqlString _accessToken;
        public bool HadErrors { get; private set; }
        
        /*Авторизация и получение токена для дальнейших запросов*/
        public /*Dictionary<string, string>*/ SqlString GetAccessToken(SqlString Host, SqlString ClientId, SqlString ClientSecret) 
        {
            var log = new ConsoleLogging();
            var url = Convert.ToString(Host + "/private/api/auth.php?type=json&USER_LOGIN=" + ClientId + "&USER_HASH=" + ClientSecret);
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "POST";
            if (request.CookieContainer == null)
                request.CookieContainer = new CookieContainer();
            request.AllowAutoRedirect = false;

            log.WriteInfo("Try to get response");
            var response = (HttpWebResponse) request.GetResponse();
            var resStream = response.GetResponseStream();
            var reader = new StreamReader(resStream);
            var json = reader.ReadToEnd();
            var dict = JsonConvert.DeserializeObject<Auth>(json);
            Dictionary<string, string> ress = new Dictionary<string, string>();
            if (dict.response.auth)
            {
                log.WriteInfo("Set cookies in var");
                foreach (Cookie cookieValue in response.Cookies)
                {
                    log.WriteInfo(cookieValue.ToString());
                    ress.Add(cookieValue.Name, cookieValue.Value);
                }                
                /*Иногда куки могут быть только в request.CookieContainer)*/
            }
            _accessToken = response.Cookies["session_id"].Value;
            return _accessToken;
        }


        public string SendQuery(SqlString Host, SqlString sessionId, Task newTask)
        {
            var log = new ConsoleLogging();
            bool reRunFlag = false;
            var responseText = "";

            CookieCollection cookies = new CookieCollection();
            cookies.Add(new Cookie(name: "session_id", value: Convert.ToString(sessionId), path: "/", domain: ".amocrm.ru"));
            cookies.Add(new Cookie(name: "BITRIX_SM_LOGIN", value: "amotime%40yandex.ru", path: "/", domain: ".amocrm.ru"));
            cookies.Add(new Cookie(name: "BITRIX_SM_SALE_UID", value: "0", path: "/", domain: ".amocrm.ru"));
            cookies.Add(new Cookie(name: "user_lang", value: "ru", path: "/", domain: ".amocrm.ru"));

            var url = Convert.ToString(Host + "/private/api/v2/json/tasks/set");

            string json = JsonConvert.SerializeObject(newTask);

            //var query = new QueryDuration();
            var body = Encoding.UTF8.GetBytes(json);
            var charArray = Encoding.UTF8.GetString(body).ToCharArray();
            do
            {
                if (reRunFlag)
                {
                    log.WriteInfo("Retry add new or update task..");
                    //Console.WriteLine("Retry DeleteMarketoData..");
                    reRunFlag = false;
                }

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = @"application/json";
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
                request.AllowAutoRedirect = false;
                //request.Accept = "application/json";
                request.ContentLength = charArray.Length;

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(charArray, 0, charArray.Length);
                    streamWriter.Close();
                }

                try
                {
                    var response = (HttpWebResponse)request.GetResponse();

                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseText = reader.ReadToEnd();
                        response.Close();
                    }
                    reRunFlag = false;
                }
                catch (Exception e)
                {
                    var error = "Add new or update task error. " + e.HResult + ": " + e.Message;
                    log.WriteInfo(error);
                    reRunFlag = true;
                    //Thread.Sleep(13000);
                    throw;
                }
            } while (reRunFlag);

            return responseText;
            //return JsonConvert.DeserializeObject<TaskResponseRoot>(responseText) ;
        }

    }
}
