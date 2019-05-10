using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Gather.Core;

namespace SS.Gather.Controllers.Pages
{
    [RoutePrefix("pages/rulesLayerCopy")]
    public class RulesLayerCopyController : ApiController
    {
        private const string Route = "";

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

                var ruleId = request.GetPostInt("ruleId");
                var ruleName = request.GetPostString("ruleName");

                if (Main.GatherRuleRepository.IsExists(siteId, ruleName))
                {
                    return BadRequest("复制失败，已存在相同名称的采集规则！");
                }

                var ruleInfo = Main.GatherRuleRepository.GetGatherRuleInfo(ruleId);
                ruleInfo.GatherRuleName = ruleName;
                ruleInfo.LastGatherDate = null;

                Main.GatherRuleRepository.Insert(ruleInfo);

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
