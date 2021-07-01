using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DashTechCRM.Models.User
{
    public class UserLogic
    {
        public UserObject LoginProcess(string emailid, string password)
        {
            UserDataAccess userDataAccess = new UserDataAccess();
            UserObject user = new UserObject() { EmailId = emailid, Password = password };
                DataTable dt = userDataAccess.LoginUser(user);

            if (dt.Rows.Count == 1)
            {
                DataRow dr = dt.Rows[0];
                user.UserId = int.Parse(dr["UserId"].ToString());
                user.RocketName = (dr["RocketName"].ToString());
                user.UserImageUrl = (dr["RocketName"].ToString());
                user.Role = (dr["RoleTitle"].ToString());
                return user;
            }
            return null;
        }
    }
}