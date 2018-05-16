using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using System.Collections;
using System.Text;
using System.Web;
using Microsoft.SharePoint.Administration;
using System.Security.Cryptography;
using System.IO;
using System.Net.Mail;

namespace PWC.Process.SixSigma.wp_EmailCommentNotification
{
    public partial class wp_EmailCommentNotificationUserControl : UserControl
    {
        string TeamMemberEmail = string.Empty;
        string TeamRoleName = string.Empty;
        string SiteTitle = string.Empty;
        string UserDisplayName = string.Empty;
        int RowId = 0;
        int Id = 0;
        int SigmaId = 0;
        SPListItem oSPListItem = null;
        string sixSigmaListName = "BreakThroughProcertProjectsTracking";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(Request.QueryString["ProjectId"])))
            {
                SigmaId = Convert.ToInt32(Request.QueryString["ProjectId"]);
                RowId = Convert.ToInt32(Request.QueryString["RowId"]);
                Id = Convert.ToInt32(Request.QueryString["UserID"]);
                TeamRoleName = (Convert.ToString(Request.QueryString["TeamRoleName"]));
                SiteTitle = (Convert.ToString(Request.QueryString["SiteTitle"]));
                UserDisplayName = (Convert.ToString(Request.QueryString["UserDisplayName"]));
            }

        }

        protected void btnSentEmailwithComment(object sender, EventArgs e)
        {
            string EmailComments = txtEmailComment.Text;
            SPUser UseLoginName = SPContext.Current.Web.SiteUsers.GetByID(Convert.ToInt32(Id));
            TeamMemberEmail = UseLoginName.Email;
            SendEmailtoSelectedUsers(EmailComments, TeamMemberEmail, TeamRoleName, SiteTitle, UserDisplayName);
            oSPListItem = GeSixSigmaDataByID(SigmaId);
            string Action = "Notification Sent";
            string PrviousActionLogs = Convert.ToString(oSPListItem["ProjectOverAllComments"]);
            oSPListItem["ProjectOverAllComments"] = Environment.NewLine + Action + "|" + SPContext.Current.Web.CurrentUser.Name + " | " + DateTime.Now + "|" + txtEmailComment.Text + "|" + "" + "|" + SPContext.Current.Web.CurrentUser.Name + "|##|" + PrviousActionLogs;
            oSPListItem.Update();
            RedirectOnEmail("Commit");
        }

        private void RedirectOnEmail(string result)
        {
            HttpContext context = HttpContext.Current;
            if (result == "Commit")
                context.Response.Write("<script type='text/javascript'>window.frameElement.commitPopup()</script>");
            else
                context.Response.Write("<script type='text/javascript'>window.frameElement.cancelPopUp()</script>");
            context.Response.Flush();
            context.Response.End();
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

        private void SendEmailtoSelectedUsers(string EmailComments,string TeamMember, string TeamRole, string SiteTitle, string ToUserName)
        {
            try
            {
                ArrayList toEmail = new ArrayList();
                string Subject = string.Empty;
                string TeamMemberEmailId = TeamMember;
                toEmail.Add(TeamMemberEmailId);
                StringBuilder strbody = new StringBuilder();
                SPListItem Item = GeSixSigmaDataByID(SigmaId);
                Subject = "Notification: You are required to work on Six Sigma #" + SigmaId;
                strbody = new StringBuilder();
                strbody.Append("<p><span style='font-family:Calibri;'>Dear <b>_ToName_</b>,</span></p>");
                strbody.Append("<p><span style='font-family:Calibri;'>You have been selected as a <b> _Role_ </b> to work on project  <b> _SiteTitle_ </b>.</span></p>");
                strbody.Append("<p><a href='_formURL_'><span style='font-family:Calibri;'>Click here</span></a>");
                strbody.Append("<span style='font-family:Calibri;'>&nbsp;to access the project details and take action.</span></p>");
                strbody.Append("<span style='font-family:Calibri;'><b>Comments:</b> _EmailComments_</span></p>");
                strbody.Append("<p><span style='font-family:Calibri;'>Thank you.</span></p>");
              
                string _id = HttpUtility.UrlEncode(Encrypt(Convert.ToString(SigmaId)));
                string ItemsiteUrl = SPContext.Current.Web.Url + "/SitePages/BreakThroughProcertProjectsTracking.aspx?ProjectId=" + _id;
                strbody.Replace("_formURL_", ItemsiteUrl);
                strbody.Replace("_FormNumber_", Convert.ToString(SigmaId));
                strbody.Replace("_SiteTitle_", Convert.ToString(Item["ProjectName"]));
                strbody.Replace("_Role_", TeamRole);
                strbody.Replace("_EmailComments_", EmailComments);
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
                string CurrentUserEmail = SPContext.Current.Web.CurrentUser.Email;
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

        protected void btnCloseEmail(object sender, EventArgs e)
        {
            RedirectOnEmail("Cancel");
        }
    }
}
