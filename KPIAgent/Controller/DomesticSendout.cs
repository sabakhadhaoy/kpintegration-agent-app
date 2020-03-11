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
    class DomesticSendout
    {
        Logger _logger = new Logger();
        DatabaseQCL _qcl = new DatabaseQCL();
        QueryKPTN _query = new QueryKPTN();
        ReadINI _read = new ReadINI();

        Request _sendoutRequest = new Request();

        private static string tableName = "KPDomestic_TransactionlogsKPTNSendout";

        public void GetSendoutDomestic(string bcode, int zcode, string transactionDate, List<Operators> _operator)
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

                        _sendoutRequest.kptn = kptn;
                        _sendoutRequest.transdate = transactionDate;
                        _sendoutRequest.bcode = bcode;
                        _sendoutRequest.zcode = zcode;
                        _sendoutRequest.employee = emp.userlogin;

                        string stringRequest = JsonConvert.SerializeObject(_sendoutRequest);

                        byte[] postDataBytes = enc.GetBytes(stringRequest);

                        WebRequest request = WebRequest.Create(url + "/Domestic/Sendout");
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

                        var serviceResponse = JsonConvert.DeserializeObject<DomesticSendoutResponse>(responseFromService);

                        if (serviceResponse.respcode == 0)
                        {
                            FetchSendoutDomestic(serviceResponse.respdata, transactionDate, emp.userlogin, emp.fullname, bcode);
                        }

                        else if (serviceResponse.respcode == 1)
                        {
                            _logger.Info("Domestic [SENDOUT] " + serviceResponse.respmsg + " - KPTN: " + kptn + " - Transdate: " + transactionDate + " - BranchCode: " + bcode + " - ZoneCode: " + zcode + " - Operator: " + emp.userlogin);
                            persist++;
                        }

                        else if (serviceResponse.respcode == 2)
                        {
                            _logger.Error("Domestic [SENDOUT] " + serviceResponse.respmsg + " - KPTN: " + kptn + " - Transdate: " + transactionDate + " - BranchCode: " + bcode + " - ZoneCode: " + zcode + " - Operator: " + emp.userlogin);
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
        private void FetchSendoutDomestic(List<DomesticSendoutTrans> _sendoutData, string transDate, string empID, string empFullname, string branchcode)
        {
            var _gl = _read.GLAccounts().GLAccounts;

            string server = _read.QCLConfig().Server;
            string user = _read.QCLConfig().User;
            string password = _read.QCLConfig().Password;

            string constring = "server = " + server + ";database = " + branchcode + ";uid = " + user + ";password= " + password + "; pooling=false;min pool size=0;max pool size=1000;connection lifetime=0; connection timeout=360000";

            double totalPrincipal = 0;
            double totalCharge = 0;
            double totalQuantity = _sendoutData.Count();

            bool saveSOPrincipal = false;
            bool saveSOCharge = false;

            List<string> kptnList = new List<string>();

            try
            {
                foreach (var sendoutData in _sendoutData)
                {
                    if (sendoutData.domestic_principal != 0.0)
                    {
                        totalPrincipal += sendoutData.domestic_principal;
                    }
                    if (sendoutData.domestic_charge != 0.0)
                    {
                        totalCharge += sendoutData.domestic_charge;
                    }

                    kptnList.Add(sendoutData.kptn);
                }

                string kptn = string.Join(",", kptnList.ToArray());

                using (SqlConnection con = new SqlConnection(constring))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        SqlTransaction trans = con.BeginTransaction(IsolationLevel.ReadCommitted);
                        cmd.Transaction = trans;

                        saveSOPrincipal = _qcl.SaveTransaction(_gl.Domestic_PHP_Journal, _gl.Domestic_PHP_TellerGL, _gl.Domestic_PHP_SendoutPrincipalGL, totalPrincipal, 0.0, totalQuantity, "KP Sendout Principal", "KPSOPRDOM1234", "PHP", transDate, empID, empFullname, branchcode, cmd);

                        saveSOCharge = _qcl.SaveTransaction(_gl.Domestic_PHP_Journal, _gl.Domestic_PHP_TellerGL, _gl.Domestic_PHP_SendoutChargeGL, totalCharge, 0.0, totalQuantity, "KP Sendout Charge", "KPSOPRDOM1234", "PHP", transDate, empID, empFullname, branchcode, cmd);

                        if (saveSOPrincipal && saveSOCharge)
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
