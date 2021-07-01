using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DashTechCRM.Models
{
    public class CommonHelperClass
    {
        DataTable dt = new DataTable();
        ConnectionDB dl = new ConnectionDB();
        List<SqlParameter> p = new List<SqlParameter>();
        public static string _serializeDatatable(DataTable dt)
        {
            var settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            settings.Converters.Add(new DataSetConverter());
            return JsonConvert.SerializeObject(dt, settings);
        }
        public static string _serializeDataSet(DataSet ds)
        {
            var settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            settings.Converters.Add(new DataSetConverter());
            return JsonConvert.SerializeObject(ds, settings);
        }

        public DataTable GetfollowupStatusByDepartment(string Department)
        {
            try
            {
                return dl.GetDataTable("select StatusName as [ID] ,StatusName as [TEXT]  from FollowUpStatusMaster where Department = '" + Department + "' and isActive = 1 order by Sorder");
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "CommonHelperClass/GetfollowupStatusByDepartment");
                throw;
            }
        }
        public DataTable GetTeamOfUser(int userId)
        {
            try
            {
                return dl.GetDataTable("select UAD.UserId as [ID],UAD.FullName as [TEXT] from TeamDetailsManage TDM INNER JOIN TeamMaster TM on TDM.TeamMasterId = TM.ID INNER JOIN UserAccountDetails UAD on UAD.UserId = TDM.MemberId where TM.LeaderId = " + userId);
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "CommonHelperClass/GetTeamOfUser");
                throw;
            }
        }

        public static void InsertErrorLog(string Error, string ReportName)
        {
            ConnectionDB dl = new ConnectionDB();
            List<SqlParameter> p = new List<SqlParameter>();
            p.Add(new SqlParameter("@ErrorMessage", Error));
            p.Add(new SqlParameter("@ErrorURL", ReportName));
            dl.Execute_NonQuery("ErrorLog_Insert", p.ToArray());
        }
    }


}
