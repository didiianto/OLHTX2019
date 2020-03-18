using OLHTX2019.Models.EntityManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OLHTX2019.Controllers
{
    public class BaseController : Controller
    {
        public UnitOfWork unitOfWork;

        public BaseController()
        {

        }

        protected override void OnActionExecuting(ActionExecutingContext ctx)
        {
            base.OnActionExecuting(ctx);
            unitOfWork = new UnitOfWork();
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }
    }
}