using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using System.Xml;
using Microsoft.SharePoint.WebPartPages;
using System.Web.UI.WebControls.WebParts;
using System.Globalization;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Taxonomy;

namespace PWC.Process.SixSigma.Features.Feature1
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("b6838461-a164-4222-9605-8efe6c1b3a82")]
    public class Feature1EventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite siteCollection = properties.Feature.Parent as SPSite)
                {
                    using (SPWeb web = siteCollection.OpenWeb())
                    {
                        SPList list = web.Lists["Documents"];
                        string memGrpName = "Folder_Templates_Member";
                        string memPermName = "PWC Contribute";
                        string visGrpName = "Folder_Templates_Visitor";
                        string visPermName = "Read";
                        string blackbeltGroupName = "BlackBelt";
                        string greenbeltGroupName = "GreenBelt";
                        FormHomePage(siteCollection.OpenWeb().Url + "/SitePages/BreakThroughProcertProjectsTracking.aspx", "Header", "BreakThroughProcertProjectsTracking.aspx", "");
                        AddGroupUsers(siteCollection.OpenWeb().Url + "/SitePages/AddGroupUsers.aspx", "Header", "", "");
                        EmailCommentNotification(siteCollection.OpenWeb().Url + "/SitePages/EmailNotification.aspx", "Header", "EmailNotification.aspx", "");
                        CreateList(web, "Lookup_Organization_List", "This List Is used In BreakThroughProCertProjectsTracking.", "Lookup_Organization_List", "Lookup_Organization_List");
                        CreateList(web, "Lookup_ProcertMultilingual_List", "This List Is used In BreakThroughProCertProjectsTracking.", "Lookup_ProcertMultilingual_List", "Lookup_ProcertMultilingual_List");
                        CreateList(web, "Lookup_Plant_List", "This List Is used In BreakThroughProcertProjectsTracking.", "Lookup_Plant_List", "Lookup_Plant_List");
                        CreateList(web, "Lookup_ProjectType_List", "This List Is used In BreakThroughProcertProjectsTracking.", "Lookup_ProjectType_List", "Lookup_ProjectType_List");
                        CreateList(web, "Lookup_ProjectTeamData_List", "This List Is used In BreakThroughProcertProjectsTracking.", "Lookup_ProjectTeamData_List", "Lookup_ProjectTeamData_List");
                        CreateList(web, "Lookup_ProjectTeamRole_List", "This List Is used In BreakThroughProcertProjectsTracking.", "Lookup_ProjectTeamRole_List", "Lookup_ProjectTeamRole_List");
                        CreateList(web, "BreakThroughProcertProjectsTracking", "This List Is used In BreakThroughProcertProjectsTracking.", "BreakThrough Procert Projects Tracking", "BreakThroughProcertProjectsTracking");
                        CreateList(web, "Lookup_OtherAttachments_List", "This List Is used In BreakThroughProCertProjectsTracking.", "Lookup_OtherAttachments_List", "Lookup_OtherAttachments_List");
                        CreateList(web, "Lookup_ProcertEmailConfiguration", "This List Is used In BreakThroughProCertProjectsTracking.", "Lookup_ProcertEmailConfiguration", "Lookup_ProcertEmailConfiguration");

                        CreateList(web, "Lookup_Metricsarea_List", "This List Is used In BreakThroughProCertProjectsTracking.", "Lookup_Metricsarea_List", "Lookup_Metricsarea_List");

                        CreateDiscussionList(web);
                        DeletecolumninDiscussion(web);
                        AddColumnToDiscussion(web);

                        if (!ContainsGroup(web.SiteGroups, blackbeltGroupName))
                            CreateSubSiteGroup(web, blackbeltGroupName, memPermName, "This is Six Sigma Black Belt Group");
                        if (!ContainsGroup(web.SiteGroups, greenbeltGroupName))
                            CreateSubSiteGroup(web, greenbeltGroupName, memPermName, "This is Six Sigma Green Belt Group");
                        if (!ContainsGroup(web.SiteGroups, memGrpName))
                            CreateSFGroup(web, memGrpName, memPermName, "member");
                        if (!ContainsGroup(web.SiteGroups, visGrpName))
                            CreateSFGroup(web, visGrpName, visPermName, "visitor");
                        SPFolderCollection spFolderColl = list.RootFolder.SubFolders;
                        if (!web.GetFolder(web.ServerRelativeUrl + list.RootFolder + "/Templates").Exists)
                        {
                            SPFolder folder = spFolderColl.Add("Templates");
                            SPContentType secFolCT = list.ContentTypes["PWC Secure Folder"];
                            folder.Item[SPBuiltInFieldId.ContentTypeId] = secFolCT.Id;
                            folder.Item.IconOverlay = "SecureFolderIcon.png";
                            folder.Item.SystemUpdate();
                            string[] FolderUrls = { "Info", "Info/Background", "Info/Problem Statement", "Info/Project Metrics", "Info/Benefits", "Info/Costs", "Info/Financial Analysis", "Info/Milestones", "Updates", "Updates/Quad Charts", "Gates", "Gates/Define", "Gates/Measure", "Gates/Analyze", "Gates/Improve", "Gates/Control", "Final Report" };
                            CreateSubFolders(FolderUrls, folder);
                            SetFolderPermissions(web, "Templates", memGrpName, visGrpName, blackbeltGroupName, greenbeltGroupName, folder);
                        }

                        GrantPermissionOnSitePages(web);


                    }


                }
            });
        }


         
        private void AddGroupUsers(string pageUrl, string zoneId, string Title, string Description)
        {

            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {

                    using (SPSite sharepointSite = new SPSite(pageUrl))
                    {

                        using (SPWeb sharepointWeb = sharepointSite.OpenWeb())
                        {

                            sharepointWeb.AllowUnsafeUpdates = true;
                            SPFile file = sharepointWeb.GetFile(pageUrl);
                            if (null != file)
                            {

                                using (SPLimitedWebPartManager mgr = file.GetLimitedWebPartManager(PersonalizationScope.Shared))
                                {
                                    if (null != mgr)
                                    {
                                        SPQuery query = new SPQuery();
                                        query.Query = String.Format(CultureInfo.CurrentCulture, "<Where><Eq><FieldRef Name='Title'/><Value Type='Text'>wp_AddGroupUsers</Value></Eq></Where>");
                                        SPList webPartGallery = sharepointWeb.GetCatalog(SPListTemplateType.WebPartCatalog);
                                        SPListItemCollection webPartGalCol = webPartGallery.GetItems(query);
                                        XmlReader xmlReader = new XmlTextReader(webPartGalCol[0].File.OpenBinaryStream());
                                        string errorMessage;
                                        System.Web.UI.WebControls.WebParts.WebPart SiteProcertForm = mgr.ImportWebPart(xmlReader, out errorMessage);
                                        SiteProcertForm.Title = Title;
                                        SiteProcertForm.Description = Description;
                                        SiteProcertForm.ChromeState = System.Web.UI.WebControls.WebParts.PartChromeState.Normal;
                                        SiteProcertForm.ChromeType = System.Web.UI.WebControls.WebParts.PartChromeType.None;
                                        mgr.AddWebPart(SiteProcertForm, "", SiteProcertForm.ZoneIndex);
                                        sharepointWeb.Update();

                                    }
                                }
                            }
                        }
                    }
                });

            }
            catch (Exception ex)
            {
                ex.ToString();

            }
        }

        private static void AddCustomAction(SPList _list, string _title)
        {
            if (_list != null)
            {
                var action = _list.UserCustomActions.Add();
                action.Location = "CommandUI.Ribbon.ListView";
                action.Sequence = 20;
                action.Title = _title;
                action.CommandUIExtension = string.Format(@"<CommandUIExtension><CommandUIDefinitions>
                                    <CommandUIDefinition Location=""Ribbon.ListItem.Manage.ViewProperties"">
                                    </CommandUIDefinition>
                                    <CommandUIDefinition Location=""Ribbon.ListItem.Manage.EditProperties"">
                                    </CommandUIDefinition>
                                    <CommandUIDefinition Location=""Ribbon.ListItem.New.NewListItem"">
                                    </CommandUIDefinition>
                                    </CommandUIDefinitions></CommandUIExtension>");

                action.Update();
            }
            _list.Update();
        }

        private static void RemoveCustomAction(SPList _list, string _title)
        {
            if (_list != null)
            {
                foreach (SPUserCustomAction action in _list.UserCustomActions)
                {
                    if (action.Title == _title)
                    {
                        action.Delete();
                        break;
                    }
                }
            }
            _list.Update();
        }

        private void CreateDiscussionList(SPWeb web)
        {
            SPList list = web.Lists.TryGetList("BreakThroughProcertProjectsTrackingDiscussions");
            try
            {
                web.AllowUnsafeUpdates = true;
                if (list == null)
                {
                    web.AllowUnsafeUpdates = true;


                    web.Lists.Add("BreakThroughProcertProjectsTrackingDiscussions", "This List Is used In BreakThroughProcertProjectsTracking", SPListTemplateType.DiscussionBoard);


                    web.AllowUnsafeUpdates = false;
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Inside outer catch in createDiscussionList in PWC Six Sigma..Error is--" + ex.Message);
            }


        }

        private void DeletecolumninDiscussion(SPWeb web)
        {
            SPList list = web.Lists.TryGetList("BreakThroughProcertProjectsTrackingDiscussions");
            try
            {
                web.AllowUnsafeUpdates = true;
                if (list != null)
                {
                    list.EnableAttachments = false;
                    SPField field = list.Fields["Contains Technical Data?"];
                    if (field != null)
                    {
                        list.Fields.Delete(field.StaticName);

                        list.Update();
                    }
                }
                web.AllowUnsafeUpdates = false;
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Inside outer catch in deletecolumninDiscussion in PWC MeetingSpace..Error is--" + ex.Message);
            }

        }

        private static void AddColumnToDiscussion(SPWeb web)
        {
            try
            {
                SPList list = web.Lists.TryGetList("BreakThroughProcertProjectsTrackingDiscussions");
                web.AllowUnsafeUpdates = true;
                SPFieldNumber fldEmpID1 = (SPFieldNumber)list.Fields.CreateNewField(
                SPFieldType.Number.ToString(), "SigmaId");
                fldEmpID1.ShowInViewForms = true;
                SPFieldCollection fields = list.Fields;

                if (!fields.ContainsField("SigmaId"))
                {
                    list.Fields.Add(fldEmpID1);
                }
                list.Update();
                web.AllowUnsafeUpdates = false;
            }
            catch (Exception ex)
            {

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

        private static void SetFolderPermissions(SPWeb web, string folderName, string memGrpName, string visGrpName, string blackbeltGroupName, string greenbeltGroupName, SPFolder fldr)
        {
            try
            {


                SPRoleDefinition pwcContribute = web.RoleDefinitions["PWC Contribute"];

                SPRoleDefinition read = web.RoleDefinitions["Read"];

                SPRoleDefinition viewOnly = web.RoleDefinitions["View Only"];

                SPRoleDefinition pwcBronzeAdmin = web.RoleDefinitions["PWC Bronze Admin"];

                SPRoleDefinition fullControl = web.RoleDefinitions["Full Control"];

                SPGroup newMemGroup = web.SiteGroups[memGrpName];

                SPGroup newVisGroup = web.SiteGroups[visGrpName];
                SPGroup BBGroup = web.SiteGroups[blackbeltGroupName];
                SPGroup GBGroup = web.SiteGroups[greenbeltGroupName];

                SPListItem li = fldr.Item;
                if (li.Name.Equals(folderName))
                {
                    li.BreakRoleInheritance(false);
                    while (li.RoleAssignments.Count > 0)
                    {
                        li.RoleAssignments.Remove(0);
                    }
                    SPRoleAssignment spRoleAssignMemGrp = new SPRoleAssignment(newMemGroup);
                    spRoleAssignMemGrp.RoleDefinitionBindings.Add(pwcContribute);
                    li.RoleAssignments.Add(spRoleAssignMemGrp);

                    SPRoleAssignment spRoleAssignVisGrp = new SPRoleAssignment(newVisGroup);
                    spRoleAssignVisGrp.RoleDefinitionBindings.Add(read);
                    li.RoleAssignments.Add(spRoleAssignVisGrp);

                    SPRoleAssignment spRoleAssignBBGroup = new SPRoleAssignment(BBGroup);
                    spRoleAssignBBGroup.RoleDefinitionBindings.Add(read);
                    li.RoleAssignments.Add(spRoleAssignBBGroup);

                    SPRoleAssignment spRoleAssignGBGroup = new SPRoleAssignment(GBGroup);
                    spRoleAssignGBGroup.RoleDefinitionBindings.Add(read);
                    li.RoleAssignments.Add(spRoleAssignGBGroup);
                    //SPRoleAssignment spRoleAssignBronzeAdminGrp = new SPRoleAssignment(bronzeAdminGroup);
                    //spRoleAssignBronzeAdminGrp.RoleDefinitionBindings.Add(pwcBronzeAdmin);
                    //spRoleAssignBronzeAdminGrp.RoleDefinitionBindings.Add(pwcContribute);
                    // li.RoleAssignments.Add(spRoleAssignBronzeAdminGrp);

                    //SPRoleAssignment spRoleAssignOwnerGrp = new SPRoleAssignment(ownerGroup);
                    //spRoleAssignOwnerGrp.RoleDefinitionBindings.Add(fullControl);
                    // li.RoleAssignments.Add(spRoleAssignOwnerGrp);


                }
            }
            catch (Exception e)
            {
                e.Message.ToString();
                // ULSLogger.LogErrorInULS("Inside catch in setFolderPermissions() in PWC Secure Folder Feature---" + e.Message);
            }
        }

        protected bool ContainsPermLevel(SPWeb web, string permLevel)
        {
            try
            {
                SPRoleDefinition roleDef = web.RoleDefinitions[permLevel];
                return true;
            }
            catch (SPException e)
            {
                e.Message.ToString();
                ULSLogger.LogErrorInULS("Inside catch in ContainsPermLevel() in PWC Secure Folder Feature----" + e.Message);
                return false;
            }
        }

        protected void CreateSFGroup(SPWeb web, string grpName, string permName, string member_visitor)
        {
            web.AllowUnsafeUpdates = true;
            SPMember bronzeAdminMember = null;
            try
            {
                ULSLogger.LogErrorInULS("Inside try in createGroup() in PWC Secure Folder Feature");
                if (ContainsPermLevel(web, permName))
                {
                    SPGroupCollection sgc = web.SiteGroups;
                    string adminGrpName = string.Empty;
                    foreach (SPGroup grp in sgc)
                    {
                        adminGrpName = grp.Name;
                        if (adminGrpName.EndsWith("Bronze Admin"))
                        {
                            ULSLogger.LogErrorInULS("Inside foreach if in createGroup() in PWC Secure Folder Feature.. Found Bronze Admin group---name is---" + adminGrpName);
                            bronzeAdminMember = grp;
                        }
                    }
                    if (member_visitor.Equals("member"))
                    {
                        web.SiteGroups.Add(grpName, bronzeAdminMember, null, "Members for the folder " + grpName);

                    }
                    if (member_visitor.Equals("visitor"))
                    {
                        web.SiteGroups.Add(grpName, bronzeAdminMember, null, "Visitors for the folder " + grpName);
                    }
                }
            }
            catch (Exception e)
            {
                e.Message.ToString();
                ULSLogger.LogErrorInULS("Inside catch in createGroup() in PWC Secure Folder Feature---" + e.Message);
            }
        }

        protected bool ContainsGroup(SPGroupCollection groupCollection, string index)
        {
            try
            {
                SPGroup testGroup = groupCollection[index];
                return true;
            }
            catch (SPException e)
            {
                e.Message.ToString();
                ULSLogger.LogErrorInULS("Inside catch in ContainsGroup() in PWC Secure Folder Feature----" + e.Message);
                return false;
            }
        }

        private void FormHomePage(string pageUrl, string zoneId, string Title, string Description)
        {

            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {

                    using (SPSite sharepointSite = new SPSite(pageUrl))
                    {

                        using (SPWeb sharepointWeb = sharepointSite.OpenWeb())
                        {

                            sharepointWeb.AllowUnsafeUpdates = true;
                            SPFile file = sharepointWeb.GetFile(pageUrl);
                            if (null != file)
                            {

                                using (SPLimitedWebPartManager mgr = file.GetLimitedWebPartManager(PersonalizationScope.Shared))
                                {
                                    if (null != mgr)
                                    {
                                        SPQuery query = new SPQuery();
                                        query.Query = String.Format(CultureInfo.CurrentCulture, "<Where><Eq><FieldRef Name='Title'/><Value Type='Text'>wp_SixSigma</Value></Eq></Where>");
                                        SPList webPartGallery = sharepointWeb.GetCatalog(SPListTemplateType.WebPartCatalog);
                                        SPListItemCollection webPartGalCol = webPartGallery.GetItems(query);
                                        XmlReader xmlReader = new XmlTextReader(webPartGalCol[0].File.OpenBinaryStream());
                                        string errorMessage;
                                        System.Web.UI.WebControls.WebParts.WebPart SiteAuditForm = mgr.ImportWebPart(xmlReader, out errorMessage);
                                        SiteAuditForm.Title = Title;
                                        SiteAuditForm.Description = Description;
                                        SiteAuditForm.ChromeState = System.Web.UI.WebControls.WebParts.PartChromeState.Normal;
                                        SiteAuditForm.ChromeType = System.Web.UI.WebControls.WebParts.PartChromeType.None;
                                        mgr.AddWebPart(SiteAuditForm, "", SiteAuditForm.ZoneIndex);
                                        sharepointWeb.Update();

                                    }
                                }
                            }
                        }
                    }
                });

            }
            catch (Exception ex)
            {
                ex.ToString();

            }
        }

        private void EmailCommentNotification(string pageUrl, string zoneId, string Title, string Description)
        {

            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {

                    using (SPSite sharepointSite = new SPSite(pageUrl))
                    {

                        using (SPWeb sharepointWeb = sharepointSite.OpenWeb())
                        {

                            sharepointWeb.AllowUnsafeUpdates = true;
                            SPFile file = sharepointWeb.GetFile(pageUrl);
                            if (null != file)
                            {

                                using (SPLimitedWebPartManager mgr = file.GetLimitedWebPartManager(PersonalizationScope.Shared))
                                {
                                    if (null != mgr)
                                    {
                                        SPQuery query = new SPQuery();
                                        query.Query = String.Format(CultureInfo.CurrentCulture, "<Where><Eq><FieldRef Name='Title'/><Value Type='Text'>wp_EmailCommentNotification</Value></Eq></Where>");
                                        SPList webPartGallery = sharepointWeb.GetCatalog(SPListTemplateType.WebPartCatalog);
                                        SPListItemCollection webPartGalCol = webPartGallery.GetItems(query);
                                        XmlReader xmlReader = new XmlTextReader(webPartGalCol[0].File.OpenBinaryStream());
                                        string errorMessage;
                                        System.Web.UI.WebControls.WebParts.WebPart SiteAuditForm = mgr.ImportWebPart(xmlReader, out errorMessage);
                                        SiteAuditForm.Title = Title;
                                        SiteAuditForm.Description = Description;
                                        SiteAuditForm.ChromeState = System.Web.UI.WebControls.WebParts.PartChromeState.Normal;
                                        SiteAuditForm.ChromeType = System.Web.UI.WebControls.WebParts.PartChromeType.None;
                                        mgr.AddWebPart(SiteAuditForm, "", SiteAuditForm.ZoneIndex);
                                        sharepointWeb.Update();

                                    }
                                }
                            }
                        }
                    }
                });

            }
            catch (Exception ex)
            {
                ex.ToString();

            }
        }

        public static SPList CreateList(SPWeb web, string listName, string description, string displayName, string templateName)
        {

            SPList list = web.Lists.TryGetList(listName);
            try
            {
                if (list == null)
                {
                    web.AllowUnsafeUpdates = true;

                    var lstTemp = web.Site.GetCustomListTemplates(web);

                    var template = lstTemp[templateName];

                    var listId = web.Lists.Add(listName, description, template);

                    list = web.Lists[listId];
                    list.Title = listName;

                    if (listName.Equals("BreakThroughProcertProjectsTracking"))
                    {
                        string[] colName = {"Bacground Attachments", "Problem Attachments", "ProjectMetrics Attachments", "Benifits Attachments", "Costs Attachments", "Financial Attachments", "Milestones Attachments", "Define Attachments", "Measure Attachments", "Analyze Attachments", "Investigate Attachments", "Control Attachments", "FinalReport Attachments" };
                       
                        for (int i = 0; i < colName.Length; i++)
                        {
                            SPFieldMultiLineText multiLineField = (SPFieldMultiLineText)list.Fields.CreateNewField(
                            SPFieldType.Note.ToString(), colName[i]);
                            multiLineField.RichText = true;
                            multiLineField.RichTextMode = SPRichTextMode.FullHtml;
                            multiLineField.ShowInEditForm = false;
                            multiLineField.ShowInNewForm = false;
                            multiLineField.ShowInDisplayForm = true;
                            multiLineField.ShowInViewForms = true;
                            SPFieldCollection fields = list.Fields;
                            if (!fields.ContainsField(colName[i]))
                            {
                                list.Fields.Add(multiLineField);
                            }
                            list.Update();
                            //int viewsCount = list.Views.Count;
                            //for (int j = 0; j < viewsCount; j++)
                            //{
                            //    SPView view = list.Views[j];
                            //    if (!view.ViewFields.Exists(colName[j]))
                            //    {
                            //        view.ViewFields.Add(list.Fields[colName[i]]);
                            //        view.Update();
                            //    }
                            //}


                        }

                    }

                    
                    list.Update();
                    if (listName.Equals("BreakThroughProcertProjectsTracking"))
                    {
                        AddCustomAction(list, "HideProperties");
                        SPFieldCollection Tagfields = list.Fields;
                        string[] TagcolName = { "Tags" };
                        if (!Tagfields.ContainsField(TagcolName[0]))
                        {
                            TaxonomySession taxonomySession = new TaxonomySession(SPContext.Current.Site);
                            TermStore termStore = taxonomySession.TermStores["Managed Metadata Service"];
                            termStore = taxonomySession.DefaultSiteCollectionTermStore;
                            Group GroupTerm = termStore.CreateGroup("BreakThroughProcertProjectsTracking");
                            TermSet termset = GroupTerm.CreateTermSet("Tags");
                            termStore.CommitAll();
                            TaxonomyField taxonomyField = list.Fields.CreateNewField("TaxonomyFieldType", "Tags") as TaxonomyField;
                            taxonomyField.AllowMultipleValues = true;
                            list.Fields.Add(taxonomyField);
                            TaxonomyField field = list.Fields["Tags"] as TaxonomyField;
                            field.Title = "Tags";
                            field.Update(true);
                            taxonomyField.SspId = termset.Id;
                            field.Update(true);

                        }
                    }
                   
                    list.Update();
                    web.AllowUnsafeUpdates = false;
                }
            }

            catch (Exception ex)
            {
                //  ULSLogger.LogErrorInULS(ex.Message, TraceSeverity.Unexpected);
            }

            return list;
        }

        public static void CreateSubSiteGroup(SPWeb web, string groupName, string PermissionLevel, string groupDescription)
        {
            try
            {
                web.AllowUnsafeUpdates = true;
                SPUserCollection users = web.AllUsers;
                SPUser owner = web.SiteAdministrators[0];
                SPMember member = web.SiteAdministrators[0];
                SPGroupCollection groups = web.SiteGroups;
                groups.Add(groupName, member, owner, groupDescription);
                SPGroup newSPGroup = groups[groupName];
                newSPGroup.OnlyAllowMembersViewMembership = false;
                newSPGroup.Update();
                SPRoleDefinition role = web.RoleDefinitions[PermissionLevel];
                SPRoleAssignment roleAssignment = new SPRoleAssignment(newSPGroup);
                roleAssignment.RoleDefinitionBindings.Add(role);
                web.RoleAssignments.Add(roleAssignment);
                web.Update();
                web.AllowUnsafeUpdates = false;
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS(ex.Message, TraceSeverity.Unexpected);
            }
        }

        public static void GrantPermissionOnSitePages(SPWeb web)
        {
            try
            {
                ULSLogger.LogErrorInULS("Inside try in GrantPermissionOnSitePages in PWC SixSigma Solution..");
                SPGroup BlackbeltGroup = web.SiteGroups["BlackBelt"];
                SPGroup GreenbeltGroup = web.SiteGroups["GreenBelt"];


                string path = SPContext.Current.Web.ServerRelativeUrl;
                int pos = path.LastIndexOf("/") + 1;
                string grpStartName = path.Substring(pos, path.Length - pos);

                SPGroup BronzeAdminGroup = web.SiteGroups[grpStartName + " Bronze Admin"];
                SPGroup ownersGroup = web.SiteGroups[grpStartName + " Owners"];
                SPList sitePageslist = web.Lists["Site Pages"];

                SPRoleDefinition pwcContribute = web.RoleDefinitions["PWC Contribute"];
                SPRoleDefinition viewOnly = web.RoleDefinitions["View Only"];
                SPRoleDefinition fullControl = web.RoleDefinitions["Full Control"];
                SPRoleDefinition pwclimited = web.RoleDefinitions["Limited Access"];

                SPListItemCollection spColl = sitePageslist.Items;


                string WelcomePage = web.RootFolder.WelcomePage;
                string SplitedPage = WelcomePage.Split('/')[1];

                foreach (SPListItem oSPListItem in spColl)
                {
                    try
                    {
                        if (oSPListItem.Name == "BreakThroughProcertProjectsTracking.aspx" || oSPListItem.Name == SplitedPage)
                        {
                            oSPListItem.BreakRoleInheritance(true);
                            ULSLogger.LogErrorInULS("Inside try in GrantPermissionOnSitePages in PWC SixSigma Solution..Got the item with the name--" + oSPListItem.Name);

                            SPRoleAssignment BronzeAdminRoleAssignment = new SPRoleAssignment(BronzeAdminGroup);

                            BronzeAdminRoleAssignment.RoleDefinitionBindings.Add(pwcContribute);
                            BronzeAdminRoleAssignment.RoleDefinitionBindings.Remove(pwclimited);
                            BronzeAdminRoleAssignment.RoleDefinitionBindings.Remove(viewOnly);
                            oSPListItem.RoleAssignments.Add(BronzeAdminRoleAssignment);
                            oSPListItem.Update();




                            SPRoleAssignment BlackbeltRoleAssignment = new SPRoleAssignment(BlackbeltGroup);

                            BlackbeltRoleAssignment.RoleDefinitionBindings.Add(pwcContribute);
                            BlackbeltRoleAssignment.RoleDefinitionBindings.Remove(pwclimited);
                            BlackbeltRoleAssignment.RoleDefinitionBindings.Remove(viewOnly);
                            oSPListItem.RoleAssignments.Add(BlackbeltRoleAssignment);
                            oSPListItem.Update();

                            SPRoleAssignment GreenbeltRoleAssignment = new SPRoleAssignment(GreenbeltGroup);

                            GreenbeltRoleAssignment.RoleDefinitionBindings.Add(pwcContribute);
                            GreenbeltRoleAssignment.RoleDefinitionBindings.Remove(pwclimited);
                            GreenbeltRoleAssignment.RoleDefinitionBindings.Remove(viewOnly);
                            oSPListItem.RoleAssignments.Add(GreenbeltRoleAssignment);
                            oSPListItem.Update();


                        }

                    }
                    catch (Exception gpPermExc)
                    {
                        ULSLogger.LogErrorInULS("Inside foreach catch in GrantPermissionOnSitePages in PWC WorkReleaseManagement Solution. Error adding permission----" + gpPermExc);
                    }
                }
            }
            catch (Exception gpExc)
            {
                ULSLogger.LogErrorInULS("Inside catch in GrantPermissionOnSitePages in PWC WorkReleaseManagement Solution. Error is----" + gpExc.Message);
            }
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                using (SPSite siteCollection = properties.Feature.Parent as SPSite)
                {
                    using (SPWeb web = siteCollection.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;
                        SPList Pageslistdetails = web.Lists["Site Pages"];
                        int first = -1, second = -1, third = -1, fourth = -1;
                        for (int i = Pageslistdetails.Items.Count - 1; i >= 0; i--)
                        {
                            if (Pageslistdetails.Items[i].Name == "BreakThroughProcertProjectsTracking.aspx")
                            {
                                first = Pageslistdetails.Items[i].ID;

                            }

                            if (Pageslistdetails.Items[i].Name == "EmailNotification.aspx")
                            {
                                second = Pageslistdetails.Items[i].ID;

                            }
                            if (Pageslistdetails.Items[i].Name == "AddGroupUsers.aspx")
                            {
                                third = Pageslistdetails.Items[i].ID;

                            }

                        }
                        if (first != -1)
                            Pageslistdetails.Items.DeleteItemById(first);
                        if (second != -1)
                            Pageslistdetails.Items.DeleteItemById(second);
                        if (third != -1)
                            Pageslistdetails.Items.DeleteItemById(third);
                        if (fourth != -1)
                            Pageslistdetails.Items.DeleteItemById(fourth);

                        Pageslistdetails.Update();

                        web.AllowUnsafeUpdates = false;
                    }
                }
            });
        }
    }
}


