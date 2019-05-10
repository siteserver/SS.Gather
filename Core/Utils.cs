using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;

namespace SS.Gather.Core
{
    public static class Utils
    {
        public const string PluginId = "SS.Gather";

        public static string PagePlaceHolder = "[SITESERVER_PAGE]";//内容翻页占位符

        public static string GetChannelListBoxTitle(int siteId, int channelId, string nodeName, int parentsCount, bool isLastNode, IList<bool> isLastNodeArray)
        {
            var str = string.Empty;
            if (channelId == siteId)
            {
                isLastNode = true;
            }
            if (isLastNode == false)
            {
                isLastNodeArray[parentsCount] = false;
            }
            else
            {
                isLastNodeArray[parentsCount] = true;
            }
            for (var i = 0; i < parentsCount; i++)
            {
                str = string.Concat(str, isLastNodeArray[i] ? "　" : "│");
            }

            str = string.Concat(str, isLastNode ? "└" : "├");
            str = string.Concat(str, Utils.MaxLengthText(nodeName, 8));

            return str;
        }

        public static string MaxLengthText(string inputString, int maxLength, string endString = "...")
        {
            var retVal = inputString;
            try
            {
                if (maxLength > 0)
                {
                    var decodedInputString = HttpUtility.HtmlDecode(retVal);
                    retVal = decodedInputString;

                    var totalLength = maxLength * 2;
                    var length = 0;
                    var builder = new StringBuilder();

                    var isOneBytesChar = false;
                    var lastChar = ' ';

                    if (!string.IsNullOrEmpty(retVal))
                    {
                        foreach (var singleChar in retVal.ToCharArray())
                        {
                            builder.Append(singleChar);

                            if (IsTwoBytesChar(singleChar))
                            {
                                length += 2;
                                if (length >= totalLength)
                                {
                                    lastChar = singleChar;
                                    break;
                                }
                            }
                            else
                            {
                                length += 1;
                                if (length == totalLength)
                                {
                                    isOneBytesChar = true;//已经截取到需要的字数，再多截取一位
                                }
                                else if (length > totalLength)
                                {
                                    lastChar = singleChar;
                                    break;
                                }
                                else
                                {
                                    isOneBytesChar = !isOneBytesChar;
                                }
                            }
                        }
                    }
                    if (isOneBytesChar && length > totalLength)
                    {
                        builder.Length--;
                        var theStr = builder.ToString();
                        retVal = builder.ToString();
                        if (char.IsLetter(lastChar))
                        {
                            for (var i = theStr.Length - 1; i > 0; i--)
                            {
                                var theChar = theStr[i];
                                if (!IsTwoBytesChar(theChar) && char.IsLetter(theChar))
                                {
                                    retVal = retVal.Substring(0, i - 1);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            //int index = retVal.LastIndexOfAny(new char[] { ' ', '\t', '\n', '\v', '\f', '\r', '\x0085' });
                            //if (index != -1)
                            //{
                            //    retVal = retVal.Substring(0, index);
                            //}
                        }
                    }
                    else
                    {
                        retVal = builder.ToString();
                    }

                    var isCut = decodedInputString != retVal;
                    retVal = HttpUtility.HtmlEncode(retVal);

                    if (isCut && endString != null)
                    {
                        retVal += endString;
                    }
                }
            }
            catch
            {
                // ignored
            }

            return retVal;
        }

        private static Encoding Gb2312 { get; } = Encoding.GetEncoding("gb2312");

        private static bool IsTwoBytesChar(char chr)
        {
            // 使用中文支持编码
            return Gb2312.GetByteCount(new[] { chr }) == 2;
        }

        public static bool IsDirectoryExists(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        public static string GetDirectoryPath(string path)
        {
            var ext = Path.GetExtension(path);
            var directoryPath = !string.IsNullOrEmpty(ext) ? Path.GetDirectoryName(path) : path;
            return directoryPath;
        }

        public static void CreateDirectoryIfNotExists(string path)
        {
            var directoryPath = GetDirectoryPath(path);

            if (!IsDirectoryExists(directoryPath))
            {
                try
                {
                    Directory.CreateDirectory(directoryPath);
                }
                catch
                {
                    //Scripting.FileSystemObject fso = new Scripting.FileSystemObjectClass();
                    //string[] directoryNames = directoryPath.Split('\\');
                    //string thePath = directoryNames[0];
                    //for (int i = 1; i < directoryNames.Length; i++)
                    //{
                    //    thePath = thePath + "\\" + directoryNames[i];
                    //    if (StringUtils.Contains(thePath.ToLower(), ConfigUtils.Instance.PhysicalApplicationPath.ToLower()) && !IsDirectoryExists(thePath))
                    //    {
                    //        fso.CreateFolder(thePath);
                    //    }
                    //}                    
                }
            }
        }
    }
}
