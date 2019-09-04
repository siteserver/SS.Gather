using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.Plugin;
using SS.Gather.Cli;

namespace SS.Gather.Core
{
    public class GatherRuleRepository
    {
        private readonly Repository<GatherRuleInfo> _repository;

        public GatherRuleRepository()
        {
            _repository = new Repository<GatherRuleInfo>(Context.Environment.DatabaseType, Context.Environment.ConnectionString);
        }

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(GatherRuleInfo.Id);
            public const string GatherRuleName = nameof(GatherRuleInfo.GatherRuleName);
            public const string SiteId = nameof(GatherRuleInfo.SiteId);
            //public const string CookieString = nameof(GatherRuleInfo.CookieString);
            //public const string GatherUrlIsCollection = nameof(GatherRuleInfo.GatherUrlIsCollection);
            //public const string GatherUrlCollection = nameof(GatherRuleInfo.GatherUrlCollection);
            //public const string GatherUrlIsSerialize = nameof(GatherRuleInfo.GatherUrlIsSerialize);
            //public const string GatherUrlSerialize = nameof(GatherRuleInfo.GatherUrlSerialize);
            //public const string SerializeFrom = nameof(GatherRuleInfo.SerializeFrom);
            //public const string SerializeTo = nameof(GatherRuleInfo.SerializeTo);
            //public const string SerializeInterval = nameof(GatherRuleInfo.SerializeInterval);
            //public const string SerializeIsOrderByDesc = nameof(GatherRuleInfo.SerializeIsOrderByDesc);
            //public const string SerializeIsAddZero = nameof(GatherRuleInfo.SerializeIsAddZero);
            //public const string ChannelId = nameof(GatherRuleInfo.ChannelId);
            //public const string Charset = nameof(GatherRuleInfo.Charset);
            //public const string UrlInclude = nameof(GatherRuleInfo.UrlInclude);
            //public const string TitleInclude = nameof(GatherRuleInfo.TitleInclude);
            //public const string ContentExclude = nameof(GatherRuleInfo.ContentExclude);
            //public const string ContentHtmlClearCollection = nameof(GatherRuleInfo.ContentHtmlClearCollection);
            //public const string ContentHtmlClearTagCollection = nameof(GatherRuleInfo.ContentHtmlClearTagCollection);
            public const string LastGatherDate = nameof(GatherRuleInfo.LastGatherDate);
            //public const string ListAreaStart = nameof(GatherRuleInfo.ListAreaStart);
            //public const string ListAreaEnd = nameof(GatherRuleInfo.ListAreaEnd);
            //public const string ContentChannelStart = nameof(GatherRuleInfo.ContentChannelStart);
            //public const string ContentChannelEnd = nameof(GatherRuleInfo.ContentChannelEnd);
            //public const string ContentTitleStart = nameof(GatherRuleInfo.ContentTitleStart);
            //public const string ContentTitleEnd = nameof(GatherRuleInfo.ContentTitleEnd);
            //public const string ContentContentStart = nameof(GatherRuleInfo.ContentContentStart);
            //public const string ContentContentEnd = nameof(GatherRuleInfo.ContentContentEnd);
            //public const string ContentNextPageStart = nameof(GatherRuleInfo.ContentNextPageStart);
            //public const string ContentNextPageEnd = nameof(GatherRuleInfo.ContentNextPageEnd);
            //public const string ContentAttributes = nameof(GatherRuleInfo.ContentAttributes);
            //public const string ContentAttributesXml = nameof(GatherRuleInfo.ContentAttributesXml);
            //public const string ExtendValues = nameof(GatherRuleInfo.ExtendValues);
        }

        public void Insert(GatherRuleInfo gatherRuleInfo)
        {
            _repository.Insert(gatherRuleInfo);
        }

        private void UpdateLastGatherDate(int ruleId)
        {
            _repository.Update(Q
                .Set(Attr.LastGatherDate, DateTime.Now)
                .Where(Attr.Id, ruleId)
            );
        }

