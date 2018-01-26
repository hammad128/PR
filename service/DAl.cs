using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;

namespace GestureService
{
    class DAL
    {
        SqlConnection connection;
        public DAL()
        {
            connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionStringPR"].ToString());
        }

        public DataTable GetPedingAPMRequests()
        {
            DataTable dt = new DataTable();
            SqlCommand sqlCommand = new SqlCommand("GM_Select_PendingAPMRequest", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter adp = new SqlDataAdapter(sqlCommand);
            connection.Open();
            adp.Fill(dt);
            connection.Close();
            return dt;
        }
        public void UpdateUserGestureDetail(int GestureDetailCode, int GestureStatusCode, int? PrintStatusCode)
        {
            SqlCommand sqlCommand = new SqlCommand("GM_Update_UserGestureDetail", connection);
            sqlCommand.Parameters.AddWithValue("@GestureDetailCode", GestureDetailCode);
            sqlCommand.Parameters.AddWithValue("@GestureStatusCode", GestureStatusCode);
            if (PrintStatusCode != null)
            {
                sqlCommand.Parameters.AddWithValue("@PrintingStatusCode", PrintStatusCode);
            }
            sqlCommand.CommandType = CommandType.StoredProcedure;
            connection.Open();
            sqlCommand.ExecuteNonQuery();
            connection.Close();
        }
        public void SendSMS()
        {
            SqlCommand sqlCommand = new SqlCommand("GM_Deliver_SMS", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            connection.Open();
            sqlCommand.ExecuteNonQuery();
            connection.Close();
        }
        public void SendEmail()
        {
            SqlCommand sqlCommand = new SqlCommand("GM_Send_Email", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            connection.Open();
            sqlCommand.ExecuteNonQuery();
            connection.Close();
        }

        public void SendNotice()
        {
            SqlCommand sqlCommand = new SqlCommand("GM_Send_Notice", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            connection.Open();
            sqlCommand.ExecuteNonQuery();
            connection.Close();
        }

        public DataSet GetDatasetForPrintingWebAPIInBulk(int GestureDetailCode, string DocumentIDs)
        {
            DataSet ds = new DataSet();
            SqlCommand sqlCommand = new SqlCommand("APM_Select_DataSetForPrintingBulk", connection);
            sqlCommand.Parameters.AddWithValue("@GestureDetailCode", GestureDetailCode);
            sqlCommand.Parameters.AddWithValue("@DocumentIDs", DocumentIDs);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter adp = new SqlDataAdapter(sqlCommand);
            connection.Open();
            adp.Fill(ds);
            connection.Close();
            return ds;
        }
    }

    public enum OG_Gestures
    {
        Email = 2,
        SMS = 3,
        Ecard = 4,
        Card = 5,
        Letter = 6
    }

    public enum OG_GestureStatus
    {
        Pending = 1,
        Delivered = 2
    }
    public enum OG_PrintingStatus
    {
        Pending = 1,
        SendForPrinting = 2
    }
}
