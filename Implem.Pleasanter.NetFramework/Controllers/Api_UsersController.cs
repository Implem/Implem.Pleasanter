﻿using Implem.Pleasanter.NetFramework.Libraries.Requests;
using System.Web.Mvc;
namespace Implem.Pleasanter.NetFramework.Controllers
{
    [AllowAnonymous]
    public class Api_UsersController : Controller
    {
        [HttpPost]
        public ContentResult Get()
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.Api_UsersController();
            var result = controller.Get(context: context);
            return result;
        }
    }
}