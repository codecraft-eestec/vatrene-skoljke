﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CrarftedFood.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports
        public ActionResult Index(bool delivery, bool order, bool invoice)
        {
            return View();
        }


    }
}