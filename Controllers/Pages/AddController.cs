using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Gather.Core;

namespace SS.Gather.Controllers.Pages
{
    [RoutePrefix("pages/add")]
    public class AddController : ApiController
    {
        private const string Route = "";
        private const string RouteAttributes = "attributes";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var siteId = request.GetQueryInt("siteId");
                var ruleId = request.GetQueryInt("ruleId");

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, Utils.PluginId))
                {
                    return Unauthorized();
                }

                GatherRuleInfo ruleInfo;
                List<string> contentHtmlClearList;
                List<string> contentHtmlClearTagList;
                Dictionary<string, string> attributesDict;
                if (ruleId > 0)
                {
                    ruleInfo = Main.GatherRuleRepository.GetGatherRuleInfo(ruleId);
                    contentHtmlClearList =
                        TranslateUtils.StringCollectionToStringList(ruleInfo.ContentHtmlClearCollection);
                    contentHtmlClearTagList = TranslateUtils.StringCollectionToStringList(ruleInfo.ContentHtmlClearTagCollection);
                    attributesDict = TranslateUtils.JsonDeserialize<Dictionary<string, string>>(ruleInfo.ContentAttributesXml);
                }
                else
                {
                    ruleInfo = new GatherRuleInfo
                    {
                        SiteId = siteId,
                        Charset = ECharsetUtils.GetValue(ECharset.utf_8),
                        IsOrderByDesc = true,
                        GatherUrlIsCollection = true,
                        ContentHtmlClearCollection = "",
                        ContentHtmlClearTagCollection = ""
                    };
                    contentHtmlClearList = new List<string>
                    {
                        "script",
                        "object",
                        "iframe"
                    };
                    contentHtmlClearTagList = new List<string>
                    {
                        "font",
                        "div",
                        "span"
                    };
                    attributesDict = new Dictionary<string, string>();
                }

                var channels = new List<KeyValuePair<int, string>>();
                var channelIdList = Context.ChannelApi.GetChannelIdList(siteId);
                var isLastNodeArray = new bool[channelIdList.Count];
                foreach (var theChannelId in channelIdList)
                {
                    var channelInfo = Context.ChannelApi.GetChannelInfo(siteId, theChannelId);

                    var title = Utils.GetChannelListBoxTitle(siteId, channelInfo.Id, channelInfo.ChannelName, channelInfo.ParentsCount, channelInfo.LastNode, isLastNodeArray);
                    channels.Add(new KeyValuePair<int, string>(channelInfo.Id, title));
                }

                var charsets = ECharsetUtils.GetAllCharsets();
                
                return Ok(new
                {
                    Value = ruleInfo,
                    Channels = channels,
                    Charsets = charsets,
                    ContentHtmlClearList = contentHtmlClearList,
                    ContentHtmlClearTagList = contentHtmlClearTagList,
                    AttributesDict = attributesDict
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RouteAttributes)]
        public IHttpActionResult GetAttributes()
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var siteId = request.GetQueryInt("siteId");
                var channelId = request.GetQueryInt("channelId");
                var ruleId = request.GetQueryInt("ruleId");

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, Utils.PluginId))
                {
                    return Unauthorized();
                }

                var attributes = new List<InputListItem>();
                var contentAttributes = Context.ContentApi.GetInputStyles(siteId, channelId);
                var selectedAttributes = new List<string>();

                if (ruleId > 0)
                {
                    var ruleInfo = Main.GatherRuleRepository.GetGatherRuleInfo(ruleId);
                    selectedAttributes = TranslateUtils.StringCollectionToStringList(ruleInfo.ContentAttributes);
                }

                foreach (var attribute in contentAttributes)
                {
                    if (StringUtils.EqualsIgnoreCase(attribute.AttributeName, ContentAttribute.Title)
                     || StringUtils.EqualsIgnoreCase(attribute.AttributeName, ContentAttribute.Content)
                     || StringUtils.EqualsIgnoreCase(attribute.AttributeName, ContentAttribute.Id)
                     || StringUtils.EqualsIgnoreCase(attribute.AttributeName, ContentAttribute.LastEditDate)
                     || StringUtils.EqualsIgnoreCase(attribute.AttributeName, ContentAttribute.AddUserName)
                     || StringUtils.EqualsIgnoreCase(attribute.AttributeName, ContentAttribute.LastEditUserName)
                     || StringUtils.EqualsIgnoreCase(attribute.AttributeName, ContentAttribute.AdminId)
                     || StringUtils.EqualsIgnoreCase(attribute.AttributeName, ContentAttribute.UserId)
                     || StringUtils.EqualsIgnoreCase(attribute.AttributeName, ContentAttribute.SourceId)
                     || StringUtils.EqualsIgnoreCase(attribute.AttributeName, ContentAttribute.HitsByDay)
                     || StringUtils.EqualsIgnoreCase(attribute.AttributeName, ContentAttribute.HitsByWeek)
                     || StringUtils.EqualsIgnoreCase(attribute.AttributeName, ContentAttribute.HitsByMonth)
                     || StringUtils.EqualsIgnoreCase(attribute.AttributeName, ContentAttribute.LastHitsDate)
                     || StringUtils.EqualsIgnoreCase(attribute.AttributeName, "CheckUserName")
                     || StringUtils.EqualsIgnoreCase(attribute.AttributeName, "CheckDate")
                     || StringUtils.EqualsIgnoreCase(attribute.AttributeName, "CheckReasons")) continue;

                    var listItem = new InputListItem
                    {
                        Value = attribute.AttributeName,
                        Text = attribute.DisplayName
                    };
                    if (StringUtils.ContainsIgnoreCase(selectedAttributes, attribute.AttributeName))
                    {
                        listItem.Selected = true;
                    }
                    attributes.Add(listItem);
                }

                return Ok(new
                {
                    Value = attributes
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Submit()
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var siteId = request.GetQueryInt("siteId");

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, Utils.PluginId))
                {
                    return Unauthorized();
                }

                var ruleInfo = request.GetPostObject<GatherRuleInfo>("ruleInfo");
                var contentHtmlClearList = request.GetPostObject<List<string>>("contentHtmlClearList");
                var contentHtmlClearTagList = request.GetPostObject<List<string>>("contentHtmlClearTagList");
                var contentAttributeList = request.GetPostObject<List<string>>("contentAttributeList");
                var attributesDict = request.GetPostObject<Dictionary<string, string>>("attributesDict");
                ruleInfo.ContentHtmlClearCollection = TranslateUtils.ObjectCollectionToString(contentHtmlClearList);
                ruleInfo.ContentHtmlClearTagCollection = TranslateUtils.ObjectCollectionToString(contentHtmlClearTagList);
                ruleInfo.ContentAttributes = TranslateUtils.ObjectCollectionToString(contentAttributeList);
                ruleInfo.ContentAttributesXml = TranslateUtils.JsonSerialize(attributesDict);

                if (ruleInfo.Id > 0)
                {
                    Main.GatherRuleRepository.Update(ruleInfo);
                }
                else
                {
                    if (Main.GatherRuleRepository.IsExists(siteId, ruleInfo.GatherRuleName))
                    {
                        return BadRequest("保存失败，已存在相同名称的采集规则！");
                    }

                    ruleInfo.SiteId = siteId;
                    Main.GatherRuleRepository.Insert(ruleInfo);
                }

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
