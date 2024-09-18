using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class Calibration
    {
        public Calibration()
        {
            CalibrationAttachments = new HashSet<CalibrationAttachment>();
        }

        public int Id { get; set; }
        public int DeviceId { get; set; }
        public string EngineerId { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }

        public virtual AssetDetail Device { get; set; }
        public virtual AspNetUser Engineer { get; set; }
        public virtual ICollection<CalibrationAttachment> CalibrationAttachments { get; set; }
    }
}
