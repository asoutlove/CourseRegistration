using CoreProject.Managers;
using CoreProject.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Ubay_CourseRegistration.Students
{
    public partial class StudentDropCourse : System.Web.UI.Page
    {
        StudentManagers _studentManagers = new StudentManagers();
        //使用PagedDataSource物件實現課程資料repeater分頁
        readonly PagedDataSource _pgsource = new PagedDataSource();
        //預設會是當前年份月份，根據上個月及下個月按鈕的使用，改變數值成不同年份月份
        static DateTime datetime = DateTime.Now;
        //課程資料repeater數字首尾頁索引 
        int _firstIndex, _lastIndex;
        //設定課程資料repeater項目數量
        private int _pageSize = 10;
        //用來裝目前登入學生帳號
        string _ID;
        //用來裝目前登入學生所有還可退的課程
        static DataTable dt_courses = new DataTable();
        //退課購物車 用來裝目前登入學生已選的課程
        static DataTable dt_cart = new DataTable();

        //用來記錄課程資料repeater當前頁
        private int CurrentPage
        {
            //當前頁傳回屬性值，且回傳期間記住分頁的記錄總數，以便在按最後頁按鈕時，可以得到最後一頁的索引
            get
            {
                if (ViewState["CurrentPage"] == null)
                {
                    return 0;
                }
                return ((int)ViewState["CurrentPage"]);
            }
            set
            {
                ViewState["CurrentPage"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _ID = Session["Acc_sum_ID"].ToString();
            if (Page.IsPostBack) return;
            //GetCourseRecord();
            BindDataIntoRepeater();

            //查詢教師的下拉選單內容方法
            _studentManagers.ReadTeacherTable(ref ddlTeacher);

            //先new一次將資料表清空，建立要刪除的購物車欄位
            dt_cart = new DataTable();
            dt_cart.Columns.Add("Course_ID", typeof(string));
            dt_cart.Columns.Add("C_Name", typeof(string));
            dt_cart.Columns.Add("Price", typeof(int));

            //建立月曆上的年月顯示資訊
            monthOnCalendar.Text = $"{datetime.ToString("yyyy/MM")}月課程紀錄";
            CreateCalendar();

        }



        private void BindDataIntoRepeater()
        {
            //呼叫方法查詢並列出當前登入的學生的可退的課程資料
            var dtt = dt_courses = _studentManagers.SearchCourseRecordToDrop(_ID, txtCourseID.Text, txtCourseName.Text, txtStartDate1.Text, txtStartDate2.Text, txtPlace.Text, TxtPrice1.Text, TxtPrice2.Text, ddlTeacher.SelectedValue);
            _pgsource.DataSource = dtt.DefaultView;
            //啟用分頁
            _pgsource.AllowPaging = true;
            //要在Repeater顯示的項目數 
            _pgsource.PageSize = _pageSize;
            //當前頁索引
            _pgsource.CurrentPageIndex = CurrentPage;
            //維持顯示 Total pages
            ViewState["TotalPages"] = _pgsource.PageCount;
            // 顯示現在頁數之於總頁數  Example: "Page 1 of 10"
            lblpage.Text = "Page " + (CurrentPage + 1) + " of " + _pgsource.PageCount;
            //課程資料repeater的First, Last, Previous, Next 按鈕的使用控制
            lbPrevious.Enabled = !_pgsource.IsFirstPage;
            lbNext.Enabled = !_pgsource.IsLastPage;
            lbFirst.Enabled = !_pgsource.IsFirstPage;
            lbLast.Enabled = !_pgsource.IsLastPage;
            dt_courses = dtt;
            // Bind資料進Repeater
            rptResult.DataSource = _pgsource;
            rptResult.DataBind();
            HandlePaging();
        }

        #region 課程Repeater下方按鈕功能
        //處理課程Repeater下方分頁頁碼
        private void HandlePaging()
        {
            var dtt = new DataTable();
            dtt.Columns.Add("PageIndex"); 
            dtt.Columns.Add("PageText");

            //設定下方頁數頁碼的首尾頁索引數值
            _firstIndex = CurrentPage - 5;
            if (CurrentPage > 5)
                _lastIndex = CurrentPage + 5;
            else
                _lastIndex = 10;

            // 檢查最後一頁是否大於總頁數，然後將其設為總頁數，並回推第一個可跳轉分頁的頁碼
            if (_lastIndex > Convert.ToInt32(ViewState["TotalPages"]))
            {
                _lastIndex = Convert.ToInt32(ViewState["TotalPages"]);
                _firstIndex = _lastIndex - 10;
            }

            //如果第一頁索引小於0時 將他設回0
            if (_firstIndex < 0)
                _firstIndex = 0;

            //根據前面的first 和 last page索引，建立頁碼
            for (var i = _firstIndex; i < _lastIndex; i++)
            {
                var dr = dtt.NewRow();
                dr[0] = i;
                dr[1] = i + 1;
                dtt.Rows.Add(dr);
            }
            rptPaging.DataSource = dtt;
            rptPaging.DataBind();
        }


        protected void rptPaging_ItemCommand(object source, DataListCommandEventArgs e)
        {
            //如果點選新的頁碼 ，才會繫結的該頁的PageIndex成CurrentPage。並重新bind指定頁碼的repeater分頁
            if (!e.CommandName.Equals("newPage")) return;
            CurrentPage = Convert.ToInt32(e.CommandArgument.ToString());
            BindDataIntoRepeater();
        }

        protected void lbFirst_Click1(object sender, EventArgs e)
        {
            //使頁碼歸零 ，0會繫結PageIndex成CurrentPage。並重新bind回repeater分頁到第一頁
            CurrentPage = 0;
            BindDataIntoRepeater();
        }

        protected void lbPrevious_Click1(object sender, EventArgs e)
        {
            //使頁碼減一 ，會更新繫結PageIndex成CurrentPage。並重新bind回repeater分頁到前一頁
            CurrentPage -= 1;
            BindDataIntoRepeater();
        }

        protected void lbNext_Click1(object sender, EventArgs e)
        {
            //使頁碼加一 ，會更新繫結PageIndex成CurrentPage。並重新bind回repeater分頁到後頁
            CurrentPage += 1;
            BindDataIntoRepeater();
        }

        protected void lbLast_Click1(object sender, EventArgs e)
        {
            //使當前頁根據TotalPages更新 ，會繫結總頁數  即最後一頁成CurrentPage。並重新bind回repeater分頁到最後
            CurrentPage = (Convert.ToInt32(ViewState["TotalPages"]) - 1);
            BindDataIntoRepeater();
        }
        protected void rptPaging_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            //取出頁數LinkButton值，紀錄lbPaging參數，當不是CurrentPage就不要紀錄進lnkPage，以便後續設定實際當前分頁狀態
            var lnkPage = (LinkButton)e.Item.FindControl("lbPaging");
            if (lnkPage.CommandArgument != CurrentPage.ToString()) return;
            //使無法重複點選當前所在頁連結
            lnkPage.Enabled = false;
            //設定當前分頁頁碼格顏色
            lnkPage.BackColor = Color.FromName("#F75C2F");
        }
        #endregion

        //搜尋課程button，將搜尋結果呼叫Bind進repeater及建立月曆 以更新畫面資料
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            //查詢選課資料放到dt_courses跟前端綁定的rptResult
            rptResult.DataSource = dt_courses = _studentManagers.SearchCourseRecordToDrop(_ID, txtCourseID.Text, txtCourseName.Text, txtStartDate1.Text, txtStartDate2.Text, txtPlace.Text, TxtPrice1.Text, TxtPrice2.Text, ddlTeacher.SelectedValue);
            BindDataIntoRepeater();
            CreateCalendar();
            rptResult.DataBind();
        }

        //建立月曆表格內容
        protected void CreateCalendar()//int InYear, int InMonth)
        {
            //依據使用者搜尋值帶出查詢的可退課程資料
            DataTable dt_course = _studentManagers.SearchCourseRecordToDrop(_ID, txtCourseID.Text, txtCourseName.Text, txtStartDate1.Text, txtStartDate2.Text, txtPlace.Text, TxtPrice1.Text, TxtPrice2.Text, ddlTeacher.SelectedValue);
            //後續用來裝載每日每堂課程資訊
            DataTable dt_calendar = new DataTable();
            dt_calendar.Columns.Add(new DataColumn("Date"));
            dt_calendar.Columns.Add(new DataColumn("Course"));
            dt_calendar.Columns.Add(new DataColumn("Place"));
            dt_calendar.Columns.Add(new DataColumn("StartTime"));

            //找到當前月份的1號，並記錄他屬於該週的哪一天
            int j = (int)datetime.AddDays(-datetime.Day + 1).DayOfWeek;
            //填滿1號前該週空格
            for (int i = 0; i < j; i++)
                dt_calendar.Rows.Add("");
            //產生當前月的日期列表 逐日增加
            for (int i = 1; i <= DateTime.DaysInMonth(datetime.Year, datetime.Month); i++)
            {
                DataRow dr = dt_calendar.NewRow();
                //裝日期號碼
                dr[0] = i.ToString();
                //用來存放有課日時的課程資訊清單
                List<StudentCourseTimeModel> _tempClassList = new List<StudentCourseTimeModel>();

                foreach (DataRow r in dt_course.Rows)
                {
                    //使用正規表達式調整輸出的課程時段格式
                    Regex regex = new Regex(@"\d{2}:\d{2}");
                    //記錄每一筆課程資料 塞入清單
                    StudentCourseTimeModel _tempclass = new StudentCourseTimeModel((DateTime)r["StartDate"], (DateTime)r["EndDate"], $"{r["C_Name"]} {r["Place_Name"]} {regex.Match(r["StartTime"].ToString())}");
                    if (!_tempClassList.Contains(_tempclass))
                        _tempClassList.Add(_tempclass);
                }
                //每次迴圈清空 重新裝新課程資料串 ，如果是與開課日期相同星期時間，且在開課至結訓時間區間內，則將課程資料串加入該日要列出的課表
                string _tmpstr = string.Empty;
                foreach (StudentCourseTimeModel tempclass in _tempClassList)
                {
                    if (tempclass.Check(DateTime.Parse($"{datetime.Year}/{datetime.Month}/{i}")))
                    {
                        _tmpstr += $"{tempclass.ClassName}<br>";
                    }
                }
                //串接好的課程資訊字串 填入此迴圈當前日期
                dr[1] = _tmpstr;
                dt_calendar.Rows.Add(dr);
            }

            //資料綁定至月曆
            Calendar.DataSource = dt_calendar;
            Calendar.DataBind();

            //設定月曆上當天顏色
            if (datetime.ToString("yyyy/MM") == DateTime.Now.ToString("yyyy/MM"))
                Calendar.Items[datetime.Day + j - 1].BackColor = Color.LightPink;
        }

        //月曆的上、下一月功能
        protected void NextMonth_Click(object sender, EventArgs e)
        {
            switch (((Button)sender).CommandName)
            {
                case "Next":
                    datetime = datetime.AddMonths(1);
                    break;
                case "Previous":
                    datetime = datetime.AddMonths(-1);
                    break;
            }
            monthOnCalendar.Text = $"{datetime.ToString("yyyy/MM")}月課程紀錄";
            CreateCalendar();
        }

        #region 退課功能

        //簡介內容
        protected void ShowRemark(object sender, CommandEventArgs e)
        {
            DataRow dr = GetCurrentCourse(e.CommandArgument.ToString())[0];
            //三元運算  如果簡介是NULL也可以有通知
            Remarks.Text = string.IsNullOrEmpty(dr["CourseIntroduction"].ToString())
                ? "此課程暫無簡介"
                : (string)dr["CourseIntroduction"];
        }

        //退課勾選框狀態更換時
        protected void AddCourseCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //點選時物件傳入轉型回CheckBox,後續才能取得勾選與否狀態
            CheckBox ckbox = (CheckBox)sender;
            //CheckBox繫結Course_ID的取得並轉文字
            DataRow result = GetCurrentCourse(ckbox.Text)[0];
            //當勾選框狀態是勾起時，將勾選的課程資料加入待退課購物車
            if (ckbox.Checked)
            {
                //為購物車新增一列 裝載新勾的課程
                DataRow dr = dt_cart.NewRow();
                dr["Course_ID"] = result["Course_ID"];
                dr["C_Name"] = result["C_Name"];
                dr["Price"] = result["Price"];
                //LINQ 判斷目前新勾的課程是否已經有在購物車了，確認沒有選過才讓資料真正帶入購物車中
                int results = dt_cart.AsEnumerable().Where(d =>
                {
                    return d["Course_ID"].ToString() == result["Course_ID"].ToString();
                }).ToList().Count;
                if (results == 0)
                    dt_cart.Rows.Add(dr);
            }
            //當勾選框狀態是取消勾選時，將該課程資料從購物車移出
            else
            {
                //先將目前購物車資料複製一份集合，避免直接跑取消的foreach時，一移除造成的總數不合異常
                DataRow[] rows = new DataRow[dt_cart.Rows.Count];
                //複製出來的購物車清單放入，從索引0開始
                dt_cart.Rows.CopyTo(rows, 0);
                //用複製出來的清單當作對照，刪除原本購物車內 與取消勾選的對照一致的資料
                foreach (DataRow row in rows)
                    if (row["Course_ID"].ToString() == ckbox.Text)
                        dt_cart.Rows.Remove(row);
            }
            //更新購物車內容物實際畫面
            Repeater1.DataSource = dt_cart;
            Repeater1.DataBind();
        }
        protected List<DataRow> GetCurrentCourse(string Course_ID)
        {
            //LINQ  當dt_courses課程表裡的ID等於傳入的ID時，就將其加入集合
            List<DataRow> results = dt_courses.AsEnumerable().Where(d =>
            {
                return d["Course_ID"].ToString() == Course_ID;
            }).ToList();
            return results;//回傳集合
        }


        //計算目前選課總金額
        protected int TotalPrice()
        {
            int totalprice = 0;
            //取得購物車內每筆課程金額進行加總
            foreach (DataRow dr in dt_cart.Rows)
                totalprice += (int)dr["Price"];
            //回傳計算完成的總額
            return totalprice;
        }

        /// <summary>
        /// 送出退課需求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCheckout_Click(object sender, EventArgs e)
        {
            //將退課需求加入資料庫
            _studentManagers.AddCart(_ID, dt_cart, "DropCart");
            //將退課需求修改到Registration_record
            _studentManagers.DropCourse(_ID, dt_cart, DateTime.Now);
            //退課流程完成後跳轉回歷史課程頁面
            Response.Redirect("~/Students/StudentCourseRecord.aspx");
        }
        #endregion

    }
}