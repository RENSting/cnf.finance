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

        /// <summary>
        /// 税 = 结算 * 项目税率
        /// </summary>
        public decimal Tax { get; set; }
    }
}
