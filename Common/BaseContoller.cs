
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Threading.Tasks;

namespace PublicRelationWeb.Common
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {          	

            if (Request.Cookies[Constants.userIDCookie] != null)
            {                
                if (IsViewResult(filterContext) == true)
                {
                    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                    //if (!GetAuthentication() && actionName != "Authentication" && actionName != "Login" && actionName != "GesturesList" && actionName != "GestureDetails" && actionName != "EditEvent" && actionName != "EditAudience" && actionName != "EditCategory" && actionName != "EditUser")
                    if (!GetAuthentication() && actionName != "Authentication" && actionName != "Login")
                    {
                        filterContext.Result = new RedirectResult("~/Home/Authentication");
                    }
                    else
                        return;
                }
                else return;
            }
            else if (this.ControllerContext.RouteData.Values["action"].ToString() != "Login")
            {
                filterContext.Result = new RedirectResult("~/Home/Login");

            }
        }

        public bool GetAuthentication()
        {

            SqlParameter[] sqlParams = new SqlParameter[] {
               new SqlParameter("@NavigationURL", this.ControllerContext.RouteData.Values["action"].ToString()),
               new SqlParameter("@UserCode",Constants.GetUserID())           
            };

            DataSet ds = Task.Run(async () => await DataAccess.ExecuteDatasetAsync(AppConfigurations.ConnectionString, CommandType.StoredProcedure,
                       "Select_Authorization", sqlParams)
                    ).Result as DataSet;
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                return Convert.ToBoolean(ds.Tables[0].Rows[0][0]);
            else
                return false;

        }

        public bool IsViewResult(ActionExecutingContext filterContext)
        { 
            bool IsASyncAction = filterContext.ActionDescriptor.GetType() == typeof(System.Web.Mvc.ReflectedActionDescriptor) ? true : false;
            bool IsAction = IsASyncAction;
            if (IsASyncAction == true)
            {
                IsAction = ((ReflectedActionDescriptor)filterContext.ActionDescriptor).MethodInfo.ReturnType == typeof(ActionResult) ? true : false;
            }
          return  IsAction;
        }

        
    }
}
