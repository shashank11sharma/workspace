using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Collections;
using System.IO;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.UI;
using System.Data;
using System.Globalization;
using System.Web;
using PWC.Process.SixSigma.wp_SixSigma;

namespace PWC.Process.SixSigma.Layouts.PWC.Process.SixSigma
{
    public partial class GetAttachment : LayoutsPageBase
    {
        string siteUrl = string.Empty;
        string siteDocName = string.Empty;
        string BrowseUrl = string.Empty;
        string BrowseDocName = string.Empty;
        string itemId = string.Empty;
        string currentsiteurl = string.Empty;
        string currentListName = string.Empty;
        bool IsBrowse = false;
        bool IsUpload = false;
        bool IsBrowseOnUpload = false;
        string folderNameOnUpload = string.Empty;
        string folderNameOnBrowse = string.Empty;
        string fileallreadyexist = string.Empty;
        string culture = string.Empty;
        string AttachedColumn = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            ULSLogger.LogErrorInULS("Start Page_Load: ");
            culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            //culture = "pl";
            if (!string.IsNullOrEmpty(Convert.ToString(Request.QueryString["AttachmentColumn"])))
            {
                AttachedColumn = Convert.ToString(Request.QueryString["AttachmentColumn"]);
            }
            if (!string.IsNullOrEmpty(Convert.ToString(Request.QueryString["ItemId"])))
            {
                itemId = Convert.ToString(Request.QueryString["ItemId"]);
            }
            TasksVariables();
            try
            {
                if (Request.Form["__EVENTTARGET"] != null)
                {
                    string CtrlID = Request.Form["__EVENTTARGET"];

                    if (ViewState["ItemUrl"] != null && (ArrayList)ViewState["ItemUrl"] != null && !CtrlID.Contains("btnok") && !FileUpoad.HasFile)
                    {
                        ArrayList item = (ArrayList)ViewState["ItemUrl"];
                        string COLUMN_NAME = AttachedColumn;
                        string urlSite = SPContext.Current.Web.Url;
                        GetConfigurations(AttachedColumn, itemId);
                        string itemUrl = string.Empty;
                        string itemName = string.Empty;
                        string itemUrlNew = string.Empty;
                        using (SPSite currentsite = new SPSite(siteUrl))
                        {

                            using (SPWeb DestWeb = currentsite.OpenWeb())
                            {
                                itemUrlNew = DestWeb.Url;
                                string listName = string.Empty;
                                if (siteDocName.Contains("/"))
                                {
                                    listName = siteDocName.Split('/')[0];
                                }
                                else
                                {
                                    listName = siteDocName;
                                }
                                SPList listdata = DestWeb.Lists[listName];
                                try
                                {
                                    SPListItem itemDoc = listdata.GetItemById(Convert.ToInt32(item[3]));
                                    SPFile file = itemDoc.File;
                                    file.CheckIn("");
                                }
                                catch (Exception ex)
                                {

                                }
                                itemUrl = listdata.GetItemById(Convert.ToInt32(item[3])).Url;
                                itemName = listdata.GetItemById(Convert.ToInt32(item[3])).Name;

                            }
                        }


                        using (SPSite currentsite = new SPSite(urlSite))
                        {

                            using (SPWeb DestWeb = currentsite.OpenWeb())
                            {
                                SPList listdata = DestWeb.Lists[new Guid(Request.QueryString["ListId"])];
                                //SPListItem listitemtoadd = listdata.GetItemById(Convert.ToInt16(Convert.ToString(Request.QueryString["ItemId"])));
                                int sigmaId = Convert.ToInt16(Convert.ToString(Request.QueryString["ItemId"]));
                                SPListItem listitemtoadd = listdata.GetItemById(sigmaId);
                                SPFieldMultiLineText multilineField = listitemtoadd.Fields.GetField(COLUMN_NAME) as SPFieldMultiLineText;
                                string Multitext = multilineField.GetFieldValueAsHtml(listitemtoadd[COLUMN_NAME], listitemtoadd);
                                string itemname = Convert.ToString(item[1]).Substring(Convert.ToString(item[1]).LastIndexOf('/') + 1, (Convert.ToString(item[1]).Length - Convert.ToString(item[1]).LastIndexOf('/')) - 1);
                                Multitext = Multitext.Replace(Convert.ToString(item[4]), itemUrlNew + "/" + itemUrl);
                                Multitext = Multitext.Replace("href=" + Convert.ToString(item[4]), "href=" + itemUrlNew + "/" + itemUrl);
                                Multitext = Multitext.Replace(itemname, itemName);
                                listitemtoadd[AttachedColumn] = Multitext;
                                listitemtoadd.Update();
                                Response.Clear();
                                Response.Write(String.Format(@"<script language=""javascript"" type=""text/javascript""> 
                    window.frameElement.commonModalDialogClose(1, ""{0}"");</script>", ""));
                                Response.End();
                            }
                        }


                    }
                }

            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("In Page_Load: " + ex.Message, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected);
            }




            if (!string.IsNullOrEmpty(Convert.ToString(Request.QueryString["Popup"])))
            {
                Response.Clear();
                Response.Write(String.Format(@"<script language=""javascript"" type=""text/javascript""> 
                    window.frameElement.commonModalDialogClose(1, ""{0}"");</script>", ""));
                Response.End();
            }
            if (!Page.IsPostBack)
            {

                try
                {

                    GetConfigurations(AttachedColumn, itemId);

                    //BindTreeView(false, TreeViewDoc, BrowseUrl, BrowseDocName);
                    //BindTreeView(true, treeViewUpload, siteUrl, siteDocName);

                    setControls();

                }
                catch (Exception ex)
                {
                    trReportType.Style["Display"] = "none";
                    lblerror.Text = "Attach File form Sharepoint/Local System is not enabled.Please contact your system administrator.";
                    ddlTypeOfReport.Items.FindByValue("2").Attributes["Style"] = "display:none";
                    ddlTypeOfReport.Items.FindByValue("1").Attributes["Style"] = "display:none";
                }
            }
        }
        private void TasksVariables()
        {
            string script = string.Format("var culture = '{0}';", culture);
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myScript", script, true);
        }

        private void setControls()
        {
            if (culture == "fr")
            {
                ddlTypeOfReport.Items[0].Text = "A partir de ce site";
                ddlTypeOfReport.Items[1].Text = "Du système local";
            }
            else if (culture == "pl")
            {
                ddlTypeOfReport.Items[0].Text = "Z tej witryny";
                ddlTypeOfReport.Items[1].Text = "Z Systemu lokalnego";
            }
            else
            {
                ddlTypeOfReport.Items[0].Text = "From this Site";
                ddlTypeOfReport.Items[1].Text = "From Local System";
            }
            // BindTreeView(IsBrowseOnUpload, treeViewUpload, siteUrl, siteDocName);
            if (IsBrowse && IsUpload)
            {

                if (IsBrowseOnUpload)
                {
                    trOverWrite.Style["Display"] = "none";
                    trUpload.Style["Display"] = "none";
                    trNotes.Style["Display"] = "none";
                    if (hdnNext.Value != "yes")
                    {
                        trBrowseOnUpload.Style["Display"] = "";
                    }

                }
                else
                {
                    trNotes.Style["Display"] = "none";
                    trOverWrite.Style["Display"] = "none";
                    trBrowseOnUpload.Style["Display"] = "none";
                }
            }
            else if (IsUpload)
            {

                ddlTypeOfReport.Items.FindByValue("1").Attributes["Style"] = "display:none";

                if (IsBrowseOnUpload)
                {
                    if (hdnNext.Value != "yes")
                    {
                        trBrowseOnUpload.Style["Display"] = "";
                    }

                }
                else
                {
                    trBrowseOnUpload.Style["Display"] = "none";
                }
            }
            else if (IsBrowse)
            {
                ddlTypeOfReport.Items.FindByValue("2").Attributes["Style"] = "display:none";

            }
            else
            {
                trReportType.Style["Display"] = "none";
                lblerror.Text = "Attach File form Sharepoint/Local System is not enabled.Please contact your system administrator.";
                ddlTypeOfReport.Items.FindByValue("2").Attributes["Style"] = "display:none";
                ddlTypeOfReport.Items.FindByValue("1").Attributes["Style"] = "display:none";
            }
        }

        private void GetConfigurations(string AttachedColumn, string ID)
        {
            string urlSite = SPContext.Current.Web.Url;
            using (SPSite currentsite = new SPSite(urlSite))
            {
                using (SPWeb currentWeb = currentsite.OpenWeb())
                {
                    currentsiteurl = currentsite.Url;
                    currentWeb.AllowUnsafeUpdates = true;
                    SPList listdata = currentWeb.Lists[new Guid(Request.QueryString["ListId"])];
                    currentListName = listdata.Title;
                    siteUrl = urlSite;
                    BrowseUrl = urlSite;

                    if (AttachedColumn == "Bacground Attachments")
                    {
                        folderNameOnUpload = "Project" + ID + "/Info/Background";
                        siteDocName = "Documents/Project" + ID + "/Info/Background";
                        folderNameOnBrowse = "Project" + ID + "/Info/Background"; ;
                        BrowseDocName = "Documents/Project" + ID + "/Info/Background";
                    }
                    else if (AttachedColumn == "Problem Attachments")
                    {
                        folderNameOnUpload = "Project" + ID + "/Info/Problem Statement";
                        siteDocName = "Documents/Project" + ID + "/Info/Problem Statement";
                        folderNameOnBrowse = "Project" + ID + "/Info/Problem Statement"; ;
                        BrowseDocName = "Documents/Project" + ID + "/Info/Problem Statement";
                    }
                    else if (AttachedColumn == "ProjectMetrics Attachments")
                    {
                        folderNameOnUpload = "Project" + ID + "/Info/Project Metrics";
                        siteDocName = "Documents/Project" + ID + "/Info/Project Metrics";
                        folderNameOnBrowse = "Project" + ID + "/Info/Project Metrics"; ;
                        BrowseDocName = "Documents/Project" + ID + "/Info/Project Metrics";
                    }
                    else if (AttachedColumn == "Benifits Attachments")
                    {
                        folderNameOnUpload = "Project" + ID + "/Info/Benefits";
                        siteDocName = "Documents/Project" + ID + "/Info/Benefits";
                        folderNameOnBrowse = "Project" + ID + "/Info/Benefits"; ;
                        BrowseDocName = "Documents/Project" + ID + "/Info/Benefits";
                    }
                    else if (AttachedColumn == "Costs Attachments")
                    {
                        folderNameOnUpload = "Project" + ID + "/Info/Costs";
                        siteDocName = "Documents/Project" + ID + "/Info/Costs";
                        folderNameOnBrowse = "Project" + ID + "/Info/Costs"; ;
                        BrowseDocName = "Documents/Project" + ID + "/Info/Costs";
                    }
                    else if (AttachedColumn == "Financial Attachments")
                    {
                        folderNameOnUpload = "Project" + ID + "/Info/Financial Attachments";
                        siteDocName = "Documents/Project" + ID + "/Info/Financial Attachments";
                        folderNameOnBrowse = "Project" + ID + "/Info/Financial Attachments";
                        BrowseDocName = "Documents/Project" + ID + "/Info/Financial Attachments";
                    }
                    else if (AttachedColumn == "Milestones Attachments")
                    {
                        folderNameOnUpload = "Project" + ID + "/Info/Milestones";
                        siteDocName = "Documents/Project" + ID + "/Info/Milestones";
                        folderNameOnBrowse = "Project" + ID + "/Info/Milestones"; ;
                        BrowseDocName = "Documents/Project" + ID + "/Info/Milestones";
                    }
                    else if (AttachedColumn == "Define Attachments")
                    {
                        folderNameOnUpload = "Project" + ID + "/Gates/Define";
                        siteDocName = "Documents/Project" + ID + "/Gates/Define";
                        folderNameOnBrowse = "Project" + ID + "/Gates/Define"; ;
                        BrowseDocName = "Documents/Project" + ID + "/Gates/Define";
                    }
                    else if (AttachedColumn == "Measure Attachments")
                    {
                        folderNameOnUpload = "Project" + ID + "/Gates/Measure";
                        siteDocName = "Documents/Project" + ID + "/Gates/Measure";
                        folderNameOnBrowse = "Project" + ID + "/Gates/Measure"; ;
                        BrowseDocName = "Documents/Project" + ID + "/Gates/Measure";
                    }
                    else if (AttachedColumn == "Analyze Attachments")
                    {
                        folderNameOnUpload = "Project" + ID + "/Gates/Analyze";
                        siteDocName = "Documents/Project" + ID + "/Gates/Analyze";
                        folderNameOnBrowse = "Project" + ID + "/Gates/Analyze"; ;
                        BrowseDocName = "Documents/Project" + ID + "/Gates/Analyze";
                    }
                    else if (AttachedColumn == "Investigate Attachments")
                    {
                        folderNameOnUpload = "Project" + ID + "/Gates/Improve";
                        siteDocName = "Documents/Project" + ID + "/Gates/Improve";
                        folderNameOnBrowse = "Project" + ID + "/Gates/Improve"; ;
                        BrowseDocName = "Documents/Project" + ID + "/Gates/Improve";
                    }
                    else if (AttachedColumn == "Control Attachments")
                    {
                        folderNameOnUpload = "Project" + ID + "/Gates/Control";
                        siteDocName = "Documents/Project" + ID + "/Gates/Control";
                        folderNameOnBrowse = "Project" + ID + "/Gates/Control"; ;
                        BrowseDocName = "Documents/Project" + ID + "/Gates/Control";
                    }
                    else if (AttachedColumn == "FinalReport Attachments")
                    {
                        folderNameOnUpload = "Project" + ID + "/Final Report";
                        siteDocName = "Documents/Project" + ID + "/Final Report";
                        folderNameOnBrowse = "Project" + ID + "/Final Report"; ;
                        BrowseDocName = "Documents/Project" + ID + "/Final Report";
                    }

                    IsBrowse = false;
                    IsUpload = true;
                    IsBrowseOnUpload = false;
                    hdnIsBrowseUpload.Value = "false";
                    if (string.IsNullOrEmpty(siteUrl) || string.IsNullOrEmpty(siteDocName) || string.IsNullOrEmpty(BrowseUrl) || string.IsNullOrEmpty(BrowseDocName))
                    {
                        lblerror.Text = "Source location to select attachment not set. Please contact your system administrator.";
                        trReportType.Style["Display"] = "none";
                        lblerror.Text = "Attach File form Sharepoint/Local System is not enabled.Please contact your system administrator.";
                        ddlTypeOfReport.Items.FindByValue("2").Attributes["Style"] = "display:none";
                        ddlTypeOfReport.Items.FindByValue("1").Attributes["Style"] = "display:none";

                    }
                    else
                    {
                        lblerror.Text = "";
                        trReportType.Style["Display"] = "";
                    }

                }
            }
        }

        private void BindTreeView(bool IsBrowseOnUpload, TreeView treeView, string browseUrl, string libName)
        {
            try
            {
                if (treeView.Nodes.Count == 0)
                {

                    string folderUrl = string.Empty;
                    SPDocumentLibrary doclibrary = null;
                    using (SPSite site = new SPSite(browseUrl))
                    {
                        using (SPWeb wb = site.OpenWeb())
                        {
                            string docLib = string.Empty;

                            if (libName.Contains("/"))
                            {
                                docLib = libName.Split('/')[0];
                                ULSLogger.LogErrorInULS("Inside BindTreeView line1 in site " + wb.Title + " and folder url is: " + folderUrl);
                                //folderUrl = getfolderUrl(libName, folderUrl, wb, docLib);
                                string url = wb.ServerRelativeUrl + "/" + libName;
                                folderUrl = wb.GetFolder(url).Url;
                                ULSLogger.LogErrorInULS("Inside BindTreeView line2 in site " + wb.Title + " and folder url is: " + folderUrl);
                            }
                            else
                            {
                                docLib = libName;

                            }
                            doclibrary = (SPDocumentLibrary)wb.Lists[docLib];

                            SPFolder root = null;
                            if (!string.IsNullOrEmpty(folderUrl))
                            {
                                root = wb.GetFolder(folderUrl);
                            }
                            else
                            {
                                root = doclibrary.RootFolder;
                            }
                            //SPListItem item = (SPListItem)root.Item;
                            //SPModerationInformation abc= item.ModerationInformation;
                            //abc.Status=SPModerationStatusType.Approved;
                            //item.Update();

                            string baseURL = wb.Url.ToString();

                            TreeNode node = new TreeNode();
                            node = Utility.GetFolderNode(treeView, IsBrowseOnUpload, node, root, baseURL);
                            if (!string.IsNullOrEmpty(folderUrl))
                            {
                                //node.Text = libName.Split('/')[1];
                                node.Text = root.Name;
                            }
                            else
                            {
                                node.Text = doclibrary.Title;
                            }
                            hdnRootURL.Value = baseURL + "/" + root.Url;
                            hdnBaseUrl.Value = baseURL + "/";
                            //node.NavigateUrl = doclib.DefaultViewUrl;
                            if (treeView.ID != "TreeViewDoc")
                            {
                                node.NavigateUrl = "javascript:clickNodeFolder(this,'" + baseURL + "/" + root.Url + "','" + node.Text + "')";

                            }
                            else
                            {
                                node.NavigateUrl = "javascript:void(0);";
                            }
                            //long size = Utility.GetFolderSize(root) / 1024;
                            //long numFiles = Utility.GetNumberOfFilesInFolder(root);
                            // node.ToolTip = "Size:" + size.ToString() + " KBs " + " Files:" + numFiles.ToString();
                            node.ImageUrl = baseURL + "/_layouts/images/folder.gif";
                            treeView.Nodes.Add(node);
                            treeView.ShowLines = true;
                            treeView.EnableViewState = false;
                            treeView.CollapseAll();

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error in Bind TreeView and the Error is: " + ex.Message);
            }
        }

        private static string getfolderUrl(string libName, string folderUrl, SPWeb wb, string docLib)
        {
            SPList list = wb.Lists[docLib];

            SPQuery query = new SPQuery();


            // string url = wb.ServerRelativeUrl +"/"+libName;
            //string  folderurl = wb.GetFolder(url).Url;

            query.ViewXml = "<View Scope=\"RecursiveAll\"/>";
            SPListItemCollection listitems = list.GetItems(query);
            DataTable dt = list.GetItems(query).GetDataTable();
            foreach (SPListItem item in listitems)
            {
                if (item.ContentType.Name.Contains(list.ContentTypes["Folder"].Name))
                {
                    if (item.Folder != null)
                    {
                        if (item.Folder.Url.Equals(libName))
                        {
                            folderUrl = item.Folder.Url;
                        }
                    }

                }

            }
            return folderUrl;
        }






        protected void ModalOk_Click(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;

            // ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "loadingshow", "FreezeScreen();", true);
            GetConfigurations(AttachedColumn, itemId);
            Attachdocument(AttachedColumn);
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "loadinghide1", "unFreeze();", true);

            context.Response.Write("<script type='text/javascript'>window.frameElement.commitPopup()</script>");
            context.Response.Flush();
            context.Response.End();

            /// Page.Response.Clear(); Page.Response.Write(String.Format(CultureInfo.InvariantCulture, "<script type=\"text/javascript\">window.frameElement.commonModalDialogClose({0}, {1});</script>")); Page.Response.End();
            // Response.Clear();
            //Response.Write(String.Format(@"<script language=""javascript"" type=""text/javascript""> 
            //          window.frameElement.commonModalDialogClose(1, ""{0}"");</script>", ""));
            //Response.Flush();
            //Response.End();

        }

        private void Attachdocument(string AttachedColumn)
        {
            try
            {
                string getlink = string.Empty;
                string siteurl = string.Empty;
                string listName = string.Empty;
                getlink = Dialogvalue.Text;

                string COLUMN_NAME = AttachedColumn;
                string urlSite = SPContext.Current.Web.Url;
                string Multitext = string.Empty;
                using (SPSite currentsite = new SPSite(urlSite))
                {
                    using (SPWeb DestWeb = currentsite.OpenWeb())
                    {
                        SPList listdata = DestWeb.Lists[new Guid(Request.QueryString["ListId"])];
                        SPListItem listitemtoadd = listdata.GetItemById(Convert.ToInt16(Convert.ToString(Request.QueryString["ItemId"])));

                        SPFieldMultiLineText multilineField = listitemtoadd.Fields.GetField(COLUMN_NAME) as SPFieldMultiLineText;
                        Multitext = multilineField.GetFieldValueAsHtml(listitemtoadd[COLUMN_NAME], listitemtoadd);

                    }
                }

                if (!Multitext.Contains(getlink))
                {
                    AttachtoList(getlink, AttachedColumn);
                }



            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("In Attachdocument: " + ex.Message, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected);
            }
        }

        private void AttachtoList(string getlink, string AttachedColumn)
        {
            string[] split = { "##" };
            string urlsite = SPContext.Current.Web.Url;
            using (SPSite site = new SPSite(urlsite))
            {
                using (SPWeb DestWeb = site.OpenWeb())
                {
                    DestWeb.AllowUnsafeUpdates = true;

                    SPList listdata = DestWeb.Lists[new Guid(Request.QueryString["ListId"])];
                    SPListItem listitemtoadd = listdata.GetItemById(Convert.ToInt16(Convert.ToString(Request.QueryString["ItemId"])));
                    string savedlink = Convert.ToString(listitemtoadd[AttachedColumn]);

                    string COLUMN_NAME = AttachedColumn;
                    string sharePointNewLine = "<br/>";
                    SPFieldMultiLineText multilineField = listitemtoadd.Fields.GetField(COLUMN_NAME) as SPFieldMultiLineText;

                    if (multilineField != null)
                    {
                        // Get the field value as HTML
                        string text = multilineField.GetFieldValueAsHtml(listitemtoadd[COLUMN_NAME], listitemtoadd);
                        //text= text.Replace("</div>", "");
                        // Append the text
                        if (string.IsNullOrEmpty(text))
                        {

                            if (getlink.Split(split, StringSplitOptions.RemoveEmptyEntries).Length > 1)
                            {
                                foreach (string newlink in getlink.Split(split, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    if (!string.IsNullOrEmpty(newlink) && !text.Contains(newlink))
                                    {
                                        text = text + sharePointNewLine + "<a href='" + newlink + "'>" + newlink + "</a>";
                                    }
                                }
                            }
                            else
                            {
                                text = "<a href='" + getlink.Split(split, StringSplitOptions.RemoveEmptyEntries)[0] + "'>" + getlink.Split(split, StringSplitOptions.RemoveEmptyEntries)[0] + "</a>" + sharePointNewLine;
                            }
                        }
                        else
                        {
                            foreach (string newlink in getlink.Split(split, StringSplitOptions.RemoveEmptyEntries))
                            {
                                if (!string.IsNullOrEmpty(newlink) && !text.Contains(newlink))
                                {
                                    text = text + sharePointNewLine + "<a href='" + newlink + "'>" + newlink + "</a>";
                                }
                            }
                        }
                        // text = text + "</div>";
                        listitemtoadd[AttachedColumn] = text;
                    }

                    listitemtoadd.Update();

                    DestWeb.AllowUnsafeUpdates = false;
                    // BindTreeView(IsBrowseOnUpload, TreeViewDoc, BrowseUrl, BrowseDocName);
                    //TreeViewDoc.ExpandAll();
                }
            }
        }

        protected void btnok_click(object sender, EventArgs e)
        {
            Response.Clear();
            Response.Write(String.Format(@"<script language=""javascript"" type=""text/javascript""> 
                        window.frameElement.commonModalDialogClose(1, ""{0}"");</script>", ""));
            Response.End();
        }

        protected ArrayList Attachfile()
        {
            int taskid = Convert.ToInt32(Convert.ToString(Request.QueryString["TaskId"]));
            int meetingid = Convert.ToInt32(Convert.ToString(Request.QueryString["MeetingID"]));
            FileStream fileStream = null;
            string fileName = string.Empty;
            ArrayList arr = new ArrayList();
            string _filepath = string.Empty;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite oSite = new SPSite(siteUrl))
                {
                    using (SPWeb oWeb = oSite.OpenWeb())
                    {


                        fileName = System.IO.Path.GetFileName(FileUpoad.PostedFile.FileName);
                        string _fileTime = DateTime.Now.ToFileTime().ToString();
                        string _fileorgPath = System.IO.Path.GetFullPath(FileUpoad.PostedFile.FileName);
                        string _newfilePath = _fileTime + "~" + fileName;
                        double length = (FileUpoad.PostedFile.InputStream.Length) / 1024;

                        string tempFolder = Environment.GetEnvironmentVariable("TEMP");
                        _filepath = tempFolder + _newfilePath;

                        FileUpoad.PostedFile.SaveAs(_filepath);
                        string error = string.Empty;
                        String filetobeuploaded = System.IO.Path.GetFileName(_filepath);

                        try
                        {
                            fileStream = File.OpenRead(_filepath);
                        }
                        catch (Exception ex)
                        {
                            error = ex.StackTrace + ex.Message;
                            ULSLogger.LogErrorInULS(ex.Message, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected);
                        }
                    }
                }
            });
            using (SPSite oSite = new SPSite(siteUrl))
            {
                using (SPWeb oWeb = oSite.OpenWeb())
                {
                    string libName = siteDocName;
                    string folderUrl = string.Empty;
                    SPDocumentLibrary doclibrary = null;
                    SPUser user = oWeb.EnsureUser(SPContext.Current.Web.CurrentUser.LoginName);
                    string docLib = string.Empty;

                    if (libName.Contains("/"))
                    {
                        docLib = libName.Split('/')[0];

                        //folderUrl = getfolderUrl(libName, folderUrl, oWeb, docLib);
                        string url = oWeb.ServerRelativeUrl + "/" + libName;
                        folderUrl = oWeb.GetFolder(url).Url;
                    }
                    else
                    {
                        docLib = libName;

                    }
                    doclibrary = (SPDocumentLibrary)oWeb.Lists[docLib];

                    SPFolder root = null;
                    if (!string.IsNullOrEmpty(folderUrl))
                    {
                        root = oWeb.GetFolder(folderUrl);
                    }
                    else
                    {
                        root = doclibrary.RootFolder;
                    }

                    if (IsBrowseOnUpload && !string.IsNullOrEmpty(txtFolderUrl.Text))
                    {
                        root = oWeb.GetFolder(txtFolderUrl.Text);
                    }
                    else if (!string.IsNullOrEmpty(folderUrl))
                    {
                        root = oWeb.GetFolder(folderUrl);
                    }
                    else
                    {
                        root = doclibrary.RootFolder;
                    }


                    // Prepare to upload
                    Boolean replaceExistingFiles = true;

                    // Upload document
                    oWeb.AllowUnsafeUpdates = true;
                    SPFile spfile = null;


                    if (taskid != 0 || meetingid != 0)
                    {
                        SPFolder Folder = null;
                        string folderAbsoluteUrl = string.Empty;
                        if (IsBrowseOnUpload && !string.IsNullOrEmpty(txtFolderUrl.Text))
                        {
                            Folder = oWeb.GetFolder(txtFolderUrl.Text);
                        }
                        else if (taskid != 0)
                        {
                            folderAbsoluteUrl = doclibrary.RootFolder.ServerRelativeUrl + "/Task Attachments/TaskID" + taskid;
                            Folder = oWeb.GetFolder(folderAbsoluteUrl);
                        }
                        else
                        {
                            folderAbsoluteUrl = doclibrary.RootFolder.ServerRelativeUrl + "/Meeting Attachments/MeetingID" + meetingid;
                            Folder = oWeb.GetFolder(folderAbsoluteUrl);
                        }


                        if (Folder.Exists)
                        {
                            try
                            {
                                //fileStream = File.OpenRead(_filepath);
                                spfile = Folder.Files.Add(fileName, fileStream, false); //11 March - argument changed from true to false -> file could not be overwrite

                                spfile.Update();
                            }
                            catch (Exception ex)
                            {
                                ULSLogger.LogErrorInULS(ex.Message, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected);
                                fileallreadyexist = ex.Message; //11 March
                                return arr; //11 March
                            }
                        }



                    }

                    else
                    {



                        try
                        {

                            spfile = root.Files.Add(fileName, fileStream, false, "", true);
                        }
                        catch (Exception ex)
                        {
                            fileallreadyexist = ex.Message;
                            //Page.ClientScript.RegisterStartupScript(typeof(string), "Alert1", "<script type='text/javascript'>alert("+ex.Message+");</script>");

                            return arr;
                        }
                    }

                    int id = spfile.Item.ID;
                    SPListItem item = spfile.Item;

                    // Commit 
                    root.Update();
                    try
                    {
                        item["Author"] = user;
                        item["Editor"] = user;
                        item.UpdateOverwriteVersion();
                        //item.SystemUpdate(false);
                    }
                    catch (Exception ex)
                    {
                        ULSLogger.LogErrorInULS(ex.Message, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected);

                    }
                    oWeb.AllowUnsafeUpdates = false;

                    arr.Add(item.Web.Url);
                    arr.Add(item.File.Url);
                    arr.Add(item.ParentList.Forms[PAGETYPE.PAGE_EDITFORM].Url);
                    arr.Add(Convert.ToString(item.ID));



                }
            }


            return arr;

        }




        private bool checkFolderExixts(SPWeb web, string folderAbsoluteUrl)
        {
            try
            {
                return web.GetFolder(folderAbsoluteUrl).Exists;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        protected void lnkUpload_Click(object sender, EventArgs e)
        {
            ULSLogger.LogErrorInULS("Start lnkUpload_Click: ");
            try
            {

                GetConfigurations(AttachedColumn, itemId);

                if (FileUpoad.HasFile)
                {
                    if (FileUpoad.PostedFile.ContentLength == 0)
                    {

                        ClientScript.RegisterStartupScript(typeof(string), "Alert1", "<script type='text/javascript'>alert('Please select a valid file. file size should not be zero');</script>");
                        BindTreeView(false, TreeViewDoc, BrowseUrl, BrowseDocName);
                        BindTreeView(true, treeViewUpload, siteUrl, siteDocName);
                        //TreeViewDoc.ExpandAll();
                        uploadAttachment.Style["Display"] = "";
                        setControls();

                        trheight.Style["Display"] = "";
                        trheight.Style["height"] = "40px";
                        trOverWrite.Style["Display"] = "none";
                        trNotes.Style["Display"] = "none";
                        fldSelectSourceFile.Style["Display"] = "";
                        if (IsBrowseOnUpload)
                        {

                            trNext.Style["Display"] = "none";
                            trBrowseOnUpload.Style["Display"] = "none";
                            trUpload.Style["Display"] = "";
                            btnBack.Style["Display"] = "";
                            trFolderLocation.Style["Display"] = "none";

                        }

                        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "loadinghide10", "unFreeze();", true);
                    }

                    else
                    {

                        ArrayList itemdoc = Attachfile();
                        if (itemdoc != null && itemdoc.Count > 0)
                        {
                            string getlink = itemdoc[0] + "/" + itemdoc[1];

                            itemdoc.Add(getlink);
                            string COLUMN_NAME = "Bacground Attachments";
                            string urlSite = SPContext.Current.Web.Url;
                            string Multitext = string.Empty;
                            using (SPSite currentsite = new SPSite(urlSite))
                            {
                                using (SPWeb DestWeb = currentsite.OpenWeb())
                                {
                                    SPList listdata = DestWeb.Lists[new Guid(Request.QueryString["ListId"])];
                                    SPListItem listitemtoadd = listdata.GetItemById(Convert.ToInt16(Convert.ToString(Request.QueryString["ItemId"])));

                                    SPFieldMultiLineText multilineField = listitemtoadd.Fields.GetField(COLUMN_NAME) as SPFieldMultiLineText;
                                    Multitext = multilineField.GetFieldValueAsHtml(listitemtoadd[COLUMN_NAME], listitemtoadd);

                                }
                            }

                            if (!Multitext.Contains(getlink))
                            {
                                AttachtoList(getlink, AttachedColumn);
                            }
                            //string editurl = string.Format("{0}{1}?ID={2}&isDlg=1", itemdoc.Web.Url, "/" + itemdoc.ParentList.Forms[PAGETYPE.PAGE_EDITFORM].Url, itemdoc.ID);
                            string editurl = string.Format("{0}{1}?ID={2}&Source=" + SPContext.Current.Web.Url + "/_layouts/PWC.Process.SixSigma/GetAttachment.aspx?Popup=true&IsDlg=1", itemdoc[0], "/" + itemdoc[2], Convert.ToInt32(itemdoc[3]));
                            // string strScript = "<script type='text/javascript'>ShowNewPage('" + editurl + "')</script>";
                            // ClientScript.RegisterStartupScript(typeof(string), "Msg", strScript);
                            uploadAttachment.Style["Display"] = "";
                            ViewState["ItemUrl"] = itemdoc;
                            StringBuilder functionSyntax = new StringBuilder();
                            functionSyntax.AppendLine("function popupparams() {");
                            functionSyntax.AppendLine("var url ='" + editurl + "';");
                            functionSyntax.AppendLine("popupmodaluiNew(url);}");
                            functionSyntax.AppendLine("_spBodyOnLoadFunctionNames.push('popupparams');");
                            Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "ModalHostScript", functionSyntax.ToString(), true);
                            //BindTreeView(false, TreeViewDoc, BrowseUrl, BrowseDocName);

                            setControls();

                            trheight.Style["Display"] = "";
                            trheight.Style["height"] = "40px";
                            trOverWrite.Style["Display"] = "none";
                            trNotes.Style["Display"] = "none";
                            fldSelectSourceFile.Style["Display"] = "";
                            if (IsBrowseOnUpload)
                            {
                                trNext.Style["Display"] = "none";
                                trBrowseOnUpload.Style["Display"] = "none";
                                trUpload.Style["Display"] = "";
                                btnBack.Style["Display"] = "";
                                trFolderLocation.Style["Display"] = "none";

                            }
                            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "loadinghide2", "unFreeze();", true);

                            // Response.Redirect(editurl);


                        }

                        else
                        {
                            string errorMessage = string.Empty;
                            if (fileallreadyexist.Contains("invalid characters"))
                            {

                                if (culture == "fr")
                                {
                                    errorMessage = "Le nom de fichier est invalide. Un nom de fichier ne peut pas contenir les caractères suivants: \\ / []: . ! * ? \" < > | # $ & { } % ~";
                                }
                                else if (culture == "pl")
                                {
                                    errorMessage = "Nazwa pliku jest niepoprawna. Nazwa nie może zawierać znaków: \\ / []: . ! * ? \" < > | # $ & { } % ~";
                                }
                                else
                                {
                                    errorMessage = "The file name is invalid. A file name cannot contain any of the following characters: \\ / []: . ! * ? \" < > | # $ & { } % ~ ";
                                }
                            }
                            else if (fileallreadyexist.Contains("already exists"))
                            {
                                if (culture == "fr")
                                {
                                    errorMessage = "Un fichier avec ce nom existe déjà.";
                                }
                                else if (culture == "pl")
                                {
                                    errorMessage = "Plik o takiej nazwie już istnieje.";
                                }
                                else
                                {
                                    errorMessage = "A file with this name already exists.";
                                }
                            }

                            Page.ClientScript.RegisterStartupScript(typeof(string), "Alert1", "<script type='text/javascript'>alert('" + errorMessage + "');</script>");
                            BindTreeView(true, treeViewUpload, siteUrl, siteDocName);
                            BindTreeView(false, TreeViewDoc, BrowseUrl, BrowseDocName);
                            uploadAttachment.Style["Display"] = "";
                            setControls();
                            trOverWrite.Style["Display"] = "none";
                            trheight.Style["Display"] = "";
                            trheight.Style["height"] = "40px";
                            trNotes.Style["Display"] = "none";
                            fldSelectSourceFile.Style["Display"] = "";
                            if (IsBrowseOnUpload)
                            {
                                trNext.Style["Display"] = "none";
                                trBrowseOnUpload.Style["Display"] = "none";
                                trUpload.Style["Display"] = "";
                                btnBack.Style["Display"] = "";
                                trFolderLocation.Style["Display"] = "none";


                            }

                            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "loadinghide7", "unFreeze();", true);
                        }
                    }
                }
                else
                {
                    string errorMessage = string.Empty;

                    if (culture == "fr")
                    {
                        errorMessage = "S\\'il vous plaît choisir un fichier que vous souhaitez télécharger.";
                    }
                    else if (culture == "pl")
                    {
                        errorMessage = "Proszę wybrać plik, który ma być zaimportowany.";
                    }
                    else
                    {
                        errorMessage = "Please choose a file which you want to upload.";
                    }
                    ClientScript.RegisterStartupScript(typeof(string), "Alert", "<script type='text/javascript'>alert('" + errorMessage + "');</script>");
                    BindTreeView(true, treeViewUpload, BrowseUrl, BrowseDocName);
                    BindTreeView(false, TreeViewDoc, BrowseUrl, BrowseDocName);
                    uploadAttachment.Style["Display"] = "";
                    setControls();
                    trOverWrite.Style["Display"] = "none";
                    trheight.Style["Display"] = "";
                    trheight.Style["height"] = "40px";
                    trNotes.Style["Display"] = "none";
                    fldSelectSourceFile.Style["Display"] = "";
                    if (IsBrowseOnUpload)
                    {
                        trNext.Style["Display"] = "none";
                        trBrowseOnUpload.Style["Display"] = "none";
                        trUpload.Style["Display"] = "";
                        btnBack.Style["Display"] = "";
                        trFolderLocation.Style["Display"] = "none";


                    }

                    ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "loadinghide8", "unFreeze();", true);
                    //TreeViewDoc.ExpandAll();

                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error lnkUpload_Click: " + ex.Message, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected);



            }
        }

        protected void ddlTypeOfReport_Change(object sender, EventArgs e)
        {


            GetConfigurations(AttachedColumn, itemId);

            if (ddlTypeOfReport.SelectedValue == "1")
            {

                trheight.Style["Display"] = "none";
                browseAttachment.Style["Display"] = "";
                uploadAttachment.Style["Display"] = "none";
                trFolderLocation.Style["Display"] = "none";
                treeViewUpload.Nodes.Clear();
                BindTreeView(false, TreeViewDoc, BrowseUrl, BrowseDocName);
            }
            else
            {

                TreeViewDoc.Nodes.Clear();
                trheight.Style["height"] = "0";
                trheight.Style["height"] = "20";
                trheight.Style["Display"] = "";
                trheight.Style["Display"] = "none";
                uploadAttachment.Style["Display"] = "";
                fldSelectSourceFile.Style["Display"] = "";

                browseAttachment.Style["Display"] = "none";
                trFolderLocation.Style["Display"] = "none";
                trOverWrite.Style["Display"] = "none";
                trNotes.Style["Display"] = "none";

                var IsBrowseUpload = hdnIsBrowseUpload.Value;
                if (IsBrowseUpload == "true")
                {

                    fldSelectSourceFile.Style["Display"] = "none";
                    trNotes.Style["Display"] = "none";
                    trOverWrite.Style["Display"] = "none";
                    trheight.Style["Display"] = "none";
                    trUpload.Style["Display"] = "none";
                    trBrowseOnUpload.Style["Display"] = "";
                    trNext.Style["Display"] = "";
                }
                BindTreeView(true, treeViewUpload, siteUrl, siteDocName);

            }

            setControls();
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "loadinghide", "unFreeze();", true);
        }
    }
}