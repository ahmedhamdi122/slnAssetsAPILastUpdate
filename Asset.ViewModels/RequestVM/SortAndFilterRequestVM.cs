using System;


namespace Asset.ViewModels.RequestVM
{
   public class SortAndFilterRequestVM
    {
        public int sortOrder { get; set; }
        public string sortFiled { get; set; }
        public SearchRequestVM SearchObj { get; set; }
    }
}
