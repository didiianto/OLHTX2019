using ClosedXML.Excel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using OLHTX2019.Helper;
using OLHTX2019.Models.DB;
using OLHTX2019.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OLHTX2019.Controllers
{
    public class Admin20191008Controller : BaseController
    {
        // GET: Admin20191008

        [AuthorizeCustom]
        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult Dashboard_Grid([DataSourceRequest] DataSourceRequest request, string filterBy)
        {
            return Json(GetRegistrationList(filterBy).ToDataSourceResult(request));
        }

        private IEnumerable<RegistrationViewModel> GetRegistrationList(string filterBy)
        {
            List<Registration> regList = unitOfWork.RegistrationRepository.Get().ToList();

            if (!String.IsNullOrEmpty(filterBy))
            {
                string[] filterArray = filterBy.Split(',');

                foreach(string filterKey in filterArray)
                {
                    if (!String.IsNullOrEmpty(filterKey))
                    {
                        if (filterKey == "INVT" || filterKey == "READ" || filterKey == "CLICK" || filterKey == "CPLT")
                            regList = regList.Where(x => x.StepsAction == filterKey).ToList();
                        if (filterKey == "AllRegistered")
                            regList = regList.Where(x => x.Status == "OK" || x.Status == "DFT").ToList();
                        if (filterKey == "OK")
                            regList = regList.Where(x => x.Status == filterKey).ToList();
                        if (filterKey == "DFT")
                            regList = regList.Where(x => x.Status == filterKey).ToList();
                        if (filterKey == "AllGeneric")
                            regList = regList.Where(x => x.IsOpenForm == true || x.IsOpenForm == false).ToList();
                        if (filterKey == "True" || filterKey == "False")
                        {
                            bool isOpenForm = false;
                            Boolean.TryParse(filterKey, out isOpenForm);
                            regList = regList.Where(x => x.IsOpenForm == isOpenForm).ToList();
                        }
                    }
                }

            }


            string loginId = Security.GetCurrentUser();
            var user = unitOfWork.AdministratorRepository.Get(filter: x => x.LoginId == loginId).Single();

            return regList.Select(reg => new RegistrationViewModel
            {
                IdStringEncypted = GeneralHelper.Base64Encode(reg.Id.ToString()),
                Id = reg.Id,
                SerialNo = reg.SerialNo,
                Name = reg.Name,
                Designation = reg.Designation,
                //IDType = GeneralHelper.GetIdTypeName(reg.IDType),
                //IDNumber = reg.IDNumber,
                Mobile = reg.Mobile,
                Email = reg.Email,
                Organisation = reg.Organisation,
                StepAction = GeneralHelper.GetRegStepStatus(reg.StepsAction),
                OpenForm = reg.IsOpenForm == true ? "Yes" : "No",
                DateSubmited = reg.DateSubmited,
                AdminRole = user.Role
            });
            
        }

        [AuthorizeCustom]
        public ActionResult DeleteRegistration(Guid id)
        {
            if(id == Guid.Empty || id == null)
                return Json(new { success = false, errorMessage = "Data not found." });
            else
            {
                unitOfWork.RegistrationRepository.Delete(id);
                unitOfWork.Save();
            }
            
            return Json(new { success = true });
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult GetRegList()
        {
            var regs = unitOfWork.RegistrationRepository.Get();
            RegistrationListViewModel model = new RegistrationListViewModel();
            model.RegistrationViewModelList = new List<RegistrationViewModel>();
            foreach (var item in regs)
            {
                RegistrationViewModel mItem = new RegistrationViewModel();
                mItem.Id = item.Id;
                model.RegistrationViewModelList.Add(mItem);
            }
            return View(model);
        
        }

        [HttpPost]
        public ActionResult Login(string loginId, string password)
        {
            string errorMessage = "";
            errorMessage += Validate.ValidStringField(loginId, "Login ID", true, 50);
            errorMessage += Validate.ValidStringField(password, "Password", true, 50);
            if(!String.IsNullOrEmpty(errorMessage))
                return Json(new { success = true, errorMessage = errorMessage });

            List<Administrator> administrator = unitOfWork.AdministratorRepository.Get(filter: x => x.LoginId == loginId && x.Password == password).ToList();
            if(administrator.Count() == 1)
            {
                if(administrator[0].IsRequiredOTP == false)
                {
                    administrator[0].DateLastLogin = DateTime.Now;
                    unitOfWork.AdministratorRepository.Update(administrator[0]);
                    unitOfWork.Save();

                    Security.SetCurrentUser(loginId);


                    return Json(new { success = true, errorMessage = "jump-otp" });
                }
                else
                {
                    administrator[0].OtpCode = Guid.NewGuid().ToString().Substring(0, 6);
                    administrator[0].DateOtpGenerated = DateTime.Now;

                    unitOfWork.AdministratorRepository.Update(administrator[0]);
                    unitOfWork.Save();
                    if (!String.IsNullOrEmpty(administrator[0].Mobile))
                        SMS.SendSMSToAdmin(administrator[0].Mobile, administrator[0].OtpCode);
                    else
                        Email.SenOTPCOde(administrator[0].Id, unitOfWork);

                    return Json(new { success = true, errorMessage = "keyed-otp" });
                }
            }

            return Json(new { success = false, errorMessage = "Invalid Login ID or Password." });
        }

        [HttpPost]
        public ActionResult SetLoginSession(string loginId, string otpCode)
        {
            List<Administrator> admin = unitOfWork.AdministratorRepository.Get(filter: x => x.LoginId == loginId && x.OtpCode == otpCode).ToList();
            if(admin.Count == 1)
            {
                admin[0].DateLastLogin = DateTime.Now;
                unitOfWork.AdministratorRepository.Update(admin[0]);
                unitOfWork.Save();

                Security.SetCurrentUser(loginId);
                

                return Json(new { success = true });
            }
            return Json(new { success = false, errorMessage = "Invalid Otp Code" });
        }

        [AuthorizeCustom]
        [HttpPost]
        public FileResult GenerateReport()
        {
            XLWorkbook workbook = new XLWorkbook();
            IXLWorksheet workSheet = null;
            MemoryStream memory = new MemoryStream();

            int x = 1;
            int y = 1;

            string sheetName = "Sheet1";
            workSheet = workbook.Worksheets.Add(sheetName);

            workSheet.Cell(y, x).SetValue("No."); x++;
            workSheet.Cell(y, x).SetValue("Serial No"); x++;
            workSheet.Cell(y, x).SetValue("Name"); x++;
            workSheet.Cell(y, x).SetValue("Designation"); x++;
            workSheet.Cell(y, x).SetValue("Organisation"); x++;
            //workSheet.Cell(y, x).SetValue("ID Type"); x++;
            //workSheet.Cell(y, x).SetValue("ID Number"); x++;
            workSheet.Cell(y, x).SetValue("Mobile"); x++;
            workSheet.Cell(y, x).SetValue("Email"); x++;
            workSheet.Cell(y, x).SetValue("Generic"); x++;
            workSheet.Cell(y, x).SetValue("Date of Submit"); x++;

            workSheet.SheetView.Freeze(1, 1); //freeze header

            y = 2;
            int count = 1;
            var regList = unitOfWork.RegistrationRepository.Get(filter => filter.Status == RegStatus.OK.ToString()).OrderBy(r=>r.DateSubmited);
            foreach(var item in regList)
            {
                x = 1;
                workSheet.Cell(y, x).SetValue(count.ToString()); x++;
                workSheet.Cell(y, x).SetValue(item.SerialNo); x++;
                workSheet.Cell(y, x).SetValue(item.Name); x++;
                workSheet.Cell(y, x).SetValue(item.Designation); x++;
                workSheet.Cell(y, x).SetValue(item.Organisation); x++;
                //workSheet.Cell(y, x).SetValue(GeneralHelper.GetIdTypeName(item.IDType)); x++;
                //workSheet.Cell(y, x).SetValue(item.IDNumber); x++;
                workSheet.Cell(y, x).SetValue(item.Mobile); x++;
                workSheet.Cell(y, x).SetValue(item.Email); x++;
                workSheet.Cell(y, x).SetValue(item.IsOpenForm == true ? "Yes" : "No"); x++;
                workSheet.Cell(y, x).SetValue(((DateTime)item.DateSubmited).ToString("dd MMM yyyy HH:mm:ss")); x++;
                y++;
                count++;
                
            }
            workbook.SaveAs(memory);
            return File(memory.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Report.xlsx");
        }

        [AuthorizeCustom]
        [HttpPost]
        public FileResult GenerateReportAll()
        {
            XLWorkbook workbook = new XLWorkbook();
            IXLWorksheet workSheet = null;
            MemoryStream memory = new MemoryStream();

            int x = 1;
            int y = 1;

            string sheetName = "Sheet1";
            workSheet = workbook.Worksheets.Add(sheetName);

            workSheet.Cell(y, x).SetValue("No."); x++;
            workSheet.Cell(y, x).SetValue("Serial No"); x++;
            workSheet.Cell(y, x).SetValue("Name"); x++;
            workSheet.Cell(y, x).SetValue("Designation"); x++;
            workSheet.Cell(y, x).SetValue("Organisation"); x++;
            //workSheet.Cell(y, x).SetValue("ID Type"); x++;
            //workSheet.Cell(y, x).SetValue("ID Number"); x++;
            workSheet.Cell(y, x).SetValue("Mobile"); x++;
            workSheet.Cell(y, x).SetValue("Email"); x++;
            workSheet.Cell(y, x).SetValue("Image"); x++;
            workSheet.Cell(y, x).SetValue("Generic"); x++;            
            workSheet.Cell(y, x).SetValue("Status"); x++;
            workSheet.Cell(y, x).SetValue("Date of Submit"); x++;

            workSheet.SheetView.Freeze(1, 1); //freeze header

            y = 2;
            int count = 1;
            //var regList = unitOfWork.RegistrationRepository.Get(filter => filter.Status == RegStatus.OK.ToString()).OrderBy(r => r.DateSubmited);
            var regList = unitOfWork.RegistrationRepository.Get().OrderBy(r => r.DateSubmited);
            foreach (var item in regList)
            {

                // STEP ACTION
                // DFT = DRAFT; INVT = INVITATION; READ; CLICK; CPLT = COMPLETE
                string stepAction = "DRAFT";
                if (item.StepsAction == "INVT")
                    stepAction = "INVITATION";
                else if (item.StepsAction == "READ")
                    stepAction = "READ";
                else if (item.StepsAction == "CLICK")
                    stepAction = "CLICK";
                else if (item.StepsAction == "CPLT")
                    stepAction = "COMPLETE";


                x = 1;
                workSheet.Cell(y, x).SetValue(count.ToString()); x++;
                workSheet.Cell(y, x).SetValue(item.SerialNo); x++;
                workSheet.Cell(y, x).SetValue(item.Name); x++;
                workSheet.Cell(y, x).SetValue(item.Designation); x++;
                workSheet.Cell(y, x).SetValue(item.Organisation); x++;
                //workSheet.Cell(y, x).SetValue(GeneralHelper.GetIdTypeName(item.IDType)); x++;
                //workSheet.Cell(y, x).SetValue(item.IDNumber); x++;
                workSheet.Cell(y, x).SetValue(item.Mobile); x++;
                workSheet.Cell(y, x).SetValue(item.Email); x++;
                workSheet.Cell(y, x).SetValue(item.Image == null ? "No" : "Yes"); x++;
                workSheet.Cell(y, x).SetValue(item.IsOpenForm == true ? "Yes" : "No"); x++;                
                workSheet.Cell(y, x).SetValue(stepAction); x++;
                workSheet.Cell(y, x).SetValue(item.DateSubmited != null ? ((DateTime)item.DateSubmited).ToString("dd MMM yyyy HH:mm:ss") : ""); x++;
                y++;
                count++;

            }
            workbook.SaveAs(memory);
            return File(memory.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Report.xlsx");
        }
        public ActionResult PrintAllBadge()
        {
            List<Registration> regList = unitOfWork.RegistrationRepository.Get(filter: x => x.Status == RegStatus.OK.ToString()).ToList();
            return File(GeneralHelper.GenerateAllBadge(regList, unitOfWork), "application/pdf");
        }
        public ActionResult PrintSingleBadge()
        {
            return File(GeneralHelper.GenerateSingleBadge(new Guid("2a2fe654-19bf-45d0-a42b-68673e481004"), unitOfWork), "application/pdf");
        }

        [AuthorizeCustom]
        public ActionResult ResendConfirmation(Guid id)
        {
            if(id == Guid.Empty || id == null)
            {
                Response.StatusCode = 404;
            }

            Email.SendConfirmation(id, unitOfWork);
            return RedirectToAction("Dashboard");
        }

        [AuthorizeCustom]
        public ActionResult Invite(Guid id)
        {
            if (id == Guid.Empty || id == null)
            {
                Response.StatusCode = 404;
            }

            Email.SentInvite(id, unitOfWork);
            
            return Json(new { success = true, message = "Invitation sent."});
        }


        public ActionResult UpoadInvite()
        {
            return View();
        }

        [AuthorizeCustom]
        public ActionResult BlastInvite()
        {
            //int count = 0;
            //DateTime dateCreated = DateTime.Parse("2019-10-15 17:36:00.000"); //should be change everytime want to blast
            //var regs = unitOfWork.RegistrationRepository.Get(filter: x => x.DateCreated == dateCreated && x.Status != RegStatus.OK.ToString() && x.StepsAction == RegStepAction.DFT.ToString());
            //foreach(var item in regs)
            //{
            //    try
            //    {
            //        Email.SentInvite(item.Id, unitOfWork);
            //    }
            //    catch { }
            //}

            return RedirectToAction("Dashboard");

            //return Content("<script language='javascript' type='text/javascript'>alert('"+ count + "');</script>");
        }
        
        public ActionResult SaveImageToDrive()
        {
            var regs = unitOfWork.RegistrationRepository.Get(filter: x => x.Status == RegStatus.OK.ToString());
            var setting = GeneralHelper.GetSetting(unitOfWork);
            foreach (var item in regs)
            {
                string photoDirectory = setting.WebLocation + "Storage\\UserPhoto";
                if (!Directory.Exists(photoDirectory))
                {
                    Directory.CreateDirectory(photoDirectory);
                }
                
                System.IO.File.WriteAllBytes(photoDirectory + "\\" + item.Id.ToString() + ".jpg", item.Image);
            }

            return RedirectToAction("Dashboard");
        }
    }
}