        public void Update(GatherRuleInfo gatherRuleInfo)
        {
            _repository.Update(gatherRuleInfo);
        }

        public void Delete(int ruleId)
        {
            _repository.Delete(Q
                .Where(Attr.Id, ruleId)
            );
        }

        public GatherRuleInfo GetGatherRuleInfo(int ruleId)
        {
            return _repository.Get(Q
                .Where(Attr.Id, ruleId)
            );
        }

        public IEnumerable<(int Id, int SiteId)> GetGatherRuleIdList(List<int> includes, List<int> excludes)
        {
            var query = Q.Select(Attr.Id, Attr.SiteId).OrderBy(Attr.Id);
            if (includes != null && includes.Count > 0)
            {
                query.WhereIn(Attr.Id, includes);
            }
            else if (excludes != null && excludes.Count > 0)
            {
                query.WhereNotIn(Attr.Id, excludes);
            }

            return _repository.GetAll<(int Id, int SiteId)>(query);
        }

        public bool IsExists(int siteId, string ruleName)
        {
            return _repository.Exists(Q
                .Where(Attr.GatherRuleName, ruleName)
                .Where(Attr.SiteId, siteId)
            );
        }

        public string GetImportGatherRuleName(int siteId, string gatherRuleName)
        {
            string importGatherRuleName;
            if (gatherRuleName != null && gatherRuleName.IndexOf("_", StringComparison.Ordinal) != -1)
            {
                var gatherRuleNameCount = 0;
                var lastGatherRuleName = gatherRuleName.Substring(gatherRuleName.LastIndexOf("_", StringComparison.Ordinal) + 1);
                var firstGatherRuleName = gatherRuleName.Substring(0, gatherRuleName.Length - lastGatherRuleName.Length);

                try
                {
                    gatherRuleNameCount = int.Parse(lastGatherRuleName);
                }
                catch
                {
                    // ignored
                }

                gatherRuleNameCount++;
                importGatherRuleName = firstGatherRuleName + gatherRuleNameCount;
            }
            else
            {
                importGatherRuleName = gatherRuleName + "_1";
            }

            if (_repository.Exists(Q
                .Where(Attr.GatherRuleName, gatherRuleName)
                .Where(Attr.SiteId, siteId)
            ))
            {
                importGatherRuleName = GetImportGatherRuleName(siteId, importGatherRuleName);
            }

            return importGatherRuleName;
        }

        public IList<GatherRuleInfo> GetGatherRuleInfoList(int siteId)
        {
            return _repository.GetAll(Q
                .Where(Attr.SiteId, siteId)
                .OrderBy(Attr.Id)
            );
        }

        public IList<string> GetGatherRuleNameList(int siteId)
        {
            return _repository.GetAll<string>(Q
                .Select(Attr.GatherRuleName)
                .Where(Attr.SiteId, siteId)
            );
        }

