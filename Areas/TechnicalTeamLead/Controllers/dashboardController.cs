using DashTechCRM.Models;
using DashTechCRM.Models.User;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace DashTechCRM.Areas.TechnicalTeamLead.Controllers
{
    public class dashboardController : Controller
    {
        #region Data connection Objects
        DataTable dt = new DataTable();
        List<SqlParameter> p = new List<SqlParameter>();
        ConnectionDB dl = new ConnectionDB();
        #endregion


        // GET: TechnicalTeamLead/dashboard
        public ActionResult Index()
        {
            return View();
        }

        #region Task Title Master
        public ActionResult TaskTitleMasterManage()
        {
            return View();
        }
        public string InsertUpdateTaskTitle(string parameter)
        {
            try
            {
                dynamic prm = JObject.Parse(parameter);
                UserObject user = Session["userInfo"] as UserObject;
                p = new List<SqlParameter>();
                string userId = user != null ? user.UserId.ToString() : "";
                p.Add(new SqlParameter("@TaskTitleId", Convert.ToString(prm.TaskTitleId)));
                p.Add(new SqlParameter("@TaskName", Convert.ToString(prm.TaskName)));
                p.Add(new SqlParameter("@IsActive", Convert.ToString(prm.IsActive)));
                p.Add(new SqlParameter("@CreatedBy", userId));
                p.Add(new SqlParameter("@Flag", Convert.ToString(prm.flag)));
                object result = dl.Execute_Scaler("TaskTitleMaster_InsertUpdate", p.ToArray());
                return result.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public string GetTaskTitleMaster()
        {
            try
            {
                dt = dl.GetDataTable("TaskTitleMaster_Get");
                return CommonHelperClass._serializeDatatable(dt);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public string DeleteTaskTitle(string parameter)
        {
            try
            {
                dynamic prm = JObject.Parse(parameter);
                p = new List<SqlParameter>();
                p.Add(new SqlParameter("@Id", Convert.ToString(prm.Id)));
                object result = dl.Execute_NonQuery("TaskTitleMaster_Delete", p.ToArray());
                return result.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        #endregion

        #region Task Assign Manage
        public ActionResult TaskAssignManage()
        {
            return View();
        }
        public string TaskTitleGet()
        {
            try
            {
                dt = dl.GetDataTable("TaskTitleGet");
                return CommonHelperClass._serializeDatatable(dt);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public string GetUsersByRole(string parameter)
        {
            try
            {
                dynamic prm = JObject.Parse(parameter);

                p = new List<SqlParameter>();
                p.Add(new SqlParameter("@RoleId", Convert.ToString(prm.userRole)));
                dt = dl.GetDataTable("GetUsersByRole", p.ToArray());
                return CommonHelperClass._serializeDatatable(dt);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public string GetStatus()
        {
            dt = dl.GetDataTable("TaskStatusDetails_GetStatus");
            return CommonHelperClass._serializeDatatable(dt);
        }
        public string AssignTaskToExperts(string parameter)
        {
            try
            {
                UserObject user = Session["userInfo"] as UserObject;
                p = new List<SqlParameter>();
                dynamic prm = JObject.Parse(parameter);
                string userId = user != null ? user.UserId.ToString() : "";
                p.Add(new SqlParameter("@TaskId", Convert.ToString(prm.TaskId)));
                p.Add(new SqlParameter("@TaskTitleId", Convert.ToString(prm.taskTitle)));
                p.Add(new SqlParameter("@TaskDetails", Convert.ToString(prm.taskDetails)));
                p.Add(new SqlParameter("@AssignBy", userId));
                p.Add(new SqlParameter("@AssignTo", Convert.ToString(prm.assignTo)));
                p.Add(new SqlParameter("@AssignDate", Convert.ToString(DateTime.Now.ToString("yyyy-MM-dd"))));
                p.Add(new SqlParameter("@AssignTime", Convert.ToString(DateTime.Now.TimeOfDay)));
                p.Add(new SqlParameter("@EndDate", Convert.ToString(prm.endDate)));
                p.Add(new SqlParameter("@EndTime", Convert.ToString(prm.endTime)));
                p.Add(new SqlParameter("@Status", Convert.ToString(prm.status)));
                p.Add(new SqlParameter("@Feedback", Convert.ToString(prm.feedback)));
                p.Add(new SqlParameter("@FLAG", Convert.ToString(prm.flag)));
                object result = dl.Execute_Scaler("TaskMaster_InsertUpdate_TechnicalLead_TechnicalTeamManager", p.ToArray());
                return result.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public string TaskMasterGet()
        {
            try
            {
                dt = dl.GetDataTable("TaskMasterGet");
                return CommonHelperClass._serializeDatatable(dt);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public string DeleteTask(string parameter)
        {
            try
            {
                p = new List<SqlParameter>();
                dynamic prm = JObject.Parse(parameter);
                p.Add(new SqlParameter("@Id", Convert.ToString(prm.id)));
                object result = dl.Execute_NonQuery("TaskMaster_Delete", p.ToArray());
                return result.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        #endregion

        #region Task Manage (Candidate Technical ExpertDetails) // ASSIGN CANDIDATE TO EXPERTS
        public ActionResult TaskManage()
        {
            return View();
        }
        public string TaskMasteGet()
        {
            try
            {
                UserObject user = Session["userInfo"] as UserObject;
                p = new List<SqlParameter>();
                string userId = user != null ? user.UserId.ToString() : "";
                p.Add(new SqlParameter("@UserId", userId));
                dt = dl.GetDataTable("TaskMasteGet_TechnicalExperts", p.ToArray());
                return CommonHelperClass._serializeDatatable(dt);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public string CandidateTechnicalExpertDetailsGet()
        {
            try
            {
                dt = dl.GetDataTable("CandidateTechnicalExpertDetailsGet");
                return CommonHelperClass._serializeDatatable(dt);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public string CandidateMasterBindCandidate()
        {
            try
            {
                dt = dl.GetDataTable("CandidateMasterBindCandidate");
                return CommonHelperClass._serializeDatatable(dt);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public string CandidateTechnicalExpertDetailsInsertUpdate(string parameter)
        {
            try
            {
                dynamic prm = JObject.Parse(parameter);
                p = new List<SqlParameter>();
                UserObject user = Session["userInfo"] as UserObject;
                string assignedBy = user != null ? user.RocketName.ToString() : "";
                p.Add(new SqlParameter("@CTID", Convert.ToString(prm.ctId)));
                p.Add(new SqlParameter("@CTAssignDate", Convert.ToString(DateTime.Now.ToString("yyyy-MM-dd"))));
                p.Add(new SqlParameter("@CTAssingedRemarks", Convert.ToString(prm.assignedRemarks)));
                p.Add(new SqlParameter("@CTReassignedRemarks", Convert.ToString(prm.reAssignRemarks)));
                p.Add(new SqlParameter("@RefAssignedCandidateId", Convert.ToString(prm.candidateId)));
                p.Add(new SqlParameter("@RefAssignedExpertId", Convert.ToString(prm.technicalExpert)));
                p.Add(new SqlParameter("@AssignedBy", assignedBy));
                p.Add(new SqlParameter("@RefBatchId", "0"));
                p.Add(new SqlParameter("@Flag", Convert.ToString(prm.flag)));
                object result = dl.Execute_Scaler("CandidateTechnicalExpertDetails_InsertUpdate", p.ToArray());
                return result.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        #endregion

        #region Task Manage Master

        public string TaskManageMasterGet()
        {
            try
            {
                dt = dl.GetDataTable("TaskManageMasterGet");
                return CommonHelperClass._serializeDatatable(dt);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public string TaskCategoryMasterGet()
        {
            dt = dl.GetDataTable("TaskCategoryMasterGet");
            return CommonHelperClass._serializeDatatable(dt);
        }
        public string TaskManageMasterInsertUpdate(string parameter)
        {
            try
            {
                dynamic prm = JObject.Parse(parameter);
                p = new List<SqlParameter>();
                p.Add(new SqlParameter("@TMId", Convert.ToString(prm.TMId)));
                p.Add(new SqlParameter("@RefTaskCatId", Convert.ToString(prm.taskCategory)));
                p.Add(new SqlParameter("@TaskDate", Convert.ToString(DateTime.Now.ToString("yyyy-MM-dd"))));
                p.Add(new SqlParameter("@TaskStartTime", Convert.ToString(prm.startTime)));
                p.Add(new SqlParameter("@TaskEndTime", Convert.ToString(prm.endTime)));
                p.Add(new SqlParameter("@TaskStatus", Convert.ToString(prm.TaskManageMasterStatus)));
                p.Add(new SqlParameter("@RefCTId", Convert.ToString(prm.refCTId)));
                p.Add(new SqlParameter("@TotalMin", Convert.ToString(prm.taskRunningMinutes)));
                p.Add(new SqlParameter("@TaskRemarks", Convert.ToString(prm.taskRemarks)));
                object result = dl.Execute_Scaler("TaskManageMaster_InsertUpdate", p.ToArray());
                return result.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        #endregion
    }
}
