using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Test.Models
{
    public class PrescriptionDetailFile
    {
        [Required]
        [DisplayName("Medicine Code")]
        public int PreDMedCode { get; set; }

        [Required]
        [DisplayName("Remarks")]
        public string PreDRemarks { get; set; }

        [Required]
        [DisplayName("Quantity")]
        public int PreDQty { get; set; }
    }
}