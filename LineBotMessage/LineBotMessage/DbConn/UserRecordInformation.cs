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

                if (dr["Issue"] == DBNull.Value)
                    this._Issue = null;
                else
                    this._Issue = Convert.ToString(dr["Issue"]);
                if (dr["Time"] == DBNull.Value)
                    this._Time = null;
                else
                    this._Time = Convert.ToDateTime(dr["Time"]);

                if (dr["Step"] == DBNull.Value)
                    this._Step = null;
                else
                    this._Step = Convert.ToString(dr["Step"]);

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
            this._Step = "";
        }
        #endregion

        #region Private Properties
        private int _Id;
        private String _Issue;
        private DateTime? _Time;
        private string _Step = "";
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
        public string Step
        {
            get { return _Step; }
            set { _Step = value; }
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
                            Id = Convert.ToInt32(db.Select()[0]["id"]);
                            Issue = db.Select()[0]["Issue"].ToString();
                            Time = Convert.ToDateTime(db.Select()[0]["Time"]);
                            Step = db.Select()[0]["Step"].ToString();

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
        public bool Create()
        {
            DataTable db = new DataTable();
            StringBuilder sbCmd = new StringBuilder();
            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    Console.WriteLine("--------------------連線成功--------------------");
                    using (var cmd = new NpgsqlCommand("INSERT INTO UserRecord(Id,Issue,Time,Step)VALUES(@Id,@Issue,NOW(),@Step);", conn))
                    {
                        cmd.Parameters.AddWithValue("Id", Id);
                        cmd.Parameters.AddWithValue("Issue", Issue);
                        cmd.Parameters.AddWithValue("Step", Step);
                        cmd.Connection= conn;
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
        public bool Update()
        {
            DataTable db = new DataTable();
            StringBuilder sbCmd = new StringBuilder();
            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    Console.WriteLine("--------------------連線成功--------------------");
                    using (var cmd = new NpgsqlCommand("UPDATE UserRecord SET Issue = @Issue , Step = @Step WHERE Id = @Id;", conn))
                    {
                        cmd.Parameters.AddWithValue("Id", Id);
                        cmd.Parameters.AddWithValue("Issue", Issue);
                        cmd.Parameters.AddWithValue("Step", Step);
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
        public void Save()
        {
            if (!string.IsNullOrWhiteSpace(Id.ToString()))
            {
                this.Update();
            }
            else
            {
                this.Create();
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
