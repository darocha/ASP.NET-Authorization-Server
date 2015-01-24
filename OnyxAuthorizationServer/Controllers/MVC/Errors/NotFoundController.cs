using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Onyx.Authorization.Controllers.MVC
{
    [RoutePrefix("/notfound")]
    public class NotFoundController : Controller
    {
        // GET: Notfound
        public ActionResult Index()
        {
            Response.StatusCode = 404;
            return View();
        }
    }
}