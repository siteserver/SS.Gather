using System;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Gather.Core;

namespace SS.Gather.Controllers.Pages
{
    [RoutePrefix("pages/rulesLayerTest")]
    public class RulesLayerTestController : ApiController
    {
        private const string Route = "";
        private const string RouteActionsGetContentUrls = "actions/getContentUrls";

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
                var gatherUrls = GatherUtility.GetGatherUrlList(ruleInfo);
                
                return Ok(new
                {
                    Value = ruleInfo,
                    GatherUrls = gatherUrls
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsGetContentUrls)]
        public IHttpActionResult GetContentUrls()
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

                var gatherUrl = request.GetPostString("gatherUrl");
                var ruleInfo = Main.GatherRuleRepository.GetGatherRuleInfo(ruleId);

                var regexUrlInclude = GatherUtility.GetRegexString(ruleInfo.UrlInclude);
                var regexListArea = GatherUtility.GetRegexArea(ruleInfo.ListAreaStart, ruleInfo.ListAreaEnd);

                var contentUrls = GatherUtility.GetContentUrls(gatherUrl, ruleInfo.Charset, ruleInfo.CookieString, regexListArea, regexUrlInclude);
                
                return Ok(new
                {
                    Value = contentUrls
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
