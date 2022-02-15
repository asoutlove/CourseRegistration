using CoreProject.Helpers;
using CoreProject.Models;
using System;
using System.Data.SqlClient;

namespace Ubay_CourseRegistration.Managers
{
    public partial class ManagerRegion : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            if (this.IsUpdateMode()) //判斷是否為修改模式 
            {
                //宣告一個變數 去接取所點選欲修改的使用者的GUID
                Guid temp;
                Guid.TryParse(Request.QueryString["Manager_ID"], out temp);

                this.LoadAccount(temp); //將所點選的使用者的GUID丟到讀取帳號資料方法
                this.LB1.Text = "修改管理人資料"; //將標題改為"修改管理人資料"
                this.Regis.Text = "確認修改"; //將確認新增按鈕改為 "確認修改"
            }
            else //如果是新增模式 隱藏Label內容("(若不更改密碼則無需填寫)")
            {
                this.Label3.Visible = false;
                this.Label4.Visible = false;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        protected void CreateAdmin_Click(object sender, EventArgs e) //點擊確認按鈕後
        {
            //為了將使用者所輸入的資料丟進model裡 先建立model的空間
            Account_summaryModel asmodel = new Account_summaryModel();
            AccountModel acmodel = new AccountModel();

            //將使用者的所輸入的資料放進model
            asmodel.firstname = this.txtFirstname.Text;
            asmodel.lastname = this.txtLastname.Text;
            asmodel.department = this.txtDepartment.Text;
            acmodel.Account = this.txtAccount.Text;
            acmodel.password = this.txtPassword.Text;
            asmodel.Pwdcheck = this.txtPwdcheck.Text;
            asmodel.datetime = DateTime.Now; // 取得現在時間
            string createtime = asmodel.datetime.ToString("yyyy/MM/dd HH:mm:ss"); // 轉成字串
            acmodel.Type = true;

            //抓取建立者/修改者的guid
            Guid Creator;
            Creator = (Guid)Session["Acc_sum_ID"];

            if (this.IsUpdateMode()) //如果是修改模式 
            {

                //宣告一個變數 去接取所點選欲修改的使用者的GUID
                Guid temp;
                Guid.TryParse(Request.QueryString["Manager_ID"], out temp);
                acmodel.Acc_sum_ID = temp;


                var manager = new ManagerManagers();
                var model = manager.GetAccountViewModel(temp);

                var Managers = new ManagerManagers();

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
                else if (!string.IsNullOrEmpty(this.txtPwdcheck.Text) ||
                    !string.IsNullOrEmpty(this.txtPassword.Text)) //如果新密碼及確認新密碼兩個欄位中，任一欄位不是空值(代表使用者想改密碼)，進入判斷式
                {
                    if (acmodel.password != asmodel.Pwdcheck)
                    {
                        this.WarningMsg.Text = "新密碼確認不一致，請重新輸入";
                        return;
                    }
                }
                else if (string.IsNullOrEmpty(this.txtPwdcheck.Text) &&
                    string.IsNullOrEmpty(this.txtPassword.Text)) //如果新密碼及確認新密碼兩個欄位皆為空值(代表使用者不想改密碼)，將使用者的舊密碼代入欄位裡
                {
                    acmodel.password = model.password;
                }
                ManagerManagers.UpdateAdminTablel(acmodel, asmodel, createtime, Creator);
                this.WarningMsg.Text = "修改成功";
            }
            else //如果是新增模式
            {
                acmodel.Acc_sum_ID = Guid.NewGuid();

                SqlConnection conn = new SqlConnection(DBBase.GetConnectionString());

                conn.Open();

                SqlCommand accountcheck = new SqlCommand("Select * From Account_summary Where Account='" + txtAccount.Text + "'", conn); //從資料庫抓帳號，跟使用者輸入的帳號做比對
                SqlDataReader accChk = accountcheck.ExecuteReader();

                if (string.IsNullOrEmpty(asmodel.firstname) || string.IsNullOrEmpty(asmodel.lastname) ||
                    string.IsNullOrEmpty(asmodel.department) || string.IsNullOrEmpty(acmodel.Account) ||
                    string.IsNullOrEmpty(acmodel.password) || string.IsNullOrEmpty(asmodel.Pwdcheck))
                {
                    this.WarningMsg.Text = "所有欄位皆為必填，不可為空!";
                }
                else if (acmodel.password != asmodel.Pwdcheck)
                {
                    this.WarningMsg.Text = "確認密碼不一致，請重新輸入";
                }
                else if (accChk.Read())
                {
                    this.WarningMsg.Text = "已有相同帳號，請重新輸入";
                }
                else
                {
                    ManagerManagers.InsertAdminTablel(acmodel, asmodel, createtime, Creator);
                    this.WarningMsg.Text = "新增成功";
                }
            }
        }

        private void LoadAccount(Guid temp) //將使用者的個人資料輸出到欄位上
        {
            var manager = new ManagerManagers();
            var model = manager.GetAccountViewModel(temp);
            if (string.IsNullOrEmpty(Request.QueryString["Manager_ID"]))
            {
                temp = (Guid)Session["Acc_sum_ID"];
            }
            if (model == null)
            {
                Response.Redirect("~/Managers/ManagerSearch.aspx");
            }
            this.txtFirstname.Text = model.firstname;
            this.txtLastname.Text = model.lastname;
            this.txtDepartment.Text = model.department;
            this.txtAccount.Text = model.Account;
        }

        private bool IsUpdateMode() //判斷是否為修改模式
        {
            string qsID = Request.QueryString["Manager_ID"];
            Guid temp;
            if (!string.IsNullOrEmpty(Request.QueryString["Manager_ID"]))
            {
                if (Guid.TryParse(qsID, out temp))
                    return true;
            }
            return false;
        }
        protected void Turnback_Click(object sender, EventArgs e) //返回按鈕
        {
            Response.Redirect("~/Managers/ManagerSearch.aspx");
        }
    }
}
