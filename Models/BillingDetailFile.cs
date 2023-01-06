using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Test.Models
{
    public class BillingDetailFile
    {
        [Required(ErrorMessage = "Count is required")]
        public int BillDCount { get; set; }

        [Required(ErrorMessage = "Select a Description")]
        public string BillDDesc { get; set; }

        [Required(ErrorMessage = "Enter an amount")]
        [Range(1, Double.MaxValue, ErrorMessage = "Input must be greater than 0")]
        [RegularExpression("^-?\\d+(,\\d+)*(\\.\\d+(e\\d+)?)?$", ErrorMessage = "Input must be a number")]
        [DataType(DataType.Currency)]
        public double BillDAmount { get; set; }
    }
}