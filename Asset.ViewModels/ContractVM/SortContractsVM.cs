﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.ViewModels.ContractVM
{
    public class SortContractsVM
    {
        public string ContractNumber { get; set; }
        public string Subject { get; set; }
        public string ContractDate { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string SortStatus { get; set; }
        public string SortBy { get; set; }
    }
}
