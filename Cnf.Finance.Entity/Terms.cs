using System;
using System.Collections.Generic;

namespace Cnf.Finance.Entity
{
    public partial class Terms
    {
        public Terms()
        {
            PerformTerms = new HashSet<PerformTerms>();
            PlanTerms = new HashSet<PlanTerms>();
        }

        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int TermsCategory { get; set; }
        public decimal? TargetAmount { get; set; }
        public DateTime? TargetDate { get; set; }
        public string Provision { get; set; }
        public string Remarks { get; set; }

        public virtual Project Project { get; set; }
        public virtual ICollection<PerformTerms> PerformTerms { get; set; }
        public virtual ICollection<PlanTerms> PlanTerms { get; set; }
    }
}