        public async Task GatherChannelsAsync(IAdministratorInfo adminInfo, int siteId, int ruleId, string guid, bool isCli)
        {
            var cache = ProgressCache.Init(guid, "开始获取链接...");
            if (isCli) await CliUtils.PrintLine(cache.Message);

            var gatherRuleInfo = Main.GatherRuleRepository.GetGatherRuleInfo(ruleId);
            var siteInfo = Context.SiteApi.GetSiteInfo(siteId);
            var channelInfo = Context.ChannelApi.GetChannelInfo(siteId, gatherRuleInfo.ChannelId);
            if (channelInfo == null)
            {
                channelInfo = Context.ChannelApi.GetChannelInfo(siteId, siteId);
                gatherRuleInfo.ChannelId = siteId;
            }

            var regexUrlInclude = GatherUtility.GetRegexString(gatherRuleInfo.UrlInclude);
            var regexTitleInclude = GatherUtility.GetRegexString(gatherRuleInfo.TitleInclude);
            var regexContentExclude = GatherUtility.GetRegexString(gatherRuleInfo.ContentExclude);
            var regexListArea = GatherUtility.GetRegexArea(gatherRuleInfo.ListAreaStart, gatherRuleInfo.ListAreaEnd);
            var regexChannel = GatherUtility.GetRegexChannel(gatherRuleInfo.ContentChannelStart, gatherRuleInfo.ContentChannelEnd);
            var regexContent = GatherUtility.GetRegexContent(gatherRuleInfo.ContentContentStart, gatherRuleInfo.ContentContentEnd);
            var regexContent2 = string.Empty;
            if (!string.IsNullOrEmpty(gatherRuleInfo.ContentContentStart2) && !string.IsNullOrEmpty(gatherRuleInfo.ContentContentEnd2))
            {
                regexContent2 = GatherUtility.GetRegexContent(gatherRuleInfo.ContentContentStart2, gatherRuleInfo.ContentContentEnd2);
            }
            var regexContent3 = string.Empty;
            if (!string.IsNullOrEmpty(gatherRuleInfo.ContentContentStart3) && !string.IsNullOrEmpty(gatherRuleInfo.ContentContentEnd3))
            {
                regexContent3 = GatherUtility.GetRegexContent(gatherRuleInfo.ContentContentStart3, gatherRuleInfo.ContentContentEnd3);
            }
            var regexNextPage = GatherUtility.GetRegexUrl(gatherRuleInfo.ContentNextPageStart, gatherRuleInfo.ContentNextPageEnd);
            var regexTitle = GatherUtility.GetRegexTitle(gatherRuleInfo.ContentTitleStart, gatherRuleInfo.ContentTitleEnd);
            var contentAttributes = TranslateUtils.StringCollectionToStringList(gatherRuleInfo.ContentAttributes);
            var attributesDict = TranslateUtils.JsonDeserialize<Dictionary<string, string>>(gatherRuleInfo.ContentAttributesXml);

            var contentUrls = GatherUtility.GetContentUrlList(gatherRuleInfo, regexListArea, regexUrlInclude, cache);

            cache.TotalCount = gatherRuleInfo.GatherNum > 0 ? gatherRuleInfo.GatherNum : contentUrls.Count;
            cache.IsSuccess = true;
            cache.Message = "开始采集内容...";
            if (isCli) await CliUtils.PrintLine(cache.Message);

            var channelIdAndContentIdList = new List<KeyValuePair<int, int>>();

            foreach (var contentUrl in contentUrls)
            {
                if (GatherOne(siteInfo, channelInfo, gatherRuleInfo.IsSaveImage, gatherRuleInfo.IsSetFirstImageAsImageUrl, gatherRuleInfo.IsSaveFiles, gatherRuleInfo.IsEmptyContentAllowed, gatherRuleInfo.IsSameTitleAllowed, gatherRuleInfo.IsChecked, gatherRuleInfo.Charset, contentUrl, gatherRuleInfo.CookieString, regexTitleInclude, regexContentExclude, gatherRuleInfo.ContentHtmlClearCollection, gatherRuleInfo.ContentHtmlClearTagCollection, gatherRuleInfo.ContentReplaceFrom, gatherRuleInfo.ContentReplaceTo, regexTitle, regexContent, regexContent2, regexContent3, regexNextPage, regexChannel, contentAttributes, attributesDict, channelIdAndContentIdList, adminInfo, out var title, out var errorMessage))
                {
                    cache.SuccessCount++;
                    cache.IsSuccess = true;
                    cache.Message = $"采集成功：{title}";
                    if (isCli) await CliUtils.PrintLine(cache.Message);
                }
                else
                {
                    cache.FailureCount++;
                    cache.IsSuccess = false;
                    cache.Message = errorMessage;
                    if (isCli) await CliUtils.PrintErrorAsync($"采集失败：{errorMessage}");
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
            if (isCli) await CliUtils.PrintLine(cache.Message);
        }

        public void GatherContents(IAdministratorInfo adminInfo, int siteId, int ruleId, int channelId, string guid, List<string> urls)
        {
            var cache = ProgressCache.Init(guid, "开始获取链接...");

            var gatherRuleInfo = Main.GatherRuleRepository.GetGatherRuleInfo(ruleId);
            var siteInfo = Context.SiteApi.GetSiteInfo(siteId);
            var channelInfo = Context.ChannelApi.GetChannelInfo(siteId, channelId) ?? Context.ChannelApi.GetChannelInfo(siteId, siteId);

            var regexTitleInclude = GatherUtility.GetRegexString(gatherRuleInfo.TitleInclude);
            var regexContentExclude = GatherUtility.GetRegexString(gatherRuleInfo.ContentExclude);
            var regexChannel = GatherUtility.GetRegexChannel(gatherRuleInfo.ContentChannelStart, gatherRuleInfo.ContentChannelEnd);
            var regexContent = GatherUtility.GetRegexContent(gatherRuleInfo.ContentContentStart, gatherRuleInfo.ContentContentEnd);
            var regexContent2 = string.Empty;
            if (!string.IsNullOrEmpty(gatherRuleInfo.ContentContentStart2) && !string.IsNullOrEmpty(gatherRuleInfo.ContentContentEnd2))
            {
                regexContent2 = GatherUtility.GetRegexContent(gatherRuleInfo.ContentContentStart2, gatherRuleInfo.ContentContentEnd2);
            }
            var regexContent3 = string.Empty;
            if (!string.IsNullOrEmpty(gatherRuleInfo.ContentContentStart3) && !string.IsNullOrEmpty(gatherRuleInfo.ContentContentEnd3))
            {
                regexContent3 = GatherUtility.GetRegexContent(gatherRuleInfo.ContentContentStart3, gatherRuleInfo.ContentContentEnd3);
            }
            var regexNextPage = GatherUtility.GetRegexUrl(gatherRuleInfo.ContentNextPageStart, gatherRuleInfo.ContentNextPageEnd);
            var regexTitle = GatherUtility.GetRegexTitle(gatherRuleInfo.ContentTitleStart, gatherRuleInfo.ContentTitleEnd);
            var contentAttributes = TranslateUtils.StringCollectionToStringList(gatherRuleInfo.ContentAttributes);
            var attributesDict = TranslateUtils.JsonDeserialize<Dictionary<string, string>>(gatherRuleInfo.ContentAttributesXml);

            cache.TotalCount = gatherRuleInfo.GatherNum > 0 ? gatherRuleInfo.GatherNum : urls.Count;
            cache.IsSuccess = true;
            cache.Message = "开始采集内容...";

            var channelIdAndContentIdList = new List<KeyValuePair<int, int>>();

            foreach (var contentUrl in urls)
            {
                if (GatherOne(siteInfo, channelInfo, gatherRuleInfo.IsSaveImage, gatherRuleInfo.IsSetFirstImageAsImageUrl, gatherRuleInfo.IsSaveFiles, gatherRuleInfo.IsEmptyContentAllowed, gatherRuleInfo.IsSameTitleAllowed, gatherRuleInfo.IsChecked, gatherRuleInfo.Charset, contentUrl, gatherRuleInfo.CookieString, regexTitleInclude, regexContentExclude, gatherRuleInfo.ContentHtmlClearCollection, gatherRuleInfo.ContentHtmlClearTagCollection, gatherRuleInfo.ContentReplaceFrom, gatherRuleInfo.ContentReplaceTo, regexTitle, regexContent, regexContent2, regexContent3, regexNextPage, regexChannel, contentAttributes, attributesDict, channelIdAndContentIdList, adminInfo, out var title, out var errorMessage))
                {
                    cache.SuccessCount++;
                    cache.IsSuccess = true;
                    cache.Message = $"采集成功：{title}";
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

        private bool GatherOne(ISiteInfo siteInfo, IChannelInfo channelInfo, bool isSaveImage, bool isSetFirstImageAsImageUrl, bool isSaveFiles, bool isEmptyContentAllowed, bool isSameTitleAllowed, bool isChecked, string charset, string url, string cookieString, string regexTitleInclude, string regexContentExclude, string contentHtmlClearCollection, string contentHtmlClearTagCollection, string contentReplaceFrom, string contentReplaceTo, string regexTitle, string regexContent, string regexContent2, string regexContent3, string regexNextPage, string regexChannel, IEnumerable<string> contentAttributes, Dictionary<string, string> attributesDict, ICollection<KeyValuePair<int, int>> channelIdAndContentIdList, IAdministratorInfo adminInfo, out string title, out string errorMessage)
        {
            title = string.Empty;

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
                if (!WebClientUtils.GetRemoteHtml(url, ECharsetUtils.GetEnumType(charset), cookieString, out var contentHtml, out errorMessage))
                {
                    return false;
                }
                title = GatherUtility.GetContent("title", regexTitle, contentHtml);
                var content = GatherUtility.GetContent("content", regexContent, contentHtml);
                if (string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(regexContent2))
                {
                    content = GatherUtility.GetContent("content", regexContent2, contentHtml);
                }
                if (string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(regexContent3))
                {
                    content = GatherUtility.GetContent("content", regexContent3, contentHtml);
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
                    if (GatherUtility.IsMatch(regexTitleInclude, title) == false)
                    {
                        errorMessage = $"标题不符合要求：{url}";
                        return false;
                    }
                }
                if (!string.IsNullOrEmpty(regexContentExclude))
                {
                    content = GatherUtility.Replace(regexContentExclude, content, string.Empty);
                }
                if (!string.IsNullOrEmpty(contentHtmlClearCollection))
                {
                    var htmlClearList = GatherUtility.StringCollectionToList(contentHtmlClearCollection);
                    foreach (var htmlClear in htmlClearList)
                    {
                        var clearRegex = $@"<{htmlClear}[^>]*>.*?<\/{htmlClear}>";
                        content = GatherUtility.Replace(clearRegex, content, string.Empty);
                    }
                }
                if (!string.IsNullOrEmpty(contentHtmlClearTagCollection))
                {
                    var htmlClearTagList = GatherUtility.StringCollectionToList(contentHtmlClearTagCollection);
                    foreach (var htmlClearTag in htmlClearTagList)
                    {
                        var clearRegex = $@"<{htmlClearTag}[^>]*>";
                        content = GatherUtility.Replace(clearRegex, content, string.Empty);
                        clearRegex = $@"<\/{htmlClearTag}>";
                        content = GatherUtility.Replace(clearRegex, content, string.Empty);
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
                            title = GatherUtility.Replace($"({from.Replace(" ", "\\s")})(?!</a>)(?![^><]*>)", title, contentReplaceTo);
                            content = GatherUtility.Replace($"({from.Replace(" ", "\\s")})(?!</a>)(?![^><]*>)", content, contentReplaceTo);
                        }
                    }
                    else
                    {
                        var tos = TranslateUtils.StringCollectionToStringCollection(contentReplaceTo);
                        for (var i = 0; i < fromList.Count; i++)
                        {
                            title = GatherUtility.Replace($"({fromList[i].Replace(" ", "\\s")})(?!</a>)(?![^><]*>)", title, tos[i]);
                            content = GatherUtility.Replace($"({fromList[i].Replace(" ", "\\s")})(?!</a>)(?![^><]*>)", content, tos[i]);
                        }
                    }
                }

                var contentNextPageUrl = GatherUtility.GetUrl(regexNextPage, contentHtml, url);
                if (!string.IsNullOrEmpty(contentNextPageUrl))
                {
                    try
                    {
                        content = GatherUtility.GetPageContent(content, charset, contentNextPageUrl, cookieString, regexContentExclude, contentHtmlClearCollection, contentHtmlClearTagCollection, regexContent, regexContent2, regexContent3, regexNextPage);
                    }
                    catch (Exception ex)
                    {
                        errorMessage = ex.Message;
                        return false;
                    }
                }

                var channel = GatherUtility.GetContent("channel", regexChannel, contentHtml);
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
                    var repository = new Repository(Context.Environment.DatabaseType,
                            Context.Environment.ConnectionString, tableName);

                    if (repository.Exists(Q.Where(ContentAttribute.ChannelId, channelId).Where(ContentAttribute.Title, title)))
                    {
                        errorMessage = $"已包含相同标题：{title}";
                        return false;
                    }
                }

                var contentInfo = Context.ContentApi.NewInstance(siteInfo.Id, channelId);

                if (adminInfo != null)
                {
                    contentInfo.AdminId = adminInfo.Id;
                    contentInfo.AddUserName = adminInfo.UserName;
                }

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
                        var normalStart = GatherUtility.GetStartValue(attributesDict, attributeName);
                        var normalEnd = GatherUtility.GetEndValue(attributesDict, attributeName);

                        //采集为空时的默认值
                        var normalDefault = GatherUtility.GetDefaultValue(attributesDict, attributeName);

                        var regex = GatherUtility.GetRegexAttributeName(attributeName, normalStart, normalEnd);
                        var value = GatherUtility.GetContent(attributeName, regex, contentHtml);

                        //采集为空时的默认值
                        if (string.IsNullOrEmpty(value))
                        {
                            value = normalDefault;
                        }

                        if (ContentAttribute.AllAttributes.Value.Contains(attributeName))
                        {
                            if (StringUtils.EqualsIgnoreCase(ContentAttribute.AddDate, attributeName))
                            {
                                value = StringUtils.ReplaceFirst("：", value, ":");
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

                var firstImageUrl = string.Empty;
                if (isSaveImage)
                {
                    var originalImageSrcList = GatherUtility.GetOriginalImageSrcList(content);
                    var imageSrcList = GatherUtility.GetImageSrcList(url, content);
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
                    var imageSrcList = GatherUtility.GetImageSrcList(url, content);
                    if (imageSrcList.Count > 0)
                    {
                        firstImageUrl = imageSrcList[index: 0];
                    }
                }

                if (isSetFirstImageAsImageUrl && string.IsNullOrEmpty(contentInfo.ImageUrl))
                {
                    contentInfo.ImageUrl = firstImageUrl;
                }

                if (isSaveFiles)
                {
                    var originalLinkHrefList = GatherUtility.GetOriginalLinkHrefList(content);
                    var linkHrefList = GatherUtility.GetLinkHrefList(url, content);
                    if (originalLinkHrefList.Count == linkHrefList.Count)
                    {
                        for (var i = 0; i < originalLinkHrefList.Count; i++)
                        {
                            var originalLinkHref = originalLinkHrefList[i];
                            var linkHref = linkHrefList[i];

                            var fileExtension = PathUtils.GetExtension(originalLinkHref);
                            var fileName = $"{StringUtils.GetShortGuid(false)}{fileExtension}";
                            var filePath = Context.SiteApi.GetUploadFilePath(siteInfo.Id, fileName);
                            Utils.CreateDirectoryIfNotExists(filePath);
                            try
                            {
                                WebClientUtils.SaveRemoteFileToLocal(linkHref, filePath);
                                var fileUrl = Context.SiteApi.GetSiteUrlByFilePath(filePath);
                                content = content.Replace(originalLinkHref, fileUrl);
                            }
                            catch
                            {
                                // ignored
                            }
                        }
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
    }
}
