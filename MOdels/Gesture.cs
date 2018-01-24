using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PublicRelationWeb.Models
{
    public class GestureModel : Message
    {
        public int EventID { get; set; }
        public string EncryptedEventCode { get; set; }
        public string EventName { get; set; }
        public int UserTypeCode { get; set; }
        public List<Gesture> GestureList { get; set; }
        public IEnumerable<SelectListItem> SmsClientList { get; set; }       
    }
   
    public class Gesture
    {
        public int GestureCode { get; set; }
        public bool CheckedStatus { get; set; }
        public string GestureName { get; set; }
        public string SmsText { get; set; }
        public string EmailSubject { get; set; }
        [AllowHtml]
        public string EmailBody { get; set; }
        public int? SmsGestureCode { get; set; }
        public string SmsSender { get; set; }
        public string EmailSender { get; set; }
        public int AudienceCode { get; set; }       
        public IEnumerable<SelectListItem> AudienceList { get; set; }        
    }

}
