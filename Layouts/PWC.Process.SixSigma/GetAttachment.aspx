<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GetAttachment.aspx.cs" Inherits="PWC.Process.SixSigma.Layouts.PWC.Process.SixSigma.GetAttachment" DynamicMasterPageFile="~masterurl/default.master" %>
<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <style type="text/css">
        fieldset
        {
            padding: 1em;
            border: 1px solid #676767; /*font:80%/1 sans-serif;*/
        }
        .table
        {
            padding-left: 30px;
            padding-top: 20px;
            padding-bottom: 5px;
        }
        
        .td
        {
            margin: 0;
            padding: 2px;
        }
        .New
        {
            background-color: #1e90ff;
            color: #000000;
        }
        .tdh
        {
            padding-left: 20px;
            font-size: 18px;
            font-weight: bold;
            color: #000000;
            font-family: Swis721 LtCn BT;
        }
        
        .labelHeader
        {
            font-family: "Swis721 LtCn BT";
            font-size: 20px;
            color: #FFFFFF;
        }
        
        .labelColumns
        {
            font-family: "Swis721 LtCn BT";
            font-size: 16px;
            color: #000000;
        }
        .labelColumnsDropdown
        {
            font-family: "Verdana, Arial";
            font-size: 12px;
            color: #000000;
        }
        .labelColumnNote
        {
            font-family: "Verdana, Arial";
            font-size: 12px;
            color: #000000;
        }
        
        #breadCrumbNew
        {
            width: 960px;
            padding: 10px 10px 5px 10px;
            margin: 0 auto;
        }
        #breadCrumbNew ul
        {
            margin: 0px;
            padding: 0px;
            list-style: none;
        }
        #breadCrumbNew ul li
        {
            float: left;
            margin: 0px;
            padding: 0px;
            list-style: none;
            background: url(/_layouts/static/images/bredcrum_arrow.png) 0px 0px no-repeat;
            padding-left: 18px;
            margin-left: 3px;
            font-size: 12px;
            color: #3e3e3e;
            font-family: "PT_Sans-Web-Regular" , "Arial" , "Helvetica" , "sans-serif";
        }
        #breadCrumbNew ul li:first-child
        {
            background: none;
            padding-left: 0px;
            margin-left: 0px;
        }
        #breadCrumbNew ul li a
        {
            color: #3e3e3e;
        }
        #breadCrumbNew ul li.acnchorLast a
        {
            font-weight: bold;
            text-decoration: none;
            cursor: text;
        }
        #breadCrumbNew ul li.acnchorLast a:hover
        {
            font-weight: bold;
            text-decoration: none;
            cursor: text;
        }
        
        .modal
        {
            position: fixed;
            top: 0;
            left: 0;
            background-color: black;
            z-index: 99;
            opacity: 0.8;
            filter: alpha(opacity=80);
            -moz-opacity: 0.8;
            min-height: 100%;
            width: 100%;
        }
        .loading
        {
            font-family: Arial;
            font-size: 10pt;
            border: 5px solid #67CFF5;
            width: 200px;
            height: 100px;
            display: none;
            position: fixed;
            background-color: White;
            z-index: 999;
        }
        
        .spaced label
        {
            margin-right: 30px; /* Or any other value */
        }
    </style>
    <script src="/_layouts/static/scripts/jquery-1.11.0.min.js" type="text/javascript"></script>
    <script src="/_layouts/static/scripts/AddAttachmentDailogue.js" type="text/javascript"></script>
    <script src="/_layouts/static/scripts/ModalHost.js" type="text/javascript"></script>
    <script src="/_layouts/15/PWC.Process.SixSigma/js/Modal.js" type="text/javascript"></script>
    <script type="text/javascript">



        function FreezeScreen() {
            var msg;
            var msgloading;
            if (culture == "fr") {
                msg = "S\\'il vous plaît patienter pendant le chargement de contenu ..";
                msgloading = "Chargement...";
            }
            else if (culture == "pl") {
                msg = "Poczekaj obciążeń treści ..";
                msgloading = "Ładowanie...";
            }
            else {
                msg = "Please wait while the content loads..";
                msgloading = "Loading...";
            }
            var temp = '<span>' + msg + '</span>';
            window.parent.eval("window.waitDialog = SP.UI.ModalDialog.showWaitScreenWithNoClose('" + msgloading + "','" + temp + "',65,350);");
            var a = parent.document;
            $('.ms-dlgTitleBtns', a).attr('style', 'display: none');
            return true;
        }

        function unFreeze() {
            if (window.parent.waitDialog != null) {
                var a = parent.document;
                $('.ms-dlgTitleBtns', a).removeAttr('style');
                window.parent.waitDialog.close();
            }
            return true;
        }


        $(document).ready(function () {


            var div = document.getElementById("<%=treeViewUpload.ClientID %>")

            var divTreeView = $("#" + div.id);

            divTreeView.find('a:eq(2)').removeAttr('style');

            divTreeView.find('a:eq(2)').css({ "color": "#2F557B",
                "background-color": "#FFFFC4",
                "border": "solid 1px #2F557B",
                "cursor": "default"
            });


            document.getElementById("<%=txtFolderUrl.ClientID %>").value = document.getElementById("<%=hdnRootURL.ClientID %>").value;

        });

        function createList(array) {
            var html = '<ul>';
            $.each(array, function (k, item) {
                html += '<li>' + item.name;
                if (item.subdir) {
                    html += createList(item.subdir);
                }
                html += '</li>';
            });
            html += '</ul>';
            return html;
        }


        function setfields() {

            document.getElementById("<%=hdnNext.ClientID%>").value = "yes";
            if (document.getElementById("<%=trUpload.ClientID%>").style.display == "") {

                // $("#<%=trheight.ClientID%>").removeAttr('height');
                // $("#<%=trheight.ClientID%>").css('height', 150);
                document.getElementById("<%=trheight.ClientID %>").style.display = "none";
                document.getElementById("<%=trFolderLocation.ClientID%>").style.display = "none";
                document.getElementById("<%=fldSelectSourceFile.ClientID %>").style.display = "none";
                document.getElementById("<%=btnBack.ClientID%>").style.display = "none";
                document.getElementById("<%=trUpload.ClientID%>").style.display = "none";
                document.getElementById("<%=trBrowseOnUpload.ClientID%>").style.display = "";
                document.getElementById("<%=trNext.ClientID%>").style.display = "";
                document.getElementById("<%=trOverWrite.ClientID%>").style.display = "none";
                $("#<%=btnNext.ClientID%>").val('Next');
                document.getElementById("<%=trNotes.ClientID%>").style.display = "none";

                return false;
            }
            else {
                document.getElementById("<%=fldSelectSourceFile.ClientID %>").style.display = "";
                document.getElementById("<%=trOverWrite.ClientID%>").style.display = "none";
                $("#<%=trheight.ClientID%>").removeAttr('height');
                $("#<%=trheight.ClientID%>").css('height', 20);
                document.getElementById("<%=trheight.ClientID %>").style.display = "";
                var IsBrowseUpload = document.getElementById("<%=hdnIsBrowseUpload.ClientID%>").value;

                var siteUrl = document.getElementById("<%=hdnBaseUrl.ClientID%>").value;

                var folders = document.getElementById("<%=txtFolderUrl.ClientID %>").value.replace(siteUrl, "").split('/');
                var html = '<ul id="ulbreadcrum">';
                for (var i = 0; i < folders.length; i++) {
                    if (i == folders.length - 1) {
                        html += '<li class="acnchorLast" style="font-weight:bold">' + folders[i] + '</li>';
                    }
                    else {
                        html += '<li>' + folders[i] + '</li>';
                    }
                }
                html += '</ul>';
                $("#breadCrumbNew").html(html);
                document.getElementById("<%=trFolderLocation.ClientID%>").style.display = "none";

                document.getElementById("<%=btnBack.ClientID%>").style.display = "";
                document.getElementById("<%=trUpload.ClientID%>").style.display = "";
                document.getElementById("<%=trBrowseOnUpload.ClientID%>").style.display = "none";
                document.getElementById("<%=trNext.ClientID%>").style.display = "none";
                document.getElementById("<%=trNotes.ClientID%>").style.display = "none";
                return false;
            }

        }


        function checkfilesize() {

            var size = document.getElementById("<%=FileUpoad.ClientID%>").files[0].size;

            if (size == 0) {

                alert("Please select a valid file.file size should not be zero.")
                return false;

            }

            else {

                return true;
            }

        }

        function checkfolders() {

            $check = true;

            var file = $("#<%=FileUpoad.ClientID%>").val();  //Fetch the filename of the submitted file

            if (file == '') {    //Check if a file was selected
                //Place warning text below the upload control
                if (culture == "fr") {
                    alert("S'il vous plaît choisir un fichier que vous souhaitez télécharger.");
                    $check = false;
                    //unFreeze();
                    return false;
                }
                else if (culture == "pl") {
                    alert("Proszę wybrać plik, który ma być zaimportowany.");
                    $check = false;
                    //unFreeze();
                    return false;
                }
                else {
                    alert("Please choose a file which you want to upload.");
                    $check = false;
                    //unFreeze();
                    return false;
                }

            }
            FreezeScreen();
            return $check;

        }




        // Look for a change every time the page is loaded.

        function clickNode(sender, url, docName) {
            var urltofind = false;
            var div = document.getElementById("<%=TreeViewDoc.ClientID %>")

            var divTreeView = $("#" + div.id);

            divTreeView.find('a').each(function () {

                // $(this).removeAttr('style');
            });

            divTreeView.find('a').each(function () {

                if ($(this).text() == docName) {
                    if ($(this).attr('style') == undefined) {

                        $(this).css({ "color": "#2F557B",
                            "background-color": "#FFFFC4",
                            "border": "solid 1px #2F557B",
                            "cursor": "default"
                        });
                    }
                    else {
                        $(this).removeAttr('style');
                        urltofind = true;
                    }
                }
                else {
                    // $(this).removeAttr('style');

                }
            });



            if (urltofind) {
                document.getElementById("<%=Dialogvalue.ClientID %>").value = document.getElementById("<%=Dialogvalue.ClientID %>").value.replace(url + '##', '');
            }
            else {

                document.getElementById("<%=Dialogvalue.ClientID %>").value += url + "##";
            }


            //do other stuff here       
        }

        function clickNodeFolder(sender, url, docName) {

            var div = document.getElementById("<%=treeViewUpload.ClientID %>")

            var divTreeView = $("#" + div.id);

            divTreeView.find('a').each(function () {

                $(this).removeAttr('style');
            });

            divTreeView.find('a').each(function () {

                if ($(this).text() == docName) {

                    $(this).css({ "color": "#2F557B",
                        "background-color": "#FFFFC4",
                        "border": "solid 1px #2F557B",
                        "cursor": "default"
                    });
                }
                else {
                    $(this).removeAttr('style');

                }
            });





            document.getElementById("<%=txtFolderUrl.ClientID %>").value = url;


            //do other stuff here       
        }


        function checklink() {

            if (document.getElementById("<%=Dialogvalue.ClientID %>").value.trim() == "") {
                if (culture == "fr") {
                    alert("S'il vous plaît sélectionner le fichier à joindre");
                    return false;
                }
                else if (culture == "pl") {
                    alert("Proszę wybrać plik do załączenia");
                    return false;
                }
                else {
                    alert("Please select the file to attach");
                    return false;
                }
            }
            FreezeScreen();
            return true;

        }


        //        function FreezeScreen() {

        //            var waitScreen = SP.UI.ModalDialog.showWaitScreenWithNoClose("Loading", "Please wait");
        //            var options = SP.UI.$create_DialogOptions();
        //            options.url = targetUrl;
        //            options.autoSize = true;
        //            options.allowMaximize = false;
        //            options.allowClose = true;
        //            options.args = waitScreen; //pass the reference of the wait screen to the dialog

        //            SP.UI.ModalDialog.showModalDialog(options);        

        //           var temp = '<span style="font-size: 8pt">Please wait while the content loads.</span>';
        //           window.parent.eval("window.waitDialog = SP.UI.ModalDialog.showWaitScreenWithNoClose('Loading...','" + temp + "',60,320,'','',false);");
        //            return true;
        //        }

        //        function unFreeze() {
        //            if (window.parent.waitDialog != null) {
        //                window.parent.waitDialog.close();
        //            }
        //            return true;
        //        }


        function changetype(rbtlist) {


            for (var i = 0; i < rbtlist.rows.length; ++i) {

                if (rbtlist.rows[i].cells[0].firstChild.checked) {

                    FreezeScreen();
                    document.getElementById("<%=trheight.ClientID %>").style.display = "none";
                    document.getElementById("<%=browseAttachment.ClientID %>").style.display = "";
                    document.getElementById("<%=uploadAttachment.ClientID %>").style.display = "none";
                    document.getElementById("<%=trFolderLocation.ClientID %>").style.display = "none";

                }
                else if (rbtlist.rows[i].cells[1].firstChild.checked) {
                    FreezeScreen();

                    $("#<%=trheight.ClientID%>").removeAttr('height');
                    $("#<%=trheight.ClientID%>").css('height', 20);
                    document.getElementById("<%=trheight.ClientID %>").style.display = "";
                    document.getElementById("<%=trheight.ClientID %>").style.display = "";
                    document.getElementById("<%=uploadAttachment.ClientID %>").style.display = "";
                    document.getElementById("<%=fldSelectSourceFile.ClientID %>").style.display = "";

                    document.getElementById("<%=browseAttachment.ClientID %>").style.display = "none";
                    document.getElementById("<%=trFolderLocation.ClientID %>").style.display = "none";
                    document.getElementById("<%=trOverWrite.ClientID%>").style.display = "none";
                    document.getElementById("<%=trNotes.ClientID%>").style.display = "none";

                    var IsBrowseUpload = document.getElementById("<%=hdnIsBrowseUpload.ClientID%>").value;
                    if (IsBrowseUpload == "true") {

                        document.getElementById("<%=fldSelectSourceFile.ClientID %>").style.display = "none";
                        document.getElementById("<%=trNotes.ClientID%>").style.display = "none";
                        document.getElementById("<%=trOverWrite.ClientID%>").style.display = "none";
                        document.getElementById("<%=trheight.ClientID %>").style.display = "none";
                        document.getElementById("<%=trUpload.ClientID %>").style.display = "none";
                        document.getElementById("<%=trBrowseOnUpload.ClientID %>").style.display = "";
                        document.getElementById("<%=trNext.ClientID %>").style.display = "";
                    }

                }
                else {

                }
            }
        }

        function ShowProgress() {

            setTimeout(function () {

                var modal = $('<div />');

                modal.addClass("modal");

                $(document).append(modal);

                var loading = $(".loading");

                loading.show();

                var top = Math.max($(window).height() / 2 - loading[0].offsetHeight / 2, 0);

                var left = Math.max($(window).width() / 2 - loading[0].offsetWidth / 2, 0);

                loading.css({ top: top, left: left });

            }, 200);

        }

        function hideloading() {

            var loading = $(".loading");

            loading.hide();
        }


    </script>
