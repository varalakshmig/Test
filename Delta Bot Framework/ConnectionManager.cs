using SampleBotTemplate.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Collections;
using System.Text;

namespace SampleBotTemplate
{
    public static class ConnectionManager
    {
        /// <summary>
        /// Connecting Azure Sql Database By Using ADO.Net Connection String
        /// </summary>
        /// <returns></returns>

        public static string Json;


        public static SqlConnection GetSqlConnection()
        {
            //Dev
            string ADOConnectionstring = "Server=tcp:dlbotdbs1.database.windows.net,1433;Initial Catalog=DLBotDB;Persist Security Info=False;User ID=DlBotdbs1;Password=delta@1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            //PROD
            //string ADOConnectionstring = "Server=tcp:dlbotdbs1.database.windows.net,1433;Initial Catalog=Lpsmldb;Persist Security Info=False;User ID=DlBotdbs1;Password=delta@1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            var connection = new SqlConnection(ADOConnectionstring);
            connection.Open();
            return connection;
        }


        /// <summary>
        /// /Connecting Azure Sql Database By Using OleDb Connection String
        /// </summary>
        /// <returns></returns>
        public static OleDbConnection GetOleDbConnection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["OleDbConnectionString"].ConnectionString;
            var connection = new OleDbConnection(connectionString);
            connection.Open();
            return connection;
        }

