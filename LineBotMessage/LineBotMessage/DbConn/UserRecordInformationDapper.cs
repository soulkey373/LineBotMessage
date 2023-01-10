﻿using Npgsql;
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
        private string connString = "Host=soulkeydb.internal;Port=5432;Username=postgres;Password=xwOCnnjArOaAnBZ;Database=runoobdb";
        public static NpgsqlConnection OpenConnection(string connstr)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connstr);
            conn.Open();
            return conn;
        }

        public IList<UserRecord> Load(int id)
        {
            IList<UserRecord> list;

            using (var conn = OpenConnection(connString))
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
        public bool Create( UserRecord record)
        {
            IList<UserRecord> list;

            using (var conn = OpenConnection(connString))
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
                finally
                { 
                    conn.Close();
                    Console.WriteLine("紀錄meal成功");
                }


            }
        }

        public bool Update( UserRecord record)
        {
            using (var conn = OpenConnection(connString))
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
        public bool Delete()
        {
            using (var conn = OpenConnection(connString))
            {
                try
                {
                    string deleteSQL = "DELETE FROM UserRecord ";
                    conn.Execute(deleteSQL);
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
