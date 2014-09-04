using System;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Ecommerce.Orders.Model;
using Telerik.Sitefinity.Modules.Ecommerce;
using Telerik.Sitefinity.Modules.Ecommerce.Orders;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Claims;
using Telerik.Sitefinity.Security.Model;
using Telerik.Sitefinity.Services;

namespace SitefinityWebApp
{
    public partial class RemoveTax : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = ClaimsManager.GetCurrentIdentity();
            var exempt = IsUserInRole(currentUser.Name, "Exempt");
            HttpCookie shoppingCartCookie = SystemManager.CurrentHttpContext.Request.Cookies[EcommerceConstants.OrdersConstants.ShoppingCartIdCookieName];
            if (exempt && shoppingCartCookie!=null)
            {
                var oMan = OrdersManager.GetManager();
                
                TaxClass taxClass = oMan.GetTaxClasses().Where(t => t.Title == "Exempt").SingleOrDefault();
               var cartId = new Guid(shoppingCartCookie.Value);
               var cartOrder = oMan.GetCartOrder(cartId);
               var details = oMan.GetCartDetails().Where(d => d.Parent.Id == cartOrder.Id);
               foreach (var item in details)
               {
                   item.TaxClassId = taxClass.Id;
                   item.TaxRate = 0;
               }
               cartOrder.EffectiveTaxRate = 0;
               cartOrder.Tax = 0;
               oMan.SaveChanges();
            }
        }

        public static bool IsUserInRole(string userName, string roleName)
        {
            bool isUserInRole = false;
 
            UserManager userManager = UserManager.GetManager();
            RoleManager roleManager = RoleManager.GetManager("Default");
 
            bool userExists = userManager.UserExists(userName);
            bool roleExists = roleManager.RoleExists(roleName);
 
            if (userExists && roleExists)
            {
                User user = userManager.GetUser(userName);
                isUserInRole = roleManager.IsUserInRole(user.Id, roleName);
            }
 
            return isUserInRole;
        }
    }
}