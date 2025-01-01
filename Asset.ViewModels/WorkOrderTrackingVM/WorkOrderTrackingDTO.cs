using Asset.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.ViewModels.WorkOrderTrackingVM
{
    public class WorkOrderTrackingDTO
    {

        public string workOrderStatusIcon { get; set; }
        public string CreatedBy { get; set; }
        public string workOrderStatusName { get; set; }
        public string workOrderStatusNameAr { get; set; }
        public string workOrderStatusColor { get; set; }
        public int Id { get; set; }
        public DateTime? CreationDate { get; set; }
        public string Notes { get; set; }
  
        public int WorkOrderTrackingId { get; set; }
    }
}
