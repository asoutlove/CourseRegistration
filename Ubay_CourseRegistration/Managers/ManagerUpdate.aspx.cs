using CoreProject.Helpers;
using CoreProject.Models;
using System;
using System.Data.SqlClient;

namespace Ubay_CourseRegistration.Managers
{
    public partial class ManagerUpdate : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            Guid temp;
            Guid.TryParse(Request.QueryString["Manager_ID"], out temp);
            this.LoadAccount(temp);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void UpdateAdmin_Click(object sender, EventArgs e)
        {

            Account_summaryModel asmodel = new Account_summaryModel();
            AccountModel acmodel = new AccountModel();
            //將使用者輸入的資料放進模型裡
            asmodel.firstname = this.txtFirstname.Text;
            asmodel.lastname = this.txtLastname.Text;
            asmodel.department = this.txtDepartment.Text;
            acmodel.Account = this.txtAccount.Text;
            acmodel.password = this.txtNewPassword.Text;
            asmodel.Pwdcheck = this.txtPwdcheck.Text;
            asmodel.datetime = DateTime.Now; // 取得現在時間
            string updatetime = asmodel.datetime.ToString("yyyy/MM/dd HH:mm:ss"); // 轉成字串
            acmodel.Type = true;

            //抓修改者的guid
            Guid editor;
            editor = (Guid)Session["Acc_sum_ID"];

            var manager = new ManagerManagers();
            var model = manager.GetAccountViewModel(editor); //將修改者的guid拿去資料庫裡撈帳號資料出來做帳號比對

            SqlConnection conn = new SqlConnection(DBBase.GetConnectionString()); //連線字串
            conn.Open(); 
            var Managers = new ManagerManagers();

            SqlCommand passwordcheck = new SqlCommand("Select * From Account_summary Where password = '" + this.txtPassword.Text + "'", conn); //將使用者輸入的密碼跟資料庫裡的資料做比對
            SqlDataReader pwdchk = passwordcheck.ExecuteReader();

            this.WarningMsg.Text = "";
            if (string.IsNullOrEmpty(this.txtFirstname.Text))
            {
                this.WarningMsg.Text = "姓氏欄位不可為空!";
                return;
            }
            else if (string.IsNullOrEmpty(this.txtLastname.Text))
            {
                this.WarningMsg.Text = "名字欄位不可為空!";
                return;
            }
            else if (string.IsNullOrEmpty(this.txtDepartment.Text))
            {
                this.WarningMsg.Text = "單位欄位不可為空!";
                return;
            }
            else if (string.IsNullOrEmpty(this.txtAccount.Text))
            {
                this.WarningMsg.Text = "帳號不可為空!";
                return;
            }
            else if (model.Account != this.txtAccount.Text) //如果帳號欄位裡的值，並不是此登入者帳號原先的帳號，進入判斷式(判斷使用者是否欲更改帳號，若是，進入內層判斷式)
            {
                if (Managers.GetAccount(this.txtAccount.Text.Trim()) != null) //判斷欲更改的新帳號是否已有相同帳號
                {
                    this.WarningMsg.Text = "已有相同帳號，請重新輸入";
                    return;
                }
            }
            else if (!string.IsNullOrEmpty(this.txtNewPassword.Text) ||
                !string.IsNullOrEmpty(this.txtPwdcheck.Text) ||
                !string.IsNullOrEmpty(this.txtPassword.Text)) //如果舊密碼、新密碼、確認新密碼三個欄位中，任一欄位不是空值(代表使用者想改密碼)，進入判斷式
            {
                if (!pwdchk.Read())
                {
                    this.WarningMsg.Text = "舊密碼輸入錯誤，請重新輸入";
                    return;
                }
                else if (acmodel.password != asmodel.Pwdcheck)
                {
                    this.WarningMsg.Text = "新密碼確認不一致，請重新輸入";
                    return;
                }
            }
            else if (string.IsNullOrEmpty(this.txtNewPassword.Text) &&
                string.IsNullOrEmpty(this.txtPwdcheck.Text) &&
                string.IsNullOrEmpty(this.txtPassword.Text)) //如果舊密碼、新密碼、確認新密碼三個欄位，所有欄位皆為空值(代表使用者不想改密碼)，將使用者的舊密碼代入欄位裡
            {
                acmodel.password = model.password;
            }
            ManagerManagers.UpdateAdminTablel(acmodel, asmodel, updatetime, editor);
            this.WarningMsg.Text = "修改成功";
        }


        private void LoadAccount(Guid updater) //將登入者帳號的資料放進各欄位的方法
        {

            if (string.IsNullOrEmpty(Request.QueryString["Manager_ID"]))
            {
                updater = (Guid)Session["Acc_sum_ID"];
            }

            var manager = new ManagerManagers();
            var model = manager.GetAccountViewModel(updater);

            //如果修改者guid對不上 返回上一頁
            if (model == null)
            {
                Response.Redirect("~/Managers/ManagerMainPage.aspx");
            }
            this.txtFirstname.Text = model.firstname;
            this.txtLastname.Text = model.lastname;
            this.txtDepartment.Text = model.department;
            this.txtAccount.Text = model.Account;

        }
    }
}