using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmoCRMDownloader.Classes
{
    class ConsoleLogging
    {
        public DateTime StartTime;
        public DateTime StopTime;
        public int EventID;

        public ConsoleLogging(string mes, string EventName)
        {
            //var eventID = new Object();
            StartTime = DateTime.Now;
            if (mes.Length > 0)
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + ". " + mes + " . . .\n");
            /*if (EventName.Length > 0) //TODO
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.ProcessEventLog_BeginWrite", connection))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // set up the parameters
                            cmd.Parameters.Add("@EventName", SqlDbType.VarChar, 100);
                            cmd.Parameters.Add("@ProcessEventId", SqlDbType.Int).Direction = ParameterDirection.Output;

                            // set parameter values
                            cmd.Parameters["@EventName"].Value = mes;

                            // open connection and execute stored procedure
                            connection.Open();
                            cmd.ExecuteNonQuery();

                            // read output value from @NewId
                            EventID = Convert.ToInt32(cmd.Parameters["@ProcessEventId"].Value);
                            connection.Close();

                            //SqlCommand cmd2 = new SqlCommand("EXEC dbo.ProcessEventLog_BeginWrite @EventName = '" + mes + "' ", connection);
                            //eventID = cmd2.ExecuteScalar();
                        }
                        catch (Exception e)
                        {
                            connection.Close();
                            Console.WriteLine("ConsoleLogging. {0}: {1}", e.HResult.ToString(), e.Message);
                        }
                        finally
                        {
                            connection.Close();
                        }
                        //EventID = Convert.ToInt32(eventID);
                    }
                }
            }*/
        }

        public ConsoleLogging(string mes)
        {
            StartTime = DateTime.Now;
            if (mes.Length > 0)
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + mes + " . . .\n");
        }

        public ConsoleLogging()
        {

        }

        public void WriteInfo(string info, bool ToDB = false)
        {
            Console.WriteLine(info);
            /*if (ToDB) //TODO
            {
                //var EventID = 0;
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.ProcessEventLog_BeginWrite", connection))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // set up the parameters
                            cmd.Parameters.Add("@EventName", SqlDbType.VarChar, 100);
                            cmd.Parameters.Add("@ProcessEventId", SqlDbType.Int).Direction = ParameterDirection.Output;

                            cmd.Parameters["@EventName"].Value = info;

                            connection.Open();
                            cmd.ExecuteNonQuery();

                            EventID = Convert.ToInt32(cmd.Parameters["@ProcessEventId"].Value);
                            connection.Close();

                        }
                        catch (Exception e)
                        {
                            connection.Close();
                            Console.WriteLine("WriteInfo. {0}: {1}", e.HResult.ToString(), e.Message);
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                    using (SqlCommand cmd = new SqlCommand("dbo.ProcessEventLog_EndWrite", connection))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add("@ProcessEventId", SqlDbType.Int);

                            cmd.Parameters["@ProcessEventId"].Value = EventID;

                            connection.Open();
                            cmd.ExecuteNonQuery();

                            connection.Close();
                        }
                        catch (Exception e)
                        {
                            connection.Close();
                            Console.WriteLine("WriteInfo. {0}: {1}", e.HResult.ToString(), e.Message);
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
            }*/
        }

        public void WriteError(string info)
        {
            Console.WriteLine(info);
            //var EventID = 0;
            /*using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString))
            //TODO
            {
                using (SqlCommand cmd = new SqlCommand("dbo.ProcessEventLog_BeginWrite", connection))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // set up the parameters
                        cmd.Parameters.Add("@EventName", SqlDbType.VarChar, 100);
                        cmd.Parameters.Add("@ProcessEventId", SqlDbType.Int).Direction = ParameterDirection.Output;

                        cmd.Parameters["@EventName"].Value = info;

                        connection.Open();
                        cmd.ExecuteNonQuery();

                        EventID = Convert.ToInt32(cmd.Parameters["@ProcessEventId"].Value);
                        connection.Close();

                    }
                    catch (Exception e)
                    {
                        connection.Close();
                        Console.WriteLine("WriteInfo. {0}: {1}", e.HResult.ToString(), e.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                using (SqlCommand cmd = new SqlCommand("dbo.ProcessEventLog_EndWrite", connection))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@ProcessEventId", SqlDbType.Int);

                        cmd.Parameters["@ProcessEventId"].Value = EventID;

                        connection.Open();
                        cmd.ExecuteNonQuery();

                        connection.Close();
                    }
                    catch (Exception e)
                    {
                        connection.Close();
                        Console.WriteLine("WriteInfo. {0}: {1}", e.HResult.ToString(), e.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }*/
        }

        public void WriteDuration(string message, string InsertedRows = null)
        {
            StopTime = DateTime.Now;
            Console.WriteLine("\t" + message);
            Console.WriteLine("\tStart at " + StartTime.ToString("HH:mm:ss"));
            Console.WriteLine("\tStop at " + StopTime.ToString("HH:mm:ss"));
            if (InsertedRows != null)
                Console.WriteLine("\tInsert {0} row(s) ", InsertedRows);
            Console.WriteLine("\tDuration "
                              + "" + String.Format("{0:00}", Convert.ToInt16((StopTime - StartTime).Hours))
                              + ":" + String.Format("{0:00}", Convert.ToInt16((StopTime - StartTime).Minutes))
                              + ":" + String.Format("{0:00}", Convert.ToInt16((StopTime - StartTime).Seconds))
                              + "." + String.Format("{0:00}", Convert.ToInt16((StopTime - StartTime).Milliseconds)));
            /*if (EventID > 0) //TODO
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString))
                {
                    connection.Open();
                    try
                    {
                        SqlCommand cmd = new SqlCommand("EXEC dbo.ProcessEventLog_SetDetail @EventId = " + EventID + ", @InsertRows = " + InsertedRows, connection);
                        cmd.ExecuteScalar();
                        SqlCommand cmd2 = new SqlCommand("EXEC dbo.[ProcessEventLog_EndWrite] " + EventID, connection);
                        cmd2.ExecuteScalar();
                    }
                    catch (Exception e)
                    {
                        connection.Close();
                        Console.WriteLine("Logger.WriteDuration. {0}: {1}", e.HResult.ToString(), e.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }*/
        }
    }
    }
}
