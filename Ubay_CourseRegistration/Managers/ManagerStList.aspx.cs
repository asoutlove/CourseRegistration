using CoreProject.Helpers;
using CoreProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Ubay_CourseRegistration.Managers
{
    public partial class ManagerStList : System.Web.UI.Page
    {
        const int _pageSize = 10;


        internal class PagingLink
        {
            public string name { get; set; }
            public string Link { get; set; }                           
            public string Idn { get; set; }
        }



        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.LoadGridView();
                this.RestoreParameters();
            }
        }

        private void RestoreParameters()
        {
            string name = Request.QueryString["name"];
            string idn = Request.QueryString["idn"];

            if (!string.IsNullOrEmpty(name))
                this.txtName.Text = name;

            if (!string.IsNullOrEmpty(idn))
                this.txtIdn.Text = idn;
        }

        private string GetQueryString(bool includePage, int? pageIndex)
        {
            //----- Get Query string parameters -----
            string page = Request.QueryString["Page"];
            string name = Request.QueryString["name"];
            string idn = Request.QueryString["idn"];
            //----- Get Query string parameters -----


            List<string> conditions = new List<string>();

            if (!string.IsNullOrEmpty(page) && includePage)
                conditions.Add("Page=" + page);

            if (!string.IsNullOrEmpty(name))
                conditions.Add("name=" + name);

            if (!string.IsNullOrEmpty(idn))
                conditions.Add("Idn=" + idn);

            if (pageIndex.HasValue)
                conditions.Add("Page=" + pageIndex.Value);

            string retText =
                (conditions.Count > 0)
                    ? "?" + string.Join("&", conditions)
                    : string.Empty;

            return retText;
        }


        private void LoadGridView()
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
            string idn = Request.QueryString["idn"];




            int totalSize = 0;

            var manager = new ManagerManagers();
            var list = manager.GetStudentViewModels(name, idn, out totalSize, pIndex, _pageSize);
            int pages = PagingHelper.CalculatePages(totalSize, _pageSize);

            List<PagingLink> pagingList = new List<PagingLink>();
            for (var i = 1; i <= pages; i++)
            {
                pagingList.Add(new PagingLink()
                {
                    Link = $"ManagerStList.aspx{this.GetQueryString(false, i)}",
                    name = $"{i}",
                    Idn = $" {i} 頁"
                });
            }

            this.repPaging.DataSource = pagingList;
            this.repPaging.DataBind();

            this.GridView1.DataSource = list;
            this.GridView1.DataBind();
        }


        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string name = this.txtName.Text;
            string idn = this.txtIdn.Text;


            string template = "?Page=1";

            if (!string.IsNullOrEmpty(name))
                template += "&name=" + name;

            if (!string.IsNullOrEmpty(idn))
                template += "&idn=" + idn;

            Response.Redirect("ManagerStList.aspx" + template);
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
                    manager.DeleteStudentViewModel(id,delete);

                    this.LoadGridView();
                    this.lblMsg.Text = "已刪除。";
                    this.lblMsg.Visible = true;
                }
            }
        }
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            ManagerManagers manager = new ManagerManagers();

            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Separator)
            {
                StudentAccountViewModel mode = e.Row.DataItem as StudentAccountViewModel;
                Literal ltgender = e.Row.FindControl("gender") as Literal;

                string val = manager.GetgenderName(mode.gender);
                ltgender.Text = val;
            }

        }

    }
}