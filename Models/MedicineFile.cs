using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Test.Models
{
    public class MedicineFile
    {
        [Required(ErrorMessage = "Medicine code is required")]
        [DisplayName("Code")]
        [Range(1013400000, 1013699999, ErrorMessage = "Code must be at range between 1013400000 - 1013699999")]
        public int MedCode { get; set; }

        [Required(ErrorMessage = "Medicine name is required")]
        [DisplayName("Name")]
        public string MedName { get; set; }

        [Required(ErrorMessage = "Medicine Dose is required")]
        [DisplayName("Dose")]
        [Range(1, int.MaxValue, ErrorMessage = "Doses must be greater than 0")]
        public int MedDose { get; set; }

        [Required(ErrorMessage = "Medicine description is required")]
        [DisplayName("Description")]
        public string MedDesc { get; set; }

        public int OldCode { get; set; }
    }
}