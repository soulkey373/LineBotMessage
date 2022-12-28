using Microsoft.AspNetCore.Mvc.Formatters;
using Npgsql;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace LineBotMessage.DbConn
{
    public class UserRecordInformation
    {

        public UserRecordInformation()
        {
            this.Init();
        }

        public UserRecordInformation(DataRow dr)
        {
            try
            {
                this._Id = Convert.ToInt32(dr["Id"]);

                this._Issue = Convert.ToString(dr["Issue"]);

                if (dr["Time"] == DBNull.Value)
                    this._Time = null;
                else
                    this._Time = Convert.ToDateTime(dr["Time"]);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #region Init
        private void Init()
        {
            this._Id = 0;
            this._Issue = "";
            this._Time = null;
        }
        #endregion

        #region Private Properties
        private int _Id;
        private String _Issue;
        private DateTime? _Time;
        #endregion

        #region Public Properties
        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        public string Issue
        {
            get { return _Issue; }
            set { _Issue = value; }
        }
        public DateTime? Time
        {
            get { return _Time; }
            set { _Time = value; }
        }

        #endregion

        #region Method
        private string connString = "Host=soulkeydb.internal;Port=5432;Username=postgres;Password=xwOCnnjArOaAnBZ;Database=runoobdb";
        public bool Load()
        {
            DataTable db = new DataTable();
            StringBuilder sbCmd = new StringBuilder();
            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    Console.WriteLine("--------------------連線成功--------------------");
                    using (var cmd = new NpgsqlCommand("SELECT*FROM UserRecord", conn))
                    {
                        using (NpgsqlDataReader? reader = cmd.ExecuteReader())
                        {

                            db = ConvertToDataTable(reader);
                            Console.WriteLine("目前UserRecord裡有{0}筆資料", db.Rows.Count);
                            _Id = Convert.ToInt32(db.Select()[0]["id"]);
                            _Issue = db.Select()[0]["Issue"].ToString();
                            _Time = Convert.ToDateTime(db.Select()[0]["Time"]);

                        }
                    }
                    conn.Close();
                    Console.WriteLine("--------------------連線關閉--------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("--------------------讀取資料庫失敗--------------------", ex.ToString());
            }


            return true;
        }

        public bool Create(int Id, string Issue)
        {
            DataTable db = new DataTable();
            StringBuilder sbCmd = new StringBuilder();
            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    Console.WriteLine("--------------------連線成功--------------------");
                    using (var cmd = new NpgsqlCommand("INSERT INTO UserRecord(Id,Issue,Time)VALUES(@Id, @Issue,NOW());", conn))
                    {
                        cmd.Parameters.AddWithValue("Id", Id);
                        cmd.Parameters.AddWithValue("Issue", Issue);

                        int nRows = cmd.ExecuteNonQuery();
                        Console.Out.WriteLine(String.Format("新增的行數:{0}筆", nRows));
                    }
                    conn.Close();
                    Console.WriteLine("--------------------連線關閉--------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--------------------新增資料進資料庫失敗--------------------\n{ex.ToString()}");
            }


            return true;
        }

        public bool Update(int Id, string Issue)
        {
            DataTable db = new DataTable();
            StringBuilder sbCmd = new StringBuilder();
            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    Console.WriteLine("--------------------連線成功--------------------");
                    using (var cmd = new NpgsqlCommand("UPDATE UserRecord SET Issue=@Issue WHERE Id=@Id;", conn))
                    {
                        cmd.Parameters.AddWithValue("Id", Id);
                        cmd.Parameters.AddWithValue("Issue", Issue);

                        int nRows = cmd.ExecuteNonQuery();
                        Console.Out.WriteLine(String.Format("更新的行數:{0}筆", nRows));
                    }
                    conn.Close();
                    Console.WriteLine("--------------------連線關閉--------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--------------------更新資料資料庫失敗--------------------\n{ex.ToString()}");
            }

            return true;
        }
        public bool Delete()
        {
            DataTable db = new DataTable();
            StringBuilder sbCmd = new StringBuilder();
            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    Console.WriteLine("--------------------連線成功--------------------");
                    using (var cmd = new NpgsqlCommand("DELETE FROM UserRecord", conn))
                    {
                        int nRows = cmd.ExecuteNonQuery();
                        Console.Out.WriteLine(String.Format("目前表裡剩餘的行數:{0}筆", nRows));
                    }
                    conn.Close();
                    Console.WriteLine("--------------------連線關閉--------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--------------------刪除資料資料庫失敗--------------------\n{ex.ToString()}");
            }

            return true;
        }
        #endregion

        #region 連postgres範例
        public void getPostgresDate()
        {
           
            DataTable dt = new DataTable();
            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    Console.WriteLine("--------------------連線成功--------------------");
                    using (var cmd = new NpgsqlCommand("SELECT*FROM cars", conn))
                    {
                        using (NpgsqlDataReader? reader = cmd.ExecuteReader())
                        {

                            dt = ConvertToDataTable(reader);
                            string dtId = "";
                            string dtName = "";
                            string dtPrice = "";
                            dtId = dt.Select()[0]["id"].ToString();
                            dtName = dt.Select()[0]["name"].ToString();
                            dtPrice = dt.Select()[0]["price"].ToString();
                            Console.WriteLine($"Id = {dtId}\nName = {dtName}\nPrice = {dtPrice}");
                        }
                    }
                }
                Console.WriteLine("--------------------連線關閉--------------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine("連線失敗", ex.ToString());
            }
        }
        #endregion

        #region Commom
        public static DataTable? ConvertToDataTable(NpgsqlDataReader dataReader)
        {

            try
            {
                DataTable dataTable = new DataTable();
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    DataColumn column = new DataColumn();
                    column.DataType = dataReader.GetFieldType(i);
                    column.ColumnName = dataReader.GetName(i);
                    dataTable.Columns.Add(column);
                }

                while (dataReader.Read())
                {
                    DataRow row = dataTable.NewRow();
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        row[i] = dataReader[i].ToString();
                    }
                    dataTable.Rows.Add(row);
                    row = null;
                }
                dataReader.Close();
                return dataTable;
            }
            catch (Exception e)
            {
                Console.WriteLine("解析DataReader錯誤", e.ToString());
                return null;
            }

        }
        #endregion

    }
}
