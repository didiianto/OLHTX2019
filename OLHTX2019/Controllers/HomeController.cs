using OLHTX2019.Helper;
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
    public class HomeController : BaseController
    {
        // GET: Home
        public ActionResult Index(string id)
        {
            Guid _id = Guid.Empty;

            if (String.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index", "Error");
            }
            else
            {
                if (Validate.IsRegFormOpenForm(id))
                {
                    RegistrationViewModel openFormModel = new RegistrationViewModel();
                    openFormModel.IdStringEncypted = id; //store open form key as temporary on IdStringEncypted
                    return View(openFormModel);
                }
                else
                {
                    try
                    {
                        id = GeneralHelper.Base64Decode(id);
                        Guid.TryParse(id, out _id);
                    }
                    catch
                    {
                        return RedirectToAction("Index", "Error");
                    }
                }
            }

            var reg = unitOfWork.RegistrationRepository.GetByID(_id);
            if(reg.Status == RegStatus.OK.ToString())
            {
                return RedirectToAction("ThankYou", "Registration");
            }
            RegistrationViewModel model = new RegistrationViewModel();
            model.Id = reg.Id;
            model.IdStringEncypted = GeneralHelper.Base64Encode(id.ToString());

            return View(model);
        }

        public ActionResult TrackEmail(string id)
        {
            Guid _id = Guid.Empty;

            if (String.IsNullOrEmpty(id))
            {
                Response.StatusCode = 409;
            }
            else
            {
                try
                {
                    id = GeneralHelper.Base64Decode(id);
                    Guid.TryParse(id, out _id);
                }
                catch
                {
                    Response.StatusCode = 409;
                }
            }

            var reg = unitOfWork.RegistrationRepository.GetByID(_id);
            var setting = GeneralHelper.GetSetting(unitOfWork);
            if(reg.StepsAction == RegStepAction.INVT.ToString() || reg.StepsAction == RegStepAction.DFT.ToString())
                reg.StepsAction = RegStepAction.READ.ToString(); //Email read
            unitOfWork.RegistrationRepository.Update(reg);
            unitOfWork.Save();

            var weblocation = setting.WebLocation + "Images\\";
            var path = Path.Combine(weblocation, "dot.jpg"); //validate the path for security or use other means to generate the path.
            return base.File(path, "image/jpeg");
        }

        public ActionResult Invite()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Invite(string email)
        {
            Email.SentInviteOpenForm(email, unitOfWork);
            return Json(new { success = true });
        }

    }
}