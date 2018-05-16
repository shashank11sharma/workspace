using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;

namespace PWC.Process.SixSigma
{
    class Utility
    {
        public static long GetFolderSize(SPFolder folder)
        {
            long folderSize = 0;
            foreach (SPFile file in folder.Files)
            {
                folderSize += file.Length;
            }
            foreach (SPFolder subfolder in folder.SubFolders)
            {
                folderSize += GetFolderSize(subfolder);
            }
            return folderSize;
        }

        public static int GetNumberOfFilesInFolder(SPFolder folder)
        {
            int folderNum = 0;
            foreach (SPFile file in folder.Files)
            {
                folderNum += 1;
            }
            foreach (SPFolder subfolder in folder.SubFolders)
            {
                folderNum += GetNumberOfFilesInFolder(subfolder);
            }
            return folderNum;
        }

        public static List<FileInfo> GetFilesInFolder(SPFolder folder)
        {
            List<FileInfo> result = new List<FileInfo>();
            FileInfo fileinfo;
            foreach (SPFile file in folder.Files)
            {
                fileinfo = new FileInfo();
                fileinfo.Name = file.Name;
               // fileinfo.Size = file.Length / 1024;
                fileinfo.URL = file.Url;
                fileinfo.IconURL = file.IconUrl;
                fileinfo.File = file;
                result.Add(fileinfo);
            }
            return result;
        }

        public static List<FolderInfo> GetFoldersInFolder(SPFolder folder)
        {
            List<FolderInfo> result = new List<FolderInfo>();
            FolderInfo folderinfo;
            SPFolderCollection subFolders = folder.SubFolders;
            foreach (SPFolder subFolder in subFolders)
            {
                folderinfo = new FolderInfo();
                folderinfo.Name = subFolder.Name;
                //folderinfo.Size = GetFolderSize(subFolder) / 1024;
                folderinfo.URL = subFolder.Url;
                //folderinfo.FilesNumber = GetNumberOfFilesInFolder(subFolder);
                result.Add(folderinfo);

            }
            return result;
        }

        public static TreeNode GetFolderNode(TreeView treeView, bool IsBrowseOnUpload,TreeNode node, SPFolder folder, string baseURL)
        {
            List<FolderInfo> folders = GetFoldersInFolder(folder);
            folders.Sort(new FolderInfoComparer(SortDirection.Ascending));
            TreeNode folderNode;
            for (int j = 0; j <= folders.Count - 1; j++)
            {
                folderNode = new TreeNode();
                //folderNode.NavigateUrl = baseURL + "/" + folders[j].URL;
                if (treeView.ID == "TreeViewDoc")
                {
                    folderNode.NavigateUrl = "javascript:void(0);";
                }
                else
                {
                    folderNode.NavigateUrl = "javascript:clickNodeFolder(this,'" + baseURL + "/" + folders[j].URL + "','" + folders[j].Name + "')";
                }
                folderNode.ImageUrl = baseURL + "/_layouts/images/folder.gif";
                folderNode.Text = folders[j].Name;
                if (folders[j].Name.ToLower() != "forms" && folders[j].Name.ToLower() !="documents")
                {
                   // folderNode.ToolTip = "Size:" + folders[j].Size.ToString() + " KBs " + " Files:" + folders[j].FilesNumber.ToString();
                    SPFolder subfolder = folder.SubFolders[folders[j].URL];
                    folderNode.ChildNodes.Add(GetFolderNode(treeView,IsBrowseOnUpload, folderNode, subfolder, baseURL));
                    node.ChildNodes.Add(folderNode);
                }
            }
            if (treeView.ID=="TreeViewDoc")
            {
            
            TreeNode fileNode;
            List<FileInfo> files = GetFilesInFolder(folder);
            files.Sort(new FileInfoComparer(SortDirection.Ascending));
            for (int i = 0; i <= files.Count - 1; i++)
            {
                fileNode = new TreeNode();
                // fileNode.ShowCheckBox = true;
                fileNode.ImageUrl = baseURL + "/_layouts/images/" + files[i].IconURL;
                fileNode.NavigateUrl = baseURL + "/" + files[i].URL;
                fileNode.Text = files[i].Name;
                //fileNode.ToolTip = "Size:" + files[i].Size + " KBs ";
                fileNode.NavigateUrl = "javascript:clickNode(this,'" + baseURL + "/" + files[i].URL + "','" + files[i].Name + "')";
                node.ChildNodes.Add(fileNode);
            }
            }
            return node;
        }


        public static SPField CreateSiteColumn(SPWeb web, string displayName,
        SPFieldType fieldType, string groupDescriptor)
        {
            if (!web.Fields.ContainsField(displayName))
            {
                string fieldName = web.Fields.Add(displayName, fieldType, false);
                SPField field = web.Fields.GetFieldByInternalName(fieldName);
                field.Group = groupDescriptor;
                field.Update();
                return field;
            }
            return web.Fields[displayName];
        }


    }
}
