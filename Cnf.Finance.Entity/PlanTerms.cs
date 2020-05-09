using System;
using System.Collections.Generic;

namespace Cnf.Finance.Entity
{
    public partial class PlanTerms
    {
        public int Id { get; set; }
        public int PlanId { get; set; }
        public int TermsId { get; set; }
        public string Comments { get; set; }

        public virtual Plan Plan { get; set; }
        public virtual Terms Terms { get; set; }
    }
}
