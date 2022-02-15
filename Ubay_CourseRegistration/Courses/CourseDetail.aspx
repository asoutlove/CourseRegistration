<%@ Page Title="" Language="C#" MasterPageFile="~/Managers/ManagerMaster.Master" AutoEventWireup="true" CodeBehind="CourseDetail.aspx.cs" Inherits="Ubay_CourseRegistration.Courses.CourseDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="margin:20px;">
        <asp:Label ID="Course_title" runat="server" Text="修改課程" Style="margin: 0,auto; left: 45%; position: relative;" Font-Size="20pt"></asp:Label>
        <br />
        <br />
        <div>
            課程ID：
            <asp:TextBox ID="txtCourseID" runat="server"></asp:TextBox>
        </div>
        <br />
        <br />
        <div>
            課程名稱：
            <asp:TextBox ID="txtCourseName" runat="server"></asp:TextBox>
        </div>
        <br />
        <br />
        <div>
            授課教師：
        <asp:DropDownList ID="tcList" runat="server">

        </asp:DropDownList>
        </div>
        <br />
        <br />
        <div>
            開課時間：
            <asp:TextBox ID="Startdate" runat="server" TextMode="Date"></asp:TextBox>
            <asp:TextBox ID="Starttime" runat="server" TextMode="Time"></asp:TextBox>
        </div>
        <br />
        <br />
        <div>
            結訓日期：
            <asp:TextBox ID="Enddate" runat="server" TextMode="Date"></asp:TextBox>
        </div>
        <br />
        <br />
        <div>
            選課人數上限：
            <asp:TextBox ID="maxNum" runat="server" TextMode="Number"></asp:TextBox>
        </div>
        <br />
        <br />
        <div>
            上課地點：
            <asp:TextBox ID="Place" runat="server"></asp:TextBox>
        </div>
        <br />
        <br />
        <div>
            課程簡介：<br />
            <asp:TextBox ID="txtCourseIntroduction" runat="server" TextMode="MultiLine" Height="500" Width="600"></asp:TextBox>
        </div>
        <br />
        <br />
        <div>
            價格：
            <asp:TextBox ID="Price" runat="server" TextMode="Number"></asp:TextBox>
        </div>
        <br />
        <br />
        <asp:Button ID="btn_Course" runat="server" Text="確認修改" OnClick="btn_Course_Click"  />
        <br /><br />
        <asp:Label ID="lbMsg" runat="server"  Visible="false" ForeColor="Red"></asp:Label>
        <%--<asp:TextBox ID="saveMinnum" runat="server" Visible="false"></asp:TextBox>--%>
    </div>
</asp:Content>
