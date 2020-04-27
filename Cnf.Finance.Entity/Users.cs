using System;
using System.Collections.Generic;
using System.Text;

namespace Cnf.Finance.Entity
{
    public class Users
    {
        public int UserId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public int Role { get; set; }
        public bool ActiveStatus { get; set; }
        public int? OrganizationId { get; set; }

        public virtual Organization Organization { get; set; }
    }
}
