using Dapper;
using LineBotMessage.Dtos;
using LineBotMessage.Dtos.Webhook;
using Npgsql;

namespace LineBotMessage.DbConn
{
    public class AiRecordInformationDapper
    {
        private string connString = "Host=soulkeydb.internal;Port=5432;Username=postgres;Password=xwOCnnjArOaAnBZ;Database=runoobdb";
        public static NpgsqlConnection OpenConnection(string connstr)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connstr);
            conn.Open();
            return conn;
        }
        public List<Aimodel> Load(Aimodel record)
        {
            List<Aimodel> list;

            using (var conn = OpenConnection(connString))
            {
                try
                {
                    string querySQL = "SELECT *FROM airecord where userid=@userid ORDER By createtime desc LIMIT 1";
                    list = conn.Query<Aimodel>(querySQL, record).ToList();
                    //Console.WriteLine($"Load 成功，讀取id:{record.userid}");
                    return list;
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("Load 出現錯誤將會回傳空的model：" + ex.Message);
                    return null;
                }
            }
        }
        public bool Create(Aimodel record)
        {
            Aimodel list;

            using (var conn = OpenConnection(connString))
            {
                try
                {
                    string insertSQL = @"INSERT INTO airecord(userid, prompt,createtime,ongoingtime,iscontinue) 
                                VALUES(@userid, @prompt, @createtime, @ongoingtime, @iscontinue)";
                    conn.Execute(insertSQL, record);
                    //Console.WriteLine($"Create 成功，新增id:{record.userid}");
                    return true;
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("Create 出現錯誤：" + ex.Message);
                    return false;
                }
            }
        }

        public bool Update(Aimodel record)
        {
            using (var conn = OpenConnection(connString))
            {
                try
                {
                    string updateSQL = @"UPDATE airecord SET prompt=@prompt, 
                                createtime=@createtime,ongoingtime=@ongoingtime,iscontinue=@iscontinue WHERE userid=@userid";
                    conn.Execute(updateSQL, record);
                    //Console.WriteLine($"Update 成功，更新id:{record.userid}");
                    return true;
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("Update 出現錯誤：" + ex.Message);
                    return false;
                }
            }
        }
        public bool Delete(Aimodel record)
        {
            using (var conn = OpenConnection(connString))
            {
                try
                {
                    string deleteSQL = "DELETE FROM airecord  where userid=@userid";
                    conn.Execute(deleteSQL, record);
                    //Console.WriteLine($"Delete 成功，刪除id:{record.userid}");
                    return true;
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("Delete 出現錯誤：" + ex.Message);
                    return false;
                }
            }
        }
        public bool DeleteALL()
        {
            using (var conn = OpenConnection(connString))
            {
                try
                {
                    string deleteSQL = "DELETE FROM airecord";
                    conn.Execute(deleteSQL);
                    //Console.WriteLine($"Delete 成功，已刪除全部資料");
                    return true;
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("Delete 出現錯誤：" + ex.Message);
                    return false;
                }
            }
        }

    }
}
