using Asset.Models;
using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API
{
    public partial class Section
    {
        public Section()
        {
            InverseParentSection = new HashSet<Section>();
        }

        public int Id { get; set; }
        public int? ParentSectionId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Route { get; set; }
        public int DisplayOrder { get; set; }
        public int Level { get; set; }
        public string Icon { get; set; }

        public virtual Section ParentSection { get; set; }
        public virtual ICollection<Section> InverseParentSection { get; set; }
        public virtual ICollection<Module> Modules { get; set; }

    }
}
