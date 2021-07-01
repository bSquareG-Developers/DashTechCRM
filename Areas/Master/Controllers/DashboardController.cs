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

        // Return all department List
        public string BindDepartment()
        {
            try
            {
                return CommonHelperClass._serializeDatatable(dl.GetDataTable("select *,Id as DeptId from DepartmentMaster"));
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "Master/DashBoard/BindDepartment");
                throw;
            }
        }
        //Insert new department
        public string Department_Insert(string parameter)
        {
            try
            {
                dynamic prm = JObject.Parse(parameter);
                string departmentName = Convert.ToString(prm.DepartmentName);
                string query = string.Format("Insert into DepartmentMaster (DepartmentName) values ('{0}')", departmentName);
                return dl.Execute_NonQuery(query).ToString();
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "Master/Dashboard/Department_Insert");
                throw;
            }
        }
        public string Department_Update(string parameter)
        {
            try
            {
                dynamic prm = JObject.Parse(parameter);
                string Id = Convert.ToString(prm.DeptId);
                string departmentName = Convert.ToString(prm.DepartmentName);
                string query = string.Format("Update DepartmentMaster set DepartmentName = '{0}' where Id = {1}",
                    departmentName, Id);
                return dl.Execute_NonQuery(query).ToString();
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "Master/Dashboard/Department_Update");
                throw;
            }
        }

        public string BindDepartmentwiseRole()
        {
            try
            {
                return CommonHelperClass._serializeDatatable(dl.GetDataTable("select DRT.Id as [DepartmentRoleId],RM.RoleId,RM.RoleTitle,DM.DepartmentName,DM.Id as [DepartmentId] from DepartmentRoleTable DRT Inner Join RoleMaster RM on DRT.RoleId =RM.RoleId Inner Join DepartmentMaster DM on DM.Id = DRT.DeptId order by DRT.DeptId,DRT.RoleId,DRT.SOrder"));
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "Master/Dashboard/BindDepartmentwiseRole");
                throw;
            }
        }
        public string GetUserwithRole()
        {
            try
            {
                return CommonHelperClass._serializeDatatable(dl.GetDataTable("select UserId as [ID],(UAD.FullName + ' - ' + RM.RoleTitle) as [TEXT] from UserAccountDetails UAD Inner Join RoleMaster RM on RM.RoleId = UAD.RefRoleId"));
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "Master/Dashboard/GetUserwithRole");
                throw;
            }
        }

        public string GetRoles()
        {
            try
            {
                return CommonHelperClass._serializeDatatable(dl.GetDataTable("select RoleId as [ID],[RoleTitle] as [TEXT] from rolemaster order by 1"));
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "Master/Dashboard/GetRoles");
                throw;
            }
        }

        public string saveTeam(string parameter)
        {

            try
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
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "Master/Dashboard/saveTeam");
                throw;
            }
        }

        public string GetTeamDetails()
        {
            try
            {
                return CommonHelperClass._serializeDatatable(dl.GetDataTable("select TM.TeamName ,TM.LeaderId,TM.ReportingPersonId,TM.DepartmentId, Leader.FullName as[LeaderName],ReportingPerson.FullName as [ReportingPerson],DM.DepartmentName,Convert(varchar,TM.Timest,106) as [CreatedDate] from TeamMaster  TM Inner join UserAccountDetails Leader on Leader.UserId = TM.LeaderId Inner join UserAccountDetails ReportingPerson on ReportingPerson.UserId = TM.ReportingPersonId Inner Join DepartmentMaster DM on DM.Id = TM.DepartmentId  order by TM.Id"));
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "Master/Dashboard/GetTeamDetails");
                throw;
            }
        }

        public string GetTeamDetailsManage()
        {
            try
            {
                return CommonHelperClass._serializeDatatable(dl.GetDataTable("select TD.Id,TM.TeamName ,Leader.FullName as LeaderName,  TeamMember.FullName as MemberName,DM.DepartmentName,Convert(varchar,TM.timest,106) as TeamCreatedDate from TeamDetailsManage TD Inner join UserAccountDetails TeamMember on TD.MemberId = TeamMember.UserId INNER JOIN TeamMaster TM on TM.Id = TD.TeamMasterId INNER JOIN UserAccountDetails Leader on Leader.UserId = TM.LeaderId INNER JOIN DepartmentMaster DM on DM.Id = TM.DepartmentId order by TM.Id desc"));
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "Master/Dashboard/GetTeamDetailsManage");
                throw;
            }
        }

        public string BindTeamList()
        {
            try
            {
                return CommonHelperClass._serializeDatatable(dl.GetDataTable("select ID , TeamName as [TEXT] from TeamMaster"));
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "Master/Dashboard/BindTeamList");
                throw;
            }
        }


        public string BindRemainingMembers()
        {
            try
            {
                return CommonHelperClass._serializeDatatable(dl.GetDataTable("select UAD.UserId as [ID] , (UAD.fullName + ' - ' + RM.RoleTitle)as [TEXT] from UserAccountDetails UAD INNER JOIN RoleMaster RM on RM.RoleId = UAD.refRoleId where UAD.UserId not in (Select MemberId from TeamDetailsManage) order by 1"));
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "Master/Dashboard/BindRemainingMembers");
                throw;
            }
        }

        public string SaveTeamMember(string parameter)
        {
            try
            {
                dynamic prm = JObject.Parse(parameter);
                string teamId = Convert.ToString(prm.teamId);
                string teamMemberId = Convert.ToString(prm.teamMemberId);
                string query = string.Format("Insert into TeamDetailsManage (TeamMasterId,MemberId,IsActive) values ({0},{1},{2})", teamId, teamMemberId, 1);
                return dl.Execute_NonQuery(query).ToString();
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "Master/Dashboard/SaveTeamMember");
                throw;
            }
        }


        // this will delete mapped member
        public string TeamMember_Delete(string parameter)
        {
            try
            {
                dynamic prm = JObject.Parse(parameter);
                string id = Convert.ToString(prm.ID);
                string query = string.Format("Delete from TeamDetailsManage where Id = {0}", id);
                return dl.Execute_NonQuery(query).ToString();
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "Master/Dashboard/TeamMember_Delete");
                throw;
            }
        }

        public string changeRoleDepartment(string parameter)
        {
            try
            {
                dynamic prm = JObject.Parse(parameter);
                string departmentRoleId = Convert.ToString(prm.departmentRoleId);
                string role = Convert.ToString(prm.role);
                string department = Convert.ToString(prm.department);
                string query = string.Format("update DepartmentRoleTable set DeptId ={0},RoleId ={1} where Id = {2}", department, role, departmentRoleId);
                return dl.Execute_NonQuery(query).ToString();
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "Master/Dashboard/changeRoleDepartment");
                throw;
            }
        }
    }
}