        public static string GetData(string BotName)
        {

            try
            {
                string query = "Select Json from GetJsonObjectTable where BotName = '" + BotName + "'";

                using (var connection = GetSqlConnection())
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    Data dt = new Data();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        dt.JsonData = reader["Json"].ToString();
                    }
                    Json = dt.JsonData;

                }
            }
            catch (Exception ex)
            {

            }
            return Json;
        }
        public static string GetlangCode(string language)
        {
            string code = string.Empty;
            try
            {
                string query = "Select code from languageCode where language = '" + language + "'";

                using (var connection = GetSqlConnection())
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        code = reader["code"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return code;
        }

        public static string getLangCode(CustomerResponse cust)
        {
            string language = string.Empty;
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("getLanguage", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@bot_nm", SqlDbType.VarChar).Value = cust.botName;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                cust.languageSelected = reader["lang_selected"].ToString().Trim();
                                language = reader["language"].ToString().Trim();
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
            return language;
        }

        public static string getResponseContextForBot(string UserId)
        {
            string respcnt = string.Empty;
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("GetResponseContextForBot", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserId", SqlDbType.VarChar).Value = UserId;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                respcnt = reader["ResponseContext"].ToString().Trim();
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
            return respcnt;
        }

        public static string GetResponseForRepeatComponent(string UserId, string componentInput)
        {
            string respcnt = string.Empty;
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("GetResponseForRepeatComponent", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserId", SqlDbType.VarChar).Value = UserId;
                    cmd.Parameters.Add("@componentInput", SqlDbType.VarChar).Value = componentInput;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                respcnt = reader["txn_txt"].ToString().Trim();
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
            return respcnt;
        }
        public static Customer GetCustomerData(string userId, Customer custInfo)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("GetCustomerDataForFlight", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserId", SqlDbType.VarChar).Value = userId;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        if (reader.HasRows)
                        {
                            custInfo.FirstName = reader["first_nm"].ToString().Trim();
                            custInfo.LastName = reader["last_nm"].ToString().Trim();
                            custInfo.PNR = reader["pnr"].ToString().Trim();
                            custInfo.bagTagNumber = reader["bagtagnumber"].ToString().Trim();
                        }
                    }
                    connection.Close();
                }

            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
            return custInfo;
        }
        public static CustomerResponse GetLastConversationDetails(string UserId, CustomerResponse objCustomerResponse)
        {
            string respcnt = string.Empty;
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("GetLastConversationDetails", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserId", SqlDbType.VarChar).Value = UserId;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                objCustomerResponse.responseContext = reader["ResponseContext"].ToString().Trim();
                                objCustomerResponse.ResponseType = reader["response_type"].ToString().Trim();
                                objCustomerResponse.respTxt = reader["response"].ToString().Trim();
                                objCustomerResponse.ResponseAction = "";
                                objCustomerResponse.IncomingContext = reader["context"].ToString().Trim();
                                objCustomerResponse.attribute = reader["attribute"].ToString().Trim();
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
            return objCustomerResponse;
        }
        public static string GetFlightNo(string UserId, BookingStatus bookingStatus)
        {
            string flightNo = string.Empty;
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("GetFlightNo", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserId", SqlDbType.VarChar).Value = UserId;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                flightNo = reader["FlightNo"].ToString().Trim();
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
            return flightNo;

        }
        public static string GetSelectedFlight(string UserId)
        {
            string SelectedflightNo = string.Empty;
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("GetSelectedFlight", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserId", SqlDbType.VarChar).Value = UserId;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                SelectedflightNo = reader["SelectedFlightNo"].ToString().Trim();
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
            return SelectedflightNo;
        }
        public static string getComponentType(string ComponentName)
        {
            string componentType = string.Empty;
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("GetComponentType", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@comp_nm", SqlDbType.VarChar).Value = ComponentName;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                componentType = reader["comp_typ"].ToString().Trim();
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
            return componentType;
        }

        public static void DeleteResponseContextForBot(string userId)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("DeleteResponseContextForBot", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserId", SqlDbType.VarChar).Value = userId;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                        connection.Close();
                }

            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
        }
        public static void InsOrUpdateIntentForBot(string Intent, string userId)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("InsOrUpdateIntentForBot", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Intent", SqlDbType.VarChar).Value = Intent;
                    cmd.Parameters.Add("@UserId", SqlDbType.VarChar).Value = userId;
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
        }

        public static void InsOrUpdateDefaultResponseForBot(string response, string bot)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("InsOrUpdateDefaultResponseForBot", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@response", SqlDbType.VarChar).Value = response;
                    cmd.Parameters.Add("@bot_nm", SqlDbType.VarChar).Value = bot;
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
        }
        public static void InsertTransaction(string channel, string txn_txt, string responseContext, string botName, string UserId, string intent, string entities, string intentScore)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("InsertTransactionForBotCopy", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@txn_dt", SqlDbType.SmallDateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@chnl_id", SqlDbType.VarChar).Value = channel;
                    cmd.Parameters.Add("@txn_conv_id", SqlDbType.VarChar).Value = " ";
                    cmd.Parameters.Add("@txn_txt", SqlDbType.VarChar).Value = txn_txt;
                    cmd.Parameters.Add("@txn_state_txt", SqlDbType.VarChar).Value = responseContext;
                    cmd.Parameters.Add("@txn_ctr", SqlDbType.VarChar).Value = " ";
                    cmd.Parameters.Add("@sel_txt", SqlDbType.VarChar).Value = " ";
                    cmd.Parameters.Add("@order_id", SqlDbType.VarChar).Value = " ";
                    cmd.Parameters.Add("@bot_nm", SqlDbType.VarChar).Value = botName;
                    cmd.Parameters.Add("@UserId", SqlDbType.VarChar).Value = UserId;
                    cmd.Parameters.Add("@Qry_intent_cd", SqlDbType.VarChar).Value = intent;
                    cmd.Parameters.Add("@Qry_intent_conf_vlu", SqlDbType.VarChar).Value = intentScore;
                    cmd.Parameters.Add("@Qry_entity_lst", SqlDbType.VarChar).Value = entities;
                    cmd.Parameters.Add("@Learned_ind", SqlDbType.Char, 1).Value = null;
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
        }

        public static void InsOrUpdateFlightStatus(FlightStatus objFlightStatus)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("InsOrUpdateFlightStatus", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@flt_no", SqlDbType.VarChar).Value = objFlightStatus.flightNumber;
                    cmd.Parameters.Add("@dep_stn", SqlDbType.Char).Value = objFlightStatus.departureAirportCode;
                    cmd.Parameters.Add("@dep_city", SqlDbType.VarChar).Value = objFlightStatus.departureCityName;
                    cmd.Parameters.Add("@arr_stn", SqlDbType.Char).Value = objFlightStatus.arrivalAirportCode;
                    cmd.Parameters.Add("@arr_city", SqlDbType.VarChar).Value = objFlightStatus.arrivalAirportName;
                    cmd.Parameters.Add("@dep_dt", SqlDbType.DateTime).Value = objFlightStatus.departureLocalTimeScheduled;
                    cmd.Parameters.Add("@arr_dt", SqlDbType.DateTime).Value = objFlightStatus.arrivalLocalTimeScheduled;
                    cmd.Parameters.Add("@terminal_vlu", SqlDbType.Char).Value = objFlightStatus.departureGate;
                    cmd.Parameters.Add("@gate_vlu", SqlDbType.Char).Value = objFlightStatus.arrivalGate;
                    cmd.Parameters.Add("@status_ind", SqlDbType.Char).Value = "";
                    cmd.Parameters.Add("@order_id", SqlDbType.Int).Value = 1;
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
        }
        public static void UpdateResponseContextForBot(CustomerResponse objCustomerResponse, string userId)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("UpdateResponseContextForBot", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ResponseContext", SqlDbType.VarChar).Value = objCustomerResponse.responseContext;
                    cmd.Parameters.Add("@response", SqlDbType.VarChar).Value = objCustomerResponse.RespTxtCopy;
                    cmd.Parameters.Add("@response_type", SqlDbType.VarChar).Value = objCustomerResponse.ResponseType;
                    cmd.Parameters.Add("@response_action", SqlDbType.VarChar).Value = objCustomerResponse.ResponseAction;
                    cmd.Parameters.Add("@context", SqlDbType.VarChar).Value = objCustomerResponse.IncomingContext;
                    cmd.Parameters.Add("@UserId", SqlDbType.VarChar).Value = userId;
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }

            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
        }


        public static void UpdateResponseContextForBotCopy(CustomerResponse objCustomerResponse, string userId)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("UpdateResponseContextForBotCopy", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ResponseContext", SqlDbType.VarChar).Value = objCustomerResponse.responseContext;
                    cmd.Parameters.Add("@response", SqlDbType.VarChar).Value = objCustomerResponse.RespTxtCopy;
                    cmd.Parameters.Add("@response_type", SqlDbType.VarChar).Value = objCustomerResponse.ResponseType;
                    cmd.Parameters.Add("@response_action", SqlDbType.VarChar).Value = objCustomerResponse.ResponseAction;
                    cmd.Parameters.Add("@context", SqlDbType.VarChar).Value = objCustomerResponse.IncomingContext;
                    cmd.Parameters.Add("@attribute", SqlDbType.VarChar).Value = objCustomerResponse.attribute;
                    cmd.Parameters.Add("@UserId", SqlDbType.VarChar).Value = userId;
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }

            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
        }
        public static string getLuisLink(string BotName)
        {
            string luis = string.Empty;

            try
            {
                string query = "Select LuisLink from GetJsonObjectTable where BotName = '" + BotName + "'";

                using (var connection = GetSqlConnection())
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    Data dt = new Data();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        luis = reader["LuisLink"].ToString();
                    }

                }

            }
            catch (Exception ex)
            {

            }
            return luis;
        }
        public static string GetKnowledgebaseID(string name)
        {
            string kbid = string.Empty;

            try
            {
                string query = "Select kbid from bfd_knowledgebase where name = '" + name + "'";

                using (var connection = GetSqlConnection())
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    Data dt = new Data();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        kbid = reader["kbid"].ToString();
                    }

                }

            }
            catch (Exception ex)
            {
                Trace.TraceError("Error:" + ex + " Method: ConnectionManager ");
                throw (ex);
            }
            return kbid;
        }
        public class Data
        {
            public string JsonData { get; set; }
        }

        public static string FindMenu(string menu)
        {
            try
            {
                string query = "SELECT  ((CASE WHEN '" + menu + "' = menuid THEN 'Y' WHEN UPPER('" + menu + "') = UPPER(menu_nm) THEN 'Y' WHEN UPPER(menu_kwd) like UPPER('%-" + menu + "-%') THEN 'P' ELSE 'N' END ) + '(' +  menuid +  ')' + menu_nm) AS menu_txt FROM pm_menu WHERE menu_kwd like '%-" + menu + "-%'";

                using (var connection = GetSqlConnection())
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        if (reader.HasRows)
                        {
                            return Convert.ToString(reader["menu_txt"]);
                        }
                        else
                        {
                            return " ";
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                return "E";
            }
        }

        public static string GetCusData(string phone)
        {

            try
            {
                string query = "SELECT CAST(MAX(ctr) AS CHAR(2)) + CAST(MAX(selind) AS CHAR(1)) + (MAX(((CAST(ctr as CHAR(1))) + CAST(cstate AS CHAR(5))))) + MAX(stop_ind) + MAX(consumer_id) + (MAX(((CAST(selind as CHAR(1))) + coalesce(menu,'') ))) as custxt FROM pm_transaction WHERE userphone = '" + phone + "'";

                using (var connection = GetSqlConnection())
                {
                    SqlCommand command = new SqlCommand(query, connection);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        if (reader.HasRows)
                        {
                            return Convert.ToString(reader["custxt"]);
                        }
                        else
                        {
                            return null;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                return "error";
            }
        }

        public static char fltdtchk(String phone)
        {
            try
            {
                string query = "select MAX(datediff(hh,GETDATE(),flt_dt )) AS fltdd from pm_transaction WHERE userphone = '" + phone + "'";

                using (var connection = GetSqlConnection())
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        if (reader.HasRows)
                        {

                            if (Convert.ToInt16(reader["fltdd"]) > 48)
                            {
                                return 'V';
                            }
                            else
                            {
                                return 'E';
                            }

                        }
                        else
                        {
                            return ' ';
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                return 'E';
            }
        }

        public static void UpdatePNR(string PNR, string userId)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("UpdatePNR", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@PNR", SqlDbType.VarChar).Value = PNR;
                    cmd.Parameters.Add("@UserId", SqlDbType.VarChar).Value = userId;
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
        }
        public static void UpdateFirstName(string FirstName, string userId)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("UpdateFirstName", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = FirstName;
                    cmd.Parameters.Add("@UserId", SqlDbType.VarChar).Value = userId;
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
        }
        public static void UpdateFlightNo(string FlightNo, string userId)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("UpdateFlightNo", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@FlightNo", SqlDbType.VarChar).Value = FlightNo;
                    cmd.Parameters.Add("@UserId", SqlDbType.VarChar).Value = userId;
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
        }
        public static void UpdateSelectedFlight(string SelectedFlightNo, string userId)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("UpdateSelectedFlight", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@SelectedFlightNo", SqlDbType.VarChar).Value = SelectedFlightNo;
                    cmd.Parameters.Add("@UserId", SqlDbType.VarChar).Value = userId;
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
        }
        public static void UpdateLastName(string LastName, string userId)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("UpdateLastName", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@LastName", SqlDbType.VarChar).Value = LastName;
                    cmd.Parameters.Add("@UserId", SqlDbType.VarChar).Value = userId;
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
        }

        /* public static void GetCustomerData(string userId, CustomerResponse custResInfo)
         {
             try
             {
                 using (var connection = GetSqlConnection())
                 {
                     SqlCommand cmd = new SqlCommand("GetCustomerDataForIROP", connection);
                     cmd.CommandType = CommandType.StoredProcedure;
                     cmd.Parameters.Add("@UserId", SqlDbType.VarChar).Value = userId;

                     using (SqlDataReader reader = cmd.ExecuteReader())
                     {
                         reader.Read();
                         if (reader.HasRows)
                         {
                             custResInfo.firstName = reader["FirstName"].ToString().Trim();
                             custResInfo.lastName = reader["SecondName"].ToString().Trim();
                             custResInfo.pnr = reader["PNR"].ToString().Trim();
                         }
                     }
                     connection.Close();
                 }

             }
             catch (Exception e)
             {
                 Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                 throw (e);
             }
         }*/

        public static CustomerResponse GetResponseInfoForIntent(string botName, CustomerResponse objCustomerResponse)
        {
            try
            {
                string interimIntent = (objCustomerResponse.botName.ToLower() == "luismasterbot" && objCustomerResponse.intent.ToLower() != "greetuser") ? "None" : objCustomerResponse.intent;

                using (var connection = GetSqlConnection())
                {
                    SqlCommand command = new SqlCommand("GetResponseInfoForIntent", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@intent", SqlDbType.VarChar).Value = interimIntent;
                    command.Parameters.Add("@context", SqlDbType.VarChar).Value = objCustomerResponse.responseContext;
                    command.Parameters.Add("@botname", SqlDbType.VarChar).Value = botName;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        int count = 0;
                        if (reader.HasRows)
                        {

                            while (reader.Read())
                            {
                                count++;
                                if (count == 1)
                                {
                                    objCustomerResponse.ResponseType = reader["response_typ"].ToString().Trim();
                                    objCustomerResponse.respTxt = reader["response_txt"].ToString().Replace("\\n", "\n").Trim();
                                    objCustomerResponse.OutgoingContext = reader["response_context"].ToString().Trim();
                                    //objCustomerResponse.ResponseAction = reader["response_action"].ToString().Trim();
                                    objCustomerResponse.Comp_Name = reader["component_name"].ToString().Trim();
                                    objCustomerResponse.attribute = reader["attribute"].ToString().Trim();
                                }

                                if ((objCustomerResponse.InputMessage.ToLower() == reader["request_txt"].ToString().Trim().ToLower()) && count > 1)
                                {

                                    objCustomerResponse.ResponseType = reader["response_typ"].ToString().Trim();
                                    objCustomerResponse.respTxt = reader["response_txt"].ToString().Replace("\\n", "\n").Trim();
                                    objCustomerResponse.OutgoingContext = reader["response_context"].ToString().Trim();
                                    //objCustomerResponse.ResponseAction = reader["response_action"].ToString().Trim();
                                    objCustomerResponse.Comp_Name = reader["component_name"].ToString().Trim();
                                    objCustomerResponse.attribute = reader["attribute"].ToString().Trim();
                                    break;
                                }

                            }



                        }
                        else if (objCustomerResponse.Entity != null && GetKnowledgeBaseResponse(interimIntent, "Airport", objCustomerResponse.Entity).Count > 0)
                        {
                            objCustomerResponse.respTxt = "";
                            objCustomerResponse.ResponseType = "TEXT";
                            List<string> res = GetKnowledgeBaseResponse(interimIntent, "Airport", objCustomerResponse.Entity);
                            if (res.Count > 1)
                            {
                                objCustomerResponse.respTxt = objCustomerResponse.Entity + " means ";
                            }
                            int i = res.Count;
                            foreach (string str in res)
                            {
                                objCustomerResponse.respTxt += str;
                                i = i - 1;
                                if (i > 0)
                                {
                                    objCustomerResponse.respTxt += " It also means ";
                                }
                            }
                        }
                        else
                        {
                            objCustomerResponse.ResponseType = "TEXT";
                            string defaultMessage = GetDefaultMessage(objCustomerResponse.botName).Trim();
                            if (defaultMessage == string.Empty)
                            {
                                objCustomerResponse.respTxt = "Yikes! I wasn’t able to understand you. Let me grab an agent.";
                            }
                            else
                            {
                                objCustomerResponse.respTxt = defaultMessage;
                            }

                            if (ConfigurationManager.AppSettings["BotName"].ToLower() == "pilotbot")
                                objCustomerResponse.OutgoingContext = objCustomerResponse.responseContext;
                            else
                                objCustomerResponse.OutgoingContext = "None";


                        }

                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
            return objCustomerResponse;
        }
        public static bool SaveResponse(string columnValue, string userID, string context, string documentid, string botname)
        {
            string columnName = string.Empty;
            string tableName = string.Empty;
            string attributeName = string.Empty;
            if (context.ToLower() == "getuserid") columnValue = Utility.Utility.HashPassword(columnValue);
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand command = new SqlCommand("GetPersistData", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@context", SqlDbType.VarChar).Value = context;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        if (reader.HasRows)
                        {
                            columnName = reader["columnnm"].ToString().Trim();
                            tableName = reader["tablenm"].ToString().Trim();
                            attributeName = reader["DocumentDBAttribute"].ToString();
                        }
                    }
                    command = new SqlCommand("UpdateTableQuery", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@tablename", SqlDbType.VarChar).Value = tableName;
                    command.Parameters.Add("@columnname", SqlDbType.VarChar).Value = columnName;
                    command.Parameters.Add("@columnvalue", SqlDbType.VarChar).Value = columnValue;
                    command.Parameters.Add("@userid", SqlDbType.VarChar).Value = userID;
                    command.ExecuteNonQuery();
                    //DocumentDBOperations.UpdateDocument(documentid, attributeName, columnValue);
                    //UpdateSaveAttributeIndicator(botname, attributeName, userID);
                    return true;
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager-SaveResponse ");
                throw (e);
            }

        }

        public static bool SaveResponseCopy(string columnValue, string userID, string context, string documentid, string botname)
        {
            string columnName = string.Empty;
            string tableName = string.Empty;
            string attributeName = string.Empty;
            string filterName = string.Empty;
            if (context.ToLower() == "getuserid") columnValue = Utility.Utility.HashPassword(columnValue);
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand command = new SqlCommand("GetPersistData", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@context", SqlDbType.VarChar).Value = context;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        if (reader.HasRows)
                        {
                            columnName = reader["columnnm"].ToString().Trim();
                            tableName = reader["tablenm"].ToString().Trim();
                            attributeName = reader["DocumentDBAttribute"].ToString();
                        }
                    }
                    command = new SqlCommand("UpdateTableQueryCopy", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@tablename", SqlDbType.VarChar).Value = tableName;
                    command.Parameters.Add("@columnname", SqlDbType.VarChar).Value = columnName;
                    command.Parameters.Add("@columnvalue", SqlDbType.VarChar).Value = columnValue;
                    command.Parameters.Add("@userid", SqlDbType.VarChar).Value = botname;
                    command.ExecuteNonQuery();
                    //DocumentDBOperations.UpdateDocument(documentid, attributeName, columnValue);
                    //UpdateSaveAttributeIndicator(botname, attributeName, userID);
                    return true;
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager-SaveResponse ");
                throw (e);
            }

        }
        public static CustomerResponse VerifyCustomer(string userid, CustomerResponse custResInfo)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("RecognizeUser", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserId", SqlDbType.VarChar).Value = userid;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        if (reader.HasRows)
                        {
                            custResInfo.firstName = reader["first_nm"].ToString().Trim();
                            custResInfo.lastName = reader["last_nm"].ToString().Trim();
                            custResInfo.pnr = reader["pnr"].ToString().Trim();
                        }
                    }
                    connection.Close();
                }

            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
            return custResInfo;
        }
        public static void InsOrUpdateDfd_Orders(string fname, string lname, string channelid, string pnr, string userid)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("InsOrUpdateDfd_Orders", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ChannelId", SqlDbType.VarChar).Value = channelid;
                    cmd.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = fname == null ? "" : fname;
                    cmd.Parameters.Add("@LastName", SqlDbType.VarChar).Value = lname == null ? "" : lname;
                    cmd.Parameters.Add("@PNR", SqlDbType.VarChar).Value = pnr == null ? "" : pnr;
                    cmd.Parameters.Add("@userid", SqlDbType.VarChar).Value = userid;
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
        }

        public static void InsOrUpdateDefinitions(string keyword, string definition)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("InsOrUpdateDefinitions", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@defn_kwd", SqlDbType.VarChar).Value = keyword;
                    cmd.Parameters.Add("@defn_txt", SqlDbType.VarChar).Value = definition == null ? "" : definition;

                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
        }
        public static void InsertBotCreatorUsers(string userid)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("InsertBotCreatorUsers", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@userid", SqlDbType.VarChar).Value = userid;
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
        }
        public static bool CheckBot(string botname)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("CheckBotName", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@botname", SqlDbType.VarChar).Value = botname;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader["mycount"].ToString() == "0")
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    connection.Close();
                }
                return false;
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
        }
        public static void updateBotCreatorLanguage(string language)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("UpdateBotCreatorLang", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@language", SqlDbType.VarChar).Value = language;
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
        }
        public static bool CheckLanguage(CustomerResponse custRes)
        {
            try
            {

                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("checkLanguage", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@language", SqlDbType.VarChar).Value = custRes.language;
                    cmd.Parameters.Add("@bot_nm", SqlDbType.VarChar).Value = MessagesController.userbotname; cmd.ExecuteNonQuery();
                    SqlParameter returnParameter = cmd.Parameters.Add("@returnval", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    cmd.ExecuteNonQuery();
                    int result = (int)cmd.Parameters["@returnval"].Value;
                    connection.Close();
                    if (result == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
        }
        public static void InsOrUpdateKnowledgebase(string name, string kbid)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("InsOrUpdateKnowledgebase", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = name;
                    cmd.Parameters.Add("@kbid", SqlDbType.VarChar).Value = kbid;

                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: ConnectionManager ");
                throw (e);
            }
        }
        public static void MapComponentEntriesByIntent(string inputIntent)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("MapComponentEntries", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@intent", SqlDbType.VarChar).Value = inputIntent;
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error : " + e + " Method: ConnectionManager ");
                throw (e);
            }
        }
        public static string CheckIfUsersDocumentDBIdExist(string userid, string bot_nm, string event_ind)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("CheckIfUserDocumentIDExist", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@bot_nm", SqlDbType.VarChar).Value = bot_nm;
                    cmd.Parameters.Add("@userid", SqlDbType.VarChar).Value = userid;
                    cmd.Parameters.Add("@event_ind", SqlDbType.VarChar).Value = event_ind;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader.HasRows)
                        {
                            return reader["documentid"].ToString();
                        }
                    }
                    return "";
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error : " + e + " Method: ConnectionManager ");
                throw (e);
            }
        }
        public static void InsertUsersDocumentDBId(string userid, string botname, string documentid, string event_ind)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("SaveUsersDocumentDBId", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@bot_nm", SqlDbType.VarChar).Value = botname;
                    cmd.Parameters.Add("@userid", SqlDbType.VarChar).Value = userid;
                    cmd.Parameters.Add("@documentid", SqlDbType.VarChar).Value = documentid;
                    cmd.Parameters.Add("@event_ind", SqlDbType.VarChar).Value = event_ind;
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error : " + e + " Method: ConnectionManager ");
                throw (e);
            }
        }
        public static string GetPropertyToValidate(string botname, string context)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("GetAttributeForValidation", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@inputintent", SqlDbType.VarChar).Value = botname;
                    cmd.Parameters.Add("@context", SqlDbType.VarChar).Value = context;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader.HasRows)
                        {
                            return reader["Attributes"].ToString();
                        }
                        else
                        {
                            return "";
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error : " + e + " Method: ConnectionManager ");
                throw (e);
            }
            return "";
        }
        public static void UpdateSaveAttributeIndicator(string botname, string propName, string userid)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("UpdateSaveAttributeIndicator", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@botname", SqlDbType.VarChar).Value = botname;
                    cmd.Parameters.Add("@attribute", SqlDbType.VarChar).Value = propName;
                    cmd.Parameters.Add("@userid", SqlDbType.VarChar).Value = userid;
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error : " + e + " Method: ConnectionManager ");
                throw (e);
            }
        }
        public static List<string> GetComponentOutputAttribute(string botname, string componentname)
        {
            List<string> response = new List<string>();
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("GetComponentOutputAttributes", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@botname", SqlDbType.VarChar).Value = botname;
                    cmd.Parameters.Add("@event_ind", SqlDbType.VarChar).Value = componentname;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string output = reader["OutputAttributes"].ToString();
                        string[] attributes = output.Split(';');
                        if (attributes.Length > 0)
                        {
                            foreach (string str in attributes)
                            {
                                response.Add(str);
                            }
                        }
                    }
                }
                return response;
            }
            catch (Exception e)
            {
                Trace.TraceError("Error : " + e + " Method: ConnectionManager ");
                throw (e);
            }
        }
        public static CustomerResponse GetLangList(CustomerResponse custRes)
        {
            List<string> response = new List<string>();
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("getLanguageList", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string output = reader["language"].ToString();
                        custRes.languageList.Add(output);
                    }
                }
                return custRes;
            }
            catch (Exception e)
            {
                Trace.TraceError("Error : " + e + " Method: ConnectionManager ");
                throw (e);
            }
        }
        public static void ClearSavedAttributes(string botname, string userid)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("ClearSavedAttributes", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@botname", SqlDbType.VarChar).Value = botname;
                    cmd.Parameters.Add("@userid", SqlDbType.VarChar).Value = userid;
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error : " + e + " Method: ConnectionManager ");
                throw (e);
            }
        }
        public static void AddLogicEntry(string intent, string botname, string request_Text)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("AddLogicEntry", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@botname", SqlDbType.VarChar).Value = botname;
                    cmd.Parameters.Add("@intent", SqlDbType.VarChar).Value = intent;
                    cmd.Parameters.Add("@text", SqlDbType.VarChar).Value = request_Text;
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error : " + e + " Method: ConnectionManager ");
                throw (e);
            }
        }
        public static List<string> GetKnowledgeBaseResponse(string intent, string entity, string request)
        {
            List<string> output = new List<string>();
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("getkbresponse", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@intent", SqlDbType.VarChar).Value = intent;
                    cmd.Parameters.Add("@entity", SqlDbType.VarChar).Value = entity;
                    cmd.Parameters.Add("@request", SqlDbType.VarChar).Value = request;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        output.Add(reader["response"].ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error : " + e + " Method: ConnectionManager ");
                throw (e);
            }
            return output;
        }
        public static void InsertThankNote(string botname, string intent, string note)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("AddThankNote", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@botname", SqlDbType.VarChar).Value = botname;
                    cmd.Parameters.Add("@request_txt", SqlDbType.VarChar).Value = note;
                    cmd.Parameters.Add("@request_intent", SqlDbType.VarChar).Value = intent;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error : " + e + " Method: ConnectionManager ");
                throw (e);
            }
        }
        public static void UpdateWelcomeNote(string botname, string text)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("UpdateWelcomeNote", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@botname", SqlDbType.VarChar).Value = botname;
                    cmd.Parameters.Add("@text", SqlDbType.VarChar).Value = text;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error : " + e + " Method: ConnectionManager ");
                throw (e);
            }
        }
        public static string GetDefaultMessage(string botname)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("GetDefaultMessage", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@botname", SqlDbType.VarChar).Value = botname;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        return reader["default_response"].ToString();
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error : " + e + " Method: ConnectionManager ");
                throw (e);
            }
            return string.Empty;
        }
        public static string GetStatus(string botname, string keyValue)
        {
            string tableName = string.Empty;
            string tableKey = string.Empty;
            string replyText = string.Empty;
            string columnName = string.Empty;
            StringBuilder responseobj = new StringBuilder();
            try
            {
                string query = "Select * from BotWebChatsLinkData where bot_nm = '" + botname + "'";

                using (var connection = GetSqlConnection())
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        tableName = reader["tablename"].ToString();
                        tableKey = reader["tablekey"].ToString();
                    }

                    query = "Select * from " + tableName + " where " + tableKey + " = '" + keyValue + "'";
                    command = new SqlCommand(query, connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        var table = reader.GetSchemaTable();
                        replyText = "Please find the status below.\n\n";
                        foreach (DataRow row in table.Rows)
                        {
                            //responseobj.Append
                            columnName = row["ColumnName"].ToString();
                            if (columnName.ToLower() != "userid" && columnName.ToLower() != "id")
                                replyText += "" + columnName + " : " + reader[columnName].ToString() + "\n\n";
                            //replyText += "CurrentTrackerPhase : " + currentTrackerPhase + "\n\n";
                        }
                    }
                }
                return replyText;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Invalid attempt to read when no data is present"))
                    replyText = "Sorry we dont find any matching record for the ID given. Can you please verify and post your query once again.";
                else
                {
                    Trace.TraceError("Error : " + e + " Method: ConnectionManager ");
                    throw (e);
                }

            }
            return replyText;
        }
        public static string UpdateStatus(string botname,CustomerResponse custObj)
        {
            string tableName = string.Empty;
            string tableKey = string.Empty;
            string replyText = string.Empty;
            string columnName = string.Empty;
            int i = 1;
           
            try
            {
                string query = "Select * from BotWebChatsLinkData where bot_nm = '" + botname + "'";

                using (var connection = GetSqlConnection())
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        tableName = reader["tablename"].ToString();
                        tableKey = reader["tablekey"].ToString();
                    }

                    

                    query = "Select * from " + tableName ;
                    command = new SqlCommand(query, connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        var table = reader.GetSchemaTable();
                        foreach (DataRow row in table.Rows)
                        {
                            //get column name
                            columnName = row["ColumnName"].ToString();
                            //insert into persist
                            UpdatePersistTable(columnName, tableName, columnName);
                            //insert into logic table
                            if (i == 1)
                            {
                                
                                InsOrUpdateLogicTable(botname, custObj.InputMessage, "None", "what is your " + columnName, "updatestatus", "TEXT", "redirect" + i.ToString(), "SAVE", columnName);
                            }
                            if (i > 1 && i < table.Rows.Count)
                            {
                                InsOrUpdateLogicTable(botname, custObj.InputMessage, "redirect" + (i-1).ToString(), "what is your " + columnName, "None", "TEXT", "redirect" + i.ToString(), "SAVE", columnName);
                            }
                            if (i == table.Rows.Count)
                            {
                                InsOrUpdateLogicTable(botname, custObj.InputMessage, "redirect" + (i - 1).ToString(), "what is your " + columnName, "None", "TEXT", "None", "SAVE", columnName);
                            }
                            i++;
                        }
                    }

                   
                }
                return replyText;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Invalid attempt to read when no data is present"))
                    replyText = "Sorry we dont find any matching record for the ID given. Can you please verify and post your query once again.";
                else
                {
                    Trace.TraceError("Error : " + e + " Method: ConnectionManager ");
                    throw (e);
                }

            }
            return replyText;
        }

        public static void UpdatePersistTable(string context,string tableName, string columnName)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("InsOrUpdatePersist", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@table", SqlDbType.VarChar).Value = tableName;
                    cmd.Parameters.Add("@column", SqlDbType.VarChar).Value = columnName;
                    cmd.Parameters.Add("@context", SqlDbType.VarChar).Value = context;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error : " + e + " Method: ConnectionManager ");
                throw (e);
            }
        }

        public static void InsOrUpdateLogicTable(string botName, string request, string context, string response, string intent, string responseType, string responseContext, string action, string attribute)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("InsOrUpdateLogicTableForBotCreator", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@bot_nm", SqlDbType.VarChar).Value = botName;
                    cmd.Parameters.Add("@request_txt", SqlDbType.VarChar).Value = request;
                    cmd.Parameters.Add("@context", SqlDbType.VarChar).Value = context;
                    cmd.Parameters.Add("@response_txt", SqlDbType.VarChar).Value = response;
                    cmd.Parameters.Add("@request_intent", SqlDbType.VarChar).Value = intent;
                    cmd.Parameters.Add("@response_typ", SqlDbType.VarChar).Value = responseType;
                    cmd.Parameters.Add("@response_context", SqlDbType.VarChar).Value = responseContext;
                    cmd.Parameters.Add("@response_action", SqlDbType.VarChar).Value = action;
                    cmd.Parameters.Add("@attribute", SqlDbType.VarChar).Value = attribute;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error : " + e + " Method: ConnectionManager ");
                throw (e);
            }
        }
        public static List<string> GetCategory(string price)
        {

            List<string> typelist = new List<string>();
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("GetTypeBasedOnPrice", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@price", SqlDbType.VarChar).Value = price;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        typelist.Add(reader["Type"].ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error : " + e + " Method: ConnectionManager ");
                throw (e);
            }
            return typelist;
        }
        public static List<ReverseQuery> GetResult(string type)
        {
            List<ReverseQuery> revQuery = new List<ReverseQuery>();
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("GetResultsBasedOnType", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@type", SqlDbType.VarChar).Value = type;
                    cmd.Parameters.Add("@price", SqlDbType.VarChar).Value = MessagesController.price;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ReverseQuery query = new ReverseQuery();
                        query.Stations = reader["Station"].ToString();
                        query.Type = reader["Type"].ToString();
                        query.Price = reader["Price"].ToString();
                        revQuery.Add(query);
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error : " + e + " Method: ConnectionManager ");
                throw (e);
            }
            return revQuery;
        }

        public static string GetCityCode(string city)
        {
            try
            {
                using (var connection = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("GetCityCode", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@city", city).Value = city;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        return reader["defn_kwd"].ToString();
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error : " + e + " Method: ConnectionManager ");
                throw (e);
            }
            return string.Empty;
        }
        /// <summary>
        /// This function gets all the documents from cosmos and returns a list of Documents
        /// </summary>
        /// <returns>List of Document Object</returns>
        public static List<SampleBotTemplate.DataAccess.CosmosDocument> GetCosmosData()
        {
            List<SampleBotTemplate.DataAccess.CosmosDocument> liscomp = new List<SampleBotTemplate.DataAccess.CosmosDocument>();
            try
            {
                string query = "select defn_kwd,defn_txt,defn_category,approved_ind,user_id,bidirection from bfd_definitions";

                using (var connection = GetSqlConnection())
                {
                    SqlCommand command = new SqlCommand(query, connection);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SampleBotTemplate.DataAccess.CosmosDocument comp = new SampleBotTemplate.DataAccess.CosmosDocument();
                            comp.keyword = comp.id = reader["defn_kwd"].ToString().Replace("'", "").Replace("/", " or ");
                            comp.definition = reader["defn_txt"].ToString().Replace("'", "").Replace("/", " or ");
                            if (comp.definition.Length > 255)
                            {
                                comp.definition = comp.definition.Substring(0, 200) + "...";
                            }
                            comp.category = reader["defn_category"].ToString().Replace("'", "").Replace("/", " or ");
                            comp.approvedind = reader["approved_ind"].ToString().Replace("'", "").Replace("/", " or ");
                            comp.userid = reader["user_id"].ToString().Replace("'", "").Replace("/", " or ");
                            comp.homogens = new List<SampleBotTemplate.DataAccess.Homogen>();
                            comp.isbidirectional = reader["bidirection"].ToString().Trim();
                            liscomp.Add(comp);
                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }
            return liscomp;
        }


    }
}