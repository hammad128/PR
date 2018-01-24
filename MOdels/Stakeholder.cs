using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PublicRelationWeb.Controllers;

namespace PublicRelationWeb.Models
{
    public class Stakeholder : Message
    {
        [Display(Name = "Category Name")]
        [AllowHtml]
        [RegularExpression(@"^[^<>]*$", ErrorMessage = "Please Enter Valid {0}")]
        [Required(ErrorMessage = "Please Enter {0}")]
        public string CategoryName { get; set; }
        public int? ParentCategoryCode { get; set; }
        public IEnumerable<SelectListItem> ParentCategoryList { get; set; }
        public bool isVIP { get; set; }
        public bool IsParentVisible { get; set; }
    }
    public class Users : Message
    {
        public int? StakeholderCode { get; set; }
        [AllowHtml]
        [Display(Name = "Full Name")]
        [Required(ErrorMessage = "Please Enter Full Name")]
        [StringLength(50, ErrorMessage = "Full Name Shouldn't be Exceed from 50 Characters")]
        [RegularExpression(@"^([a-zA-Z\s'-]{3,50})$", ErrorMessage = "Please Enter Valid Name")]
        public string FullName { get; set; }
        
        [Required(ErrorMessage = "Please Select Category")]
        public int CategoryCode { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Please Enter Email")]
        [EmailAddress(ErrorMessage = "Please Enter a Valid Email Address")]
        public string Email { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Please Enter Designation")]
        [StringLength(50, ErrorMessage = "Designation Shouldn't be Exceed from 50 Characters")]
        [RegularExpression(@"^([a-zA-Z\s'-]{2,50})$", ErrorMessage = "Please Enter Valid Designation")]
        public string Designation { get; set; }
        [AllowHtml]
        [StringLength(50, ErrorMessage = "Organization Shouldn't be Exceed from 50 Characters")]
        [RegularExpression(@"^([a-zA-Z\s]{2,50}([a-zA-Z0-9\s'-]{3,15})?)$", ErrorMessage = "Please Enter Valid Organization")]
        public string Organization { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Please Enter Cell Phone Number")]
        [RegularExpression(@"^(\d{11})$", ErrorMessage = "Please Enter Valid Cell Phone Number")]
        public string CellPhone { get; set; }
        [AllowHtml]
        [RegularExpression(@"^(\d{11})$", ErrorMessage = "Please Enter Valid Cell Phone Number")]
        public string CellPhone2 { get; set; }
        [AllowHtml]
        [StringLength(100, ErrorMessage = "Residential Address Shouldn't be Exceed from 100 Characters")]
        [RegularExpression(@"^([0-9a-zA-Z #,-/]{3,100})$", ErrorMessage = "Incorrect Residence Address")]
        public string ResidenceAddress { get; set; }
        [AllowHtml]
        [StringLength(100, ErrorMessage = "Business Address Shouldn't be Exceed from 100 Characters")]
        [RegularExpression(@"^([0-9a-zA-Z #,-/]{3,100})$", ErrorMessage = "Incorrect Business Address")]
        public string BusinessAddress { get; set; }
        [Required(ErrorMessage = "Please Select Gender")]
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        [AllowHtml]
        [RegularExpression(@"^(\d{5}-\d{7}-\d{1})$", ErrorMessage = "Please Enter Valid CNIC")]
        public string CNIC { get; set; }
        public IEnumerable<SelectListItem> ChildCategoryList { get; set; }
        public string UserImage { get; set; }
        public HttpPostedFileBase File { get; set; }
    }
}
