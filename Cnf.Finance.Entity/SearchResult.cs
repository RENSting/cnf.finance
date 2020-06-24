using System;
using System.Collections.Generic;
using System.Text;

namespace Cnf.Finance.Entity
{
    public class SearchResult<T>
    {
        public int Total { get; set; }

        public IEnumerable<T> Records { get; set; }
    }
}
