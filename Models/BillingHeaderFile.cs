using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Test.Models
{
    public class BillingHeaderFile
    {
        [Required(ErrorMessage = "Billing Code is required")]
        [DisplayName("Bill ID")]
        [Range(1013200000, 1013399999, ErrorMessage = "Billing Code must be between 1013200000 - 1013399999")]
        public int BillHNo { get; set; }

        [Required(ErrorMessage = "Patient Code is required")]
        [DisplayName("Patient Code")]
        [Range(1011400000, 1011699999, ErrorMessage = "Patient Code must be between 1011400000 - 1011699999")]
        public int BillHPatCode { get; set; }

        [Required(ErrorMessage = "Billing Date is required")]
        [DisplayName("Date")]
        [DataType(DataType.Date)]
        public DateTime BillHDate { get; set; }

        [Required(ErrorMessage = "Total Amount is required")]
        [DisplayName("Total Amount")]
        [Range(1, Double.MaxValue, ErrorMessage = "Total Amount must be greater than 0")]
        [DataType(DataType.Currency)]
        public double BillHTotAmt { get; set; }

        public int OldCode { get; set; }

        public List<BillingDetailFile> BillingDetails { get; set; }
        public BillingHeaderFile()
        {
            BillingDetails = new List<BillingDetailFile>();
        }
    }
}