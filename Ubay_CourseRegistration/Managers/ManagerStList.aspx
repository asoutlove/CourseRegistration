<%@ Page Title="" Language="C#" MasterPageFile="~/Managers/ManagerMaster.Master" AutoEventWireup="true" CodeBehind="ManagerStList.aspx.cs" Inherits="Ubay_CourseRegistration.Managers.ManagerStList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="margin:20px;">
        <h1 style="margin: 0,auto; left: 85%; position: relative;">學生資料維護</h1>
        <a href="ManagerStDetail.aspx">新增</a>

        <div>
            進階搜尋：
        <p>
            姓名:
            <asp:TextBox runat="server" ID="txtName"></asp:TextBox>
            身分證字號:
            <asp:TextBox runat="server" ID="txtIdn"></asp:TextBox>

            <asp:Button runat="server" ID="btnSearch" Text="Search" OnClick="btnSearch_Click" />
        </p>
        </div>

        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" OnRowDataBound="GridView1_RowDataBound" OnRowCommand="GridView1_RowCommand">
            <Columns>
                <asp:TemplateField HeaderText="姓名">
                    <ItemTemplate>
                        <a href="ManagerStDetail.aspx?Student_ID=<%# Eval("Student_ID") %>">
                            <%# Eval("S_FirstName") %><%# Eval("S_LastName") %>
                        </a>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="性別">
                    <ItemTemplate>
                        <asp:Literal ID="gender" runat="server"></asp:Literal>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="Idn" HeaderText="身份證字號" />
                <asp:BoundField DataField="CellPhone" HeaderText="手機" />
                <asp:BoundField DataField="Address" HeaderText="地址" />


                <asp:TemplateField HeaderText="Act">
                    <ItemTemplate>
                        <asp:Button runat="server" ID="btnDelete" Text="刪除" CommandName="DeleteItem"
                            CommandArgument='<%# Eval("Student_ID") %>' OnClientClick='<%#Eval("Idn","return confirm(\"確定要刪除{0}嗎?\");") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <asp:Repeater runat="server" ID="repPaging">
            <ItemTemplate>
                <a href="<%# Eval("Link") %>" title="<%# Eval("Idn") %>">第<%# Eval("Idn") %></a>
            </ItemTemplate>
        </asp:Repeater>
        <br />
        <asp:Label runat="server" ID="lblMsg" ForeColor="Red"></asp:Label>
    </div>

</asp:Content>
