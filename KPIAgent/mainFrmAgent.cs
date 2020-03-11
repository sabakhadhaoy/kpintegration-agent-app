using System;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using KPIAgent.Controller;
using KPIAgent.Helper;
using KPIAgent.Model;

namespace KPIAgent
{
    public partial class mainFrmAgent : Form
    {
        Logger _logger = new Logger();

        DomesticSendout _domesticSO = new DomesticSendout();
        DomesticPayout _domesticPO = new DomesticPayout();
        DomesticSendoutCancel _domesticSOCancel = new DomesticSendoutCancel();
        DomesticPayoutCancel _domesticPOCancel = new DomesticPayoutCancel();

        ReadINI _read = new ReadINI();

        string selectedDate;

        public mainFrmAgent()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var QCL = _read.QCLConfig();

            lblBcFrom.Text = QCL.From;
            lblBcTo.Text = QCL.To;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void KPIAgent_DoubleClick(object sender, EventArgs e)
        {
            Show();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            pBar.Visible = true;
            btnStart.Enabled = false;
            dateSelect.Enabled = false;
            
            _logger.Info("Starting Integration...");

            timer1.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            pBar.Visible = false;
            btnStart.Enabled = true;
            dateSelect.Enabled = true;

            timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int interval = 5;
            timer1.Interval = (1000 * 60 * interval);
            ProcessBcode();
        }
        private void ProcessBcode()
        {
            var QCL = _read.QCLConfig();

            int bcode_from = Convert.ToInt32(QCL.From);
            int bcode_to = Convert.ToInt32(QCL.To);

            while (bcode_from <= bcode_to)
            {
                string bcode = bcode_from.ToString().Count() == 2 ? "0" + bcode_from.ToString() : bcode_from.ToString();
                selectedDate = dateSelect.Text;

                DateTime selected = DateTime.Parse(selectedDate);
                DateTime now = DateTime.Today;

                while (selected <= now)
                {
                    string transdate = selected.ToString("yyyy-MM-dd");

                    dateSelect.Text = transdate;

                    GetOperator(bcode, QCL.Zonecode, transdate);

                    selected = selected.AddDays(1);
                }

                bcode_from++;
            }
        }
        private void GetOperator(string bcode, int zcode, string transdate)
        {
            string url = _read.serviceUrl();

            try
            {
                WebRequest request = WebRequest.Create(url + "/Operator/?bcode=" + bcode + "&zcode=" + zcode);
                request.Credentials = CredentialCache.DefaultCredentials;

                WebResponse response = request.GetResponse();
                Stream datastream = response.GetResponseStream();
                StreamReader reader = new StreamReader(datastream);
                string responseFromService = reader.ReadToEnd();

                var serviceReponse = JsonConvert.DeserializeObject<OperatorResponse>(responseFromService);

                if (serviceReponse.respcode == 0)
                {
                    _domesticSO.GetSendoutDomestic(bcode, zcode, transdate, serviceReponse.respdata);

                    _domesticPO.GetPayoutDomestic(bcode, zcode, transdate, serviceReponse.respdata);

                    _domesticSOCancel.GetSendoutCancelDomestic(bcode, zcode, transdate, serviceReponse.respdata);

                    _domesticPOCancel.GetPayoutCancelDomestic(bcode, zcode, transdate, serviceReponse.respdata);
                }

                else if (serviceReponse.respcode == 1)
                {
                    _logger.Info(serviceReponse.respmsg + " - BranchCode: " + bcode + " ZoneCode: " + zcode);
                }

                else if (serviceReponse.respcode == 2)
                {
                    _logger.Error(serviceReponse.respmsg + " - BranchCode: " + bcode + " ZoneCode: " + zcode);
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
        }
    }
}
