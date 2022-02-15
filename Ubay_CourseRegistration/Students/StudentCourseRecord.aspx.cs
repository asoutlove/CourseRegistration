using CoreProject.Managers;
using CoreProject.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Ubay_CourseRegistration.Students
{
    public partial class StudentCourseRecord : System.Web.UI.Page
    {
        StudentManagers _studentManagers = new StudentManagers();
        //使用PagedDataSource物件實現課程資料repeater分頁
        readonly PagedDataSource _pgsource = new PagedDataSource();
        //預設會是當前年份月份，根據上個月及下個月按鈕的使用，改變數值成不同年份月份
        static DateTime datetime = DateTime.Now;
        //課程資料repeater下方頁碼數字首尾頁索引 
        int _firstIndex, _lastIndex;
        //設定課程資料repeater項目數量
        private int _pageSize = 10;
        //用來裝目前登入學生帳號
        string _ID ;

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
            //查詢教師的下拉選單內容方法
            _studentManagers.ReadTeacherTable(ref ddlTeacher);

            BindDataIntoRepeater();

            //建立月曆上的年月顯示資訊
            monthOnCalendar.Text = $"{datetime.ToString("yyyy/MM")}月課程紀錄";
            CreateCalendar();
        }

        //查詢並列出當前登入的學生的歷史報名資料
        private void BindDataIntoRepeater()
        {
            //呼叫方法查詢並列出當前登入的學生的歷史報名資料
            var dtt = _studentManagers.SearchCouser(_ID, txtCourseID.Text, txtCourseName.Text, txtStartDate1.Text, txtStartDate2.Text, txtPlace.Text, TxtPrice1.Text, TxtPrice2.Text, ddlTeacher.SelectedValue);
            //取得當前登入的學生的歷史報名資料
            _pgsource.DataSource = dtt.DefaultView;
            //在資料繫結控制項中啟用分頁
            _pgsource.AllowPaging = true;
            //要在Repeater顯示的項目數 
            _pgsource.PageSize = _pageSize;
            //目前頁面索引
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
            // Bind資料進Repeater 設定分頁頁碼、跳頁按鈕
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
        public void btnSearch_Click(object sender, EventArgs e)
        {
            rptResult.DataSource = _studentManagers.SearchCouser(_ID,txtCourseID.Text,txtCourseName.Text,txtStartDate1.Text,txtStartDate2.Text,txtPlace.Text,TxtPrice1.Text,TxtPrice2.Text,ddlTeacher.SelectedValue);
            BindDataIntoRepeater();
            CreateCalendar();
            rptResult.DataBind();
        }

        //建立月曆表格內容
        protected void CreateCalendar()
        {
            //依據使用者搜尋值帶出查詢的歷史課程紀錄
            DataTable dt_course = _studentManagers.SearchCouser(_ID, txtCourseID.Text, txtCourseName.Text, txtStartDate1.Text, txtStartDate2.Text, txtPlace.Text, TxtPrice1.Text, TxtPrice2.Text, ddlTeacher.SelectedValue);
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
            //資料綁定
            Calendar.DataSource = dt_calendar;
            Calendar.DataBind();
            //設定月曆上當天顏色
            if (datetime.ToString("yyyy/MM") == DateTime.Now.ToString("yyyy/MM"))
                Calendar.Items[datetime.Day + j - 1].BackColor = Color.LightPink;
        }

        //月曆的上、下個月共用功能
        protected void NextMonth_Click(object sender, EventArgs e)
        {
            switch (((Button)sender).CommandName)
            {
                //當點選下一月時 將當前月份+1
                case "Next":
                    datetime = datetime.AddMonths(1);
                    break;
                //當點選前一月時 將當前月份-1
                case "Previous":
                    datetime = datetime.AddMonths(-1);
                    break;
            }
            monthOnCalendar.Text = $"{datetime.ToString("yyyy/MM")}月課程紀錄";
            //需聯動到指定月份的月曆行程
            CreateCalendar();
        }

    }
}