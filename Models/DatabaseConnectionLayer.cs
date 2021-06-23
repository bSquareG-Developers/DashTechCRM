using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DashTechCRM.Models
{
    public class dbConnectionModel
    {
        public SqlConnection conn { get; set; }
        public SqlCommand cmd { get; set; }
        public SqlDataAdapter adpt { get; set; }
        public DataTable dt { get; set; }
        public DataSet ds { get; set; }
        public string query { get; set; }

        public dbConnectionModel()
        {
            //conn = new SqlConnection(ConfigurationManager.ConnectionStrings["locationConnection"].ConnectionString);
            conn = new SqlConnection(ConfigurationManager.ConnectionStrings["onlineConnection"].ConnectionString);
        }
        public DataTable runQuery()
        {
            adpt = new SqlDataAdapter(query, conn);
            dt = new DataTable();
            adpt.Fill(dt);
            return dt;
        }
        public void runCmd(string cmdQuery)
        {
            cmd = new SqlCommand(cmdQuery, conn);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        //this function will return the dictionary that we can change to json
        public List<Dictionary<string, string>> GetDictionary()
        {
            dt = runQuery();
            List<Dictionary<string, string>> keyValuePairs = new List<Dictionary<string, string>>();
            DataColumnCollection cols = dt.Columns;
            foreach (DataRow row in dt.Rows)
            {
                Dictionary<string, string> pairs = new Dictionary<string, string>();
                foreach (DataColumn col in cols)
                {
                    if (!DBNull.Value.Equals(row[col.ColumnName.ToString()]))
                    {
                        if (col.ColumnName.Contains("Status"))
                        {
                            pairs.Add(col.ColumnName.ToString(), CandidateStatusModel.GetShortCode(row[col.ColumnName].ToString()));
                            continue;
                        }
                        string valFormate = row[col.ColumnName].ToString();
                        if (valFormate.Contains("00 AM"))
                        {
                            valFormate = valFormate.Split(' ')[0];
                        }
                        pairs.Add(col.ColumnName.ToString(), valFormate);
                    }
                    else
                    {
                        pairs.Add(col.ColumnName.ToString(), "N/A");
                    }
                }
                keyValuePairs.Add(pairs);
            }
            return keyValuePairs;
        }
    }
}
