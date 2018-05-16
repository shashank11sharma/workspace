<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="wp_AddGroupUsersUserControl.ascx.cs"
    Inherits="PWC.Process.SixSigma.wp_AddGroupUsers.wp_AddGroupUsersUserControl" %>
<link href="/_layouts/15/PWC.Process.SixSigma/css/SelectUsers.css" rel="stylesheet"
    type="text/css" />
<script src="/_layouts/15/PWC.Process.SixSigma/js/jquery-1.11.0.min.js" type="text/javascript"></script>
<script src="/_layouts/15/PWC.Process.SixSigma/js/SelectUsersPopUp.js" type="text/javascript"></script>
<script src="/_layouts/15/PWC.Process.SixSigma/js/jquery.SPServices-0.7.1a.js" type="text/javascript"></script>
<script src="/_layouts/15/PWC.Process.SixSigma/js/Modal.js" type="text/javascript"></script>
 
<script type="text/javascript">
    var LanguageId = 1;
    var url;
    var groupname;
    var SigmaId = 0;
    var User;
    function setLanguage(language) {
        var languagequery = "<Query><Where><Eq><FieldRef Name='LanguageValue' /><Value Type='Number'>" + language + "</Value></Eq></Where></Query>";
        $().SPServices({
            debug: true,
            operation: "GetListItems",
            async: false,
            listName: "Lookup_ProcertMultilingual_List",
            CAMLQuery: languagequery,
            completefunc: function (xData, Status) {
                //     alert(xData.responseText);
                $(xData.responseXML).SPFilterNode("z:row").each(function () {


                    $("[id*='lblSearch']").text($(this).attr("ows_lblSearch"));
                    $("[id*='spanSendEmail']").text($(this).attr("ows_LabelspanSendEmail"));
                    $("[id*='lblmanageattendee']").text($(this).attr("ows_BtnReassign"));
                    $("[id*='spanAvailable']").text($(this).attr("ows_lblAvailable"));
                    $("[id*='spanSelected']").text($(this).attr("ows_lblSelected"));

                    $("[id*='ConfirmUsers']").val($(this).attr("ows_BtnOk"));
                    $("[id*='Cancel']").val($(this).attr("ows_BtnCancel"));





                });
            }
        });
    }
    $(document).ready(function () {


        $("#s4-ribbonrow").hide();

        var a = parent.document;
        $('#dialogTitleSpan', a).attr('style', 'display: none');

        url = window.location.href; ;

        LanguageId = getParameterByName("LanguageId", url);
        groupname = getParameterByName("Group", url);
        SigmaId = getParameterByName("SigmaId", url);
       
        if (groupname === ("BlackBelt")) {
            document.getElementById('<%= spanBlkBlts.ClientID %>').style.display = "";
            document.getElementById('<%= spanBlkBlt.ClientID %>').style.display = "";
            document.getElementById('<%= spanblckbelt.ClientID %>').style.display = "";


        }
        else if (groupname === ("GreenBelt")) {
            document.getElementById('<%= spanGrnBlts.ClientID %>').style.display = "";
            document.getElementById('<%= spanGrnBlt.ClientID %>').style.display = "";
            document.getElementById('<%= spanGrnbelt.ClientID %>').style.display = "";


        }
        setLanguage(LanguageId)
        filterMainSelection();

    });

    function getParameterByName(name, url) {
        if (!url) url = window.location.href;
        name = name.replace(/[\[\]]/g, "\\$&");
        var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
        if (!results) return null;
        if (!results[2]) return '';
        return decodeURIComponent(results[2].replace(/\+/g, " "));
    }

</script>

