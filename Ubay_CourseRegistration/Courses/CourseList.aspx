<%@ Page Title="" Language="C#" MasterPageFile="~/Managers/ManagerMaster.Master" AutoEventWireup="true" CodeBehind="CourseList.aspx.cs" Inherits="Ubay_CourseRegistration.Courses.CourseList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .auto-style1 {
            width: 80%;
        }

        table {
            border-collapse: collapse;
        }

        table, td, th {
            border: 1px solid #A0674B;
        }

        th {
            background-color: #ECB88A;
            color: white;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div role="dialog" class="el_dialog" style="margin: 5vh 0vh 5vh 0vh; width: 1160px;">
        <div class="studentPage_header">
            <%--課程歷史紀錄分頁標題--%>
            <h1 style="margin: 0,auto; left: 50%; position: relative; width: 100%;">課程歷史紀錄</h1>
        </div>

        <div class="studentPage_body">
            <div>
                <%--搜尋--%>
                <div style="margin: 0,auto; left: 20%; position: relative;">
                    <p>
                        課程ID:
                        <asp:TextBox runat="server" ID="txtCourseID" Width="5%" MaxLength="10"></asp:TextBox>
                        課程名稱:<asp:TextBox runat="server" ID="txtCourseName" MaxLength="20"></asp:TextBox>
                        教師: 
            <asp:DropDownList runat="server" ID="ddlTeacher">
            </asp:DropDownList>
                        授課時間:
            <asp:TextBox runat="server" ID="txtStartDate1" TextMode="Date" placeholder="from"></asp:TextBox>~
            <asp:TextBox runat="server" ID="txtStartDate2" TextMode="Date" placeholder="to"></asp:TextBox>
                    </p>
                    <p>
                        教室:
                        <asp:TextBox runat="server" ID="txtPlace" Width="5%" MaxLength="10"></asp:TextBox>
                        價格:
            <asp:TextBox runat="server" ID="TxtPrice1" placeholder="最小值" TextMode="Number"></asp:TextBox>~
            <asp:TextBox runat="server" ID="TxtPrice2" placeholder="最大值" TextMode="Number"></asp:TextBox>

                        課程狀態:
            <asp:DropDownList ID="ddlCourseStatus" runat="server">
                <asp:ListItem Text="" Value="0"></asp:ListItem>
                <asp:ListItem Text="開放" Value="1"></asp:ListItem>
                <asp:ListItem Text="關閉" Value="2"></asp:ListItem>
            </asp:DropDownList>

                        <asp:Button runat="server" ID="btnSearch" Text="Search" OnClick="btnSearch_Click" />
                        <br />
                        <br />
                        <a href="CourseDetail.aspx">新增課程</a>
                    </p>
                </div>
                <br />

                <%--顯示所有歷史課程repeater--%>
                <div style="margin: 0,auto; left: 20%; position: relative;">
                    <div>
                        <asp:Repeater ID="rptResult" runat="server" OnItemCommand="rptResult_ItemCommand">
                            <HeaderTemplate>
                                <table class="auto-style1">
                                    <tr>
                                        <th>課程ID</th>
                                        <th>課程名稱</th>
                                        <th>教師</th>
                                        <th>開課時間</th>
                                        <th>結訓日期</th>
                                        <th>選課人數上限</th>
                                        <th>上課地點</th>
                                        <th>價格</th>
                                        <th>狀態</th>
                                        <th>刪除</th>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td><%#Eval("Course_ID") %></td>
                                    <td><a href="CourseDetail.aspx?Course_ID=<%# Eval("Course_ID") %>"><%#Eval("C_Name") %></td>
                                    <td><%#Eval("Teacher_FirstName") %> <%#Eval("Teacher_LastName") %></td>
                                    <td><%#Eval("StartDate", "{0:yyyy-MM-dd}") %> <%#Eval("StartTime","{0:hh}") %>:<%#Eval("StartTime","{0:mm}") %></td>
                                    <td><%#Eval("EndDate", "{0:yyyy-MM-dd}") %></td>
                                    <td>上限:<%#Eval("MaxNumEnrolled") %>人 / 已選:<%#Eval("MinNumEnrolled") %></td>
                                    <td><%#Eval("Place_Name") %></td>
                                    <td><%#Eval("Price","{0,0:C0}") %></td>
                                    <td><%#((Eval("d_date").ToString()=="")?"開放":"關閉")%></td>
                                    <td>
                                        <asp:Button runat="server" ID="btnDelete" Text="Del" CommandName="DeleteItem"
                                            CommandArgument='<%# Eval("Course_ID") %>' OnClientClick='<%#Eval("C_Name","return confirm(\"確定要刪除{0}嗎?\");") %>' /></td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                    <asp:Label ID="lblMsg" runat="server" ForeColor="Red" Font-Size="16px"></asp:Label>
                    <div style="margin-top: 20px;">
                        <%--Repeater的跳頁控制--%>
                        <table style="margin: 0,auto; left: 20%; position: relative; width: 600px;">
                            <tr>
                                <td>
                                    <asp:LinkButton ID="lbFirst" runat="server" OnClick="lbFirst_Click1" ForeColor="#A0674B">First</asp:LinkButton>
                                </td>
                                <td>
                                    <asp:LinkButton ID="lbPrevious" runat="server" OnClick="lbPrevious_Click1" ForeColor="#A0674B">Previous</asp:LinkButton>
                                </td>
                                <td>
                                    <asp:DataList ID="rptPaging" runat="server"
                                        OnItemCommand="rptPaging_ItemCommand"
                                        OnItemDataBound="rptPaging_ItemDataBound" RepeatDirection="Horizontal" ForeColor="#A0674B">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lbPaging" runat="server"
                                                CommandArgument='<%# Eval("PageIndex") %>' CommandName="newPage"
                                                Text='<%# Eval("PageText") %> ' Width="20px" ForeColor="#A0674B"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:DataList>
                                </td>
                                <td>
                                    <asp:LinkButton ID="lbNext" runat="server" OnClick="lbNext_Click1" ForeColor="#A0674B">Next</asp:LinkButton>
                                </td>
                                <td>
                                    <asp:LinkButton ID="lbLast" runat="server" OnClick="lbLast_Click1" ForeColor="#A0674B">Last</asp:LinkButton>
                                </td>
                                <td>
                                    <asp:Label ID="lblpage" runat="server" Text="" ForeColor="#A0674B"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
            <%--顯示所有課程紀錄月曆--%>
            <div style="left: 20%; position: relative; width: 900px;">

                <asp:Table Width="100%" runat="server">
                    <asp:TableRow>
                        <asp:TableCell HorizontalAlign="Center">
                            <asp:Label ID="monthOnCalendar" runat="server"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell Width="10%" HorizontalAlign="Center">
                        <asp:Button runat="server" Text="上個月" CommandName="Previous" OnClick="NextMonth_Click" />
                        </asp:TableCell>
                        <asp:TableCell Width="10%" HorizontalAlign="Center">
                        <asp:Button runat="server" Text="下個月" CommandName="Next" OnClick="NextMonth_Click" />
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>

                <asp:DataList ID="Calendar" runat="server" RepeatDirection="Horizontal" RepeatColumns="7" Width="100%">
                    <HeaderTemplate>
                        <asp:Table runat="server" Width="100%">
                            <asp:TableRow>
                                <asp:TableCell HorizontalAlign="Center" BorderStyle="None">星期日</asp:TableCell>
                                <asp:TableCell HorizontalAlign="Center" BorderStyle="None">星期一</asp:TableCell>
                                <asp:TableCell HorizontalAlign="Center" BorderStyle="None">星期二</asp:TableCell>
                                <asp:TableCell HorizontalAlign="Center" BorderStyle="None">星期三</asp:TableCell>
                                <asp:TableCell HorizontalAlign="Center" BorderStyle="None">星期四</asp:TableCell>
                                <asp:TableCell HorizontalAlign="Center" BorderStyle="None">星期五</asp:TableCell>
                                <asp:TableCell HorizontalAlign="Center" BorderStyle="None">星期六</asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Panel runat="server" ID="Panel1" ScrollBars="Auto" Height="100">
                            <div style="width: 100px;">
                                <div style="text-align: center; vertical-align: top;">
                                    <label><%#Eval("Date")%></label>
                                </div>
                                <div style="text-align: left; vertical-align: top; height: 100%">
                                    <h5><%#Eval("Course")%><%#Eval("StartTime") %></h5>
                                </div>
                            </div>
                        </asp:Panel>
                    </ItemTemplate>
                </asp:DataList>
            </div>
        </div>
    </div>
</asp:Content>