</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <%--<asp:UpdatePanel ID="updatePanel" runat="server">
        <ContentTemplate>--%>
    <asp:TextBox runat="server" ID="Dialogvalue" CssClass="modalhiddenfield" onchange="checkTextChange();"
        Style="display: none; visibility: hidden;">
    </asp:TextBox>
    <asp:TextBox runat="server" ID="txtFolderUrl" Style="display: none; visibility: hidden;">
    </asp:TextBox>
    <asp:Panel CssClass="pnlmargin" Height="380px" Width="625px" ID="pnlConfirm" runat="server">
        <table width="100%" style="border-left-style: Solid; border-left-color: black; border-right-style: Solid;
            border-right-color: black; border-left-width: 1px; border-right-width: 1px" cellpadding="6px"
            cellspacing="0px" bgcolor="#FFFFFF">
            <tr class="New" height="15">
                <td class="labelHeader" colspan="2">
                    Please select option to attach file.
                </td>
            </tr>
            <tr height="10" bgcolor="#000000">
                <td class="labelHeader" colspan="2">
                </td>
            </tr>
            <tr height="10">
                <td class="labelHeader" colspan="2">
                </td>
            </tr>
            <tr width="100%">
                <td class="labelColumns" colspan="2">
                    <asp:Label ID="lblerror" runat="server" Text="" ForeColor="Red"></asp:Label>
                </td>
            </tr>
            <tr runat="server" width="100%" id="trReportType" style="display: none" height="15">
                <td width="25%" style="font-family: Verdana, Arial, sans-serif; font-size: 10pt;
                    color: #676767; font-weight: bold;">
                    Attach File:
                </td>
                <td width="80%" style="font-family: Verdana, Arial, sans-serif; font-size: 10pt;
                    color: #676767; padding-left:60px;">
                    <asp:RadioButtonList ID="ddlTypeOfReport" runat="server" onclick="return changetype(this);"
                        RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="ddlTypeOfReport_Change" CssClass="spaced">
                        <asp:ListItem Text="From this Site" Value="1"></asp:ListItem>
                        <asp:ListItem Text="From Local System" Value="2"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr id="browseAttachment" style="display: none" runat="server" width="100%">
                <td colspan="2">
                    <fieldset>
                        <legend style="font-family: Verdana, Arial, sans-serif; font-size: 10pt; color: #676767">
                            Select file to attach</legend>
                        <div style="display: block; overflow: auto; height: 175px;">
                            <table width="100%" height="175px;">
                                <tr id="Tr1" runat="server" valign="top">
                                    <td colspan="2">
                                        <asp:TreeView ID="TreeViewDoc" runat="server" SelectedNodeStyle-ForeColor="Yellow"
                                            SelectedNodeStyle-BackColor="Black">
                                        </asp:TreeView>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <table width="100%">
                            <tr id="Tr2" runat="server" valign="bottom">
                                <td colspan="2" align="right">
                                    <asp:Button ID="btnsave" runat="server" Text="Attach" OnClick="ModalOk_Click" OnClientClick="return checklink();" />
                                    <asp:Label ID="lblmsg" runat="server" Style="color: Maroon"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </td>
            </tr>
            <tr id="uploadAttachment" style="display: none" runat="server" width="100%">
                <td colspan="2">
                    <table width="100%">
                        <tr id="fldSelectSourceFile" runat="server" style="display: none">
                            <td>
                                <fieldset>
                                    <legend style="font-family: Verdana, Arial, sans-serif; font-size: 10pt; color: #676767"
                                        id="fromLocalSystem">Select file to attach</legend>
                                    <table width="100%" style="margin-top: 15px;">
                                        <tr id="trFolderLocation" style="display: none" runat="server" valign="top" height="40">
                                            <td style="width: 100%" colspan="2" align="left" valign="top">
                                                <div id="breadCrumbNew" style="width: 550px; margin-left: -10px;" align="left" valign="top">
                                                </div>
                                            </td>
                                        </tr>
                                        <tr valign="top" id="trUpload" runat="server" height="15px">
                                            <td style="width: 100%" colspan="2">
                                                <asp:FileUpload ID="FileUpoad" runat="server" CssClass="ms-fileinput" size="30" Height="24px"
                                                    Style="vertical-align: top" />
                                            </td>
                                        </tr>
                                        <tr id="trOverWrite" style="display: none" runat="server">
                                            <td colspan="2" align="left">
                                                <asp:CheckBox ID="chkOverWrite" runat="server" Checked="true" />
                                                <asp:Label ID="lblOverWrite" Style="font-family: Verdana, Arial, sans-serif; font-size: 10pt;
                                                    color: #676767" Text="Add as a new version to existing files" runat="server" />
                                            </td>
                                        </tr>
                                        <tr height="10px">
                                        </tr>
                                        <tr id="trNotes" runat="server" style="display: none">
                                            <td colspan="2" align="left" style="font-family: Verdana,Arial; font-size: 11px;
                                                color: #000000;">
                                                <span><b><u>Instructions:</u></b></span><br />
                                                1.Click on Browse button to select the file to upload from local system.<br />
                                                2.Then click on Upload button button to upload the file to SharePoint and attach
                                                it to item.<br />
                                                <br />
                                                Note:To go back to the previous screen click on Back.
                                            </td>
                                        </tr>
                                        <tr height="80">
                                            <td align="right" colspan="2">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right" colspan="2">
                                                <asp:Button Text="Attach" ID="lnkUpload" runat="server" OnClick="lnkUpload_Click"
                                                    Width="6em" OnClientClick="return checkfolders();" />
                                                <asp:Button Text="Back" ID="btnBack" runat="server" Width="6em" OnClientClick="return setfields();"
                                                    Style="display: none" />
                                            </td>
                                        </tr>
                                    </table>
                                </fieldset>
                            </td>
                        </tr>
                        <tr id="trBrowseOnUpload" style="display: none" runat="server" width="100%">
                            <td colspan="2">
                                <fieldset>
                                    <legend style="font-family: Verdana, Arial, sans-serif; font-size: 10pt; color: #676767">
                                        Select destination folder</legend>
                                    <div style="display: block; overflow: auto; height: 135px;">
                                        <table width="100%" height="200px;">
                                            <tr id="Tr4" runat="server" valign="top">
                                                <td colspan="2">
                                                    <asp:TreeView ID="treeViewUpload" runat="server" SelectedNodeStyle-ForeColor="Yellow"
                                                        SelectedNodeStyle-BackColor="Black">
                                                    </asp:TreeView>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </fieldset>
                            </td>
                        </tr>
                        <tr height="10px">
                        </tr>
                        <tr id="trNext" style="display: none" runat="server" width="100%" align="right" height="50">
                            <td align="left" style="font-family: Verdana,Arial; font-size: 11px; color: #000000;
                                display: none">
                                <span><b><u>Instructions:</u></b></span><br />
                                1.Please navigate to the folder location where you want to upload the document.<br />
                                2.Select the folder by clicking on it and then click on Next.
                            </td>
                            <td>
                                <asp:Button Text="Next" ID="btnNext" runat="server" OnClientClick="return setfields();" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr id="trheight" width="100%" height="260" runat="server">
            </tr>
            <tr width="100%" bgcolor="#000000">
                <td align="left" colspan="2">
                    <asp:Button ID="btnok" UseSubmitBehavior="false" runat="server" Text="Cancel" OnClientClick="javascript:return ModalCancel_click();" OnClick="btnok_click" />
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="hdnIsBrowseUpload" runat="server" />
        <asp:HiddenField ID="hdnNext" runat="server" />
        <asp:HiddenField ID="hdnRootURL" runat="server" />
        <asp:HiddenField ID="hdnBaseUrl" runat="server" />
    </asp:Panel>
    <%--  </ContentTemplate>
      <Triggers>
               <asp:PostBackTrigger ControlID="lnkUpload"  />
               </Triggers>
    </asp:UpdatePanel>--%>
    <div class="loading" align="center">
        Please wait while content loads...<br />
        <br />
        <img src="/_LAYOUTS/static/images/loader.gif" alt="" />
    </div>
</asp:Content>
<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Attach File
</asp:Content>
<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea"
    runat="server">
</asp:Content>