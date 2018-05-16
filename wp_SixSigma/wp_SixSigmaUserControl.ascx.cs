using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Collections;
using Microsoft.SharePoint.Utilities;
using System.Text;
using Microsoft.Office.Server.UserProfiles;
using System.Collections.Generic;
using System.Xml;
using Microsoft.SharePoint.WebPartPages;
using System.Web;
using System.Net.Mail;
using System.Net.Mime;
using Microsoft.SharePoint.Administration;
using System.Globalization;
using System.IO;
using Microsoft.SharePoint.WebControls;
using System.Security.Cryptography;
using Microsoft.SharePoint.Taxonomy;
using System.Reflection;


namespace PWC.Process.SixSigma.wp_SixSigma
{
    public partial class wp_SixSigmaUserControl : UserControl
    {
        int sigmaId = 0; Guid AgendaListId; Guid strAgendaItemId;int TabValue=0;
        int LanguageId = 1;
        string CurrentSiteUrl = SPContext.Current.Site.Url;
        string SiteUrl = SPContext.Current.Site.Url;
        string AttachmentErrorMessage = string.Empty;
        string sixSigmaListName = "BreakThroughProcertProjectsTracking";
        string documentsListName = "Documents";
        bool UserInBBGroup = false;
        bool UserInGBGroup = false;
        bool UserIsSponser = false;
        bool ConcurrentSaving = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Setting Culture Value
            if (!string.IsNullOrEmpty(HiddenLanguage.Value))
            {
                LanguageId = Convert.ToInt32(HiddenLanguage.Value);              
            }

            // Getting Sigma Project Id from the Query String
            if (!string.IsNullOrEmpty(Convert.ToString(Request.QueryString["ProjectId"])))
            {
                sigmaId = Convert.ToInt32(Decrypt(Convert.ToString(Request.QueryString["ProjectId"])));
                if (!string.IsNullOrEmpty(Convert.ToString(Request.QueryString["SelectedTab"])))
                {
                    TabValue = Convert.ToInt32((Convert.ToString(Request.QueryString["SelectedTab"])));
                    SelectedTab.Value = Convert.ToString(TabValue);
                }
            }

            string CultureValue = string.Format("var culture = '{0}',sigmaID = '{1}';", LanguageId, (string.IsNullOrEmpty(Convert.ToString(ViewState["SigmaId"])) ? sigmaId : Convert.ToInt32(Convert.ToString(ViewState["SigmaId"]))));
            ScriptManager.RegisterStartupScript(updatePanel, updatePanel.GetType(), "CultureValue", CultureValue, true);
            // Hiding CSS using set on Load function
            ScriptManager.RegisterStartupScript(updatePanel, updatePanel.GetType(), "OnLoad", "setonLoad();", true);  
            
            // user is Member of
            UserInBBGroup = IsMemberOf("BlackBelt");
            UserInGBGroup = IsMemberOf("GreenBelt");

            setDateCss();
            setDate();
            
