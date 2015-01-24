using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Onyx.Authorization.Controllers.MVC
{
    [RoutePrefix("/forbidden")]
    public class ForbiddenController : Controller
    {
        // GET: Forbidden
        public ActionResult Index()
        {
            Response.StatusCode = 403;
            return View();
        }
    }
}