using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PublicRelationWeb.Models
{
    public class Event : Message
    {
        public Event()
        {

        }
        [Display(Name = "Event Name")]
        [AllowHtml]
        [RegularExpression(@"^[^<>]*$", ErrorMessage = "Please Enter Valid {0}")]
        [Required(ErrorMessage = "Please Enter {0}")]
        public string EventName { get; set; }
        public int EventCode { get; set; }
        [Required(ErrorMessage = "Please Select User Type")]
        public int UserTypeCode { get; set; }
        [Required(ErrorMessage = "Please Select Event Category")]
        public int EventCategoryCode { get; set; }
        public IEnumerable<SelectListItem> UserTypeList { get; set; }
        public IEnumerable<SelectListItem> EventCategoryList { get; set; }
        //[Required(ErrorMessage = "Please enter employee name")]
        //public string EmployeeName { get; set; }
        //[Required(ErrorMessage = "Please select department")]
        //public int DepartmentCode { get; set; }
        //public IEnumerable<SelectListItem> DepartmentList { get; set; }
        //[Required(ErrorMessage = "Please select event")]
        //public int EventCode { get; set; }
        //public IEnumerable<SelectListItem> EventList { get; set; }
        //[Required(ErrorMessage = "Please select gesture")]
        //public int GestureCode { get; set; }
        //public IEnumerable<SelectListItem> GestureList { get; set; }

        public int? Month { get; set; }
        public int? Day { get; set; }
        public bool isActivate { get; set; }
    }

    public class Audience 
    {
        public int AudienceCode { get; set; }
        public string AudienceName { get; set; }
        public int UserTypeCode { get; set; }
        public string FilterCodes { get; set; }
        public List<AudienceFilter> AudienceFilterList { get; set; }
       
    }

    public class AudienceFilter
    {    
        public int FilterCode { get; set; }
        public string ReferenceCode { get; set; }
    }
     
}
