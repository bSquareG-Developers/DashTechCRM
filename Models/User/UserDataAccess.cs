using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DashTechCRM.Models.User
{
    public class UserDataAccess
    {
        dbConnectionModel dbSql = new dbConnectionModel();
        public DataTable LoginUser(UserObject user)
        {
            string query = string.Format(@"Select RoleTitle, UserId, RocketName, EmailId, UserImageUrl from UserAccountDetails inner join RoleMaster on RoleId = RefRoleId where EmailId = '{0}' and Password = '{1}'", user.EmailId, user.Password);
            dbSql.query = query;
            DataTable dt = dbSql.runQuery();
            return dt;
        }
    }
}