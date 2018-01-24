
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace PublicRelationWeb.Common
{
    public static class DataAccess
    {

        #region Async Helpers

        public async static Task<int> ExecuteNonQueryAsync(string connectionString, CommandType commandType, string commandText, SqlParameter[] commandParameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = commandType;
                    command.CommandText = commandText;
                    if (commandParameters != null && commandParameters.Length > 0)
                        command.Parameters.AddRange(commandParameters);
                    int retval = await command.ExecuteNonQueryAsync();
                    connection.Close();
                    command.Parameters.Clear();
                    return retval;
                }
            }
        }

        public async static Task<DataSet> ExecuteDatasetAsync(string connectionString, CommandType commandType, string commandText, SqlParameter[] commandParameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandTimeout = 0;
                    command.CommandType = commandType;
                    command.CommandText = commandText;

                    if (commandParameters != null && commandParameters.Length > 0)
                        command.Parameters.AddRange(commandParameters);

                    DataSet ds = await Task.Run(() => { return FillData(command, connection); });

                    return ds;
                }
            }
        }

        public static DataTable CreateTableFromSchema(DataTable schemaTable)
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < schemaTable.Rows.Count; i++)
            {
                dt.Columns.Add(schemaTable.Rows[i]["ColumnName"].ToString());
            }
            return dt;
        }

        public static DataRow GetDataRow(IDataRecord dr, DataTable dt)
        {
            DataRow drow;
            drow = dt.NewRow();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                drow[dt.Columns[i].ColumnName] = dr[dt.Columns[i].ColumnName];
            }
            return drow;
        }

        private static DataSet FillData(SqlCommand comm, SqlConnection connectionName)
        {
            SqlDataAdapter adp = new SqlDataAdapter();

            adp.SelectCommand = comm;

            DataSet ds = new DataSet();

            adp.Fill(ds);

            return ds;
        }

        #endregion


        #region Sync Helper
        public static bool ExecuteNonQuery(string connectionString, string spName, SqlParameter[] parameters)
        {
            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand(spName, sqlcon);
                    command.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        foreach (SqlParameter param in parameters)
                        {
                            command.Parameters.Add(param);
                        }
                    }
                    if (sqlcon != null && sqlcon.State != ConnectionState.Open)
                        sqlcon.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    if (sqlcon != null && sqlcon.State == ConnectionState.Open)
                        sqlcon.Close();
                }

                return true;
            }
        }

        public static int? ExecuteScalar(string connectionString, string spName, SqlParameter[] parameters)
        {
            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                try
                {
                    int? newId = null;
                    SqlCommand command = new SqlCommand(spName, sqlcon);
                    command.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        foreach (SqlParameter param in parameters)
                            command.Parameters.Add(param);
                    }
                    sqlcon.Open();
                    newId = Convert.ToInt32(command.ExecuteScalar());

                    return newId;
                }
                catch (Exception)
                {
                    return -1;
                }
                finally
                {
                    if (sqlcon != null && sqlcon.State == ConnectionState.Open)
                        sqlcon.Close();
                }
            }
        }

        public static DataSet GetDataSet(string connectionString, string spName, SqlParameter[] parameters)
        {
            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand(spName, sqlcon);
                    command.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        foreach (SqlParameter param in parameters)
                        {
                            command.Parameters.Add(param);
                        }
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);

                    return ds;
                }
                catch (Exception)
                {
                    return null;
                }
                finally
                {
                    if (sqlcon != null && sqlcon.State == ConnectionState.Open)
                        sqlcon.Close();
                }
            }
        }

        public static DataTable GetDataTable(string connectionString, string spName, SqlParameter[] parameters)
        {
            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand(spName, sqlcon);
                    command.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        foreach (SqlParameter param in parameters)
                        {
                            command.Parameters.Add(param);
                        }
                    }
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    return table;
                }
                catch (Exception)
                {
                    return null;
                }
                finally
                {
                    if (sqlcon != null && sqlcon.State == ConnectionState.Open)
                        sqlcon.Close();
                }
            }
        }

      
        public static DataTable GetDataTableByQuery(string connectionString, string query, SqlParameter[] parameters)
        {
            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand(query, sqlcon);
                    command.CommandType = CommandType.Text;
                    if (parameters != null)
                    {
                        foreach (SqlParameter param in parameters)
                        {
                            command.Parameters.Add(param);
                        }
                    }
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    return table;
                }
                catch (Exception)
                {
                    return null;
                }
                finally
                {
                    if (sqlcon != null && sqlcon.State == ConnectionState.Open)
                        sqlcon.Close();
                }
            }
        }

        public static int? GetScalerValueByQuery(string connectionString, string query, SqlParameter[] parameters)
        {
            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand(query, sqlcon);
                    command.CommandType = CommandType.Text;
                    if (parameters != null)
                    {
                        foreach (SqlParameter param in parameters)
                        {
                            command.Parameters.Add(param);
                        }
                    }
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    sqlcon.Open();
                    return (int?)command.ExecuteScalar();
                }
                catch (Exception)
                {
                    return -1;
                }
                finally
                {
                    if (sqlcon != null && sqlcon.State == ConnectionState.Open)
                        sqlcon.Close();
                }
            }
        }
        #endregion

    }
}
