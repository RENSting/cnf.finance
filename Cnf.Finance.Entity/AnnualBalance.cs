using System;
using System.Collections.Generic;

namespace Cnf.Finance.Entity
{
    public partial class AnnualBalance
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int Year { get; set; }
        public int BalanceCategory { get; set; }
        public decimal Balance { get; set; }

        public virtual Project Project { get; set; }
    }
}
