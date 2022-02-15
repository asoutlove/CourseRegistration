<%@ Page Title="" Language="C#" MasterPageFile="~/Managers/ManagerMaster.Master" AutoEventWireup="true" CodeBehind="ManagerRegion.aspx.cs" Inherits="Ubay_CourseRegistration.Managers.ManagerRegion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <br />
        <asp:Label ID="LB1" runat="server" Text="新增管理人資料" Style="margin: 0,auto; left: 15%; position: relative;" Font-Size="20pt"></asp:Label><br />
        <br />
        <div style="margin: 0,auto; left: 30%; position: relative;">
            <div>姓氏：<asp:TextBox runat="server" ID="txtFirstname" MaxLength="50" onkeyup="value=value.replace(/[^\w\u4E00-\u9FA5]/g, '')"></asp:TextBox></div>
            <br />
            <div>名字：<asp:TextBox runat="server" ID="txtLastname" MaxLength="50" onkeyup="value=value.replace(/[^\w\u4E00-\u9FA5]/g, '')"></asp:TextBox></div>
            <br />
            <div>單位：<asp:TextBox runat="server" ID="txtDepartment" MaxLength="50" onkeyup="value=value.replace(/[^\w\u4E00-\u9FA5]/g, '')"></asp:TextBox></div>
            <br />
            <div>帳號：<asp:TextBox runat="server" ID="txtAccount" MaxLength="50" onkeyup="value=value.replace(/[\W]/g,'')"></asp:TextBox></div>
            <br />
            <div>新密碼：
                <asp:TextBox runat="server" TextMode="Password" ID="txtPassword" MaxLength="50" onkeyup="value=value.replace(/[\W]/g,'')">
                </asp:TextBox>
                <asp:Label ID="Label3" runat="server" Text="(若不更改密碼則無需填寫)" Visible="true">
                </asp:Label>
            </div>
            <br />
            <div style="margin-left: -65px;">
                再次確認新密碼：
                <asp:TextBox runat="server" TextMode="Password" ID="txtPwdcheck" MaxLength="50" onkeyup="value=value.replace(/[\W]/g,'')">
                </asp:TextBox>
                <asp:Label ID="Label4" runat="server" Text="(若不更改密碼則無需填寫)" Visible="true">
                </asp:Label>
            </div>
            <br />
            <br />
            <asp:Button ID="Regis" runat="server" Text="確認註冊" Style="margin: 0,auto; left: 25%; position: relative;" OnClick="CreateAdmin_Click" />
            <asp:Button ID="Turnbackbtn" runat="server" Text="返回" OnClick="Turnback_Click" Style="margin: 0,auto; left: 25%;position: relative;"/>
            <br />
            <br />
            <asp:Label ID="WarningMsg" runat="server" Style="color: red; font-size: 20px; font-weight: bolder;"></asp:Label>
            <br />
            <asp:Literal ID="Literal1" runat="server"></asp:Literal>
            <br />

        </div>
    </div>

</asp:Content>
