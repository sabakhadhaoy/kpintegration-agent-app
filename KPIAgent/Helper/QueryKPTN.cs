using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPIAgent.Helper
{
    class QueryKPTN
    {
        Logger _logger = new Logger();
        ReadINI _read = new ReadINI();
        public string getKPTN(string tableName, string transDate, string empID, string branchcode)
        {
            string server = _read.QCLConfig().Server;
            string user = _read.QCLConfig().User;
            string password = _read.QCLConfig().Password;

            int empcode = Convert.ToInt32(empID.Substring(4));

            string constring = "server = " + server + ";database = " + branchcode + ";uid = " + user + ";password= " + password + "; pooling=false;min pool size=0;max pool size=1000;connection lifetime=0; connection timeout=360000";

            List<string> kptnList = new List<string>();

            try
            {
                using (SqlConnection con = new SqlConnection(constring))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "select KPTN from " + tableName + " where usr_id = @empcode and Transdate = @transDate";
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@empcode", empcode);
                        cmd.Parameters.AddWithValue("@transDate", transDate);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                while (dr.Read())
                                {
                                    kptnList.Add(dr["KPTN"].ToString());
                                }

                                string kptn = string.Join(",", kptnList.ToArray());

                                return kptn;
                            }
                            else
                            {
                                return "0";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                return "0";
            }
        }
        public bool InsertKPTN(string tablename, string kptn, string transdate, string empID, string branchcode, SqlCommand cmd)
        {
            int empcode = Convert.ToInt32(empID.Substring(4));


            try
            {
                cmd.CommandText = "insert into " + tablename + "(usr_id,KPTN,Transdate) values(@empcode, @kptn, @transdate)";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@empcode", empcode);
                cmd.Parameters.AddWithValue("@kptn", kptn);
                cmd.Parameters.AddWithValue("@transdate", transdate);

                cmd.ExecuteNonQuery();

                _logger.Info("Insert Success - Tablename:" + tablename + " - Employeecode: " + empcode + " - KPTN: " + kptn + " - Date: " + transdate);
                return true;

            }
            catch (Exception ex)
            {
                _logger.Error("Insert Failed - Tablename:" + tablename + " - Employeecode: " + empcode + " - KPTN: " + kptn + " - Date: " + transdate + " - Error: " + ex.ToString());
                return false;
            }
        }
    }
}
