using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPIAgent.Model
{
    public class Request
    {
        public string kptn { get; set; }
        public string transdate { get; set; }
        public string bcode { get; set; }
        public int zcode { get; set; }
        public string employee { get; set; }
    }
}
