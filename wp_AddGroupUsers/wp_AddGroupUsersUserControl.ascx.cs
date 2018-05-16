using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Microsoft.SharePoint.Administration;
using System.Net.Mail;
using Microsoft.Office.Server.UserProfiles;
using PWC.Process.SixSigma;
using System.Security.Cryptography;
using System.IO;

namespace PWC.Process.SixSigma.wp_AddGroupUsers
{
    public partial class wp_AddGroupUsersUserControl : UserControl
    {
        int PassportCategoryId = 0;
        int LanguageId = 1;
        string MeetingCat = string.Empty;
        string ApplicationType = string.Empty;
        SPWeb currentWeb = SPContext.Current.Web;
        string SiteUrl = SPContext.Current.Site.Url;
        string Usr = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(Request.QueryString["SigmaId"])))
                {
                    PassportCategoryId = Convert.ToInt32(Convert.ToString(Request.QueryString["SigmaId"]));
                    MeetingCat = Convert.ToString(Request.QueryString["Group"]);
                    Usr = Convert.ToString(Request.QueryString["User"]);
                   
                }

                SPGroup testingOwnersGroup = SPContext.Current.Web.SiteGroups[MeetingCat];
                SPUserCollection userColl = testingOwnersGroup.Users;
                DataTable dtUsers = new DataTable();
                DataColumn dcName = new DataColumn("Name", typeof(string));
                DataColumn dcLoginName = new DataColumn("LoginName", typeof(string));
                dtUsers.Columns.Add(dcName);
                dtUsers.Columns.Add(dcLoginName);
                dtUsers.AcceptChanges();
                foreach (SPUser user in userColl)
                {

                    if (!(user.Name == "NT AUTHORITY\\Authenticated Users") && !(user.IsDomainGroup) && !(user.Name == "System Account"))
                    {
                        DataRow dr = dtUsers.NewRow();
                        dr["Name"] = user.Name;
                        dr["LoginName"] = user.LoginName;
                        dtUsers.Rows.Add(dr);
                        dtUsers.AcceptChanges();
                    }

                }

                LB_MainSelection.DataSource = dtUsers;
                LB_MainSelection.DataTextField = "Name";
                LB_MainSelection.DataValueField = "LoginName";
                LB_MainSelection.DataBind();

                LB_CentralData.DataSource = dtUsers;
                LB_CentralData.DataTextField = "Name";
                LB_CentralData.DataValueField = "LoginName";
                LB_CentralData.DataBind();

               
                if (!Page.IsPostBack)
                {
                    if (!(string.IsNullOrEmpty(Usr)) && !(Usr.Equals("undefined")))
                    {


                        SPUser myUser = currentWeb.EnsureUser(Usr);
                        DataTable SelectedUsers = new DataTable();
                        DataColumn SelctddcName = new DataColumn("Name", typeof(string));
                        DataColumn SelctddcLoginName = new DataColumn("LoginName", typeof(string));
                        SelectedUsers.Columns.Add(SelctddcName);
                        SelectedUsers.Columns.Add(SelctddcLoginName);
                        SelectedUsers.AcceptChanges();
                        DataRow dr = SelectedUsers.NewRow();
                        dr["Name"] = myUser.Name;
                        dr["LoginName"] = myUser.LoginName;
                        SelectedUsers.Rows.Add(dr);
                        SelectedUsers.AcceptChanges();
                        LB_SelectedUserList.DataSource = SelectedUsers;
                        LB_SelectedUserList.DataTextField = "Name";
                        LB_SelectedUserList.DataValueField = "LoginName";
                        LB_SelectedUserList.DataBind();
                    }

                    var hdnhidden1 = HiddenField1.ClientID;
                    var hdnhidden2 = HiddenField2.ClientID;

                    string script = string.Format("var hdn1 = '{0}';var hdn2 = '{1}';", hdnhidden1, hdnhidden2);

                    Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "myScript", script, true);
                }
            }
            catch (Exception ex)
            {
                ULSLogger.LogErrorInULS("Inside catch in Page_Load() in PWC.Process.SixSigma Feature..Error is--" + ex.Message);
            }
        }

        
        protected void btnOk_Click(object sender, EventArgs e)
        {
            UpdateAssignedUsers();
        }

        private void UpdateAssignedUsers()
        {
            try
            {
                ArrayList external_attendee = new ArrayList();

                string[] in_input = HiddenField1.Value.Split(',');
                string[] ex_input = HiddenField2.Value.Split(',');

                for (int inattend = 0; inattend < in_input.Length && inattend < ex_input.Length; inattend++)
                {

                    string in_attend = in_input[inattend].ToString().ToLower();
                    string ex_attend = ex_input[inattend].ToString().ToLower();
                    string newExtuser = ex_attend.Replace("[external]", "").Trim(); ;
                    if (in_attend.Equals(newExtuser))
                    {
                        external_attendee.Add(ex_input[inattend].ToString().Replace("[External]", "").Trim());
                    }

                    else
                    {

                        SPUser requireduser = currentWeb.EnsureUser(in_attend);
                        hiddenPreviousUser.Value = Convert.ToString(requireduser.ID);


                    }
                }


                RedirectOnOK();
            }

            catch (Exception Ex) { ULSLogger.LogErrorInULS("Inside catch in btnOk_Click() in PWC.Process.SixSigma Feature..Error is--" + Ex.Message); }

        }


        protected void btnCancel_Click(object sender, EventArgs e)
        {
            RedirectOnOK();
        }

        private void RedirectOnOK()
        {
            ScriptManager.RegisterStartupScript(updatePanel, updatePanel.GetType(), "close", "window.frameElement.commitPopup();", true);
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
    }
}
