using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.BMPservice;

namespace WebApplication1.Controllers
{
    public class ProxyController : Controller
    {
        private IEnumerable<Contact> allContacts;
        public ProxyController()
        {
            if (Login.TryLogin())
            {
                allContacts = ProxyService.GetOdataCollection();
            }
        }

        public ActionResult Index()
        {
            return View(allContacts);
        }

        public ViewResult Create()
        {
            return View(new Contact());
        }

        [HttpPost]
        public ActionResult Create(Contact newContact)
        {
                ProxyService.CreateBpmEntityByOdataHttpExample(newContact);
                return RedirectToAction("Index");
        }

        public ViewResult Update(Guid contactId)
        {
            Contact contact = allContacts.FirstOrDefault(p => p.Id == contactId);
           return View(contact);
        }

        [HttpPost]
        public ActionResult Update(Contact contact)
        {
                ProxyService.UpdateExistingBpmEnyityByOdataHttpExample(contact);
                return RedirectToAction("Index");
        }

        public ActionResult Delete(Guid contactId)
        {
                ProxyService.DeleteBpmEntityByOdataHttpExample(contactId);
                return RedirectToAction("Index");
        }
    }
}