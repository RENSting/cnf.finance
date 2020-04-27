using System;
using System.Collections.Generic;

namespace Cnf.Finance.Entity
{
    public partial class PerformTerms
    {
        public int Id { get; set; }
        public int PerformId { get; set; }
        public int TermsId { get; set; }
        public string Comments { get; set; }

        public virtual Perform Perform { get; set; }
        public virtual Terms Terms { get; set; }
    }
}
