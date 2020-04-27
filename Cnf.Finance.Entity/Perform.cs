using System;
using System.Collections.Generic;

namespace Cnf.Finance.Entity
{
    public partial class Perform
    {
        public Perform()
        {
            PerformTerms = new HashSet<PerformTerms>();
        }

        public int Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int ProjectId { get; set; }
        public decimal? Incoming { get; set; }
        public decimal? Settlement { get; set; }
        public decimal? Retrieve { get; set; }

        public virtual Project Project { get; set; }
        public virtual ICollection<PerformTerms> PerformTerms { get; set; }
    }
}
