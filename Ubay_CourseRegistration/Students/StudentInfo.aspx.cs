using CoreProject.Managers;
using CoreProject.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ubay_CourseRegistration.Managers;

namespace Ubay_CourseRegistration.Students
{
    public partial class StudentInfo : System.Web.UI.Page
    {
        //接受檔案類型
        private string[] _allowExts = { ".jpg", ".png", ".bmp", ".gif",".jpeg" };
        //存檔路徑
        private string _saveFolder = "~/FileDownload/";

        protected void Page_Init(object sender, EventArgs e)
        {
            //讀取學校清單
            if (!IsPostBack)
            {
                StudentManagers stmanagers = new StudentManagers();
                stmanagers.GetSchoolList(ref school);
            }
            //取URL的ID
            Guid temp;
            Guid.TryParse(Request.QueryString["ID"], out temp);

            this.idn.Enabled = false;
            this.idn.BackColor = System.Drawing.Color.DarkGray;
            this.fname.Enabled = false;
            this.fname.BackColor = System.Drawing.Color.DarkGray;
            this.lname.Enabled = false;
            this.lname.BackColor = System.Drawing.Color.DarkGray;
            this.LoadAccount(temp);

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //關閉JQ驗證
            UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
            //引起PostBack時不將密碼欄清空
            if (IsPostBack)
            {
                pwd.Attributes.Add("value", pwd.Text);
                newpwd.Attributes.Add("value", newpwd.Text);
                renewpwd.Attributes.Add("value", renewpwd.Text);
            }
        }
        //取Session的ID讓資料庫查詢，並將該筆ID的資料帶入文字框
        private void LoadAccount(Guid id)
        {
            id = (Guid)Session["Acc_sum_ID"];
            var manager = new ManagerManagers();
            var model = manager.GetStudentViewModel(id);
            if (model == null)
                Response.Redirect("~/Students/StudentInfo.aspx");


            this.fname.Text = model.S_FirstName;
            this.lname.Text = model.S_LastName;
            this.idn.Text = model.Idn;
            this.pwd.Text = model.password;
            this.gender.SelectedValue = model.gender.ToString();
            this.birthday.Text = model.Birthday.ToString("yyyy-MM-dd");
            this.email.Text = model.Email;
            this.phone.Text = model.CellPhone;
            this.address.Text = model.Address;
            this.experience.SelectedValue = model.Experience.ToString();
            if (model.Experience == true)
            {
                this.exyear.Visible = true;
                this.yearshow.Visible = true;

            }
            this.exyear.Text = model.ExYear.ToString();
            this.education.Text = model.Education.ToString();
            if (model.Education.ToString() == "3" || model.Education.ToString() == "4")
            {
                this.schoolshow.Visible = true;
                this.school.Visible = true;
            }

            this.school.SelectedValue = model.School_ID.ToString();
            this.psn.Text = model.PassNumber;
            if (!string.IsNullOrEmpty(model.PassPic))
            {
                this.Image1.ImageUrl = _saveFolder + model.PassPic;
                this.Image1.Visible = true;
            }


        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var manager = new ManagerManagers();
            var DBmanager = new DBAccountManager();

            StudentAccountViewModel model = new StudentAccountViewModel();
            //修改資料時須檢查是否有輸入舊密碼
            if (DBmanager.Chackpwd(this.idn.Text, this.pwd.Text) == null)
            {
                model.password = null;
                this.lbmsg.Text = "請輸入舊密碼";
                this.lbmsg.Visible = true;
                return;
            }
            else
            {
                model.password = this.pwd.Text;
            }

            //如有輸入新密碼文字框時，則認定為要更新密碼，並檢查新密碼與確認新密碼的一致性
            if (!string.IsNullOrEmpty(this.newpwd.Text) ||
            !string.IsNullOrEmpty(this.renewpwd.Text))
            {

                if (this.newpwd.Text == this.renewpwd.Text)
                {
                    model.password = this.renewpwd.Text.Trim();
                }
                else
                {
                    model.password = null;
                    this.lbmsg.Text = "新密碼和確認新密碼不一致";
                    this.lbmsg.Visible = true;
                    return;
                }
            }
            else
            {
                model.password = this.pwd.Text;
            }



            //標有*的文字框不可為空值的檢查
            if (this.fname.Text != string.Empty &&
                this.lname.Text != string.Empty &&
                this.idn.Text != string.Empty &&
                this.gender.Text != string.Empty &&
                this.birthday.Text != string.Empty &&
                this.email.Text != string.Empty &&
                this.phone.Text != string.Empty &&
                this.address.Text != string.Empty &&
                this.experience.Text != string.Empty &&
                this.education.Text != string.Empty)
            {
                model.S_FirstName = this.fname.Text.Trim();
                model.S_LastName = this.lname.Text.Trim();
                model.Idn = this.idn.Text.Trim();
                model.Email = this.email.Text.Trim();
                model.CellPhone = this.phone.Text.Trim();
                model.gender = Convert.ToBoolean(this.gender.SelectedValue);
                model.Birthday = Convert.ToDateTime(this.birthday.Text);
                model.Email = this.email.Text.Trim();
                model.CellPhone = this.phone.Text.Trim();
                model.Address = this.address.Text.Trim();
                model.PassNumber = this.psn.Text.Trim();
                if (string.IsNullOrEmpty(this.Image1.ImageUrl))
                {
                    string pic1 = this.GetNewFileName(this.passpic);
                    if (string.IsNullOrEmpty(pic1))
                    {
                        this.lbmsg.Text = "檔案僅接受.jpeg, .jpg, .png, .bmp, .gif";
                        this.lbmsg.Visible = true;
                        return;
                    }
                    else
                    {
                        model.PassPic = pic1;
                    }


                }
                else
                {
                    string img1 = this.Image1.ImageUrl;
                    model.PassPic = Path.GetFileName(img1);
                }


            }
            else
            {
                this.lbmsg.Text = "*欄位不可為空";
                this.lbmsg.Visible = true;
                return;
            }

            //檢查有無程式經驗選擇"有"時，是否有選擇年數
            if (this.experience.SelectedItem.Text == "有")
            {
                if (this.exyear.SelectedItem.Text == "請選擇")
                {

                    model.ExYear = 0;
                    this.lbmsg.Visible = true;
                    this.lbmsg.Text = "需選擇年數";
                    return;
                }
                else
                {
                    model.Experience = Convert.ToBoolean(this.experience.SelectedItem.Value);
                    model.ExYear = Convert.ToInt32(this.exyear.Text);

                }
            }
            else
            {
                model.Experience = Convert.ToBoolean(this.experience.SelectedItem.Value);
                model.ExYear = 0;

            }
            //檢查學歷為大學、研究所時，是否有選擇學校
            if (this.education.SelectedItem.Text == "大學" || this.education.SelectedItem.Text == "研究所")
            {
                if (this.school.SelectedItem.Text == "請選擇")
                {

                    model.School_ID = Convert.ToInt32(this.school.Text);
                    this.lbmsg.Visible = true;
                    this.lbmsg.Text = "需選擇學校";

                    return;
                }
                else
                {
                    model.Education = Convert.ToInt32(this.education.Text);
                    model.School_ID = Convert.ToInt32(this.school.Text);

                }
            }
            else
            {
                model.Education = Convert.ToInt32(this.education.Text);
                model.School_ID = 0;
            }

            model.Student_ID = (Guid)Session["Acc_sum_ID"];
            model.Acc_sum_ID = (Guid)Session["Acc_sum_ID"];
            model.e_empno = (Guid)Session["Acc_sum_ID"];
            manager.UpdataStudent(model);
            this.lbmsg.Text = "修改成功";
            this.lbmsg.Visible = true;



        }
        //取得護照照片檔案名稱，並重新取名為GUID
        private string GetNewFileName(FileUpload fu)
        {
            //如無檔案則回傳空字串
            if (!fu.HasFile)
                return string.Empty;

            //取得檔案
            var uFile = fu.PostedFile;
            //取得檔案名稱
            var fileName = uFile.FileName;
            //取得副檔名(檔案類型)
            string fileExt = System.IO.Path.GetExtension(fileName);
            //判別檔案類型
            if (!_allowExts.Contains(fileExt.ToLower()))
                return string.Empty;

            //存檔路徑
            string path = Server.MapPath(_saveFolder);
            //取名為GUID
            string newFileName = Guid.NewGuid().ToString() + fileExt;
            //路徑+檔名
            string fullPath = System.IO.Path.Combine(path, newFileName);
            //存檔
            uFile.SaveAs(fullPath);
            return newFileName;
        }
        //有無程式經驗根據選項顯示年數
        protected void experience_SelectedIndexChanged(object sender, EventArgs e)
        {
            StudentAccountViewModel model = new StudentAccountViewModel();

            if (this.experience.SelectedItem.Text == "有" || model.Experience != false)
            {
                this.yearshow.Visible = true;
                this.exyear.Visible = true;

            }
            else
            {
                this.yearshow.Visible = false;
                this.exyear.Visible = false;

            }
        }
        //學歷根據選項顯示學校

        protected void education_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.education.SelectedItem.Text == "大學" || this.education.SelectedItem.Text == "研究所")
            {
                this.schoolshow.Visible = true;
                this.school.Visible = true;

            }
            else
            {
                this.schoolshow.Visible = false;
                this.school.Visible = false;

            }
        }



    }
}