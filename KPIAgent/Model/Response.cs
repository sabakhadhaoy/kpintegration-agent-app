using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPIAgent.Model
{
    public class OperatorResponse
    {
        public int respcode { get; set; }
        public string respmsg { get; set; }
        public List<Operators> respdata { get; set; }
    }
    public class Operators
    {
        public string fullname { get; set; }
        public string userlogin { get; set; }
    }

    public class DomesticSendoutResponse
    {
        public int respcode { get; set; }
        public string respmsg { get; set; }
        public List<DomesticSendoutTrans> respdata { get; set; }
    }
    public class DomesticSendoutTrans
    {
        public string kptn { get; set; }
        public double domestic_principal { get; set; }
        public double domestic_charge { get; set; }
    }
    public class DomesticSendoutCancelResponse
    {
        public int respcode { get; set; }
        public string respmsg { get; set; }
        public List<DomesticSendoutCancelTrans> respdata { get; set; }
    }
    public class DomesticSendoutCancelTrans
    {
        public string kptn { get; set; }
        public double principal { get; set; }
        public double charge { get; set; }
        public double cancelcharge { get; set; }
        public string cancelreason { get; set; }
    }
    public class DomesticPayoutResponse
    {
        public int respcode { get; set; }
        public string respmsg { get; set; }
        public List<DomesticPayoutTrans> respdata { get; set; }
    }
    public class DomesticPayoutTrans
    {
        public string kptn { get; set; }
        public double domestic_principal { get; set; }
    }
    public class DomesticPayoutCancelResponse
    {
        public int respcode { get; set; }
        public string respmsg { get; set; }
        public List<DomesticPayoutCancelTrans> respdata { get; set; }
    }
    public class DomesticPayoutCancelTrans
    {
        public string kptn { get; set; }
        public double domestic_wrongpayout_principal { get; set; }
    }

}
