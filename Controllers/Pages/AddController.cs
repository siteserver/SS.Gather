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
                if (ruleId > 0)
                {
                    ruleInfo = Main.GatherRuleRepository.GetGatherRuleInfo(ruleId);
                    contentHtmlClearList =
                        TranslateUtils.StringCollectionToStringList(ruleInfo.ContentHtmlClearCollection);
                    contentHtmlClearTagList = TranslateUtils.StringCollectionToStringList(ruleInfo.ContentHtmlClearTagCollection);
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
                    ContentHtmlClearTagList = contentHtmlClearTagList
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
