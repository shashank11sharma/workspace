using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Xml;
using System.Data;
using System.Web.UI.WebControls;
using System.Globalization;
using Microsoft.SharePoint.Administration;

namespace PWC.Process.SixSigma.Layouts.PWC.Process.SixSigma
{
    public partial class RemoveAttachment : LayoutsPageBase
    {
        string siteUrl = string.Empty;
        string siteDocName = string.Empty;
        string BrowseUrl = string.Empty;
        string BrowseDocName = string.Empty;
        bool isallowDeletionoffiles = false;
        string culture = string.Empty;
        string AttachedColumn = string.Empty;
        string itemId = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
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
                if (!Page.IsPostBack)
                {
                    string urlSite = SPContext.Current.Web.Url;
                    string COLUMN_NAME = AttachedColumn;
                    using (SPSite currentsite = new SPSite(urlSite))
                    {
                        using (SPWeb DestWeb = currentsite.OpenWeb())
                        {
                            SPList listdata = DestWeb.Lists[new Guid(Request.QueryString["ListId"])];
                            SPListItem listitemtoadd = listdata.GetItemById(Convert.ToInt16(Convert.ToString(Request.QueryString["ItemId"])));

                            SPFieldMultiLineText multilineField = listitemtoadd.Fields.GetField(COLUMN_NAME) as SPFieldMultiLineText;

                            if (multilineField != null)
                            {
                                // Get the field value as HTML
                                string text = multilineField.GetFieldValueAsHtml(listitemtoadd[COLUMN_NAME], listitemtoadd);
                                if (!string.IsNullOrEmpty(text))
                                {
                                    string sharePointNewLine = "<br/>";
                                    XmlDocument doc = new XmlDocument();
                                    //string finalHtml=divAttachments.InnerHtml.fi
                                    string attachmentHTML = "<div>" + text + "</div>";
                                    doc.LoadXml(attachmentHTML);
                                    XmlNodeList nodeList = doc.GetElementsByTagName("a");
                                    string newhtml = string.Empty;

                                    DataTable dtActionLog = new DataTable();
                                    DataColumn c1 = new DataColumn("link", typeof(string));
                                    dtActionLog.Columns.Add(c1);

                                    foreach (XmlNode node in nodeList)
                                    {

                                        DataRow drw = dtActionLog.NewRow();
                                        drw[0] = node.InnerXml;
                                        dtActionLog.Rows.Add(drw);
                                        dtActionLog.AcceptChanges();

                                    }
                                    grdview.DataSource = dtActionLog;
                                    grdview.DataBind();

                                }
                                else
                                {
                                    lblerror.Text = "No Attachment Found.";
                                    btnsave.Style["Display"] = "none";
                                    trerror.Style["Display"] = "";
                                    btnRemoveFile.Style["Display"] = "none";
                                }

                            }

                        }
                    }

                    GetConfigurations(AttachedColumn,itemId);

                    if (isallowDeletionoffiles)
                    {
                        btnRemoveFile.Visible = true;
                    }
                    else
                    {
                        btnRemoveFile.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void TasksVariables()
        {
            try
            {

                string script = string.Format("var culture = '{0}';", culture);
                //ClientScript.re
                ClientScript.RegisterStartupScript(Page.GetType(), "myScript", script, true);

            }

            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  TasksVariables: " + ex.Message, TraceSeverity.Unexpected);
            }



        }

        protected void grdview_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void ModalOk_Click(object sender, EventArgs e)
        {
            try
            {

                string urlSite = SPContext.Current.Web.Url;
                string COLUMN_NAME = AttachedColumn;
                using (SPSite currentsite = new SPSite(urlSite))
                {
                    using (SPWeb DestWeb = currentsite.OpenWeb())
                    {
                        SPList listdata = DestWeb.Lists[new Guid(Request.QueryString["ListId"])];
                        SPListItem listitemtoadd = listdata.GetItemById(Convert.ToInt16(Convert.ToString(Request.QueryString["ItemId"])));
                        string text = string.Empty;
                        string sharePointNewLine = "<br/>";
                        text = getAttachments(sharePointNewLine, text);
                        listitemtoadd[COLUMN_NAME] = text;
                        listitemtoadd.Update();
                        Response.Clear();
                        Response.Write(String.Format(@"<script language=""javascript"" type=""text/javascript""> 
                        window.frameElement.commonModalDialogClose(1, ""{0}"");</script>", ""));
                        Response.End();
                    }
                }
            }
            catch (Exception ex)
            {
            }

        }



        protected void RemoveFile_Click(object sender, EventArgs e)
        {
            try
            {


                GetConfigurations(AttachedColumn,itemId);
                using (SPSite site = new SPSite(siteUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {

                        if (grdview.Rows.Count > 0)
                        {
                            foreach (GridViewRow gvrow in grdview.Rows)
                            {
                                if (((CheckBox)gvrow.Cells[0].FindControl("chkselect")).Checked == true)
                                {
                                    string url = ((HyperLink)gvrow.Cells[1].FindControl("hyplink")).NavigateUrl;
                                    SPFile file = web.GetFile(url);
                                    file.Delete();
                                }
                            }
                        }


                    }

                }



                string urlSite = SPContext.Current.Web.Url;
                string COLUMN_NAME = AttachedColumn;
                using (SPSite currentsite = new SPSite(urlSite))
                {
                    using (SPWeb DestWeb = currentsite.OpenWeb())
                    {
                        SPList listdata = DestWeb.Lists[new Guid(Request.QueryString["ListId"])];
                        SPListItem listitemtoadd = listdata.GetItemById(Convert.ToInt16(Convert.ToString(Request.QueryString["ItemId"])));
                        string text = string.Empty;
                        string sharePointNewLine = "<br/>";
                        text = getAttachments(sharePointNewLine, text);
                        listitemtoadd[COLUMN_NAME] = text;
                        listitemtoadd.Update();

                    }
                }


                Response.Clear();
                Response.Write(String.Format(@"<script language=""javascript"" type=""text/javascript""> 
                window.frameElement.commonModalDialogClose(1, ""{0}"");</script>", ""));
                Response.End();

            }
            catch (Exception ex)
            {
            }

        }



        private void GetConfigurations(string AttachedColumn, string ID)
        {
            string urlSite = SPContext.Current.Web.Url;
            using (SPSite currentsite = new SPSite(urlSite))
            {
                using (SPWeb currentWeb = currentsite.OpenWeb())
                {
                    currentWeb.AllowUnsafeUpdates = true;
                    SPList listdata = currentWeb.Lists[new Guid(Request.QueryString["ListId"])];
                    siteUrl = urlSite;
                    BrowseUrl = urlSite;

                    if (AttachedColumn == "Bacground Attachments")
                    {
                        siteDocName = "Documents/Project" + ID + "/Info/Background";
                        BrowseDocName = "Documents/Project" + ID + "/Info/Background";
                    }
                    else if (AttachedColumn == "Problem Attachments")
                    {
                        siteDocName = "Documents/Project" + ID + "/Info/Problem Statement";
                        BrowseDocName = "Documents/Project" + ID + "/Info/Problem Statement";
                    }
                    else if (AttachedColumn == "ProjectMetrics Attachments")
                    {
                        siteDocName = "Documents/Project" + ID + "/Info/Project Metrics";
                        BrowseDocName = "Documents/Project" + ID + "/Info/Project Metrics";
                    }
                    else if (AttachedColumn == "Benifits Attachments")
                    {
                        siteDocName = "Documents/Project" + ID + "/Info/Benefits";
                        BrowseDocName = "Documents/Project" + ID + "/Info/Benefits";
                    }
                    else if (AttachedColumn == "Costs Attachments")
                    {
                        siteDocName = "Documents/Project" + ID + "/Info/Costs";
                        BrowseDocName = "Documents/Project" + ID + "/Info/Costs";
                    }
                    else if (AttachedColumn == "Financial Attachments")
                    {
                        siteDocName = "Documents/Project" + ID + "/Info/Financial Attachments";
                        BrowseDocName = "Documents/Project" + ID + "/Info/Financial Attachments";
                    }
                    else if (AttachedColumn == "Milestones Attachments")
                    {
                        siteDocName = "Documents/Project" + ID + "/Info/Milestones";
                        BrowseDocName = "Documents/Project" + ID + "/Info/Milestones";
                    }
                    else if (AttachedColumn == "Define Attachments")
                    {
                        siteDocName = "Documents/Project" + ID + "/Gates/Define";
                        BrowseDocName = "Documents/Project" + ID + "/Gates/Define";
                    }
                    else if (AttachedColumn == "Measure Attachments")
                    {
                        siteDocName = "Documents/Project" + ID + "/Gates/Measure";
                        BrowseDocName = "Documents/Project" + ID + "/Gates/Measure";
                    }
                    else if (AttachedColumn == "Analyze Attachments")
                    {
                        siteDocName = "Documents/Project" + ID + "/Gates/Analyze";
                        BrowseDocName = "Documents/Project" + ID + "/Gates/Analyze";
                    }
                    else if (AttachedColumn == "Investigate Attachments")
                    {
                        siteDocName = "Documents/Project" + ID + "/Gates/Improve";
                        BrowseDocName = "Documents/Project" + ID + "/Gates/Improve";
                    }
                    else if (AttachedColumn == "Control Attachments")
                    {
                        siteDocName = "Documents/Project" + ID + "/Gates/Control";
                        BrowseDocName = "Documents/Project" + ID + "/Gates/Control";
                    }
                    else if (AttachedColumn == "FinalReport Attachments")
                    {
                        siteDocName = "Documents/Project" + ID + "/Final Report";
                        BrowseDocName = "Documents/Project" + ID + "/Final Report";
                    }
                    isallowDeletionoffiles = true;
                }
            }
        }



        private string getAttachments(string sharePointNewLine, string text)
        {
            try
            {
                if (grdview.Rows.Count > 0)
                {
                    foreach (GridViewRow gvrow in grdview.Rows)
                    {
                        if (((CheckBox)gvrow.Cells[0].FindControl("chkselect")).Checked != true)
                        {
                            text = text + "<a href='" + ((HyperLink)gvrow.Cells[1].FindControl("hyplink")).NavigateUrl + "'>" + ((HyperLink)gvrow.Cells[1].FindControl("hyplink")).Text + "</a>" + sharePointNewLine;
                        }
                    }
                }
                text = text.Remove(text.LastIndexOf('<'), 5);

            }
            catch (Exception ex)
            {
                return text;
            }
            return text;
        }

        protected void btnok_click(object sender, EventArgs e)
        {
            Response.Clear();
            Response.Write(String.Format(@"<script language=""javascript"" type=""text/javascript""> 
                        window.frameElement.commonModalDialogClose(1, ""{0}"");</script>", ""));
            Response.End();
        }


    }
}
