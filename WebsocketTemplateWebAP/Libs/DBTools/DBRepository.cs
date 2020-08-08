using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebsocketTemplateWebAP.Models;

namespace WebsocketTemplateWebAP.Libs.DBTools
{
    public class DBRepository
    {
        private SqlConnection _sqlConnection { get; set; }
        public DBRepository()
        {
            _sqlConnection = new SqlConnection(DBTool.ConnectionString);
        }

        public UserInfo UserLoginLogic(string userName, string password)
        {
            UserInfo userInfo = null;
            var results = _sqlConnection.Query("dbo.NewUserLogin", new { P1 = userName, P2 = password},
                    commandType: CommandType.StoredProcedure);

            foreach (dynamic item in results)
            {
                var row = item as IDictionary<string, object>;
                userInfo = new UserInfo
                {
                    UserName = row["UserName"].ToString(),
                    Token = row["Token"].ToString(),
                    ActionTime = DateTime.Now
                };
                
            }

            return userInfo;
        }

        public void UserLogoutSP(string userName)
        {
            var results = _sqlConnection.Query("dbo.UserLogout", new { P1 = userName }, commandType: CommandType.StoredProcedure);
        }
    }
}
