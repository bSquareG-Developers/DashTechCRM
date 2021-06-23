using DashTechCRM.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Web.Mvc;

namespace DashTechCRM.Areas.Master.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Master/Dashboard
        ConnectionDB dl = new ConnectionDB();
        DataTable dt = new DataTable();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ManageDepartment()
        {
            return View();
        }

        // Return all department List
        public string BindDepartment()
        {
            return CommonHelperClass._serializeDatatable(dl.GetDataTable("select *,Id as DeptId from DepartmentMaster"));
        }
        //Insert new department
        public string Department_Insert(string parameter)
        {
            dynamic prm = JObject.Parse(parameter);
            string departmentName = Convert.ToString(prm.DepartmentName);
            string query = string.Format("Insert into DepartmentMaster (DepartmentName) values ('{0}')", departmentName);
            return dl.Execute_NonQuery(query).ToString();
        }
        public string Department_Update(string parameter)
        {
            dynamic prm = JObject.Parse(parameter);
            string Id = Convert.ToString(prm.DeptId);
            string departmentName = Convert.ToString(prm.DepartmentName);
            string query = string.Format("Update DepartmentMaster set DepartmentName = '{0}' where Id = {1}",
                departmentName, Id);
            return dl.Execute_NonQuery(query).ToString();
        }

        public string BindDepartmentwiseRole()
        {
            return CommonHelperClass._serializeDatatable(dl.GetDataTable("select DRT.Id as [DepartmentRoleId],RM.RoleId,RM.RoleTitle,DM.DepartmentName,DM.Id as [DepartmentId] from DepartmentRoleTable DRT Inner Join RoleMaster RM on DRT.RoleId =RM.RoleId Inner Join DepartmentMaster DM on DM.Id = DRT.DeptId order by DRT.DeptId,DRT.RoleId,DRT.SOrder"));
        }
        public string GetUserwithRole()
        {
            return CommonHelperClass._serializeDatatable(dl.GetDataTable("select UserId as [ID],(UAD.FullName + ' - ' + RM.RoleTitle) as [TEXT] from UserAccountDetails UAD Inner Join RoleMaster RM on RM.RoleId = UAD.RefRoleId"));
        }

        public string GetRoles()
        {
            return CommonHelperClass._serializeDatatable(dl.GetDataTable("select RoleId as [ID],[RoleTitle] as [TEXT] from rolemaster order by 1"));
        }

        public string saveTeam(string parameter)
        {

            ConnectionDB dl = new ConnectionDB();
            dynamic prm = JObject.Parse(parameter);
            string leaderId = Convert.ToString(prm.leaderId);
            string teamName = Convert.ToString(prm.teamName);
            string reportingPersonName = Convert.ToString(prm.reportingPerson);
            string department = Convert.ToString(prm.department);
            string query = string.Format("Insert into TeamMaster (TeamName,LeaderId,ReportingPersonId,DepartmentId,isTeamActive,Timest) values ('{0}',{1},{2},{3},{4},'{5}')", teamName, leaderId, reportingPersonName, department, 1, DateTime.Now.Date);
            return dl.Execute_NonQuery(query).ToString();
        }

        public string GetTeamDetails()
        {
            return CommonHelperClass._serializeDatatable(dl.GetDataTable("select TM.TeamName ,TM.LeaderId,TM.ReportingPersonId,TM.DepartmentId, Leader.FullName as[LeaderName],ReportingPerson.FullName as [ReportingPerson],DM.DepartmentName,Convert(varchar,TM.Timest,106) as [CreatedDate] from TeamMaster  TM Inner join UserAccountDetails Leader on Leader.UserId = TM.LeaderId Inner join UserAccountDetails ReportingPerson on ReportingPerson.UserId = TM.ReportingPersonId Inner Join DepartmentMaster DM on DM.Id = TM.DepartmentId  order by TM.Id"));
        }

        public string GetTeamDetailsManage()
        {
            return CommonHelperClass._serializeDatatable(dl.GetDataTable("select TD.Id,TM.TeamName ,Leader.FullName as LeaderName,  TeamMember.FullName as MemberName,DM.DepartmentName,Convert(varchar,TM.timest,106) as TeamCreatedDate from TeamDetailsManage TD Inner join UserAccountDetails TeamMember on TD.MemberId = TeamMember.UserId INNER JOIN TeamMaster TM on TM.Id = TD.TeamMasterId INNER JOIN UserAccountDetails Leader on Leader.UserId = TM.LeaderId INNER JOIN DepartmentMaster DM on DM.Id = TM.DepartmentId order by TM.Id desc"));
        }

        public string BindTeamList()
        {
            return CommonHelperClass._serializeDatatable(dl.GetDataTable("select ID , TeamName as [TEXT] from TeamMaster"));
        }

        public string BindRemainingMembers()
        {
            return CommonHelperClass._serializeDatatable(dl.GetDataTable("select UAD.UserId as [ID] , UAD.fullName as [TEXT] from UserAccountDetails UAD where UAD.UserId not in (Select MemberId from TeamDetailsManage) order by 1"));
        }

        public string SaveTeamMember(string parameter)
        {
            dynamic prm = JObject.Parse(parameter);
            string teamId = Convert.ToString(prm.teamId);
            string teamMemberId = Convert.ToString(prm.teamMemberId);
            string query = string.Format("Insert into TeamDetailsManage (TeamMasterId,MemberId,IsActive) values ({0},{1},{2})", teamId, teamMemberId, 1);
            return dl.Execute_NonQuery(query).ToString();
        }

        public string TeamMember_Delete(string parameter)
        {
            dynamic prm = JObject.Parse(parameter);
            string id = Convert.ToString(prm.ID);
            string query = string.Format("Delete from TeamDetailsManage where Id = {0}", id);
            return dl.Execute_NonQuery(query).ToString();
        }
    }
}
