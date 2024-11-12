using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.ViewModels.AssetDetailVM
{
    public class SortAndFilterVM
    {
       public int sortOrder { get; set; }
        public string sortFiled { get; set; }
        public SearchAssetDetailVM SearchObj { get; set; }

    }
}
