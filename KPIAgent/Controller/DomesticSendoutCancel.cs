using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;
using KPIAgent.Helper;
using KPIAgent.Model;
using Newtonsoft.Json;

namespace KPIAgent.Controller
{
    class DomesticSendoutCancel
    {
        Logger _logger = new Logger();
        DatabaseQCL _qcl = new DatabaseQCL();
        QueryKPTN _query = new QueryKPTN();
        ReadINI _read = new ReadINI();

        Request _sendoutCancelRequest = new Request();

        private static string tableName = "KPDomestic_TransactionlogsKPTNSendoutCancel";

        public void GetSendoutCancelDomestic(string bcode, int zcode, string transactionDate, List<Operators> _operator)
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

                        _sendoutCancelRequest.kptn = kptn;
                        _sendoutCancelRequest.transdate = transactionDate;
                        _sendoutCancelRequest.bcode = bcode;
                        _sendoutCancelRequest.zcode = zcode;
                        _sendoutCancelRequest.employee = emp.userlogin;

                        string stringRequest = JsonConvert.SerializeObject(_sendoutCancelRequest);

                        byte[] postDataBytes = enc.GetBytes(stringRequest);

                        WebRequest request = WebRequest.Create(url + "/Domestic/SendoutCancel");
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

                        var serviceResponse = JsonConvert.DeserializeObject<DomesticSendoutCancelResponse>(responseFromService);

                        if (serviceResponse.respcode == 0)
                        {
                            FetchSendoutCancelDomestic(serviceResponse.respdata, transactionDate, emp.userlogin, emp.fullname, bcode);
                        }

                        else if (serviceResponse.respcode == 1)
                        {
                            _logger.Info("Domestic [SENDOUT CANCEL] " + serviceResponse.respmsg + " - KPTN: " + kptn + " - Transdate: " + transactionDate + " - BranchCode: " + bcode + " - ZoneCode: " + zcode + " - Operator: " + emp.userlogin);
                            persist++;
                        }

                        else if (serviceResponse.respcode == 2)
                        {
                            _logger.Error("Domestic [SENDOUT CANCEL] " + serviceResponse.respmsg + " - KPTN: " + kptn + " - Transdate: " + transactionDate + " - BranchCode: " + bcode + " - ZoneCode: " + zcode + " - Operator: " + emp.userlogin);
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

        private void FetchSendoutCancelDomestic(List<DomesticSendoutCancelTrans> _sendoutCancelData, string transDate, string empID, string empFullname, string branchcode)
        {
            var _gl = _read.GLAccounts().GLAccounts;

            string server = _read.QCLConfig().Server;
            string user = _read.QCLConfig().User;
            string password = _read.QCLConfig().Password;

            string constring = "server = " + server + ";database = " + branchcode + ";uid = " + user + ";password= " + password + "; pooling=false;min pool size=0;max pool size=1000;connection lifetime=0; connection timeout=360000";

            double totalRTSPrincipal = 0;
            double totalDSOPrincipal = 0;
            double totalCSOPrincipal = 0;

            double totalRFCCharge = 0;
            double totalDSOCharge = 0;
            double totalCSOCharge = 0;

            double totalRTSQuantity = 0;
            double totalRFCQuantity = 0;
            double totalDSOQuantity = 0;
            double totalCSOQuantity = 0;

            bool saveRTSPrincipal = false;
            bool saveDSOPrincipal = false;
            bool saveDSOCharge = false;
            bool saveRFCCharge = false;
            bool saveCSOPrincipal = false;
            bool saveCSOCharge = false;

            List<string> kptnList = new List<string>();

            try
            {
                foreach (var sendoutCancelData in _sendoutCancelData)
                {
                    switch (sendoutCancelData.cancelreason.Trim())
                    {
                        case "Return to Sender":
                            totalRTSQuantity += 1;
                            totalRTSPrincipal += sendoutCancelData.principal;
                            break;

                        case "Request for Change":
                            totalRFCQuantity += 1;
                            totalRFCCharge += sendoutCancelData.cancelcharge;
                            break;

                        case "Double Sendout":
                            totalDSOQuantity += 1;
                            totalDSOPrincipal += sendoutCancelData.principal + sendoutCancelData.charge;
                            totalDSOCharge += sendoutCancelData.cancelcharge;
                            break;

                        case "Cancel Sendout":
                            totalCSOQuantity += 1;
                            totalCSOPrincipal += sendoutCancelData.principal + sendoutCancelData.charge;
                            totalCSOCharge += sendoutCancelData.cancelcharge;
                            break;
                    }
                    kptnList.Add(sendoutCancelData.kptn);
                }

                string kptn = string.Join(",", kptnList.ToArray());

                using (SqlConnection con = new SqlConnection(constring))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        SqlTransaction trans = con.BeginTransaction(IsolationLevel.ReadCommitted);
                        cmd.Transaction = trans;

                        if (totalRTSQuantity > 0)
                        {
                            saveRTSPrincipal = _qcl.SaveTransaction(_gl.Domestic_PHP_Journal, _gl.Domestic_PHP_ReturnToSenderGL, _gl.Domestic_PHP_TellerGL, totalRTSPrincipal, 0.0, totalRTSQuantity, "RTS / Cancelled Sendout", "KPSOPRDOM1234", "PHP", transDate, empID, empFullname, branchcode, cmd);
                        }

                        if (totalDSOQuantity > 0)
                        {
                            saveDSOPrincipal = _qcl.SaveTransaction(_gl.Domestic_PHP_Journal, _gl.Domestic_PHP_SendoutCancelGL, _gl.Domestic_PHP_TellerGL, totalDSOPrincipal, totalDSOQuantity, 0.0, "Double / Cancelled Sendout", "KPSOPRDOM1234", "PHP", transDate, empID, empFullname, branchcode, cmd);
                            saveDSOCharge = _qcl.SaveTransaction(_gl.Domestic_PHP_Journal, _gl.Domestic_PHP_TellerGL, _gl.Domestic_PHP_SendoutCancelGL, totalDSOCharge, 0.0, totalDSOQuantity, "KP Other Income", "KPSOPRDOM1234", "PHP", transDate, empID, empFullname, branchcode, cmd);
                        }

                        if (totalRFCQuantity > 0)
                        {
                            saveRFCCharge = _qcl.SaveTransaction(_gl.Domestic_PHP_Journal, _gl.Domestic_PHP_TellerGL, _gl.Domestic_PHP_RFCGL, totalRFCCharge, 0.0, totalRFCQuantity, "KP Other Income", "KPSOPRDOM1234", "PHP", transDate, empID, empFullname, branchcode, cmd);
                        }

                        if (totalCSOQuantity > 0)
                        {
                            saveCSOPrincipal = _qcl.SaveTransaction(_gl.Domestic_PHP_Journal, _gl.Domestic_PHP_SendoutCancelGL, _gl.Domestic_PHP_TellerGL, totalCSOPrincipal, totalCSOQuantity, 0.0, "RTS / Cancelled Sendout", "KPSOPRDOM1234", "PHP", transDate, empID, empFullname, branchcode, cmd);
                            saveCSOCharge = _qcl.SaveTransaction(_gl.Domestic_PHP_Journal, _gl.Domestic_PHP_TellerGL, _gl.Domestic_PHP_SendoutCancelGL, totalCSOCharge, 0.0, totalCSOQuantity, "KP Other Income", "KPSOPRDOM1234", "PHP", transDate, empID, empFullname, branchcode, cmd);
                        }

                        if (saveRTSPrincipal || saveDSOPrincipal && saveDSOCharge || saveRFCCharge || saveCSOPrincipal && saveCSOCharge)
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
