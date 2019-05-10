using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Gather.Core;

namespace SS.Gather.Controllers.Pages
{
    [RoutePrefix("pages/rulesLayerGather")]
    public class RulesLayerGatherController : ApiController
    {
        private const string Route = "";
        private const string RouteActionsGather = "actions/gather";
        private const string RouteActionsGetStatus = "actions/getStatus";

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

                var ruleInfo = Main.GatherRuleRepository.GetGatherRuleInfo(ruleId);

                var channels = new List<KeyValuePair<int, string>>();
                var channelIdList = Context.ChannelApi.GetChannelIdList(siteId);
                var isLastNodeArray = new bool[channelIdList.Count];
                foreach (var theChannelId in channelIdList)
                {
                    var channelInfo = Context.ChannelApi.GetChannelInfo(siteId, theChannelId);

                    var title = Utils.GetChannelListBoxTitle(siteId, channelInfo.Id, channelInfo.ChannelName, channelInfo.ParentsCount, channelInfo.LastNode, isLastNodeArray);
                    channels.Add(new KeyValuePair<int, string>(channelInfo.Id, title));
                }

                return Ok(new
                {
                    Value = ruleInfo,
                    Channels = channels
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
                var ruleId = request.GetQueryInt("ruleId");

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, Utils.PluginId))
                {
                    return Unauthorized();
                }

                var ruleInfo = Main.GatherRuleRepository.GetGatherRuleInfo(ruleId);
                ruleInfo.ChannelId = request.GetPostInt("channelId");
                ruleInfo.GatherNum = request.GetPostInt("gatherNum");
                ruleInfo.GatherUrlIsCollection = request.GetPostBool("gatherUrlIsCollection");
                ruleInfo.GatherUrlIsSerialize = request.GetPostBool("gatherUrlIsSerialize");
                ruleInfo.GatherUrlCollection = request.GetPostString("gatherUrlCollection");
                ruleInfo.GatherUrlSerialize = request.GetPostString("gatherUrlSerialize");
                ruleInfo.SerializeFrom = request.GetPostInt("serializeFrom");
                ruleInfo.SerializeTo = request.GetPostInt("serializeTo");
                ruleInfo.SerializeInterval = request.GetPostInt("serializeInterval");
                ruleInfo.SerializeIsOrderByDesc = request.GetPostBool("serializeIsOrderByDesc");
                ruleInfo.SerializeIsAddZero = request.GetPostBool("serializeIsAddZero");
                ruleInfo.UrlInclude = request.GetPostString("urlInclude");

                Main.GatherRuleRepository.Update(ruleInfo);

                return Ok(new
                {
                    Value = Guid.NewGuid().ToString()
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsGather)]
        public IHttpActionResult Gather()
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

                var guid = request.GetPostString("guid");
                var ruleInfo = Main.GatherRuleRepository.GetGatherRuleInfo(ruleId);

                var adminInfo = Context.AdminApi.GetAdminInfoByUserId(request.AdminId);

                Task.Run(() => GatherUtility.Gather(adminInfo, siteId, ruleId, guid)).ConfigureAwait(false).GetAwaiter();

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

        [HttpPost, Route(RouteActionsGetStatus)]
        public IHttpActionResult GetStatus()
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

                var guid = request.GetPostString("guid");
                var cache = ProgressCache.GetCache(guid);

                var percentage = cache.TotalCount == 0 ? string.Empty : (cache.SuccessCount / cache.TotalCount).ToString("0.00%");

                return Ok(new
                {
                    Value = cache,
                    Percentage = percentage
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}
