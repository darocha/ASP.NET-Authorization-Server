using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Onyx.Authorization.Controllers.MVC
{
    [RoutePrefix("/unauthorized")]
    public class UnauthorizedController : Controller
    {
        // GET: Notfound
        public ActionResult Index()
        {
            Response.StatusCode = 401;
            return View();
        }
    }
}