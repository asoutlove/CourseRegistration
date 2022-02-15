<%@ Page Title="" Language="C#" MasterPageFile="~/Managers/ManagerMaster.Master" AutoEventWireup="true" CodeBehind="ManagerSearch.aspx.cs" Inherits="Ubay_CourseRegistration.Managers.ManagerSearch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<div style="margin:20px;">
        <h1 style="margin: 0,auto; left: 85%; position: relative;">查詢管理人資料</h1>
        <a href="ManagerRegion.aspx">新增</a>

        <div>
            進階搜尋：
        <p>
            姓名:
            <asp:TextBox runat="server" ID="txtName"></asp:TextBox>
            帳號:
            <asp:TextBox runat="server" ID="txtAccount"></asp:TextBox>

            <asp:Button runat="server" ID="btnSearch" Text="Search" OnClick="btnSearch_Click" />
        </p>
        </div>

        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" OnRowCommand="GridView1_RowCommand">
            <Columns>
                <asp:TemplateField HeaderText="姓名">
                    <ItemTemplate>  
                        <a href="ManagerRegion.aspx?Manager_ID=<%# Eval("Manager_ID") %>">
                            <%# Eval("firstname") %><%# Eval("lastname") %>
                        </a>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="Department" HeaderText="單位" />
                <asp:BoundField DataField="Account" HeaderText="帳號" />


                <asp:TemplateField HeaderText="Act">
                    <ItemTemplate>
                        <asp:Button runat="server" ID="btnDelete" Text="Del" CommandName="DeleteItem"
                            CommandArgument='<%# Eval("Manager_ID") %>' OnClientClick="return confirm('確定刪除嗎?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <asp:Repeater runat="server" ID="repPaging">
            <ItemTemplate>
                <a href="<%# Eval("Link") %>" title="<%# Eval("Account") %>">第<%# Eval("Account") %></a>
            </ItemTemplate>
        </asp:Repeater>
        <br />
        <asp:Label runat="server" ID="lblMsg" ForeColor="Red"></asp:Label>
    </div>

</asp:Content>

