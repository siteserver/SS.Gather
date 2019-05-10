using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using Datory;
using SiteServer.Plugin;

namespace SS.Gather.Core
{
    public static class GatherUtility
    {
        /*
         * 通用：.*?
         * 所有链接：<a\s*.*?href=(?:"(?<url>[^"]*)"|'(?<url>[^']*)'|(?<url>\S+)).*?>
         * */

        private static RegexOptions Options = RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace;

        private static List<string> GetImageSrcList(string baseUrl, string html)
        {
            var regex = "(img|input)[^><]*\\s+src\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*))";
            return GetUrls(regex, html, baseUrl);
        }

        private static List<string> GetOriginalImageSrcList(string html)
        {
            var regex = "(img|input)[^><]*\\s+src\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*))";
            return GetContents("url", regex, html);
        }

        private static List<string> GetFlashSrcList(string baseUrl, string html)
        {
            var regex = "embed\\s+[^><]*src\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*))|param\\s+[^><]*value\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*))";
            return GetUrls(regex, html, baseUrl);
        }

        private static List<string> GetOriginalFlashSrcList(string html)
        {
            var regex = "embed\\s+[^><]*src\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*))|param\\s+[^><]*value\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*))";
            return GetContents("url", regex, html);
        }

        private static List<string> GetStyleImageUrls(string baseUrl, string html)
        {
            var list = GetUrls("url\\((?<url>[^\\(\\)]*)\\)", html, baseUrl);
            var urlList = new List<string>();
            foreach (var url in list)
            {
                if (!urlList.Contains(url) && EFileSystemTypeUtils.IsImage(PathUtils.GetExtension(url)))
                {
                    urlList.Add(url);
                }
            }
            return urlList;
        }

        private static List<string> GetOriginalStyleImageUrls(string html)
        {
            var list = GetContents("url", "url\\((?<url>[^\\(\\)]*)\\)", html);
            var urlList = new List<string>();
            foreach (var url in list)
            {
                if (!urlList.Contains(url) && EFileSystemTypeUtils.IsImage(PathUtils.GetExtension(url)))
                {
                    urlList.Add(url);
                }
            }
            return urlList;
        }

        private static List<string> GetBackgroundImageSrcList(string baseUrl, string html)
        {
            return GetUrls("background\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*))", html, baseUrl);
        }

        private static List<string> GetOriginalBackgroundImageSrcList(string html)
        {
            return GetContents("url", "background\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*))", html);
        }

        private static List<string> GetCssHrefList(string baseUrl, string html)
        {
            //string regex = "link\\s+[^><]*href\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>\\S+))|@import\\s*url\\((?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>\\S+))\\)";

            return GetUrls("link\\s+[^><]*href\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*))|\\@import\\s*url\\s*\\(\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>.*?))\\s*\\)", html, baseUrl);
        }

        private static List<string> GetOriginalCssHrefList(string html)
        {
            return GetContents("url", "link\\s+[^><]*href\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*))|\\@import\\s*url\\s*\\(\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>.*?))\\s*\\)", html);
        }

        private static List<string> GetScriptSrcList(string baseUrl, string html)
        {
            return GetUrls("script\\s+[^><]*src\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*))", html, baseUrl);
        }

        private static List<string> GetOriginalScriptSrcList(string html)
        {
            return GetContents("url", "script\\s+[^><]*src\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*))", html);
        }

        //public static List<string> GetTagInnerContents(string tagName, string html)
        //{
        //    return GetContents("content", $"<{tagName}\\s+[^><]*>\\s*(?<content>[\\s\\S]+?)\\s*</{tagName}>", html);
        //}

        //public static List<string> GetTagContents(string tagName, string html)
        //{
        //    var list = new List<string>();

        //    var regex = $@"<({tagName})[^>]*>(.*?)</\1>|<{tagName}[^><]*/>";

        //    var matches = Regex.Matches(html, regex, RegexOptions.IgnoreCase);
        //    foreach (Match match in matches)
        //    {
        //        if (match.Success)
        //        {
        //            list.Add(match.Result("$0"));
        //        }
        //    }

        //    return list;
        //}

        //public static string GetTagName(string html)
        //{
        //    var match = Regex.Match(html, "<([^>\\s]+)[\\s\\SS]*>", RegexOptions.IgnoreCase);
        //    if (match.Success)
        //    {
        //        return match.Result("$1");
        //    }
        //    return string.Empty;
        //}

        //public static string GetInnerContent(string tagName, string html)
        //{
        //    var regex = $"<{tagName}[^><]*>(?<content>[\\s\\S]+?)</{tagName}>";
        //    return GetContent("content", regex, html);
        //}

        //public static string GetAttributeContent(string attributeName, string html)
        //{
        //    var regex =
        //        $"<[^><]+\\s*{attributeName}\\s*=\\s*(?:\"(?<value>[^\"]*)\"|'(?<value>[^']*)'|(?<value>[^>\\s]*)).*?>";
        //    return GetContent("value", regex, html);
        //}

        private static List<string> GetUrls(string html, string baseUrl)
        {
            var regex = "<a\\s*.*?href\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*)).*?>";
            return GetUrls(regex, html, baseUrl);
        }

