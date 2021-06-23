using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace DashTechCRM.Models
{
    public class ConnectionDB
    {
        int _commandTimeOut = 0;
        private readonly SqlConnection _connection;
        public ConnectionDB()
        {
            //_connection = new SqlConnection(ConfigurationManager.ConnectionStrings["locationConnection"].ConnectionString);
            _connection = new SqlConnection(ConfigurationManager.ConnectionStrings["onlineConnection"].ConnectionString);
        }

        Boolean Con_Open(SqlConnection con)
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            return true;
        }

        Boolean Con_Close(SqlConnection con)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }

            return true;
        }

        public DataSet GetDataSet(string str)
        {
            DataSet dsGd = new DataSet();
            try
            {
                Con_Open(_connection);
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = _connection;
                    cmd.CommandTimeout = _commandTimeOut;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = str;
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    sda.Fill(dsGd, "tbl");
                }
                Con_Close(_connection);
                return dsGd;
            }
            catch (Exception e)
            {
                dsGd = null;
                Con_Close(_connection);
                Console.WriteLine(e);
                return null;
            }

        }

        public DataSet GetDataSet(string spName, SqlParameter[] p)
        {
            DataSet dsGd = new DataSet();
            try
            {
                Con_Open(_connection);
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = _connection;
                    cmd.CommandTimeout = _commandTimeOut;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = spName;
                    cmd.Parameters.AddRange(p);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    sda.Fill(dsGd);
                }

                Con_Close(_connection);
                return dsGd;
            }
            catch (Exception e)
            {
                Con_Close(_connection);
                dsGd = null;
                Console.WriteLine(e);
                return null;
            }
        }

        public DataTable GetDataTable(string str)
        {
            DataTable dtGd = new DataTable();
            try
            {
                Con_Open(_connection);
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = _connection;
                    cmd.CommandTimeout = _commandTimeOut;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = str;
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    sda.Fill(dtGd);
                }

                Con_Close(_connection);
                return dtGd;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public DataTable GetDataTable(string spName, SqlParameter[] p)
        {
            DataTable dtGd = new DataTable();
            try
            {
                Con_Open(_connection);
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = _connection;
                    cmd.CommandTimeout = _commandTimeOut;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = spName;
                    cmd.Parameters.AddRange(p);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    sda.Fill(dtGd);
                }
                Con_Close(_connection);
                return dtGd;
            }
            catch (Exception e)
            {
                Con_Close(_connection);
                dtGd = null;
                Console.WriteLine(e);
                return null;
            }
            finally
            {
                dtGd?.Dispose();
                Con_Close(_connection);
            }
        }

        public object Execute_Scaler(string query)
        {
            try
            {
                Con_Open(_connection);
                object values = new object();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = _connection;
                    cmd.CommandTimeout = _commandTimeOut;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = query;
                    values = cmd.ExecuteScalar();
                }

                Con_Close(_connection);
                return values;
            }
            catch (Exception e)
            {
                Con_Close(_connection);
                Console.WriteLine(e);
                return null;
            }
        }

        public object Execute_Scaler(string spName, SqlParameter[] p)
        {
            object objScal;
            try
            {
                Con_Open(_connection);
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = _connection;
                    cmd.CommandTimeout = _commandTimeOut;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = spName;
                    cmd.Parameters.AddRange(p);
                    objScal = cmd.ExecuteScalar();
                }
                Con_Close(_connection);
                return objScal;
            }
            catch (Exception e)
            {
                Con_Close(_connection);
                Console.WriteLine(e);
                return null;
            }
        }

        public bool Execute_NonQuery(string query)
        {
            try
            {
                Con_Open(_connection);
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = _connection;
                    cmd.CommandTimeout = _commandTimeOut;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }

                Con_Close(_connection);
                return true;
            }
            catch (Exception e)
            {
                Con_Close(_connection);
                Console.WriteLine(e);
                return false;
            }
            finally
            {
                Con_Close(_connection);
            }
        }
        public bool Execute_NonQuery(string spName, SqlParameter[] p)
        {
            try
            {
                Con_Open(_connection);
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = _connection;
                    cmd.CommandTimeout = _commandTimeOut;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = spName;
                    cmd.Parameters.AddRange(p);
                    cmd.ExecuteNonQuery();
                }

                Con_Close(_connection);
                return true;
            }
            catch (Exception e)
            {
                Con_Close(_connection);
                Console.WriteLine(e);
                return false;
            }
            finally
            {
                Con_Close(_connection);
            }

        }

        public bool Execute_NonQuery(string spName, SqlParameter[] p, ref int rowAffacted)
        {
            try
            {
                Con_Open(_connection);
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = _connection;
                    cmd.CommandTimeout = _commandTimeOut;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = spName;
                    cmd.Parameters.AddRange(p);
                    rowAffacted = cmd.ExecuteNonQuery();
                }

                Con_Close(_connection);
                return true;
            }
            catch (Exception e)
            {
                Con_Close(_connection);
                Console.WriteLine(e);
                rowAffacted = 0;
                return false;
            }
            finally
            {
                Con_Close(_connection);
            }
        }
    }
}