<center>
    <asp:UpdatePanel ID="updatePanel" runat="server">
        <ContentTemplate>
            <table id="selectUserTable" runat="server" class="tableOtherCss">
                <tr>
                    <td align="left" style="font-family: Calibri; color: Black; font-size: 12pt; text-align: left;"
                        bgcolor="#ffffff" colspan="3">
                        <span id="lblmanageattendee" runat="server"></span><span runat="server" id="spanblckbelt"
                            style="display: none">Black Belt</span> <span runat="server" id="spanGrnbelt" style="display: none">
                                Green Belt</span>
                        <%--<asp:Label ID="lblProjectInfo" runat="server"></asp:Label>--%>
                    </td>
                </tr>
                <tr style="height: 5px; background-color: #000000">
                    <td colspan="3">
                    </td>
                </tr>
                <tr>
                    <td align="left" colspan="2" style="padding-bottom: 5px; padding-top: 20px">
                        <asp:Label ID="lblSearch" runat="server" CssClass="labelNames" Font-Size="15px" Text="Search"></asp:Label>
                        &nbsp;
                        <asp:TextBox ID="search_TB" runat="server" ForeColor="GrayText" onblur="if(this.value==''){this.value=this.defaultValue;this.style.color='GrayText'}"
                            value="Type username to search" onfocus="if(this.value==this.defaultValue){this.value='';this.style.color='Black'}"
                            onKeyup="javascript:filterUserList();" Width="230px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="padding-left: 2px">
                        <span runat="server" id="spanAvailable" style="font-family: Calibri; font-size: 15px;
                            color: Black;">Available</span> <span runat="server" id="spanBlkBlts" style="font-family: Calibri;
                                font-size: 15px; color: Black; display: none">Black Belts </span><span runat="server"
                                    id="spanGrnBlts" style="font-family: Calibri; font-size: 15px; color: Black;
                                    display: none">Green Belts </span>
                    </td>
                    <td>
                        <span runat="server" id="spanSelected" style="font-family: Calibri; font-size: 15px;
                            color: Black;">Selected</span> <span runat="server" id="spanBlkBlt" style="font-family: Calibri;
                                font-size: 15px; color: Black; display: none">Black Belt</span> <span runat="server"
                                    id="spanGrnBlt" style="font-family: Calibri; font-size: 15px; color: Black; display: none">
                                    Green Belt</span>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:ListBox ID="LB_MainSelection" runat="server" CssClass="userNamesLHS" Height="270px"
                            ondblclick="javascript:addUsers('no',LanguageId);" Rows="5" SelectionMode="Single" Width="285px">
                        </asp:ListBox>
                        <asp:ListBox ID="LB_CentralData" runat="server" CssClass="labelNames" Height="200px"
                            SelectionMode="Single" Style="display: none" Width="285px"></asp:ListBox>
                    </td>
                    <td>
                        <asp:Button ID="bt_addUser" runat="server" CssClass="btn" OnClientClick="javascript:return addUsers('no',LanguageId);"
                            Style="width: 75px; margin-bottom: 5px" Text="&gt;&gt;" />
                        <br />
                        <asp:Button ID="bt_removeUser" runat="server" CssClass="btn" OnClientClick="javascript:return removeUsers('yes',LanguageId);"
                            Style="width: 75px;" Text="&lt;&lt;" />
                    </td>
                    <td>
                        <asp:ListBox ID="LB_SelectedUserList" runat="server" CssClass="userNamesRHS" Height="270px"
                            ondblclick="javascript:removeUsers('yes',LanguageId);" SelectionMode="Single" Width="285px">
                        </asp:ListBox>
                    </td>
                </tr>
                <tr style="height: 5px;">
                    <td colspan="3">
                        <asp:Label ID="ErrorLabel" Font-Size="11pt" Font-Names="Calibri" ForeColor="Red"
                            runat="server"></asp:Label>
                    </td>
                </tr>
                
                <tr bgcolor="#000000">
                    <td align="right" colspan="3">
                        <asp:Button ID="ConfirmUsers" runat="server" CssClass="btn" OnClick="btnOk_Click"
                            OnClientClick="javascript:return setUserValues();" Style="width: 100px;" Text="OK" />
                        <asp:Button ID="Cancel" runat="server" CssClass="btn" OnClick="btnCancel_Click" Style="width: 100px;"
                      OnClientClick="javascript:return ModalCancel_click();"  Text="Cancel" />
                    </td>
                </tr>
                <tr style="height: 5px; background-color: #0070C0">
                    <td colspan="3">
                    </td>
                </tr>
            </table>
            <asp:HiddenField ID="HiddenField1" runat="server" />
            <asp:HiddenField ID="hiddenPreviousUser" runat="server" />
            <asp:HiddenField ID="HiddenField2" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</center>
