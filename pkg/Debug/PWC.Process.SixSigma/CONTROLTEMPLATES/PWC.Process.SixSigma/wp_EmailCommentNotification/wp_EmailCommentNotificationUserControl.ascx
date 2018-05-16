<%@ Assembly Name="PWC.Process.SixSigma, Version=1.0.0.0, Culture=neutral, PublicKeyToken=3cb004228fce7fc8" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="wp_EmailCommentNotificationUserControl.ascx.cs" Inherits="PWC.Process.SixSigma.wp_EmailCommentNotification.wp_EmailCommentNotificationUserControl" %>




<link rel="stylesheet" href="/_layouts/15/PWC.Process.SixSigma/css/jquery-ui.css" />
<link rel="stylesheet" href="/_layouts/15/PWC.Process.SixSigma/css/formSixSigma.css" />
<script src="/_layouts/15/PWC.Process.SixSigma/js/jquery-1.11.0.min.js" type="text/javascript"></script>
<script src="/_layouts/15/PWC.Process.SixSigma/js/jquery.SPServices-0.7.1a.js" type="text/javascript"></script>
<script src="/_layouts/15/PWC.Process.SixSigma/js/jquery-ui.js" type="text/javascript"></script>
<script src="/_layouts/15/PWC.Process.SixSigma/js/Modal.js" type="text/javascript"></script>

    <script type="text/javascript">
    $(document).ready(function () {
        $('#s4-ribboncont').hide();
        $('#s4-topheader2').hide();
        $('#s4-ribbonrow').hide();
        $('.header-secondrow').hide();
        $('.s4-title').hide();
        $('.s4-lp').hide();
        $('#ftr - lower').hide();

    });

    function ModalClose() {

        SP.UI.ModalDialog.commonModalDialogClose(SP.UI.DialogResult.cancel, 'Cancel clicked');
    }
    </script>

            <style type="text/css"> 
.myGridStyle
{
background-color: #EBE8ED;
border-collapse: collapse;
padding: 5px;
color: Black;
}
.myGridStyle td
{
padding: 5px;
border: 1px solid White;
font-family: 'swis721 LtCn BT';
font-size: 10pt;
color: black;
text-align: left;
} 
.myGridStyle th
{
padding: 5px;
border: 1px solid White;
font-family: 'swis721 LtCn BT';
font-size: 12pt;
color: black;
text-align: left;
background: #CECACA;
}
.New
{
background-color: #1e90ff;
color: #000000;
}
.labelHeader
{
text-align: center;
font-family: "Swis721 LtCn BT";
font-size: 14pt;
color: #FFFFFF;
} 
.errorCSS
{
font-family: "Swis721 LtCn BT";
font-size: 16px;
color: Red;
}
.dropdown
{
padding:3px;
}
</style>

    <center>
    <table width="800px" style="border-left-style: Solid; border-left-color: black; border-right-style: Solid;
        border-right-color: black; border-left-width: 1px; border-right-width: 1px" cellpadding="6px"
        cellspacing="0px" bgcolor="#FFFFFF">
        <tr class="New">
            <td align="left">
                <asp:label id="ExportDetails" runat="server" font-size="14pt" text="Comments"
                    font-names="Swis721 LtCn BT" forecolor="White"></asp:label>
            </td>
        </tr>
        <tr height="10" bgcolor="#000000">
            <td>
            </td>
        </tr>
        <tr height="10">
            <td>
                <div style="width: 100%;">
                    <div class="squarebox">

                    <asp:TextBox ID="txtEmailComment" TextMode="MultiLine" CssClass="ECTS_label"
                            Height="100px" Width="99%" MaxLength="4000" Rows="6" 
                          runat="server" Font-Names="Calibri" Font-Size="12pt"></asp:TextBox>
                    
                   
                      
                    </div>
                </div>
            </td>
        </tr>
       
        <tr  bgcolor="#000000">
            <td align="right" width="100%">

                <asp:Button ID="btnSixSigmaSave" runat="server" Text="Notify" Height="30px" Width="90px"
                                Font-Names="Calibri" Font-Size="12pt" OnClientClick=""
                                OnClick="btnSentEmailwithComment" /> 

                                <asp:Button ID="btnclose" runat="server" Text="Close" Height="30px" Width="90px"
                                Font-Names="Calibri" Font-Size="12pt" OnClientClick=""
                                OnClick="btnCloseEmail" />
                <%--<input type="button" id="close" value="Close" onclick="ModalClose()" />--%>
            </td>
        </tr>
    </table>

</center>
