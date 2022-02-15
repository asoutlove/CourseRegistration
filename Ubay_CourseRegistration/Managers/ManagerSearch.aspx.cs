using CoreProject.Helpers;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Ubay_CourseRegistration.Managers
{
    public partial class ManagerSearch : System.Web.UI.Page
    {
        const int _pageSize = 10;


        internal class PagingLink
        {
            public string name { get; set; }
            public string Link { get; set; }
            public string Account { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.LoadManagerGridView();
                this.RestoreParameters();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string name = this.txtName.Text;
            string account = this.txtAccount.Text;


            string template = "?Page=1";

            if (!string.IsNullOrEmpty(name))
                template += "&name=" + name;

            if (!string.IsNullOrEmpty(account))
                template += "&account=" + account;

            Response.Redirect("ManagerSearch.aspx" + template);
        }

        private void RestoreParameters()
        {
            string name = Request.QueryString["name"];
            string account = Request.QueryString["Account"];

            if (!string.IsNullOrEmpty(name))
                this.txtName.Text = name;

            if (!string.IsNullOrEmpty(account))
                this.txtAccount.Text = account;
        }

        private string GetQueryString(bool includePage, int? pageIndex)
        {
            //----- Get Query string parameters -----
            string page = Request.QueryString["Page"];
            string name = Request.QueryString["name"];
            string account = Request.QueryString["Account"];
            //----- Get Query string parameters -----


            List<string> conditions = new List<string>();

            if (!string.IsNullOrEmpty(page) && includePage)
                conditions.Add("Page=" + page);

            if (!string.IsNullOrEmpty(name))
                conditions.Add("name=" + name);

            if (!string.IsNullOrEmpty(account))
                conditions.Add("Account=" + account);

            if (pageIndex.HasValue)
                conditions.Add("Page=" + pageIndex.Value);

            string retText =
                (conditions.Count > 0)
                    ? "?" + string.Join("&", conditions)
                    : string.Empty;

            return retText;
        }

        private void LoadManagerGridView()
        {
            //----- Get Query string parameters -----
            string page = Request.QueryString["Page"];
            int pIndex = 0;
            if (string.IsNullOrEmpty(page))
                pIndex = 1;
            else
            {
                int.TryParse(page, out pIndex);

                if (pIndex <= 0)
                    pIndex = 1;
            }

            string name = Request.QueryString["name"];
            string account = Request.QueryString["Account"];

            int totalSize = 0;

            var manager = new ManagerManagers();
            var list = manager.GetManagerViewModels(name, account, out totalSize, pIndex, _pageSize);
            int pages = PagingHelper.CalculatePages(totalSize, _pageSize);

            List<PagingLink> pagingList = new List<PagingLink>();
            for (var i = 1; i <= pages; i++)
            {
                pagingList.Add(new PagingLink()
                {
                    Link = $"ManagerSearch.aspx{this.GetQueryString(false, i)}",
                    name = $"{i}",
                    Account = $" {i} 頁"
                });
            }

            this.repPaging.DataSource = pagingList;
            this.repPaging.DataBind();

            this.GridView1.DataSource = list;
            this.GridView1.DataBind();
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string cmdName = e.CommandName;
            string arg = e.CommandArgument.ToString();

            if (cmdName == "DeleteItem")
            {
                Guid id;
                if (Guid.TryParse(arg, out id))
                {
                    var manager = new ManagerManagers();
                    Guid delete = (Guid)Session["Acc_sum_ID"];
                    manager.DeleteManagerViewModel(id, delete);

                    this.LoadManagerGridView();
                    this.lblMsg.Text = "已刪除。";
                    this.lblMsg.Visible = true;
                }
            }
        }
    }
}