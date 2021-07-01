using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace DashTechCRM.Models.Reports
{
    public class CandidateDetailBoardModel
    {
        public int CandidateId { get; set; }
        public string CandidateName { get; set; }
        public DateTime OnBoardingDate { get; set; }
        public string SalesAssocaites { get; set; }
        public DateTime? ResumeProcessStarted { get; set; }
        public string ExpertCVMakerName { get; set; }
        public DateTime? ResumeProcessEnded { get; set; }
        public int TotalResumeDays { get; set; }
        public string CurrentStatus { get; set; }
        public DateTime? TrainingStartDate { get; set; }
        public DateTime? TrainingEndDate { get; set; }
        public int TotalTrainingDays { get; set; }
        public string TrainerName { get; set; }
        public DateTime? ResumeUnderstandingDate { get; set; }
        public string ResumeUnderstandingBy { get; set; }
        public DateTime? MarketingStartDate { get; set; }
        public int TotalMarketingDays { get; set; }
        public string RecruiterName { get; set; }
        public DateTime? PODate { get; set; }
        public string LocationName { get; set; }
    }
    public class FollowupDetailsModel
    {
        public int CandidateId { get; set; }
        public string Status { get; set; }
        public string FollowupBy { get; set; }
        public DateTime FollowupDate { get; set; }
        public TimeSpan FollowUpTime { get; set; }
        public string Remarks { get; set; }

    }

    public class CandidateDetailBoardLogic
    {
        dbConnectionModel sqlObj;
        public CandidateDetailBoardLogic()
        {
            sqlObj = new dbConnectionModel();
        }

        public CandidateDetailBoardModel GetCandidate(int id)
        {
            DateTime cd = DateTime.Now.Date;
            CandidateDetailBoardModel candidate = new CandidateDetailBoardModel();
            string query = @"select CandidateMaster.CandidateStatus,                            CandidateMaster.CandidateId,CandidateMaster.CandidateName,CandidateMaster.MarketingStartDate,
                POData.PODate, CandidateMaster.Date as OnBoardingDate,FullName as SalesAssociateName  from 
                CandidateMaster inner join UserAccountDetails on  UserId = RefSalesAssociate
                left join PODetails as POData on POData.CandidateId = CandidateMaster.CandidateId
                where CandidateStatus <> 'Deleted' and CandidateMaster.CandidateId = " + id;

            sqlObj.adpt = new System.Data.SqlClient.SqlDataAdapter(query, sqlObj.conn);
            sqlObj.dt = new System.Data.DataTable();
            sqlObj.adpt.Fill(sqlObj.dt);
            if (sqlObj.dt.Rows.Count == 0)
            {
                return candidate;
            }
            else
            {
                foreach (DataRow row in sqlObj.dt.Rows)
                {
                    if (row["CandidateName"].ToString().Length == 0) { continue; }
                    candidate.CandidateId = int.Parse(row["CandidateId"].ToString());
                    candidate.CandidateName = row["CandidateName"].ToString();
                    candidate.CurrentStatus = row["CandidateStatus"].ToString();
                    candidate.SalesAssocaites = row["SalesAssociateName"].ToString();
                    candidate.OnBoardingDate = DateTime.Parse(row["OnBoardingDate"].ToString());
                    if (!DBNull.Value.Equals(row["MarketingStartDate"]))
                    {
                        candidate.MarketingStartDate = DateTime.Parse(row["MarketingStartDate"].ToString());

                    }
                    if (!DBNull.Value.Equals(row["PODate"]) && candidate.MarketingStartDate != null)
                    {
                        candidate.PODate = DateTime.Parse(row["PODate"].ToString());
                        DateTime md = (DateTime)candidate.MarketingStartDate;
                        DateTime pd = (DateTime)candidate.PODate;
                        candidate.TotalMarketingDays = pd.Subtract(md).Days;
                    }
                    else if (candidate.MarketingStartDate != null)
                    {
                        DateTime md = (DateTime)candidate.MarketingStartDate;
                        candidate.TotalMarketingDays = cd.Subtract(md).Days;
                    }
                }
                query = string.Format(@"
                select Date as ResumeProcessDate, FullName as ExpertCVName,  CandidateTimeLine.Status  from CandidateTimeLine  
                inner join UserAccountDetails on UserId = ChangedBy
                where CandidateTimeLine.Status like '%Resume Call%' and CandidateTimeLine.RefCandidateId = {0}
                order by Date", id);

                sqlObj.adpt = new System.Data.SqlClient.SqlDataAdapter(query, sqlObj.conn);
                sqlObj.dt = new System.Data.DataTable();
                sqlObj.adpt.Fill(sqlObj.dt);

                if (sqlObj.dt.Rows.Count == 1)
                {
                    DataRow row = sqlObj.dt.Rows[0];
                    candidate.ExpertCVMakerName = row["ExpertCVName"].ToString();
                    candidate.ResumeProcessStarted = DateTime.Parse(row["ResumeProcessDate"].ToString());
                }

                query = string.Format(@"
                    select Date as ResumeProcessDate, FullName as ExpertCVName,  CandidateTimeLine.Status  from CandidateTimeLine  
                    inner join UserAccountDetails on UserId = ChangedBy
                    where CandidateTimeLine.Status like '%Resume Verification%' and CandidateTimeLine.RefCandidateId = {0}
                    order by Date", id);

                sqlObj.adpt = new System.Data.SqlClient.SqlDataAdapter(query, sqlObj.conn);
                sqlObj.dt = new System.Data.DataTable();
                sqlObj.adpt.Fill(sqlObj.dt);

                if (sqlObj.dt.Rows.Count == 1)
                {
                    DataRow row = sqlObj.dt.Rows[0];
                    //candidate.ExpertCVMakerName = row["ExpertCVName"].ToString();
                    candidate.ResumeProcessEnded = DateTime.Parse(row["ResumeProcessDate"].ToString());
                    if (candidate.ResumeProcessStarted != null)
                    {
                        DateTime rsd = (DateTime)candidate.ResumeProcessStarted;
                        DateTime red = (DateTime)candidate.ResumeProcessEnded;
                        candidate.TotalResumeDays = red.Subtract(rsd).Days;
                    }
                }
                else
                {
                    if (candidate.ResumeProcessStarted != null)
                    {
                        DateTime rsd = (DateTime)candidate.ResumeProcessStarted;
                        candidate.TotalResumeDays = cd.Subtract(rsd).Days;
                    }
                }

                query = string.Format(@"
                        select CatM.TaskCatTitle, Expert.FullName as TrainerName, tmng.TaskDate
                        from CandidateTechnicalExpertDetails
                        inner join CandidateMaster on CandidateMaster.CandidateId = RefAssignedCandidateId
                        inner join UserAccountDetails as Expert on RefAssignedExpertId = Expert.UserId
                        inner join TaskManageMaster as tmng on  tmng.RefCTId = CandidateTechnicalExpertDetails.CTId
                        left join CandidateBatchDetails on  CandidateBatchDetails.BatchId = RefBatchId 
                        left join TaskCategoryMaster as CatM on CatM.TaskCatId = tmng.RefTaskCatId
                        where  CandidateMaster.CandidateId = {0} and RefTaskCatId = 2
                        order by tmng.TaskDate desc", id);

                sqlObj.adpt = new System.Data.SqlClient.SqlDataAdapter(query, sqlObj.conn);
                sqlObj.dt = new System.Data.DataTable();
                sqlObj.adpt.Fill(sqlObj.dt);

                if (sqlObj.dt.Rows.Count >= 1)
                {
                    DataRow row = sqlObj.dt.Rows[0];
                    candidate.ResumeUnderstandingBy = row["TrainerName"].ToString();
                    candidate.ResumeUnderstandingDate = DateTime.Parse(row["TaskDate"].ToString());
                }

                query = string.Format(@"
                        select Expert.FullName as TrainerName, CandidateBatchDetails.BatchStartDate as BSD,
                        CandidateBatchDetails.BatchEndDate as BED
                        from CandidateTechnicalExpertDetails
                        inner join CandidateMaster on CandidateMaster.CandidateId = RefAssignedCandidateId
                        inner join UserAccountDetails as Expert on RefAssignedExpertId = Expert.UserId
                        left join CandidateBatchDetails on  CandidateBatchDetails.BatchId = RefBatchId 
                        where CandidateMaster.CandidateId = {0} order by CandidateBatchDetails.BatchStartDate desc", id);


                sqlObj.adpt = new System.Data.SqlClient.SqlDataAdapter(query, sqlObj.conn);
                sqlObj.dt = new System.Data.DataTable();
                sqlObj.adpt.Fill(sqlObj.dt);

                if (sqlObj.dt.Rows.Count >= 1)
                {
                    DataRow row = sqlObj.dt.Rows[0];
                    candidate.TrainerName = row["TrainerName"].ToString();
                    if (!DBNull.Value.Equals(row["BSD"]))
                    {
                        candidate.TrainingStartDate = DateTime.Parse(row["BSD"].ToString());
                        DateTime td = (DateTime)candidate.TrainingStartDate;
                        candidate.TotalTrainingDays = cd.Subtract(td).Days;
                    }
                    if (!DBNull.Value.Equals(row["BED"]))
                    {
                        candidate.TrainingEndDate = DateTime.Parse(row["BED"].ToString());
                        DateTime td = (DateTime)candidate.TrainingStartDate;
                        DateTime ed = (DateTime)candidate.TrainingEndDate;
                        candidate.TotalTrainingDays = ed.Subtract(td).Days;
                    }
                }

                query = string.Format(@"
                        select FullName as RecruiterName, LocationName from CandidateMaster 
                        inner join CandidateMarketingDetails on CandidateId = RefCandidateId
                        inner join CandidateAssign on refMarketingId = MarketingId
                        inner join TeamDetails on RefTeamId = TeamId
                        inner join UserAccountDetails on UserId = Member
                        inner join LocationMaster on LocationId = RefLocationId
                        where  CandidateAssign.IsActive = 1 and CandidateId = {0}", id);


                sqlObj.adpt = new System.Data.SqlClient.SqlDataAdapter(query, sqlObj.conn);
                sqlObj.dt = new System.Data.DataTable();
                sqlObj.adpt.Fill(sqlObj.dt);

                if (sqlObj.dt.Rows.Count >= 1)
                {
                    DataRow row = sqlObj.dt.Rows[0];
                    candidate.RecruiterName = row["RecruiterName"].ToString();
                    candidate.LocationName = row["LocationName"].ToString();
                }

            }
            if (candidate.LocationName == "")
            {
                query = string.Format(@"
                        select LocationName from CandidateMaster
                        inner join UserAccountDetails on UserId = RefSalesAssociate
                        inner join LocationMaster on LocationId = RefLocationId
                        where  CandidateId = {0}", id);

                sqlObj.adpt = new System.Data.SqlClient.SqlDataAdapter(query, sqlObj.conn);
                sqlObj.dt = new System.Data.DataTable();
                sqlObj.adpt.Fill(sqlObj.dt);

                if (sqlObj.dt.Rows.Count >= 1)
                { }
            }
            return candidate;
        }

        public List<CandidateDetailBoardModel> GetDetails(int? refLocationId = null)
        {
            List<CandidateDetailBoardModel> candidateDetails = new List<CandidateDetailBoardModel>();
            dashTech_crm_Entities db = new dashTech_crm_Entities();
            List<CandidateMaster> list = new List<CandidateMaster>();
            if (refLocationId != null)
            {
                list = db.CandidateMasters.Where(a => a.CandidateStatus != "Deleted" && a.UserAccountDetail.RefLocationId == refLocationId).ToList();
            }
            else
            {
                list = db.CandidateMasters.Where(a => a.CandidateStatus != "Deleted").ToList();
            }
            for (int i = 0; i < list.Count; i++)
            {
                CandidateDetailBoardModel m = new CandidateDetailBoardModel();

                m = GetCandidate(list[i].CandidateId);
                m.ResumeUnderstandingBy = string.Format(@"St.{0} <br> Ed.{1} <br>by {2}", m.ResumeProcessEnded == null ? "N/A" : m.ResumeProcessEnded.ToString(), m.ResumeUnderstandingDate, m.ResumeUnderstandingBy);
                if (m.CandidateName == "" || m.CandidateName.ToLower().Contains("other")) { continue; };
                candidateDetails.Add(m);
            }

            return candidateDetails;
        }
    }
}
