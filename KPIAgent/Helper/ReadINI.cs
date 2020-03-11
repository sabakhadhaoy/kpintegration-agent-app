using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KPIAgent.Model;

namespace KPIAgent.Helper
{
    class ReadINI
    {
        GLAccounts _gl = new GLAccounts();
        QCLConfig _qcldb = new QCLConfig();
        
        public ListGL GLAccounts()
        {
            var ini = new IniFile.IniFile("C:\\bosconfig\\GLAccounts.ini");

            _gl.Domestic_PHP_TellerGL = ini.IniReadValue("Domestic Peso", "TellerGL").ToString().PadLeft(9);
            _gl.Domestic_PHP_Journal = ini.IniReadValue("Domestic Peso", "Journal").ToString();
            _gl.Domestic_PHP_SendoutPrincipalGL = ini.IniReadValue("Domestic Peso", "SendoutPrincipal").ToString().PadLeft(9);
            _gl.Domestic_PHP_SendoutChargeGL = ini.IniReadValue("Domestic Peso", "SendoutCharge").ToString().PadLeft(9);
            _gl.Domestic_PHP_PayoutPrincipalGL = ini.IniReadValue("Domestic Peso", "PayoutPrincipal").ToString().PadLeft(9);
            _gl.Domestic_PHP_PayoutCancelGL = ini.IniReadValue("Domestic Peso", "PayoutCancel").ToString().PadLeft(9);
            _gl.Domestic_PHP_ReturnToSenderGL = ini.IniReadValue("Domestic Peso", "ReturnToSender").ToString().PadLeft(9);
            _gl.Domestic_PHP_SendoutCancelGL = ini.IniReadValue("Domestic Peso", "SendoutCancel").ToString().PadLeft(9);
            _gl.Domestic_PHP_RFCGL = ini.IniReadValue("Domestic Peso", "RFC").ToString().PadLeft(9);

            _gl.Billspay_TellerGL = ini.IniReadValue("Billspay Peso", "TellerGL").ToString().PadLeft(9);
            _gl.Billspay_Journal = ini.IniReadValue("Billspay Peso", "Journal").ToString();
            _gl.Billspay_PrincipalGL = ini.IniReadValue("Billspay Peso", "BillspayPrincipal").ToString().PadLeft(9);
            _gl.Billspay_ChargeGL = ini.IniReadValue("Billspay Peso", "BillspayCharge").ToString().PadLeft(9);
            _gl.Billspay_CancelGL = ini.IniReadValue("Billspay Peso", "BillspayCancel").ToString().PadLeft(9);
            _gl.Billspay_RFCGL = ini.IniReadValue("Billspay Peso", "BillspayRFC").ToString().PadLeft(9);

            _gl.Global_PHP_TellerGL = ini.IniReadValue("Global Peso", "TellerGL").ToString().PadLeft(9);
            _gl.Global_PHP_Journal = ini.IniReadValue("Global Peso", "Journal").ToString();
            _gl.Global_PHP_SendoutPrincipalGL = ini.IniReadValue("Global Peso", "SendoutPrincipal").ToString().PadLeft(9);
            _gl.Global_PHP_SendoutChargeGL = ini.IniReadValue("Global Peso", "SendoutCharge").ToString().PadLeft(9);
            _gl.Global_PHP_PayoutPrincipalGL = ini.IniReadValue("Global Peso", "PayoutPrincipal").ToString().PadLeft(9);
            _gl.Global_PHP_PayoutCancelGL = ini.IniReadValue("Global Peso", "PayoutCancel").ToString().PadLeft(9);
            _gl.Global_PHP_ReturnToSenderGL = ini.IniReadValue("Global Peso", "RTS").ToString().PadLeft(9);
            _gl.Global_PHP_SendoutCancelGL = ini.IniReadValue("Global Peso", "SendoutCancel").ToString().PadLeft(9);
            _gl.Global_PHP_RFCGL = ini.IniReadValue("Global Peso", "RFC").ToString().PadLeft(9);

            _gl.Global_USD_TellerGL = ini.IniReadValue("Global Dollar", "TellerGL").ToString().PadLeft(9);
            _gl.Global_USD_Journal = ini.IniReadValue("Global Dollar", "Journal").ToString();
            _gl.Global_USD_SendoutPrincipalGL = ini.IniReadValue("Global Dollar", "SendoutPrincipal").ToString().PadLeft(9);
            _gl.Global_USD_SendoutChargeGL = ini.IniReadValue("Global Dollar", "SendoutCharge").ToString().PadLeft(9);
            _gl.Global_USD_PayoutPrincipalGL = ini.IniReadValue("Global Dollar", "PayoutPrincipal").ToString().PadLeft(9);
            _gl.Global_USD_PayoutCancelGL = ini.IniReadValue("Global Dollar", "PayoutCancel").ToString().PadLeft(9);
            _gl.Global_USD_ReturnToSenderGL = ini.IniReadValue("Global Dollar", "RTS").ToString().PadLeft(9);
            _gl.Global_USD_SendoutCancelGL = ini.IniReadValue("Global Dollar", "SendoutCancel").ToString().PadLeft(9);
            _gl.Global_USD_RFCGL = ini.IniReadValue("Global Dollar", "RFC").ToString().PadLeft(9);

            _gl.Corporate_PHP_TellerGL = ini.IniReadValue("Corporate Partner Peso", "TellerGL").ToString().PadLeft(9);
            _gl.Corporate_PHP_Journal = ini.IniReadValue("Corporate Partner Peso", "Journal").ToString();
            _gl.Corporate_PHP_PayoutPrincipalGL = ini.IniReadValue("Corporate Partner Peso", "PayoutPrincipal").ToString().PadLeft(9);
            _gl.Corporate_PHP_PayoutCancelGL = ini.IniReadValue("Corporate Partner Peso", "PayoutCancel").ToString().PadLeft(9);

            _gl.Corporate_USD_TellerGL = ini.IniReadValue("Corporate Partner Dollar", "TellerGL").ToString().PadLeft(9);
            _gl.Corporate_USD_Journal = ini.IniReadValue("Corporate Partner Dollar", "Journal").ToString();
            _gl.Corporate_USD_PayoutPrincipalGL = ini.IniReadValue("Corporate Partner Dollar", "PayoutPrincipal").ToString().PadLeft(9);
            _gl.Corporate_USD_PayoutCancelGL = ini.IniReadValue("Corporate Partner Dollar", "PayoutCancel").ToString().PadLeft(9);

            _gl.PaymentSolution_PHP_TellerGL = ini.IniReadValue("Payment Solution Peso", "TellerGL").ToString().PadLeft(9);
            _gl.PaymentSolution_PHP_Journal = ini.IniReadValue("Payment Solution Peso", "Journal").ToString();
            _gl.PaymentSolution_PHP_PayoutPrincipalGL = ini.IniReadValue("Payment Solution Peso", "PaymentSolutions").ToString().PadLeft(9);

            _gl.KPTOGO_PHP_TellerGL = ini.IniReadValue("KPTOGO Peso", "TellerGL").ToString().PadLeft(9);
            _gl.KPTOGO_PHP_Journal = ini.IniReadValue("KPTOGO Peso", "Journal").ToString();
            _gl.KPTOGO_PHP_SendoutPrincipalGL = ini.IniReadValue("KPTOGO Peso", "SendoutPrincipal").ToString().PadLeft(9);
            _gl.KPTOGO_PHP_SendoutChargeGL = ini.IniReadValue("KPTOGO Peso", "SendoutCharge").ToString().PadLeft(9);
            _gl.KPTOGO_PHP_PayoutPrincipalGL = ini.IniReadValue("KPTOGO Peso", "PayoutPrincipal").ToString().PadLeft(9);
            _gl.KPTOGO_PHP_PayoutCancelGL = ini.IniReadValue("KPTOGO Peso", "PayoutCancel").ToString().PadLeft(9);
            _gl.KPTOGO_PHP_ReturnToSenderGL = ini.IniReadValue("KPTOGO Peso", "RTS").ToString().PadLeft(9);
            _gl.KPTOGO_PHP_SendoutCancelGL = ini.IniReadValue("KPTOGO Peso", "SendoutCancel").ToString().PadLeft(9);
            _gl.KPTOGO_PHP_RFCGL = ini.IniReadValue("KPTOGO Peso", "RFC").ToString().PadLeft(9);

            _gl.Wallet_PHP_TellerGL = ini.IniReadValue("Wallet Peso", "TellerGL").ToString().PadLeft(9);
            _gl.Wallet_PHP_Journal = ini.IniReadValue("Wallet Peso", "Journal").ToString();
            _gl.Wallet_PHP_SendoutPrincipalGL = ini.IniReadValue("Wallet Peso", "SendoutPrincipal").ToString().PadLeft(9);
            _gl.Wallet_PHP_SendoutChargeGL = ini.IniReadValue("Wallet Peso", "SendoutCharge").ToString().PadLeft(9);
            _gl.Wallet_PHP_PayoutPrincipalGL = ini.IniReadValue("Wallet Peso", "PayoutPrincipal").ToString().PadLeft(9);
            _gl.Wallet_PHP_PayoutCancelGL = ini.IniReadValue("Wallet Peso", "PayoutCancel").ToString().PadLeft(9);
            _gl.Wallet_PHP_ReturnToSenderGL = ini.IniReadValue("Wallet Peso", "RTS").ToString().PadLeft(9);
            _gl.Wallet_PHP_SendoutCancelGL = ini.IniReadValue("Wallet Peso", "SendoutCancel").ToString().PadLeft(9);
            _gl.Wallet_PHP_RFCGL = ini.IniReadValue("Wallet Peso", "RFC").ToString().PadLeft(9);

            _gl.Kiosk_PHP_TellerGL = ini.IniReadValue("Kiosk Peso", "TellerGL").ToString().PadLeft(9);
            _gl.Kiosk_PHP_Journal = ini.IniReadValue("Kiosk Peso", "Journal").ToString();
            _gl.Kiosk_PHP_SendoutPrincipalGL = ini.IniReadValue("Kiosk Peso", "SendoutPrincipal").ToString().PadLeft(9);
            _gl.Kiosk_PHP_SendoutChargeGL = ini.IniReadValue("Kiosk Peso", "SendoutCharge").ToString().PadLeft(9);
            _gl.Kiosk_PHP_PayoutPrincipalGL = ini.IniReadValue("Kiosk Peso", "PayoutPrincipal").ToString().PadLeft(9);
            _gl.Kiosk_PHP_PayoutCancelGL = ini.IniReadValue("Kiosk Peso", "PayoutCancel").ToString().PadLeft(9);
            _gl.Kiosk_PHP_ReturnToSenderGL = ini.IniReadValue("Kiosk Peso", "RTS").ToString().PadLeft(9);
            _gl.Kiosk_PHP_SendoutCancelGL = ini.IniReadValue("Kiosk Peso", "SendoutCancel").ToString().PadLeft(9);
            _gl.Kiosk_PHP_RFCGL = ini.IniReadValue("Kiosk Peso", "RFC").ToString().PadLeft(9);

            _gl.KP8_PHP_TellerGL = ini.IniReadValue("KP8 Peso", "TellerGL").ToString().PadLeft(9);
            _gl.KP8_PHP_Journal = ini.IniReadValue("KP8 Peso", "Journal").ToString();
            _gl.KP8_PHP_SendoutPrincipalGL = ini.IniReadValue("KP8 Peso", "SendoutPrincipal").ToString().PadLeft(9);
            _gl.KP8_PHP_SendoutChargeGL = ini.IniReadValue("KP8 Peso", "SendoutCharge").ToString().PadLeft(9);
            _gl.KP8_PHP_PayoutPrincipalGL = ini.IniReadValue("KP8 Peso", "PayoutPrincipal").ToString().PadLeft(9);
            _gl.KP8_PHP_PayoutCancelGL = ini.IniReadValue("KP8 Peso", "PayoutCancel").ToString().PadLeft(9);
            _gl.KP8_PHP_ReturnToSenderGL = ini.IniReadValue("KP8 Peso", "RTS").ToString().PadLeft(9);
            _gl.KP8_PHP_SendoutCancelGL = ini.IniReadValue("KP8 Peso", "SendoutCancel").ToString().PadLeft(9);
            _gl.KP8_PHP_RFCGL = ini.IniReadValue("KP8 Peso", "RFC").ToString().PadLeft(9);

            _gl.Express_PHP_TellerGL = ini.IniReadValue("Express Peso", "TellerGL").ToString().PadLeft(9);
            _gl.Express_PHP_Journal = ini.IniReadValue("Express Peso", "Journal").ToString();
            _gl.Express_PHP_SendoutPrincipalGL = ini.IniReadValue("Express Peso", "SendoutPrincipal").ToString().PadLeft(9);
            _gl.Express_PHP_SendoutChargeGL = ini.IniReadValue("Express Peso", "SendoutCharge").ToString().PadLeft(9);
            _gl.Express_PHP_PayoutPrincipalGL = ini.IniReadValue("Express Peso", "PayoutPrincipal").ToString().PadLeft(9);
            _gl.Express_PHP_PayoutCancelGL = ini.IniReadValue("Express Peso", "PayoutCancel").ToString().PadLeft(9);
            _gl.Express_PHP_ReturnToSenderGL = ini.IniReadValue("Express Peso", "RTS").ToString().PadLeft(9);
            _gl.Express_PHP_SendoutCancelGL = ini.IniReadValue("Express Peso", "SendoutCancel").ToString().PadLeft(9);
            _gl.Express_PHP_RFCGL = ini.IniReadValue("Express Peso", "RFC").ToString().PadLeft(9);

            return new ListGL{  GLAccounts = _gl };
           
        }
        public QCLConfig QCLConfig()
        {
            var ini = new IniFile.IniFile(Application.StartupPath + "\\qclconfig\\qcldb.ini");

            _qcldb.Server = ini.IniReadValue("QCL Database", "Server");
            _qcldb.User = ini.IniReadValue("QCL Database", "User");
            _qcldb.Password = ini.IniReadValue("QCL Database", "Password");
            _qcldb.From = ini.IniReadValue("QCL Database", "From");
            _qcldb.To = ini.IniReadValue("QCL Database", "To");
            _qcldb.Zonecode = Convert.ToInt32(ini.IniReadValue("QCL Database", "Zonecode"));


            return new QCLConfig
            {
                Server = _qcldb.Server,
                User = _qcldb.User,
                Password = _qcldb.Password,
                From = _qcldb.From,
                To = _qcldb.To,
                Zonecode = _qcldb.Zonecode,
                
            };

        }
        public string serviceUrl()
        {
            var ini = new IniFile.IniFile(Application.StartupPath + "\\qclconfig\\qcldb.ini");

            string readUrl = ini.IniReadValue("Integration Service", "url");

            return readUrl;
        }
    }
}
