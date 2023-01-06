using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Test.Models
{
    public class PrescriptionHeaderFile
    {
        [Required(ErrorMessage = "Prescription is required")]
        [DisplayName("Prescription Code")]
        [Range(1013000000, 1013199999, ErrorMessage = "Prescription Code must be at range between 1013000000 - 1013199999")]
        public int PresHCode { get; set; }

        [Required]
        [DisplayName("Consultation Code")]
        [Range(1012100000, 1012399999, ErrorMessage = "Consultation Code must be at range between 1012100000 - 1012399999")]
        public int PresHConsNo { get; set; }

        [Required]
        [DisplayName("Patient Code")]
        [Range(1011400000, 1011699999, ErrorMessage = "Patient Code must be at range between 1011400000 - 1011699999")]
        public int PresHPatCode { get; set; }

        [Required]
        [DisplayName("Date")]
        [DataType(DataType.Date)]
        public DateTime PresHDate { get; set; }
        public List<PrescriptionDetailFile> PrescriptionDetails { get; set; }

        public int OldCode { get; set; }

        public PrescriptionHeaderFile()
        {
            PrescriptionDetails = new List<PrescriptionDetailFile>();
        }
    }
}