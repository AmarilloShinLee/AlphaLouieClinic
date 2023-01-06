using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test.Models
{
    public class DescriptionTypeModel
    {
        public int DescriptionID { get; set; }
        public string DescriptionType { get; set; }
        public DescriptionTypeModel(int descriptionID, string descriptionType)
        {
            DescriptionID = descriptionID;
            DescriptionType = descriptionType;
        }

        public static IEnumerable<DescriptionTypeModel> GetDescriptionTypes()
        {
            return new List<DescriptionTypeModel>
            {
                new DescriptionTypeModel(0, "Consultation"),
                new DescriptionTypeModel(1, "Immunization"),
            };
        }
    }
}