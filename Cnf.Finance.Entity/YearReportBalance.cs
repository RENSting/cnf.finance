using System;
using System.Collections.Generic;
using System.Text;

namespace Cnf.Finance.Entity
{
    public class YearReportBalance
    {
        public int Year { get; set; }
        public decimal Incoming { get; set; }
        public decimal Settlement { get; set; }
        public decimal Retrievable { get; set; }
    }
}