        private static List<string> GetUrls(string regex, string html, string baseUrl)
        {
            var urlList = new List<string>();
            if (string.IsNullOrEmpty(regex))
            {
                regex = "<a\\s*.*?href\\s*=\\s*(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>[^>\\s]*)).*?>";
            }
            var groupName = "url";
            var list = GetContents(groupName, regex, html);
            foreach (var rawUrl in list)
            {
                var url = PageUtils.GetUrlByBaseUrl(rawUrl, baseUrl);
                if (!string.IsNullOrEmpty(url) && !urlList.Contains(url))
                {
                    urlList.Add(url);
                }
            }
            return urlList;
        }

        private static string GetUrl(string regex, string html, string baseUrl)
        {
            return PageUtils.GetUrlByBaseUrl(GetContent("url", regex, html), baseUrl);
        }

        private static string GetContent(string groupName, string regex, string html)
        {
            var content = string.Empty;
            if (string.IsNullOrEmpty(regex)) return content;
            if (regex.IndexOf("<" + groupName + ">", StringComparison.Ordinal) == -1)
            {
                return regex;
            }

            var reg = new Regex(regex, Options);
            var match = reg.Match(html);
            if (match.Success)
            {
                content = match.Groups[groupName].Value;
            }

            return content;
        }

        private static string Replace(string regex, string input, string replacement)
        {
            if (string.IsNullOrEmpty(input)) return input;
            var reg = new Regex(regex, Options);
            return reg.Replace(input, replacement);
        }

        //public static string Replace(string regex, string input, string replacement, int count)
        //{
        //    if (count == 0)
        //    {
        //        return Replace(regex, input, replacement);
        //    }

        //    if (string.IsNullOrEmpty(input)) return input;
        //    var reg = new Regex(regex, Options);
        //    return reg.Replace(input, replacement, count);
        //}

        private static bool IsMatch(string regex, string input)
        {
            var reg = new Regex(regex, Options);
            return reg.IsMatch(input);
        }

        private static List<string> GetContents(string groupName, string regex, string html)
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(regex)) return list;

            var reg = new Regex(regex, Options);

