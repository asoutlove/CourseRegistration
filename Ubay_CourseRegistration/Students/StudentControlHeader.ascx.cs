using System;

namespace Ubay_CourseRegistration.Students
{
    public partial class StudentControlHeader : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //取得存在SESSION的帳號名稱
            ltAccount.Text = (string)Session["Account"];
        }

        protected void logout(object sender, EventArgs e)
        {
            Session.RemoveAll();
            Response.Redirect("~/Login.aspx");
        }
    }
}