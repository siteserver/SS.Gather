using System;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Gather.Core;

namespace SS.Gather.Controllers.Pages
{
    [RoutePrefix("pages/rules")]
    public class RulesController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetList()
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

                var ruleInfoList = Main.GatherRuleRepository.GetGatherRuleInfoList(siteId);
                foreach (var ruleInfo in ruleInfoList)
                {
                    var gatherUrlList = GatherUtility.GetGatherUrlList(ruleInfo);
                    if (gatherUrlList != null && gatherUrlList.Count > 0)
                    {
                        var url = gatherUrlList[0];
                        ruleInfo.Set("gatherUrl", url);
                    }
                }

                return Ok(new
                {
                    Value = ruleInfoList
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route(Route)]
        public IHttpActionResult Delete()
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

                Main.GatherRuleRepository.Delete(ruleId);

                var ruleInfoList = Main.GatherRuleRepository.GetGatherRuleInfoList(siteId);

                return Ok(new
                {
                    Value = ruleInfoList
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
