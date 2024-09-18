using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class RequestTracking
    {
        public RequestTracking()
        {
            RequestDocuments = new HashSet<RequestDocument>();
        }

        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime? DescriptionDate { get; set; }
        public int? RequestStatusId { get; set; }
        public int? RequestId { get; set; }
        public string CreatedById { get; set; }
        public bool? IsOpened { get; set; }
        public int? HospitalId { get; set; }

        public virtual AspNetUser CreatedBy { get; set; }
        public virtual Request Request { get; set; }
        public virtual RequestStatus RequestStatus { get; set; }
        public virtual ICollection<RequestDocument> RequestDocuments { get; set; }
    }
}
