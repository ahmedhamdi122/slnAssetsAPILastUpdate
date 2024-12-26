using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset.ViewModels.WorkOrderVM
{
    public class WorkOrderResultVM
    {
        public List<GetData> Results { get; set; }
        public int Count { get; set; }
        public class GetData
        {
            public int Id { get; set; }
            public string Note { get; set; }
            public string ClosedDate { get; set; }

            public int WorkOrderStatusId { get; set; }
            public string Subject { get; set; }
            public TimeSpan? ElapsedTime { get; set; }
            public string WorkOrderNumber { get; set; }
            public DateTime? CreationDate { get; set; }
            public string CreatedBy { get; set; }
            public string RequestSubject { get; set; }
            public string BarCode { get; set; }
            public string StatusName { get; set; }
            public string StatusNameAr { get; set; }
            public string statusColor { get; set; }
            public string statusIcon { get; set; }
            public string AssetName { get; set; }
            public string AssetNameAr { get; set; }
            public string SerialNumber { get; set; }
            public string ModelNumber { get; set; }
        }
    }
}
