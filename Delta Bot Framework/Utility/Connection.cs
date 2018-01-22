using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleBotTemplate
{
    public class Connection
    {
        //private members
        private string azuredbconnstring = string.Empty;
        private SqlConnection azuresqlconn = new SqlConnection();

        //public members
        public Connection(string connstring)
        {
            AzureDBConnString = connstring;
            GetAzureSQLConnection();
        }
        public string AzureDBConnString
        {
            get
            {
                return azuredbconnstring;
            }
            set
            {
                azuredbconnstring = value;
            }

        }
        public SqlConnection AzureSQLConn
        {
            get
            {
                return azuresqlconn;
            }
            set
            {
                azuresqlconn = value;
            }

        }
        public void GetAzureSQLConnection()
        {
            try
            {
                AzureSQLConn = new SqlConnection(AzureDBConnString);
                AzureSQLConn.Open();
            }
            catch (Exception e)
            {
                throw new Exception("Error in BOTSerive Library Connection.cs File in GetAzureSQLConnection Method - " + e.Message);
            }
        }
        public void CloseAzureSQLConnection()
        {
            try
            {
                if (AzureSQLConn.State == ConnectionState.Open)
                {
                    AzureSQLConn.Close();
                    AzureSQLConn.Dispose();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error in BOTSerive Library Connection.cs File in CloseAzureSQLConnection Method - " + e.Message);
            }
        }
    }
}
