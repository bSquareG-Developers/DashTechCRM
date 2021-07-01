using System.Collections.Generic;

namespace DashTechCRM.Models
{
    public class CandidateStatusModel
    {
        public CandidateStatusModel()
        {
            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            valuePairs.Add("Deleted", "DLT");
            valuePairs.Add("Dropped", "DRP");
            valuePairs.Add("Expert", "EXP");
            valuePairs.Add("In Marketing", "MKT");
            valuePairs.Add("In Training", "TRN");
            valuePairs.Add("Technical Query", "TQC");
            valuePairs.Add("Not Responding To Resume Team", "NRR");
            valuePairs.Add("On Hold", "ONH");
            valuePairs.Add("Placed", "PLD");
            valuePairs.Add("Resume Preparation", "RSP");
            valuePairs.Add("Resume Verification", "RSV");
            valuePairs.Add("RUC Done", "RUD");
            valuePairs.Add("RUC Pending", "RUP");
            valuePairs.Add("Sales'", "SAL");


            Dictionary<string, string> valuePairsFollowUp = new Dictionary<string, string>();
            valuePairs.Add("Dispute Amount", "Dispute");
            //valuePairs.Add("", "");
            valuePairs.Add("Expert", "EXP");
            valuePairs.Add("In Marketing", "MKT");
            valuePairs.Add("In Training", "TRN");
            valuePairs.Add("Technical Query", "TQC");
            valuePairs.Add("Not Responding To Resume Team", "NRR");
            valuePairs.Add("On Hold", "ONH");
            valuePairs.Add("Placed", "PLD");
            valuePairs.Add("Resume Preparation", "RSP");
            valuePairs.Add("Resume Verification", "RSV");
            valuePairs.Add("RUC Done", "RUD");
            valuePairs.Add("RUC Pending", "RUP");
            valuePairs.Add("Sales'", "SAL");

        }
        public static List<Dictionary<string, string>> status = new List<Dictionary<string, string>>();


        public static List<Dictionary<string, string>> followupStatus = new List<Dictionary<string, string>>();
        public static string GetShortCode(string status)
        {
            switch (status)
            {

                case "Deleted": return "<span style='padding:5px ; width:80px; ' class='badge badge-primary' style='font-size:11px'>DLT</span>";
                case "Dropped": return "<span style='padding:5px ; width:80px; ' class='badge badge-primary' style='font-size:11px'>DRP</span>";
                case "Expert": return "<span style='padding:5px ; width:80px; ' class='badge badge-info' style='font-size:11px'>EXP</span>";
                case "Expret:Resume Call Done": return "<span style='padding:5px ; width:80px; ' class='badge badge-info' style='font-size:11px'>EXPCALL</span>";
                case "Expret:Resume Preperation": return "<span style='padding:5px ; width:80px; ' class='badge badge-info' style='font-size:11px'>EXPPRE</span>";
                case "In Marketing": return "<span style='padding:5px ; width:80px; ' class='badge badge-success' style='font-size:11px'>MKT</span>";
                case "In Training": return "<span style='padding:5px ; width:80px; ' class='badge badge-light' style='font-size:11px'>TRN</span>";
                case "Not Responding To Resume Team": return "<span style='padding:5px ; width:80px; ' class='badge badge-info' style='font-size:11px'>EXPNOT</span>";
                case "Expert:Not Responding To Resume Team": return "<span style='padding:5px ; width:80px; ' class='badge badge-info' style='font-size:11px'>EXPNOT</span>";
                case "On Hold": return "<span style='padding:5px ; width:80px; ' class='badge badge-warning' style='font-size:11px'>ONH</span>";
                case "Placed": return "<i class='badge badge-dark' style='font-size:11px;padding:5px ; width:80px'>PLD</i>";
                case "Resume Preparation": return "<span style='padding:5px ; width:80px; ' class='badge badge-info' style='font-size:11px'>CVPRE</span>";
                case "Resume Verification": return "<span style='padding:5px ; width:80px; ' class='badge badge-info' style='font-size:11px'>CVVER</span>";
                case "Expert:Resume Preparation": return "<span style='padding:5px ; width:80px; ' class='badge badge-info' style='font-size:11px'>CVPRE</span>";
                case "Expert:Resume Verification": return "<span style='padding:5px ; width:80px; ' class='badge badge-info' style='font-size:11px'>CVVER</span>";
                case "RUC Done": return "<span style='padding:5px ; width:80px; ' class='badge badge-info' style='font-size:11px'>RUCD</span>";
                case "RUC Pending": return "<span style='padding:5px ; width:80px; ' class='badge badge-info' style='font-size:11px'>RUCP</span>";
                case "Expert:RUC Pending": return "<span style='padding:5px ; width:80px; ' class='badge badge-info' style='font-size:11px'>RUCP</span>";
                case "Sales": return "<span style='padding:5px ; width:80px; ' class='badge badge-primary' style='font-size:11px'>SAL</span>";
                case "Expert:Start Marketing": return "<span style='padding:5px ; width:80px; ' class='badge badge-primary' style='font-size:11px'>STRMRK</span>";
                default: return status;

            }
        }
        //public static string GetShortCode(string status)
        //{
        //    switch (status)
        //    {

        //        case "Deleted": return "DLT";
        //        case "Dropped": return "DRP";
        //        case "Expert": return "EXP";
        //        case "Expret:Resume Call Done": return "EXPCALL";
        //        case "Expret:Resume Preperation": return "EXPPRE";
        //        case "In Marketing": return "MKT";
        //        case "In Training": return "TRN";
        //        case "Not Responding To Resume Team": return "EXPNOT";
        //        case "Expert:Not Responding To Resume Team": return "EXPNOT";
        //        case "On Hold": return "ONH";
        //        case "Placed": return "PLD";
        //        case "Resume Preparation": return "CVPRE";
        //        case "Resume Verification": return "CVVER";
        //        case "Expert:Resume Preparation": return "CVPRE";
        //        case "Expert:Resume Verification": return "CVVER";
        //        case "RUC Done": return "RUCD";
        //        case "RUC Pending": return "RUCP";
        //        case "Expert:RUC Pending": return "RUCP";
        //        case "Sales": return "SAL";
        //        case "Expert:Start Marketing": return "STRMRK";
        //        default: return status;
        //    }
        //}
    }

}
