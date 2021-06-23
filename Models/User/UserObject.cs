using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DashTechCRM.Models.User
{
    public class UserObject
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string RocketName { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }
        public string LocationName { get; set; }
        public string Role { get; set; }
        public string UserImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastLogin { get; set; }
        public string CompanyName { get; set; }
        public DateTime JoiningDate { get; set; }
        public string DefUrl { get; set; }
    }
}