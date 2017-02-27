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
using AmoCRMDownloader.Models;
using Newtonsoft.Json;

namespace AmoCRMDownloader.Classes
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
    }


    public class AmoCRMProvider
    {
        public string _accessToken;
        //private CookieCollection cookies;
        private bool _criticalError;
        public bool HadErrors { get; private set; }
        
        /*Авторизация и получение токена для дальнейших запросов*/
        public string GetAccessToken() 
        {
            var log = new ConsoleLogging();
            var url = Program.Host + "/private/api/auth.php?type=json&USER_LOGIN=" + Program.ClientId + "&USER_HASH=" + Program.ClientSecret;
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
            if (dict.response.auth)
            {
                log.WriteInfo("Get success auth");
                foreach (Cookie cookieValue in response.Cookies)
                {
                    log.WriteInfo(cookieValue.ToString());
                }
                _accessToken = response.Cookies["session_id"].Value;
                /*Иногда куки могут быть только в request.CookieContainer)*/
            }
            return _accessToken;
        }


        public TaskResponse SendNewTask(string sessionId, Task newTask)
        {
            var log = new ConsoleLogging();
            bool reRunFlag = false;
            var responseText = "";

            CookieCollection cookies = new CookieCollection();
            cookies.Add(new Cookie(name: "session_id", value: sessionId, path: "/", domain: ".amocrm.ru"));
            cookies.Add(new Cookie(name: "BITRIX_SM_LOGIN", value: "amotime%40yandex.ru", path: "/", domain: ".amocrm.ru"));
            cookies.Add(new Cookie(name: "BITRIX_SM_SALE_UID", value: "0", path: "/", domain: ".amocrm.ru"));
            cookies.Add(new Cookie(name: "user_lang", value: "ru", path: "/", domain: ".amocrm.ru"));

            var url = Program.Host + "/private/api/v2/json/tasks/set";

            string json = JsonConvert.SerializeObject(newTask);

            //var query = new QueryDuration();
            var body = Encoding.UTF8.GetBytes(json);
            var charArray = Encoding.UTF8.GetString(body).ToCharArray();
            do
            {
                if (reRunFlag)
                {
                    log.WriteInfo("Retry add new task..");
                    //Console.WriteLine("Retry DeleteMarketoData..");
                    reRunFlag = false;
                }

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
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
                    var error = "Add new task. " + e.HResult + ": " + e.Message;
                    log.WriteInfo(error);
                    reRunFlag = true;
                    Thread.Sleep(2000);
                }
            } while (reRunFlag);

            return JsonConvert.DeserializeObject<TaskResponse>(responseText);
        }


        /*Создаем новую таску с параметрами*/
        public string[] NewTask(string sessionId)
        {
            string[] ids = new []{""}; 
                //TODO
            CookieCollection cookies = new CookieCollection();
            cookies.Add(new Cookie(name: "session_id", value: sessionId, path: "/", domain: ".amocrm.ru"));
            cookies.Add(new Cookie(name: "BITRIX_SM_LOGIN", value: "amotime%40yandex.ru", path: "/", domain: ".amocrm.ru"));
            cookies.Add(new Cookie(name: "BITRIX_SM_SALE_UID", value: "0", path: "/", domain: ".amocrm.ru"));
            cookies.Add(new Cookie(name: "user_lang", value: "ru", path: "/", domain: ".amocrm.ru"));

            var log = new ConsoleLogging();
            var url = Program.Host + "/private/api/v2/json/tasks/set";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(cookies);
            request.AllowAutoRedirect = false;

            log.WriteInfo("Try to create new task");
            var response = (HttpWebResponse)request.GetResponse();
            var resStream = response.GetResponseStream();
            var reader = new StreamReader(resStream);
            var json = reader.ReadToEnd();
            var dict = JsonConvert.DeserializeObject<Auth>(json);
            if (dict.response.auth)
            {

            }
            return ids; 
                //TODO
        }

        private void InsertIntoDB(DataTable dtToIns, string tableName)
        {
            var log = new ConsoleLogging();
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString))
            {
                connection.Open();
                //SqlTransaction transaction = connection.BeginTransaction();

                using (var bulkCopy = new SqlBulkCopy(connection /*, SqlBulkCopyOptions.Default, transaction*/))
                {
                    bulkCopy.DestinationTableName = tableName;
                    try
                    {
                        bulkCopy.WriteToServer(dtToIns);
                    }
                    catch (Exception e)
                    {
                        //transaction.Rollback();
                        connection.Close();
                        log.WriteInfo("InsertIntoDB. " + e.HResult + ": " + e.Message + "; " + e.InnerException);
                        InsertErrorIntoDB(e.HResult.ToString(), "InsertIntoDB. " + e.Message);
                        HadErrors = true;
                    }
                }
                //transaction.Commit();
            }
        }

        private void InsertErrorIntoDB(string ErrorCode, string ErrorMessage)
        {
            var log = new ConsoleLogging();

            if (ErrorCode != "-2146233079" /*Время ожидания операции истекло*/
                    && ErrorCode != "-2146232800" /*Не удается прочитать данные из транспортного соединения: Удаленный хост принудительно разорвал существующее подключение.*/)
                //TODO
                /*using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.AddNewError", connection))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // set up the parameters
                            cmd.Parameters.Add("@ErrorCode", SqlDbType.VarChar, 50);
                            cmd.Parameters.Add("@ErrorMessage", SqlDbType.VarChar, 250);

                            // set parameter values
                            cmd.Parameters["@ErrorCode"].Value = ErrorCode;
                            cmd.Parameters["@ErrorMessage"].Value = ErrorMessage;

                            // open connection and execute stored procedure
                            connection.Open();
                            cmd.ExecuteNonQuery();

                            connection.Close();
                        }
                        catch (Exception e)
                        {
                            connection.Close();
                            log.WriteInfo("InsertErrorIntoDB. " + e.HResult + ":" + e.Message);
                        }
                        finally
                        {
                            connection.Close();
                        }
                        //EventID = Convert.ToInt32(eventID);
                    }
                    //transaction.Commit();
                }*/

            if (!Directory.Exists(@"C:\Services\MarketoDownloader\ErrorLogs\"))
            {
                Directory.CreateDirectory(@"C:\Services\MarketoDownloader\ErrorLogs\");
            }
            using (StreamWriter file = new StreamWriter(@"C:\Services\MarketoDownloader\ErrorLogs\" + DateTime.Now.ToString("yyyy-MM-dd") + "_Errors.txt", true))
            {
                file.WriteLine(DateTime.Now.ToString("hh:mm:ss") + ". " + ErrorCode + ":" + ErrorMessage);
            }
        }

    }
}