            // change the name of the document of the QuadCharts
            try
            {
                if (ViewState["ItemUrl"] != null && (ArrayList)ViewState["ItemUrl"] != null)
                {
                    ArrayList ArrayItem = (ArrayList)ViewState["ItemUrl"];
                    SPList list = SPContext.Current.Web.Lists[documentsListName];
                    SPListItem itemDoc = list.GetItemById(Convert.ToInt32(ArrayItem[3]));

                    foreach (GridViewRow row in GridQuadDetails.Rows)
                    {
                        HiddenField Id = (HiddenField)row.FindControl("hdnDocumentsID");
                        string IDValue = Id.Value;
                        if (itemDoc.ID == Convert.ToInt32(IDValue))
                        {
                            ((HyperLink)row.FindControl("lnkResolve1")).Text = itemDoc.Name;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error In Changing the Name in QuadCharts: " + ex.Message, TraceSeverity.Unexpected);
            }

            if (!IsPostBack) // First time opening the form
            {
                if (Request.UrlReferrer != null && !Request.UrlReferrer.ToString().Contains("BreakThroughProcertProjectsTracking.aspx"))
                    ViewState["SourceUrl"] = Request.UrlReferrer.ToString();
                if (sigmaId == 0)
                {
                    DisplayLinkAttachmentMessage("");
                    StringBuilder functionSyntax = new StringBuilder();
                    functionSyntax.AppendLine("function disableThreeTabs() {");
                    functionSyntax.AppendLine("disableThreeTabs();}");
                    functionSyntax.AppendLine("_spBodyOnLoadFunctionNames.push('disableThreeTabs');");
                    Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Disable", functionSyntax.ToString(), true);
                    BindSixSigmaDropDowns();

                    // Bind Project team Roles
                    BindProjectTeamRoles(SiteUrl, sigmaId);

                    BindSecondAttchmentGrid(SiteUrl, sigmaId);
                    // Bind Quad Charts
                    BindQuadCharts(SiteUrl, sigmaId);

                    StatusBasedControls("Draft");
                }
                else
                {
                    BindSixSigmaDropDowns();
                    // Getting Six Sigma Item by Query string ID
                    SPListItem lstItem = GeSixSigmaDataByID(sigmaId);

                    // setting View state for concurrent saving
                    ViewState["formOpenTime"] = Convert.ToDateTime(lstItem["Modified"]);
                    // Set buttons Control
                    if (lstItem != null)
                    {
                        string status = Convert.ToString(lstItem["ProjectStatus"]);
                        string ProjectSponser = Convert.ToString(lstItem["ProjectSponsor"]);
                        if (!string.IsNullOrEmpty(ProjectSponser))
                        {
                            if (SPContext.Current.Web.CurrentUser.LoginName == SPContext.Current.Web.SiteUsers.GetByID(Convert.ToInt32(ProjectSponser.Split('#')[0].Split(';')[0])).LoginName)
                            {
                                UserIsSponser = true;
                            }
                        }
                        StatusBasedControls(status);
                    }

                    // Binding Documents to All Grid
                    string[] colName = { "Bacground_x0020_Attachments", "Problem_x0020_Attachments", "ProjectMetrics_x0020_Attachments", "Benifits_x0020_Attachments", "Costs_x0020_Attachments", "Financial_x0020_Attachments", "Milestones_x0020_Attachments", "Define_x0020_Attachments", "Measure_x0020_Attachments", "Analyze_x0020_Attachments", "Investigate_x0020_Attachments", "Control_x0020_Attachments", "FinalReport_x0020_Attachments" };
                    for (int i = 0; i < colName.Length; i++)
                    {
                        BindDocuments(Convert.ToString(sigmaId), colName[i]);
                    }
                    // Setting Supporting attachments URLs
                    SetSupportingAttachmentsURL();

                    // Bind Project team Roles
                    BindProjectTeamRoles(SiteUrl, sigmaId);

                    BindSecondAttchmentGrid(SiteUrl, sigmaId);
                    // Bind Quad Charts
                    BindQuadCharts(SiteUrl, sigmaId);

                    // Bind Attachment Grid
                    BindAttachmentGrid(SiteUrl, sigmaId);

                    // Bind Discussion Grid
                    bindDiscussionGridTable(sigmaId);

                    // Bind Action Logs
                    BindActionLogs(lstItem);

                    #region Set Controls Labels
                    SetControlsLabel(lstItem);
                    #endregion
                }
            }
            else
            {
                // Binding Taxonomy Fields in the Six Sigma Form
                bindTagsCol(SiteUrl);
                if (sigmaId == 0)
                {
                    //BindSixSigmaDropDowns();
                    sigmaId = Convert.ToInt32(ViewState["SigmaId"]);
                }
                SPListItem lstItem = GeSixSigmaDataByID(sigmaId);
                string CtrlID = string.Empty;
                if (Request.Form["__EVENTTARGET"] != null &&
                    Request.Form["__EVENTTARGET"] != string.Empty)
                {
                    CtrlID = Request.Form["__EVENTTARGET"];
                    if (CtrlID.Contains("lnkBackground") || CtrlID.Contains("lnkprbStatement") || CtrlID.Contains("lnkprojectmetrics") || CtrlID.Contains("lnkBenefites") || CtrlID.Contains("lnkCost") || CtrlID.Contains("lnkMileStones"))
                    {
                        SelectedTab.Value = "0";
                    }
                    else if (CtrlID.Contains("lnktrashdelete") || CtrlID.Contains("QuadChartsEdit") || CtrlID.Contains("lbtnDeleteQuadFiles"))
                    {
                        SelectedTab.Value = "1";
                    }
                    else if (CtrlID.Contains("lnkDefine") || CtrlID.Contains("lnkMeasure") || CtrlID.Contains("lnkAnalyze") || CtrlID.Contains("lnkInvestigate") || CtrlID.Contains("lnkcontrol") || CtrlID.Contains("lnkfinalReport"))
                    {
                        SelectedTab.Value = "2";
                    }
                    if (CtrlID.Contains("lnkResolve1"))
                    {
                        if (ViewState["hashtable"] != null && ViewState["indexNumber"] != null)
                        {
                            Hashtable hashtable = (Hashtable)ViewState["hashtable"];
                            int indexNumber = (int)ViewState["indexNumber"];
                            if (hashtable.ContainsKey(indexNumber))
                            {
                                System.Web.UI.WebControls.Label lblsentemail = GridProjectTeam.Rows[indexNumber].FindControl("lblSentEmail") as System.Web.UI.WebControls.Label;
                                lblsentemail.Visible = true;
                            }
                        }
                    }
                    else if (CtrlID.Contains("lnkbtnupload"))
                    {
                        SetPeopleEditorControls(lstItem);
                    }
                    //else if (!(CtrlID.EndsWith("lnkResolve2") || CtrlID.EndsWith("lnkResolve")))
                    //{
                    //    SetPeopleEditorControls(lstItem);
                    //}

                    //BindActionLogs(lstItem);
                }

                BindActionLogs(lstItem);
                // Set buttons Control
                if (lstItem != null)
                {
                    string status = Convert.ToString(lstItem["ProjectStatus"]);
                    string ProjectSponser = Convert.ToString(lstItem["ProjectSponsor"]);
                    if (!string.IsNullOrEmpty(ProjectSponser))
                    {
                        if (SPContext.Current.Web.CurrentUser.LoginName == SPContext.Current.Web.SiteUsers.GetByID(Convert.ToInt32(ProjectSponser.Split('#')[0].Split(';')[0])).LoginName)
                        {
                            UserIsSponser = true;
                        }
                    }
                     
                    //if (!string.IsNullOrEmpty(ProjectSponser))
                    //{
                    //    projectSponserUserEditor.CommaSeparatedAccounts = Convert.ToString(SPContext.Current.Web.SiteUsers.GetByID(Convert.ToInt32(ProjectSponser.Split('#')[0].Split(';')[0])).LoginName);
                    //}
                    StatusBasedControls(status);
                  
                   
                    // Bind Quad Charts---to be removed.
                    if (!String.IsNullOrEmpty(hiddenFieldQuadCharts.Value))
                    {
                        BindQuadCharts(SiteUrl, sigmaId);
                        hiddenFieldQuadCharts.Value = "";
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(updatePanel, updatePanel.GetType(), "disableThreeTabs", "disableThreeTabs();", true);
                }

                // Binding Documents to All Grid
                string[] colName = { "Bacground_x0020_Attachments", "Problem_x0020_Attachments", "ProjectMetrics_x0020_Attachments", "Benifits_x0020_Attachments", "Costs_x0020_Attachments", "Milestones_x0020_Attachments", "Define_x0020_Attachments", "Measure_x0020_Attachments", "Analyze_x0020_Attachments", "Investigate_x0020_Attachments", "Control_x0020_Attachments", "FinalReport_x0020_Attachments" };
                for (int i = 0; i < colName.Length; i++)
                {
                    BindDocuments(Convert.ToString(sigmaId), colName[i]);
                }
                BindAttachmentGrid(SiteUrl, sigmaId);

                #region Set Controls Labels
                //SetControlsLabel(lstItem);
                #endregion
            }
            // Binding Taxonomy 
            // register the client script for taxonomy control initialization
            String key = "TaxonomyWebTaggingAjaxIncludeOnce";
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered(base.GetType(), key))
            {
                this.Page.ClientScript.RegisterClientScriptBlock(base.GetType(), key, GetReloadJavaScript(taxTags), true);
            }
        }



        private void bindTagsCol(string CurrentSiteURL)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {

                    using (SPSite site = new SPSite(CurrentSiteURL))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            try
                            {

                                // Binding Taxonomy Fields in the Six Sigma Form
                                SPList list = web.Lists["BreakThroughProcertProjectsTracking"];
                                if (list != null)
                                {
                                    TaxonomyField taxonomyField = list.Fields["Tags"] as TaxonomyField;
                                    if (taxonomyField != null)
                                    {
                                        taxTags.SspId.Add(taxonomyField.SspId);
                                        taxTags.TermSetId.Add(taxonomyField.TermSetId);
                                    }
                                }  
                               
                            }
                            catch (Exception ex)
                            {
                                ULSLogger.LogErrorInULS(ex.Message);
                            }

                        }
                    }
                });
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  BindAttachmentGrid: " + ex.Message, TraceSeverity.Unexpected);
            }

        }

        private string GetReloadJavaScript(TaxonomyWebTaggingControl taxonomyControl)
		{
			String script = String.Empty;
			String containerId = SPEncode.ScriptEncode(taxonomyControl.Controls[1].ClientID);
			Type type_TaxonomyWebTaggingControl = typeof(TaxonomyWebTaggingControl);
			MethodInfo mi_getOnloadJavascript = type_TaxonomyWebTaggingControl.GetMethod("getOnloadJavascript", BindingFlags.NonPublic | BindingFlags.Instance);
			String fullScript = (String)mi_getOnloadJavascript.Invoke(taxonomyControl, null);
			int pos = fullScript.IndexOf(String.Format("function {0}_load()", containerId));
			if (pos > -1)
			{
				StringBuilder builder = new StringBuilder();
				builder.Append("var myPrm = Sys.WebForms.PageRequestManager.getInstance();");
				builder.Append("myPrm.add_endRequest(EndRequest);");
				builder.Append("function EndRequest(sender, args)");
				builder.Append("{");

				builder.Append(fullScript.Substring(1, pos-1));
				builder.Append("Microsoft.SharePoint.Taxonomy.ScriptForWebTaggingUI.onLoad('");
				builder.Append(containerId);
				builder.Append("');");
				builder.Append("}}");
					
				script = builder.ToString();
			}
			return script;
		}

        private void setDate()
        {
            if (PlandateProjectAuthorization.IsDateEmpty)
                PlandateDefine.Enabled = false;
            else
                PlandateDefine.MinDate = PlandateProjectAuthorization.SelectedDate;
            if (PlandateDefine.IsDateEmpty)
                PlandateMeasure.Enabled = false;
            else
                PlandateMeasure.MinDate = PlandateDefine.SelectedDate;
            if (PlandateMeasure.IsDateEmpty)
                PlandateAnalyze.Enabled = false;
            else
                PlandateAnalyze.MinDate = PlandateMeasure.SelectedDate;
            if (PlandateAnalyze.IsDateEmpty)
                PlandateImprove.Enabled = false;
            else
                PlandateImprove.MinDate = PlandateAnalyze.SelectedDate;
            if (PlandateImprove.IsDateEmpty)
                PlandateControl.Enabled = false;
            else
                PlandateControl.MinDate = PlandateImprove.SelectedDate;
            if (PlandateControl.IsDateEmpty)
                PlandateFinalReportApprove.Enabled = false;
            else
                PlandateFinalReportApprove.MinDate = PlandateControl.SelectedDate;

        }

        private void DisplayLinkAttachmentMessage(string display)
        {
            AddLinkBackgroundMsg.Style["Display"] = display;
            AddLinkProblemStatementMsg.Style["Display"] = display;
            AddLinkProjectMetricsMsg.Style["Display"] = display;
            AddLinkBenfitsMsg.Style["Display"] = display;
            AddLinkCosstsMsg.Style["Display"] = display;
            AddLinkFinancialMsg.Style["Display"] = display;
            AddLinkMilestonesMsg.Style["Display"] = display;
            AddLinkDefinesMsg.Style["Display"] = display;
            AddLinkMeasureMsg.Style["Display"] = display;
            AddLinkAnalyzeMsg.Style["Display"] = display;
            AddLinkInvestigateMsg.Style["Display"] = display;
            AddLinkControlMsg.Style["Display"] = display;
            AddLinkFinalReportMsg.Style["Display"] = display;
        }

        private void setProjectInformationControls(bool readOnly)
        {
            // Project Identification
            txtProjectName.ReadOnly = readOnly;
            ddlorgnisation.Enabled = !readOnly;
            projectSponserUserEditor.Enabled = !readOnly;
            ddlplant.Enabled = !readOnly;
            BlackbeltuserEditor.Enabled =! readOnly;
            ddlprojecttype.Enabled = !readOnly;
            GreenbeltuserEditor.Enabled = !readOnly;
            taxTags.Disabled = readOnly;

            //Background
            txtBackground.ReadOnly = readOnly;

            //Project Statement and Objectives
            txtProjectstatementobj.ReadOnly = readOnly;

            //Project Metrics
            ddlMetricCost.Enabled = !readOnly;
            txtmetriccost.ReadOnly = readOnly;
            txtCostBaseline.ReadOnly = readOnly;
            txtCostGoal.ReadOnly = readOnly;

            ddlQualityMetrics.Enabled = !readOnly;
            txtmetricquality.ReadOnly = readOnly;
            txtQualityBaseline.ReadOnly = readOnly;
            txtQualityGoal.ReadOnly = readOnly;

            ddlDeliveryMetrics.Enabled = !readOnly;
            txtmetricdelivery.ReadOnly = readOnly;
            txtDeliveryBaseline.ReadOnly = readOnly;
            txtDeliveryGoal.ReadOnly = readOnly;

            ddlothermetric.Enabled = !readOnly;
            txtmetricother.ReadOnly = readOnly;
            txtotherbaseline.ReadOnly = readOnly;
            txtothergoal.ReadOnly = readOnly;

            ddlothermetric1.Enabled = !readOnly;
            txtmetricother1.ReadOnly = readOnly;
            txtotherbaseline1.ReadOnly = readOnly;
            txtothergoal1.ReadOnly = readOnly;

            //Planned Financial Analysis
            txtplannedActualCost.ReadOnly = readOnly;
            txtplannedActualBenefits.ReadOnly = readOnly;
            txtBenefits.ReadOnly = readOnly;


            //Milestones

        }

        private void StatusBasedControls(string status)
        {
            btnUnlockform.Visible = false;
            btnlockform.Visible = false;
            btnEditCompleted.Visible = false;
            btnSixSigmaSave.Visible = false;
            btnProjectAuthorization.Visible = false;
            btnSponsorApproval.Visible = false;
            btnBBApproval.Visible = false;
            btnReturnProjectLead.Visible = false;
            btnDefineRequestApproval.Visible = false;
            btnDefineBBApproval.Visible = false;
            btnDefineReturntoProjectlead.Visible = false;
            btnMeasureRequestApproval.Visible = false;
            btnMeasureBBApproval.Visible = false;
            btnMeasureReturntoProjectlead.Visible = false;
            btnAnalyzeRequestApproval.Visible = false;
            btnAnalyzeBBApproval.Visible = false;
            btnAnalyzeReturntoProjectlead.Visible = false;
            btnInvestigateRequestApproval.Visible = false;
            btnInvestigateBBApproval.Visible = false;
            btnInvestigateReturntoProjectlead.Visible = false;
            btnControlRequestApproval.Visible = false;
            btnControlBBApproval.Visible = false;
            btnControlReturntoProjectlead.Visible = false;
            btnFinalreportRequestApproval.Visible = false;
            btnFinalreportBBApproval.Visible = false;
            btnFinalreportReturntoProjectlead.Visible = false;

            lnkDefineAddDocuments.Enabled = false;
            lnkDefineRemoveDocuments.Enabled = false;
            lnkMeasureAddDocuments.Enabled = false;
            lnkMeasureRemobveDocuments.Enabled = false;
            lnkAnalyzeAddDocuments.Enabled = false;
            lnkAnalyzeRemoveDocuments.Enabled = false;
            lnkInvestigateAddDocuments.Enabled = false;
            lnkInvestigateRemoveDocuments.Enabled = false;
            lnkcontrolAddDocuments.Enabled = false;
            lnkcontrolRemoveDocuments.Enabled = false;
            lnkfinalReportAddDocuments.Enabled = false;
            lnkfinalReportRemoveDocuments.Enabled = false;

            // All text box for Gates tab read only
            txtdefineComment.ReadOnly = true;
            txtMeasurecomment.ReadOnly = true;
            txtAnalyzecomment.ReadOnly = true;
            txtinvestigatecomment.ReadOnly = true;
            txtControlcomment.ReadOnly = true;
            txtFinalreportcomment.ReadOnly = true;

            PlandateProjectAuthorization.Enabled = false;
            if (status != "Draft")
            {
                PlandateDefine.Enabled = false;
                PlandateMeasure.Enabled = false;
                PlandateAnalyze.Enabled = false;
                PlandateImprove.Enabled = false;
                PlandateControl.Enabled = false;
                PlandateFinalReportApprove.Enabled = false;
            }
            if (status != "Draft" && !UserInBBGroup)
            {
                setProjectInformationControls(true);
            }
            switch (status)
            {
                case "Draft":
                    ScriptManager.RegisterStartupScript(updatePanel, updatePanel.GetType(), "disableGates", "disableGates();", true);
                    ScriptManager.RegisterStartupScript(updatePanel, updatePanel.GetType(), "enableUpdatesAttachments", "enableUpdatesAttachments();", true);
                    btnSixSigmaSave.Visible = true;
                    PlandateProjectAuthorization.Enabled = true;
                    if (UserInGBGroup)
                    {
                        btnProjectAuthorization.Visible = true;
                    }
                    break;
                case "Awaiting Project Authorization by Project Sponsor":
                    ScriptManager.RegisterStartupScript(updatePanel, updatePanel.GetType(), "disableGates", "disableGates();", true);
                    if (UserIsSponser)
                    {
                        btnSponsorApproval.Visible = true;
                        btnReturnProjectLead.Visible = true;
                    }
                    if (UserInBBGroup)
                    {
                        btnUnlockform.Visible = true;
                    }
                    break;
                case "Awaiting Project Authorization by Black Belt":
                    ScriptManager.RegisterStartupScript(updatePanel, updatePanel.GetType(), "disableGates", "disableGates();", true);
                    if (UserInBBGroup)
                    {
                        btnBBApproval.Visible = true;
                        btnReturnProjectLead.Visible = true;
                        btnUnlockform.Visible = true;
                    }
                    break;
                case "Define":
                    if (UserInGBGroup)
                    {
                        btnDefineRequestApproval.Visible = true;
                        lnkDefineAddDocuments.Enabled = true;
                        lnkDefineRemoveDocuments.Enabled = true;
                        txtdefineComment.ReadOnly = false;
                    }
                    if (UserInBBGroup)
                    {
                        btnUnlockform.Visible = true;
                    }
                    btnSixSigmaSave.Visible = true;
                    DefineColorId.Style["background-color"] = "#FFFFCC";
                    break;
                case "Edit Exception":
                    PlandateProjectAuthorization.Enabled = true;
                    PlandateDefine.Enabled = true;
                    PlandateMeasure.Enabled = true;
                    PlandateAnalyze.Enabled = true;
                    PlandateImprove.Enabled = true;
                    PlandateControl.Enabled = true;
                    PlandateFinalReportApprove.Enabled = true;
                    if (UserInGBGroup)
                    {
                        btnSixSigmaSave.Visible = true;
                        btnEditCompleted.Visible = true;
                        setProjectInformationControls(false);
                    }
                    if (UserInBBGroup)
                    {
                        btnlockform.Visible = true;
                    }
                    break;
                case "Awaiting Define Gate Black Belt Approval":
                    if (UserInBBGroup)
                    {
                        btnDefineBBApproval.Visible = true;
                        btnDefineReturntoProjectlead.Visible = true;
                        txtdefineComment.ReadOnly = false;
                        btnUnlockform.Visible = true;
                    }
                    DefineColorId.Style["background-color"] = "#FFFFCC";
                    break;
                case "Measure":
                    if (UserInGBGroup)
                    {
                        btnMeasureRequestApproval.Visible = true;
                        //btnMeasureReturntoProjectlead.Visible = true;
                        lnkMeasureAddDocuments.Enabled = true;
                        lnkMeasureRemobveDocuments.Enabled = true;
                        txtMeasurecomment.ReadOnly = false;
                    }
                    if (UserInBBGroup)
                    {
                        btnUnlockform.Visible = true;
                    }
                    btnSixSigmaSave.Visible = true;
                    DefineColorId.Style["background-color"] = "#C3FDB8";
                    MeasureColorId.Style["background-color"] = "#FFFFCC";
                    break;
                case "Awaiting Measure Gate Black Belt Approval":
                    if (UserInBBGroup)
                    {
                        btnMeasureBBApproval.Visible = true;
                        btnMeasureReturntoProjectlead.Visible = true;
                        txtMeasurecomment.ReadOnly = false;
                        btnUnlockform.Visible = true;
                    }
                    DefineColorId.Style["background-color"] = "#C3FDB8";
                    MeasureColorId.Style["background-color"] = "#FFFFCC";
                    break;
                case "Analyze":
                    if (UserInGBGroup)
                    {
                        btnAnalyzeRequestApproval.Visible = true;
                        //btnAnalyzeReturntoProjectlead.Visible = true;
                        lnkAnalyzeAddDocuments.Enabled = true;
                        lnkAnalyzeRemoveDocuments.Enabled = true;
                        txtAnalyzecomment.ReadOnly = false;
                    }
                    if (UserInBBGroup)
                    {
                        btnUnlockform.Visible = true;
                    }
                    btnSixSigmaSave.Visible = true;
                    DefineColorId.Style["background-color"] = "#C3FDB8";
                    MeasureColorId.Style["background-color"] = "#C3FDB8";
                    AnalyzeColorId.Style["background-color"] = "#FFFFCC";
                    break;
                case "Awaiting Analyze Gate Black Belt Approval":
                    if (UserInBBGroup)
                    {
                        btnAnalyzeBBApproval.Visible = true;
                        btnAnalyzeReturntoProjectlead.Visible = true;
                        txtAnalyzecomment.ReadOnly = false;
                        btnUnlockform.Visible = true;
                    }
                    DefineColorId.Style["background-color"] = "#C3FDB8";
                    MeasureColorId.Style["background-color"] = "#C3FDB8";
                    AnalyzeColorId.Style["background-color"] = "#FFFFCC";
                    break;
                case "Improve":
                    if (UserInGBGroup)
                    {
                        btnInvestigateRequestApproval.Visible = true;
                        //btnInvestigateReturntoProjectlead.Visible = true;
                        lnkInvestigateAddDocuments.Enabled = true;
                        lnkInvestigateRemoveDocuments.Enabled = true;
                        txtinvestigatecomment.ReadOnly = false;
                    }
                    if (UserInBBGroup)
                    {
                        btnUnlockform.Visible = true;
                    }
                    btnSixSigmaSave.Visible = true;
                    DefineColorId.Style["background-color"] = "#C3FDB8";
                    MeasureColorId.Style["background-color"] = "#C3FDB8";
                    AnalyzeColorId.Style["background-color"] = "#C3FDB8";
                    InvestigateColorId.Style["background-color"] = "#FFFFCC";

                    break;
                case "Awaiting Improve Gate Black Belt Approval":
                    if (UserInBBGroup)
                    {
                        btnInvestigateBBApproval.Visible = true;
                        btnInvestigateReturntoProjectlead.Visible = true;
                        txtinvestigatecomment.ReadOnly = false;
                        btnUnlockform.Visible = true;
                    }

                    DefineColorId.Style["background-color"] = "#C3FDB8";
                    MeasureColorId.Style["background-color"] = "#C3FDB8";
                    AnalyzeColorId.Style["background-color"] = "#C3FDB8";
                    InvestigateColorId.Style["background-color"] = "#FFFFCC";
                    break;
                case "Control":
                    if (UserInGBGroup)
                    {
                        btnControlRequestApproval.Visible = true;
                        //btnControlReturntoProjectlead.Visible = true;
                        lnkcontrolAddDocuments.Enabled = true;
                        lnkcontrolRemoveDocuments.Enabled = true;
                        txtControlcomment.ReadOnly = false;
                    }
                    if (UserInBBGroup)
                    {
                        btnUnlockform.Visible = true;
                    }
                    btnSixSigmaSave.Visible = true;
                    DefineColorId.Style["background-color"] = "#C3FDB8";
                    MeasureColorId.Style["background-color"] = "#C3FDB8";
                    AnalyzeColorId.Style["background-color"] = "#C3FDB8";
                    InvestigateColorId.Style["background-color"] = "#C3FDB8";
                    ControlColorId.Style["background-color"] = "#FFFFCC";
                    break;
                case "Awaiting Control Gate Black Belt Approval":
                    if (UserInBBGroup)
                    {
                        btnControlBBApproval.Visible = true;
                        btnControlReturntoProjectlead.Visible = true;
                        txtControlcomment.ReadOnly = false;
                        btnUnlockform.Visible = true;
                    }
                    DefineColorId.Style["background-color"] = "#C3FDB8";
                    MeasureColorId.Style["background-color"] = "#C3FDB8";
                    AnalyzeColorId.Style["background-color"] = "#C3FDB8";
                    InvestigateColorId.Style["background-color"] = "#C3FDB8";
                    ControlColorId.Style["background-color"] = "#FFFFCC";
                    break;
                case "Final Report Preparation":
                    if (UserInGBGroup)
                    {
                        btnFinalreportRequestApproval.Visible = true;
                        //btnFinalreportReturntoProjectlead.Visible = true;
                        lnkfinalReportAddDocuments.Enabled = true;
                        lnkfinalReportRemoveDocuments.Enabled = true;
                        txtFinalreportcomment.ReadOnly = false;
                    }
                    if (UserInBBGroup)
                    {
                        btnUnlockform.Visible = true;
                    }
                    btnSixSigmaSave.Visible = true;
                    DefineColorId.Style["background-color"] = "#C3FDB8";
                    MeasureColorId.Style["background-color"] = "#C3FDB8";
                    AnalyzeColorId.Style["background-color"] = "#C3FDB8";
                    InvestigateColorId.Style["background-color"] = "#C3FDB8";
                    ControlColorId.Style["background-color"] = "#C3FDB8";
                    FinalReportColorId.Style["background-color"] = "#FFFFCC";
                    break;
                case "Awaiting Final Report Black Belt Approval":
                    if (UserInBBGroup)
                    {
                        btnFinalreportBBApproval.Visible = true;
                        btnFinalreportReturntoProjectlead.Visible = true;
                        txtFinalreportcomment.ReadOnly = false;
                        btnUnlockform.Visible = true;
                    }
                    DefineColorId.Style["background-color"] = "#C3FDB8";
                    MeasureColorId.Style["background-color"] = "#C3FDB8";
                    AnalyzeColorId.Style["background-color"] = "#C3FDB8";
                    InvestigateColorId.Style["background-color"] = "#C3FDB8";
                    ControlColorId.Style["background-color"] = "#C3FDB8";
                    FinalReportColorId.Style["background-color"] = "#FFFFCC";
                    break;
                case "Final Report Approved":
                    if (UserInBBGroup)
                    {
                        btnUnlockform.Visible = true;
                    }
                    lnkBackgroundAddDocuments.Enabled = false;
                    lnkBackgroundRemoveDocuments.Enabled = false;
                    lnkBenefitesAddDocuments.Enabled = false;
                    lnkBenefitesRemoveDocuments.Enabled = false;
                    lnkcontrolAddDocuments.Enabled = false;
                    lnkcontrolRemoveDocuments.Enabled = false;
                    lnkCostAddDocuments.Enabled = false;
                    lnkCostRemoveDocuments.Enabled = false;
                    lnkFinancialAddDocuments.Enabled = false;
                    lnkFinancialAddDocuments.Enabled = false;
                    lnkFinancialRemoveDocuments.Enabled = false;
                    lnkMileStonesAddDocuments.Enabled = false;
                    lnkMileStonesRemoveDocuments.Enabled = false;
                    lnkprojectmetricsAddDocuments.Enabled = false;
                    lnkprojectmetricsRemoveDocuments.Enabled = false;
                    lnkprbStatementAddDocuments.Enabled = false;
                    lnkprbStatementRemoveDocuments.Enabled = false;
                    DefineColorId.Style["background-color"] = "#C3FDB8";
                    MeasureColorId.Style["background-color"] = "#C3FDB8";
                    AnalyzeColorId.Style["background-color"] = "#C3FDB8";
                    InvestigateColorId.Style["background-color"] = "#C3FDB8";
                    ControlColorId.Style["background-color"] = "#C3FDB8";
                    FinalReportColorId.Style["background-color"] = "#C3FDB8";
                    break;
                default:
                    break;
            }
        }

        protected void SetCompletionDetails(SPListItem oSPListItem, string Status)
        {
            switch (Status)
            {
                case "Define":
                    oSPListItem["ProjectAuthorizationDate"] = DateTime.Now;
                    break;
                case "Measure":
                    oSPListItem["DefineCompletionDate"] = DateTime.Now;
                    oSPListItem["DefineApprover"] = SPContext.Current.Web.CurrentUser;
                    break;
                case "Analyze":
                    oSPListItem["MeasureCompletionDate"] = DateTime.Now;
                    oSPListItem["MeasureApprover"] = SPContext.Current.Web.CurrentUser;
                    break;
                case "Improve":
                    oSPListItem["AnalyzeCompletionDate"] = DateTime.Now;
                    oSPListItem["AnalyzeApprover"] = SPContext.Current.Web.CurrentUser;
                    break;
                case "Control":
                    oSPListItem["ImproveCompletionDate"] = DateTime.Now;
                    oSPListItem["InvestigateApprover"] = SPContext.Current.Web.CurrentUser;
                    break;
                case "Final Report Preparation":
                    oSPListItem["ControlCompletionDate"] = DateTime.Now;
                    oSPListItem["ControlApprover"] = SPContext.Current.Web.CurrentUser;
                    break;
                case "Final Report Approved":
                    oSPListItem["FinalReportCompletionDate"] = DateTime.Now;
                    oSPListItem["FinalReportApprover"] = SPContext.Current.Web.CurrentUser;
                    break;
            }

            SPContext.Current.Web.ValidateFormDigest();
            oSPListItem.Update();
        }

        private void setDateCss()
        {
            PlandateProjectAuthorization.DatePickerFrameUrl = SPContext.Current.Web.Url + "/_layouts/iframe.aspx";
            PlandateDefine.DatePickerFrameUrl = SPContext.Current.Web.Url + "/_layouts/iframe.aspx";
            PlandateMeasure.DatePickerFrameUrl = SPContext.Current.Web.Url + "/_layouts/iframe.aspx";
            PlandateAnalyze.DatePickerFrameUrl = SPContext.Current.Web.Url + "/_layouts/iframe.aspx";
            PlandateImprove.DatePickerFrameUrl = SPContext.Current.Web.Url + "/_layouts/iframe.aspx";
            PlandateControl.DatePickerFrameUrl = SPContext.Current.Web.Url + "/_layouts/iframe.aspx";
            PlandateFinalReportApprove.DatePickerFrameUrl = SPContext.Current.Web.Url + "/_layouts/iframe.aspx";
            ActualdateProjectAuthorization.DatePickerFrameUrl = SPContext.Current.Web.Url + "/_layouts/iframe.aspx";
            ActualdateDefine.DatePickerFrameUrl = SPContext.Current.Web.Url + "/_layouts/iframe.aspx";
            ActualdateMeasure.DatePickerFrameUrl = SPContext.Current.Web.Url + "/_layouts/iframe.aspx";
            ActualdateAnalyze.DatePickerFrameUrl = SPContext.Current.Web.Url + "/_layouts/iframe.aspx";
            ActualdateImprove.DatePickerFrameUrl = SPContext.Current.Web.Url + "/_layouts/iframe.aspx";
            ActualdateControl.DatePickerFrameUrl = SPContext.Current.Web.Url + "/_layouts/iframe.aspx";
            ActualdateFinalReportApprove.DatePickerFrameUrl = SPContext.Current.Web.Url + "/_layouts/iframe.aspx";

        }

        public void SetSupportingAttachmentsURL()
        {

            SPList list = SPContext.Current.Web.Lists[sixSigmaListName];
            strAgendaItemId = (Guid)list.ID;
            string PageUrl = SPContext.Current.Site.Url.ToString();
            string linkURLBackground = PageUrl + "/_layouts/PWC.Process.SixSigma/GetAttachment.aspx" + "?ItemId=" + sigmaId + "&&ListId=" + strAgendaItemId + "&AttachmentColumn=Bacground Attachments&IsDlg=" + 1;
            string linkURLProblemStatement = PageUrl + "/_layouts/PWC.Process.SixSigma/GetAttachment.aspx" + "?ItemId=" + sigmaId + "&&ListId=" + strAgendaItemId + "&AttachmentColumn=Problem Attachments&IsDlg=" + 1;
            string linkURLProjectMetrics = PageUrl + "/_layouts/PWC.Process.SixSigma/GetAttachment.aspx" + "?ItemId=" + sigmaId + "&&ListId=" + strAgendaItemId + "&AttachmentColumn=ProjectMetrics Attachments&IsDlg=" + 1;
            string linkURLBenefits = PageUrl + "/_layouts/PWC.Process.SixSigma/GetAttachment.aspx" + "?ItemId=" + sigmaId + "&&ListId=" + strAgendaItemId + "&AttachmentColumn=Benifits Attachments&IsDlg=" + 1;
            string linkURLCosts = PageUrl + "/_layouts/PWC.Process.SixSigma/GetAttachment.aspx" + "?ItemId=" + sigmaId + "&&ListId=" + strAgendaItemId + "&AttachmentColumn=Costs Attachments&IsDlg=" + 1;
            string linkURLFinancial = PageUrl + "/_layouts/PWC.Process.SixSigma/GetAttachment.aspx" + "?ItemId=" + sigmaId + "&&ListId=" + strAgendaItemId + "&AttachmentColumn=Financial Attachments&IsDlg=" + 1;
            string linkURLMilestones = PageUrl + "/_layouts/PWC.Process.SixSigma/GetAttachment.aspx" + "?ItemId=" + sigmaId + "&&ListId=" + strAgendaItemId + "&AttachmentColumn=Milestones Attachments&IsDlg=" + 1;
            string linkURLDefine = PageUrl + "/_layouts/PWC.Process.SixSigma/GetAttachment.aspx" + "?ItemId=" + sigmaId + "&&ListId=" + strAgendaItemId + "&AttachmentColumn=Define Attachments&IsDlg=" + 1;
            string linkURLMeasure = PageUrl + "/_layouts/PWC.Process.SixSigma/GetAttachment.aspx" + "?ItemId=" + sigmaId + "&&ListId=" + strAgendaItemId + "&AttachmentColumn=Measure Attachments&IsDlg=" + 1;
            string linkURLAnalyze = PageUrl + "/_layouts/PWC.Process.SixSigma/GetAttachment.aspx" + "?ItemId=" + sigmaId + "&&ListId=" + strAgendaItemId + "&AttachmentColumn=Analyze Attachments&IsDlg=" + 1;
            string linkURLInvestigate = PageUrl + "/_layouts/PWC.Process.SixSigma/GetAttachment.aspx" + "?ItemId=" + sigmaId + "&&ListId=" + strAgendaItemId + "&AttachmentColumn=Investigate Attachments&IsDlg=" + 1;
            string linkURLControls = PageUrl + "/_layouts/PWC.Process.SixSigma/GetAttachment.aspx" + "?ItemId=" + sigmaId + "&&ListId=" + strAgendaItemId + "&AttachmentColumn=Control Attachments&IsDlg=" + 1;
            string linkURLFinalReport = PageUrl + "/_layouts/PWC.Process.SixSigma/GetAttachment.aspx" + "?ItemId=" + sigmaId + "&&ListId=" + strAgendaItemId + "&AttachmentColumn=FinalReport Attachments&IsDlg=" + 1;


            lnkBackgroundAddDocuments.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + linkURLBackground + "','Link Document')");
            lnkprbStatementAddDocuments.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + linkURLProblemStatement + "','Link Document')");
            lnkprojectmetricsAddDocuments.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + linkURLProjectMetrics + "','Link Document')");
            lnkBenefitesAddDocuments.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + linkURLBenefits + "','Link Document')");
            lnkCostAddDocuments.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + linkURLCosts + "','Link Document')");
            lnkFinancialAddDocuments.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + linkURLFinancial + "','Link Document')");
            lnkMileStonesAddDocuments.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + linkURLMilestones + "','Link Document')");
            lnkDefineAddDocuments.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + linkURLDefine + "','Link Document')");
            lnkMeasureAddDocuments.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + linkURLMeasure + "','Link Document')");
            lnkAnalyzeAddDocuments.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + linkURLAnalyze + "','Link Document')");
            lnkInvestigateAddDocuments.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + linkURLInvestigate + "','Link Document')");
            lnkcontrolAddDocuments.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + linkURLControls + "','Link Document')");
            lnkfinalReportAddDocuments.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + linkURLFinalReport + "','Link Document')");


            string linkURLBackgroundRe = PageUrl + "/_layouts/PWC.Process.SixSigma/RemoveAttachment.aspx" + "?ItemId=" + sigmaId + "&&ListId=" + strAgendaItemId + "&AttachmentColumn=Bacground Attachments&IsDlg=" + 1;
            string linkURLProblemStatementRe = PageUrl + "/_layouts/PWC.Process.SixSigma/RemoveAttachment.aspx" + "?ItemId=" + sigmaId + "&&ListId=" + strAgendaItemId + "&AttachmentColumn=Problem Attachments&IsDlg=" + 1;
            string linkURLProjectMetricsRe = PageUrl + "/_layouts/PWC.Process.SixSigma/RemoveAttachment.aspx" + "?ItemId=" + sigmaId + "&&ListId=" + strAgendaItemId + "&AttachmentColumn=ProjectMetrics Attachments&IsDlg=" + 1;
            string linkURLBenefitsRe = PageUrl + "/_layouts/PWC.Process.SixSigma/RemoveAttachment.aspx" + "?ItemId=" + sigmaId + "&&ListId=" + strAgendaItemId + "&AttachmentColumn=Benifits Attachments&IsDlg=" + 1;
            string linkURLCostsRe = PageUrl + "/_layouts/PWC.Process.SixSigma/RemoveAttachment.aspx" + "?ItemId=" + sigmaId + "&&ListId=" + strAgendaItemId + "&AttachmentColumn=Costs Attachments&IsDlg=" + 1;

            string linkURLFinancialRe = PageUrl + "/_layouts/PWC.Process.SixSigma/RemoveAttachment.aspx" + "?ItemId=" + sigmaId + "&&ListId=" + strAgendaItemId + "&AttachmentColumn=Financial Attachments&IsDlg=" + 1;

            string linkURLMilestonesRe = PageUrl + "/_layouts/PWC.Process.SixSigma/RemoveAttachment.aspx" + "?ItemId=" + sigmaId + "&&ListId=" + strAgendaItemId + "&AttachmentColumn=Milestones Attachments&IsDlg=" + 1;
            string linkURLDefineRe = PageUrl + "/_layouts/PWC.Process.SixSigma/RemoveAttachment.aspx" + "?ItemId=" + sigmaId + "&&ListId=" + strAgendaItemId + "&AttachmentColumn=Define Attachments&IsDlg=" + 1;
            string linkURLMeasureRe = PageUrl + "/_layouts/PWC.Process.SixSigma/RemoveAttachment.aspx" + "?ItemId=" + sigmaId + "&&ListId=" + strAgendaItemId + "&AttachmentColumn=Measure Attachments&IsDlg=" + 1;
            string linkURLAnalyzeRe = PageUrl + "/_layouts/PWC.Process.SixSigma/RemoveAttachment.aspx" + "?ItemId=" + sigmaId + "&&ListId=" + strAgendaItemId + "&AttachmentColumn=Analyze Attachments&IsDlg=" + 1;
            string linkURLInvestigateRe = PageUrl + "/_layouts/PWC.Process.SixSigma/RemoveAttachment.aspx" + "?ItemId=" + sigmaId + "&&ListId=" + strAgendaItemId + "&AttachmentColumn=Investigate Attachments&IsDlg=" + 1;
            string linkURLControlsRe = PageUrl + "/_layouts/PWC.Process.SixSigma/RemoveAttachment.aspx" + "?ItemId=" + sigmaId + "&&ListId=" + strAgendaItemId + "&AttachmentColumn=Control Attachments&IsDlg=" + 1;
            string linkURLFinalReportRe = PageUrl + "/_layouts/PWC.Process.SixSigma/RemoveAttachment.aspx" + "?ItemId=" + sigmaId + "&&ListId=" + strAgendaItemId + "&AttachmentColumn=FinalReport Attachments&IsDlg=" + 1;



            lnkBackgroundRemoveDocuments.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + linkURLBackgroundRe + "','Remove Document')");
            lnkprbStatementRemoveDocuments.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + linkURLProblemStatementRe + "','Remove Document')");
            lnkprojectmetricsRemoveDocuments.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + linkURLProjectMetricsRe + "','Remove Document')");
            lnkBenefitesRemoveDocuments.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + linkURLBenefitsRe + "','Remove Document')");
            lnkCostRemoveDocuments.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + linkURLCostsRe + "','Remove Document')");
            lnkFinancialRemoveDocuments.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + linkURLFinancialRe + "','Remove Document')");
            lnkMileStonesRemoveDocuments.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + linkURLMilestonesRe + "','Remove Document')");
            lnkDefineRemoveDocuments.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + linkURLDefineRe + "','Remove Document')");
            lnkMeasureRemobveDocuments.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + linkURLMeasureRe + "','Remove Document')");
            lnkAnalyzeRemoveDocuments.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + linkURLAnalyzeRe + "','Remove Document')");
            lnkInvestigateRemoveDocuments.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + linkURLInvestigateRe + "','Remove Document')");
            lnkcontrolRemoveDocuments.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + linkURLControlsRe + "','Remove Document')");
            lnkfinalReportRemoveDocuments.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + linkURLFinalReportRe + "','Remove Document')");
        }

        public void BindSixSigmaDropDowns()
        {
            try
            {
                ddlorgnisation.Items.Clear();
                ddlplant.Items.Clear();
                ddlprojecttype.Items.Clear();
                ddlothermetric.Items.Clear();
                ddlothermetric1.Items.Clear();
                ddlMetricCost.Items.Clear();
                ddlQualityMetrics.Items.Clear();
                ddlDeliveryMetrics.Items.Clear();
                ddlQualityMetrics.Items.Add(new ListItem("--Select--", "0", true));
                ddlDeliveryMetrics.Items.Add(new ListItem("--Select--", "0", true));
                ddlMetricCost.Items.Add(new ListItem("--Select--", "0", true));
                ddlorgnisation.Items.Add(new ListItem("--Select--", "0", true));
                ddlplant.Items.Add(new ListItem("--Select--", "0", true));
                ddlprojecttype.Items.Add(new ListItem("--Select--", "0", true));
                ddlothermetric.Items.Add(new ListItem("--Select--", "0", true));
                ddlothermetric1.Items.Add(new ListItem("--Select--", "0", true));
               

                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (SPSite site = SPContext.Current.Site)
                    {
                        using (SPWeb drpwerb = site.OpenWeb())
                        {
                            try
                            {
                                SPList Organisationlist = drpwerb.Lists["Lookup_Organization_List"];
                                SPQuery OrganisationQuery = new SPQuery();
                                OrganisationQuery.Query = "<GroupBy><FieldRef Name=\"Organization\"/></GroupBy>";
                                SPListItemCollection OrganisationQueryitems = Organisationlist.GetItems(OrganisationQuery);

                                foreach (SPListItem OrgItem in OrganisationQueryitems)
                                {
                                    if (!string.IsNullOrEmpty(Convert.ToString(OrgItem["Organization"])))
                                    {
                                        if (ddlorgnisation.Items.FindByText(OrgItem["Organization"].ToString()) == null)
                                        {
                                            ListItem ThisItem = new ListItem();
                                            ThisItem.Text = OrgItem["Organization"].ToString();
                                            ThisItem.Value = OrgItem["Organization"].ToString();
                                            ddlorgnisation.Items.Add(ThisItem);
                                            ddlorgnisation.DataBind();
                                        }
                                    }
                                }



                                SPList Plantlist = drpwerb.Lists["Lookup_Plant_List"];
                                SPQuery PlantQuery = new SPQuery();
                                PlantQuery.Query = "<GroupBy><FieldRef Name=\"Plant\"/></GroupBy>";
                                SPListItemCollection PlantQueryitems = Plantlist.GetItems(PlantQuery);

                                foreach (SPListItem PlantItem in PlantQueryitems)
                                {
                                    if (!string.IsNullOrEmpty(Convert.ToString(PlantItem["Plant"])))
                                    {
                                        if (ddlplant.Items.FindByText(PlantItem["Plant"].ToString()) == null)
                                        {
                                            ListItem ThisItem = new ListItem();
                                            ThisItem.Text = PlantItem["Plant"].ToString();
                                            ThisItem.Value = PlantItem["Plant"].ToString();
                                            ddlplant.Items.Add(ThisItem);
                                            ddlplant.DataBind();


                                        }
                                    }
                                }





                                SPList MetricAreaList = drpwerb.Lists["Lookup_Metricsarea_List"];
                                SPQuery metricQuery = new SPQuery();
                                metricQuery.Query = "<OrderBy><FieldRef Name='ID' Ascending='true' /></OrderBy>";
                                SPListItemCollection metricQueryitems = MetricAreaList.GetItems(metricQuery);

                                foreach (SPListItem metricQueryItem in metricQueryitems)
                                {
                                    if (!string.IsNullOrEmpty(Convert.ToString(metricQueryItem["Metricsarea"])))
                                    {
                                        if (ddlMetricCost.Items.FindByText(metricQueryItem["Metricsarea"].ToString()) == null)
                                        {
                                            ListItem ThisItem = new ListItem();
                                            ThisItem.Text = metricQueryItem["Metricsarea"].ToString();
                                            ThisItem.Value = metricQueryItem["Metricsarea"].ToString();
                                            ddlMetricCost.Items.Add(ThisItem);
                                            ddlMetricCost.DataBind();
                                        }


                                        if (ddlQualityMetrics.Items.FindByText(metricQueryItem["Metricsarea"].ToString()) == null)
                                        {
                                            ListItem ThisItem = new ListItem();
                                            ThisItem.Text = metricQueryItem["Metricsarea"].ToString();
                                            ThisItem.Value = metricQueryItem["Metricsarea"].ToString();
                                            ddlQualityMetrics.Items.Add(ThisItem);
                                            ddlQualityMetrics.DataBind();
                                        }

                                        if (ddlDeliveryMetrics.Items.FindByText(metricQueryItem["Metricsarea"].ToString()) == null)
                                        {
                                            ListItem ThisItem = new ListItem();
                                            ThisItem.Text = metricQueryItem["Metricsarea"].ToString();
                                            ThisItem.Value = metricQueryItem["Metricsarea"].ToString();
                                            ddlDeliveryMetrics.Items.Add(ThisItem);
                                            ddlDeliveryMetrics.DataBind();
                                        }


                                        if (ddlothermetric.Items.FindByText(metricQueryItem["Metricsarea"].ToString()) == null)
                                        {
                                            ListItem ThisItem = new ListItem();
                                            ThisItem.Text = metricQueryItem["Metricsarea"].ToString();
                                            ThisItem.Value = metricQueryItem["Metricsarea"].ToString();
                                            ddlothermetric.Items.Add(ThisItem);
                                            ddlothermetric.DataBind();
                                        }


                                        if (ddlothermetric1.Items.FindByText(metricQueryItem["Metricsarea"].ToString()) == null)
                                        {
                                            ListItem ThisItem = new ListItem();
                                            ThisItem.Text = metricQueryItem["Metricsarea"].ToString();
                                            ThisItem.Value = metricQueryItem["Metricsarea"].ToString();
                                            ddlothermetric1.Items.Add(ThisItem);
                                            ddlothermetric1.DataBind();
                                        }

                                    }
                                }


                                SPList MultilingualList = drpwerb.Lists["Lookup_ProcertMultilingual_List"];
                                SPQuery MultilingualQuery = new SPQuery();
                                MultilingualQuery.Query = "<GroupBy><FieldRef Name=\"Title\"/></GroupBy>";
                                SPListItemCollection MultilingualQueryitems = MultilingualList.GetItems(MultilingualQuery);

                                foreach (SPListItem MultiLingualItem in MultilingualQueryitems)
                                {
                                    if (!string.IsNullOrEmpty(Convert.ToString(MultiLingualItem["Title"])))
                                    {
                                        if (ddlProcessForm1.Items.FindByText(MultiLingualItem["Title"].ToString()) == null)
                                        {
                                            ListItem ThisItem = new ListItem();
                                            ThisItem.Text = MultiLingualItem["Title"].ToString();
                                            ThisItem.Value = MultiLingualItem["LanguageValue"].ToString();
                                            ddlProcessForm1.Items.Add(ThisItem);
                                            ddlProcessForm1.DataBind();
                                            ddlProcessForm2.Items.Add(ThisItem);
                                            ddlProcessForm2.DataBind();
                                            ddlProcessForm3.Items.Add(ThisItem);
                                            ddlProcessForm3.DataBind();
                                            ddlProcessForm4.Items.Add(ThisItem);
                                            ddlProcessForm4.DataBind();
                                            ddlProcessForm5.Items.Add(ThisItem);
                                            ddlProcessForm5.DataBind();


                                        }
                                    }
                                }


                                SPList ProjectTypelist = drpwerb.Lists["Lookup_ProjectType_List"];
                                SPQuery ProjectTypeQuery = new SPQuery();
                                ProjectTypeQuery.Query = "<GroupBy><FieldRef Name=\"ProjectType\"/></GroupBy>";
                                SPListItemCollection ProjectTypeQueryitems = ProjectTypelist.GetItems(PlantQuery);
                                foreach (SPListItem ProjectTypeItem in ProjectTypeQueryitems)
                                {
                                    if (!string.IsNullOrEmpty(Convert.ToString(ProjectTypeItem["ProjectType"])))
                                    {
                                        if (ddlprojecttype.Items.FindByText(ProjectTypeItem["ProjectType"].ToString()) == null)
                                        {
                                            ListItem ThisItem = new ListItem();
                                            ThisItem.Text = ProjectTypeItem["ProjectType"].ToString();
                                            ThisItem.Value = ProjectTypeItem["ProjectType"].ToString();
                                            ddlprojecttype.Items.Add(ThisItem);
                                            ddlprojecttype.DataBind();


                                        }
                                    }
                                }

                                // Binding Taxonomy Fields in the Six Sigma Form
                                SPList list = drpwerb.Lists["BreakThroughProcertProjectsTracking"]; 
                                if (list != null)  
                                {
                                    TaxonomyField taxonomyField = list.Fields["Tags"] as TaxonomyField;  
                                    if (taxonomyField != null)  
                                    {
                                        taxTags.SspId.Add(taxonomyField.SspId);
                                        taxTags.TermSetId.Add(taxonomyField.TermSetId);  
                                    }  
                                }  

                            }
                            catch (Exception ex)
                            {
                                ULSLogger.LogErrorInULS("Error - In Adding SixSigmaDropdowns List Items" + ex.Message, TraceSeverity.Unexpected);

                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error In Bind in SixSigmaDropdowns: " + ex.Message, TraceSeverity.Unexpected);
            }


        }

        private SPListItem GeSixSigmaDataByID(int id)
        {
            SPListItem item = null;
            try
            {
                string siteUrl = SPContext.Current.Site.Url;
                using (SPSite oSite = new SPSite(siteUrl))
                {
                    using (SPWeb oWeb = oSite.OpenWeb())
                    {
                        SPList lstDemand = oWeb.Lists[sixSigmaListName];

                        item = lstDemand.GetItemById(id);
                    }
                }
                return item;
            }
            catch (Exception ex)
            {
                return item;

            }

        }

        private void SetControlsLabel(SPListItem lstItem)
        {
            try
            {
                #region Bind SixSigma TabsData

                if (lstItem != null)
                {
                    FinalReportSupportingDocuments.Style["Display"] = "";
                    ControlsSupportingDocuments.Style["Display"] = "";
                    InvestigateSupportingDocuments.Style["Display"] = "";
                    AnalyzeSupportingDocuments.Style["Display"] = "";
                    MeasureSupportingDocuments.Style["Display"] = "";
                    DefineSupportingDocuments.Style["Display"] = "";
                    BackgroundSupportingDocuments.Style["Display"] = "";
                    ProblemStatementSupportingDocuments.Style["Display"] = "";
                    BenfitsSupportingDocuments.Style["Display"] = "";
                    ProjectMetricsSupportingDocuments.Style["Display"] = "";
                    CosstsSupportingDocuments.Style["Display"] = "";
                    MilestonesSupportingDocuments.Style["Display"] = "";
                    FinancialSupportingDocuments.Style["Display"] = "";
                }

                string SixSigmaProjectSponser = Convert.ToString(lstItem["ProjectSponsor"]);
                string SixSigmaProjectBBUsers = Convert.ToString(lstItem["ProjectBBUsers"]);
                string SixSigmaProjectGBUsers = Convert.ToString(lstItem["ProjectGBUsers"]);
                string SixSigmaProjectStatus = Convert.ToString(lstItem["ProjectStatus"]);
                WorkflowStatus.Text = Convert.ToString(lstItem["ProjectStatus"]);
                GatesStatus.Text = Convert.ToString(lstItem["ProjectStatus"]);
                attachmentStatus.Text = Convert.ToString(lstItem["ProjectStatus"]);
                BasicInfoStatus.Text = Convert.ToString(lstItem["ProjectStatus"]);
                UpdatesStatus.Text = Convert.ToString(lstItem["ProjectStatus"]);
                lblProjectId1.Text = Convert.ToString(lstItem["ProjectId"]);
                lblProjectId2.Text = Convert.ToString(lstItem["ProjectId"]);
                lblProjectId3.Text = Convert.ToString(lstItem["ProjectId"]);
                lblProjectId4.Text = Convert.ToString(lstItem["ProjectId"]);
                lblProjectId5.Text = Convert.ToString(lstItem["ProjectId"]);
                ProjectName1.Text = Convert.ToString(lstItem["ProjectName"]);
                ProjectName2.Text = Convert.ToString(lstItem["ProjectName"]);
                ProjectName3.Text = Convert.ToString(lstItem["ProjectName"]);
                ProjectName4.Text = Convert.ToString(lstItem["ProjectName"]);
                ProjectName5.Text = Convert.ToString(lstItem["ProjectName"]);

                txtdefineComment.Text = Convert.ToString(lstItem["DefineComment"]);
                txtMeasurecomment.Text = Convert.ToString(lstItem["MeasureComment"]);
                txtAnalyzecomment.Text = Convert.ToString(lstItem["AnalyzeComment"]);
                txtinvestigatecomment.Text = Convert.ToString(lstItem["InvestigateComment"]);
                txtControlcomment.Text = Convert.ToString(lstItem["ControlComment"]);
                txtFinalreportcomment.Text = Convert.ToString(lstItem["FinalReportComment"]);

                ActualdateProjectAuthorization.SelectedDate = Convert.ToDateTime(lstItem["ProjectAuthorizationDate"]).Date;
                ActualdateDefine.SelectedDate = Convert.ToDateTime(lstItem["DefineCompletionDate"]).Date;
                ActualdateMeasure.SelectedDate = Convert.ToDateTime(lstItem["MeasureCompletionDate"]).Date;
                ActualdateAnalyze.SelectedDate = Convert.ToDateTime(lstItem["AnalyzeCompletionDate"]).Date;
                ActualdateImprove.SelectedDate = Convert.ToDateTime(lstItem["ImproveCompletionDate"]).Date;
                ActualdateControl.SelectedDate = Convert.ToDateTime(lstItem["ControlCompletionDate"]).Date;
                ActualdateFinalReportApprove.SelectedDate = Convert.ToDateTime(lstItem["FinalReportCompletionDate"]).Date;

                if (lstItem["DefineCompletionDate"] != null)
                {
                    string UserValueFromList = Convert.ToString(lstItem["DefineApprover"]);
                   // lblDefineCompletionDate.Text = "<span style='color: blue'>" + getDisplayName(UserValueFromList) + " [ " + getAccountName(UserValueFromList).Split('\\')[1] + " ] approved on " + Convert.ToDateTime(lstItem["DefineCompletionDate"]).ToString("MMM dd, yyyy") + "</span>";
                    lblDefineCompletionDate.Text = "<span style='color: blue'>" + getDisplayName(UserValueFromList) + " [ " + getAccountName(UserValueFromList).Split('\\')[1] + " ] </span>";
                    lblapproveddefineOn.Text = "<span style='color: blue'>approved</span>"; 
                    lblonapprove.Text = "<span style='color: blue'> on " + Convert.ToDateTime(lstItem["DefineCompletionDate"]).ToString("MMM dd, yyyy") + "</span>"; 

                }
                if (lstItem["MeasureCompletionDate"] != null)
                {
                    string UserValueFromList = Convert.ToString(lstItem["MeasureApprover"]);
                    lblMeasureCompletionDate.Text = "<span style='color: blue'>" + getDisplayName(UserValueFromList) + " [ " + getAccountName(UserValueFromList).Split('\\')[1] + " ] </span>";
                    lblapprovedMeasureOn.Text = "<span style='color: blue'>approved</span>";
                    lblonmeasure.Text = "<span style='color: blue'> on " + Convert.ToDateTime(lstItem["MeasureCompletionDate"]).ToString("MMM dd, yyyy") + "</span>"; 
                }
                if (lstItem["AnalyzeCompletionDate"] != null)
                {
                    string UserValueFromList = Convert.ToString(lstItem["AnalyzeApprover"]);
                    lblAnalyzeCompletionDate.Text = "<span style='color: blue'>" + getDisplayName(UserValueFromList) + " [ " + getAccountName(UserValueFromList).Split('\\')[1] + " ] </span>";
                    lblapprovedAnalyzeOn.Text = "<span style='color: blue'>approved</span>";
                    lblonanalyze.Text = "<span style='color: blue'> on " + Convert.ToDateTime(lstItem["AnalyzeCompletionDate"]).ToString("MMM dd, yyyy") + "</span>"; 
                }
                if (lstItem["ImproveCompletionDate"] != null)
                {
                    string UserValueFromList = Convert.ToString(lstItem["InvestigateApprover"]);
                    lblInvestigateCompletionDate.Text = "<span style='color: blue'>" + getDisplayName(UserValueFromList) + " [ " + getAccountName(UserValueFromList).Split('\\')[1] + " ] </span>";
                    lblapprovedImproveOn.Text = "<span style='color: blue'>approved</span>";
                    lblonImprove.Text = "<span style='color: blue'> on " + Convert.ToDateTime(lstItem["ImproveCompletionDate"]).ToString("MMM dd, yyyy") + "</span>"; 
                }
                if (lstItem["ControlCompletionDate"] != null)
                {
                    string UserValueFromList = Convert.ToString(lstItem["ControlApprover"]);
                    lblControlCompletionDate.Text = "<span style='color: blue'>" + getDisplayName(UserValueFromList) + " [ " + getAccountName(UserValueFromList).Split('\\')[1] + " ] </span>";
                    lblapprovedControlOn.Text = "<span style='color: blue'>approved</span>";
                    lbloncontrol.Text = "<span style='color: blue'> on " + Convert.ToDateTime(lstItem["ControlCompletionDate"]).ToString("MMM dd, yyyy") + "</span>"; 
                }
                if (lstItem["FinalReportCompletionDate"] != null)
                {
                    string UserValueFromList = Convert.ToString(lstItem["FinalReportApprover"]);
                    lblFinalReportCompletionDate.Text = "<span style='color: blue'>" + getDisplayName(UserValueFromList) + " [ " + getAccountName(UserValueFromList).Split('\\')[1] + " ] </span>";
                    lblapprovedFinalReportOn.Text = "<span style='color: blue'>approved</span>";
                    lblonfinal.Text = "<span style='color: blue'> on " + Convert.ToDateTime(lstItem["FinalReportCompletionDate"]).ToString("MMM dd, yyyy") + "</span>"; 
                }

                if (lstItem["Tags"] != null)
                {
                    TaxonomyFieldValueCollection value = lstItem["Tags"] as TaxonomyFieldValueCollection;
                    taxTags.Text =Convert.ToString(value);
                }

                if (!string.IsNullOrEmpty(SixSigmaProjectSponser))
                {
                    projectSponserUserEditor.CommaSeparatedAccounts = Convert.ToString(SPContext.Current.Web.SiteUsers.GetByID(Convert.ToInt32(SixSigmaProjectSponser.Split('#')[0].Split(';')[0])).LoginName);
                }

                if (!string.IsNullOrEmpty(SixSigmaProjectBBUsers))
                {
                    BlackbeltuserEditor.CommaSeparatedAccounts = Convert.ToString(SPContext.Current.Web.SiteUsers.GetByID(Convert.ToInt32(SixSigmaProjectBBUsers.Split('#')[0].Split(';')[0])).LoginName);
                }


                if (!string.IsNullOrEmpty(SixSigmaProjectGBUsers))
                {
                    GreenbeltuserEditor.CommaSeparatedAccounts = Convert.ToString(SPContext.Current.Web.SiteUsers.GetByID(Convert.ToInt32(SixSigmaProjectGBUsers.Split('#')[0].Split(';')[0])).LoginName);
                }



                txtProjectName.Text = Convert.ToString(lstItem["ProjectName"]);
                //lblProjectID.Text = Convert.ToString(lstItem["ProjectId"]);

                txtBackground.Text = Convert.ToString(lstItem["ProjectBackground"]);
                txtProjectstatementobj.Text = Convert.ToString(lstItem["ProjectProblemStatement"]);
                txtBenefits.Text = Convert.ToString(lstItem["ProjectBenefits"]);
                txtcosts.Text = Convert.ToString(lstItem["ProjectCosts"]);

                // Metrics setion
                if (!string.IsNullOrEmpty(Convert.ToString(lstItem["AreaCost"])))
                {
                    ddlMetricCost.ClearSelection();

                    ddlMetricCost.Items.FindByText(Convert.ToString(lstItem["AreaCost"])).Selected = true;
                }



                if (!string.IsNullOrEmpty(Convert.ToString(lstItem["AreaQuality"])))
                {
                    ddlQualityMetrics.ClearSelection();

                    ddlQualityMetrics.Items.FindByText(Convert.ToString(lstItem["AreaQuality"])).Selected = true;
                }


                if (!string.IsNullOrEmpty(Convert.ToString(lstItem["AreaDelivery"])))
                {
                    ddlDeliveryMetrics.ClearSelection();

                    ddlDeliveryMetrics.Items.FindByText(Convert.ToString(lstItem["AreaDelivery"])).Selected = true;
                }


                if (!string.IsNullOrEmpty(Convert.ToString(lstItem["AreaOther"])))
                {
                    ddlothermetric.ClearSelection();

                    ddlothermetric.Items.FindByText(Convert.ToString(lstItem["AreaOther"])).Selected = true;
                }

                if (!string.IsNullOrEmpty(Convert.ToString(lstItem["AreaOther1"])))
                {
                    ddlothermetric1.ClearSelection();

                    ddlothermetric1.Items.FindByText(Convert.ToString(lstItem["AreaOther1"])).Selected = true;
                }

                txtmetriccost.Text = Convert.ToString(lstItem["CostMetrics"]);
                txtmetricquality.Text = Convert.ToString(lstItem["QualityMetrics"]);
                txtmetricdelivery.Text = Convert.ToString(lstItem["DeliveryMetrics"]);
                txtmetricother.Text = Convert.ToString(lstItem["OtherMetric"]);
                txtmetricother1.Text = Convert.ToString(lstItem["OtherMetric1"]);


                txtCostBaseline.Text = Convert.ToString(lstItem["CostBaseline"]);
                txtCostGoal.Text = Convert.ToString(lstItem["CostGoal"]);
              //  ddlQualityMetrics.Text = Convert.ToString(lstItem["QualityMetrics"]);
                txtQualityBaseline.Text = Convert.ToString(lstItem["QualityBaseline"]);
                txtQualityGoal.Text = Convert.ToString(lstItem["QualityGoal"]);
             //   txtDeliveryMetrics.Text = Convert.ToString(lstItem["DeliveryMetrics"]);
                txtDeliveryBaseline.Text = Convert.ToString(lstItem["DeliveryBaseline"]);
                txtDeliveryGoal.Text = Convert.ToString(lstItem["DeliveryGoal"]);
                
                



                txtotherbaseline.Text = Convert.ToString(lstItem["OtherBaseline"]);
                txtothergoal.Text = Convert.ToString(lstItem["OtherGoal"]);
                txtotherbaseline1.Text = Convert.ToString(lstItem["OtherBaseline1"]);
                txtothergoal1.Text = Convert.ToString(lstItem["OtherGoal1"]);


                txtplannedActualCost.Text = Convert.ToString(lstItem["PlannedFinancialCost"]);
                txtplannedActualBenefits.Text = Convert.ToString(lstItem["PalnnedFinancialBenefits"]);
                txtActualCost.Text = Convert.ToString(lstItem["ActualFinancialCost"]);
                txtActualbenefits.Text = Convert.ToString(lstItem["ActualFinancialBenefits"]);



                // Milestones section

                PlandateProjectAuthorization.SelectedDate = Convert.ToDateTime(lstItem["PlannedProjectAuthorizationDate"]);

                PlandateDefine.SelectedDate = Convert.ToDateTime(lstItem["PlannedDefineDate"]);

                PlandateMeasure.SelectedDate = Convert.ToDateTime(lstItem["PlannedMeasureDate"]);

                PlandateAnalyze.SelectedDate = Convert.ToDateTime(lstItem["PlannedAnalyzeDate"]);

                PlandateImprove.SelectedDate = Convert.ToDateTime(lstItem["PlannedImproveDate"]);

                PlandateControl.SelectedDate = Convert.ToDateTime(lstItem["PlannedControlDate"]);

                PlandateFinalReportApprove.SelectedDate = Convert.ToDateTime(lstItem["PlannedFinalReportApprovalDate"]);

                if (!string.IsNullOrEmpty(Convert.ToString(lstItem["ProjectOrganization"])))
                {
                    ddlorgnisation.ClearSelection();
                    ddlorgnisation.Items.FindByText(Convert.ToString(lstItem["ProjectOrganization"])).Selected = true;
                }

                if (!string.IsNullOrEmpty(Convert.ToString(lstItem["ProjectPlan"])))
                {
                    ddlplant.ClearSelection();
                    ddlplant.Items.FindByText(Convert.ToString(lstItem["ProjectPlan"])).Selected = true;
                }

                if (!string.IsNullOrEmpty(Convert.ToString(lstItem["ProjectType"])))
                {
                    ddlprojecttype.ClearSelection();
                    ddlprojecttype.Items.FindByText(Convert.ToString(lstItem["ProjectType"])).Selected = true;
                }
                sigmaId = Convert.ToInt32(lstItem["ID"]);
                if (sigmaId != 0)
                {
                    string[] colName = { "Bacground_x0020_Attachments", "Problem_x0020_Attachments", "ProjectMetrics_x0020_Attachments", "Benifits_x0020_Attachments", "Costs_x0020_Attachments", "Financial_x0020_Attachments" ,"Milestones_x0020_Attachments", "Define_x0020_Attachments", "Measure_x0020_Attachments", "Analyze_x0020_Attachments", "Investigate_x0020_Attachments", "Control_x0020_Attachments", "FinalReport_x0020_Attachments" };
                    for (int i = 0; i < colName.Length; i++)
                    {
                        BindDocuments(Convert.ToString(sigmaId), colName[i]);
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On User SetControlsLabel: " + ex.Message, TraceSeverity.Unexpected);
            }

        }

        //This method was used to set the value in PeopleEditorControls when they are disabled.
        //Example Status!=Draft, when user is a part of Members group only & he tries to set his physical location, at that time these controls are
        //in read only mode thus fail to set values.
        //Updated on 19Jan2017 by Sid
        private void SetPeopleEditorControls(SPListItem lstItem)
        {
            try
            {
                if (lstItem != null)
                {
                     
                        
                        string SixSigmaProjectBBUsers = Convert.ToString(lstItem["ProjectBBUsers"]);
                        string SixSigmaProjectGBUsers = Convert.ToString(lstItem["ProjectGBUsers"]);
                        
                        if (!string.IsNullOrEmpty(SixSigmaProjectBBUsers))
                        {
                            BlackbeltuserEditor.CommaSeparatedAccounts = Convert.ToString(SPContext.Current.Web.SiteUsers.GetByID(Convert.ToInt32(SixSigmaProjectBBUsers.Split('#')[0].Split(';')[0])).LoginName);
                        }
                        if (!string.IsNullOrEmpty(SixSigmaProjectGBUsers))
                        {
                            GreenbeltuserEditor.CommaSeparatedAccounts = Convert.ToString(SPContext.Current.Web.SiteUsers.GetByID(Convert.ToInt32(SixSigmaProjectGBUsers.Split('#')[0].Split(';')[0])).LoginName);
                        }
                   
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On User SetPeopleEditorControls: " + ex.Message, TraceSeverity.Unexpected);
            }
        }

        public string getDisplayName(string userFieldFromList)
        {
            if (!String.IsNullOrEmpty(userFieldFromList))
                try
                {
                    if (userFieldFromList.Contains("\\"))
                    {
                        SPUser user = SPContext.Current.Web.EnsureUser(userFieldFromList);
                        if (user != null)
                            return user.Name;
                        else
                            return null;
                    }
                    else
                    {
                        SPFieldUserValue value = new SPFieldUserValue(SPContext.Current.Web, userFieldFromList);
                        if (value.User != null)
                            return value.User.Name;
                        else
                            return null;
                    }
                }
                catch (Exception ex) { return null; }
            return "";
        }

        public string getEmailID(string userFieldFromList)
        {
            if (!String.IsNullOrEmpty(userFieldFromList))
                try
                {
                    if (userFieldFromList.Contains("\\"))
                    {
                        SPUser user = SPContext.Current.Web.EnsureUser(userFieldFromList);
                        if (user != null)
                            return user.Email;
                        else
                            return null;
                    }
                    else
                    {
                        SPFieldUserValue value = new SPFieldUserValue(SPContext.Current.Web, userFieldFromList);
                        if (value.User != null)
                            return value.User.Email;
                        else
                            return null;
                    }
                }
                catch (Exception ex) { return null; }
            return "";
        }

        public string getAccountName(string userFieldFromList)
        {
            if (!String.IsNullOrEmpty(userFieldFromList))
                try
                {
                    if (userFieldFromList.Contains("\\"))
                    {
                        SPUser user = SPContext.Current.Web.EnsureUser(userFieldFromList);
                        if (user != null)
                            return user.LoginName;
                        else
                            return null;
                    }
                    else
                    {
                        SPFieldUserValue value = new SPFieldUserValue(SPContext.Current.Web, userFieldFromList);
                        if (value.User != null)
                            return value.User.LoginName;
                        else
                            return null;
                    }
                }
                catch (Exception ex) { return null; }
            else return "";

        }

        private bool IsMemberOf(string groupName)
        {
            SPUser user = SPContext.Current.Web.CurrentUser;

            try
            {
                if (user.Groups[groupName] != null)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        private void BindActionLogs(SPListItem lstItem)
        {
            try
            {
                DataTable dtActionLog = new DataTable();
                DataColumn ButtonAction = new DataColumn("ButtonAction", typeof(string));
                DataColumn SubmittedBy = new DataColumn("SubmittedBy", typeof(string));
                DataColumn Time = new DataColumn("Date", typeof(string));
                DataColumn Comments = new DataColumn("Comments", typeof(string));
                DataColumn Status = new DataColumn("Status", typeof(string));
                DataColumn ByWhom = new DataColumn("ByWhom", typeof(string));
                DataColumn SubmittedUserLoginName = new DataColumn("LoginName", typeof(string));
                dtActionLog.Columns.Add(ButtonAction);
                dtActionLog.Columns.Add(SubmittedBy);
                dtActionLog.Columns.Add(Time);
                dtActionLog.Columns.Add(Comments);
                dtActionLog.Columns.Add(Status);
                dtActionLog.Columns.Add(ByWhom);
                dtActionLog.Columns.Add(SubmittedUserLoginName);
                try
                {
                    BindActionLogs(Convert.ToString(lstItem["ProjectOverAllComments"]), Convert.ToString(lstItem["ProjectStatus"]), dtActionLog, grdviewAction);
                    BindWorkFlowHistoryLogs(Convert.ToString(lstItem["WorkFlowHistoryLogs"]), Convert.ToString(lstItem["ProjectStatus"]), dtActionLog, GridApproval);
                }

                catch (Exception ex)
                {
                    ULSLogger.LogErrorInULS("Error in  BindActionLogs" + ex.Message);
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error in  BindActionLogs" + ex.Message);
            }


        }

        private void BindActionLogs(string logs, string WRMStatus, DataTable dtActionLog, GridView grdviewAction)
        {
            try
            {

                dtActionLog.Clear();
                string[] arraylogHistory = null;

                string logsdetails = logs.Replace("undefined", "");
                arraylogHistory = logsdetails.Split(new string[] { "|##|" }, StringSplitOptions.None);


                for (int length = 0; length < arraylogHistory.Length; length++)
                {
                    if (!string.IsNullOrEmpty(arraylogHistory[length]))
                    {
                        DataRow drw = dtActionLog.NewRow();
                        string[] insideHistory = arraylogHistory[length].Split('|');
                        for (int insidelength = 0; insidelength < insideHistory.Length; insidelength++)
                        {
                            drw[insidelength] = insideHistory[insidelength];
                        }

                        dtActionLog.Rows.Add(drw);
                        dtActionLog.AcceptChanges();
                    }
                }

                if (dtActionLog.Rows.Count == 0)
                {
                    DataTable EmptyLogs = new DataTable();
                    DataColumn ButtonAction = new DataColumn("ButtonAction", typeof(string));
                    DataColumn SubmittedBy = new DataColumn("SubmittedBy", typeof(string));
                    DataColumn Time = new DataColumn("Date", typeof(string));
                    DataColumn Comments = new DataColumn("Comments", typeof(string));
                    DataColumn Status = new DataColumn("Status", typeof(string));
                    DataColumn ByWhom = new DataColumn("ByWhom", typeof(string));
                    DataColumn SubmittedUserLoginName = new DataColumn("LoginName", typeof(string));
                    EmptyLogs.Columns.Add(ButtonAction);
                    EmptyLogs.Columns.Add(SubmittedBy);
                    EmptyLogs.Columns.Add(Time);
                    EmptyLogs.Columns.Add(Comments);
                    EmptyLogs.Columns.Add(Status);
                    EmptyLogs.Columns.Add(ByWhom);
                    EmptyLogs.Columns.Add(SubmittedUserLoginName);
                    DataRow row = EmptyLogs.NewRow(); ;
                    EmptyLogs.Rows.Add(row);
                    EmptyLogs.AcceptChanges();
                    grdviewAction.DataSource = EmptyLogs;
                    grdviewAction.DataBind();


                }
                else
                {

                    grdviewAction.DataSource = dtActionLog;

                    grdviewAction.DataBind();

                }


            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error in  BindActionLogsMethod" + ex.Message);
            }
        }

        private void BindWorkFlowHistoryLogs(string logs, string WRMStatus, DataTable dtActionLog, GridView GridApproval)
        {
            try
            {

                dtActionLog.Clear();
                string[] arraylogHistory = null;

                string logsdetails = logs.Replace("undefined", "");
                arraylogHistory = logsdetails.Split(new string[] { "|##|" }, StringSplitOptions.None);


                for (int length = 0; length < arraylogHistory.Length; length++)
                {
                    if (!string.IsNullOrEmpty(arraylogHistory[length]))
                    {
                        DataRow drw = dtActionLog.NewRow();
                        string[] insideHistory = arraylogHistory[length].Split('|');
                        for (int insidelength = 0; insidelength < insideHistory.Length; insidelength++)
                        {
                            drw[insidelength] = insideHistory[insidelength];

                        }

                        dtActionLog.Rows.Add(drw);
                        dtActionLog.AcceptChanges();
                    }
                }

                DataTable result = dtActionLog.Select("Status <>''").CopyToDataTable();


                if (result.Rows.Count == 0)
                {
                    DataTable EmptyLogs = new DataTable();
                    DataColumn ButtonAction = new DataColumn("ButtonAction", typeof(string));
                    DataColumn SubmittedBy = new DataColumn("SubmittedBy", typeof(string));
                    DataColumn Time = new DataColumn("Date", typeof(string));
                    DataColumn Comments = new DataColumn("Comments", typeof(string));
                    DataColumn Status = new DataColumn("Status", typeof(string));
                    DataColumn ByWhom = new DataColumn("ByWhom", typeof(string));
                    DataColumn SubmittedUserLoginName = new DataColumn("LoginName", typeof(string));
                    EmptyLogs.Columns.Add(ButtonAction);
                    EmptyLogs.Columns.Add(SubmittedBy);
                    EmptyLogs.Columns.Add(Time);
                    EmptyLogs.Columns.Add(Comments);
                    EmptyLogs.Columns.Add(Status);
                    EmptyLogs.Columns.Add(ByWhom);
                    EmptyLogs.Columns.Add(SubmittedUserLoginName);
                    DataRow row = EmptyLogs.NewRow(); ;
                    EmptyLogs.Rows.Add(row);
                    EmptyLogs.AcceptChanges();
                    GridApproval.DataSource = EmptyLogs;
                    GridApproval.DataBind();
                }
                else
                {

                    GridApproval.DataSource = result;
                    GridApproval.DataBind();
                }




            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error in  BindWorkFlowHistoryLogsMethod" + ex.Message);
            }
        }

        protected void BindDocuments(string SigmaId, string ColumnName)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite Site = new SPSite(SiteUrl))
                    {
                        using (SPWeb web = Site.OpenWeb())
                        {
                            web.AllowUnsafeUpdates = true;
                            SPList list = web.Lists[sixSigmaListName];
                            AgendaListId = (list.ID);
                            SPQuery Query = new SPQuery();
                            Query.Query = "<Where><Eq><FieldRef Name='ID' /><Value Type='Number'>" + SigmaId + "</Value></Eq></Where>";
                            SPListItemCollection itemcoll = list.GetItems(Query);

                            DataTable dt = new DataTable();
                            dt = itemcoll.GetDataTable();
                            if (dt != null)
                            {
                                DataTable NewAttachmentNameList = new DataTable();
                                DataColumn AttachmentID = new DataColumn("ID", typeof(string));
                                DataColumn AttachmentName = new DataColumn("Name", typeof(string));
                                DataColumn AttachmentUrl = new DataColumn("Url", typeof(string));
                                NewAttachmentNameList.Columns.Add(AttachmentName);
                                NewAttachmentNameList.Columns.Add(AttachmentID);
                                NewAttachmentNameList.Columns.Add(AttachmentUrl);

                                string Attachments = Convert.ToString(dt.Rows[0][ColumnName]);//"Bacground_x0020_Attachments"
                                //int EditItem = web.GetListItem(Attachments).ID;
                                if (!string.IsNullOrEmpty(Attachments))
                                {
                                    ArrayList Namelist = new ArrayList();
                                    string[] lines = Attachments.Split(new[] { "<br />" }, StringSplitOptions.None);
                                    for (int i = 0; i < lines.Length; i++)
                                    {
                                        string value1 = string.Empty;
                                        XmlDocument doc = new XmlDocument();
                                        string attachmentHTML = "<div>" + lines[i] + "</div>";
                                        doc.LoadXml(attachmentHTML);
                                        XmlNodeList nodeList = doc.GetElementsByTagName("a");
                                        foreach (XmlNode node in nodeList)
                                        {
                                            value1 = node.Attributes["href"].Value;
                                            Namelist.Add(value1);
                                        }
                                    }
                                    if (Namelist.Count > 0)
                                    {
                                        for (int j = 0; j < Namelist.Count; j++)
                                        {
                                            try
                                            {
                                                SPListItem item = web.GetListItem(Convert.ToString(Namelist[j]));
                                                DataRow drw = NewAttachmentNameList.NewRow();
                                                drw["Name"] = item.Name;
                                                drw["ID"] = item.ID;
                                                drw["Url"] = web.Url + "/" + item.Url;
                                                NewAttachmentNameList.Rows.Add(drw);
                                            }
                                            catch (Exception ex)
                                            {
                                                ULSLogger.LogErrorInULS("Error occured in Bind Document" + ex.Message.ToString());
                                            }
                                        }
                                        NewAttachmentNameList.AcceptChanges();
                                    }
                                }
                                //string[] colName = { "Bacground_x0020_Attachments", "Problem_x0020_Attachments", "ProjectMetrics_x0020_Attachments", "Benifits_x0020_Attachments", "Costs_x0020_Attachments", "Milestones_x0020_Attachments", "Define_x0020_Attachments", "Measure_x0020_Attachments", "Analyze_x0020_Attachments", "Investigate_x0020_Attachments", "Control_x0020_Attachments", "FinalReport_x0020_Attachments" };
                                if (ColumnName == "Bacground_x0020_Attachments")
                                {
                                    if (NewAttachmentNameList != null && NewAttachmentNameList.Rows.Count > 0)
                                    {
                                        grdBackgrounDocuments.Visible = true;
                                        grdBackgrounDocuments.DataSource = NewAttachmentNameList;
                                        grdBackgrounDocuments.DataBind();

                                        if (NewAttachmentNameList.Rows.Count > 2)
                                        {
                                            divBackground.Style["height"] = "70px";
                                        }
                                        else
                                        {
                                            divBackground.Style["height"] = "";
                                        }
                                    }
                                    else
                                    {
                                        grdBackgrounDocuments.Visible = false;
                                        divBackground.Style["height"] = "";
                                    }
                                }
                                else if (ColumnName == "Problem_x0020_Attachments")
                                {
                                    if (NewAttachmentNameList != null && NewAttachmentNameList.Rows.Count > 0)
                                    {
                                        grdprbStatmentDocuments.Visible = true;
                                        grdprbStatmentDocuments.DataSource = NewAttachmentNameList;
                                        grdprbStatmentDocuments.DataBind();

                                        if (NewAttachmentNameList.Rows.Count > 2)
                                        {
                                            divPrbStatement.Style["height"] = "70px";
                                        }
                                        else
                                        {
                                            divPrbStatement.Style["height"] = "";
                                        }
                                    }
                                    else
                                    {
                                        grdprbStatmentDocuments.Visible = false;
                                        divPrbStatement.Style["height"] = "";
                                    }
                                }
                                else if (ColumnName == "ProjectMetrics_x0020_Attachments")
                                {
                                    if (NewAttachmentNameList != null && NewAttachmentNameList.Rows.Count > 0)
                                    {
                                        grdprjectMetricsDocuments.Visible = true;
                                        grdprjectMetricsDocuments.DataSource = NewAttachmentNameList;
                                        grdprjectMetricsDocuments.DataBind();

                                        if (NewAttachmentNameList.Rows.Count > 2)
                                        {
                                            divPrjectMetrics.Style["height"] = "70px";
                                        }
                                        else
                                        {
                                            divPrjectMetrics.Style["height"] = "";
                                        }
                                    }
                                    else
                                    {
                                        grdprjectMetricsDocuments.Visible = false;
                                        divPrjectMetrics.Style["height"] = "";
                                    }
                                }
                                else if (ColumnName == "Benifits_x0020_Attachments")
                                {
                                    if (NewAttachmentNameList != null && NewAttachmentNameList.Rows.Count > 0)
                                    {
                                        grdBenfitsDocuments.Visible = true;
                                        grdBenfitsDocuments.DataSource = NewAttachmentNameList;
                                        grdBenfitsDocuments.DataBind();

                                        if (NewAttachmentNameList.Rows.Count > 2)
                                        {
                                            divBenifits.Style["height"] = "70px";
                                        }
                                        else
                                        {
                                            divBenifits.Style["height"] = "";
                                        }
                                    }
                                    else
                                    {
                                        grdBenfitsDocuments.Visible = false;
                                        divBenifits.Style["height"] = "";
                                    }
                                }
                                else if (ColumnName == "Costs_x0020_Attachments")
                                {
                                    if (NewAttachmentNameList != null && NewAttachmentNameList.Rows.Count > 0)
                                    {
                                        grdCostsDocuments.Visible = true;
                                        grdCostsDocuments.DataSource = NewAttachmentNameList;
                                        grdCostsDocuments.DataBind();

                                        if (NewAttachmentNameList.Rows.Count > 2)
                                        {
                                            divCosts.Style["height"] = "70px";
                                        }
                                        else
                                        {
                                            divCosts.Style["height"] = "";
                                        }
                                    }
                                    else
                                    {
                                        grdCostsDocuments.Visible = false;
                                        divCosts.Style["height"] = "";
                                    }
                                }
                                else if (ColumnName == "Financial_x0020_Attachments")
                                {
                                    if (NewAttachmentNameList != null && NewAttachmentNameList.Rows.Count > 0)
                                    {
                                        grdFinancialDocuments.Visible = true;
                                        grdFinancialDocuments.DataSource = NewAttachmentNameList;
                                        grdFinancialDocuments.DataBind();

                                        if (NewAttachmentNameList.Rows.Count > 2)
                                        {
                                            divfinancial.Style["height"] = "70px";
                                        }
                                        else
                                        {
                                            divfinancial.Style["height"] = "";
                                        }
                                    }
                                    else
                                    {
                                        grdFinancialDocuments.Visible = false;
                                        divfinancial.Style["height"] = "";
                                    }
                                }
                                else if (ColumnName == "Milestones_x0020_Attachments")
                                {
                                    if (NewAttachmentNameList != null && NewAttachmentNameList.Rows.Count > 0)
                                    {
                                        grdMilestonesDocuments.Visible = true;
                                        grdMilestonesDocuments.DataSource = NewAttachmentNameList;
                                        grdMilestonesDocuments.DataBind();

                                        if (NewAttachmentNameList.Rows.Count > 2)
                                        {
                                            divMilestones.Style["height"] = "70px";
                                        }
                                        else
                                        {
                                            divMilestones.Style["height"] = "";
                                        }
                                    }
                                    else
                                    {
                                        grdMilestonesDocuments.Visible = false;
                                        divMilestones.Style["height"] = "";
                                    }
                                }
                                else if (ColumnName == "Define_x0020_Attachments")
                                {
                                    if (NewAttachmentNameList != null && NewAttachmentNameList.Rows.Count > 0)
                                    {
                                        grdDefineDocuments.Visible = true;
                                        grdDefineDocuments.DataSource = NewAttachmentNameList;
                                        grdDefineDocuments.DataBind();

                                        if (NewAttachmentNameList.Rows.Count > 2)
                                        {
                                            DivDefine.Style["height"] = "70px";
                                        }
                                        else
                                        {
                                            DivDefine.Style["height"] = "";
                                        }
                                    }
                                    else
                                    {
                                        grdDefineDocuments.Visible = false;
                                        DivDefine.Style["height"] = "";
                                    }
                                }
                                else if (ColumnName == "Measure_x0020_Attachments")
                                {
                                    if (NewAttachmentNameList != null && NewAttachmentNameList.Rows.Count > 0)
                                    {
                                        grdMeasureDocuments.Visible = true;
                                        grdMeasureDocuments.DataSource = NewAttachmentNameList;
                                        grdMeasureDocuments.DataBind();

                                        if (NewAttachmentNameList.Rows.Count > 2)
                                        {
                                            DivMeasure.Style["height"] = "70px";
                                        }
                                        else
                                        {
                                            DivMeasure.Style["height"] = "";
                                        }
                                    }
                                    else
                                    {
                                        grdMeasureDocuments.Visible = false;
                                        DivMeasure.Style["height"] = "";
                                    }
                                }
                                else if (ColumnName == "Analyze_x0020_Attachments")
                                {
                                    if (NewAttachmentNameList != null && NewAttachmentNameList.Rows.Count > 0)
                                    {
                                        grdAnalyzeDocuments.Visible = true;
                                        grdAnalyzeDocuments.DataSource = NewAttachmentNameList;
                                        grdAnalyzeDocuments.DataBind();

                                        if (NewAttachmentNameList.Rows.Count > 2)
                                        {
                                            DivAnalyze.Style["height"] = "70px";
                                        }
                                        else
                                        {
                                            DivAnalyze.Style["height"] = "";
                                        }
                                    }
                                    else
                                    {
                                        grdAnalyzeDocuments.Visible = false;
                                        DivAnalyze.Style["height"] = "";
                                    }
                                }



                                else if (ColumnName == "Investigate_x0020_Attachments")
                                {
                                    if (NewAttachmentNameList != null && NewAttachmentNameList.Rows.Count > 0)
                                    {
                                        grdInvestigateDocuments.Visible = true;
                                        grdInvestigateDocuments.DataSource = NewAttachmentNameList;
                                        grdInvestigateDocuments.DataBind();

                                        if (NewAttachmentNameList.Rows.Count > 2)
                                        {
                                            DivInvestigate.Style["height"] = "70px";
                                        }
                                        else
                                        {
                                            DivInvestigate.Style["height"] = "";
                                        }

                                    }
                                    else
                                    {
                                        grdInvestigateDocuments.Visible = false;
                                        DivInvestigate.Style["height"] = "";
                                    }
                                }



                                else if (ColumnName == "Control_x0020_Attachments")
                                {
                                    if (NewAttachmentNameList != null && NewAttachmentNameList.Rows.Count > 0)
                                    {
                                        grdControlDocuments.Visible = true;
                                        grdControlDocuments.DataSource = NewAttachmentNameList;
                                        grdControlDocuments.DataBind();

                                        if (NewAttachmentNameList.Rows.Count > 2)
                                        {
                                            DivControl.Style["height"] = "70px";
                                        }
                                        else
                                        {
                                            DivControl.Style["height"] = "";
                                        }

                                    }
                                    else
                                    {
                                        grdControlDocuments.Visible = false;
                                        DivControl.Style["height"] = "";
                                    }
                                }


                                else if (ColumnName == "FinalReport_x0020_Attachments")
                                {
                                    if (NewAttachmentNameList != null && NewAttachmentNameList.Rows.Count > 0)
                                    {
                                        grdFinalReportDocuments.Visible = true;
                                        grdFinalReportDocuments.DataSource = NewAttachmentNameList;
                                        grdFinalReportDocuments.DataBind();

                                        if (NewAttachmentNameList.Rows.Count > 2)
                                        {
                                            DivFinalReport.Style["height"] = "70px";
                                        }
                                        else
                                        {
                                            DivFinalReport.Style["height"] = "";
                                        }
                                    }
                                    else
                                    {
                                        grdFinalReportDocuments.Visible = false;
                                        DivFinalReport.Style["height"] = "";
                                    }
                                }

                            }
                        }
                    }
                });
            }
            catch (Exception Ex)
            {
                ULSLogger.LogErrorInULS("Inside catch in BindDocuments() in PWC.Process.SixSigma Feature..Error is--" + Ex.Message);
            }
        }

        protected void OnPageIndexChangeDocuments(object sender, GridViewPageEventArgs e)
        {
            //  grdBackgrounDocuments.PageIndex = e.NewPageIndex;
            //  BindDocuments(HiddenAgendaId);
        }

        private static void EnableNextPrevNavigationForNumericPagedGrid(GridView gv)
        {
            if (gv.BottomPagerRow == null)
                return;
            Table pagerTable = (Table)gv.BottomPagerRow.Controls[0].Controls[0];

            bool prevAdded = false;
            if (gv.PageIndex != 0)
            {
                TableCell prevCell = new TableCell();
                LinkButton prevLink = new LinkButton
                {
                    Text = "< Prev",
                    CommandName = "Page",
                    CommandArgument = ((LinkButton)pagerTable.Rows[0].Cells[gv.PageIndex - 1].Controls[0]).CommandArgument
                };
                prevLink.Style["text-decoration"] = "none";
                prevLink.Style["font-weight"] = "bold";
                prevLink.ForeColor = System.Drawing.Color.FromName("#0072bc");
                prevLink.Style["font-size"] = "10pt";
                prevCell.Controls.Add(prevLink);
                pagerTable.Rows[0].Cells.AddAt(0, prevCell);
                prevAdded = true;
            }

            if (gv.PageIndex != gv.PageCount - 1)
            {

                TableCell nextCell = new TableCell();
                LinkButton nextLink = new LinkButton
                {
                    Text = "Next >",
                    CommandName = "Page",
                    CommandArgument = ((LinkButton)pagerTable.Rows[0].Cells[gv.PageIndex +
                      (prevAdded ? 2 : 1)].Controls[0]).CommandArgument
                };
                nextLink.Style["text-decoration"] = "none";
                nextLink.Style["font-weight"] = "bold";
                nextLink.ForeColor = System.Drawing.Color.FromName("#0072bc");
                nextLink.Style["font-size"] = "10pt";
                nextCell.Controls.Add(nextLink);
                pagerTable.Rows[0].Cells.Add(nextCell);

            }
        }

        protected void grdBackgrounDocuments_DataBound(object sender, EventArgs e)
        {

            //   EnableNextPrevNavigationForNumericPagedGrid(grdBackgrounDocuments);

        }

        protected void grdBackgrounDocuments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string PageUrl = SPContext.Current.Site.Url.ToString();
                    string HiddenDocumentsEditValue = ((HiddenField)(e.Row.FindControl("hdnDocumentsID"))).Value;
                    string HiddenAgendaId = string.Empty;
                    Image OnEditWindow = (Image)e.Row.FindControl("DocumentsimageFaculty");
                    string _id = HttpUtility.UrlEncode(Encrypt(Convert.ToString(sigmaId)));
                    string documentEditUrl = PageUrl + "/Documents/Forms/EditForm.aspx?ID=" + HiddenDocumentsEditValue + "&Source=" + PageUrl + "/SitePages/BreakThroughProcertProjectsTracking.aspx?ProjectId=" + _id;
                    OnEditWindow.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + documentEditUrl + "','Documents - EditItem')");
                    OnEditWindow.ToolTip = "Click to edit the document properties.";
                    e.Row.Cells[1].Attributes.Add("style", "word-break:break-all;word-wrap:break-word");
                    string DocumentName = ((HyperLink)(e.Row.Cells[2].FindControl("lblName"))).Text;
                    HyperLink hypname = (HyperLink)e.Row.Cells[2].FindControl("lblName");
                    HiddenField hypname1 = (HiddenField)e.Row.Cells[2].FindControl("hdnUrl");

                    hypname.Attributes["onclick"] = String.Format("window.open('{0}','_blank')", hypname1.Value);
                   // Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                  //  SPUtility.MapToIcon(SPContext.Current.Web, DocumentName, string.Empty, IconSize.Size16);

                  //  image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icxlsx.png";


                    if (DocumentName.Contains(".xlsx") || DocumentName.Contains(".xls"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icxlsx.png";

                    }
                    else if (DocumentName.Contains(".png") || DocumentName.Contains(".jpg") || DocumentName.Contains(".jpeg") || DocumentName.Contains(".gif") || DocumentName.Contains(".bmp"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icpng.gif";
                    }
                    else if (DocumentName.Contains(".txt"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/ictxt.gif";

                    }
                    else if (DocumentName.Contains(".docx") || DocumentName.Contains(".doc"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icdocx.png";

                    }
                    else if (DocumentName.Contains(".pdf"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        SPUtility.MapToIcon(SPContext.Current.Web, DocumentName, string.Empty, IconSize.Size16);
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icgen.gif";
                    }
                    else if (DocumentName.Contains(".pptx") || DocumentName.Contains(".ppt"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icpptx.png";

                    }
                    else
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/ictxt.gif";
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Inside catch in grdBackgrounDocuments_RowDataBound() in PWC.Process.SixSigma Feature..Error is--" + ex.Message);
            }

        }

        protected void grdprbStatmentDocuments_DataBound(object sender, EventArgs e)
        {

            //   EnableNextPrevNavigationForNumericPagedGrid(grdBackgrounDocuments);

        }

        protected void grdprbStatmentDocuments_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            try
            {

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string PageUrl = SPContext.Current.Site.Url.ToString();
                    string HiddenDocumentsEditValue = ((HiddenField)(e.Row.FindControl("hdnDocumentsID"))).Value;
                    string HiddenAgendaId = string.Empty;
                    Image OnEditWindow = (Image)e.Row.FindControl("DocumentsimageFaculty");
                    string _id = HttpUtility.UrlEncode(Encrypt(Convert.ToString(sigmaId)));
                    string documentEditUrl = PageUrl + "/Documents/Forms/EditForm.aspx?ID=" + HiddenDocumentsEditValue + "&Source=" + PageUrl + "/SitePages/BreakThroughProcertProjectsTracking.aspx?ProjectId=" + _id;
                    OnEditWindow.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + documentEditUrl + "','Documents - EditItem')");
                    OnEditWindow.ToolTip = "Click to edit the document properties.";
                    e.Row.Cells[1].Attributes.Add("style", "word-break:break-all;word-wrap:break-word");
                    string DocumentName = ((HyperLink)(e.Row.Cells[2].FindControl("lblName"))).Text;
                    HyperLink hypname = (HyperLink)e.Row.Cells[2].FindControl("lblName");
                    HiddenField hypname1 = (HiddenField)e.Row.Cells[2].FindControl("hdnUrl");
                    hypname.Attributes["onclick"] = String.Format("window.open('{0}','_blank')", hypname1.Value);
                    if (DocumentName.Contains(".xlsx") || DocumentName.Contains(".xls"))
                    {

                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icxlsx.png";

                    }
                    else if (DocumentName.Contains(".png") || DocumentName.Contains(".jpg") || DocumentName.Contains(".jpeg") || DocumentName.Contains(".gif") || DocumentName.Contains(".bmp"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icpng.gif";

                    }
                    else if (DocumentName.Contains(".txt"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/ictxt.gif";

                    }
                    else if (DocumentName.Contains(".docx") || DocumentName.Contains(".doc"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icdocx.png";

                    }
                    else if (DocumentName.Contains(".pdf"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icgen.gif";

                    }
                    else if (DocumentName.Contains(".pptx") || DocumentName.Contains(".ppt"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icpptx.png";

                    }
                    else
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/ictxt.gif";

                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Inside catch in grdBackgrounDocuments_RowDataBound() in PWC.Process.SixSigma Feature..Error is--" + ex.Message);
            }

        }

        protected void grdprjectMetricsDocuments_DataBound(object sender, EventArgs e)
        {

            //EnableNextPrevNavigationForNumericPagedGrid(grdBackgrounDocuments);

        }

        protected void grdprjectMetricsDocuments_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            try
            {

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string PageUrl = SPContext.Current.Site.Url.ToString();
                    string HiddenDocumentsEditValue = ((HiddenField)(e.Row.FindControl("hdnDocumentsID"))).Value;
                    string HiddenAgendaId = string.Empty;
                    Image OnEditWindow = (Image)e.Row.FindControl("DocumentsimageFaculty");
                    string _id = HttpUtility.UrlEncode(Encrypt(Convert.ToString(sigmaId)));
                    string documentEditUrl = PageUrl + "/Documents/Forms/EditForm.aspx?ID=" + HiddenDocumentsEditValue + "&Source=" + PageUrl + "/SitePages/BreakThroughProcertProjectsTracking.aspx?ProjectId=" + _id;
                    OnEditWindow.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + documentEditUrl + "','Documents - EditItem')");
                    OnEditWindow.ToolTip = "Click to edit the document properties.";
                    e.Row.Cells[1].Attributes.Add("style", "word-break:break-all;word-wrap:break-word");
                    string DocumentName = ((HyperLink)(e.Row.Cells[2].FindControl("lblName"))).Text;
                    HyperLink hypname = (HyperLink)e.Row.Cells[2].FindControl("lblName");
                    HiddenField hypname1 = (HiddenField)e.Row.Cells[2].FindControl("hdnUrl");
                    hypname.Attributes["onclick"] = String.Format("window.open('{0}','_blank')", hypname1.Value);
                    if (DocumentName.Contains(".xlsx") || DocumentName.Contains(".xls"))
                    {

                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icxlsx.png";

                    }
                    else if (DocumentName.Contains(".png") || DocumentName.Contains(".jpg") || DocumentName.Contains(".jpeg") || DocumentName.Contains(".gif") || DocumentName.Contains(".bmp"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icpng.gif";

                    }
                    else if (DocumentName.Contains(".txt"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/ictxt.gif";

                    }
                    else if (DocumentName.Contains(".docx") || DocumentName.Contains(".doc"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icdocx.png";

                    }
                    else if (DocumentName.Contains(".pdf"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icgen.gif";

                    }
                    else if (DocumentName.Contains(".pptx") || DocumentName.Contains(".ppt"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icpptx.png";

                    }
                    else
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/ictxt.gif";

                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Inside catch in grdBackgrounDocuments_RowDataBound() in PWC.Process.SixSigma Feature..Error is--" + ex.Message);
            }

        }

        protected void grdBenfitsDocuments_DataBound(object sender, EventArgs e)
        {

            //EnableNextPrevNavigationForNumericPagedGrid(grdBackgrounDocuments);

        }

        protected void grdBenfitsDocuments_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            try
            {

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string PageUrl = SPContext.Current.Site.Url.ToString();
                    string HiddenDocumentsEditValue = ((HiddenField)(e.Row.FindControl("hdnDocumentsID"))).Value;
                    string HiddenAgendaId = string.Empty;
                    Image OnEditWindow = (Image)e.Row.FindControl("DocumentsimageFaculty");
                    string _id = HttpUtility.UrlEncode(Encrypt(Convert.ToString(sigmaId)));
                    string documentEditUrl = PageUrl + "/Documents/Forms/EditForm.aspx?ID=" + HiddenDocumentsEditValue + "&Source=" + PageUrl + "/SitePages/BreakThroughProcertProjectsTracking.aspx?ProjectId=" + _id;
                    OnEditWindow.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + documentEditUrl + "','Documents - EditItem')");
                    OnEditWindow.ToolTip = "Click to edit the document properties.";
                    e.Row.Cells[1].Attributes.Add("style", "word-break:break-all;word-wrap:break-word");
                    string DocumentName = ((HyperLink)(e.Row.Cells[2].FindControl("lblName"))).Text;
                    HyperLink hypname = (HyperLink)e.Row.Cells[2].FindControl("lblName");
                    HiddenField hypname1 = (HiddenField)e.Row.Cells[2].FindControl("hdnUrl");

                    hypname.Attributes["onclick"] = String.Format("window.open('{0}','_blank')", hypname1.Value);
                    if (DocumentName.Contains(".xlsx") || DocumentName.Contains(".xls"))
                    {

                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icxlsx.png";

                    }
                    else if (DocumentName.Contains(".png") || DocumentName.Contains(".jpg") || DocumentName.Contains(".jpeg") || DocumentName.Contains(".gif") || DocumentName.Contains(".bmp"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icpng.gif";

                    }
                    else if (DocumentName.Contains(".txt"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/ictxt.gif";

                    }
                    else if (DocumentName.Contains(".docx") || DocumentName.Contains(".doc"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icdocx.png";

                    }
                    else if (DocumentName.Contains(".pdf"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icgen.gif";

                    }
                    else if (DocumentName.Contains(".pptx") || DocumentName.Contains(".ppt"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");

                    }
                    else
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/ictxt.gif";

                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Inside catch in grdBackgrounDocuments_RowDataBound() in PWC.Process.SixSigma Feature..Error is--" + ex.Message);
            }

        }

        protected void grdCostsDocuments_DataBound(object sender, EventArgs e)
        {

            //   EnableNextPrevNavigationForNumericPagedGrid(grdBackgrounDocuments);

        }

        protected void grdCostsDocuments_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            try
            {

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string PageUrl = SPContext.Current.Site.Url.ToString();
                    string HiddenDocumentsEditValue = ((HiddenField)(e.Row.FindControl("hdnDocumentsID"))).Value;
                    string HiddenAgendaId = string.Empty;
                    Image OnEditWindow = (Image)e.Row.FindControl("DocumentsimageFaculty");
                    string _id = HttpUtility.UrlEncode(Encrypt(Convert.ToString(sigmaId)));
                    string documentEditUrl = PageUrl + "/Documents/Forms/EditForm.aspx?ID=" + HiddenDocumentsEditValue + "&Source=" + PageUrl + "/SitePages/BreakThroughProcertProjectsTracking.aspx?ProjectId=" + _id;
                    OnEditWindow.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + documentEditUrl + "','Documents - EditItem')");
                    OnEditWindow.ToolTip = "Click to edit the document properties.";
                    e.Row.Cells[1].Attributes.Add("style", "word-break:break-all;word-wrap:break-word");
                    string DocumentName = ((HyperLink)(e.Row.Cells[2].FindControl("lblName"))).Text;
                    HyperLink hypname = (HyperLink)e.Row.Cells[2].FindControl("lblName");

                    HiddenField hypname1 = (HiddenField)e.Row.Cells[2].FindControl("hdnUrl");


                    hypname.Attributes["onclick"] = String.Format("window.open('{0}','_blank')", hypname1.Value);
                    if (DocumentName.Contains(".xlsx") || DocumentName.Contains(".xls"))
                    {

                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icxlsx.png";

                    }
                    else if (DocumentName.Contains(".png") || DocumentName.Contains(".jpg") || DocumentName.Contains(".jpeg") || DocumentName.Contains(".gif") || DocumentName.Contains(".bmp"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icpng.gif";

                    }
                    else if (DocumentName.Contains(".txt"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/ictxt.gif";

                    }
                    else if (DocumentName.Contains(".docx") || DocumentName.Contains(".doc"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icdocx.png";

                    }
                    else if (DocumentName.Contains(".pdf"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icgen.gif";

                    }
                    else if (DocumentName.Contains(".pptx") || DocumentName.Contains(".ppt"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icpptx.png";

                    }
                    else
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/ictxt.gif";

                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Inside catch in grdBackgrounDocuments_RowDataBound() in PWC.Process.SixSigma Feature..Error is--" + ex.Message);
            }

        }

        protected void grdMilestonesDocuments_DataBound(object sender, EventArgs e)
        {

            //   EnableNextPrevNavigationForNumericPagedGrid(grdBackgrounDocuments);

        }

        protected void grdFinancialDocuments_DataBound(object sender, EventArgs e)
        {

            //   EnableNextPrevNavigationForNumericPagedGrid(grdBackgrounDocuments);

        }

        protected void grdFinancialDocuments_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            try
            {

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string PageUrl = SPContext.Current.Site.Url.ToString();
                    string HiddenDocumentsEditValue = ((HiddenField)(e.Row.FindControl("hdnDocumentsID"))).Value;
                    string HiddenAgendaId = string.Empty;
                    Image OnEditWindow = (Image)e.Row.FindControl("DocumentsimageFaculty");
                    string _id = HttpUtility.UrlEncode(Encrypt(Convert.ToString(sigmaId)));
                    string documentEditUrl = PageUrl + "/Documents/Forms/EditForm.aspx?ID=" + HiddenDocumentsEditValue + "&Source=" + PageUrl + "/SitePages/BreakThroughProcertProjectsTracking.aspx?ProjectId=" + _id;
                    OnEditWindow.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + documentEditUrl + "','Documents - EditItem')");
                    OnEditWindow.ToolTip = "Click to edit the document properties.";
                    e.Row.Cells[1].Attributes.Add("style", "word-break:break-all;word-wrap:break-word");



                    string DocumentName = ((HyperLink)(e.Row.Cells[2].FindControl("lblName"))).Text;
                    HyperLink hypname = (HyperLink)e.Row.Cells[2].FindControl("lblName");

                    HiddenField hypname1 = (HiddenField)e.Row.Cells[2].FindControl("hdnUrl");


                    hypname.Attributes["onclick"] = String.Format("window.open('{0}','_blank')", hypname1.Value);
                    if (DocumentName.Contains(".xlsx") || DocumentName.Contains(".xls"))
                    {

                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icxlsx.png";

                    }
                    else if (DocumentName.Contains(".png") || DocumentName.Contains(".jpg") || DocumentName.Contains(".jpeg") || DocumentName.Contains(".gif") || DocumentName.Contains(".bmp"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icpng.gif";

                    }
                    else if (DocumentName.Contains(".txt"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/ictxt.gif";

                    }
                    else if (DocumentName.Contains(".docx") || DocumentName.Contains(".doc"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icdocx.png";

                    }
                    else if (DocumentName.Contains(".pdf"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icgen.gif";

                    }
                    else if (DocumentName.Contains(".pptx") || DocumentName.Contains(".ppt"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icpptx.png";

                    }
                    else
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/ictxt.gif";

                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Inside catch in grdBackgrounDocuments_RowDataBound() in PWC.Process.SixSigma Feature..Error is--" + ex.Message);
            }

        }

        protected void grdMilestonesDocuments_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            try
            {

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string PageUrl = SPContext.Current.Site.Url.ToString();
                    string HiddenDocumentsEditValue = ((HiddenField)(e.Row.FindControl("hdnDocumentsID"))).Value;
                    string HiddenAgendaId = string.Empty;
                    Image OnEditWindow = (Image)e.Row.FindControl("DocumentsimageFaculty");
                    string _id = HttpUtility.UrlEncode(Encrypt(Convert.ToString(sigmaId)));
                    string documentEditUrl = PageUrl + "/Documents/Forms/EditForm.aspx?ID=" + HiddenDocumentsEditValue + "&Source=" + PageUrl + "/SitePages/BreakThroughProcertProjectsTracking.aspx?ProjectId=" + _id;
                    OnEditWindow.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + documentEditUrl + "','Documents - EditItem')");
                    OnEditWindow.ToolTip = "Click to edit the document properties.";
                    e.Row.Cells[1].Attributes.Add("style", "word-break:break-all;word-wrap:break-word");



                    string DocumentName = ((HyperLink)(e.Row.Cells[2].FindControl("lblName"))).Text;
                    HyperLink hypname = (HyperLink)e.Row.Cells[2].FindControl("lblName");

                    HiddenField hypname1 = (HiddenField)e.Row.Cells[2].FindControl("hdnUrl");


                    hypname.Attributes["onclick"] = String.Format("window.open('{0}','_blank')", hypname1.Value);
                    if (DocumentName.Contains(".xlsx") || DocumentName.Contains(".xls"))
                    {

                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icxlsx.png";

                    }
                    else if (DocumentName.Contains(".png") || DocumentName.Contains(".jpg") || DocumentName.Contains(".jpeg") || DocumentName.Contains(".gif") || DocumentName.Contains(".bmp"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icpng.gif";

                    }
                    else if (DocumentName.Contains(".txt"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/ictxt.gif";

                    }
                    else if (DocumentName.Contains(".docx") || DocumentName.Contains(".doc"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icdocx.png";

                    }
                    else if (DocumentName.Contains(".pdf"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icgen.gif";

                    }
                    else if (DocumentName.Contains(".pptx") || DocumentName.Contains(".ppt"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icpptx.png";

                    }
                    else
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/ictxt.gif";

                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Inside catch in grdBackgrounDocuments_RowDataBound() in PWC.Process.SixSigma Feature..Error is--" + ex.Message);
            }

        }

        protected void grdDefineDocuments_DataBound(object sender, EventArgs e)
        {

            //   EnableNextPrevNavigationForNumericPagedGrid(grdBackgrounDocuments);

        }

        protected void grdDefineDocuments_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            try
            {

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string PageUrl = SPContext.Current.Site.Url.ToString();
                    string HiddenDocumentsEditValue = ((HiddenField)(e.Row.FindControl("hdnDocumentsID"))).Value;
                    string HiddenAgendaId = string.Empty;
                    Image OnEditWindow = (Image)e.Row.FindControl("DocumentsimageFaculty");
                    string _id = HttpUtility.UrlEncode(Encrypt(Convert.ToString(sigmaId)));
                    string documentEditUrl = PageUrl + "/Documents/Forms/EditForm.aspx?ID=" + HiddenDocumentsEditValue + "&Source=" + PageUrl + "/SitePages/BreakThroughProcertProjectsTracking.aspx?ProjectId=" + _id;
                    OnEditWindow.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + documentEditUrl + "','Documents - EditItem')");
                    OnEditWindow.ToolTip = "Click to edit the document properties.";
                    e.Row.Cells[1].Attributes.Add("style", "word-break:break-all;word-wrap:break-word");

                    string DocumentName = ((HyperLink)(e.Row.Cells[2].FindControl("lblName"))).Text;
                    HyperLink hypname = (HyperLink)e.Row.Cells[2].FindControl("lblName");
                    HiddenField hypname1 = (HiddenField)e.Row.Cells[2].FindControl("hdnUrl");
                    hypname.Attributes["onclick"] = String.Format("window.open('{0}','_blank')", hypname1.Value);
                    if (DocumentName.Contains(".xlsx") || DocumentName.Contains(".xls"))
                    {

                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icxlsx.png";

                    }
                    else if (DocumentName.Contains(".png") || DocumentName.Contains(".jpg") || DocumentName.Contains(".jpeg") || DocumentName.Contains(".gif") || DocumentName.Contains(".bmp"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icpng.gif";

                    }
                    else if (DocumentName.Contains(".txt"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/ictxt.gif";

                    }
                    else if (DocumentName.Contains(".docx") || DocumentName.Contains(".doc"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icdocx.png";

                    }
                    else if (DocumentName.Contains(".pdf"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icgen.gif";

                    }
                    else if (DocumentName.Contains(".pptx") || DocumentName.Contains(".ppt"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icpptx.png";

                    }
                    else
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/ictxt.gif";

                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Inside catch in grdBackgrounDocuments_RowDataBound() in PWC.Process.SixSigma Feature..Error is--" + ex.Message);
            }

        }

        protected void grdMeasureDocuments_DataBound(object sender, EventArgs e)
        {

            //   EnableNextPrevNavigationForNumericPagedGrid(grdBackgrounDocuments);

        }

        protected void grdMeasureDocuments_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            try
            {

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string PageUrl = SPContext.Current.Site.Url.ToString();
                    string HiddenDocumentsEditValue = ((HiddenField)(e.Row.FindControl("hdnDocumentsID"))).Value;
                    string HiddenAgendaId = string.Empty;
                    Image OnEditWindow = (Image)e.Row.FindControl("DocumentsimageFaculty");
                    string _id = HttpUtility.UrlEncode(Encrypt(Convert.ToString(sigmaId)));
                    string documentEditUrl = PageUrl + "/Documents/Forms/EditForm.aspx?ID=" + HiddenDocumentsEditValue + "&Source=" + PageUrl + "/SitePages/BreakThroughProcertProjectsTracking.aspx?ProjectId=" + _id;
                    OnEditWindow.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + documentEditUrl + "','Documents - EditItem')");
                    OnEditWindow.ToolTip = "Click to edit the document properties.";
                    e.Row.Cells[1].Attributes.Add("style", "word-break:break-all;word-wrap:break-word");

                    string DocumentName = ((HyperLink)(e.Row.Cells[2].FindControl("lblName"))).Text;
                    HyperLink hypname = (HyperLink)e.Row.Cells[2].FindControl("lblName");

                    HiddenField hypname1 = (HiddenField)e.Row.Cells[2].FindControl("hdnUrl");


                    hypname.Attributes["onclick"] = String.Format("window.open('{0}','_blank')", hypname1.Value);
                    if (DocumentName.Contains(".xlsx") || DocumentName.Contains(".xls"))
                    {

                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icxlsx.png";

                    }
                    else if (DocumentName.Contains(".png") || DocumentName.Contains(".jpg") || DocumentName.Contains(".jpeg") || DocumentName.Contains(".gif") || DocumentName.Contains(".bmp"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icpng.gif";

                    }
                    else if (DocumentName.Contains(".txt"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/ictxt.gif";

                    }
                    else if (DocumentName.Contains(".docx") || DocumentName.Contains(".doc"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icdocx.png";

                    }
                    else if (DocumentName.Contains(".pdf"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icgen.gif";

                    }
                    else if (DocumentName.Contains(".pptx") || DocumentName.Contains(".ppt"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icpptx.png";

                    }
                    else
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/ictxt.gif";

                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Inside catch in grdBackgrounDocuments_RowDataBound() in PWC.Process.SixSigma Feature..Error is--" + ex.Message);
            }

        }

        protected void grdAnalyzeDocuments_DataBound(object sender, EventArgs e)
        {

            //   EnableNextPrevNavigationForNumericPagedGrid(grdBackgrounDocuments);

        }

        protected void grdAnalyzeDocuments_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            try
            {

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string PageUrl = SPContext.Current.Site.Url.ToString();
                    string HiddenDocumentsEditValue = ((HiddenField)(e.Row.FindControl("hdnDocumentsID"))).Value;
                    string HiddenAgendaId = string.Empty;
                    Image OnEditWindow = (Image)e.Row.FindControl("DocumentsimageFaculty");
                    string _id = HttpUtility.UrlEncode(Encrypt(Convert.ToString(sigmaId)));
                    string documentEditUrl = PageUrl + "/Documents/Forms/EditForm.aspx?ID=" + HiddenDocumentsEditValue + "&Source=" + PageUrl + "/SitePages/BreakThroughProcertProjectsTracking.aspx?ProjectId=" + _id;
                    OnEditWindow.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + documentEditUrl + "','Documents - EditItem')");
                    OnEditWindow.ToolTip = "Click to edit the document properties.";
                    e.Row.Cells[1].Attributes.Add("style", "word-break:break-all;word-wrap:break-word");

                    string DocumentName = ((HyperLink)(e.Row.Cells[2].FindControl("lblName"))).Text;
                    HyperLink hypname = (HyperLink)e.Row.Cells[2].FindControl("lblName");

                    HiddenField hypname1 = (HiddenField)e.Row.Cells[2].FindControl("hdnUrl");


                    hypname.Attributes["onclick"] = String.Format("window.open('{0}','_blank')", hypname1.Value);
                    if (DocumentName.Contains(".xlsx") || DocumentName.Contains(".xls"))
                    {

                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icxlsx.png";

                    }
                    else if (DocumentName.Contains(".png") || DocumentName.Contains(".jpg") || DocumentName.Contains(".jpeg") || DocumentName.Contains(".gif") || DocumentName.Contains(".bmp"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icpng.gif";

                    }
                    else if (DocumentName.Contains(".txt"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/ictxt.gif";

                    }
                    else if (DocumentName.Contains(".docx") || DocumentName.Contains(".doc"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icdocx.png";

                    }
                    else if (DocumentName.Contains(".pdf"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icgen.gif";

                    }
                    else if (DocumentName.Contains(".pptx") || DocumentName.Contains(".ppt"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icpptx.png";

                    }
                    else
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/ictxt.gif";


                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Inside catch in grdBackgrounDocuments_RowDataBound() in PWC.Process.SixSigma Feature..Error is--" + ex.Message);
            }

        }

        protected void grdInvestigateDocuments_DataBound(object sender, EventArgs e)
        {

            //   EnableNextPrevNavigationForNumericPagedGrid(grdBackgrounDocuments);

        }

        protected void grdInvestigateDocuments_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            try
            {

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string PageUrl = SPContext.Current.Site.Url.ToString();
                    string HiddenDocumentsEditValue = ((HiddenField)(e.Row.FindControl("hdnDocumentsID"))).Value;
                    string HiddenAgendaId = string.Empty;
                    Image OnEditWindow = (Image)e.Row.FindControl("DocumentsimageFaculty");
                    string _id = HttpUtility.UrlEncode(Encrypt(Convert.ToString(sigmaId)));
                    string documentEditUrl = PageUrl + "/Documents/Forms/EditForm.aspx?ID=" + HiddenDocumentsEditValue + "&Source=" + PageUrl + "/SitePages/BreakThroughProcertProjectsTracking.aspx?ProjectId=" + _id;
                    OnEditWindow.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + documentEditUrl + "','Documents - EditItem')");
                    OnEditWindow.ToolTip = "Click to edit the document properties.";
                    e.Row.Cells[1].Attributes.Add("style", "word-break:break-all;word-wrap:break-word");


                    string DocumentName = ((HyperLink)(e.Row.Cells[2].FindControl("lblName"))).Text;
                    HyperLink hypname = (HyperLink)e.Row.Cells[2].FindControl("lblName");

                    HiddenField hypname1 = (HiddenField)e.Row.Cells[2].FindControl("hdnUrl");


                    hypname.Attributes["onclick"] = String.Format("window.open('{0}','_blank')", hypname1.Value);
                    if (DocumentName.Contains(".xlsx") || DocumentName.Contains(".xls"))
                    {

                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icxlsx.png";

                    }
                    else if (DocumentName.Contains(".png") || DocumentName.Contains(".jpg") || DocumentName.Contains(".jpeg") || DocumentName.Contains(".gif") || DocumentName.Contains(".bmp"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icpng.gif";

                    }
                    else if (DocumentName.Contains(".txt"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/ictxt.gif";

                    }
                    else if (DocumentName.Contains(".docx") || DocumentName.Contains(".doc"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icdocx.png";


                    }
                    else if (DocumentName.Contains(".pdf"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icgen.gif";

                    }
                    else if (DocumentName.Contains(".pptx") || DocumentName.Contains(".ppt"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icpptx.png";

                    }
                    else
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/ictxt.gif";


                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Inside catch in grdBackgrounDocuments_RowDataBound() in PWC.Process.SixSigma Feature..Error is--" + ex.Message);
            }

        }

        protected void grdControlDocuments_DataBound(object sender, EventArgs e)
        {

            //   EnableNextPrevNavigationForNumericPagedGrid(grdBackgrounDocuments);

        }

        protected void grdControlDocuments_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            try
            {

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string PageUrl = SPContext.Current.Site.Url.ToString();
                    string HiddenDocumentsEditValue = ((HiddenField)(e.Row.FindControl("hdnDocumentsID"))).Value;
                    string HiddenAgendaId = string.Empty;
                    Image OnEditWindow = (Image)e.Row.FindControl("DocumentsimageFaculty");
                    string _id = HttpUtility.UrlEncode(Encrypt(Convert.ToString(sigmaId)));
                    string documentEditUrl = PageUrl + "/Documents/Forms/EditForm.aspx?ID=" + HiddenDocumentsEditValue + "&Source=" + PageUrl + "/SitePages/BreakThroughProcertProjectsTracking.aspx?ProjectId=" + _id;
                    OnEditWindow.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + documentEditUrl + "','Documents - EditItem')");
                    OnEditWindow.ToolTip = "Click to edit the document properties.";
                    e.Row.Cells[1].Attributes.Add("style", "word-break:break-all;word-wrap:break-word");

                    string DocumentName = ((HyperLink)(e.Row.Cells[2].FindControl("lblName"))).Text;
                    HyperLink hypname = (HyperLink)e.Row.Cells[2].FindControl("lblName");

                    HiddenField hypname1 = (HiddenField)e.Row.Cells[2].FindControl("hdnUrl");


                    hypname.Attributes["onclick"] = String.Format("window.open('{0}','_blank')", hypname1.Value);
                    if (DocumentName.Contains(".xlsx") || DocumentName.Contains(".xls"))
                    {

                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icxlsx.png";


                    }
                    else if (DocumentName.Contains(".png") || DocumentName.Contains(".jpg") || DocumentName.Contains(".jpeg") || DocumentName.Contains(".gif") || DocumentName.Contains(".bmp"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icpng.gif";


                    }
                    else if (DocumentName.Contains(".txt"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/ictxt.gif";


                    }
                    else if (DocumentName.Contains(".docx") || DocumentName.Contains(".doc"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icdocx.png";


                    }
                    else if (DocumentName.Contains(".pdf"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icgen.gif";

                    }
                    else if (DocumentName.Contains(".pptx") || DocumentName.Contains(".ppt"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icpptx.png";

                    }
                    else
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/ictxt.gif";

                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Inside catch in grdBackgrounDocuments_RowDataBound() in PWC.Process.SixSigma Feature..Error is--" + ex.Message);
            }

        }

        protected void grdFinalReportDocuments_DataBound(object sender, EventArgs e)
        {

            //   EnableNextPrevNavigationForNumericPagedGrid(grdBackgrounDocuments);

        }

        protected void grdFinalReportDocuments_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            try
            {

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string PageUrl = SPContext.Current.Site.Url.ToString();
                    string HiddenDocumentsEditValue = ((HiddenField)(e.Row.FindControl("hdnDocumentsID"))).Value;
                    string HiddenAgendaId = string.Empty;
                    Image OnEditWindow = (Image)e.Row.FindControl("DocumentsimageFaculty");
                    string _id = HttpUtility.UrlEncode(Encrypt(Convert.ToString(sigmaId)));
                    string documentEditUrl = PageUrl + "/Documents/Forms/EditForm.aspx?ID=" + HiddenDocumentsEditValue + "&Source=" + PageUrl + "/SitePages/BreakThroughProcertProjectsTracking.aspx?ProjectId=" + _id;
                    OnEditWindow.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + documentEditUrl + "','Documents - EditItem')");
                    OnEditWindow.ToolTip = "Click to edit the document properties.";
                    e.Row.Cells[1].Attributes.Add("style", "word-break:break-all;word-wrap:break-word");

                    string DocumentName = ((HyperLink)(e.Row.Cells[2].FindControl("lblName"))).Text;
                    HyperLink hypname = (HyperLink)e.Row.Cells[2].FindControl("lblName");

                    HiddenField hypname1 = (HiddenField)e.Row.Cells[2].FindControl("hdnUrl");


                    hypname.Attributes["onclick"] = String.Format("window.open('{0}','_blank')", hypname1.Value);
                    if (DocumentName.Contains(".xlsx") || DocumentName.Contains(".xls"))
                    {

                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icxlsx.png";

                    }
                    else if (DocumentName.Contains(".png") || DocumentName.Contains(".jpg") || DocumentName.Contains(".jpeg") || DocumentName.Contains(".gif") || DocumentName.Contains(".bmp"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icpng.gif";

                    }
                    else if (DocumentName.Contains(".txt"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/ictxt.gif";

                    }
                    else if (DocumentName.Contains(".docx") || DocumentName.Contains(".doc"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icdocx.png";

                    }
                    else if (DocumentName.Contains(".pdf"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icgen.gif";

                    }
                    else if (DocumentName.Contains(".pptx") || DocumentName.Contains(".ppt"))
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/icpptx.png";

                    }
                    else
                    {
                        Image image = (Image)e.Row.Cells[0].FindControl("imageType");
                        image.ImageUrl = "~/_layouts/15/PWC.Process.SixSigma/images/ictxt.gif";

                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Inside catch in grdBackgrounDocuments_RowDataBound() in PWC.Process.SixSigma Feature..Error is--" + ex.Message);
            }

        }

        protected void btnupload_Click(object sender, EventArgs e)
        {
            try
            {
                SelectedTab.Value = "3";
                string siteUrl = string.Empty;
                if (UploadFile.HasFile)
                {
                    if (UploadFile.PostedFile.ContentLength == 0)
                    {
                        trUploadErrorMsg.Style["display"] = "";
                        lblUploadErrorMsg.Style["display"] = "";
                        lblUploadErrorMsg.Text = "Please select a valid file. File size should not be zero.";
                    }
                    else
                    {
                        ArrayList itemdoc = Attachfile();
                        if (itemdoc != null && itemdoc.Count > 0)
                        {
                            trUploadErrorMsg.Style["display"] = "none"; //22March
                            lblUploadErrorMsg.Style["display"] = "none"; //22March
                            string getlink = itemdoc[0] + "/" + itemdoc[1];
                            itemdoc.Add(getlink);
                            ViewState["ItemUrl"] = itemdoc;
                            string editurl = string.Format("{0}{1}?ID={2}&Source=" + SPContext.Current.Web.Url + "/SitePages/BreakThroughProcertProjectsTracking.aspx.aspx?ProjectId=" + Encrypt(Convert.ToString(sigmaId)), itemdoc[0], "/" + itemdoc[2], Convert.ToInt32(itemdoc[3]));
                            StringBuilder functionSyntax = new StringBuilder();
                            functionSyntax.AppendLine("function popupparams() {");
                            functionSyntax.AppendLine("var url ='" + editurl + "';");
                            functionSyntax.AppendLine("popupmodaluiNewAttach(url);}");
                            functionSyntax.AppendLine("_spBodyOnLoadFunctionNames.push('popupparams');");
                            Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "ModalHostScript", functionSyntax.ToString(), true);
                            //SetControlsLabel(itemno);
                            AddAutoFeed("Nouveau document '" + itemdoc[4] + "' a été téléchargé dans la section 'Autres Accessoires'. Vous pouvez y accéder à partir de l’onglet Pièces jointes. /<br/><br/>New Document '" + itemdoc[4] + "' has been uploaded to the 'Other Attachments' section. You can access it from the Attachments Tab.", sigmaId);
                        }
                        else
                        {
                            //else added on 22march //Uploading file failed
                            string errorMessage = string.Empty;
                            if (AttachmentErrorMessage.Contains("already exists"))
                            {
                                errorMessage = "Un fichier avec ce nom existe déjà. /<br/>A file with this name already exists.";
                                trUploadErrorMsg.Style["display"] = "";
                                lblUploadErrorMsg.Style["display"] = "";
                                lblUploadErrorMsg.Text = errorMessage;

                                ScriptManager.RegisterStartupScript(updatePanel, updatePanel.GetType(), "abc", "SetOnAttachment();", true);
                            }
                            else if (AttachmentErrorMessage.Contains("invalid characters"))
                            {
                                errorMessage = "Le nom de fichier est invalide. Un nom de fichier ne peut pas contenir les caractères suivants: \\ / []: . ! * ? \" < > | # $ & { } % ~  /<br/>The file name is invalid. A file name cannot contain any of the following characters: \\ / []: . ! * ? \" < > | # $ & { } % ~ ";
                                trUploadErrorMsg.Style["display"] = "";
                                lblUploadErrorMsg.Style["display"] = "";
                                lblUploadErrorMsg.Text = errorMessage;
                                ScriptManager.RegisterStartupScript(updatePanel, updatePanel.GetType(), "abc", "SetOnAttachment();", true);
                            }
                            GetAttachments(SiteUrl);
                        }
                    }
                }

                else
                {
                    if (UploadFile.PostedFile.ContentLength == 0)
                    {
                        trUploadErrorMsg.Style["display"] = "";
                        lblUploadErrorMsg.Style["display"] = "";
                        lblUploadErrorMsg.Text = "Please select a valid file. File size should not be zero.";
                        //Page.ClientScript.RegisterStartupScript(typeof(string), "Alert1", "<script type='text/javascript'>alert('Please select a valid file. File size should not be zero.');</script>");
                        return;
                    }

                }


            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  btnupload_Click: " + ex.Message, TraceSeverity.Unexpected);
            }
        }

        protected ArrayList Attachfile()
        {
            string siteUrl = string.Empty;
            SPUser user = SPContext.Current.Web.CurrentUser;
            ArrayList arr = new ArrayList();
            FileStream fileStream = null;
            string fileName = string.Empty;
            string FolderName = string.Empty;
            int id = 0;
            SPListItem item = null;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite oSite = new SPSite(SPContext.Current.Site.Url))
                {
                    using (SPWeb oWeb = oSite.OpenWeb())
                    {
                        oWeb.AllowUnsafeUpdates = true;
                        string[] libfolder = documentsListName.Split('/');
                        string NameofLib = string.Empty;
                        string Nameoffolder = string.Empty;
                        SPFile file = null;
                        if (libfolder.Length > 1)
                        {
                            NameofLib = libfolder[0];
                            Nameoffolder = libfolder[1];
                        }
                        else
                        {
                            NameofLib = documentsListName;
                        }

                        SPDocumentLibrary doclib = (SPDocumentLibrary)oWeb.Lists[NameofLib];

                        fileName = System.IO.Path.GetFileName(UploadFile.PostedFile.FileName);
                        string _fileTime = DateTime.Now.ToFileTime().ToString();
                        string _fileorgPath = System.IO.Path.GetFullPath(UploadFile.PostedFile.FileName);
                        string _newfilePath = _fileTime + "~" + fileName;
                        double length = (UploadFile.PostedFile.InputStream.Length) / 1024;

                        //Commented by Sid to change Attachment location.
                        //FolderName = "SixSigma-" + sigmaId;
                        FolderName = "OtherAttachments";
                        
                        string tempFolder = Environment.GetEnvironmentVariable("TEMP");
                        string _filepath = tempFolder + _newfilePath;
                        UploadFile.PostedFile.SaveAs(_filepath);
                        
                        //Commented by Sid to change Attachment location.
                        //SPFolderCollection folderColl = doclib.RootFolder.SubFolders;
                        //SPFolder Folder = folderColl.Add(FolderName);
                        SPFolder projectFolder = oWeb.GetFolder(oWeb.ServerRelativeUrl + "Documents/Project" + Convert.ToString(sigmaId));
                        SPFolderCollection projectFolderColl = projectFolder.SubFolders;
                        SPFolder Folder = projectFolderColl.Add(FolderName);

                        if (Folder.Exists)
                        {
                            try
                            {
                                fileStream = File.OpenRead(_filepath);
                                file = oWeb.Files.Add(oWeb.Url.ToString() + "/" + Folder.ToString() + "/" + fileName, fileStream, false); //22March - argument changed from true to false -> file could not be overwrite
                                file.Update();
                                 
                                doclib.Update();
                                SPQuery query = new SPQuery();
                                query.Folder = Folder;
                                query.ViewAttributes = "Scope=\"Recursive\"";

                                SPListItemCollection allitems = doclib.GetItems(query);
                                DataTable dt = new DataTable();
                                dt = allitems.GetDataTable();
                                AttachmentGrid.DataSource = dt;
                                AttachmentGrid.DataBind();

                            }
                            catch (Exception ex)
                            {
                                ULSLogger.LogErrorInULS(ex.Message, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected);
                                AttachmentErrorMessage = ex.Message; //22March
                                return; //22March
                            }
                        }
                        SPFolder root = null;
                        if (!string.IsNullOrEmpty(Folder.Url))
                        {
                            root = oWeb.GetFolder(Folder.Url);
                        }
                        else if (!string.IsNullOrEmpty(Nameoffolder))
                        {
                            root = oWeb.GetFolder(documentsListName);
                        }
                        else
                        {
                            root = doclib.RootFolder;
                        }
                        // Upload document
                        oWeb.AllowUnsafeUpdates = true;
                        id = file.Item.ID;
                        item = file.Item;
                        file.Update();
                        try
                        {
                            item["Author"] = user;
                            item["Editor"] = user;
                            item.UpdateOverwriteVersion();
                        }
                        catch (Exception ex)
                        {
                        }

                        oWeb.AllowUnsafeUpdates = false;
                        arr.Add(item.Web.Url);
                        arr.Add(item.File.Url);
                        arr.Add(item.ParentList.Forms[PAGETYPE.PAGE_EDITFORM].Url);
                        arr.Add(Convert.ToString(item.ID));
                        arr.Add(item.Name);
                    }
                }
            });

            return arr;
        }

        protected void AttachmentGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Remove")
                {
                    int Removeid = Convert.ToInt32(e.CommandArgument.ToString());

                    SPListItem RemovelstItem = GetRemoveById(Removeid);

                    ScriptManager.RegisterClientScriptBlock(updatePanel, updatePanel.GetType(), "tabs3", "SetOnAttachment();", true);
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  AttachmentGrid_RowCommand: " + ex.Message, TraceSeverity.Unexpected);
            }

        }

        protected void AttachmentGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (sigmaId != 0)
                    {
                        System.Web.UI.WebControls.Label LabelItemPath = (System.Web.UI.WebControls.Label)e.Row.Cells[2].FindControl("lblItemPathHidden");
                        string ItemID = LabelItemPath.Text;
                        string itemUrl = SPContext.Current.Web.Lists[documentsListName].GetItemById(Convert.ToInt32(ItemID)).Url;
                        string FullServerPath = SPContext.Current.Site.Url + "/" + itemUrl;
                        ((HyperLink)e.Row.Cells[0].FindControl("hypAttachmentLink")).Text = FullServerPath;
                        ((HyperLink)e.Row.Cells[0].FindControl("hypAttachmentLink")).NavigateUrl = FullServerPath;

                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  AttachmentGrid_RowDataBound: " + ex.Message, TraceSeverity.Unexpected);
            }


        }

        private SPListItem GetRemoveById(int id)
        {
            SelectedTab.Value = "3";
            SPListItem item = null;
            try
            {

                using (SPSite oSite = new SPSite(SPContext.Current.Site.Url))
                {
                    using (SPWeb oWeb = oSite.OpenWeb())
                    {
                        oWeb.AllowUnsafeUpdates = true;
                        
                        //Commented by Sid to change Attachment location.
                        //string FolderName = "SixSigma-" + Convert.ToString(sigmaId);
                        string FolderName = "Project" + Convert.ToString(sigmaId)+"/OtherAttachments";

                        string NameofLib = documentsListName;
                        SPDocumentLibrary doclib = (SPDocumentLibrary)oWeb.Lists[NameofLib];
                        SPFolder folder = oWeb.GetFolder(FolderName);
                        SPFileCollection files = folder.Files;
                        item = doclib.GetItemById(id);
                        SPFile file = item.File;
                        file.Delete();
                        folder.Update();
                        //item.Delete();
                        SPQuery query = new SPQuery();
                        query.ViewAttributes = "Scope=\"Recursive\"";

                        //Commented by Sid to change Attachment location.
                        //folder = doclib.RootFolder.SubFolders[FolderName];
                        SPFolder projectFolder = oWeb.GetFolder(oWeb.ServerRelativeUrl + "Documents/Project" + Convert.ToString(sigmaId));
                        folder = projectFolder.SubFolders["OtherAttachments"];
                        
                        query.Folder = folder;
                        SPListItemCollection allitems = doclib.GetItems(query);
                        int filecount = allitems.Count;
                        DataTable dt = new DataTable();
                        dt = allitems.GetDataTable();
                        AttachmentGrid.DataSource = dt;
                        AttachmentGrid.DataBind();


                    }
                }
                return item;

            }
            catch (Exception ex)
            {

                ULSLogger.LogErrorInULS("Error On  GetRemoveById: " + ex.Message, TraceSeverity.Unexpected);
                return item;
            }


        }

        private void GetAttachments(string CurrentSiteURL)
        {
            try
            {
                string url = SPContext.Current.Site.Url;
                SPSecurity.RunWithElevatedPrivileges(delegate
                {

                    using (SPSite site = new SPSite(url))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {

                            web.AllowUnsafeUpdates = true;
                            //string NameofLib = documentsListName;
                            SPDocumentLibrary doclib = (SPDocumentLibrary)web.Lists[documentsListName];
                            if (ViewState["ItemUrl"] != null && (ArrayList)ViewState["ItemUrl"] != null)
                            {
                                ArrayList item = (ArrayList)ViewState["ItemUrl"];
                                string itemUrl = web.Lists[documentsListName].GetItemById(Convert.ToInt32(item[3])).Url;
                                try
                                {
                                    SPListItem itemDoc = web.Lists[documentsListName].GetItemById(Convert.ToInt32(item[3]));
                                    // BindAttachmentGrid(CurrentSiteURL, sigmaId);
                                    web.AllowUnsafeUpdates = false;
                                }
                                catch (Exception ex)
                                {
                                    ULSLogger.LogErrorInULS("Error On  GetAttachments: " + ex.Message, TraceSeverity.Unexpected);
                                }
                            }

                        }
                    }
                });
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  GetAttachments: " + ex.Message, TraceSeverity.Unexpected);
            }
        }

        protected void AttachmentGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                //BindAttachmentGrid(SPContext.Current.Site.Url, sigmaId);
                AttachmentGrid.PageIndex = e.NewPageIndex;
                AttachmentGrid.DataBind();
                //SPListItem RemovelstItem = GetDemandDataByID(sigmaId);
                // SetControlsLabel(RemovelstItem);


            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  AttachmentGrid_PageIndexChanging: " + ex.Message, TraceSeverity.Unexpected);
            }
        }

        protected void AttachmentGrid_PageIndexChanged(object sender, EventArgs e)
        {
            //SPListItem RemovelstItem = GetDemandDataByID(sigmaId);
            //SetControlsLabel(RemovelstItem);
            ScriptManager.RegisterClientScriptBlock(updatePanel, updatePanel.GetType(), "tabs3", "SetOnAttachment();", true);
        }

        private void BindAttachmentGrid(string CurrentSiteURL, int sigmaId)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite site = new SPSite(CurrentSiteURL))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            try
                            {
                                //string NameofLib = "Documents";
                                SPDocumentLibrary doclib = (SPDocumentLibrary)web.Lists[documentsListName];
                                SPQuery query = new SPQuery();

                                //Commented by Sid to change Attachment location.
                                //string FolderName = "SixSigma-" + sigmaId;
                                string FolderName = "OtherAttachments";

                                if (!string.IsNullOrEmpty(FolderName))
                                {
                                    //Commented by Sid to change Attachment location.
                                    //SPFolder folder = doclib.RootFolder.SubFolders[FolderName];
                                    SPFolder projectFolder = web.GetFolder(web.ServerRelativeUrl + "Documents/Project" + Convert.ToString(sigmaId));
                                    SPFolder folder = projectFolder.SubFolders[FolderName];

                                    query.Folder = folder;
                                    query.ViewAttributes = "Scope=\"Recursive\"";
                                    SPListItemCollection allitems = doclib.GetItems(query);
                                    DataTable dt = new DataTable();
                                    dt = allitems.GetDataTable();
                                    AttachmentGrid.DataSource = dt;
                                    AttachmentGrid.DataBind();
                                }
                            }
                            catch (Exception ex)
                            {
                                ULSLogger.LogErrorInULS(ex.Message);
                            }

                        }
                    }
                });
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  BindAttachmentGrid: " + ex.Message, TraceSeverity.Unexpected);
            }
        }

        protected void grdviewAdmin_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    e.Row.Cells[3].Attributes.Add("style", "white-space:normal;word-wrap:break-word;");

                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  grdviewAdmin_RowDataBound: " + ex.Message, TraceSeverity.Unexpected);
            }
        }

        protected void GridApproval_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                System.Web.UI.WebControls.Label LabelItemPath = (System.Web.UI.WebControls.Label)e.Row.Cells[0].FindControl("lblComments");
            }
        }

        private void BindProjectTeamRoles(string SiteUrl, int ProjectId)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite site = new SPSite(SiteUrl))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            SPList list = web.Lists["Lookup_ProjectTeamData_List"];
                            SPQuery Teamquery = new SPQuery();
                            Teamquery.Query = "<Where><Eq><FieldRef Name='ProjectId' /><Value Type='Number'>" + ProjectId + "</Value></Eq></Where>";
                            SPListItemCollection allitems = list.GetItems(Teamquery);
                            DataTable dt = new DataTable();
                            dt = allitems.GetDataTable();
                            DataTable dtProjectTeam = new DataTable();
                            DataColumn dcID = new DataColumn("IDProjectTeam", typeof(int));

                            dcID.AutoIncrement = true;
                            dcID.AutoIncrementSeed = 0;
                            dcID.AutoIncrementStep = 1;
                            DataColumn dcTitle = new DataColumn("Title", typeof(string));
                            DataColumn dcTeamMember = new DataColumn("TeamMember", typeof(string));
                            DataColumn dcTeamRole = new DataColumn("TeamRole", typeof(string));
                            DataColumn dcMemberID = new DataColumn("MemberID", typeof(int));
                            DataColumn dcEmailSent = new DataColumn("EmailSent", typeof(string));
                            DataColumn dcPercentage = new DataColumn("Percentage", typeof(string));
                            DataColumn dcdepartment = new DataColumn("Department", typeof(string));
                            dtProjectTeam.Columns.Add(dcID);
                            dtProjectTeam.Columns.Add(dcTitle);
                            dtProjectTeam.Columns.Add(dcTeamMember);
                            dtProjectTeam.Columns.Add(dcTeamRole);
                            dtProjectTeam.Columns.Add(dcMemberID);
                            dtProjectTeam.Columns.Add(dcEmailSent);
                            dtProjectTeam.Columns.Add(dcPercentage);
                            dtProjectTeam.Columns.Add(dcdepartment);
                            if (dt == null)
                            {
                                DataRow row = dtProjectTeam.NewRow();
                                dtProjectTeam.Rows.Add(row);
                                dtProjectTeam.AcceptChanges();
                                GridProjectTeam.DataSource = dtProjectTeam;
                                GridProjectTeam.DataBind();
                                GridProjectTeam.Rows[0].Style["Display"] = "none";
                            }
                            else
                            {
                                DataTable ProjectTeamTable = (DataTable)ViewState["ProjectTeamTable"];
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    DataRow row = dtProjectTeam.NewRow();
                                    row["Title"] = dt.Rows[i]["Title"];
                                    row["TeamMember"] = dt.Rows[i]["TeamMember"];
                                    row["TeamRole"] = dt.Rows[i]["TeamRole"];
                                    row["MemberID"] = dt.Rows[i]["MemberID"];
                                    row["EmailSent"] = dt.Rows[i]["EmailSent"];
                                    row["Percentage"] = dt.Rows[i]["Percentage"];
                                    row["Department"] = dt.Rows[i]["Department"];
                                    dtProjectTeam.Rows.Add(row);
                                }
                                dtProjectTeam.AcceptChanges();
                                ViewState["ProjectTeamTable"] = dtProjectTeam;
                                GridProjectTeam.DataSource = dtProjectTeam;
                                GridProjectTeam.DataBind();
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error on BindProjectTeamRoles: " + ex.Message, TraceSeverity.Unexpected);
            }
        }

        private void BindSecondAttchmentGrid(string SiteUrl, int ProjectId)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite site = new SPSite(SiteUrl))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            SPList list = web.Lists["Lookup_OtherAttachments_List"];
                            SPQuery AttachmentQuery = new SPQuery();
                            AttachmentQuery.Query = "<Where><Eq><FieldRef Name='SigmaId' /><Value Type='Number'>" + ProjectId + "</Value></Eq></Where>";
                            SPListItemCollection allitems = list.GetItems(AttachmentQuery);
                            DataTable dt = new DataTable();
                            dt = allitems.GetDataTable();
                            DataTable dtSecAttachment = new DataTable();
                            DataColumn dcID = new DataColumn("IDAttachment", typeof(int));
                            dcID.AutoIncrement = true;
                            dcID.AutoIncrementSeed = 0;
                            dcID.AutoIncrementStep = 1;
                            DataColumn dcLinkName = new DataColumn("LinkName", typeof(string));
                            DataColumn dcLinkURL = new DataColumn("LinkURL", typeof(string));
                            dtSecAttachment.Columns.Add(dcID);
                            dtSecAttachment.Columns.Add(dcLinkName);
                            dtSecAttachment.Columns.Add(dcLinkURL);

                            if (dt == null)
                            {
                                DataRow row = dtSecAttachment.NewRow();
                                dtSecAttachment.Rows.Add(row);
                                dtSecAttachment.AcceptChanges();
                                GridAttachmentSecond.DataSource = dtSecAttachment;
                                GridAttachmentSecond.DataBind();
                                GridAttachmentSecond.Rows[0].Style["Display"] = "none";
                            }
                            else
                            {
                                DataTable AttachemntTableData = (DataTable)ViewState["SecondAttachemntTable"];
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    DataRow row = dtSecAttachment.NewRow();
                                    row["LinkName"] = dt.Rows[i]["LinkName"];
                                    string LinkURL = Convert.ToString(dt.Rows[i]["LinkURL"]).Split(',')[0];
                                    row["LinkURL"] = LinkURL;
                                    dtSecAttachment.Rows.Add(row);
                                }
                                dtSecAttachment.AcceptChanges();
                                ViewState["SecondAttachemntTable"] = dtSecAttachment;
                                GridAttachmentSecond.DataSource = dtSecAttachment;
                                GridAttachmentSecond.DataBind();
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  BindProjectTeamRoles: " + ex.Message, TraceSeverity.Unexpected);
            }
        }

        private void bindDiscussionGridTable(int id)
        {
            // string id = Request.QueryString["MeetingID"];
            SPList disList = SPContext.Current.Web.Lists["BreakThroughProcertProjectsTrackingDiscussions"];
            if (disList != null)
            {
                SPQuery query = new SPQuery();
                query.Query = "<Where><Eq><FieldRef Name='SigmaId' /><Value Type='Number'>" + id + "</Value></Eq></Where>";  // replace with id and change coloumn
                SPListItemCollection itemColl = disList.GetItems(query);
                if (itemColl.Count > 0)
                {
                    SPQuery ase = new SPQuery();
                    ase.Folder = itemColl[0].Folder;
                    SPListItemCollection colls = disList.GetItems(ase);
                    DataTable dt = new DataTable();
                    DataColumn dcID = new DataColumn("ID1", typeof(int));

                    dcID.AutoIncrement = true;
                    dcID.AutoIncrementSeed = 0;
                    dcID.AutoIncrementStep = 1;

                    dt.Columns.Add("ID");
                    dt.Columns.Add("Author");
                    dt.Columns.Add("Created");
                    dt.Columns.Add("reply");
                    dt.Columns.Add("ImageURL");
                    dt.Columns.Add(dcID);

                    dt.AcceptChanges();
                    SPListItem itemwa;
                    for (int i = colls.Count - 1; i >= 0; i--)
                    {
                        itemwa = colls[i];
                        {
                            if (!String.IsNullOrEmpty(Convert.ToString(itemwa["Body"])))
                            {
                                DataRow dr = dt.NewRow();
                                dr["ID"] = Convert.ToString(itemwa["ID"]);
                                dr["reply"] = Convert.ToString(itemwa["Body"]);
                                dr["Author"] = Convert.ToString(itemwa["Author"]).Split('#')[1];
                                dr["Created"] = Convert.ToString(itemwa["Created"]);
                                dr["ImageURL"] = GetUserProfilePictureURL(Convert.ToString(itemwa["Author"]));
                                dt.Rows.Add(dr);
                            }
                        }
                    }

                    dt.AcceptChanges();
                    ViewState["DiscussionGrid"] = dt;
                    if (dt.Rows.Count > 0)
                    {
                        discussion.Visible = true;
                        discussion.DataSource = dt;
                        discussion.DataBind();

                        btnDeleteAllPost.Visible = false;

                        if (dt.Rows.Count > 5)
                        {
                            dvdiscussion.Style["height"] = "350px";
                        }
                        else
                        {
                            dvdiscussion.Style["height"] = "";
                        }
                    }
                    else
                    {
                        discussion.Visible = false;
                        btnDeleteAllPost.Visible = false;
                        dvdiscussion.Style["height"] = "";
                    }
                }
                else
                {
                    discussion.Visible = false;
                }
            }

        }

        protected void DeleteAllPost(object sender, EventArgs e)
        {

            SPSecurity.RunWithElevatedPrivileges(delegate
            {

                SPList disList = SPContext.Current.Web.Lists["BreakThroughProcertProjectsTrackingDiscussions"];
                TddeletePost.Style["Display"] = "";
                SPContext.Current.Web.AllowUnsafeUpdates = true;

                SPQuery Query = new SPQuery();
                //    Query.Query = "<Where><Eq><FieldRef Name='AgendaId' /><Value Type='Number'>" + Convert.ToInt32(HiddenAgendaId) + "</Value></Eq></Where>"; // replace with id and change coloumn
                //Query.Folder = TotalItems[i].Folder;
                SPListItemCollection AllItems = disList.GetItems(Query);

                SPQuery query = new SPQuery();
                query.Folder = AllItems[0].Folder;

                SPListItemCollection collection = disList.GetItems(query);

                int collectionCount = collection.Count;

                for (int j = 0; j < collectionCount; j++)
                {
                    collection.Delete(0);

                }
                disList.Update();
                SPContext.Current.Web.AllowUnsafeUpdates = false;
            });

            // bindDataTable(Convert.ToInt32(HiddenAgendaId));

        }

        private string GetUserProfilePictureURL(string username)
        {
            string Url = string.Empty;
            try
            {
                
                SPServiceContext serviceContext = SPServiceContext.GetContext(SPContext.Current.Site);
                SPFieldUserValue value = new SPFieldUserValue(SPContext.Current.Web, username);
                if (value.User != null)
                {
                    UserProfileManager upm = new UserProfileManager(serviceContext);
                    UserProfile profile = upm.GetUserProfile(value.User.LoginName);
                    if (profile != null)
                    {
                        Url = Convert.ToString(profile["PictureURL"].Value);
                    }
                }

                if (string.IsNullOrEmpty(Url))
                    Url = SPContext.Current.Web.Url + "/_layouts/15/PWC.Process.SixSigma/images/Thumb.jpg";
                return Url;
                
            }
            catch(Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  BindProjectTeamRoles: " + ex.Message, TraceSeverity.Unexpected);
                return Url;
            }
        }

        public void AddAutoFeed(string AddText, int sigmaId)
        {
            SPList disList = SPContext.Current.Web.Lists["BreakThroughProcertProjectsTrackingDiscussions"];
            if (disList != null)
            {
                SPQuery query = new SPQuery();
                query.Query = "<Where><Eq><FieldRef Name='SigmaId' /><Value Type='Number'>" + Convert.ToInt32(sigmaId) + "</Value></Eq></Where>"; // replace with id and change coloumn
                SPListItemCollection itemColl = disList.GetItems(query);
                if (itemColl.Count > 0)
                {
                    SPListItem reply = SPUtility.CreateNewDiscussionReply(itemColl[0]);
                    reply["Body"] = AddText.Replace("\r\n", "<br/>"); ;
                    reply.Update();
                }
                bindDiscussionGridTable(sigmaId);
            }
        }

        protected void Bt_AddReply_Click(object sender, EventArgs e)
        {
            SelectedTab.Value = "1";
            if (!String.IsNullOrEmpty(TB_Reply.Text))
            {
                SPList disList = SPContext.Current.Web.Lists["BreakThroughProcertProjectsTrackingDiscussions"];
                if (disList != null)
                {
                    SPQuery query = new SPQuery();
                    query.Query = "<Where><Eq><FieldRef Name='SigmaId' /><Value Type='Number'>" + Convert.ToInt32(sigmaId) + "</Value></Eq></Where>"; // replace with id and change coloumn
                    SPListItemCollection itemColl = disList.GetItems(query);
                    if (itemColl.Count > 0)
                    {
                        SPListItem reply = SPUtility.CreateNewDiscussionReply(itemColl[0]);
                        reply["Body"] = TB_Reply.Text.Replace("\r\n", "<br/>"); ;
                        reply.Update();
                    }
                    bindDiscussionGridTable(Convert.ToInt32(sigmaId));
                }
            }
            TB_Reply.Text = "";
            ScriptManager.RegisterClientScriptBlock(updatePanel, updatePanel.GetType(), "hideloading", "unFreeze();", true);
        }

        protected void btnSixSigmaSave_Click(object sender, EventArgs e)
        {
            //SelectedTab.Value = "0";
            SaveSixSigmaFormData("Draft", "Saved", "Originator");
        }

        protected void Bt_CommentsOk_Click(object sender, EventArgs e)
        {
            //ScriptManager.RegisterClientScriptBlock(updatePanel, updatePanel.GetType(), "loadFreezeScreen", "FreezeScreen();", true);
            try
            {
                switch (ProjectCommentsFlag.Value)
                {
                    case "btnProjectAuthorization": UpdateSixSigmaWithComments("Awaiting Project Authorization by Project Sponsor", "Submitted", "Originator");
                        break;
                    case "btnSponsorApproval": UpdateSixSigmaWithComments("Awaiting Project Authorization by Black Belt", "Approved", "Sponsor");
                        break;
                    case "btnUnlockform": UpdateSixSigmaWithComments("Edit Exception", "Unlocked", "Black Belt");
                        break;
                    case "btnlockform": UpdateSixSigmaWithComments("Edit Exception", "Locked", "Black Belt");
                        break;
                    case "btnEditCompleted": UpdateSixSigmaWithComments("Edit Exception", "Edit Completed", "Green Belt");
                        break;
                    case "btnBBApproval": UpdateSixSigmaWithComments("Define", "Approved", "Black Belt");
                        break;
                    case "btnDefineRequestApproval": UpdateSixSigmaWithComments("Awaiting Define Gate Black Belt Approval", "Submitted", "Green Belt");
                        break;
                    case "btnDefineBBApproval": UpdateSixSigmaWithComments("Measure", "Approved", "Black Belt");
                        break;
                    case "btnMeasureRequestApproval": UpdateSixSigmaWithComments("Awaiting Measure Gate Black Belt Approval", "Submitted", "Green Belt");
                        break;
                    case "btnMeasureBBApproval": UpdateSixSigmaWithComments("Analyze", "Approved", "Black Belt");
                        break;
                    case "btnAnalyzeRequestApproval": UpdateSixSigmaWithComments("Awaiting Analyze Gate Black Belt Approval", "Submitted", "Green Belt");
                        break;
                    case "btnAnalyzeBBApproval": UpdateSixSigmaWithComments("Improve", "Approved", "Black Belt");
                        break;
                    case "btnInvestigateRequestApproval": UpdateSixSigmaWithComments("Awaiting Improve Gate Black Belt Approval", "Submitted", "Green Belt");
                        break;
                    case "btnInvestigateBBApproval": UpdateSixSigmaWithComments("Control", "Approved", "Black Belt");
                        break;
                    case "btnControlRequestApproval": UpdateSixSigmaWithComments("Awaiting Control Gate Black Belt Approval", "Submitted", "Green Belt");
                        break;
                    case "btnControlBBApproval": UpdateSixSigmaWithComments("Final Report Preparation", "Approved", "Black Belt");
                        break;
                    case "btnFinalreportRequestApproval": UpdateSixSigmaWithComments("Awaiting Final Report Black Belt Approval", "Submitted", "Green Belt");
                        break;
                    case "btnFinalreportBBApproval": UpdateSixSigmaWithComments("Final Report Approved", "Approved", "Black Belt");
                        break;
                    case "btnReturnProjectLead": ReturnedSixSigmaWithComments();
                        break;
                    case "btnDefineReturntoProjectlead": ReturnedSixSigmaWithComments();
                        break;
                    case "btnMeasureReturntoProjectlead": ReturnedSixSigmaWithComments();
                        break;
                    case "btnAnalyzeReturntoProjectlead": ReturnedSixSigmaWithComments();
                        break;
                    case "btnInvestigateReturntoProjectlead": ReturnedSixSigmaWithComments();
                        break;
                    case "btnControlReturntoProjectlead": ReturnedSixSigmaWithComments();
                        break;
                    case "btnFinalreportReturntoProjectlead": ReturnedSixSigmaWithComments();
                        break;
                }

            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  Bt_CommentsOk_Click: " + ex.Message, TraceSeverity.Unexpected);
            }
        }

        protected void Click_Close(object sender, EventArgs e)
        {
            if (ViewState["SourceUrl"] != null)
            {
                Response.Redirect(Convert.ToString(ViewState["SourceUrl"]));
            }
            else
            {
                string WelcomePage = SPContext.Current.Web.RootFolder.WelcomePage;
                Response.Redirect(SPContext.Current.Web.Url + "/" + WelcomePage);
            }
        }

        private void ReturnedSixSigmaWithComments()
        {
            string user = string.Empty;
            try
            {
                using (SPSite oSite = new SPSite(SiteUrl, SPUserToken.SystemAccount))
                {
                    using (SPWeb oWeb = oSite.OpenWeb())
                    {
                        SPList SixSigmadata = oWeb.Lists[sixSigmaListName];
                        SPListItem oSPListItem = GeSixSigmaDataByID(sigmaId);
                        string status = Convert.ToString(oSPListItem["ProjectStatus"]);
                        string PrviousWorkFlowLogs = Convert.ToString(oSPListItem["WorkFlowHistoryLogs"]);
                        string[] split = { "|##|" };
                        switch (status)
                        {
                            case "Awaiting Project Authorization by Project Sponsor": oSPListItem["ProjectStatus"] = "Draft"; user = "Sponsor";
                                oSPListItem["WorkFlowHistoryLogs"] = "";
                                status = "Return to Project Lead-Project Sponsor::Draft";
                                break;
                            case "Awaiting Project Authorization by Black Belt": oSPListItem["ProjectStatus"] = "Draft"; user = "Black Belt";
                                oSPListItem["WorkFlowHistoryLogs"] = "";
                                status = "Return to Project Lead-Black Belt::Draft";
                                break;
                            case "Define": oSPListItem["ProjectStatus"] = "Awaiting Project Authorization by Black Belt"; user = "Green Belt";
                                oSPListItem["WorkFlowHistoryLogs"] = PrviousWorkFlowLogs.Substring(PrviousWorkFlowLogs.Split(split, StringSplitOptions.RemoveEmptyEntries)[0].Length, PrviousWorkFlowLogs.Length - PrviousWorkFlowLogs.Split(split, StringSplitOptions.RemoveEmptyEntries)[0].Length);
                                status = "Return to Project Lead-Gates::Awaiting Project Authorization by Black Belt";
                                break;
                            case "Awaiting Define Gate Black Belt Approval": oSPListItem["ProjectStatus"] = "Define"; user = "Black Belt";
                                oSPListItem["WorkFlowHistoryLogs"] = PrviousWorkFlowLogs.Substring(PrviousWorkFlowLogs.Split(split, StringSplitOptions.RemoveEmptyEntries)[0].Length, PrviousWorkFlowLogs.Length - PrviousWorkFlowLogs.Split(split, StringSplitOptions.RemoveEmptyEntries)[0].Length);
                                status = "Return to Project Lead-Gates::Define";
                                break;
                            case "Measure": oSPListItem["ProjectStatus"] = "Awaiting Define Gate Black Belt Approval"; user = "Green Belt";
                                oSPListItem["WorkFlowHistoryLogs"] = PrviousWorkFlowLogs.Substring(PrviousWorkFlowLogs.Split(split, StringSplitOptions.RemoveEmptyEntries)[0].Length, PrviousWorkFlowLogs.Length - PrviousWorkFlowLogs.Split(split, StringSplitOptions.RemoveEmptyEntries)[0].Length);
                                status = "Return to Project Lead-Gates::Awaiting Define Gate Black Belt Approval";
                                break;
                            case "Awaiting Measure Gate Black Belt Approval": oSPListItem["ProjectStatus"] = "Measure"; user = "Black Belt";
                                oSPListItem["WorkFlowHistoryLogs"] = PrviousWorkFlowLogs.Substring(PrviousWorkFlowLogs.Split(split, StringSplitOptions.RemoveEmptyEntries)[0].Length, PrviousWorkFlowLogs.Length - PrviousWorkFlowLogs.Split(split, StringSplitOptions.RemoveEmptyEntries)[0].Length);
                                status = "Return to Project Lead-Gates::Measure";
                                break;

                            case "Analyze": oSPListItem["ProjectStatus"] = "Awaiting Analyze Gate Black Belt Approval"; user = "Green Belt";
                                oSPListItem["WorkFlowHistoryLogs"] = PrviousWorkFlowLogs.Substring(PrviousWorkFlowLogs.Split(split, StringSplitOptions.RemoveEmptyEntries)[0].Length, PrviousWorkFlowLogs.Length - PrviousWorkFlowLogs.Split(split, StringSplitOptions.RemoveEmptyEntries)[0].Length);
                                status = "Return to Project Lead-Gates::Awaiting Analyze Gate Black Belt Approval";
                                break;
                            case "Awaiting Analyze Gate Black Belt Approval": oSPListItem["ProjectStatus"] = "Analyze"; user = "Black Belt";
                                oSPListItem["WorkFlowHistoryLogs"] = PrviousWorkFlowLogs.Substring(PrviousWorkFlowLogs.Split(split, StringSplitOptions.RemoveEmptyEntries)[0].Length, PrviousWorkFlowLogs.Length - PrviousWorkFlowLogs.Split(split, StringSplitOptions.RemoveEmptyEntries)[0].Length);
                                status = "Return to Project Lead-Gates::Analyze";
                                break;
                            case "Improve": oSPListItem["ProjectStatus"] = "Awaiting Measure Gate Black Belt Approval"; user = "Green Belt";
                                oSPListItem["WorkFlowHistoryLogs"] = PrviousWorkFlowLogs.Substring(PrviousWorkFlowLogs.Split(split, StringSplitOptions.RemoveEmptyEntries)[0].Length, PrviousWorkFlowLogs.Length - PrviousWorkFlowLogs.Split(split, StringSplitOptions.RemoveEmptyEntries)[0].Length);
                                status = "Return to Project Lead-Gates::Awaiting Measure Gate Black Belt Approval";
                                break;
                            case "Awaiting Improve Gate Black Belt Approval": oSPListItem["ProjectStatus"] = "Improve"; user = "Black Belt";
                                oSPListItem["WorkFlowHistoryLogs"] = PrviousWorkFlowLogs.Substring(PrviousWorkFlowLogs.Split(split, StringSplitOptions.RemoveEmptyEntries)[0].Length, PrviousWorkFlowLogs.Length - PrviousWorkFlowLogs.Split(split, StringSplitOptions.RemoveEmptyEntries)[0].Length);
                                status = "Return to Project Lead-Gates::Improve";
                                break;
                            case "Control": oSPListItem["ProjectStatus"] = "Awaiting Improve Gate Black Belt Approval"; user = "Green Belt";
                                oSPListItem["WorkFlowHistoryLogs"] = PrviousWorkFlowLogs.Substring(PrviousWorkFlowLogs.Split(split, StringSplitOptions.RemoveEmptyEntries)[0].Length, PrviousWorkFlowLogs.Length - PrviousWorkFlowLogs.Split(split, StringSplitOptions.RemoveEmptyEntries)[0].Length);
                                status = "Return to Project Lead-Gates::Awaiting Improve Gate Black Belt Approval";
                                break;
                            case "Awaiting Control Gate Black Belt Approval": oSPListItem["ProjectStatus"] = "Control"; user = "Black Belt";
                                oSPListItem["WorkFlowHistoryLogs"] = PrviousWorkFlowLogs.Substring(PrviousWorkFlowLogs.Split(split, StringSplitOptions.RemoveEmptyEntries)[0].Length, PrviousWorkFlowLogs.Length - PrviousWorkFlowLogs.Split(split, StringSplitOptions.RemoveEmptyEntries)[0].Length);
                                status = "Return to Project Lead-Gates::Control";
                                break;
                            case "Final Report Preparation": oSPListItem["ProjectStatus"] = "Awaiting Control Gate Black Belt Approval"; user = "Green Belt";
                                oSPListItem["WorkFlowHistoryLogs"] = PrviousWorkFlowLogs.Substring(PrviousWorkFlowLogs.Split(split, StringSplitOptions.RemoveEmptyEntries)[0].Length, PrviousWorkFlowLogs.Length - PrviousWorkFlowLogs.Split(split, StringSplitOptions.RemoveEmptyEntries)[0].Length);
                                status = "Return to Project Lead-Gates::Awaiting Control Gate Black Belt Approval";
                                break;
                            case "Awaiting Final Report Black Belt Approval": oSPListItem["ProjectStatus"] = "Final Report Preparation"; user = "Black Belt";
                                oSPListItem["WorkFlowHistoryLogs"] = PrviousWorkFlowLogs.Substring(PrviousWorkFlowLogs.Split(split, StringSplitOptions.RemoveEmptyEntries)[0].Length, PrviousWorkFlowLogs.Length - PrviousWorkFlowLogs.Split(split, StringSplitOptions.RemoveEmptyEntries)[0].Length);
                                status = "Return to Project Lead-Gates::Final Report Preparation";
                                break;
                        }
                        string PrviousActionLogs = Convert.ToString(oSPListItem["ProjectOverAllComments"]);
                        SPUser ProjectSponsersUsers = GetUser(oWeb, projectSponserUserEditor);
                        SPUser ProjectBBUsers = GetUser(oWeb, BlackbeltuserEditor);
                        SPUser ProjectGBUsers = GetUser(oWeb, GreenbeltuserEditor);
                        oSPListItem["ProjectOverAllComments"] = Environment.NewLine + "Return to Project Lead" + "|" + SPContext.Current.Web.CurrentUser.Name + " | " + DateTime.Now + "|" + txtreturntooriginator.Text + "|" + "" + "|" + user + "|##|" + PrviousActionLogs;
                        //SentStatusBasedEmail(status, txtProjectName.Text, ProjectBBUsers, ProjectGBUsers, ProjectSponsersUsers, txtreturntooriginator.Text, oWeb);
                        oSPListItem.Update();
                        SentStatusBasedEmail(status, txtProjectName.Text, ProjectBBUsers, ProjectGBUsers, ProjectSponsersUsers, txtreturntooriginator.Text, oWeb);
                    }
                }
                ScriptManager.RegisterClientScriptBlock(updatePanel, updatePanel.GetType(), "loadUnFreezeScreen", "unFreeze();", true);
                if (ViewState["SourceUrl"] != null)
                {
                    Response.Redirect(Convert.ToString(ViewState["SourceUrl"]));
                }
                else
                {
                    string WelcomePage = SPContext.Current.Web.RootFolder.WelcomePage;
                    Response.Redirect(SPContext.Current.Web.Url + "/" + WelcomePage);
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  UpdateSixSigmaWithComments() " + ex.Message, TraceSeverity.Unexpected);
            }

        }

        private void UpdateSixSigmaWithComments(string Status, string Action, string user)
        {
            ConcurrentSaving = false;
            SaveSixSigmaFormData(Status, Action, user);
            if (!ConcurrentSaving)
            {
                if (ViewState["SourceUrl"] != null)
                {
                    Response.Redirect(Convert.ToString(ViewState["SourceUrl"]));
                }
                else
                {
                    string WelcomePage = SPContext.Current.Web.RootFolder.WelcomePage;
                    Response.Redirect(SPContext.Current.Web.Url + "/" + WelcomePage,false);
                }
            }

        }

        protected void SaveSixSigmaFormData(string Status, string Action, string User)
        {
            try
            {
               // SPSecurity.RunWithElevatedPrivileges(delegate()
              //  {
                    using (SPSite oSite = new SPSite(SiteUrl))
                    {
                        using (SPWeb oWeb = oSite.OpenWeb())
                        {
                            string currentStatus = string.Empty;
                            SPList SixSigmadata = oWeb.Lists[sixSigmaListName];
                            SPListItem oSPListItem = null;
                            SPUser ProjectSponsersUsers = GetUser(oWeb, projectSponserUserEditor);
                            SPUser ProjectBBUsers = GetUser(oWeb, BlackbeltuserEditor);
                            SPUser ProjectGBUsers = GetUser(oWeb, GreenbeltuserEditor);
                            if (sigmaId == 0) // Item Created First time
                            {
                                oWeb.AllowUnsafeUpdates = true;
                                oSPListItem = SixSigmadata.Items.Add();
                                oSPListItem.Update();
                                sigmaId = Convert.ToInt32(oSPListItem["ID"]);
                                ViewState["SigmaId"] = sigmaId;
                               
                                //Added on 27-04-2017 by shashank
                                string sigmaID = string.Format("sigmaID = '{0}';", sigmaId);
                                ScriptManager.RegisterStartupScript(updatePanel, updatePanel.GetType(), "sigmaID", sigmaID, true);
                               
                                oSPListItem["ProjectId"] = formatProjectId(sigmaId); 

                                DisplayLinkAttachmentMessage("none");
                                //Add Discussion
                                AddDiscussion(sigmaId);

                                //Create Folder Structure
                                CreateFolderStructure(oWeb, sigmaId);

                                //Added by Sid to create attachment folder inside Project
                                CreateAttachmentFolder(oWeb, sigmaId);

                                string[] FolderUrls = { "Info/Background", "Info/Problem Statement", "Info/Project Metrics", "Info/Benefits", "Info/Costs", "Info/Financial Analysis", "Info/Milestones", "Gates/Define", "Gates/Measure", "Gates/Analyze", "Gates/Improve", "Gates/Control", "Final Report" };
                                string[] colName = { "Bacground_x0020_Attachments", "Problem_x0020_Attachments", "ProjectMetrics_x0020_Attachments", "Benifits_x0020_Attachments", "Costs_x0020_Attachments", "Financial_x0020_Attachments", "Milestones_x0020_Attachments", "Define_x0020_Attachments", "Measure_x0020_Attachments", "Analyze_x0020_Attachments", "Investigate_x0020_Attachments", "Control_x0020_Attachments", "FinalReport_x0020_Attachments" };
                                for (int i = 0; i < colName.Length; i++)
                                {
                                    SPFolder folder = oWeb.GetFolder(oWeb.ServerRelativeUrl + "Documents/Project" + sigmaId + "/" + FolderUrls[i]);
                                    SPFileCollection coll = folder.Files;
                                    string text = string.Empty;
                                    string sharePointNewLine = "<br/>";
                                    foreach (SPFile file in coll)
                                    {
                                        text = text + "<a href='" + oWeb.Url + "/" + file.Item.Url + "'>" + oWeb.Url + "/" + file.Item.Url + "</a>" + sharePointNewLine;

                                    }
                                    oSPListItem[colName[i]] = text;
                                }
                                SPFieldUrlValue url = new SPFieldUrlValue();
                                url.Description = formatProjectId(sigmaId);
                                string _id = HttpUtility.UrlEncode(Encrypt(Convert.ToString(sigmaId)));
                                url.Url = CurrentSiteUrl + "/SitePages/BreakThroughProcertProjectsTracking.aspx?ProjectId=" + _id;
                                oSPListItem["ItemUrl"] = url;
                               // oSPListItem["Editor"] = SPContext.Current.Web.CurrentUser;
                                oSPListItem.Update();
                                // Setting Supporting Attachments link URL
                                SetSupportingAttachmentsURL();
                            }
                            else
                            {
                                oSPListItem = GeSixSigmaDataByID(sigmaId);
                                // Setting Supporting Attachments link URL
                                SetSupportingAttachmentsURL();
                            }
                            oSPListItem = GeSixSigmaDataByID(sigmaId);

                            SPField pplField = oSPListItem.Fields["Modified By"];
                            SPFieldUserValue fieldValue = (SPFieldUserValue)pplField.GetFieldValue(Convert.ToString(oSPListItem["Modified By"]));
                            if (ViewState["formOpenTime"] != null && Convert.ToDateTime(Convert.ToDateTime(oSPListItem["Modified"])) > (DateTime)ViewState["formOpenTime"] && !(SPContext.Current.Web.CurrentUser.ID.Equals(fieldValue.LookupId)))
                            {
                                ScriptManager.RegisterClientScriptBlock(updatePanel, updatePanel.GetType(), "loadUnFreezeScreenAlreadySaved", "unFreeze();", true);
                                ScriptManager.RegisterClientScriptBlock(updatePanel, updatePanel.GetType(), "AlreadySaved", "javascript:alert('Form is updated by other user - please close and re-open again.');", true);
                                ConcurrentSaving = true;
                            }
                            else
                            {
                                if (projectSponserUserEditor.Enabled != false)
                                {
                                    if (ProjectSponsersUsers != null)
                                    {
                                        oSPListItem["ProjectSponsor"] = ProjectSponsersUsers;
                                    }
                                }
                              //  if (BlackbeltuserEditor.Enabled != false)
                              //  {
                                    if (ProjectBBUsers != null)
                                    {
                                        oSPListItem["ProjectBBUsers"] = ProjectBBUsers;
                                    }
                              //  }
                              //  if (GreenbeltuserEditor.Enabled != false)
                              //  {
                                    if (ProjectGBUsers != null)
                                    {
                                        oSPListItem["ProjectGBUsers"] = ProjectGBUsers;
                                    }
                              //  }
                                if (Status == "Draft" && Convert.ToString(oSPListItem["ProjectStatus"]) != "" && Convert.ToString(oSPListItem["ProjectStatus"]) != "Draft")
                                {
                                    Status = Convert.ToString(oSPListItem["ProjectStatus"]);
                                    User = "Green Belt";
                                }
                                else if (Action == "Unlocked")
                                {
                                    oSPListItem["lockunlockstatus"] = BasicInfoStatus.Text;
                                    oSPListItem.Update();
                                }
                                else
                                {
                                    currentStatus = Convert.ToString(oSPListItem["ProjectStatus"]);
                                }

                                oSPListItem["ProjectStatus"] = Status;

                                if (Action == "Locked")
                                {
                                    oSPListItem["ProjectStatus"] = Convert.ToString(oSPListItem["lockunlockstatus"]);
                                    oSPListItem.Update();
                                }

                                oSPListItem["ProjectName"] = txtProjectName.Text;
                                oSPListItem["ProjectOrganization"] = ddlorgnisation.SelectedItem.Text;
                                oSPListItem["ProjectPlan"] = ddlplant.SelectedItem.Text;
                                oSPListItem["ProjectType"] = ddlprojecttype.SelectedItem.Text;
                                oSPListItem["ProjectBackground"] = txtBackground.Text;
                                oSPListItem["ProjectProblemStatement"] = txtProjectstatementobj.Text;
                                oSPListItem["ProjectBenefits"] = txtBenefits.Text;
                                oSPListItem["ProjectCosts"] = txtcosts.Text;

                                // Metrics Section
                                oSPListItem["AreaCost"] = ddlMetricCost.SelectedItem.Text;
                                oSPListItem["AreaQuality"] = ddlQualityMetrics.SelectedItem.Text;
                                oSPListItem["AreaDelivery"] = ddlDeliveryMetrics.SelectedItem.Text;
                                oSPListItem["AreaOther"] = ddlothermetric.SelectedItem.Text;
                                oSPListItem["AreaOther1"] = ddlothermetric1.SelectedItem.Text;

                                oSPListItem["CostMetrics"] = txtmetriccost.Text;
                                oSPListItem["QualityMetrics"] = txtmetricquality.Text;
                                oSPListItem["DeliveryMetrics"] = txtmetricdelivery.Text;
                                oSPListItem["OtherMetric"] = txtmetricother.Text;
                                oSPListItem["OtherMetric1"] = txtmetricother1.Text;






                                oSPListItem["CostBaseline"] = txtCostBaseline.Text;
                                oSPListItem["CostGoal"] = txtCostGoal.Text;
                                oSPListItem["QualityBaseline"] = txtQualityBaseline.Text;
                                oSPListItem["QualityGoal"] = txtQualityGoal.Text;
                                oSPListItem["DeliveryBaseline"] = txtDeliveryBaseline.Text;
                                oSPListItem["DeliveryGoal"] = txtDeliveryGoal.Text;
                                oSPListItem["OtherBaseline"] = txtotherbaseline.Text;
                                oSPListItem["OtherGoal"] = txtothergoal.Text;
                                oSPListItem["OtherBaseline1"] = txtotherbaseline1.Text;
                                oSPListItem["OtherGoal1"] = txtothergoal1.Text;


                                if (!string.IsNullOrEmpty(txtplannedActualCost.Text))
                                {
                                    oSPListItem["PlannedFinancialCost"] = Convert.ToInt32(txtplannedActualCost.Text);
                                }

                                if (!string.IsNullOrEmpty(txtplannedActualBenefits.Text))
                                {
                                    oSPListItem["PalnnedFinancialBenefits"] = Convert.ToInt32(txtplannedActualBenefits.Text);
                                }

                                if (!string.IsNullOrEmpty(txtActualCost.Text))
                                {
                                    oSPListItem["ActualFinancialCost"] = Convert.ToInt32(txtActualCost.Text);
                                }


                                if (!string.IsNullOrEmpty(txtActualbenefits.Text))
                                {
                                    oSPListItem["ActualFinancialBenefits"] = Convert.ToInt32(txtActualbenefits.Text);
                                }

                                string strOriginalTerms = taxTags.Text;
                                if (!string.IsNullOrEmpty(strOriginalTerms) || strOriginalTerms.Length > 1)
                                {
                                    oSPListItem["Tags"] = GetTaxonomyValue(SixSigmadata, "Tags", taxTags.Text);
                                }

                                //Financial Analysis

                               

                                // Milestones section
                                if (!PlandateProjectAuthorization.IsDateEmpty)
                                    oSPListItem["PlannedProjectAuthorizationDate"] = PlandateProjectAuthorization.SelectedDate;
                                if (!PlandateDefine.IsDateEmpty)
                                    oSPListItem["PlannedDefineDate"] = PlandateDefine.SelectedDate;
                                if (!PlandateMeasure.IsDateEmpty)
                                    oSPListItem["PlannedMeasureDate"] = PlandateMeasure.SelectedDate;
                                if (!PlandateAnalyze.IsDateEmpty)
                                    oSPListItem["PlannedAnalyzeDate"] = PlandateAnalyze.SelectedDate;
                                if (!PlandateImprove.IsDateEmpty)
                                    oSPListItem["PlannedImproveDate"] = PlandateImprove.SelectedDate;
                                if (!PlandateControl.IsDateEmpty)
                                    oSPListItem["PlannedControlDate"] = PlandateControl.SelectedDate;
                                if (!PlandateFinalReportApprove.IsDateEmpty)
                                    oSPListItem["PlannedFinalReportApprovalDate"] = PlandateFinalReportApprove.SelectedDate;

                                // Gates Comments Section 
                                oSPListItem["DefineComment"] = txtdefineComment.Text;
                                oSPListItem["MeasureComment"] = txtMeasurecomment.Text;
                                oSPListItem["AnalyzeComment"] = txtAnalyzecomment.Text;
                                oSPListItem["InvestigateComment"] = txtinvestigatecomment.Text;
                                oSPListItem["ControlComment"] = txtControlcomment.Text;
                                oSPListItem["FinalReportComment"] = txtFinalreportcomment.Text;

                                SetCompletionDetails(oSPListItem, Status);

                                if (Status != "Draft" && Status != "Edit Exception")
                                {
                                    string PrviousWorkFlowLogs = Convert.ToString(oSPListItem["WorkFlowHistoryLogs"]);
                                    oSPListItem["WorkFlowHistoryLogs"] = Environment.NewLine + Action + "|" + SPContext.Current.Web.CurrentUser.Name + " | " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt") + "|" + txtreturntooriginator.Text + "|" + currentStatus + "|" + User + "|" + SPContext.Current.Web.CurrentUser.LoginName.Split('\\')[1] + "|##|" + PrviousWorkFlowLogs;
                                   
                                }
                                if (Status == "Draft")
                                {
                                    string PrviousActionLogs = Convert.ToString(oSPListItem["ProjectOverAllComments"]);
                                    oSPListItem["ProjectOverAllComments"] = Environment.NewLine + Action + "|" + SPContext.Current.Web.CurrentUser.Name + " | " + DateTime.Now + "|" + txtreturntooriginator.Text + "|" + "" + "|" + User + "|##|" + PrviousActionLogs;

                                }
                                else if (Action != "Saved")
                                {
                                    string PrviousActionLogs = Convert.ToString(oSPListItem["ProjectOverAllComments"]);
                                    oSPListItem["ProjectOverAllComments"] = Environment.NewLine + Action + "|" + SPContext.Current.Web.CurrentUser.Name + " | " + DateTime.Now + "|" + txtreturntooriginator.Text + "|" + "" + "|" + User + "|##|" + PrviousActionLogs;
                                }


                                oSPListItem.Update();


                                if (Action == "Edit Completed" || Action == "Locked" || Action == "Unlocked")
                                {
                                    SentStatusBasedEmail(Action, txtProjectName.Text, ProjectBBUsers, ProjectGBUsers, ProjectSponsersUsers, txtreturntooriginator.Text, oWeb);
                                }
                                else
                                {
                                    if (Action != "Saved")
                                        SentStatusBasedEmail(Status, txtProjectName.Text, ProjectBBUsers, ProjectGBUsers, ProjectSponsersUsers, txtreturntooriginator.Text, oWeb);
                                }
                                if (!string.IsNullOrEmpty(Convert.ToString(Request.QueryString["ProjectId"])))
                                {
                                    // Update Project Team Data
                                    UpdateTeamRolesData(oWeb, sigmaId);

                                    // Update Second Attachment Data 
                                    UpdateSecondAttachmentData(oWeb, sigmaId);

                                    // Bind Project team Roles
                                    BindProjectTeamRoles(SiteUrl, sigmaId);

                                    BindSecondAttchmentGrid(SiteUrl, sigmaId);
                                    // Bind Quad Charts
                                    BindQuadCharts(SiteUrl, sigmaId);

                                    // Bind Attachment Grid
                                    BindAttachmentGrid(SiteUrl, sigmaId);

                                    // Bind Action Logs
                                    BindActionLogs(oSPListItem);

                                    // Set Controls Label
                                    SetControlsLabel(oSPListItem);


                                    StatusBasedControls(Status);
                                    ScriptManager.RegisterClientScriptBlock(updatePanel, updatePanel.GetType(), "loadUnFreezeScreen", "unFreeze();", true);
                                }
                                else
                                {
                                    ScriptManager.RegisterClientScriptBlock(updatePanel, updatePanel.GetType(), "loadUnFreezeScreen", "unFreeze();", true);
                                    //ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "savealert", "javascript:(alert('Data Saved Successfully.'));", true);
                                    SPFieldUrlValue url = new SPFieldUrlValue(Convert.ToString(oSPListItem["ItemUrl"]));
                                    Response.Redirect(url.Url);
                                }
                            }
                        }

                    }
               // });
            }

            catch (Exception ex)
            {
            }
        }

        private string formatProjectId(int sigmaId)
        {
            string projectId = Convert.ToString(sigmaId).PadLeft(3, '0');
            return "PC " + projectId;
        }

        protected void AddDiscussion(int SigmaId)
        {
            try
            {

                SPList oList = SPContext.Current.Web.Lists["BreakThroughProcertProjectsTrackingDiscussions"];
                SPContext.Current.Web.AllowUnsafeUpdates = true;
                SPListItem oSPListItem = SPUtility.CreateNewDiscussion(oList, Convert.ToString(SigmaId));
                oSPListItem["SigmaId"] = SigmaId;
                //  oSPListItem["Contains Technical Data?"] = "No";
                oSPListItem.Update();
                SPContext.Current.Web.AllowUnsafeUpdates = false;
            }
            catch (Exception Ex)
            {
                ULSLogger.LogErrorInULS("Inside catch in AddDiscussion() in PWC Six Sigma Feature..Error is--" + Ex.Message);
            }


        }

        private void UpdateTeamRolesData(SPWeb oWeb, int ID)
        {
            try
            {
                DataTable ProjectTeamTable = (DataTable)ViewState["ProjectTeamTable"];

                if (ProjectTeamTable != null)
                {
                    DeleteExistingTeamRolesList(oWeb, ID);

                    foreach (DataRow row in ProjectTeamTable.Rows) // Loop over the rows.
                    {
                        AddItemsInTeamRolesList(row, ID);
                    }
                }
                else
                {
                    DeleteExistingTeamRolesList(oWeb, ID);
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error in  AddUpdateProjectInfoandSettlementsMethod" + ex.Message);
            }
        }

        protected void AddItemsInTeamRolesList(DataRow ViewStateitem, int ProjectId)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite site = new SPSite(SiteUrl))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            SPListItem item = null;
                            web.AllowUnsafeUpdates = true;
                            SPList list = web.Lists["Lookup_ProjectTeamData_List"];
                            item = list.AddItem();  
                            string LoginName = Convert.ToString(ViewStateitem[2]);
                            SPUser user = web.SiteUsers.GetByID(Convert.ToInt32(ViewStateitem[4])); // Changed from Ensure User
                            item["TeamMember"] = user;
                            item["TeamRole"] = ViewStateitem[3];
                            item["ProjectId"] = ProjectId;
                            item["MemberID"] = user.ID;
                            item["EmailSent"] = "Notification sent.";
                            item["Percentage"] = ViewStateitem[6];
                            item["Department"] = ViewStateitem[7];
                            item.Update();
                            web.AllowUnsafeUpdates = false;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error in Adding AddItemsInTeamRolesListMethod" + ex.Message);
            }
        }

        private static void DeleteExistingTeamRolesList(SPWeb oWeb, int ID)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
               {
                   oWeb.AllowUnsafeUpdates = true;
                   SPList list = oWeb.Lists["Lookup_ProjectTeamData_List"];
                   SPQuery Query = new SPQuery();
                   Query.Query = "<Where><Eq><FieldRef Name='ProjectId' /><Value Type='Number'>" + ID + "</Value></Eq></Where>";
                   SPListItemCollection itemcoll = list.GetItems(Query);
                   int itemcountcoll = itemcoll.Count;
                   for (int intIndex = itemcountcoll - 1; intIndex > -1; intIndex--)
                   {
                       itemcoll.Delete(intIndex);
                   }
                   oWeb.AllowUnsafeUpdates = false;
               });
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error in  DeleteExistingTeamRolesListMethod" + ex.Message);
            }
        }

        private void CreateFolderStructure(SPWeb oWeb, int ID)
        {
            try
            {
                SPList list = oWeb.Lists[documentsListName];
                SPFolderCollection spFolderColl = list.RootFolder.SubFolders;
                SPFolder destinationRootfolder = spFolderColl.Add("Project" + Convert.ToString(ID));
                destinationRootfolder.Item.SystemUpdate();
                SPFolder TemplateRootFolder = oWeb.GetFolder(oWeb.ServerRelativeUrl + "Documents/Templates");
                SPFileCollection fileColl = TemplateRootFolder.Files;
                copyFiles(fileColl, destinationRootfolder);
                SPFolderCollection folderColl = TemplateRootFolder.SubFolders;
                enumerateFolders(oWeb, folderColl, destinationRootfolder);
            }
            catch (Exception ex)
            {
            }
        }

        private void CreateAttachmentFolder(SPWeb oWeb, int ID)
        {
            try
            {
                SPList list = oWeb.Lists[documentsListName];
                SPFolder projectFolder = oWeb.GetFolder(oWeb.ServerRelativeUrl + "Documents/Project" + Convert.ToString(ID));
                SPFolderCollection projectFolderCol = projectFolder.SubFolders;
                projectFolderCol.Add(projectFolder.ServerRelativeUrl + "/OtherAttachments");
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Inside catch in CreateAttachmentFolder(). Error is--"+ex.Message);
            }
        }

        private void enumerateFolders(SPWeb oWeb, SPFolderCollection folderColl, SPFolder destinationRootfolder)
        {
            foreach (SPFolder subFolder in folderColl)
            {
                SPFolder fldr = oWeb.Folders.Add(subFolder.ServerRelativeUrl.Replace("Templates", destinationRootfolder.Name));
                SPFileCollection subFiles = subFolder.Files;
                if (!subFolder.Name.Contains("Quad Charts"))
                {
                    copyFiles(subFiles, fldr);
                }
                enumerateFolders(oWeb, subFolder.SubFolders, destinationRootfolder);
            }
        }

        private void copyFiles(SPFileCollection fileColl, SPFolder destinationFolder)
        {
            foreach (SPFile file in fileColl)
            {
                try
                {
                    SPListItem SrcItem = file.Item;
                    SPListItem DestItem = destinationFolder.Files.Add(file.Name, file.OpenBinary(), true).Item;
                    SPContentType ct = SrcItem.ContentType;
                    foreach (SPField field in ct.Fields)
                    {
                        if (field.CanBeDisplayedInEditForm)
                            DestItem[field.Id] = SrcItem[field.Id];
                    }
                    DestItem.UpdateOverwriteVersion();

                }
                catch (Exception ex)
                {

                }
            }
        }

        private void CreateSubFolders(string[] folderUrls, SPFolder folder)
        {
            for (int i = 0; i < folderUrls.Length; i++)
            {
                SPFolderCollection coll = folder.SubFolders;
                coll.Add(folder.ServerRelativeUrl + "/" + folderUrls[i]);
            }

        }

        public SPUser GetUser(SPWeb web, PeopleEditor pplEditor)
        {
            SPUser User = null;
            try
            {
                string UserSeperated = pplEditor.CommaSeparatedAccounts.Split(',')[0];
                User = web.EnsureUser(UserSeperated);
                //UserName = new SPFieldUserValue(web, User.ID, User.LoginName);
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On User Retrieval: " + ex.Message, TraceSeverity.Unexpected);
            }
            return User;
        }

        public TaxonomyFieldValueCollection GetTaxonomyValue(SPList list, string fieldName, string fieldValue)
        {
            string[] fieldValueParts;
            string[] fieldValuePartsColl;
            TaxonomyField taxonomyField;
            TaxonomyFieldValue taxonomyFieldValue;
            taxonomyField = list.Fields[fieldName] as TaxonomyField;
            TaxonomyFieldValueCollection taxonomyFieldValueColl=new TaxonomyFieldValueCollection (taxonomyField);
            //fieldValueParts = fieldValue.Split(TaxonomyField.TaxonomyGuidLabelDelimiter);
            fieldValuePartsColl = fieldValue.Split(TaxonomyField.TaxonomyMultipleTermDelimiter);
            foreach (string item in fieldValuePartsColl)
            {
                fieldValueParts = item.Split(TaxonomyField.TaxonomyGuidLabelDelimiter);
               // taxonomyField = list.Fields[fieldName] as TaxonomyField;
                taxonomyFieldValue = new TaxonomyFieldValue(taxonomyField);
                taxonomyFieldValue.TermGuid = fieldValueParts[1];
                taxonomyFieldValue.Label = fieldValueParts[0];
                taxonomyFieldValueColl.Add(taxonomyFieldValue);
            }


            return taxonomyFieldValueColl;
        }

        protected void BindRoles(DropDownList Role, DropDownList Percentage)
        {
            try
            {
                using (SPSite Site = new SPSite(CurrentSiteUrl, SPUserToken.SystemAccount))
                {
                    using (SPWeb web = Site.OpenWeb())
                    {
                        SPList PCSTypelist = web.Lists["Lookup_ProjectTeamRole_List"];
                        SPQuery PCSTypelistgroupQuery = new SPQuery();
                        PCSTypelistgroupQuery.Query = "<GroupBy><FieldRef Name=\"TeamRole\"/></GroupBy>";
                        SPListItemCollection PCSTypelistitems = PCSTypelist.GetItems(PCSTypelistgroupQuery);

                        foreach (SPListItem PCSTypelistitemslistlistItem in PCSTypelistitems)
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(PCSTypelistitemslistlistItem["TeamRole"])))
                            {

                                ListItem ThisItem = new ListItem();
                                ThisItem.Text = PCSTypelistitemslistlistItem["TeamRole"].ToString();
                                ThisItem.Value = PCSTypelistitemslistlistItem["TeamRole"].ToString();
                                Role.Items.Add(ThisItem);
                                Role.DataBind();


                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  BindRoles: " + ex.Message, TraceSeverity.Unexpected);
            }

        }

        protected void CopyLastItem(object sender, EventArgs e)
        {
            //Button btn = (Button)GridQuadDetails.FooterRow.FindControl("btnAddQuadCharts");
            btnMakeCopy.Enabled = false;
            //Setting Tab Value
            SelectedTab.Value = "1";
            AddNewRowToQuadCharts("Copy");
            //ScriptManager.RegisterClientScriptBlock(updatePanel, updatePanel.GetType(), "hideloading", "unFreeze();", true);
            btnMakeCopy.Enabled = true;
            hiddenFieldQuadCharts.Value = "Copy";
        }

        protected void AddQuadCharts(object sender, EventArgs e)
        {
            //Button btn = (Button)GridQuadDetails.FooterRow.FindControl("btnAddQuadCharts");
            btnAddQuadCharts.Enabled = false;
            //Setting Tab Value
            SelectedTab.Value = "1";
            AddNewRowToQuadCharts("Add");
            //ScriptManager.RegisterClientScriptBlock(updatePanel, updatePanel.GetType(), "hideloading", "unFreeze();", true);
            btnAddQuadCharts.Enabled = true;
            hiddenFieldQuadCharts.Value = "Add";
        }

        protected void AddTeamMember(object sender, EventArgs e)
        {
            //Setting Tab Value
            SelectedTab.Value = "0";
            AddNewRowToProjectTeamGrid();
        }

        private void AddNewRowToProjectTeamGrid()
        {

            try
            {
                DataTable dtCurrentTable = new DataTable();
                DataRow drCurrentRow = null;
                if (ViewState["ProjectTeamTable"] != null)
                {
                    dtCurrentTable = (DataTable)ViewState["ProjectTeamTable"];
                    dtCurrentTable.Columns["IDProjectTeam"].AutoIncrement = true;
                    dtCurrentTable.Columns["IDProjectTeam"].AutoIncrementSeed = 0;
                    dtCurrentTable.Columns["IDProjectTeam"].AutoIncrementStep = 1;
                    drCurrentRow = AddRowToProjectTeam(dtCurrentTable, drCurrentRow);
                    // drCurrentRow["IDPCS"] = dtCurrentTable.Rows.Count - 1;
                    //add new row to DataTable
                    dtCurrentTable.Rows.Add(drCurrentRow);
                    dtCurrentTable.AcceptChanges();
                    //Store the current data to ViewState
                    ViewState["ProjectTeamTable"] = dtCurrentTable;

                }
                else
                {

                    drCurrentRow = AddRowToProjectTeam(dtCurrentTable, drCurrentRow);
                    dtCurrentTable.Rows.Add(drCurrentRow);
                    dtCurrentTable.AcceptChanges();
                    ViewState["ProjectTeamTable"] = dtCurrentTable;
                }

                GridProjectTeam.DataSource = dtCurrentTable;
                GridProjectTeam.DataBind();

            }

            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  AddNewRowToProjectTeamGrid: " + ex.Message, TraceSeverity.Unexpected);
            }


        }

        private DataRow AddRowToProjectTeam(DataTable dtCurrentTable, DataRow drCurrentRow)
        {
            if (ViewState["ProjectTeamTable"] == null)
            {
                DataColumn dcID = new DataColumn("IDProjectTeam", typeof(int));
                DataColumn dcTitle = new DataColumn("Title", typeof(string));
                DataColumn dcPCSNo = new DataColumn("TeamMember", typeof(string));
                DataColumn dcPCSType = new DataColumn("TeamRole", typeof(string));
                DataColumn dcMemberID = new DataColumn("MemberID", typeof(int));
                DataColumn dcEmailSent = new DataColumn("EmailSent", typeof(string));
                DataColumn dcPercentage = new DataColumn("Percentage", typeof(string));
                DataColumn dcdepartment = new DataColumn("Department", typeof(string));
              

                dcID.AutoIncrement = true;
                dcID.AutoIncrementSeed = 0;
                dcID.AutoIncrementStep = 1;
                dtCurrentTable.Columns.Add(dcID);
                dtCurrentTable.Columns.Add(dcTitle);
                dtCurrentTable.Columns.Add(dcPCSNo);
                dtCurrentTable.Columns.Add(dcPCSType);
                dtCurrentTable.Columns.Add(dcMemberID);
                dtCurrentTable.Columns.Add(dcEmailSent);
                dtCurrentTable.Columns.Add(dcPercentage);
                dtCurrentTable.Columns.Add(dcdepartment);
                


            }
            PeopleEditor ProjectMember = (PeopleEditor)GridProjectTeam.FooterRow.FindControl("txtFooterTeamMember");
            TextBox DepartmentEditor = (TextBox)GridProjectTeam.FooterRow.FindControl("txtfooterdepartmentEditor");
            DropDownList ProjectTeamRole = (DropDownList)GridProjectTeam.FooterRow.FindControl("ddlTeamRole");
            DropDownList ProjectPercentage = (DropDownList)GridProjectTeam.FooterRow.FindControl("ddlPercentage");

            drCurrentRow = dtCurrentTable.NewRow();

            SPUser user = SPContext.Current.Web.EnsureUser(ProjectMember.CommaSeparatedAccounts);
            drCurrentRow["TeamMember"] = user.Name;
            drCurrentRow["TeamRole"] = ProjectTeamRole.SelectedItem.Text;
            drCurrentRow["MemberID"] = user.ID;
            drCurrentRow["EmailSent"] = "Notification sent.";
            drCurrentRow["Percentage"] = ProjectPercentage.SelectedItem.Text;
            drCurrentRow["Department"] = DepartmentEditor.Text;
            
            return drCurrentRow;
        }

        protected void GridProjectTeam_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void GridProjectTeam_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

            try
            {
                DataTable ProjectTeamTable = (DataTable)ViewState["ProjectTeamTable"];
                int catid = int.Parse(GridProjectTeam.DataKeys[e.RowIndex].Value.ToString());
                GridProjectTeam.EditIndex = e.RowIndex;
                int index = e.RowIndex;
                for (int r = 0; r < ProjectTeamTable.Rows.Count; r++)
                {
                    DataRow dtRow = ProjectTeamTable.Rows[r];
                    int ID = Convert.ToInt32(dtRow["IDProjectTeam"]);
                    if (catid == ID)
                    {
                        dtRow.Delete();

                    }
                }

                ProjectTeamTable.AcceptChanges();
                for (int i = 0; i < ProjectTeamTable.Rows.Count; i++)
                {

                    ProjectTeamTable.Rows[i]["IDProjectTeam"] = i;
                    ProjectTeamTable.AcceptChanges();

                }

                if (ProjectTeamTable.Rows.Count != 0)
                {
                    GridProjectTeam.EditIndex = -1;
                    GridProjectTeam.DataSource = ProjectTeamTable;
                    GridProjectTeam.DataBind();
                    ViewState["ProjectTeamTable"] = ProjectTeamTable;
                }
                else
                {
                    DataRow row = ProjectTeamTable.NewRow();
                    ProjectTeamTable.Rows.Add(row);
                    GridProjectTeam.DataSource = ProjectTeamTable;
                    GridProjectTeam.DataBind();
                    GridProjectTeam.Rows[0].Style["Display"] = "none";
                    ViewState["ProjectTeamTable"] = null;
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  GridProjectTeam_RowDeleting: " + ex.Message, TraceSeverity.Unexpected);
            }
        }

        protected void GridProjectTeam_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                DataTable ProjectTeamTable = (DataTable)ViewState["ProjectTeamTable"];
                GridProjectTeam.PageIndex = e.NewPageIndex;
                GridProjectTeam.DataSource = ProjectTeamTable;
                GridProjectTeam.DataBind();

            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  GridProjectTeam_PageIndexChanging: " + ex.Message, TraceSeverity.Unexpected);
            }

        }

        protected void GridProjectTeam_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                GridProjectTeam.EditIndex = -1;
                DataTable ProjectTeamTable = (DataTable)ViewState["ProjectTeamTable"];
                GridProjectTeam.DataSource = ProjectTeamTable;
                GridProjectTeam.DataBind();
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  GridProjectTeam_RowCancelingEdit: " + ex.Message, TraceSeverity.Unexpected);
            }
        }

        protected void GridProjectTeam_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {

                GridProjectTeam.EditIndex = e.NewEditIndex;
                int editindex = e.NewEditIndex;
                int catid = int.Parse(GridProjectTeam.DataKeys[e.NewEditIndex].Value.ToString());
                DataTable ProjectTeamTable = (DataTable)ViewState["ProjectTeamTable"];
                GridProjectTeam.DataSource = ProjectTeamTable;
                GridProjectTeam.DataBind();

                DropDownList dlRoles = (DropDownList)GridProjectTeam.Rows[e.NewEditIndex].FindControl("ddlTeamRoleEdit");
                DropDownList dlPercentage = (DropDownList)GridProjectTeam.Rows[e.NewEditIndex].FindControl("ddlEditPercentage");
                TextBox Department = (TextBox)GridProjectTeam.Rows[e.NewEditIndex].FindControl("txtdepartmentEditor");


                BindRoles(dlRoles, dlPercentage);

                HiddenField hdnTypeName = (HiddenField)GridProjectTeam.Rows[e.NewEditIndex].FindControl("hdnRoleType");
                dlRoles.Items.FindByText(Convert.ToString(hdnTypeName.Value)).Selected = true;


                HiddenField hdnPercentageRole = (HiddenField)GridProjectTeam.Rows[e.NewEditIndex].FindControl("hdnPercentageRole");
                dlPercentage.Items.FindByText(Convert.ToString(hdnPercentageRole.Value)).Selected = true;



                HiddenField hdndepartment = (HiddenField)GridProjectTeam.Rows[e.NewEditIndex].FindControl("hdndepartment");
                Department.Text = hdndepartment.Value;



                HiddenField hdnTeamMember = (HiddenField)GridProjectTeam.Rows[e.NewEditIndex].FindControl("hdnTeamMember");
                PeopleEditor Peopleeditor = (PeopleEditor)GridProjectTeam.Rows[e.NewEditIndex].FindControl("txtSPPeopleEditor");

                SPUser UseLoginName = SPContext.Current.Web.SiteUsers.GetByID(Convert.ToInt32(ProjectTeamTable.Rows[catid]["MemberID"]));
                Peopleeditor.CommaSeparatedAccounts = UseLoginName.LoginName;
                Peopleeditor.Validate();
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  GridProjectTeam_RowEditing: " + ex.Message, TraceSeverity.Unexpected);
            }

        }

        protected void GridProjectTeam_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

            try
            {
                GridProjectTeam.EditIndex = e.RowIndex;
                int index = e.RowIndex;

                int catid = int.Parse(GridProjectTeam.DataKeys[e.RowIndex].Value.ToString());


                DropDownList TeamRole = (DropDownList)GridProjectTeam.Rows[index].FindControl("ddlTeamRoleEdit");
                string TeamRoleText = TeamRole.SelectedItem.Text;



                DropDownList TeamPercentage = (DropDownList)GridProjectTeam.Rows[index].FindControl("ddlEditPercentage");
                string TeamPercentageText = TeamPercentage.SelectedItem.Text;



                TextBox Department = (TextBox)GridProjectTeam.Rows[index].FindControl("txtdepartmentEditor");
                string DepartmentText = Department.Text;



                PeopleEditor txtTeammemberEdit = (PeopleEditor)GridProjectTeam.Rows[index].FindControl("txtSPPeopleEditor");
                string EditTeamMemberText = txtTeammemberEdit.CommaSeparatedAccounts;

                SPUser Editeduser = SPContext.Current.Web.EnsureUser(txtTeammemberEdit.CommaSeparatedAccounts);


                DataTable ProjectTeamTable = (DataTable)ViewState["ProjectTeamTable"];

                foreach (DataRow dtRow in ProjectTeamTable.Rows)
                {

                    int ID = Convert.ToInt32(dtRow["IDProjectTeam"]);

                    if (catid == ID)
                    {
                        dtRow["TeamMember"] = Editeduser.Name;
                        dtRow["TeamRole"] = TeamRoleText;
                        dtRow["Percentage"] = TeamPercentageText;
                        dtRow["Department"] = DepartmentText;

                    }

                }
                txtTeammemberEdit.Validate();
                ProjectTeamTable.AcceptChanges();
                GridProjectTeam.EditIndex = -1;
                GridProjectTeam.DataSource = ProjectTeamTable;
                GridProjectTeam.DataBind();
                ViewState["ProjectTeamTable"] = ProjectTeamTable;

            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  GridProjectTeam_RowUpdating: " + ex.Message, TraceSeverity.Unexpected);
            }
        }

        protected void GridProjectTeam_RowCommandDatabound(object sender, GridViewRowEventArgs e)
        {

            try
            {

                if (e.Row.RowType == DataControlRowType.Footer)
                {
                    BindRoles(((DropDownList)e.Row.FindControl("ddlTeamRole")), ((DropDownList)e.Row.FindControl("ddlPercentage")));
                    ((Button)e.Row.FindControl("btnAddTeamMember")).Attributes.Add("onclick", "javascript:return checkPeopleEditor('" + ((PeopleEditor)e.Row.FindControl("txtFooterTeamMember")).ClientID + "','" + ((DropDownList)e.Row.FindControl("ddlTeamRole")).ClientID + "','" + ((TextBox)e.Row.FindControl("txtfooterdepartmentEditor")).ClientID + "','" + ((DropDownList)e.Row.FindControl("ddlPercentage")).ClientID + "');");
                }

               
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  GridProjectTeam_RowCommandDatabound: " + ex.Message, TraceSeverity.Unexpected);
            }
        }

        protected void GridProjectTeam_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "EditPCS")
                {

                }
                else if (e.CommandName == "Delete")
                {

                }

                else if (e.CommandName == "Notify")
                {
                    string title = string.Empty;
                    title = "Email Notifications - Comments";
                    string[] arg = e.CommandArgument.ToString().Split(';');
                    int index = Convert.ToInt16(arg[0]);
                    SelectedTab.Value = "0";
                    HiddenField hdnmail = GridProjectTeam.Rows[index].FindControl("hdnMailSent") as HiddenField;
                    DataTable ProjectTeamTable = (DataTable)ViewState["ProjectTeamTable"];
                    int id = Convert.ToInt32(ProjectTeamTable.Rows[index]["MemberID"]);
                    string TeamMemberEmail = string.Empty;
                    SPUser UseLoginName = SPContext.Current.Web.SiteUsers.GetByID(Convert.ToInt32(id));
                    TeamMemberEmail = UseLoginName.Email;
                    string UserDisplayName = UseLoginName.Name;
                    System.Web.UI.WebControls.Label TeamRole = GridProjectTeam.Rows[index].FindControl("lblTeamRole") as System.Web.UI.WebControls.Label;
                    // Label MailSentText = GridProjectTeam.Rows[index].FindControl("lblSentEmail") as Label;
                    string TeamRoleName = TeamRole.Text;
                    string SiteTitle = SPContext.Current.Web.Title;
                    string Url = SPContext.Current.Web.Url + "/SitePages/EmailNotification.aspx?ProjectId=" + sigmaId + "&RowId=" + index + "&UserID=" + id + "&TeamRoleName=" + TeamRoleName + "&SiteTitle=" + SiteTitle + "&UserDisplayName=" + UserDisplayName;
                    ScriptManager.RegisterClientScriptBlock(updatePanel, updatePanel.GetType(), "ModalReportsNew", "popupmodaluiReturnValue1('" + Url + "','" + title + "');", true);
                    //  SendEmailtoSelectedUsers(TeamMemberEmail, TeamRoleName, SiteTitle, UserDisplayName);
                    hdnmail.Value = "MailSent";
                    Hashtable hashtable = new Hashtable();
                    if (ViewState["hashtable"] != null)
                    {
                        hashtable = (Hashtable)ViewState["hashtable"];
                    }
                    if (hdnmail.Value == "MailSent")
                    {
                        //Label lblsentemail = GridProjectTeam.Rows[index].FindControl("lblSentEmail") as Label;
                        //lblsentemail.Visible = true;
                        ViewState["indexNumber"] = index;
                        hashtable.Add(index, "MailSent");

                    }
                    else
                    {
                        hashtable.Add(index, "MailSent");
                    }
                    ViewState["hashtable"] = hashtable;


                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  GridProjectTeam_RowCommand: " + ex.Message, TraceSeverity.Unexpected);
            }

        }

        private void SentStatusBasedEmail(string Status, string ProjectName, SPUser ProjectBBUsers, SPUser ProjectGBUsers, SPUser ProjectSponsersUsers,string Comments, SPWeb web)
        {
            try
            {
                string newStatus = "";
                if (Status.Contains("::"))
                {
                    string[] spliter = { "::" };
                    newStatus = Status.Split(spliter, StringSplitOptions.None)[1];
                    Status = Status.Split(spliter, StringSplitOptions.None)[0];
                }
                else
                {
                    newStatus = Status;
                }
                SPList emailList = web.Lists.TryGetList("Lookup_ProcertEmailConfiguration");
                SPQuery query = new SPQuery();
                SPListItemCollection itemcollection = null;
                SPListItem emailItem = null;
                query.Query = "<Where><Eq><FieldRef Name='Title' /><Value Type='Text'>" + Status + "</Value></Eq></Where><OrderBy><FieldRef Name='ID' Ascending='false' /></OrderBy>";
                itemcollection = emailList.GetItems(query);
                if (itemcollection.Count >= 1)
                {
                    emailItem = itemcollection[0];
                }
                StringBuilder strbody = new StringBuilder();
                string subject = string.Empty;
                ArrayList toEmail = new ArrayList();
                switch (Status)
                {
                    case "Awaiting Project Authorization by Project Sponsor":
                        subject = Convert.ToString(emailItem["Subject"]);
                        strbody.Append(Convert.ToString(emailItem["body"]));
                        toEmail.Add(ProjectSponsersUsers.Email);
                        break;
                    case "Awaiting Project Authorization by Black Belt":
                        subject = Convert.ToString(emailItem["Subject"]);
                        strbody.Append(Convert.ToString(emailItem["body"]));
                        toEmail.Add(ProjectBBUsers.Email);
                        break;
                    case "Define":
                        subject = Convert.ToString(emailItem["Subject"]);
                        strbody.Append(Convert.ToString(emailItem["body"]));
                        toEmail.Add(ProjectGBUsers.Email);
                        foreach (GridViewRow row in GridProjectTeam.Rows)
                        {
                            HiddenField Members = (HiddenField)row.FindControl("hdmMemberId");
                            string TeamMemberId = Members.Value;
                            toEmail.Add(SPContext.Current.Web.SiteUsers.GetByID(Convert.ToInt32(TeamMemberId)).Email);
                        }
                        break;
                    case "Awaiting Define Gate Black Belt Approval":
                        subject = Convert.ToString(emailItem["Subject"]);
                        strbody.Append(Convert.ToString(emailItem["body"]));
                        toEmail.Add(ProjectBBUsers.Email);
                        break;
                    case "Measure":
                        subject = Convert.ToString(emailItem["Subject"]);
                        strbody.Append(Convert.ToString(emailItem["body"]));
                        toEmail.Add(ProjectGBUsers.Email);
                        break;
                    case "Awaiting Measure Gate Black Belt Approval":
                        subject = Convert.ToString(emailItem["Subject"]);
                        strbody.Append(Convert.ToString(emailItem["body"]));
                        toEmail.Add(ProjectBBUsers.Email);
                        break;
                    case "Analyze":
                       subject = Convert.ToString(emailItem["Subject"]);
                       strbody.Append(Convert.ToString(emailItem["body"]));
                        toEmail.Add(ProjectGBUsers.Email);
                        break;
                    case "Awaiting Analyze Gate Black Belt Approval":
                        subject = Convert.ToString(emailItem["Subject"]);
                        strbody.Append(Convert.ToString(emailItem["body"]));
                        toEmail.Add(ProjectBBUsers.Email);
                        break;
                    case "Improve":
                        subject = Convert.ToString(emailItem["Subject"]);
                        strbody.Append(Convert.ToString(emailItem["body"]));
                        toEmail.Add(ProjectGBUsers.Email);
                        break;
                    case "Awaiting Improve Gate Black Belt Approval":
                      subject = Convert.ToString(emailItem["Subject"]);
                      strbody.Append(Convert.ToString(emailItem["body"]));
                        toEmail.Add(ProjectBBUsers.Email);
                        break;
                    case "Control":
                       subject = Convert.ToString(emailItem["Subject"]);
                       strbody.Append(Convert.ToString(emailItem["body"]));
                        toEmail.Add(ProjectGBUsers.Email);
                        break;
                    case "Awaiting Control Gate Black Belt Approval":
                       subject = Convert.ToString(emailItem["Subject"]);
                       strbody.Append(Convert.ToString(emailItem["body"]));
                        toEmail.Add(ProjectBBUsers.Email);
                        break;
                    case "Final Report Preparation": 
                        subject = Convert.ToString(emailItem["Subject"]);
                        strbody.Append(Convert.ToString(emailItem["body"]));
                        toEmail.Add(ProjectGBUsers.Email);
                        break;
                    case "Awaiting Final Report Black Belt Approval":
                        subject = Convert.ToString(emailItem["Subject"]);
                        strbody.Append(Convert.ToString(emailItem["body"]));
                        toEmail.Add(ProjectBBUsers.Email);
                        break;
                    case "Unlocked":
                        subject = Convert.ToString(emailItem["Subject"]);
                        strbody.Append(Convert.ToString(emailItem["body"]));
                        toEmail.Add(ProjectGBUsers.Email);
                        break;
                    case "Locked":
                        subject = Convert.ToString(emailItem["Subject"]);
                        strbody.Append(Convert.ToString(emailItem["body"]));
                        toEmail.Add(ProjectGBUsers.Email);
                        break;
                    case "Edit Completed":
                        subject = Convert.ToString(emailItem["Subject"]);
                        strbody.Append(Convert.ToString(emailItem["body"]));
                        toEmail.Add(ProjectBBUsers.Email);
                        break;
                    case "Return to Project Lead-Project Sponsor":
                        subject = Convert.ToString(emailItem["Subject"]);
                        strbody.Append(Convert.ToString(emailItem["body"]));
                        toEmail.Add(ProjectGBUsers.Email);
                        break;
                    case "Return to Project Lead-Black Belt":
                        subject = Convert.ToString(emailItem["Subject"]);
                        strbody.Append(Convert.ToString(emailItem["body"]));
                        toEmail.Add(ProjectGBUsers.Email);
                        break;
                    case "Return to Project Lead-Gates":
                        subject = Convert.ToString(emailItem["Subject"]);
                        strbody.Append(Convert.ToString(emailItem["body"]));
                        toEmail.Add(ProjectGBUsers.Email);
                        break;
                    case "Final Report Approved":
                        subject = Convert.ToString(emailItem["Subject"]);
                        strbody.Append(Convert.ToString(emailItem["body"]));
                        toEmail.Add(ProjectGBUsers.Email);
                        break;
                    default:
                        break;
                }
                string _id = HttpUtility.UrlEncode(Encrypt(Convert.ToString(sigmaId)));
                string GateItemsiteUrl = CurrentSiteUrl + "/SitePages/BreakThroughProcertProjectsTracking.aspx?ProjectId=" + _id +"&SelectedTab="+2;
                string ItemsiteUrl = CurrentSiteUrl + "/SitePages/BreakThroughProcertProjectsTracking.aspx?ProjectId=" + _id;
                strbody.Replace("[GateForm_URL]", GateItemsiteUrl);
                strbody.Replace("[Form_URL]", ItemsiteUrl);
                strbody.Replace("[Project_Name]", ProjectName);
                strbody.Replace("[Project_Status]", newStatus);
                strbody.Replace("[Project_Lead]", ProjectGBUsers.Name);
                strbody.Replace("[Project_Comments]", Comments);
                strbody.Replace("[Sponser_Name]", ProjectSponsersUsers.Name);
                strbody.Replace("[BB_Name]", ProjectBBUsers.Name);
                SendMail(toEmail, strbody.ToString(), subject, "");
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error in sending Email Notification in Six Sigma " + ex.Message, TraceSeverity.Unexpected);
            }
        }

        private void SendEmailtoSelectedUsers(string TeamMember, string TeamRole, string SiteTitle, string ToUserName)
        {
            try
            {
                ArrayList toEmail = new ArrayList();
                string Subject = string.Empty;
                string TeamMemberEmailId = TeamMember;
                toEmail.Add(TeamMemberEmailId);
                StringBuilder strbody = new StringBuilder();
                SPListItem Item = GeSixSigmaDataByID(sigmaId);
                Subject = "Notification: You are required to work on Six Sigma #" + sigmaId;
                strbody = new StringBuilder();
                strbody.Append("<p><span style='font-family:Calibri;'>Dear <b>_ToName_</b>,</span></p>");
                strbody.Append("<p><span style='font-family:Calibri;'>You have been selected as a <b> _Role_ </b> to work on project  <b> _SiteTitle_ </b>.</span></p>");
                strbody.Append("<p><a href='_formURL_'><span style='font-family:Calibri;'>Click here</span></a>");
                strbody.Append("<span style='font-family:Calibri;'>&nbsp;to access the project details and take action.</span></p>");
                strbody.Append("<p><span style='font-family:Calibri;'>Thank you.</span></p>");
                string _id = HttpUtility.UrlEncode(Encrypt(Convert.ToString(sigmaId)));
                string ItemsiteUrl = CurrentSiteUrl + "/SitePages/BreakThroughProcertProjectsTracking.aspx?ProjectId=" + _id;
                strbody.Replace("_formURL_", ItemsiteUrl);
                strbody.Replace("_FormNumber_", Convert.ToString(sigmaId));
                strbody.Replace("_SiteTitle_", Convert.ToString(Item["ProjectName"]));
                strbody.Replace("_Role_", TeamRole);
                strbody.Replace("_ToName_", ToUserName);
                SendMail(toEmail, Convert.ToString(strbody), Subject, "");
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error in sending Notification in Six Sigma " + ex.Message, TraceSeverity.Unexpected);
            }

        }

        #region Mail Functions
        public static bool SendMail(ArrayList To, string Body, string Subject, string CCUser)
        {
            bool mailSent = false;
            try
            {

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Host = SPContext.Current.Site.WebApplication.
                OutboundMailServiceInstance.Server.Address;
                MailMessage mailMessage = new MailMessage();
                string CurrentUserEmail= SPContext.Current.Web.CurrentUser.Email;
                if (!string.IsNullOrEmpty(CurrentUserEmail))
                {
                    mailMessage.From = new MailAddress(CurrentUserEmail);
                }
                else
                {
                    mailMessage.From = new MailAddress("sharepoint@pwc.ca");
                }
               
                mailMessage.Subject = Subject;
                mailMessage.Body = Body;
                foreach (string toemail in To)
                {
                    if (toemail != null)
                        mailMessage.To.Add(toemail);
                    if (!string.IsNullOrEmpty(CCUser))
                    {
                        mailMessage.CC.Add(CCUser);
                    }
                }
                mailMessage.IsBodyHtml = true;
                smtpClient.Send(mailMessage);
                mailSent = true;
            }
            catch (Exception ex) { ULSLogger.LogErrorInULS("Error On  SendMail: " + ex.Message, TraceSeverity.Unexpected); return mailSent; }
            return mailSent;
        }
        #endregion

        private void AddNewRowToQuadCharts(string Action)
        {
            SPUser CurrentUser = SPContext.Current.Web.CurrentUser;
            using (SPSite Site = new SPSite(CurrentSiteUrl, SPUserToken.SystemAccount))
            {
                using (SPWeb web = Site.OpenWeb())
                {
                    try
                    {
                        web.AllowUnsafeUpdates = true;
                        SPList list = web.Lists[documentsListName];
                        SPFolder QuadChartRootFolder = web.GetFolder(web.ServerRelativeUrl + "Documents/Templates/Updates/Quad Charts");
                        SPFolder QuadChartRootFolderCopy = web.GetFolder(web.ServerRelativeUrl + "Documents/Project" + Convert.ToString(sigmaId) + "/Updates/Quad Charts");
                        SPFolder destinationRootfolder = web.GetFolder(web.ServerRelativeUrl + "Documents/Project" + Convert.ToString(sigmaId) + "/Updates/Quad Charts");
                        if (Action == "Add")
                        {
                            //copyQuadFile(Action, QuadChartRootFolder.Files, destinationRootfolder, list, CurrentUser);
                            SPQuery addQuery = new SPQuery();
                            addQuery.Folder = QuadChartRootFolder;
                            addQuery.Query = "<OrderBy><FieldRef Name='Created' Ascending='false' /></OrderBy>";
                            SPListItem sourceAddItem = list.GetItems(addQuery)[0];
                            if (sourceAddItem != null)
                            {
                                SPFile sourceAddFile = sourceAddItem.File;
                                copyQuadFile(Action, sourceAddFile, destinationRootfolder, list, CurrentUser);
                            }
                           
                        }
                        else if (Action == "Copy")
                        {
                            //copyQuadFile(Action, QuadChartRootFolderCopy.Files, destinationRootfolder, list, CurrentUser);
                            SPQuery copyQuery = new SPQuery();
                            copyQuery.Folder = QuadChartRootFolderCopy;
                            copyQuery.Query = "<OrderBy><FieldRef Name='Created' Ascending='false' /></OrderBy>";
                            SPListItem sourceCopyItem = list.GetItems(copyQuery)[0];
                            if (sourceCopyItem != null)
                            {
                                SPFile sourceCopyFile = sourceCopyItem.File;
                                copyQuadFile(Action, sourceCopyFile, destinationRootfolder, list, CurrentUser);
                            }
                        }
                        BindQuadCharts(CurrentSiteUrl, sigmaId);
                    }
                    catch (Exception ex)
                    {
                        ULSLogger.LogErrorInULS("Error On  AddNewRowToQuadChart: " + ex.StackTrace, TraceSeverity.Unexpected);
                    }
                    finally
                    {
                        web.AllowUnsafeUpdates = false;
                    }
                }
            }
        }

        private void copyQuadFile(string Action, SPFile file, SPFolder destinationRootfolder, SPList list, SPUser CurrentUser)
        {
            ULSLogger.LogErrorInULS("In copyQuadFile Start.........." + Action);
            //if (fileColl.Count > 0)
            //{
                //SPFile file = fileColl[0];
                string FileNameWithoutExt=string.Empty;
                string fileName = string.Empty;
                if (Action == "Add")
                {
                     FileNameWithoutExt = file.Name.Split('.')[0] + "_" + DateTime.Now.ToString("MMM");
                     fileName = FileNameWithoutExt + "." + file.Name.Split('.')[1];
                     SPQuery query = new SPQuery();
                     query.Folder = destinationRootfolder;
                     SPListItemCollection itemcollection = null;
                     query.Query = "<Where><Contains><FieldRef Name='LinkFilename' /><Value Type='Computed'>" + FileNameWithoutExt + "</Value></Contains></Where><OrderBy><FieldRef Name='ID' Ascending='false' /></OrderBy>";
                     itemcollection = list.GetItems(query);
                     if (itemcollection.Count >= 1)
                     {
                         fileName = FileNameWithoutExt + "_" + itemcollection.Count + "." + fileName.Split('.')[1];
                     }
                }
                else if (Action == "Copy") 
                {
                     //FileNameWithoutExt = file.Name.Split('.')[0];
                     //fileName = FileNameWithoutExt + "." + file.Name.Split('.')[1];
                    fileName = "Copy of " + file.Name;
                }          
                SPListItem SrcItem = file.Item;
                SPListItem DestItem = destinationRootfolder.Files.Add(fileName, file.OpenBinary(), true).Item;
                DestItem.UpdateOverwriteVersion();
                DestItem["Author"] = CurrentUser;
                DestItem["Editor"] = CurrentUser;
                DestItem.UpdateOverwriteVersion();
                ArrayList Quaditemdoc = new ArrayList();
                Quaditemdoc.Add(DestItem.Web.Url);
                Quaditemdoc.Add(DestItem.File.Url);
                Quaditemdoc.Add(DestItem.ParentList.Forms[PAGETYPE.PAGE_EDITFORM].Url);
                Quaditemdoc.Add(Convert.ToString(DestItem.ID));
                if (Quaditemdoc != null && Quaditemdoc.Count > 0)
                {
                    string editurl = string.Format("{0}{1}?ID={2}&Source=" + SPContext.Current.Web.Url + "/SitePages/BreakThroughProcertProjectsTracking.aspx.aspx?ProjectId=" + Encrypt(Convert.ToString(sigmaId)), Quaditemdoc[0], "/" + Quaditemdoc[2], Convert.ToInt32(Quaditemdoc[3]));
                    ScriptManager.RegisterClientScriptBlock(updatePanel, updatePanel.GetType(), "ModalHostScript", "popupmodaluiNewForm('" + editurl + "','');", true);
                    //AddAutoFeed("A new document is uploaded in Upload Files section", sigmaId);
                }
                ViewState["ItemUrl"] = Quaditemdoc;
            //}
            ULSLogger.LogErrorInULS("In copyQuadFile End.........."+Action);
        }

        private DataRow AddRowToQuadCharts(DataTable dtCurrentTable, DataRow drCurrentRow)
        {
            if (ViewState["QuadChartsTable"] == null)
            {
                DataColumn dcID = new DataColumn("IDQuad", typeof(int));
                DataColumn dcDateUploaded = new DataColumn("DateUploaded", typeof(string));
                DataColumn dcBy = new DataColumn("By", typeof(string));
                DataColumn dcFileUrl = new DataColumn("QuadChartFileUrl", typeof(string));

                dcID.AutoIncrement = true;
                dcID.AutoIncrementSeed = 0;
                dcID.AutoIncrementStep = 1;
                dtCurrentTable.Columns.Add(dcID);
                dtCurrentTable.Columns.Add(dcDateUploaded);
                dtCurrentTable.Columns.Add(dcBy);
                dtCurrentTable.Columns.Add(dcFileUrl);
            }
            drCurrentRow = dtCurrentTable.NewRow();
            String user = SPContext.Current.Web.CurrentUser.Name;
            drCurrentRow["DateUploaded"] = DateTime.Now.ToString("MMM dd, yyyy");
            drCurrentRow["By"] = user;
            //drCurrentRow["QuadChartFileUrl"] = SPContext.Current.Web.Url +"/"+ GetQuadFileURl();
            return drCurrentRow;
        }

        private string GetQuadFileURl()
        {
            SPFile SubFolderUrl = SPContext.Current.Web.GetFolder(SPContext.Current.Web.Url + "/Documents/" + "Project" + sigmaId + "/Updates/Quad Charts").Files[0];
            return SubFolderUrl.Url;
        }

        private void BindQuadCharts(string SiteUrl, int SigmaId)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite site = new SPSite(SiteUrl))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            //SPList list = web.Lists["Lookup_OtherAttachments_List"];
                            SPList list = web.Lists[documentsListName];
                            SPFolder quadChartsFolder = web.GetFolder(web.ServerRelativeUrl + "Documents/Project" + Convert.ToString(sigmaId) + "/Updates/Quad Charts");
                            SPQuery spQuery = new SPQuery();
                            spQuery.Folder = quadChartsFolder;

                            SPListItemCollection allitems = list.GetItems(spQuery);
                            //SPQuery Teamquery = new SPQuery();
                            //Teamquery.Query = "<Where><Eq><FieldRef Name='SigmaId' /><Value Type='Number'>" + SigmaId + "</Value></Eq></Where>";
                            //SPListItemCollection allitems = quadChartsFolder.i
                            DataTable dt = new DataTable();
                            dt = allitems.GetDataTable();
                            DataTable dtQuadCharts = new DataTable();
                            DataColumn dcID = new DataColumn("IDQuad", typeof(int));
                            dcID.AutoIncrement = true;
                            dcID.AutoIncrementSeed = 0;
                            dcID.AutoIncrementStep = 1;
                            DataColumn dcDateUploaded = new DataColumn("DateUploaded", typeof(string));
                            DataColumn dcBy = new DataColumn("By", typeof(string));
                            DataColumn dcFile = new DataColumn("QuadChartFileUrl", typeof(string));
                            DataColumn docID = new DataColumn("ID", typeof(int));
                            DataColumn docName = new DataColumn("DocName", typeof(string));
                            dtQuadCharts.Columns.Add(dcID);
                            dtQuadCharts.Columns.Add(dcDateUploaded);
                            dtQuadCharts.Columns.Add(dcBy);
                            dtQuadCharts.Columns.Add(dcFile);
                            dtQuadCharts.Columns.Add(docID);
                            dtQuadCharts.Columns.Add(docName);
                            if (dt == null)
                            {
                                DataRow row = dtQuadCharts.NewRow();
                                dtQuadCharts.Rows.Add(row);
                                dtQuadCharts.AcceptChanges();
                                GridQuadDetails.DataSource = dtQuadCharts;
                                ViewState["QuadFileTable"] = dtQuadCharts;
                                GridQuadDetails.DataBind();
                                GridQuadDetails.Rows[0].Style["Display"] = "none";
                            }
                            else
                            {
                                foreach (SPListItem item in allitems)
                                {
                                    DataRow row = dtQuadCharts.NewRow();
                                    row["DateUploaded"] = Convert.ToDateTime(item["Created"]).ToString("MMM dd, yyyy");
                                    row["By"] = getDisplayName(Convert.ToString(item["Author"]));
                                    row["QuadChartFileUrl"] = web.Url + "/" + item.Url;
                                    row["ID"] = item.ID;
                                    row["DocName"] = item.Name;
                                    dtQuadCharts.Rows.Add(row);
                                }
                                dtQuadCharts.AcceptChanges();
                                GridQuadDetails.DataSource = dtQuadCharts;
                                ViewState["QuadFileTable"] = dtQuadCharts;
                                GridQuadDetails.DataBind();


                            }



                        }
                    }

                });
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  BindQuadCharts: " + ex.Message, TraceSeverity.Unexpected);
            }

        }

        protected void GridQuadCharts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

        }

        protected void GridQuadCharts_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {

        }

        protected void GridQuadCharts_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }

        protected void GridQuadCharts_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void GridQuadCharts_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                DataTable QuadChartTable = (DataTable)ViewState["QuadChartsTable"];
                int catid = int.Parse(GridQuadDetails.DataKeys[e.RowIndex].Value.ToString());
                GridQuadDetails.EditIndex = e.RowIndex;
                int index = e.RowIndex;
                for (int r = 0; r < QuadChartTable.Rows.Count; r++)
                {
                    DataRow dtRow = QuadChartTable.Rows[r];
                    int ID = Convert.ToInt32(dtRow["IDQuad"]);
                    if (catid == ID)
                    {
                        dtRow.Delete();
                    }
                }
                QuadChartTable.AcceptChanges();
                for (int i = 0; i < QuadChartTable.Rows.Count; i++)
                {
                    QuadChartTable.Rows[i]["IDQuad"] = i;
                    QuadChartTable.AcceptChanges();
                }

                if (QuadChartTable.Rows.Count != 0)
                {
                    GridQuadDetails.EditIndex = -1;
                    GridQuadDetails.DataSource = QuadChartTable;
                    GridQuadDetails.DataBind();
                    ViewState["QuadChartsTable"] = QuadChartTable;
                }
                else
                {
                    DataRow row = QuadChartTable.NewRow();
                    QuadChartTable.Rows.Add(row);
                    GridQuadDetails.DataSource = QuadChartTable;
                    GridQuadDetails.DataBind();
                    GridQuadDetails.Rows[0].Style["Display"] = "none";
                    ViewState["QuadChartsTable"] = null;
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  QuadChartsTable_RowDeleting: " + ex.Message, TraceSeverity.Unexpected);
            }

        }

        protected void GridQuadCharts_RowEditing(object sender, GridViewEditEventArgs e)
        {

        }

        protected void GridQuadCharts_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

        }

        protected void GridQuadCharts_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        //private void UpdateQuadChartsData(SPWeb oWeb, int ID)
        //{
        //    try
        //    {
        //        DataTable QuadChartsTable = (DataTable)ViewState["QuadChartsTable"];

        //        if (QuadChartsTable != null)
        //        {
        //            DeleteExistingQuadChartsData(oWeb, ID);

        //            foreach (DataRow row in QuadChartsTable.Rows) // Loop over the rows.
        //            {
        //                AddItemsInQuadChartsList(row, ID);
        //            }
        //        }
        //        else
        //        {
        //            DeleteExistingQuadChartsData(oWeb, ID);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ULSLogger.LogErrorInULS("Error in  AddUpdateProjectInfoandSettlementsMethod" + ex.Message);
        //    }
        //}



        //protected void AddItemsInQuadChartsList(DataRow ViewStateitem, int SigmaId)
        //{
        //    try
        //    {
        //        SPSecurity.RunWithElevatedPrivileges(delegate()
        //        {
        //            using (SPSite site = new SPSite(SiteUrl))
        //            {
        //                using (SPWeb web = site.OpenWeb())
        //                {
        //                    SPListItem item = null;
        //                    web.AllowUnsafeUpdates = true;
        //                    SPList list = web.Lists["Lookup_OtherAttachments_List"];
        //                    item = list.Items.Add();
        //                    string LoginName = Convert.ToString(ViewStateitem[2]);
        //                    SPUser user = web.EnsureUser(LoginName);
        //                    item["DateUploaded"] = ViewStateitem[1];
        //                    item["By"] = user;
        //                    item["QuadChartFileUrl"] = ViewStateitem[3];
        //                    item["SigmaId"] = SigmaId;
        //                    item.Update();
        //                    web.AllowUnsafeUpdates = false;
        //                }
        //            }

        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        ULSLogger.LogErrorInULS("Error in Adding AddItemsInTeamRolesListMethod" + ex.Message);
        //    }
        //}

        //private static void DeleteExistingQuadChartsData(SPWeb oWeb, int ID)
        //{
        //    try
        //    {
        //        SPSecurity.RunWithElevatedPrivileges(delegate()
        //        {
        //            oWeb.AllowUnsafeUpdates = true;
        //            SPList list = oWeb.Lists["Lookup_OtherAttachments_List"];
        //            SPQuery Query = new SPQuery();
        //            Query.Query = "<Where><Eq><FieldRef Name='SigmaId' /><Value Type='Number'>" + ID + "</Value></Eq></Where>";
        //            SPListItemCollection itemcoll = list.GetItems(Query);
        //            int itemcountcoll = itemcoll.Count;
        //            for (int intIndex = itemcountcoll - 1; intIndex > -1; intIndex--)
        //            {
        //                itemcoll.Delete(intIndex);
        //            }
        //            oWeb.AllowUnsafeUpdates = false;
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        ULSLogger.LogErrorInULS("Error in  DeleteExistingTeamRolesListMethod" + ex.Message);
        //    }
        //}

        protected void GridQuadDetails_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void GridQuadDetails_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

        }

        protected void GridQuadDetails_RowEditing(object sender, GridViewEditEventArgs e)
        {

        }

        protected void GridQuadDetails_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                SelectedTab.Value = "1";
                DataTable QuadFileTable = (DataTable)ViewState["QuadFileTable"];
                int catid = int.Parse(GridQuadDetails.DataKeys[e.RowIndex].Value.ToString());
                GridQuadDetails.EditIndex = e.RowIndex;
                int index = e.RowIndex;
                for (int r = 0; r < GridQuadDetails.Rows.Count; r++)
                {
                    DataRow dtRow = QuadFileTable.Rows[r];
                    int ID = Convert.ToInt32(dtRow["IDQuad"]);
                    int FileID = Convert.ToInt32(dtRow["ID"]);
                    if (catid == ID)
                    {      
                        SPList list = SPContext.Current.Web.Lists[documentsListName];
                        SPQuery query = new SPQuery();
                        SPFolder destinationRootfolder = SPContext.Current.Web.GetFolder(SPContext.Current.Web.ServerRelativeUrl + "Documents/Project" + Convert.ToString(sigmaId) + "/Updates/Quad Charts");
                        query.Folder = destinationRootfolder;
                        query.Query = "<Where><Eq><FieldRef Name='ID' /><Value Type='Counter'>" + FileID + "</Value></Eq></Where>";
                        SPListItem item = list.GetItems(query)[0];
                        item.Delete();
                        dtRow.Delete();
                    }            
                }
                QuadFileTable.AcceptChanges();
                for (int i = 0; i < QuadFileTable.Rows.Count; i++)
                {
                    QuadFileTable.Rows[i]["IDQuad"] = i;
                    QuadFileTable.AcceptChanges();
                }
                if (QuadFileTable.Rows.Count != 0)
                {
                    GridQuadDetails.EditIndex = -1;
                    GridQuadDetails.DataSource = QuadFileTable;
                    GridQuadDetails.DataBind();
                    ViewState["QuadFileTable"] = QuadFileTable;
                }
                else
                {
                    DataRow row = QuadFileTable.NewRow();
                    QuadFileTable.Rows.Add(row);
                    GridQuadDetails.DataSource = QuadFileTable;
                    GridQuadDetails.DataBind();
                    GridQuadDetails.Rows[0].Style["Display"] = "none";
                    ViewState["QuadFileTable"] = null;
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  GridProjectTeam_RowDeleting: " + ex.Message, TraceSeverity.Unexpected);
            }
        }

        protected void GridQuadDetails_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

        }

        protected void GridQuadDetails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            
        }

        protected void GridQuadDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string PageUrl = SPContext.Current.Site.Url.ToString();
                    Image OnEditWindow = (Image)e.Row.FindControl("QuadChartsEdit");
                    Image OnDelteWindow = (Image)e.Row.FindControl("lbtnDeleteQuadFiles");
                    string HiddenDocumentsEditValue = ((HiddenField)(e.Row.FindControl("hdnDocumentsID"))).Value;
                    // string FileURL = GetQuadFileURl();
                    //SPFile FileID = SPContext.Current.Web.GetFolder(SPContext.Current.Web.Url + "/Documents/" + "Project" + sigmaId + "/Updates/Quad Charts").Files[0];
                    string _id = HttpUtility.UrlEncode(Encrypt(Convert.ToString(sigmaId)));
                    string documentEditUrl = PageUrl + "/Documents/Forms/EditForm.aspx?ID=" + HiddenDocumentsEditValue + "&Source=" + PageUrl + "/SitePages/BreakThroughProcertProjectsTracking.aspx?ProjectId=" + _id;
                    OnEditWindow.Attributes.Add("onclick", "javascript:popupmodaluiNewForm('" + documentEditUrl + "','Documents - EditItem')");
                    OnEditWindow.ToolTip = "Click to edit the document properties.";
                    OnDelteWindow.ToolTip = "Click to delete the document.";
                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void disRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                string MeetingBronzeAdmin = SPContext.Current.Web.Title + " Bronze Admin";
                string MeetingSiteOwners = SPContext.Current.Web.Title + "Owners";
                //e.Row.Cells[0].Attributes.Add("style", "word-break:break-all;word-wrap:break-word");
                LinkButton TrashIcon = (LinkButton)e.Row.FindControl("lnktrashdelete");
                TrashIcon.Attributes.Add("onclick", "javascript:FreezeScreen();");
                SPList disList = SPContext.Current.Web.Lists["BreakThroughProcertProjectsTrackingDiscussions"];
                string HiddenAgendaId = string.Empty;
                string hdnDiscussionId = ((HiddenField)(e.Row.FindControl("hdnDiscussionId"))).Value;
                SPQuery query = new SPQuery();
                query.Query = "<Where><Eq><FieldRef Name='SigmaId' /><Value Type='Number'>" + Convert.ToInt32(sigmaId) + "</Value></Eq></Where>";
                //  query.Query = "<Where><Eq><FieldRef Name='AgendaId' /><Value Type='Number'>" + Convert.ToInt32(HiddenAgendaId) + "</Value></Eq></Where>"; // replace with id and change coloumn
                SPListItemCollection itemColl = disList.GetItems(query);
                if (itemColl.Count > 0)
                {
                    SPQuery Query = new SPQuery();
                    Query.Folder = itemColl[0].Folder;
                    SPListItemCollection colls = disList.GetItems(Query);
                    foreach (SPListItem item in colls)
                    {
                        if (item.ID == Convert.ToInt32(hdnDiscussionId))
                        {
                            string Author = Convert.ToString(item["Author"]);

                            int UserId = Convert.ToInt32(Author.Split('#')[0].Split(';')[0]);
                            SPUser user = SPContext.Current.Web.SiteUsers.GetByID(UserId);
                            if (user.LoginName == SPContext.Current.Web.CurrentUser.LoginName)
                            {
                                TrashIcon.Visible = true;

                            }

                            else
                            {
                                TrashIcon.Visible = false;
                            }

                            if (IsMemberOf(MeetingBronzeAdmin) || IsMemberOf(MeetingSiteOwners) || SPContext.Current.Web.UserIsSiteAdmin)
                            {
                                TrashIcon.Visible = true;
                            }

                        }
                    }

                }

            }

        }

        protected void discussion_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            string MeetingBronzeAdmin = SPContext.Current.Web.Title + " Bronze Admin";
            string MeetingSiteOwners = SPContext.Current.Web.Title + "Owners";

            if (e.CommandName == "Trash")
            {
                //ScriptManager.RegisterClientScriptBlock(updatePanel, updatePanel.GetType(), "loadingshow", "ShowProgress();", true);


                GridViewRow row = (GridViewRow)(((Control)e.CommandSource).NamingContainer);

                string hdnDiscussionId = ((HiddenField)(row.FindControl("hdnDiscussionId"))).Value;


                SPList disList = SPContext.Current.Web.Lists["BreakThroughProcertProjectsTrackingDiscussions"];

                if (disList != null)
                {
                    SPContext.Current.Web.AllowUnsafeUpdates = true;
                    SPQuery query = new SPQuery();
                    query.Query = "<Where><Eq><FieldRef Name='SigmaId' /><Value Type='Number'>" + Convert.ToInt32(sigmaId) + "</Value></Eq></Where>";
                    SPListItemCollection itemColl = disList.GetItems(query);
                    if (itemColl.Count > 0)
                    {
                        SPQuery Query = new SPQuery();
                        Query.Folder = itemColl[0].Folder;
                        SPListItemCollection colls = disList.GetItems(Query);
                        foreach (SPListItem item in colls)
                        {
                            if (item.ID == Convert.ToInt32(hdnDiscussionId))
                            {
                                string Author = Convert.ToString(item["Author"]);

                                int UserId = Convert.ToInt32(Author.Split('#')[0].Split(';')[0]);
                                SPUser user = SPContext.Current.Web.SiteUsers.GetByID(UserId);

                                if (user.LoginName == SPContext.Current.Web.CurrentUser.LoginName)
                                {
                                    item.Delete();
                                    break;
                                }

                                if (IsMemberOf(MeetingBronzeAdmin) || IsMemberOf(MeetingSiteOwners) || SPContext.Current.Web.UserIsSiteAdmin)
                                {
                                    item.Delete();
                                    break;
                                }

                            }
                        }

                        bindDiscussionGridTable(sigmaId);
                        SPContext.Current.Web.AllowUnsafeUpdates = false;
                        ScriptManager.RegisterClientScriptBlock(updatePanel, updatePanel.GetType(), "loadinghide", "unFreeze();", true);

                    }
                }

            }


        }

        protected void discussion_DataBound(object sender, EventArgs e)
        {

            EnableNextPrevNavigationForNumericPagedDiscussionGrid(discussion);
        }

        private static void EnableNextPrevNavigationForNumericPagedDiscussionGrid(GridView gv)
        {
            if (gv.BottomPagerRow == null)
                return;
            Table pagerTable = (Table)gv.BottomPagerRow.Controls[0].Controls[0];

            bool prevAdded = false;
            if (gv.PageIndex != 0)
            {
                TableCell prevCell = new TableCell();
                LinkButton prevLink = new LinkButton
                {
                    Text = "< Next Posts",
                    CommandName = "Page",
                    CommandArgument = ((LinkButton)pagerTable.Rows[0].Cells[gv.PageIndex - 1].Controls[0]).CommandArgument
                };
                prevLink.Style["text-decoration"] = "none";
                prevLink.Style["font-weight"] = "bold";
                prevLink.Style["font-size"] = "10pt";
                prevLink.ForeColor = System.Drawing.Color.FromName("#0072bc");
                prevCell.Controls.Add(prevLink);
                pagerTable.Rows[0].Cells.AddAt(0, prevCell);
                prevAdded = true;
            }

            if (gv.PageIndex != gv.PageCount - 1)
            {
                TableCell nextCell = new TableCell();
                LinkButton nextLink = new LinkButton
                {
                    Text = "Previous Posts >",
                    CommandName = "Page",
                    CommandArgument = ((LinkButton)pagerTable.Rows[0].Cells[gv.PageIndex +
                      (prevAdded ? 2 : 1)].Controls[0]).CommandArgument
                };
                nextLink.Style["text-decoration"] = "none";
                nextLink.Style["font-weight"] = "bold";
                nextLink.Style["font-size"] = "10pt";
                nextLink.ForeColor = System.Drawing.Color.FromName("#0072bc");
                nextCell.Controls.Add(nextLink);
                pagerTable.Rows[0].Cells.Add(nextCell);
            }
        }

        protected void AddAttachmentNameURL(object sender, EventArgs e)
        {
            SelectedTab.Value = "3";
            try
            {
                DataTable dtCurrentTable = new DataTable();
                DataRow drCurrentRow = null;
                if (ViewState["SecondAttachemntTable"] != null)
                {
                    dtCurrentTable = (DataTable)ViewState["SecondAttachemntTable"];
                    dtCurrentTable.Columns["IDAttachment"].AutoIncrement = true;
                    dtCurrentTable.Columns["IDAttachment"].AutoIncrementSeed = 0;
                    dtCurrentTable.Columns["IDAttachment"].AutoIncrementStep = 1;
                    drCurrentRow = AddRowToAttachemntNameURL(dtCurrentTable, drCurrentRow);
                    dtCurrentTable.Rows.Add(drCurrentRow);
                    dtCurrentTable.AcceptChanges();
                    //Store the current data to ViewState
                    ViewState["SecondAttachemntTable"] = dtCurrentTable;

                }
                else
                {

                    drCurrentRow = AddRowToAttachemntNameURL(dtCurrentTable, drCurrentRow);
                    dtCurrentTable.Rows.Add(drCurrentRow);
                    dtCurrentTable.AcceptChanges();
                    ViewState["SecondAttachemntTable"] = dtCurrentTable;
                }

                GridAttachmentSecond.DataSource = dtCurrentTable;
                GridAttachmentSecond.DataBind();

            }

            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  AddNewRowToProjectTeamGrid: " + ex.Message, TraceSeverity.Unexpected);
            }
        }

        private DataRow AddRowToAttachemntNameURL(DataTable dtCurrentTable, DataRow drCurrentRow)
        {
            if (ViewState["SecondAttachemntTable"] == null)
            {
                DataColumn dcID = new DataColumn("IDAttachment", typeof(int));
                DataColumn dcLinkName = new DataColumn("LinkName", typeof(string));
                DataColumn dcLinkURL = new DataColumn("LinkURL", typeof(string));
                dcID.AutoIncrement = true;
                dcID.AutoIncrementSeed = 0;
                dcID.AutoIncrementStep = 1;
                dtCurrentTable.Columns.Add(dcID);
                dtCurrentTable.Columns.Add(dcLinkName);
                dtCurrentTable.Columns.Add(dcLinkURL);
            }
            TextBox LinkName = (TextBox)GridAttachmentSecond.FooterRow.FindControl("txtFooterLinkName");
            TextBox LinkURL = (TextBox)GridAttachmentSecond.FooterRow.FindControl("txtFooterLinkURL");

            drCurrentRow = dtCurrentTable.NewRow();
            drCurrentRow["LinkName"] = LinkName.Text;
            drCurrentRow["LinkURL"] = LinkURL.Text;
            return drCurrentRow;
        }

        protected void GridAttachmentSecond_RowCommandDatabound(object sender, GridViewRowEventArgs e)
        {

            try
            {

                if (e.Row.RowType == DataControlRowType.Footer)
                {
                    ((Button)e.Row.FindControl("btnAddAttachment")).Attributes.Add("onclick", "javascript:return checkAttachmentNameURL('" + ((TextBox)e.Row.FindControl("txtFooterLinkName")).ClientID + "','" + ((TextBox)e.Row.FindControl("txtFooterLinkURL")).ClientID + "');");
                }

            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On SecondAttachment: " + ex.Message, TraceSeverity.Unexpected);
            }
        }

        protected void GridAttachmentSecond_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

            try
            {
                GridAttachmentSecond.EditIndex = e.RowIndex;
                int index = e.RowIndex;
                int catid = int.Parse(GridAttachmentSecond.DataKeys[e.RowIndex].Value.ToString());
                TextBox LinkURL = (TextBox)GridAttachmentSecond.Rows[index].FindControl("txtLinkurlEdit");
                TextBox LinkNameEdit = (TextBox)GridAttachmentSecond.Rows[index].FindControl("txtLinkName");
                DataTable ProjectAttachemtTable = (DataTable)ViewState["SecondAttachemntTable"];

                foreach (DataRow dtRow in ProjectAttachemtTable.Rows)
                {
                    int ID = Convert.ToInt32(dtRow["IDAttachment"]);
                    if (catid == ID)
                    {
                        dtRow["LinkName"] = LinkNameEdit.Text;
                        dtRow["LinkURL"] = LinkURL.Text;
                    }

                }
                ProjectAttachemtTable.AcceptChanges();
                GridAttachmentSecond.EditIndex = -1;
                GridAttachmentSecond.DataSource = ProjectAttachemtTable;
                GridAttachmentSecond.DataBind();
                ViewState["SecondAttachemntTable"] = ProjectAttachemtTable;

            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  GridAttachmentSecond_RowUpdating: " + ex.Message, TraceSeverity.Unexpected);
            }
        }

        protected void GridAttachmentSecond_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {

                GridAttachmentSecond.EditIndex = e.NewEditIndex;
                int editindex = e.NewEditIndex;
                int catid = int.Parse(GridAttachmentSecond.DataKeys[e.NewEditIndex].Value.ToString());
                DataTable ProjectAttachemtTable = (DataTable)ViewState["SecondAttachemntTable"];
                GridAttachmentSecond.DataSource = ProjectAttachemtTable;
                GridAttachmentSecond.DataBind();

                TextBox LinkURL = (TextBox)GridAttachmentSecond.Rows[e.NewEditIndex].FindControl("txtLinkurlEdit");
                HiddenField hdnLinkURL = (HiddenField)GridAttachmentSecond.Rows[e.NewEditIndex].FindControl("hdnLinkURL");

                LinkURL.Text = (Convert.ToString(hdnLinkURL.Value));
                HiddenField hdnLinkName = (HiddenField)GridAttachmentSecond.Rows[e.NewEditIndex].FindControl("hdnLinkName");

                TextBox LinkName = (TextBox)GridAttachmentSecond.Rows[e.NewEditIndex].FindControl("txtLinkName");
                LinkName.Text = (Convert.ToString(hdnLinkName.Value));

            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  GridAttachmentSecond_RowEditing: " + ex.Message, TraceSeverity.Unexpected);
            }

        }

        protected void GridAttachmentSecond_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                GridAttachmentSecond.EditIndex = -1;
                DataTable ProjectAttachemtTable = (DataTable)ViewState["SecondAttachemntTable"];
                GridAttachmentSecond.DataSource = ProjectAttachemtTable;
                GridAttachmentSecond.DataBind();
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  GridAttachmentSecond_RowCancelingEdit: " + ex.Message, TraceSeverity.Unexpected);
            }
        }

        protected void GridAttachmentSecond_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

            try
            {
                DataTable ProjectAttachemtTable = (DataTable)ViewState["SecondAttachemntTable"];
                int catid = int.Parse(GridAttachmentSecond.DataKeys[e.RowIndex].Value.ToString());
                GridAttachmentSecond.EditIndex = e.RowIndex;
                int index = e.RowIndex;
                for (int r = 0; r < ProjectAttachemtTable.Rows.Count; r++)
                {
                    DataRow dtRow = ProjectAttachemtTable.Rows[r];
                    int ID = Convert.ToInt32(dtRow["IDAttachment"]);
                    if (catid == ID)
                    {
                        dtRow.Delete();

                    }
                }

                ProjectAttachemtTable.AcceptChanges();
                for (int i = 0; i < ProjectAttachemtTable.Rows.Count; i++)
                {

                    ProjectAttachemtTable.Rows[i]["IDAttachment"] = i;
                    ProjectAttachemtTable.AcceptChanges();

                }

                if (ProjectAttachemtTable.Rows.Count != 0)
                {
                    GridAttachmentSecond.EditIndex = -1;
                    GridAttachmentSecond.DataSource = ProjectAttachemtTable;
                    GridAttachmentSecond.DataBind();
                    ViewState["ProjectTeamTable"] = ProjectAttachemtTable;
                }
                else
                {
                    DataRow row = ProjectAttachemtTable.NewRow();
                    ProjectAttachemtTable.Rows.Add(row);
                    GridAttachmentSecond.DataSource = ProjectAttachemtTable;
                    GridAttachmentSecond.DataBind();
                    GridAttachmentSecond.Rows[0].Style["Display"] = "none";
                    ViewState["SecondAttachemntTable"] = null;
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error On  GridAttachmentSecond_RowDeleting: " + ex.Message, TraceSeverity.Unexpected);
            }
        }

        private void UpdateSecondAttachmentData(SPWeb oWeb, int ID)
        {
            try
            {
                DataTable ProjectAttachemtTable = (DataTable)ViewState["SecondAttachemntTable"];

                if (ProjectAttachemtTable != null)
                {
                    DeleteExistingAttachmentListData(oWeb, ID);

                    foreach (DataRow row in ProjectAttachemtTable.Rows) // Loop over the rows.
                    {
                        AddItemsInProjectSecondAttachmentList(row, ID);
                    }
                }
                else
                {
                    DeleteExistingAttachmentListData(oWeb, ID);
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error in  AddUpdateProjectInfoandSettlementsMethod" + ex.Message);
            }
        }

        protected void AddItemsInProjectSecondAttachmentList(DataRow ViewStateitem, int SigmaId)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite site = new SPSite(SiteUrl))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            SPListItem item = null;
                            web.AllowUnsafeUpdates = true;
                            SPList list = web.Lists["Lookup_OtherAttachments_List"];
                            item = list.Items.Add();
                            item["LinkName"] = ViewStateitem[1];
                            SPFieldUrlValue hyper = new SPFieldUrlValue();
                            hyper.Description = Convert.ToString(ViewStateitem[2]);
                            hyper.Url = Convert.ToString(ViewStateitem[2]);
                            item["LinkURL"] = hyper.Description;
                            item["SigmaId"] = SigmaId;
                            item.Update();
                            web.AllowUnsafeUpdates = false;
                        }
                    }

                });
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error in Adding AddItemsInTeamRolesListMethod" + ex.Message);
            }
        }

        private static void DeleteExistingAttachmentListData(SPWeb oWeb, int ID)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    oWeb.AllowUnsafeUpdates = true;
                    SPList list = oWeb.Lists["Lookup_OtherAttachments_List"];
                    SPQuery Query = new SPQuery();
                    Query.Query = "<Where><Eq><FieldRef Name='SigmaId' /><Value Type='Number'>" + ID + "</Value></Eq></Where>";
                    SPListItemCollection itemcoll = list.GetItems(Query);
                    int itemcountcoll = itemcoll.Count;
                    for (int intIndex = itemcountcoll - 1; intIndex > -1; intIndex--)
                    {
                        itemcoll.Delete(intIndex);
                    }
                    oWeb.AllowUnsafeUpdates = false;
                });
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Error in  DeleteExistingTeamRolesListMethod" + ex.Message);
            }
        }

        private string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";

            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {

                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {

                        cs.Write(clearBytes, 0, clearBytes.Length);

                        cs.Close();

                    }

                    clearText = Convert.ToBase64String(ms.ToArray());

                }

            }

            return clearText;

        }

        private string Decrypt(string cipherText)
        {

            string EncryptionKey = "MAKV2SPBNI99212";

            cipherText = cipherText.Replace(" ", "+");

            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (Aes encryptor = Aes.Create())
            {

                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

                encryptor.Key = pdb.GetBytes(32);

                encryptor.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {

                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {

                        cs.Write(cipherBytes, 0, cipherBytes.Length);

                        cs.Close();

                    }

                    cipherText = Encoding.Unicode.GetString(ms.ToArray());

                }

            }

            return cipherText;

        }

        protected void lnkResolve1_Click(object sender, EventArgs e)
        {
            HiddenField1.Value = "1";
        }

        protected void Plandatechange(object sender, EventArgs e)
        {
            DateTimeControl plannedDate = null;
            if (sender is DateTimeControl)
            {
                plannedDate = (DateTimeControl)sender;
            }

            if (plannedDate != null && !(plannedDate.IsDateEmpty))
            {
                switch (plannedDate.ID)
                {
                    case "PlandateProjectAuthorization":
                        PlandateDefine.MinDate = PlandateProjectAuthorization.SelectedDate;
                        PlandateDefine.Enabled = true;
                        break;
                    case "PlandateDefine":
                        PlandateMeasure.MinDate = PlandateDefine.SelectedDate;
                        PlandateMeasure.Enabled = true;
                        break;
                    case "PlandateMeasure":
                        PlandateAnalyze.MinDate = PlandateMeasure.SelectedDate;
                        PlandateAnalyze.Enabled = true;
                        break;
                    case "PlandateAnalyze":
                        PlandateImprove.MinDate = PlandateAnalyze.SelectedDate;
                        PlandateImprove.Enabled = true;
                        break;
                    case "PlandateImprove":
                        PlandateControl.MinDate = PlandateImprove.SelectedDate;
                        PlandateControl.Enabled = true;
                        break;
                    case "PlandateControl":
                        PlandateFinalReportApprove.MinDate = PlandateControl.SelectedDate;
                        PlandateFinalReportApprove.Enabled = true;
                        break;
                    case "PlandateFinalReportApprove":
                        break;
                }

            }
        }

       
        protected void lnkResolve_Click(object sender, EventArgs e)
        {

            BlackbeltuserEditor.Validate();

            if (Hidden1.Value == "true")
            {

            }



        }
        protected void lnkResolve2_Click(object sender, EventArgs e)
        {

            GreenbeltuserEditor.Validate();

            if (Hidden1.Value == "true")
            {

            }



        }
    }
}
