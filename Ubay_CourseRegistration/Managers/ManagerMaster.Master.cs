using System;
using System.Web;


namespace Ubay_CourseRegistration.Managers
{
    public partial class ManagerMaster : System.Web.UI.MasterPage
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            int Type = -1;
            if (Session["Type"] != null)
            {
                Type = (int)Session["Type"];
            }



            if (Type != 1)
            {
                HttpContext.Current.Session.RemoveAll();
                Response.Redirect("~/Login.aspx");
            }

            if (!LoginHelper.HasLogined())
            {
                Response.Redirect("~/Login.aspx");
            }

        }
    }
}