            for (var match = reg.Match(html); match.Success; match = match.NextMatch())
            {
                //list.Add(match.Groups[groupName].Value);
                var theValue = match.Groups[groupName].Value;
                if (!list.Contains(theValue))
                {
                    list.Add(theValue);
                }
            }
            return list;
        }

        private static string RemoveScripts(string html)
        {
            return Replace("<script[^><]*>.*?<\\/script>", html, string.Empty);
        }

        //public static string RemoveImages(string html)
        //{
        //    return Replace("<img[^><]*>", html, string.Empty);
        //}

        private static IEnumerable<string> StringCollectionToList(string collection)
        {
            return StringCollectionToList(collection, ',');
        }

        private static List<string> StringCollectionToList(string collection, char separator)
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(collection)) return list;

            var array = collection.Split(separator);
            foreach (var s in array)
            {
                list.Add(s.Trim());
            }
            return list;
        }

        public static string GetRegexString(string normalString)
        {
            var retVal = normalString;
            if (string.IsNullOrEmpty(normalString)) return retVal;

            var replaceChar = new[] { '\\', '^', '$', '.', '{', '[', '(', ')', ']', '}', '+', '?', '!', '#' };
            foreach (var theChar in replaceChar)
            {
                retVal = retVal.Replace(theChar.ToString(), "\\" + theChar);
            }
            retVal = retVal.Replace("*", ".*?");
            retVal = Replace("\\s+", retVal, "\\s+");
            return retVal;
        }

        public static string GetRegexArea(string normalAreaStart, string normalAreaEnd)
        {
            if (!string.IsNullOrEmpty(normalAreaStart) && !string.IsNullOrEmpty(normalAreaEnd))
            {
                return $"{GetRegexString(normalAreaStart)}\\s*(?<area>[\\s\\S]+?)\\s*{GetRegexString(normalAreaEnd)}";
            }
            return string.Empty;
        }

        public static string GetRegexUrl(string normalUrlStart, string normalUrlEnd)
        {
            if (!string.IsNullOrEmpty(normalUrlStart) && !string.IsNullOrEmpty(normalUrlEnd))
            {
                return
                    $"{GetRegexString(normalUrlStart)}(?:\"(?<url>[^\"]*)\"|'(?<url>[^']*)'|(?<url>\\S+)){GetRegexString(normalUrlEnd)}";
            }
            return string.Empty;
        }

        public static string GetRegexChannel(string normalChannelStart, string normalChannelEnd)
        {
            if (!string.IsNullOrEmpty(normalChannelStart) && !string.IsNullOrEmpty(normalChannelEnd))
            {
                return
                    $"{GetRegexString(normalChannelStart)}\\s*(?<channel>[\\s\\S]+?)\\s*{GetRegexString(normalChannelEnd)}";
            }
            return string.Empty;
        }

        public static string GetRegexTitle(string normalTitleStart, string normalTitleEnd)
        {
            if (!string.IsNullOrEmpty(normalTitleStart) && !string.IsNullOrEmpty(normalTitleEnd))
            {
                return
                    $"{GetRegexString(normalTitleStart)}\\s*(?<title>[\\s\\S]+?)\\s*{GetRegexString(normalTitleEnd)}";
            }
            return string.Empty;
        }

        public static string GetRegexContent(string normalContentStart, string normalContentEnd)
        {
            if (!string.IsNullOrEmpty(normalContentStart) && !string.IsNullOrEmpty(normalContentEnd))
            {
                return
                    $"{GetRegexString(normalContentStart)}\\s*(?<content>[\\s\\S]+?)\\s*{GetRegexString(normalContentEnd)}";
            }
            return string.Empty;
        }

        private static string GetRegexAttributeName(string attributeName, string normalAuthorStart, string normalAuthorEnd)
        {
            if (!string.IsNullOrEmpty(normalAuthorStart) && !string.IsNullOrEmpty(normalAuthorEnd))
            {
                return
                    $"{GetRegexString(normalAuthorStart)}\\s*(?<{attributeName}>[\\s\\S]+?)\\s*{GetRegexString(normalAuthorEnd)}";
            }
            return string.Empty;
        }

        public static List<string> GetGatherUrlList(GatherRuleInfo gatherRuleInfo)
        {
            var gatherUrls = new List<string>();
            if (gatherRuleInfo.GatherUrlIsCollection)
            {
                gatherUrls.AddRange(StringCollectionToList(gatherRuleInfo.GatherUrlCollection, separator: '\n'));
            }

            if (gatherRuleInfo.GatherUrlIsSerialize)
            {
                if (gatherRuleInfo.SerializeFrom <= gatherRuleInfo.SerializeTo)
                {
                    var count = 1;
                    for (var i = gatherRuleInfo.SerializeFrom; i <= gatherRuleInfo.SerializeTo; i = i + gatherRuleInfo.SerializeInterval)
                    {
                        count++;
                        if (count > 200) break;
                        var thePageNumber = i.ToString();
                        if (gatherRuleInfo.SerializeIsAddZero && thePageNumber.Length == 1)
                        {
                            thePageNumber = "0" + i;
                        }
                        gatherUrls.Add(gatherRuleInfo.GatherUrlSerialize.Replace("*", thePageNumber));
                    }
                }

                if (gatherRuleInfo.SerializeIsOrderByDesc)
                {
                    gatherUrls.Reverse();
                }
            }

            return gatherUrls;
        }

        private static List<string> GetContentUrlList(GatherRuleInfo gatherRuleInfo, string regexListArea, string regexUrlInclude, ProgressCache cache)
        {
            var gatherUrls = GetGatherUrlList(gatherRuleInfo);
            var contentUrls = new List<string>();

            foreach (var gatherUrl in gatherUrls)
            {
                cache.IsSuccess = true;
                cache.Message = "获取链接：" + gatherUrl;

                try
                {
                    var urls = GetContentUrls(gatherUrl, gatherRuleInfo.Charset, gatherRuleInfo.CookieString, regexListArea, regexUrlInclude);
                    contentUrls.AddRange(urls);
                }
                catch (Exception ex)
                {
                    cache.IsSuccess = false;
                    cache.Message = ex.Message;
                    cache.FailureMessages.Add(ex.Message);
                }
            }

            if (gatherRuleInfo.IsOrderByDesc)
            {
                contentUrls.Reverse();
            }
            return contentUrls;
        }

        public static List<string> GetContentUrls(string gatherUrl, string charset, string cookieString, string regexListArea, string regexUrlInclude)
        {
            var contentUrls = new List<string>();
            var listHtml = WebClientUtils.GetRemoteFileSource(gatherUrl, ECharsetUtils.GetEnumType(charset), cookieString);
            var areaHtml = string.Empty;

            if (!string.IsNullOrEmpty(regexListArea))
            {
                areaHtml = GetContent("area", regexListArea, listHtml);
            }

            var urlsList = GetUrls(!string.IsNullOrEmpty(areaHtml) ? areaHtml : listHtml, gatherUrl);

            var isInclude = !string.IsNullOrEmpty(regexUrlInclude);

            foreach (var url in urlsList)
            {
                if (!string.IsNullOrEmpty(url))
                {
                    var contentUrl = url.Replace("&amp;", "&");
                    if (isInclude && !IsMatch(regexUrlInclude, contentUrl))
                    {
                        continue;
                    }
                    if (!contentUrls.Contains(contentUrl))
                    {
                        contentUrls.Add(contentUrl);
                    }
                }
            }
            return contentUrls;
        }

        public static NameValueCollection GetContentNameValueCollection(string charset, string url, string cookieString, string regexContentExclude, string contentHtmlClearCollection, string contentHtmlClearTagCollection, string regexTitle, string regexContent, string regexContent2, string regexContent3, string regexNextPage, string regexChannel, List<string> contentAttributes, NameValueCollection contentAttributesXml)
        {
            var attributes = new NameValueCollection();

            var contentHtml = WebClientUtils.GetRemoteFileSource(url, ECharsetUtils.GetEnumType(charset), cookieString);
            var title = GetContent("title", regexTitle, contentHtml);
            var content = GetContent("content", regexContent, contentHtml);
            if (string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(regexContent2))
            {
                content = GetContent("content", regexContent2, contentHtml);
            }
            if (string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(regexContent3))
            {
                content = GetContent("content", regexContent3, contentHtml);
            }

            if (!string.IsNullOrEmpty(regexContentExclude))
            {
                content = Replace(regexContentExclude, content, string.Empty);
            }
            if (!string.IsNullOrEmpty(contentHtmlClearCollection))
            {
                var htmlClearList = StringCollectionToList(contentHtmlClearCollection);
                foreach (var htmlClear in htmlClearList)
                {
                    var clearRegex = $@"<{htmlClear}[^>]*>.*?<\/{htmlClear}>";
                    content = Replace(clearRegex, content, string.Empty);
                }
            }
            if (!string.IsNullOrEmpty(contentHtmlClearTagCollection))
            {
                var htmlClearTagList = StringCollectionToList(contentHtmlClearTagCollection);
                foreach (var htmlClearTag in htmlClearTagList)
                {
                    var clearRegex = $@"<{htmlClearTag}[^>]*>";
                    content = Replace(clearRegex, content, string.Empty);
                    clearRegex = $@"<\/{htmlClearTag}>";
                    content = Replace(clearRegex, content, string.Empty);
                }
            }

            var contentNextPageUrl = GetUrl(regexNextPage, contentHtml, url);
            if (!string.IsNullOrEmpty(contentNextPageUrl))
            {
                content = GetPageContent(content, charset, contentNextPageUrl, cookieString, regexContentExclude, contentHtmlClearCollection, contentHtmlClearTagCollection, regexContent, regexContent2, regexContent3, regexNextPage);
            }

            var channel = GetContent("channel", regexChannel, contentHtml);

            attributes.Add("title", title);
            attributes.Add("channel", channel);
            attributes.Add("content", StringUtils.HtmlEncode(content));

            foreach (var attributeName in contentAttributes)
            {
                var normalStart = StringUtils.ValueFromUrl(contentAttributesXml[attributeName + "_ContentStart"]);
                var normalEnd = StringUtils.ValueFromUrl(contentAttributesXml[attributeName + "_ContentEnd"]);
                var regex = GetRegexAttributeName(attributeName, normalStart, normalEnd);
                var value = GetContent(attributeName, regex, contentHtml);
                attributes.Set(attributeName, value);
            }

            return attributes;
        }

        private static bool GatherOneByUrl(ISiteInfo siteInfo, IChannelInfo channelInfo, bool isSaveImage, bool isSetFirstImageAsImageUrl, bool isEmptyContentAllowed, bool isSameTitleAllowed, bool isChecked, string charset, string url, string cookieString, string regexTitleInclude, string regexContentExclude, string contentHtmlClearCollection, string contentHtmlClearTagCollection, string contentReplaceFrom, string contentReplaceTo, string regexTitle, string regexContent, string regexContent2, string regexContent3, string regexNextPage, string regexChannel, IEnumerable<string> contentAttributes, NameValueCollection contentAttributesXml, IDictionary<int, IList<string>> contentTitleDict, ICollection<KeyValuePair<int, int>> channelIdAndContentIdList, IAdministratorInfo adminInfo, out string title, out string errorMessage)
        {
            title = string.Empty;
            errorMessage = string.Empty;

            try
            {
                //TODO:采集文件、链接标题为内容标题、链接提示为内容标题
                //string extension = PathUtils.GetExtension(url);
                //if (!EFileSystemTypeUtils.IsTextEditable(extension))
                //{
                //    if (EFileSystemTypeUtils.IsImageOrFlashOrPlayer(extension))
                //    {

                //    }
                //}
                var tableName = Context.ContentApi.GetTableName(siteInfo.Id, channelInfo.Id);
                var contentHtml = WebClientUtils.GetRemoteFileSource(url, ECharsetUtils.GetEnumType(charset), cookieString);
                title = GetContent("title", regexTitle, contentHtml);
                var content = GetContent("content", regexContent, contentHtml);
                if (string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(regexContent2))
                {
                    content = GetContent("content", regexContent2, contentHtml);
                }
                if (string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(regexContent3))
                {
                    content = GetContent("content", regexContent3, contentHtml);
                }

                //如果标题或内容为空，返回false并退出
                if (string.IsNullOrEmpty(title))
                {
                    errorMessage = $"无法获取标题：{url}";
                    return false;
                }
                if (isEmptyContentAllowed == false && string.IsNullOrEmpty(content))
                {
                    errorMessage = $"无法获取内容正文：{url}";
                    return false;
                }

                title = StringUtils.StripTags(title);

                if (!string.IsNullOrEmpty(regexTitleInclude))
                {
                    if (IsMatch(regexTitleInclude, title) == false)
                    {
                        errorMessage = $"标题不符合要求：{url}";
                        return false;
                    }
                }
                if (!string.IsNullOrEmpty(regexContentExclude))
                {
                    content = Replace(regexContentExclude, content, string.Empty);
                }
                if (!string.IsNullOrEmpty(contentHtmlClearCollection))
                {
                    var htmlClearList = StringCollectionToList(contentHtmlClearCollection);
                    foreach (var htmlClear in htmlClearList)
                    {
                        var clearRegex = $@"<{htmlClear}[^>]*>.*?<\/{htmlClear}>";
                        content = Replace(clearRegex, content, string.Empty);
                    }
                }
                if (!string.IsNullOrEmpty(contentHtmlClearTagCollection))
                {
                    var htmlClearTagList = StringCollectionToList(contentHtmlClearTagCollection);
                    foreach (var htmlClearTag in htmlClearTagList)
                    {
                        var clearRegex = $@"<{htmlClearTag}[^>]*>";
                        content = Replace(clearRegex, content, string.Empty);
                        clearRegex = $@"<\/{htmlClearTag}>";
                        content = Replace(clearRegex, content, string.Empty);
                    }
                }

                if (!string.IsNullOrEmpty(contentReplaceFrom))
                {
                    var fromList = TranslateUtils.StringCollectionToStringCollection(contentReplaceFrom);
                    var isMulti = false;
                    if (!string.IsNullOrEmpty(contentReplaceTo) && contentReplaceTo.IndexOf(value: ',') != -1)
                    {
                        if (StringUtils.GetCount(",", contentReplaceTo) + 1 == fromList.Count)
                        {
                            isMulti = true;
                        }
                    }
                    if (isMulti == false)
                    {
                        foreach (var from in fromList)
                        {
                            title = Replace($"({from.Replace(" ", "\\s")})(?!</a>)(?![^><]*>)", title, contentReplaceTo);
                            content = Replace($"({from.Replace(" ", "\\s")})(?!</a>)(?![^><]*>)", content, contentReplaceTo);
                        }
                    }
                    else
                    {
                        var tos = TranslateUtils.StringCollectionToStringCollection(contentReplaceTo);
                        for (var i = 0; i < fromList.Count; i++)
                        {
                            title = Replace($"({fromList[i].Replace(" ", "\\s")})(?!</a>)(?![^><]*>)", title, tos[i]);
                            content = Replace($"({fromList[i].Replace(" ", "\\s")})(?!</a>)(?![^><]*>)", content, tos[i]);
                        }
                    }
                }

                var contentNextPageUrl = GetUrl(regexNextPage, contentHtml, url);
                if (!string.IsNullOrEmpty(contentNextPageUrl))
                {
                    try
                    {
                        content = GetPageContent(content, charset, contentNextPageUrl, cookieString, regexContentExclude, contentHtmlClearCollection, contentHtmlClearTagCollection, regexContent, regexContent2, regexContent3, regexNextPage);
                    }
                    catch (Exception ex)
                    {
                        errorMessage = ex.Message;
                        return false;
                    }
                }

                var channel = GetContent("channel", regexChannel, contentHtml);
                var channelId = channelInfo.Id;
                if (!string.IsNullOrEmpty(channel))
                {
                    var channelIdByNodeName = 0;

                    var childChannelIdList = Context.ChannelApi.GetChannelIdList(siteInfo.Id, channelInfo.Id);
                    foreach (var childChannelId in childChannelIdList)
                    {
                        if (channel == Context.ChannelApi.GetChannelName(siteInfo.Id, childChannelId))
                        {
                            channelIdByNodeName = childChannelId;
                        }
                    }

                    //var channelIdByNodeName = ChannelManager.GetChannelIdByParentIdAndChannelName(siteInfo.Id, channelInfo.Id, channel, recursive: false);
                    if (channelIdByNodeName == 0)
                    {
                        var newChannelInfo = Context.ChannelApi.NewInstance(siteInfo.Id);

                        newChannelInfo.ParentId = channelInfo.Id;
                        newChannelInfo.ChannelName = channel;
                        newChannelInfo.ContentModelPluginId = channelInfo.ContentModelPluginId;

                        channelId = Context.ChannelApi.Insert(siteInfo.Id, newChannelInfo);
                    }
                    else
                    {
                        channelId = channelIdByNodeName;
                    }
                }

                if (!isSameTitleAllowed)
                {
                    if (!contentTitleDict.TryGetValue(channelId, out var contentTitleList))
                    {
                        var repository = new Repository(Context.Environment.DatabaseType,
                            Context.Environment.ConnectionString, tableName);
                        contentTitleList = repository.GetAll<string>(Q.Select(ContentAttribute.Title)
                            .Where(ContentAttribute.ChannelId, channelId));
                    }

                    if (contentTitleList.Contains(title))
                    {
                        errorMessage = $"已包含相同标题：{title}";
                        return false;
                    }

                    contentTitleList.Add(title);
                    contentTitleDict[channelId] = contentTitleList;
                }

                var contentInfo = Context.ContentApi.NewInstance(siteInfo.Id, channelId);

                contentInfo.AdminId = adminInfo.Id;
                contentInfo.AddUserName = adminInfo.UserName;
                contentInfo.AddDate = DateTime.Now;
                contentInfo.LastEditUserName = contentInfo.AddUserName;
                contentInfo.LastEditDate = contentInfo.AddDate;
                contentInfo.Checked = isChecked;
                contentInfo.CheckedLevel = 0;

                contentInfo.Title = title;

                foreach (var attributeName in contentAttributes)
                {
                    if (!StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.Title) && !StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.Content))
                    {
                        var normalStart = StringUtils.ValueFromUrl(contentAttributesXml[attributeName + "_ContentStart"]);
                        var normalEnd = StringUtils.ValueFromUrl(contentAttributesXml[attributeName + "_ContentEnd"]);

                        //采集为空时的默认值
                        var normalDefault = StringUtils.ValueFromUrl(contentAttributesXml[attributeName + "_ContentDefault"]);

                        var regex = GetRegexAttributeName(attributeName, normalStart, normalEnd);
                        var value = GetContent(attributeName, regex, contentHtml);

                        //采集为空时的默认值
                        if (string.IsNullOrEmpty(value))
                        {
                            value = normalDefault;
                        }

                        if (ContentAttribute.AllAttributes.Value.Contains(attributeName))
                        {
                            if (StringUtils.EqualsIgnoreCase(ContentAttribute.AddDate, attributeName))
                            {
                                contentInfo.AddDate = TranslateUtils.ToDateTime(value, DateTime.Now);
                            }
                            else if (StringUtils.EqualsIgnoreCase(ContentAttribute.IsColor, attributeName))
                            {
                                contentInfo.Color = TranslateUtils.ToBool(value, defaultValue: false);
                            }
                            else if (StringUtils.EqualsIgnoreCase(ContentAttribute.IsHot, attributeName))
                            {
                                contentInfo.Hot = TranslateUtils.ToBool(value, defaultValue: false);
                            }
                            else if (StringUtils.EqualsIgnoreCase(ContentAttribute.IsRecommend, attributeName))
                            {
                                contentInfo.Recommend = TranslateUtils.ToBool(value, defaultValue: false);
                            }
                            else if (StringUtils.EqualsIgnoreCase(ContentAttribute.IsTop, attributeName))
                            {
                                contentInfo.Top = TranslateUtils.ToBool(value, defaultValue: false);
                            }
                            else if (StringUtils.EqualsIgnoreCase(ContentAttribute.ImageUrl, attributeName))
                            {
                                if (!string.IsNullOrEmpty(value))
                                {
                                    var attachmentUrl = PageUtils.GetUrlByBaseUrl(value, url);

                                    var fileExtension = PageUtils.GetExtensionFromUrl(attachmentUrl);
                                    var fileName =
                                        $"{StringUtils.GetShortGuid(false)}{fileExtension}";

                                    var filePath = Context.SiteApi.GetUploadFilePath(siteInfo.Id, fileName);
                                    Utils.CreateDirectoryIfNotExists(filePath);
                                    try
                                    {
                                        WebClientUtils.SaveRemoteFileToLocal(attachmentUrl, filePath);
                                        contentInfo.ImageUrl = Context.SiteApi.GetSiteUrlByFilePath(filePath);
                                    }
                                    catch
                                    {
                                        // ignored
                                    }
                                }
                            }
                            else if (StringUtils.EqualsIgnoreCase(ContentAttribute.VideoUrl, attributeName))
                            {
                                if (!string.IsNullOrEmpty(value))
                                {
                                    var attachmentUrl = PageUtils.GetUrlByBaseUrl(value, url);
                                    var fileExtension = PageUtils.GetExtensionFromUrl(attachmentUrl);
                                    var fileName = $"{StringUtils.GetShortGuid(false)}{fileExtension}";
                                    var filePath = Context.SiteApi.GetUploadFilePath(siteInfo.Id, fileName);
                                    Utils.CreateDirectoryIfNotExists(filePath);
                                    try
                                    {
                                        WebClientUtils.SaveRemoteFileToLocal(attachmentUrl, filePath);
                                        contentInfo.VideoUrl = Context.SiteApi.GetSiteUrlByFilePath(filePath);
                                    }
                                    catch
                                    {
                                        // ignored
                                    }
                                }
                            }
                            else if (StringUtils.EqualsIgnoreCase(ContentAttribute.FileUrl, attributeName))
                            {
                                if (!string.IsNullOrEmpty(value))
                                {
                                    var attachmentUrl = PageUtils.GetUrlByBaseUrl(value, url);
                                    var fileExtension = PageUtils.GetExtensionFromUrl(attachmentUrl);
                                    var fileName = $"{StringUtils.GetShortGuid(false)}{fileExtension}";
                                    var filePath = Context.SiteApi.GetUploadFilePath(siteInfo.Id, fileName);
                                    Utils.CreateDirectoryIfNotExists(filePath);
                                    try
                                    {
                                        WebClientUtils.SaveRemoteFileToLocal(attachmentUrl, filePath);
                                        contentInfo.FileUrl = Context.SiteApi.GetSiteUrlByFilePath(filePath);
                                    }
                                    catch
                                    {
                                        // ignored
                                    }
                                }
                            }
                            else if (StringUtils.EqualsIgnoreCase(ContentAttribute.Hits, attributeName))
                            {
                                contentInfo.Hits = TranslateUtils.ToInt(value);
                            }
                            else
                            {
                                contentInfo.Set(attributeName, value);
                            }
                        }
                        else
                        {
                            //var styleInfo = TableStyleManager.GetTableStyleInfo(tableName, attributeName, relatedIdentities: null);
                            //value = InputParserUtility.GetContentByTableStyle(value, siteInfo, styleInfo);

                            //if (styleInfo.InputType == InputType.Image || styleInfo.InputType == InputType.Video || styleInfo.InputType == InputType.File)
                            //{
                            //    if (!string.IsNullOrEmpty(value))
                            //    {
                            //        var attachmentUrl = PageUtils.GetUrlByBaseUrl(value, url);
                            //        var fileExtension = PathUtils.GetExtension(attachmentUrl);
                            //        var fileName = $"{StringUtils.GetShortGuid(false)}{fileExtension}";
                            //        var filePath = Context.SiteApi.GetUploadFilePath(siteInfo.Id, fileName);
                            //        Utils.CreateDirectoryIfNotExists(filePath);
                            //        try
                            //        {
                            //            WebClientUtils.SaveRemoteFileToLocal(attachmentUrl, filePath);
                            //            value = Context.SiteApi.GetSiteUrlByFilePath(filePath);
                            //        }
                            //        catch
                            //        {
                            //            // ignored
                            //        }
                            //    }
                            //}

                            contentInfo.Set(attributeName, value);
                        }
                    }
                }

                if (string.IsNullOrEmpty(contentInfo.ImageUrl))
                {
                    var firstImageUrl = string.Empty;
                    if (isSaveImage)
                    {
                        var originalImageSrcList = GetOriginalImageSrcList(content);
                        var imageSrcList = GetImageSrcList(url, content);
                        if (originalImageSrcList.Count == imageSrcList.Count)
                        {
                            for (var i = 0; i < originalImageSrcList.Count; i++)
                            {
                                var originalImageSrc = originalImageSrcList[i];
                                var imageSrc = imageSrcList[i];

                                var fileExtension = PathUtils.GetExtension(originalImageSrc);
                                var fileName = $"{StringUtils.GetShortGuid(false)}{fileExtension}";
                                var filePath = Context.SiteApi.GetUploadFilePath(siteInfo.Id, fileName);
                                Utils.CreateDirectoryIfNotExists(filePath);
                                try
                                {
                                    WebClientUtils.SaveRemoteFileToLocal(imageSrc, filePath);
                                    var fileUrl = Context.SiteApi.GetSiteUrlByFilePath(filePath);
                                    content = content.Replace(originalImageSrc, fileUrl);
                                    if (firstImageUrl == string.Empty)
                                    {
                                        firstImageUrl = fileUrl;
                                    }
                                }
                                catch
                                {
                                    // ignored
                                }
                            }
                        }
                    }
                    else if (isSetFirstImageAsImageUrl)
                    {
                        var imageSrcList = GetImageSrcList(url, content);
                        if (imageSrcList.Count > 0)
                        {
                            firstImageUrl = imageSrcList[index: 0];
                        }
                    }

                    if (isSetFirstImageAsImageUrl)
                    {
                        contentInfo.ImageUrl = firstImageUrl;
                    }
                }
                //contentInfo.Content = StringUtility.TextEditorContentEncode(content, siteInfo, false);
                contentInfo.Content = content;

                //contentInfo.SourceId = SourceManager.CaiJi;

                var theContentId = Context.ContentApi.Insert(siteInfo.Id, channelInfo.Id, contentInfo);
                channelIdAndContentIdList.Add(new KeyValuePair<int, int>(contentInfo.ChannelId, theContentId));

                errorMessage = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }


        private static string GetPageContent(string previousPageContent, string charset, string url, string cookieString, string regexContentExclude, string contentHtmlClearCollection, string contentHtmlClearTagCollection, string regexContent, string regexContent2, string regexContent3, string regexNextPage)
        {
            var content = previousPageContent;
            var contentHtml = WebClientUtils.GetRemoteFileSource(url, ECharsetUtils.GetEnumType(charset), cookieString);
            var nextPageContent = GetContent("content", regexContent, contentHtml);
            if (string.IsNullOrEmpty(nextPageContent) && !string.IsNullOrEmpty(regexContent2))
            {
                nextPageContent = GetContent("content", regexContent2, contentHtml);
            }
            if (string.IsNullOrEmpty(nextPageContent) && !string.IsNullOrEmpty(regexContent3))
            {
                nextPageContent = GetContent("content", regexContent3, contentHtml);
            }

            if (!string.IsNullOrEmpty(nextPageContent))
            {
                if (string.IsNullOrEmpty(content))
                {
                    content += nextPageContent;
                }
                else
                {
                    content += Utils.PagePlaceHolder + nextPageContent;
                }
            }

            if (!string.IsNullOrEmpty(regexContentExclude))
            {
                content = Replace(regexContentExclude, content, string.Empty);
            }
            if (!string.IsNullOrEmpty(contentHtmlClearCollection))
            {
                var htmlClearList = StringCollectionToList(contentHtmlClearCollection);
                foreach (var htmlClear in htmlClearList)
                {
                    var clearRegex = $@"<{htmlClear}[^>]*>.*?<\/{htmlClear}>";
                    content = Replace(clearRegex, content, string.Empty);
                }
            }
            if (!string.IsNullOrEmpty(contentHtmlClearTagCollection))
            {
                var htmlClearTagList = StringCollectionToList(contentHtmlClearTagCollection);
                foreach (var htmlClearTag in htmlClearTagList)
                {
                    var clearRegex = $@"<{htmlClearTag}[^>]*>";
                    content = Replace(clearRegex, content, string.Empty);
                    clearRegex = $@"<\/{htmlClearTag}>";
                    content = Replace(clearRegex, content, string.Empty);
                }
            }

            var contentNextPageUrl = GetUrl(regexNextPage, contentHtml, url);
            if (!string.IsNullOrEmpty(contentNextPageUrl))
            {
                if (StringUtils.EqualsIgnoreCase(url, contentNextPageUrl))
                {
                    contentNextPageUrl = string.Empty;
                }
            }
            return !string.IsNullOrEmpty(contentNextPageUrl) ? GetPageContent(content, charset, contentNextPageUrl, cookieString, regexContentExclude, contentHtmlClearCollection, contentHtmlClearTagCollection, regexContent, regexContent2, regexContent3, regexNextPage) : content;
        }

        public static void Gather(IAdministratorInfo adminInfo, int siteId, int ruleId, string guid)
        {
            var cache = ProgressCache.Init(guid, "开始获取链接...");

            var gatherRuleInfo = Main.GatherRuleRepository.GetGatherRuleInfo(ruleId);
            var siteInfo = Context.SiteApi.GetSiteInfo(siteId);
            var channelInfo = Context.ChannelApi.GetChannelInfo(siteId, gatherRuleInfo.ChannelId);
            if (channelInfo == null)
            {
                channelInfo = Context.ChannelApi.GetChannelInfo(siteId, siteId);
                gatherRuleInfo.ChannelId = siteId;
            }

            var regexUrlInclude = GetRegexString(gatherRuleInfo.UrlInclude);
            var regexTitleInclude = GetRegexString(gatherRuleInfo.TitleInclude);
            var regexContentExclude = GetRegexString(gatherRuleInfo.ContentExclude);
            var regexListArea = GetRegexArea(gatherRuleInfo.ListAreaStart, gatherRuleInfo.ListAreaEnd);
            var regexChannel = GetRegexChannel(gatherRuleInfo.ContentChannelStart, gatherRuleInfo.ContentChannelEnd);
            var regexContent = GetRegexContent(gatherRuleInfo.ContentContentStart, gatherRuleInfo.ContentContentEnd);
            var regexContent2 = string.Empty;
            if (!string.IsNullOrEmpty(gatherRuleInfo.ContentContentStart2) && !string.IsNullOrEmpty(gatherRuleInfo.ContentContentEnd2))
            {
                regexContent2 = GetRegexContent(gatherRuleInfo.ContentContentStart2, gatherRuleInfo.ContentContentEnd2);
            }
            var regexContent3 = string.Empty;
            if (!string.IsNullOrEmpty(gatherRuleInfo.ContentContentStart3) && !string.IsNullOrEmpty(gatherRuleInfo.ContentContentEnd3))
            {
                regexContent3 = GetRegexContent(gatherRuleInfo.ContentContentStart3, gatherRuleInfo.ContentContentEnd3);
            }
            var regexNextPage = GetRegexUrl(gatherRuleInfo.ContentNextPageStart, gatherRuleInfo.ContentNextPageEnd);
            var regexTitle = GetRegexTitle(gatherRuleInfo.ContentTitleStart, gatherRuleInfo.ContentTitleEnd);
            var contentAttributes = TranslateUtils.StringCollectionToStringList(gatherRuleInfo.ContentAttributes);
            var contentAttributesXml = TranslateUtils.ToNameValueCollection(gatherRuleInfo.ContentAttributesXml);

            var contentUrls = GetContentUrlList(gatherRuleInfo, regexListArea, regexUrlInclude, cache);

            cache.TotalCount = gatherRuleInfo.GatherNum > 0 ? gatherRuleInfo.GatherNum : contentUrls.Count;
            cache.IsSuccess = true;
            cache.Message = "开始采集内容...";

            var contentTitleDict = new Dictionary<int, IList<string>>();
            var channelIdAndContentIdList = new List<KeyValuePair<int, int>>();

            foreach (var contentUrl in contentUrls)
            {
                if (GatherOneByUrl(siteInfo, channelInfo, gatherRuleInfo.IsSaveImage, gatherRuleInfo.IsSetFirstImageAsImageUrl, gatherRuleInfo.IsEmptyContentAllowed, gatherRuleInfo.IsSameTitleAllowed, gatherRuleInfo.IsChecked, gatherRuleInfo.Charset, contentUrl, gatherRuleInfo.CookieString, regexTitleInclude, regexContentExclude, gatherRuleInfo.ContentHtmlClearCollection, gatherRuleInfo.ContentHtmlClearTagCollection, gatherRuleInfo.ContentReplaceFrom, gatherRuleInfo.ContentReplaceTo, regexTitle, regexContent, regexContent2, regexContent3, regexNextPage, regexChannel, contentAttributes, contentAttributesXml, contentTitleDict, channelIdAndContentIdList, adminInfo, out var title, out var errorMessage))
                {
                    cache.SuccessCount++;
                    cache.IsSuccess = true;
                    cache.Message = $"成功采集内容：{title}";
                }
                else
                {
                    cache.FailureCount++;
                    cache.IsSuccess = false;
                    cache.Message = errorMessage;
                    cache.FailureMessages.Add(errorMessage);
                }
                if (cache.SuccessCount == cache.TotalCount) break;
            }

            //if (gatherRuleInfo.IsChecked)
            //{
            //    foreach (var channelIdAndContentId in channelIdAndContentIdList)
            //    {
            //        var channelId = channelIdAndContentId.Key;
            //        var contentId = channelIdAndContentId.Value;

            //        CreateManager.CreateContent(siteId, channelId, contentId);
            //    }
            //}

            Main.GatherRuleRepository.UpdateLastGatherDate(ruleId);

            cache.Status = ProgressCache.StatusSuccess;
            cache.IsSuccess = true;
            cache.Message = $"任务完成，共采集内容 {cache.SuccessCount} 篇。";
        }
    }
}
