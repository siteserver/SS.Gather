using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Gather.Core;

namespace SS.Gather.Controllers.Pages
{
    [RoutePrefix("pages/rulesLayerContents")]
    public class RulesLayerContentsController : ApiController
    {
        private const string Route = "";
        private const string RouteActionsGather = "actions/gather";
        private const string RouteActionsGetStatus = "actions/getStatus";
        private const string RouteImport = "actions/import";

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

                var adminToken = Context.AdminApi.GetAccessToken(request.AdminId, request.AdminName, TimeSpan.FromDays(1));

                return Ok(new
                {
                    Value = ruleInfo,
                    Channels = channels,
                    AdminToken = adminToken,
                    Guid = Guid.NewGuid().ToString()
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

                var channelId = request.GetPostInt("channelId");
                var guid = request.GetPostString("guid");
                var gatherType = request.GetPostString("gatherType");
                var fileName = request.GetPostString("fileName");
                var urls = TranslateUtils.StringCollectionToStringList(request.GetPostString("urls"), '\n');
                if (gatherType == "excel")
                {
                    var filePath = Context.UtilsApi.GetTemporaryFilesPath(fileName);
                    urls = ExcelUtils.GetContentUrls(filePath);
                }

                var adminInfo = Context.AdminApi.GetAdminInfoByUserId(request.AdminId);

                Main.GatherRuleRepository.GatherContents(adminInfo, siteId, ruleId, channelId, guid, urls);

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

        [HttpPost, Route(RouteImport)]
        public IHttpActionResult Import()
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var siteId = request.GetQueryInt("siteId");

                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, Utils.PluginId)) return Unauthorized();

                foreach (string name in HttpContext.Current.Request.Files)
                {
                    var postFile = HttpContext.Current.Request.Files[name];

                    if (postFile == null)
                    {
                        return BadRequest("Could not read zip from body");
                    }

                    var extName = Path.GetExtension(postFile.FileName);

                    var filePath = Context.UtilsApi.GetTemporaryFilesPath(postFile.FileName);
                    FileUtils.DeleteFileIfExists(filePath);

                    if (!StringUtils.EqualsIgnoreCase(extName, ".xls") && !StringUtils.EqualsIgnoreCase(extName, ".xlsx") && !StringUtils.EqualsIgnoreCase(extName, ".cvs"))
                    {
                        return BadRequest("Excel file extension is not correct");
                    }

                    postFile.SaveAs(filePath);
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
