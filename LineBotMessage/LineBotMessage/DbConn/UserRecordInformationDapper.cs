using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using System.Threading.Tasks;
using LineBotMessage.Dtos;

namespace LineBotMessage.DbConn
{
    public class UserRecordInformationDapper
    {
        public static NpgsqlConnection OpenConnection(string connstr)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connstr);
            conn.Open();
            return conn;
        }

        public  IList<UserRecord> Load(string _connStr)
        {
            IList<UserRecord> list;

            using (var conn = OpenConnection(_connStr))
            {
                try
                {
                    string querySQL = "SELECT*FROM UserRecord WHERE 1=1 AND Id=@Id";
                    list = conn.Query<UserRecord>(querySQL, new { Id = 930030 }).ToList();
                    return list;
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Load參數錯誤");
                    return null;
                }
            }

        }
    }
}
