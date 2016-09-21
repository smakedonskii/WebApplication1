using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Data.Services.Client;
using WebApplication1.BMPservice;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;

namespace WebApplication1.Models
{
    public class ProxyService
    {
        //Объявление переменной адреса сервиса OData
        private static readonly XNamespace ds = "http://schemas.microsoft.com/ado/2007/08/dataservices";
        private static readonly XNamespace dsmd = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";
        private static readonly XNamespace atom = "http://www.w3.org/2005/Atom";
        private static Uri serverUri = new Uri("http://185.47.152.138:1423/0/ServiceModel/EntityDataService.svc/");
        public static void OnSendingRequestCookie(object sender, SendingRequestEventArgs e)
        {
            Login.TryLogin();
            var req = e.Request as HttpWebRequest;
            req.CookieContainer = Login.AuthCookie;
            e.Request = req;
        }

        public static IEnumerable<Contact> GetOdataCollection()
        {
            var context = new BPMonline(serverUri);
            IEnumerable<Contact> allContacts = null;
            context.SendingRequest += new EventHandler<SendingRequestEventArgs>(OnSendingRequestCookie);
            try
            {
                allContacts = (from contacts in context.ContactCollection.OrderByDescending(p => p.CreatedOn)
                               select contacts).Take(40);
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
            return allContacts;
        }
        public static void CreateBpmEntityByOdataHttpExample(Contact newContact)
        {
            var content = new XElement(dsmd + "properties",
                          new XElement(ds + "Name", newContact.Name),
                          new XElement(ds + "MobilePhone", newContact.MobilePhone),
                          new XElement(ds + "JobTitle", newContact.JobTitle),
                          new XElement(ds + "BirthDate", newContact.BirthDate),
                          new XElement(ds + "Dear", newContact.Dear));
            var entry = new XElement(atom + "entry",
                        new XElement(atom + "content",
                        new XAttribute("type", "application/xml"), content));
            var request = (HttpWebRequest)HttpWebRequest.Create(serverUri + "ContactCollection/");
            request.Credentials = new NetworkCredential("Supervisor", "Supervisor");
            request.Method = "POST";
            request.Accept = "application/atom+xml";
            request.ContentType = "application/atom+xml;type=entry";
            using (var writer = XmlWriter.Create(request.GetRequestStream()))
            {
                entry.WriteTo(writer);
            }
            using (WebResponse response = request.GetResponse())
            {
                if (((HttpWebResponse)response).StatusCode == HttpStatusCode.Created)
                {
                    Debug.WriteLine("Contact is created!");
                }
            }
        }

        public static void UpdateExistingBpmEnyityByOdataHttpExample(Contact newContact)
        {
            var contactId = newContact.Id;
            var content = new XElement(dsmd + "properties",
                        new XElement(ds + "Name", newContact.Name),
                          new XElement(ds + "MobilePhone", newContact.MobilePhone),
                          new XElement(ds + "JobTitle", newContact.JobTitle),
                          new XElement(ds + "Dear", newContact.Dear)
            );
            var entry = new XElement(atom + "entry",
                    new XElement(atom + "content",
                            new XAttribute("type", "application/xml"),
                            content)
                    );
            var request = (HttpWebRequest)HttpWebRequest.Create(serverUri
                    + "ContactCollection(guid'" + contactId + "')");
            request.Credentials = new NetworkCredential("Supervisor", "Supervisor");
            request.Method = "PUT";
            request.Accept = "application/atom+xml";
            request.ContentType = "application/atom+xml;type=entry";
            // Запись сообщения xml в поток запроса.
            using (var writer = XmlWriter.Create(request.GetRequestStream()))
            {
                entry.WriteTo(writer);
            }
            using (WebResponse response = request.GetResponse())
            {
            }
        }

        public static void DeleteBpmEntityByOdataHttpExample(Guid contactId)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(serverUri
                    + "ContactCollection(guid'" + contactId + "')");
            request.Credentials = new NetworkCredential("Supervisor", "Supervisor");
            request.Method = "DELETE";
            using (WebResponse response = request.GetResponse())
            {
            }
        }
    }
}