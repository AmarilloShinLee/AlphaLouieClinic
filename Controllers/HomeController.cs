using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Test.Models;
using ClinicDataLibrary.BusinessLogic;
using System.Diagnostics;
using static System.Data.Entity.Infrastructure.Design.Executor;
using Microsoft.AspNet.Identity;
using Microsoft.SqlServer.Server;
using System.Globalization;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Reflection.Emit;

namespace Test.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Identity()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        #region Medicine
        public ActionResult Medicine(int oldCode)
        {
            ViewBag.Message = "Add Medicine";

            MedicineFile medicineFile = new MedicineFile
            {
                OldCode = oldCode
            };

            return View(medicineFile);
        }
        [HttpPost]
        public ActionResult Medicine(MedicineFile medicine)
        {
            ViewBag.Message = "Add/Update Medicine";

            int dataAffected;

            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Please fill in the required inputs. Thank you!" });

            //Update
            if (medicine.OldCode > 0)
            {
                dataAffected = MedicineProcessor.UpdateMedicine(medicine.MedCode, medicine.MedName, medicine.MedDose, medicine.MedDesc, medicine.OldCode);
                return Json(new { success = true, message = "Medicine updated successfully" }, JsonRequestBehavior.AllowGet);
            }

            //If ID exists
            var medFile = MedicineProcessor.LoadMedicine(medicine.MedCode);

            if (MedicineProcessor.MedicineExist())
                return Json(new { success = false, message = $"Medicine code already exists.\nMedicine code: {medicine.MedCode}" });

            dataAffected = MedicineProcessor.CreateMedicine(medicine.MedCode, medicine.MedName, medicine.MedDose, medicine.MedDesc);
            return Json(new { success = true, message = "Medicine added successfully" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditMedicine(int medCode, int oldCode)
        {
            ViewBag.Message = "Edit Medicine";

            var medFile = MedicineProcessor.LoadMedicine(medCode);

            MedicineFile medicineFile = new MedicineFile
            {
                MedCode = medFile.MedCode,
                MedName = medFile.MedName,
                MedDesc = medFile.MedDesc,
                MedDose = medFile.MedDose,
                OldCode = oldCode
            };

            Debug.WriteLine(oldCode);

            return View("Medicine", medicineFile);
        }

        public ActionResult DeleteMedicine(int medCode)
        {
            ViewBag.Message = "Delete Medicine";

            int dataAffected = MedicineProcessor.DeleteMedicine(medCode);

            var medFile = MedicineProcessor.LoadMedicine();

            List<MedicineFile> medicines = new List<MedicineFile>();

            foreach (var row in medFile)
                medicines.Add(new MedicineFile { MedCode = row.MedCode, MedName = row.MedName, MedDesc = row.MedDesc, MedDose = row.MedDose });

            return View("ViewMedicineList", medicines);
        }

        public ActionResult ViewMedicineList()
        {
            ViewBag.Message = "View Medicine List";

            var medFile = MedicineProcessor.LoadMedicine();

            List<MedicineFile> medicines = new List<MedicineFile>();

            foreach (var row in medFile)
                medicines.Add(new MedicineFile
                {
                    MedCode = row.MedCode,
                    MedName = row.MedName,
                    MedDesc = row.MedDesc,
                    MedDose = row.MedDose,
                });

            return View(medicines);
        }

        public ActionResult ViewMedicine(int medCode)
        {
            ViewBag.Message = "View Medicine";

            var medFile = MedicineProcessor.LoadMedicine(medCode);

            MedicineFile medicineFile = new MedicineFile
            {
                MedCode = medFile.MedCode,
                MedName = medFile.MedName,
                MedDesc = medFile.MedDesc,
                MedDose = medFile.MedDose,
            };

            return View(medicineFile);
        }
        #endregion

        #region Prescription
        [Authorize(Roles = "Doctor")]
        public ActionResult Prescription(int oldCode)
        {
            ViewBag.Message = "Add/Update Prescription";

            PrescriptionHeaderFile prescriptionHeaderFile = new PrescriptionHeaderFile
            {
                OldCode = oldCode
            };
            prescriptionHeaderFile.PrescriptionDetails.Insert(0, new PrescriptionDetailFile());

            return View(prescriptionHeaderFile);
        }

        [HttpPost]
        public ActionResult Prescription(PrescriptionHeaderFile prescriptionHeaderFile)
        {
            ViewBag.Message = "Add/Update Prescription";

            int dataAffectedHeader;
            int dataAffectedDetail;

            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Please fill in the required inputs. Thank you!" });

            if (prescriptionHeaderFile.OldCode > 0)
            {
                //Update
                dataAffectedHeader = PrescriptionProcessor.UpdatePrescription(prescriptionHeaderFile.PresHCode, prescriptionHeaderFile.PresHConsNo, prescriptionHeaderFile.PresHPatCode, prescriptionHeaderFile.PresHDate, User.Identity.GetUserId(), prescriptionHeaderFile.OldCode);

                foreach (PrescriptionDetailFile detailFile in prescriptionHeaderFile.PrescriptionDetails)
                    dataAffectedDetail = PrescriptionProcessor.CreatePrescriptionD(prescriptionHeaderFile.PresHCode, detailFile.PreDMedCode, detailFile.PreDRemarks, detailFile.PreDQty);

                return Json(new { success = true, message = "Prescription updated successfully" }, JsonRequestBehavior.AllowGet);
            }

            //If ID exists
            var presFile = PrescriptionProcessor.LoadPrescriptionsH(prescriptionHeaderFile.PresHCode);

            Debug.WriteLine(PrescriptionProcessor.PrescriptionExist());

            if (PrescriptionProcessor.PrescriptionExist())
                return Json(new { success = false, message = $"Prescription code already exists.\nPrescription code: {prescriptionHeaderFile.PresHCode}" });

            dataAffectedHeader = PrescriptionProcessor.CreatePrescriptionH(prescriptionHeaderFile.PresHCode, prescriptionHeaderFile.PresHConsNo, prescriptionHeaderFile.PresHPatCode, prescriptionHeaderFile.PresHDate, User.Identity.GetUserId());

            foreach (PrescriptionDetailFile detailFile in prescriptionHeaderFile.PrescriptionDetails)
                dataAffectedDetail = PrescriptionProcessor.CreatePrescriptionD(prescriptionHeaderFile.PresHCode, detailFile.PreDMedCode, detailFile.PreDRemarks, detailFile.PreDQty);

            return Json(new { success = true, message = "Prescription saved successfully" }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Doctor")]
        public ActionResult ViewPrescriptionList()
        {
            ViewBag.Message = "View Prescription List";

            var headerFile = PrescriptionProcessor.LoadPrescriptionsH();

            List<PrescriptionHeaderFile> presHList = new List<PrescriptionHeaderFile>();

            if (!(headerFile.Count() > 0))
                return View(presHList);

            foreach (var row in headerFile)
                presHList.Add(new PrescriptionHeaderFile
                {
                    PresHCode = row.PresHCode,
                    PresHConsNo = row.PresHConsNo,
                    PresHPatCode = row.PresHPatCode,
                    PresHDate = DateTime.ParseExact(row.PresHDate, "MM/dd/yyyy HH:mm:ss", null)
                });

            return View(presHList);
        }

        [Authorize(Roles = "Doctor")]
        public ActionResult EditPrescription(int presCode, int oldCode)
        {
            ViewBag.Message = "Edit Prescription";

            var presHFile = PrescriptionProcessor.LoadPrescriptionsH(presCode);

            PrescriptionHeaderFile prescriptionFile = new PrescriptionHeaderFile
            {
                PresHCode = presHFile.PresHCode,
                PresHConsNo = presHFile.PresHConsNo,
                PresHPatCode = presHFile.PresHPatCode,
                PresHDate = DateTime.ParseExact(presHFile.PresHDate, "MM/dd/yyyy HH:mm:ss", null),
                PrescriptionDetails = GetPrescriptionDetails(presHFile.PresHCode),
                OldCode = oldCode
            };

            return View("Prescription", prescriptionFile);
        }

        [Authorize(Roles = "Doctor")]
        public ActionResult DeletePrescription(int presCode)
        {
            ViewBag.Message = "Delete Prescription";

            int dataDetail = PrescriptionProcessor.DeletePrescriptionD(presCode);
            int dataHeader = PrescriptionProcessor.DeletePrescriptionH(presCode);

            var headerFile = PrescriptionProcessor.LoadPrescriptionsH();

            List<PrescriptionHeaderFile> presHList = new List<PrescriptionHeaderFile>();

            if (!(headerFile.Count() > 0))
                return View("ViewPrescriptionList", presHList);

            foreach (var row in headerFile)
                presHList.Add(new PrescriptionHeaderFile
                {
                    PresHCode = row.PresHCode,
                    PresHConsNo = row.PresHConsNo,
                    PresHPatCode = row.PresHPatCode,
                    PresHDate = DateTime.ParseExact(row.PresHDate, "MM/dd/yyyy HH:mm:ss", null)
                });

            return View("ViewPrescriptionList", presHList);
        }

        [Authorize(Roles = "Doctor")]
        public ActionResult ViewPrescription(int presCode)
        {
            ViewBag.Message = "View Prescription";

            var presHFile = PrescriptionProcessor.LoadPrescriptionsH(presCode);
            
            PrescriptionHeaderFile prescriptionFile = new PrescriptionHeaderFile
            {
                PresHCode = presHFile.PresHCode,
                PresHConsNo = presHFile.PresHConsNo,
                PresHPatCode = presHFile.PresHPatCode,
                PresHDate = DateTime.ParseExact(presHFile.PresHDate, "MM/dd/yyyy HH:mm:ss", null),
                PrescriptionDetails = GetPrescriptionDetails(presHFile.PresHCode) //Oh yes
            };

            return View(prescriptionFile);
        }

        private List<PrescriptionDetailFile> GetPrescriptionDetails(int presCode)
        {
            var presDFile = PrescriptionProcessor.LoadPrescriptionsD(presCode);

            List<PrescriptionDetailFile> prescriptionDetails = new List<PrescriptionDetailFile>();

            foreach (var row in presDFile)
                prescriptionDetails.Add(new PrescriptionDetailFile
                {
                    PreDMedCode = row.PreDMedCode,
                    PreDQty = row.PreDQty,
                    PreDRemarks = row.PreDRemarks
                });

            return prescriptionDetails;
        }
        #endregion

        #region Billing
        [Authorize(Roles = "Nurse, Secretary")]
        public ActionResult Billing(int oldCode)
        {
            ViewBag.Message = "Your contact page.";

            BillingHeaderFile billingHeaderFile = new BillingHeaderFile
            {
                OldCode = oldCode
            };
            billingHeaderFile.BillingDetails.Insert(0, new BillingDetailFile());

            return View(billingHeaderFile);
        }
        [HttpPost]
        public ActionResult Billing(BillingHeaderFile billHeaderFile)
        {
            ViewBag.Message = "Add/Update Billing";

            int dataAffectedHeader;
            int dataAffectedDetail;

            Debug.WriteLine($"Header Code: {billHeaderFile.BillHNo}");
            Debug.WriteLine($"Date: {billHeaderFile.BillHDate}");
            Debug.WriteLine($"Patient Code: {billHeaderFile.BillHPatCode}");
            Debug.WriteLine("- - - - - - - - - -");
            foreach (BillingDetailFile detailFile in billHeaderFile.BillingDetails)
            {
                Debug.WriteLine($"Count: {detailFile.BillDCount} | Description: {detailFile.BillDDesc} | Amount: {detailFile.BillDAmount}");
            }
            Debug.WriteLine($"Total Amount: {billHeaderFile.BillHTotAmt}");

            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Please fill in the required inputs. Thank you!" });

            if (billHeaderFile.OldCode > 0)
            {
                //Update
                dataAffectedHeader = BillingProcessor.UpdateBill(billHeaderFile.BillHNo, billHeaderFile.BillHPatCode, billHeaderFile.BillHTotAmt, billHeaderFile.BillHDate, User.Identity.GetUserId(), billHeaderFile.OldCode);

                foreach (BillingDetailFile detailFile in billHeaderFile.BillingDetails)
                    dataAffectedDetail = BillingProcessor.CreateBillingD(billHeaderFile.BillHNo, detailFile.BillDCount, detailFile.BillDAmount, detailFile.BillDDesc);

                return Json(new { success = true, message = "Bill updated successfully" }, JsonRequestBehavior.AllowGet);
            }

            //If ID exists
            var presFile = BillingProcessor.LoadBillingH(billHeaderFile.BillHNo);

            if (BillingProcessor.BillExists())
                return Json(new { success = false, message = $"Bill code already exists.\nBill code: {billHeaderFile.BillHNo}" });

            dataAffectedHeader = BillingProcessor.CreateBillingH(billHeaderFile.BillHNo, billHeaderFile.BillHPatCode, billHeaderFile.BillHDate, billHeaderFile.BillHTotAmt, User.Identity.GetUserId());

            foreach (BillingDetailFile detailFile in billHeaderFile.BillingDetails)
                dataAffectedDetail = BillingProcessor.CreateBillingD(billHeaderFile.BillHNo, detailFile.BillDCount, detailFile.BillDAmount, detailFile.BillDDesc);

            return Json(new { success = true, message = "Bill saved successfully" }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Nurse, Secretary")]
        public ActionResult ViewBillingList()
        {
            ViewBag.Message = "View Medicine List";

            var headerFile = BillingProcessor.LoadBillingH();

            List<BillingHeaderFile> billList = new List<BillingHeaderFile>();

            if (!(headerFile.Count() > 0))
                return View(billList);

            foreach (var row in headerFile)
                billList.Add(new BillingHeaderFile
                {
                    BillHNo = row.BillHNo,
                    BillHPatCode = row.BillHPatCode,
                    BillHTotAmt = row.BillHTotAmt,
                    BillHDate = DateTime.ParseExact(row.BillHDate, "MM/dd/yyyy HH:mm:ss", null)
                });

            return View(billList);
        }

        [Authorize(Roles = "Nurse, Secretary")]
        public ActionResult EditBill(int billCode, int oldCode)
        {
            ViewBag.Message = "Edit Bill";

            var billHFile = BillingProcessor.LoadBillingH(billCode);

            BillingHeaderFile billFile = new BillingHeaderFile
            {
                BillHNo = billHFile.BillHNo,
                BillHDate = DateTime.ParseExact(billHFile.BillHDate, "MM/dd/yyyy HH:mm:ss", null),
                BillHPatCode = billHFile.BillHPatCode,
                BillHTotAmt = billHFile.BillHTotAmt,
                BillingDetails = GetBillingDetails(billHFile.BillHNo),
                OldCode = oldCode
            };

            return View("Billing", billFile);
        }

        [Authorize(Roles = "Nurse, Secretary")]
        public ActionResult DeleteBilling(int billCode)
        {
            ViewBag.Message = "Delete Medicine";

            int dataDetail = BillingProcessor.DeleteBillingD(billCode);
            int dataHeader = BillingProcessor.DeleteBillingH(billCode);

            var headerFile = BillingProcessor.LoadBillingH();

            List<BillingHeaderFile> billList = new List<BillingHeaderFile>();

            if (!(headerFile.Count() > 0))
                return View("ViewBillingList", billList);

            foreach (var row in headerFile)
                billList.Add(new BillingHeaderFile
                {
                    BillHNo = row.BillHNo,
                    BillHDate = DateTime.ParseExact(row.BillHDate, "MM/dd/yyyy HH:mm:ss", null),
                    BillHPatCode = row.BillHPatCode,
                    BillHTotAmt = row.BillHTotAmt,
                });

            return View("ViewBillingList", billList);
        }

        [Authorize(Roles = "Nurse, Secretary")]
        public ActionResult ViewBill(int billCode)
        {
            ViewBag.Message = "View Bill";

            var billFile = BillingProcessor.LoadBillingH(billCode);

            BillingHeaderFile prescriptionFile = new BillingHeaderFile
            {
                BillHNo = billFile.BillHNo,
                BillHDate = DateTime.ParseExact(billFile.BillHDate, "MM/dd/yyyy HH:mm:ss", null),
                BillHPatCode = billFile.BillHPatCode,
                BillHTotAmt = billFile.BillHTotAmt,
                BillingDetails = GetBillingDetails(billFile.BillHNo) //Oh yes
            };

            return View(prescriptionFile);
        }

        private List<BillingDetailFile> GetBillingDetails(int billCode)
        {
            var billFile = BillingProcessor.LoadBillingD(billCode);

            List<BillingDetailFile> billDetails = new List<BillingDetailFile>();

            foreach (var row in billFile)
                billDetails.Add(new BillingDetailFile
                {
                    BillDCount = row.BillDCount,
                    BillDAmount = row.BillDAmount,
                    BillDDesc = row.BillDDesc
                });

            return billDetails;
        }
        #endregion
    }
}