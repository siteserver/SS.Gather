using System;
using System.Collections.Generic;
using Datory;
using SiteServer.Plugin;

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
            public const string CookieString = nameof(GatherRuleInfo.CookieString);
            public const string GatherUrlIsCollection = nameof(GatherRuleInfo.GatherUrlIsCollection);
            public const string GatherUrlCollection = nameof(GatherRuleInfo.GatherUrlCollection);
            public const string GatherUrlIsSerialize = nameof(GatherRuleInfo.GatherUrlIsSerialize);
            public const string GatherUrlSerialize = nameof(GatherRuleInfo.GatherUrlSerialize);
            public const string SerializeFrom = nameof(GatherRuleInfo.SerializeFrom);
            public const string SerializeTo = nameof(GatherRuleInfo.SerializeTo);
            public const string SerializeInterval = nameof(GatherRuleInfo.SerializeInterval);
            public const string SerializeIsOrderByDesc = nameof(GatherRuleInfo.SerializeIsOrderByDesc);
            public const string SerializeIsAddZero = nameof(GatherRuleInfo.SerializeIsAddZero);
            public const string ChannelId = nameof(GatherRuleInfo.ChannelId);
            public const string Charset = nameof(GatherRuleInfo.Charset);
            public const string UrlInclude = nameof(GatherRuleInfo.UrlInclude);
            public const string TitleInclude = nameof(GatherRuleInfo.TitleInclude);
            public const string ContentExclude = nameof(GatherRuleInfo.ContentExclude);
            public const string ContentHtmlClearCollection = nameof(GatherRuleInfo.ContentHtmlClearCollection);
            public const string ContentHtmlClearTagCollection = nameof(GatherRuleInfo.ContentHtmlClearTagCollection);
            public const string LastGatherDate = nameof(GatherRuleInfo.LastGatherDate);
            public const string ListAreaStart = nameof(GatherRuleInfo.ListAreaStart);
            public const string ListAreaEnd = nameof(GatherRuleInfo.ListAreaEnd);
            public const string ContentChannelStart = nameof(GatherRuleInfo.ContentChannelStart);
            public const string ContentChannelEnd = nameof(GatherRuleInfo.ContentChannelEnd);
            public const string ContentTitleStart = nameof(GatherRuleInfo.ContentTitleStart);
            public const string ContentTitleEnd = nameof(GatherRuleInfo.ContentTitleEnd);
            public const string ContentContentStart = nameof(GatherRuleInfo.ContentContentStart);
            public const string ContentContentEnd = nameof(GatherRuleInfo.ContentContentEnd);
            public const string ContentNextPageStart = nameof(GatherRuleInfo.ContentNextPageStart);
            public const string ContentNextPageEnd = nameof(GatherRuleInfo.ContentNextPageEnd);
            public const string ContentAttributes = nameof(GatherRuleInfo.ContentAttributes);
            public const string ContentAttributesXml = nameof(GatherRuleInfo.ContentAttributesXml);
            public const string ExtendValues = nameof(GatherRuleInfo.ExtendValues);
        }

        private const string SqlSelectGatherRule = "SELECT Id, GatherRuleName, SiteId, CookieString, GatherUrlIsCollection, GatherUrlCollection, GatherUrlIsSerialize, GatherUrlSerialize, SerializeFrom, SerializeTo, SerializeInterval, SerializeIsOrderByDesc, SerializeIsAddZero, ChannelId, Charset, UrlInclude, TitleInclude, ContentExclude, ContentHtmlClearCollection, ContentHtmlClearTagCollection, LastGatherDate, ListAreaStart, ListAreaEnd, ContentChannelStart, ContentChannelEnd, ContentTitleStart, ContentTitleEnd, ContentContentStart, ContentContentEnd, ContentNextPageStart, ContentNextPageEnd, ContentAttributes, ContentAttributesXml, ExtendValues FROM siteserver_GatherRule WHERE GatherRuleName = @GatherRuleName AND SiteId = @SiteId";

        private const string SqlSelectAllGatherRuleByPsId = "SELECT Id, GatherRuleName, SiteId, CookieString, GatherUrlIsCollection, GatherUrlCollection, GatherUrlIsSerialize, GatherUrlSerialize, SerializeFrom, SerializeTo, SerializeInterval, SerializeIsOrderByDesc, SerializeIsAddZero, ChannelId, Charset, UrlInclude, TitleInclude, ContentExclude, ContentHtmlClearCollection, ContentHtmlClearTagCollection, LastGatherDate, ListAreaStart, ListAreaEnd, ContentChannelStart, ContentChannelEnd, ContentTitleStart, ContentTitleEnd, ContentContentStart, ContentContentEnd, ContentNextPageStart, ContentNextPageEnd, ContentAttributes, ContentAttributesXml, ExtendValues FROM siteserver_GatherRule WHERE SiteId = @SiteId";

        private const string SqlSelectGatherRuleNameByPsId = "SELECT GatherRuleName FROM siteserver_GatherRule WHERE SiteId = @SiteId";

        private const string SqlInsertGatherRule = @"
