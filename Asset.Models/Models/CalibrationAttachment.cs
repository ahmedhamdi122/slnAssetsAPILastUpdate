using System;
using System.Collections.Generic;

#nullable disable

namespace Asset.API.Models
{
    public partial class CalibrationAttachment
    {
        public int Id { get; set; }
        public int CalibrationId { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }

        public virtual Calibration Calibration { get; set; }
    }
}
