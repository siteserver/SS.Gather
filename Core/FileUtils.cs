using SiteServer.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS.Gather.Core
{
    public static class FileUtils
    {
        //public static string TextEditorContentEncode(ISiteInfo siteInfo, string content, bool isSaveImage)
        //{
        //    if (siteInfo == null) return content;

        //    if (isSaveImage && !string.IsNullOrEmpty(content))
        //    {
        //        content = PathUtility.SaveImage(siteInfo, content);
        //    }

        //    var builder = new StringBuilder(content);

        //    var url = siteInfo.WebUrl;
        //    if (!string.IsNullOrEmpty(url) && url != "/")
        //    {
        //        StringUtils.ReplaceHrefOrSrc(builder, url, "@");
        //    }
        //    //if (!string.IsNullOrEmpty(url))
        //    //{
        //    //    StringUtils.ReplaceHrefOrSrc(builder, url, "@");
        //    //}

        //    var relatedSiteUrl = PageUtils.ParseNavigationUrl($"~/{siteInfo.SiteDir}");
        //    StringUtils.ReplaceHrefOrSrc(builder, relatedSiteUrl, "@");

        //    builder.Replace("@'@", "'@");
        //    builder.Replace("@\"@", "\"@");

        //    return builder.ToString();
        //}

        //public static string SaveImage(ISiteInfo siteInfo, string content)
        //{
        //    var originalImageSrcs = RegexUtils.GetOriginalImageSrcs(content);
        //    foreach (var originalImageSrc in originalImageSrcs)
        //    {
        //        if (!PageUtils.IsProtocolUrl(originalImageSrc) ||
        //            StringUtils.StartsWithIgnoreCase(originalImageSrc, PageUtils.ApplicationPath) ||
        //            StringUtils.StartsWithIgnoreCase(originalImageSrc, siteInfo.Additional.WebUrl))
        //            continue;
        //        var fileExtName = PageUtils.GetExtensionFromUrl(originalImageSrc);
        //        if (!EFileSystemTypeUtils.IsImageOrFlashOrPlayer(fileExtName)) continue;

        //        var fileName = GetUploadFileName(siteInfo, originalImageSrc);
        //        var directoryPath = GetUploadDirectoryPath(siteInfo, fileExtName);
        //        var filePath = PathUtils.Combine(directoryPath, fileName);

        //        try
        //        {
        //            if (!FileUtils.IsFileExists(filePath))
        //            {
        //                WebClientUtils.SaveRemoteFileToLocal(originalImageSrc, filePath);
        //                if (EFileSystemTypeUtils.IsImage(PathUtils.GetExtension(fileName)))
        //                {
        //                    FileUtility.AddWaterMark(siteInfo, filePath);
        //                }
        //            }
        //            var fileUrl = PageUtility.GetSiteUrlByPhysicalPath(siteInfo, filePath, true);
        //            content = content.Replace(originalImageSrc, fileUrl);
        //        }
        //        catch
        //        {
        //            // ignored
        //        }
        //    }
        //    return content;
        //}

        //public static string GetUploadFileName(SiteInfo siteInfo, string filePath)
        //{
        //    var fileExtension = PathUtils.GetExtension(filePath);

        //    var isUploadChangeFileName = siteInfo.Additional.IsFileUploadChangeFileName;
        //    if (IsImageExtenstionAllowed(siteInfo, fileExtension))
        //    {
        //        isUploadChangeFileName = siteInfo.Additional.IsImageUploadChangeFileName;
        //    }
        //    else if (IsVideoExtenstionAllowed(siteInfo, fileExtension))
        //    {
        //        isUploadChangeFileName = siteInfo.Additional.IsVideoUploadChangeFileName;
        //    }

        //    return GetUploadFileName(filePath, isUploadChangeFileName);
        //}

        public static bool IsFileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public static bool DeleteFileIfExists(string filePath)
        {
            var retVal = true;
            try
            {
                if (IsFileExists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch
            {
                //try
                //{
                //    Scripting.FileSystemObject fso = new Scripting.FileSystemObjectClass();
                //    fso.DeleteFile(filePath, true);
                //}
                //catch
                //{
                //    retval = false;
                //}
                retVal = false;
            }
            return retVal;
        }
    }
}
