using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace SP2016.Repository.Service
{
    public class FolderService
    {
        private ListService ListService = new ListService();

        public virtual string CreateName(string name)
        {
            Debug.Assert(!string.IsNullOrEmpty(name));

            var folderName = name.Replace(Constants.Constants.CTRL.COMMA, "");
            folderName = folderName.Replace(Constants.Constants.CTRL.COLON, "");

            folderName = folderName.Replace(Constants.Constants.CTRL.TILDA, "");
            folderName = folderName.Replace(Constants.Constants.CTRL.QUOTES, "");

            folderName = folderName.Replace(Constants.Constants.CTRL.SHARP, " ");
            folderName = folderName.Replace(Constants.Constants.CTRL.AND, " ");
            folderName = folderName.Replace(Constants.Constants.CTRL.OR, " ");
            folderName = folderName.Replace(Constants.Constants.CTRL.MULTIPLICATION, " ");
            folderName = folderName.Replace(Constants.Constants.CTRL.DIVISION, " ");
            folderName = folderName.Replace(Constants.Constants.CTRL.LESS, " ");
            folderName = folderName.Replace(Constants.Constants.CTRL.MORE, " ");
            folderName = folderName.Replace(Constants.Constants.CTRL.QUESTION, " ");
            folderName = folderName.Replace(Constants.Constants.CTRL.BRACE.OPEN, " ");
            folderName = folderName.Replace(Constants.Constants.CTRL.BRACE.CLOSE, " ");
            folderName = folderName.Replace(Constants.Constants.CTRL.SLASH, " ");
            folderName = folderName.Replace(Constants.Constants.CTRL.BACK_SLASH, " ");

            folderName = folderName.Replace(Constants.Constants.CTRL.DOT + Constants.Constants.CTRL.DOT, " ");

            if (folderName.First() == Constants.Constants.CTRL.DOT.First())
            {
                folderName = folderName.Substring(1);
            }

            if (folderName.Last() == Constants.Constants.CTRL.DOT.First())
            {
                folderName = folderName.Substring(0, folderName.Length - 1);
            }

            return folderName.Trim();
        }

        /// <summary>
        /// Создать папку
        /// </summary>
        /// <param name="web">Узел, на котором необходимо создать папку</param>
        /// <param name="list">Список, в котором создается папка</param>
        /// <param name="parentFolderRelativeUrl">Если параметр не задан, то папка будет создана в корневой папке списка</param>
        /// <param name="newFolderTitle">Название создаваемой папки</param>
        public SPFolder CreateFolder(SPWeb web, SPList list, string parentFolderRelativeUrl, string newFolderTitle)
        {
            Contract.Requires(!String.IsNullOrEmpty(newFolderTitle));

            string folderUrl = !String.IsNullOrEmpty(parentFolderRelativeUrl)
                                ? SPUtility.ConcatUrls(list.RootFolder.ServerRelativeUrl, parentFolderRelativeUrl)
                                : list.RootFolder.ServerRelativeUrl;

            newFolderTitle = CreateName(newFolderTitle);
            SPListItem newFolderItem = list.Items.Add(folderUrl, SPFileSystemObjectType.Folder, newFolderTitle);
            newFolderItem["Title"] = newFolderTitle;

            web.AllowUnsafeUpdates = true;
            newFolderItem.Update();
            web.AllowUnsafeUpdates = false;

            return newFolderItem.Folder;
        }

        /// <summary>
        /// Создать папку
        /// </summary>
        /// <param name="web">Узел, на котором необходимо создать папку</param>
        /// <param name="list">Url-адрес списка, в котором создается папка</param>
        /// <param name="parentFolderRelativeUrl">Если параметр не задан, то папка будет создана в корневой папке списка</param>
        /// <param name="newFolderTitle">Название создаваемой папки</param>
        public SPFolder CreateFolder(SPWeb web, string listUrl, string parentFolderRelativeUrl, string newFolderTitle)
        {
            return CreateFolder(web, web.GetList(listUrl), parentFolderRelativeUrl, newFolderTitle);
        }

        /// <summary>
        /// Создание иерархии папок в списке
        /// </summary>
        /// <param name="web">Узел, на котором необходимо создать путь к папке</param>
        /// <param name="list">Список, для которого создается иерархия папок</param>
        /// <param name="folderRelativeUrl">Путь к папкам, создающим иерархию. Например folder1/folder2/folder3</param>
        public void CreatePath(SPWeb web, SPList list, string folderRelativeUrl)
        {
            Contract.Requires(web != null);
            Contract.Requires(list != null);
            Contract.Requires(!string.IsNullOrEmpty(folderRelativeUrl));

            string[] folders = folderRelativeUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            SPFolder RootFolder = list.RootFolder;
            web.AllowUnsafeUpdates = true;
            for (int i = 0; i < folders.Length; i++)
            {
                string folderName = CreateName(folders[i]);
                string folderUrl = SPUtility.ConcatUrls(RootFolder.Url, folderName);

                if (FolderExists(web, folderUrl))
                {
                    RootFolder = web.GetFolder(folderUrl);
                    continue;
                }
                else
                {
                    string url = SPUtility.ConcatUrls(web.ServerRelativeUrl, RootFolder.Url);

                    SPListItem newItem = list.Items.Add(url, SPFileSystemObjectType.Folder, folderName);
                    newItem["Title"] = folderName;
                    newItem.Update();
                    RootFolder.Update();
                    RootFolder = newItem.Folder;
                }
            }

            list.RootFolder.Update();
            // list.Update();
            //  web.Update();

            web.AllowUnsafeUpdates = false;
        }

        /// <summary>
        /// Возвращает папку по URL-адресу
        /// </summary>
        /// <param name="web">Узел, на котором необходимо найти папку</param>
        /// <param name="list">Список, в котором необходимо найти папку</param>
        /// <param name="folderUrl">URL-адрес папки, относительно корневой папки списка</param>
        public SPFolder GetFolderByUrl(SPWeb web, SPList list, string folderUrl)
        {
            SPFolder folder = web.GetFolder(SPUtility.ConcatUrls(list.RootFolder.Url, folderUrl));
            if (folder.Exists)
                return folder;
            else
                return null;
        }

        /// <summary>
        /// Возвращает папку списка по URL-адресу
        /// </summary>
        /// <param name="web">Узел, на котором необходимо найти папку</param>
        /// <param name="list">URL-адрес списка, в котором необходимо найти папку</param>
        /// <param name="folderUrl">URL-адрес папки, относительно корневой папки списка</param>
        public SPFolder GetFolderByUrl(SPWeb web, string listUrl, string folderUrl)
        {
            string url = SPUtility.ConcatUrls(listUrl, folderUrl);
            SPFolder folder = web.GetFolder(url);
            if (folder.Exists)
                return folder;
            else
                return null;
        }

        public SPFolder EnsureFolder(SPWeb web, string listUrl, string folderUrl)
        {
            SPFolder folder = GetFolderByUrl(web, listUrl, folderUrl);
            string listServerRelativeUrl = SPUtility.ConcatUrls(web.ServerRelativeUrl, listUrl);
            if (folder == null)
                folder = CreateFolder(web, listServerRelativeUrl, null, folderUrl);
            return folder;
        }

        public bool FolderExists(SPWeb web, string folderRelativeUrl)
        {
            SPFolder folder = web.GetFolder(folderRelativeUrl);
            if (folder != null)
                return folder.Exists;
            else
                return false;
        }

        public void DeleteFolder(SPWeb web, string folderUrl)
        {
            Contract.Requires(!string.IsNullOrEmpty(folderUrl));

            SPFolder folder = web.GetFolder(folderUrl);
            if (folder.Exists)
                folder.Delete();
        }
    }
}
