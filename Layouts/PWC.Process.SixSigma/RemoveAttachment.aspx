<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RemoveAttachment.aspx.cs" Inherits="PWC.Process.SixSigma.Layouts.PWC.Process.SixSigma.RemoveAttachment" DynamicMasterPageFile="~masterurl/default.master" %>

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
        
         
    .myGridStyle
 {
      background-color: #EBE8ED;
      border-collapse:collapse;
      padding: 5px;
      color: Black;
      border: 1px solid black;
 }
 
  .myGridStyle td
        {
            border:1px solid White;
            padding: 3px;
			font-family: 'swis721 LtCn BT';
			font-size: 10pt;
        }
        
        .myGridStyle th
        {
            border:1px solid White;
            padding: 3px;
			font-family: 'swis721 LtCn BT';
			font-size: 14pt;
			color:white;
        }
        
    
    </style>
    <script src="/_layouts/static/scripts/jquery-1.11.0.min.js" type="text/javascript"></script>
    <script src="/_layouts/static/scripts/AddAttachmentDailogue.js" type="text/javascript"></script>
    <script src="/_layouts/15/PWC.Process.SixSigma/js/Modal.js" type="text/javascript"></script>
    <script type="text/javascript">
        function removelink(id, link) {

            $('a[href="' + link + '"]').remove();
            id.style.display = "none";
            document.getElementById('<%= hdnhtml.ClientID %>').value = $("[id*='divAttachments']").html();
            document.getElementById('<%= hdnremoveclicked.ClientID %>').value = "Yes";


        }

        function CheckAllEmp(Checkbox) {
            var GridVwHeaderChckbox = document.getElementById("<%=grdview.ClientID %>");
            for (i = 1; i < GridVwHeaderChckbox.rows.length; i++) {
                GridVwHeaderChckbox.rows[i].cells[0].getElementsByTagName("INPUT")[0].checked = Checkbox.checked;
            }
        }

        function Validate() {
            var select = false;
            var gridView = document.getElementById("<%=grdview.ClientID %>");
            var checkBoxes = gridView.getElementsByTagName("input");
            for (var i = 0; i < checkBoxes.length; i++) {
                if (checkBoxes[i].type == "checkbox" && checkBoxes[i].checked) {
                    select = true;
                    return true;
                }
            }

            if (!select) {
                if (culture == "fr") {
                    alert("S'il vous plaît sélectionner au moins un attachment à retirer.");
                }
                else if (culture == "pl") {
                    alert("Wybierz co najmniej jeden załącznik do usunięcia.");
                }
                else {
                    alert("Please select at least one attachment to remove.");
                }
                return false;
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <asp:Panel CssClass="pnlmargin" Height="350px" Width="620px" ID="pnlConfirm" runat="server">
        <table width="100%" style="border-left-style: Solid; border-left-color: black; border-right-style: Solid;
            border-right-color: black; border-left-width: 1px; border-right-width: 1px" cellpadding="6px"
            cellspacing="0px" bgcolor="#FFFFFF">
            <tr class="New" height="25">
                <td class="labelHeader" colspan="2">
                Please select attachment(s) to remove.
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
            <tr width="100%" id="trerror" runat="server" style="display:none">
                <td class="labelColumns" colspan="2">
                    <asp:Label ID="lblerror" runat="server" Text="" ForeColor="Red"></asp:Label>
                </td>
            </tr>
          
            <tr align="center" valign="top" >
                <td colspan="2" align="left" style="padding-left: 5px; padding-right: 5px; padding-top: 5px;
                    padding-bottom: 5px;">
             <div style="display: block; overflow: auto; height:260px;">
                    <asp:GridView ID="grdview" CssClass="myGridStyle" style="width:auto" runat="server" RowStyle-Wrap="false"
                        Font-Names='swis721 LtCn BT' Font-Size="14pt" HeaderStyle-BackColor="black"
                        AutoGenerateColumns="false" OnRowDataBound="grdview_RowDataBound">
                        <Columns>
                            <asp:TemplateField HeaderText="Select" ItemStyle-Width="3%" ItemStyle-VerticalAlign="Top" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Height="5px">
                                <HeaderTemplate >
                                    <asp:CheckBox ID="chkboxSelectAll" runat="server" onclick="CheckAllEmp(this);" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkselect" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Attachment(s)" ItemStyle-Width="97%" ItemStyle-VerticalAlign="Top" ItemStyle-Height="5px">
                                <ItemTemplate>
                                    <asp:HyperLink  ID="hyplink" NavigateUrl="<%# Bind('link') %>" runat="server" Text="<%# Bind('link') %>"></asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    </div>
                </td>
            </tr>
            <tr width="100%" bgcolor="#000000">
             <td colspan="2"  align="right">
                    <asp:Button ID="btnsave" runat="server" Text="Remove Link" OnClick="ModalOk_Click" OnClientClick="return Validate();" />
                       <asp:Button ID="btnRemoveFile" runat="server" Text="Remove Link and Delete Source File" OnClick="RemoveFile_Click" OnClientClick="return Validate();" Visible="false" />
                      <asp:Button ID="btnok" runat="server" Text="Cancel" OnClientClick="javascript:return ModalCancel_click();" OnClick="btnok_click" />
                </td>
               
            </tr>
            <asp:HiddenField ID="hdnhtml" runat="server" />
            <asp:HiddenField ID="hdnremoveclicked" runat="server" />
        </table>
    </asp:Panel>
</asp:Content>
<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Remove File
</asp:Content>
<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea"
    runat="server">
</asp:Content>