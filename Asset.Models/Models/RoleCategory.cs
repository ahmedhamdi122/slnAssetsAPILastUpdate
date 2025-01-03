﻿using Asset.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Asset.Models
{
   public class RoleCategory
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string NameAr { get; set; }

        public int? OrderId { get; set; }
        public virtual ICollection<ApplicationRole> ApplicationRoles { get; set; }

    }
}
