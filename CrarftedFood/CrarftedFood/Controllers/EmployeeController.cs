﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CrarftedFood.Models;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using Data;

namespace CrarftedFood.Controllers
{
    public class EmployeeController : Controller
    {
        #region LIST OF EMPLOYEES AND PROFILES
        public ActionResult Index()
        {
            EmployeesViewModel model = EmployeesViewModel.Load();
            //TODO view
            return View(model);
        }

        public ActionResult Profiles(int empId)
        {
            ShowEmployeeViewModel model = ShowEmployeeViewModel.Load(empId);
            return View();
        }
        #endregion

        #region ADD
        public ActionResult AddEmployee()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddEmployee(AddEmployeeViewModel model)
        {
            string pass = Membership.GeneratePassword(7, 0);
            string hashedPass = Data.Entities.HashPassword.SaltedHashPassword(pass, model.Email);
            Data.Entities.Employees.AddEmployee(model.Name, model.Email, hashedPass, model.Role);

            string body = "<p>Poštovani {0},</p> <p> Upravo ste dodati u bazu Crafted Food radi lakšeg naručivanja hrane kao <strong>{1}</strong>, Vaši podaci za logovanje su: <br> username: {2} <br>  password: <font color=blue>{3}</p><p>Pozdrav</p>";
            string message = string.Format(body, model.Name, model.Role, model.Email, pass);
            await SendEmail(model.Email, "Welcome to Craft Food", message);
            return View();
        }
        #endregion

        #region PASSWORD RECOVERY
        [HttpPost]
        public async Task<ActionResult> PasswordRecovery(string email)
        {
            List<object> obj = Data.Entities.Employees.PasswordRecovery(email);
            if (obj.Any())
            {
                string body = "<p>Poštovani {0},</p> <p> Vaša šifra je restartovana, Vaši novi podaci za logovanje su: <br> username: {1} <br>  password: <font color=blue>{2}</p><p>Pozdrav</p>";
                string message = string.Format(body, obj[0], obj[1], obj[2]);
                await SendEmail(email, "Password Recovery", message);

                return Json(new { success = true, message = "recovered" });
            }
            return Json(new { success = false, message = "deleted" });
        }

        public async Task SendEmail(string email, string title, string body, byte[] pdf = null)
        {
            //MemoryStream stream = new MemoryStream(pdf);
            string admin = "vatreneskoljke@gmail.com";
            string adminpass = "seashellsonfire";

            var message = new MailMessage();
            message.To.Add(new MailAddress(email));
            message.From = new MailAddress(admin);
            message.Subject = title;
            message.Body = body;
            message.IsBodyHtml = true;
            //message.Attachments.Add(new Attachment(stream, "Request.pdf", System.Net.Mime.MediaTypeNames.Application.Pdf));


            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = admin,
                    Password = adminpass
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(message);
            }
        }
        #endregion

        #region ADMIN EDIT
        public ActionResult EditEmployee(int empId)
        {
            ShowEmployeeViewModel model = ShowEmployeeViewModel.Load(empId);
            //TODO view
            return View(model);
        }

        [HttpPost]
        public ActionResult EditEmployee(ShowEmployeeViewModel model)
        {
            Data.Entities.Employees.EditEmployee(model.Id, model.Name, model.Email, model.Mobile, model.Role);
            return RedirectToAction("Profile", model.Id);
        }
        #endregion
        
        #region EDIT
        public ActionResult EditProfile(int empId)
        {
            EditEmployeeViewModel model = EditEmployeeViewModel.Load(empId);
            //TODO view
            return View(model);
        }

        [HttpPost]
        public ActionResult EditProfile(ShowEmployeeViewModel model)
        {
            Data.Entities.Employees.EditEmployee(model.Id, model.Name, model.Email, model.Mobile);
            return RedirectToAction("Profile", model.Id);
        }
        #endregion

        #region DELETE
        [HttpPost]
        public ActionResult DeleteEmployee(int empId)
        {
            Data.Entities.Employees.DeleteEmployee(empId);
            return RedirectToAction("Index");
        }
        #endregion
    }
}