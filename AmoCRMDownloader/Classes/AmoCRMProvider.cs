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
using System.Threading.Tasks;
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
        private string _accessToken;
        private CookieCollection cookies;
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
                cookies = response.Cookies;
                foreach (Cookie cookieValue in response.Cookies)
                {
                    if (cookieValue.ToString().Split('=')[0] == "session_id")
                    {
                        _accessToken = cookieValue.ToString().Split('=')[1];
                        log.WriteInfo("Found session id in cookies");
                    }
                }
                /*Иногда куки могут быть только в request.CookieContainer)*/
            }
            return _accessToken;
        }

        /*Создаем новую таску с параметрами*/
        public void NewTask()
        {
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
