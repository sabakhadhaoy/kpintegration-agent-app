using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using KPIAgent.Helper;
using KPIAgent.Model;
using Newtonsoft.Json;

namespace KPIAgent.Controller
{
    class DomesticPayout
    {
        Logger _logger = new Logger();
        DatabaseQCL _qcl = new DatabaseQCL();
        QueryKPTN _query = new QueryKPTN();
        ReadINI _read = new ReadINI();

        Request _payoutRequest = new Request();

        private static string tableName = "KPDomestic_TransactionlogsKPTNPayout";

        public void GetPayoutDomestic(string bcode, int zcode, string transactionDate, List<Operators> _operator)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string url = _read.serviceUrl();

            try
            {
                foreach (var emp in _operator)
                {
                    int persist = 0;
                    while (persist < 2)
                    {
                        string kptn = _query.getKPTN(tableName, transactionDate, emp.userlogin, bcode);

                        UTF8Encoding enc = new UTF8Encoding();

                        _payoutRequest.kptn = kptn;
                        _payoutRequest.transdate = transactionDate;
                        _payoutRequest.bcode = bcode;
                        _payoutRequest.zcode = zcode;
                        _payoutRequest.employee = emp.userlogin;

                        string stringRequest = JsonConvert.SerializeObject(_payoutRequest);

                        byte[] postDataBytes = enc.GetBytes(stringRequest);

                        WebRequest request = WebRequest.Create(url + "/Domestic/Payout");
                        request.Method = "POST";
                        request.ContentType = "application/json";
                        request.ContentLength = postDataBytes.Length;

                        Stream stream = request.GetRequestStream();
                        stream.Write(postDataBytes, 0, postDataBytes.Length);
                        stream.Close();

                        WebResponse result = request.GetResponse();
                        Stream response = result.GetResponseStream();
                        StreamReader reader = new StreamReader(response);
                        string responseFromService = reader.ReadToEnd();
                        reader.Close();
                        response.Close();

                        var serviceResponse = JsonConvert.DeserializeObject<DomesticPayoutResponse>(responseFromService);

                        if (serviceResponse.respcode == 0)
                        {
                            FetchPayoutDomestic(serviceResponse.respdata, transactionDate, emp.userlogin, emp.fullname, bcode);
                        }

                        else if (serviceResponse.respcode == 1)
                        {
                            _logger.Info("Domestic [PAYOUT] " + serviceResponse.respmsg + " - KPTN: " + kptn + " - Transdate: " + transactionDate + " - BranchCode: " + bcode + " - ZoneCode: " + zcode + " - Operator: " + emp.userlogin);
                            persist++;
                        }

                        else if (serviceResponse.respcode == 2)
                        {
                            _logger.Error("Domestic [PAYOUT] " + serviceResponse.respmsg + " - KPTN: " + kptn + " - Transdate: " + transactionDate + " - BranchCode: " + bcode + " - ZoneCode: " + zcode + " - Operator: " + emp.userlogin);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
        }

        private void FetchPayoutDomestic(List<DomesticPayoutTrans> _payoutData, string transDate, string empID, string empFullname, string branchcode)
        {
            var _gl = _read.GLAccounts().GLAccounts;

            string server = _read.QCLConfig().Server;
            string user = _read.QCLConfig().User;
            string password = _read.QCLConfig().Password;

            string constring = "server = " + server + ";database = " + branchcode + ";uid = " + user + ";password= " + password + "; pooling=false;min pool size=0;max pool size=1000;connection lifetime=0; connection timeout=360000";

            double totalPrincipal = 0;
            double totalQuantity = _payoutData.Count();

            bool savePOPrincipal = false;

            List<string> kptnList = new List<string>();

            try
            {
                foreach (var payoutData in _payoutData)
                {
                    if (payoutData.domestic_principal != 0.0)
                    {
                        totalPrincipal += payoutData.domestic_principal;
                    }
                    kptnList.Add(payoutData.kptn);
                }

                string kptn = string.Join(",", kptnList.ToArray());

                using (SqlConnection con = new SqlConnection(constring))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        SqlTransaction trans = con.BeginTransaction(IsolationLevel.ReadCommitted);
                        cmd.Transaction = trans;

                        savePOPrincipal = _qcl.SaveTransaction(_gl.Domestic_PHP_Journal, _gl.Domestic_PHP_TellerGL, _gl.Domestic_PHP_PayoutPrincipalGL, totalPrincipal, 0.0, totalQuantity, "KP Payout Principal", "KPSOPRDOM1234", "PHP", transDate, empID, empFullname, branchcode, cmd);

                        if (savePOPrincipal)
                        {
                            bool insertSuccess = _query.InsertKPTN(tableName, kptn, transDate, empID, branchcode, cmd);

                            if (insertSuccess)
                            {
                                trans.Commit();
                            }
                            else
                            {
                                trans.Rollback();
                            }
                        }
                        else
                        {
                            trans.Rollback();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
        }
    }
}
