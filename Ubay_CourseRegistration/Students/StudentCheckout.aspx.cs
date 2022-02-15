using CoreProject.Managers;
using System;
using System.Data;
using System.Text.RegularExpressions;

namespace Ubay_CourseRegistration.Students
{
    public partial class StudentCheckout : System.Web.UI.Page
    {
        //用來裝目前登入學生帳號
        string _id;
        //引用學生方法
        StudentManagers _studentManagers = new StudentManagers();
        //購物車資料表
        DataTable dt_cart;
        //計算待結帳總額
        protected int TotalPrice = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            _id = Session["Acc_sum_ID"].ToString();
            //取得目前登入學生的購物車列表
            dt_cart = _studentManagers.GetCart(_id);
            //計算總金額
            foreach (DataRow dr in dt_cart.Rows)
                TotalPrice += Convert.ToInt32(dr["Price"]);
        }

        protected void CheckOut(object sender, EventArgs e)
        {
            //比對信用卡值須為數 且數字量是xxxx-xxxx-xxxx-xxxx
            Regex regexCreditCard = new Regex(@"\d{4}-\d{4}-\d{4}-\d{4}");
            //比對到期日格式各兩碼
            Regex regexMonthYear = new Regex(@"\d{2}/\d{2}");
            //比對信用卡安全碼為三位數
            Regex regexCVN = new Regex(@"\d{3}");
            //將使用者輸入的信用卡號帶入
            string cardNumber = $"{CreditCard1.Text}{CreditCard2.Text}{CreditCard3.Text}{CreditCard4.Text}";
            StudentManagers _studentManagers = new StudentManagers();

            //比對輸入數值長度正確與否
            if (regexCreditCard.Match($"{CreditCard1.Text}-{CreditCard2.Text}-{CreditCard3.Text}-{CreditCard4.Text}").Length < 1)
            {
                ShowAlert("信用卡卡號錯誤");
                return;
            }
            //比對信用卡格式 依據信用卡驗證方法
            if (!_studentManagers.CheckCreditCardNo(cardNumber))
            {
                ShowAlert("信用卡格式錯誤");
                return;
            }
            //比對到期日格式
            if (regexMonthYear.Match($"{Month.Text}/{Year.Text}").Length < 1)
            {
                ShowAlert("月/年錯誤");
                return;
            }
            //比對信用卡安全碼
            if (regexCVN.Match($"{CVN.Text}").Length < 1)
            {
                ShowAlert("安全碼錯誤");
                return;
            }
            //完成結帳後將選的課程新增到課成記錄內，並將該課程選課人數+1
            _studentManagers.studentCheckoutCourse(_id, dt_cart, DateTime.Now);
            _studentManagers.ClearCart(_id);
            //完成選課後轉跳到學生歷史課程查詢頁
            Response.Write($"<script>confirm('選課成功');location.href = 'StudentCourseRecord.aspx';</script>");
        }

        /// <summary>
        /// 驗證後顯示訊息
        /// </summary>
        /// <param name="Msg">訊息內容</param>
        void ShowAlert(string Msg)
        {
            Response.Write($"<script>alert('{Msg}')</script>");
        }
    }
}