using System;
using System.Collections.Generic;
using System.Text;

namespace Cnf.Finance.Entity
{
    public class ReportBlock
    {
        public decimal? Incoming { get; set; }
        public decimal? Settlement { get; set; }
        public decimal? Retrievable { get; set; }

        /// <summary>
        /// 税 = 结算 * 项目税率
        /// </summary>
        public decimal? Tax { get; set; }
    }

    public class ReportItem
    {
        public ReportItem()
        {
            Plan = new ReportBlock();
            Perform = new ReportBlock();
        }

        /// <summary>
        /// 计划目标
        /// </summary>
        public ReportBlock Plan { get; set; }

        /// <summary>
        /// 实际完成
        /// </summary>
        public ReportBlock Perform { get; set; }
    }
}
