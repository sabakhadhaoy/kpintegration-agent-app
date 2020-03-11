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
    class DomesticPayoutCancel
    {
        Logger _logger = new Logger();
        DatabaseQCL _qcl = new DatabaseQCL();
        QueryKPTN _query = new QueryKPTN();
        ReadINI _read = new ReadINI();

        Request _payoutCancelRequest = new Request();

        private static string tableName = "KPDomestic_TransactionlogsKPTNPayoutCancel";

        public void GetPayoutCancelDomestic(string bcode, int zcode, string transactionDate, List<Operators> _operator)
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

                        _payoutCancelRequest.kptn = kptn;
                        _payoutCancelRequest.transdate = transactionDate;
                        _payoutCancelRequest.bcode = bcode;
                        _payoutCancelRequest.zcode = zcode;
                        _payoutCancelRequest.employee = emp.userlogin;

                        string stringRequest = JsonConvert.SerializeObject(_payoutCancelRequest);

                        byte[] postDataBytes = enc.GetBytes(stringRequest);

                        WebRequest request = WebRequest.Create(url + "/Domestic/PayoutCancel");
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

                        var serviceResponse = JsonConvert.DeserializeObject<DomesticPayoutCancelResponse>(responseFromService);

                        if (serviceResponse.respcode == 0)
                        {
                            FetchPayoutCancelDomestic(serviceResponse.respdata, transactionDate, emp.userlogin, emp.fullname, bcode);
                        }

                        else if (serviceResponse.respcode == 1)
                        {
                            _logger.Info("Domestic [PAYOUT CANCEL] " + serviceResponse.respmsg + " - KPTN: " + kptn + " - Transdate: " + transactionDate + " - BranchCode: " + bcode + " - ZoneCode: " + zcode + " - Operator: " + emp.userlogin);
                            persist++;
                        }

                        else if (serviceResponse.respcode == 2)
                        {
                            _logger.Error("Domestic [PAYOUT CANCEL] " + serviceResponse.respmsg + " - KPTN: " + kptn + " - Transdate: " + transactionDate + " - BranchCode: " + bcode + " - ZoneCode: " + zcode + " - Operator: " + emp.userlogin);
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

        private void FetchPayoutCancelDomestic(List<DomesticPayoutCancelTrans> _payoutCancelData, string transDate, string empID, string empFullname, string branchcode)
        {
            var _gl = _read.GLAccounts().GLAccounts;

            string server = _read.QCLConfig().Server;
            string user = _read.QCLConfig().User;
            string password = _read.QCLConfig().Password;

            string constring = "server = " + server + ";database = " + branchcode + ";uid = " + user + ";password= " + password + "; pooling=false;min pool size=0;max pool size=1000;connection lifetime=0; connection timeout=360000";

            double totalPrincipal = 0;
            double totalQuantity = _payoutCancelData.Count();

            bool savePOPrincipal = false;

            List<string> kptnList = new List<string>();

            try
            {
                foreach (var payoutCancelData in _payoutCancelData)
                {

                    if (payoutCancelData.domestic_wrongpayout_principal != 0.0)
                    {
                        totalPrincipal += payoutCancelData.domestic_wrongpayout_principal;
                    }
                    kptnList.Add(payoutCancelData.kptn);
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
