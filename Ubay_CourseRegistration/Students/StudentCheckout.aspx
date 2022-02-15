<%@ Page Title="" Language="C#" MasterPageFile="~/Students/StudentMaster.Master" AutoEventWireup="true" CodeBehind="StudentCheckout.aspx.cs" Inherits="Ubay_CourseRegistration.Students.StudentCheckout" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <style type="text/css">

        table {
            border-collapse: collapse;

        }

        table, td, th {
            border: 1px solid #A0674B;
        }

      </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div role ="dialog" class="el_dialog" style ="margin:5vh 0vh 5vh 0vh ; width:1160px;">
       <div class="studentPage_header">
            <%--學生結帳分頁標題--%>
            <h1 style="margin: 0,auto; left: 50%; position: relative; width: 100%;">新課程結帳</h1>
        </div>
       <div style="margin: 0,auto; left: 35%; position: relative;">
     <asp:Table ID="Table1" runat="server">
        <asp:TableRow>
            <asp:TableCell>總金額:</asp:TableCell>
            <asp:TableCell><%=TotalPrice %></asp:TableCell>
        </asp:TableRow>
        <asp:TableFooterRow>
            <asp:TableCell>信用卡卡號:</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox runat="server" ID="CreditCard1" Width="70" MaxLength="4"></asp:TextBox>-
                <asp:TextBox runat="server" ID="CreditCard2" Width="70" MaxLength="4"></asp:TextBox>-
                <asp:TextBox runat="server" ID="CreditCard3" Width="70" MaxLength="4"></asp:TextBox>-
                <asp:TextBox runat="server" ID="CreditCard4" Width="70" MaxLength="4"></asp:TextBox>
            </asp:TableCell>
        </asp:TableFooterRow>
        <asp:TableRow>
            <asp:TableCell>有效期限:</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox runat="server" ID="Month" Width="50" MaxLength="2"></asp:TextBox>/
                <asp:TextBox runat="server" ID="Year" Width="50" MaxLength="2"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableFooterRow>
            <asp:TableCell>安全碼:</asp:TableCell>
            <asp:TableCell>
                 <asp:TextBox runat="server" ID="CVN" Width="70" MaxLength="3"></asp:TextBox>
            </asp:TableCell>
        </asp:TableFooterRow>
        <asp:TableRow>
            <asp:TableCell ColumnSpan="2">
                 <asp:Button runat="server" Text="結帳" OnClick="CheckOut" />
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
                </div>
      </div>
</asp:Content>
