using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cnf.Finance.Entity
{
    public partial class Organization
    {
        public Organization()
        {
            Project = new HashSet<Project>();
            Users = new HashSet<Users>();
        }

        [Display(Name="单位ID")]
        public int OrganizationId { get; set; }

        [Display(Name="单位名称")]
        [Required(ErrorMessage ="必须输入单位名称")]
        public string Name { get; set; }

        public virtual ICollection<Project> Project { get; set; }

        public virtual ICollection<Users> Users { get; set; }
    }
}
