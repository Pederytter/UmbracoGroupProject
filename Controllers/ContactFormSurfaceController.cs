using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AarhusDevGroup.ViewModels;
using Umbraco.Web.Mvc;
using System.Net.Mail;
using Umbraco.Core.Models;


namespace AarhusDevGroup.Controllers
{
    public class ContactFormSurfaceController : SurfaceController
    {
        // GET: ContactFormSurface
        public ActionResult Index()
        {
            return PartialView("ContactForm", new ContactForm());
        }

        [HttpPost]
        public ActionResult HandleFormSubmit(ContactForm model)
        {
            if (!ModelState.IsValid) { return CurrentUmbracoPage(); }

            MailMessage message = new MailMessage();
            message.To.Add("carldevsen@gmail.com");
            message.Subject = model.Subject;
            message.From = new MailAddress(model.Email, model.Name);
            message.Body = model.Message + "\n my mail is: ";


            using (SmtpClient smtp = new SmtpClient())
            {
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network; smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true;
                smtp.Host = "smtp.gmail.com"; smtp.Port = 587;
                smtp.Credentials = new System.Net.NetworkCredential("carldevsen@gmail.com", "Default1234");
                smtp.EnableSsl = true;

                smtp.Send(message);
            }

            IContent comment = Services.ContentService.
                CreateContent(model.Subject, CurrentPage.Id, "comment");
            comment.SetValue("email", model.Email);
            comment.SetValue("cname", model.Name);
            comment.SetValue("subject", model.Subject);
            comment.SetValue("message", model.Message);
            Services.ContentService.Save(comment);

            TempData["Success"] = true;

            return RedirectToCurrentUmbracoPage();
        }
    }
}