INSERT INTO siteserver_GatherRule 
(GatherRuleName, SiteId, CookieString, GatherUrlIsCollection, GatherUrlCollection, GatherUrlIsSerialize, GatherUrlSerialize, SerializeFrom, SerializeTo, SerializeInterval, SerializeIsOrderByDesc, SerializeIsAddZero, ChannelId, Charset, UrlInclude, TitleInclude, ContentExclude, ContentHtmlClearCollection, ContentHtmlClearTagCollection, LastGatherDate, ListAreaStart, ListAreaEnd, ContentChannelStart, ContentChannelEnd, ContentTitleStart, ContentTitleEnd, ContentContentStart, ContentContentEnd, ContentNextPageStart, ContentNextPageEnd, ContentAttributes, ContentAttributesXml, ExtendValues) VALUES (@GatherRuleName, @SiteId, @CookieString, @GatherUrlIsCollection, @GatherUrlCollection, @GatherUrlIsSerialize, @GatherUrlSerialize, @SerializeFrom, @SerializeTo, @SerializeInterval, @SerializeIsOrderByDesc, @SerializeIsAddZero, @ChannelId, @Charset, @UrlInclude, @TitleInclude, @ContentExclude, @ContentHtmlClearCollection, @ContentHtmlClearTagCollection, @LastGatherDate, @ListAreaStart, @ListAreaEnd, @ContentChannelStart, @ContentChannelEnd, @ContentTitleStart, @ContentTitleEnd, @ContentContentStart, @ContentContentEnd, @ContentNextPageStart, @ContentNextPageEnd, @ContentAttributes, @ContentAttributesXml, @ExtendValues)";

        private const string SqlUpdateGatherRule = @"
UPDATE siteserver_GatherRule SET 
CookieString = @CookieString, GatherUrlIsCollection = @GatherUrlIsCollection, GatherUrlCollection = @GatherUrlCollection, GatherUrlIsSerialize = @GatherUrlIsSerialize, GatherUrlSerialize = @GatherUrlSerialize, SerializeFrom = @SerializeFrom, SerializeTo = @SerializeTo, SerializeInterval = @SerializeInterval, SerializeIsOrderByDesc = @SerializeIsOrderByDesc, SerializeIsAddZero = @SerializeIsAddZero, ChannelId = @ChannelId, Charset = @Charset, UrlInclude = @UrlInclude, TitleInclude = @TitleInclude, ContentExclude = @ContentExclude, ContentHtmlClearCollection = @ContentHtmlClearCollection, ContentHtmlClearTagCollection = @ContentHtmlClearTagCollection, LastGatherDate = @LastGatherDate, ListAreaStart = @ListAreaStart, ListAreaEnd = @ListAreaEnd, ContentChannelStart = @ContentChannelStart, ContentChannelEnd = @ContentChannelEnd, ContentTitleStart = @ContentTitleStart, ContentTitleEnd = @ContentTitleEnd, ContentContentStart = @ContentContentStart, ContentContentEnd = @ContentContentEnd, ContentNextPageStart = @ContentNextPageStart, ContentNextPageEnd = @ContentNextPageEnd, ContentAttributes = @ContentAttributes, ContentAttributesXml = @ContentAttributesXml, ExtendValues = @ExtendValues WHERE GatherRuleName = @GatherRuleName AND SiteId = @SiteId";

        private const string SqlUpdateLastGatherDate = "UPDATE siteserver_GatherRule SET LastGatherDate = @LastGatherDate WHERE GatherRuleName = @GatherRuleName AND SiteId = @SiteId";

        private const string SqlDeleteGatherRule = "DELETE FROM siteserver_GatherRule WHERE GatherRuleName = @GatherRuleName AND SiteId = @SiteId";

        private const string ParamGatherRuleName = "@GatherRuleName";
        private const string ParamSiteId = "@SiteId";

        private const string ParamCookieString = "@CookieString";
        private const string ParamGatherUrlIsCollection = "@GatherUrlIsCollection";
        private const string ParamGatherUrlCollection = "@GatherUrlCollection";
        private const string ParamGatherUrlIsSerialize = "@GatherUrlIsSerialize";
        private const string ParamGatherUrlSerialize = "@GatherUrlSerialize";
        private const string ParamGatherSerializeFrom = "@SerializeFrom";
        private const string ParamGatherSerializeTo = "@SerializeTo";
        private const string ParamGatherSerializeInternal = "@SerializeInterval";
        private const string ParamGatherSerializeOrderByDesc = "@SerializeIsOrderByDesc";
        private const string ParamGatherSerializeIsAddZero = "@SerializeIsAddZero";

        private const string ParamChannelId = "@ChannelId";
        private const string ParamCharset = "@Charset";
        private const string ParamUrlInclude = "@UrlInclude";
        private const string ParamTitleInclude = "@TitleInclude";
        private const string ParamContentExclude = "@ContentExclude";
        private const string ParamContentHtmlClearCollection = "@ContentHtmlClearCollection";
        private const string ParamContentHtmlClearTagCollection = "@ContentHtmlClearTagCollection";
        private const string ParamLastGatherDate = "@LastGatherDate";

        private const string ParamListAreaStart = "@ListAreaStart";
        private const string ParamListAreaEnd = "@ListAreaEnd";
        private const string ParamListContentChannelStart = "@ContentChannelStart";
        private const string ParamListContentChannelEnd = "@ContentChannelEnd";
        private const string ParamContentTitleStart = "@ContentTitleStart";
        private const string ParamContentTitleEnd = "@ContentTitleEnd";
        private const string ParamContentContentStart = "@ContentContentStart";
        private const string ParamContentContentEnd = "@ContentContentEnd";
        private const string ParamContentNextPageStart = "@ContentNextPageStart";
        private const string ParamContentNextPageEnd = "@ContentNextPageEnd";
        private const string ParamContentAttributes = "@ContentAttributes";
        private const string ParamContentAttributesXml = "@ContentAttributesXml";
        private const string ParamExtendValues = "@ExtendValues";

        public void Insert(GatherRuleInfo gatherRuleInfo)
        {
            _repository.Insert(gatherRuleInfo);
        }

        public void UpdateLastGatherDate(int ruleId)
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
    }
}
