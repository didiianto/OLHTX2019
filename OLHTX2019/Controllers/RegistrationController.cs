using OLHTX2019.Helper;
using OLHTX2019.Models.DB;
using OLHTX2019.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OLHTX2019.Controllers
{
    public class RegistrationController : BaseController
    {
        public ActionResult Register(string id)
        {
            RegistrationViewModel model = new RegistrationViewModel();
            if(!String.IsNullOrEmpty(id))
            {
                if (Validate.IsRegFormOpenForm(id))
                {
                    model.IdStringEncypted = id; //store open form key as temporary on IdStringEncypted
                }
                else
                {
                    Guid _id = Guid.Empty;
                    try
                    {
                        id = GeneralHelper.Base64Decode(id);
                        Guid.TryParse(id, out _id);
                    }
                    catch
                    {
                        return RedirectToAction("Index", "Error");
                    }

                    var reg = unitOfWork.RegistrationRepository.GetByID(_id);

                    if (reg.Status == RegStatus.OK.ToString() && !Security.IsAdminLogin())
                    {
                        return RedirectToAction("ThankYou");
                    }
                    if (reg.StepsAction == RegStepAction.READ.ToString())
                        reg.StepsAction = RegStepAction.CLICK.ToString(); // link is clicked

                    unitOfWork.RegistrationRepository.Update(reg);
                    unitOfWork.Save();

                    model.Id = reg.Id;
                    model.IdStringEncypted = GeneralHelper.Base64Encode(reg.Id.ToString());
                    model.Name = reg.Name;
                    //model.IDType = reg.IDType;
                    //model.IDNumber = reg.IDNumber;
                    model.Mobile = reg.Mobile;
                    model.Email = reg.Email;
                    model.Organisation = reg.Organisation;
                    model.Designation = reg.Designation;
                    model.Image = reg.Image;
                }
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }

            return View(model);
        }
        
        [HttpPost]
        public ActionResult Register
            (
            string id,
            string name,
            string designation,
            string mobile,
            string email,
            string organisation,
            HttpPostedFileBase file
            )
        {
            string Validation = ValidateRegistration(name, designation, mobile, email, organisation);

            List<Registration> regList = null;
            if(!String.IsNullOrEmpty(id) && !Validate.IsRegFormOpenForm(id))
            {
                Guid _id = Guid.Empty;
                Guid.TryParse(id, out _id);
                regList = unitOfWork.RegistrationRepository.Get(filter: x => x.Email == email && x.Id != _id).ToList();
                if(regList.Count > 0)
                {
                    return Json(new { success = false, errorMessage = "You are already registered." });
                }

            }
            else if (!String.IsNullOrEmpty(id) && Validate.IsRegFormOpenForm(id))
            {
                regList = unitOfWork.RegistrationRepository.Get(filter: x => x.Email == email).ToList();
                if (regList.Count > 0)
                {
                    return Json(new { success = false, errorMessage = "You are already registered." });
                }
            }

            if (file != null && file.ContentLength > 0)
            {
                var extention = Path.GetExtension(file.FileName);
                var allowedExtensions = new[] { ".jpg", ".jpeg" };
                if (!allowedExtensions.Contains(extention.ToLower()))
                    Validation += "Please upload .jpg, or .jpeg only.\n";

                if(file.ContentLength > (7 * 1024 * 1024))
                    Validation += "Photo exceed 6 MB.\n";
            }
            //else if (!Security.IsAdminLogin() && file.ContentLength <= 0) // comment, user request not mandatory on 17/10/2019
            //{
            //    Validation += "Please provide your photo.\n";
            //}

            if (!String.IsNullOrEmpty(Validation))
                return Json(new { success = false, errorMessage = Validation });

            OLHTX2019.Models.DB.Registration reg = null;

            if(!String.IsNullOrEmpty(id) && !Validate.IsRegFormOpenForm(id))
            {
                Guid _id = Guid.Empty;
                Guid.TryParse(id, out _id);

                reg = unitOfWork.RegistrationRepository.GetByID(_id);

                if (!Security.IsAdminLogin())
                {
                    if(reg.Status == RegStatus.OK.ToString())
                    {
                        return Json(new { success = false, errorMessage = "throw-to-thanks" }); // to handle if user go back form end page to reg form
                    }
                }
                
                reg.DateModified = DateTime.Now;
                reg.Id = _id;
            }
            else
            {
                reg = new Models.DB.Registration();
                reg.Id = Guid.NewGuid();
                reg.DateSubmited = DateTime.Now;
                reg.DateCreated = DateTime.Now;
                reg.IsOpenForm = true; //flag for who register from open form
            }

            reg.Name = name;
            reg.Designation = designation;
            //reg.IDType = idType;
            //reg.IDNumber = idNumber;
            reg.Mobile = mobile;
            reg.Email = email;
            reg.Organisation = organisation;

            if(file != null && file.ContentLength > 0)
            {
                reg.ImageName = Path.GetFileName(file.FileName);
                reg.ImageType = file.ContentType;
                byte[] bytes;
                using (BinaryReader br = new BinaryReader(file.InputStream))
                {
                    bytes = br.ReadBytes(file.ContentLength);
                }
                reg.Image = bytes;

            }
            

            if (!Validate.IsRegFormOpenForm(id))
            {
                unitOfWork.RegistrationRepository.Update(reg);
                Logging.CreateAuditLog(reg.Id, Security.IsAdminLogin() == true ? Security.GetCurrentUser() : "", "Update on Memory step1", Module.FrontEnd, unitOfWork);
            }
            else
            {
                unitOfWork.RegistrationRepository.Insert(reg);
                Logging.CreateAuditLog(reg.Id, "", "Insert on Memory  step1", Module.FrontEnd, unitOfWork);
            }

            unitOfWork.Save();

            if (String.IsNullOrEmpty(reg.SerialNo) && !Security.IsAdminLogin())
            {
                reg.SerialNo = GeneralHelper.GenerateSerial(unitOfWork);
                Logging.CreateAuditLog(reg.Id, "", "Update Serial No on Memory  step2 to= " + reg.SerialNo, Module.FrontEnd, unitOfWork);
                reg.DateSubmited = DateTime.Now;
                reg.Status = RegStatus.OK.ToString();
                Logging.CreateAuditLog(reg.Id, "", "Update status on Memory  step3 to= " + reg.Status, Module.FrontEnd, unitOfWork);
                reg.StepsAction = RegStepAction.CPLT.ToString(); // Complete
                Logging.CreateAuditLog(reg.Id, "", "Update Step action on Memory step4 to= " + reg.StepsAction, Module.FrontEnd, unitOfWork);
                unitOfWork.Save();
                Logging.CreateAuditLog(reg.Id, "", "Save SerialNo, Status, StepAction  step5 = " + reg.SerialNo + reg.Status + reg.StepsAction, Module.FrontEnd, unitOfWork);
            }

            if(reg.Status == RegStatus.OK.ToString() && !Security.IsAdminLogin())
            {
                Logging.CreateAuditLog(reg.Id, "", "Prepare Send Email step6", Module.FrontEnd, unitOfWork);
                Email.SendConfirmation(reg.Id, unitOfWork);
                Logging.CreateAuditLog(reg.Id, "", "Email Sent step7", Module.FrontEnd, unitOfWork);
            }


            string redirect = "";
            if (Security.IsAdminLogin())
                redirect = "/Admin20191008/Dashboard";
            else
                redirect = "/Registration/End";

            return Json(new { success = true, urlRed = redirect });
        }
        

        public ActionResult End()
        {
            Logging.CreateAuditLog(Guid.Empty, "", "End page", Module.FrontEnd, unitOfWork);
            return View();
        }

        public ActionResult ThankYou()
        {
            return View();
        }

        #region helper
        private string ValidateRegistration(
            string name,
            string designation,
            string mobile,
            string email,
            string organisation)
        {
            string errorMessage = "";
            bool isRequired = !Security.IsAdminLogin();

            errorMessage += Validate.ValidStringField(name, "Name", isRequired, 100);
            errorMessage += Validate.ValidStringField(designation, "Designation", isRequired, 150);
            errorMessage += Validate.ValidStringField(organisation, "Organisation", isRequired, 200);
            //errorMessage += Validate.ValidStringField(idType, "Identification Type", isRequired, 200);
            //if (isRequired && !String.IsNullOrEmpty(idNumber))
            //{
            //    if (idType == "1") //NRIC
            //    {
            //        if (!Validate.IsNRICValid(idNumber))
            //            errorMessage += "Incorrect NRIC number.";
            //    }
            //}
            //else if (isRequired && String.IsNullOrEmpty(idNumber))
            //{
            //    errorMessage += "ID number is required.";
            //}

            errorMessage += Validate.ValidStringField(email, "Email", isRequired, 150, "Email");;
            errorMessage += Validate.ValidStringField(mobile, "Mobile", isRequired, 15, "Phone");

            return errorMessage;
        }

        public ActionResult PastchData()
        {
            List<string> idStrList = new List<string>();
            idStrList.Add("fb1f4ed4-d758-4405-86eb-0356e7223270");
            idStrList.Add("ca02c8d3-6832-49ff-b0e1-0e13dfc52a18");
            idStrList.Add("ebafd6b7-2320-42d3-b979-112587b2769b");

            foreach (var item in idStrList)
            {
                var reg = unitOfWork.RegistrationRepository.GetByID(new Guid(item));
                if(reg.Status != RegStatus.OK.ToString())
                {
                    reg.SerialNo = GeneralHelper.GenerateSerial(unitOfWork);
                    Logging.CreateAuditLog(reg.Id, "", "Update Serial No on Memory  step2 to= " + reg.SerialNo, Module.FrontEnd, unitOfWork);
                    reg.DateSubmited = DateTime.Now;
                    reg.Status = RegStatus.OK.ToString();
                    Logging.CreateAuditLog(reg.Id, "", "Update status on Memory  step3 to= " + reg.Status, Module.FrontEnd, unitOfWork);
                    reg.StepsAction = RegStepAction.CPLT.ToString(); // Complete
                    Logging.CreateAuditLog(reg.Id, "", "Update Step action on Memory step4 to= " + reg.StepsAction, Module.FrontEnd, unitOfWork);
                    unitOfWork.Save();
                    Logging.CreateAuditLog(reg.Id, "", "Save SerialNo, Status, StepAction  step5 = " + reg.SerialNo + reg.Status + reg.StepsAction, Module.FrontEnd, unitOfWork);
                    
                    Logging.CreateAuditLog(reg.Id, "", "Prepare Send Email step6", Module.FrontEnd, unitOfWork);
                    Email.SendConfirmation(reg.Id, unitOfWork);
                    Logging.CreateAuditLog(reg.Id, "", "Email Sent step7", Module.FrontEnd, unitOfWork);
                }
            }

            return RedirectToAction("Dashboard", "Admin20191008");
        }

        public ActionResult CheckUniqueEmail(string email, string userId)
        {
            if (!String.IsNullOrEmpty(userId))
            {
                List<Registration> regList = null;
                if (!Validate.IsRegFormOpenForm(userId))
                {
                    userId = GeneralHelper.Base64Decode(userId);
                    Guid id = Guid.Empty;
                    Guid.TryParse(userId, out id);
                    regList = unitOfWork.RegistrationRepository.Get(filter: x => x.Email == email && x.Id != id).ToList();
                    if(regList.Count > 0)
                        return Json(new { success = false});
                }
                else
                {
                    regList = unitOfWork.RegistrationRepository.Get(filter: x => x.Email == email).ToList();
                    if (regList.Count > 0)
                        return Json(new { success = false });
                }
                
            }
            else
            {
                return Json(new { success = false });
            }

            return Json(new { success = true });
        }


        public ActionResult GetImage(Guid id)
        {
            var setting = GeneralHelper.GetSetting(unitOfWork);
            var reg = unitOfWork.RegistrationRepository.GetByID(id);

            byte[] imgData;
            if (reg.Image == null)
                imgData = System.IO.File.ReadAllBytes(setting.WebLocation + "Images\\dummy.png");
            else
                imgData = reg.Image;

            return File(imgData, "image/png");
        }


        #endregion
    }
}