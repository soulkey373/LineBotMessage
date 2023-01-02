using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using System.Threading.Tasks;
using LineBotMessage.Dtos;
using Microsoft.Build.Tasks;

namespace LineBotMessage.DbConn
{
    public class UserRecordInformationDapper
    {
        enum ActionType { CREATE, UPDATE };
        public static NpgsqlConnection OpenConnection(string connstr)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connstr);
            conn.Open();
            return conn;
        }

        public IList<UserRecord> Load(string _connStr, int id)
        {
            IList<UserRecord> list;

            using (var conn = OpenConnection(_connStr))
            {
                try
                {
                    string querySQL = "SELECT*FROM UserRecord WHERE 1=1 AND Id=@Id";
                    list = conn.Query<UserRecord>(querySQL, new { Id = id }).ToList();
                    return list;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Load 出現錯誤：" + ex.Message);
                    return null;
                }
            }
        }
        public bool Create(string _connStr, UserRecord record)
        {
            IList<UserRecord> list;

            using (var conn = OpenConnection(_connStr))
            {
                try
                {
                    string insertSQL = @"INSERT INTO UserRecord(Id, MealType, FoodType, Lat, Lon, Step, Time) 
                                VALUES(@Id, @MealType, @FoodType, @Lat, @Lon, @Step, @Time)";
                    conn.Execute(insertSQL, record);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Create 出現錯誤：" + ex.Message);
                    return false;
                }
            }
        }

        public bool Update(string _connStr, UserRecord record)
        {
            using (var conn = OpenConnection(_connStr))
            {
                try
                {
                    string updateSQL = @"UPDATE UserRecord SET MealType=@MealType, FoodType=@FoodType, 
                                Lat=@Lat, Lon=@Lon, Step=@Step, Time=@Time WHERE Id=@Id";
                    conn.Execute(updateSQL, record);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Update 出現錯誤：" + ex.Message);
                    return false;
                }
            }
        }
        public bool Delete(string _connStr, int id)
        {
            using (var conn = OpenConnection(_connStr))
            {
                try
                {
                    string deleteSQL = "DELETE FROM UserRecord WHERE Id=@Id";
                    conn.Execute(deleteSQL, new { Id = id });
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Delete 出現錯誤：" + ex.Message);
                    return false;
                }
            }
        }
    }
}
