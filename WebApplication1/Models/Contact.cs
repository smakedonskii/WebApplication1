using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.BMPservice;

namespace WebApplication1.Models
{
    public class Contact : BPMservice.Contact
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string MobilePhone { get; set; }

        public string Dear { get; set; }

        public string JobTitle { get; set; }

        public DateTime? BirthDate { get; set; }
    }
}