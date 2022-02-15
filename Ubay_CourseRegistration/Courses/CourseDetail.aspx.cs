using CoreProject.Managers;
using CoreProject.Models;
using System;

namespace Ubay_CourseRegistration.Courses
{
    public partial class CourseDetail : System.Web.UI.Page
    {
        StudentManagers _studentManagers = new StudentManagers();
        protected void Page_Init(object sender, EventArgs e)
        {
            //讀取教師清單
            if (!IsPostBack)
            {
                _studentManagers.ReadTeacherTable(ref tcList);
            }

            if (this.IsUpdateMode())
            {
                string temp = Request.QueryString["Course_ID"];
                var manager = new CourseManagers();
                int minnum = manager.GetCourse(temp).MinNumEnrolled;

                //修改模式下選課人數如大於0，則無法更改開課日期及時間
                if (minnum > 0)
                {
                    this.Startdate.Enabled = false;
                    this.Startdate.BackColor = System.Drawing.Color.DarkGray;
                    this.Starttime.Enabled = false;
                    this.Starttime.BackColor = System.Drawing.Color.DarkGray;
                }

                this.Price.Enabled = false;
                this.Price.BackColor = System.Drawing.Color.DarkGray;
                this.LoadCourse(temp);
            }
            else
            {
                this.Course_title.Text = "新增課程";
                this.btn_Course.Text = "確認新增";
            }

        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        private bool IsUpdateMode()
        {
            string qsID = Request.QueryString["Course_ID"];
            if (qsID != null)
                return true;

            return false;
        }

        private void LoadCourse(string id)
        {
            var manager = new CourseManagers();
            var model = manager.GetCourse(id);
            //如查無該筆ID的資料返回查詢頁面
            if (model == null)
                Response.Redirect("~/Courses/CourseList.aspx");

            //將資料庫內資料放入各文字框
            this.txtCourseID.Text = model.Course_ID;
            this.txtCourseName.Text = model.C_Name;
            this.tcList.Text = model.Teacher_ID.ToString();
            this.Startdate.Text = model.StartDate.ToString("yyyy-MM-dd");
            this.Starttime.Text = model.StartTime.ToString();
            this.Enddate.Text = model.EndDate.ToString("yyyy-MM-dd");
            this.maxNum.Text = model.MaxNumEnrolled.ToString();
            this.Place.Text = model.Place_ID.ToString();
            this.txtCourseIntroduction.Text = model.CourseIntroduction;
            this.Price.Text = model.Price.ToString("0");

        }

        protected void btn_Course_Click(object sender, EventArgs e)
        {
            var manager = new CourseManagers();
            CourseModel model = new CourseModel();

            if (this.IsUpdateMode())
            {
                string qsID = Request.QueryString["Course_ID"];

                manager.GetCourse(qsID);
            }
            else
            {
                model = new CourseModel();
            }


            //檢查個欄位是否為空值
            if (this.txtCourseID.Text != string.Empty &&
                this.txtCourseName.Text != string.Empty &&
                this.tcList.SelectedIndex != 0 &&
                this.Startdate.Text != string.Empty &&
                this.Starttime.Text != string.Empty &&
                this.Enddate.Text != string.Empty &&
                this.maxNum.Text != string.Empty &&
                this.Place.Text != string.Empty &&
                this.txtCourseIntroduction.Text != string.Empty &&
                this.Price.Text != string.Empty)
            {
                if (manager.GetCourseID(this.txtCourseID.Text) == null)
                {
                    model.Course_ID = this.txtCourseID.Text.Trim();
                }
                else
                {
                    this.lbMsg.Text = "課程ID不可與過去課程重複";
                    this.lbMsg.Visible = true;
                    return;
                }



                model.C_Name = this.txtCourseName.Text.Trim();
                model.Teacher_ID = Convert.ToInt32(this.tcList.SelectedValue);
                if (Convert.ToDateTime(this.Startdate.Text) < DateTime.Now.AddDays(7))
                {
                    this.lbMsg.Text = "開課日期不可為過去日期，且需距離現在7天以上";
                    this.lbMsg.Visible = true;
                    return;
                }
                //比對開課日期與結訓日期是否有日期上的衝突
                if (Convert.ToDateTime(this.Enddate.Text) <= Convert.ToDateTime(this.Startdate.Text) ||
                    Convert.ToDateTime(this.Startdate.Text) >= Convert.ToDateTime(this.Enddate.Text))
                {
                    this.lbMsg.Text = "開課日期與結訓日期衝突，請重新輸入";
                    this.lbMsg.Visible = true;
                    return;
                }
                else
                {
                    model.StartDate = Convert.ToDateTime(this.Startdate.Text);
                    model.EndDate = Convert.ToDateTime(this.Enddate.Text);
                }
                model.StartTime = TimeSpan.Parse(this.Starttime.Text);
                model.MaxNumEnrolled = Convert.ToInt32(this.maxNum.Text);
                model.Place_ID = Convert.ToInt32(this.Place.Text);
                model.CourseIntroduction = this.txtCourseIntroduction.Text;
                model.Price = Convert.ToInt32(this.Price.Text);


            }
            else
            {
                this.lbMsg.Text = "所有欄位必填";
                this.lbMsg.Visible = true;

                return;
            }
            //判斷資料庫的既有資料內，是否有教師、教室、開課日期及時間與目前輸入的值是否有重複
            var chackmodel = manager.GetAllCourse();
            if (chackmodel.Teacher_ID.ToString() == this.tcList.SelectedValue &&
                chackmodel.Place_ID.ToString() == this.Place.Text &&
                chackmodel.StartDate.DayOfWeek == Convert.ToDateTime(this.Startdate.Text).DayOfWeek &&
                chackmodel.StartDate == Convert.ToDateTime(this.Startdate.Text) &&
                chackmodel.StartTime == TimeSpan.Parse(this.Starttime.Text))
            {
                this.lbMsg.Text = "此教師於此時段已有排定課程";
                this.lbMsg.Visible = true;
                return;
            }

            if (this.IsUpdateMode())
            {

                model.e_empno = (Guid)Session["Acc_sum_ID"];
                manager.UpdateCourse(model);
                this.lbMsg.Text = "修改成功";
                this.lbMsg.Visible = true;
            }
            else
            {

                model.MinNumEnrolled = 0;
                model.b_empno = (Guid)Session["Acc_sum_ID"];
                manager.CreatCourse(model);
                this.lbMsg.Text = "新增成功";
                this.lbMsg.Visible = true;

            }

        }






    }
}