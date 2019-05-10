using System;
using System.Collections.Generic;
using System.Text;
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
        private const string RouteActionsGetContent = "actions/getContent";

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
    
        [HttpPost, Route(RouteActionsGetContent)]
        public IHttpActionResult GetContent()
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

                var contentUrl = request.GetPostString("contentUrl");
                var ruleInfo = Main.GatherRuleRepository.GetGatherRuleInfo(ruleId);

                var regexContentExclude = GatherUtility.GetRegexString(ruleInfo.ContentExclude);
                var regexChannel = GatherUtility.GetRegexChannel(ruleInfo.ContentChannelStart, ruleInfo.ContentChannelEnd);
                var regexContent = GatherUtility.GetRegexContent(ruleInfo.ContentContentStart, ruleInfo.ContentContentEnd);
                var regexContent2 = string.Empty;
                if (!string.IsNullOrEmpty(ruleInfo.ContentContentStart2) && !string.IsNullOrEmpty(ruleInfo.ContentContentEnd2))
                {
                    regexContent2 = GatherUtility.GetRegexContent(ruleInfo.ContentContentStart2, ruleInfo.ContentContentEnd2);
                }
                var regexContent3 = string.Empty;
                if (!string.IsNullOrEmpty(ruleInfo.ContentContentStart3) && !string.IsNullOrEmpty(ruleInfo.ContentContentEnd3))
                {
                    regexContent3 = GatherUtility.GetRegexContent(ruleInfo.ContentContentStart3, ruleInfo.ContentContentEnd3);
                }
                var regexNextPage = GatherUtility.GetRegexUrl(ruleInfo.ContentNextPageStart, ruleInfo.ContentNextPageEnd);
                var regexTitle = GatherUtility.GetRegexTitle(ruleInfo.ContentTitleStart, ruleInfo.ContentTitleEnd);
                var contentAttributes = TranslateUtils.StringCollectionToStringList(ruleInfo.ContentAttributes);
                var contentAttributesXml = TranslateUtils.ToNameValueCollection(ruleInfo.ContentAttributesXml);

                var attributes = GatherUtility.GetContentNameValueCollection(ruleInfo.Charset, contentUrl, ruleInfo.CookieString, regexContentExclude, ruleInfo.ContentHtmlClearCollection, ruleInfo.ContentHtmlClearTagCollection, regexTitle, regexContent, regexContent2, regexContent3, regexNextPage, regexChannel, contentAttributes, contentAttributesXml);

                var list = new List<KeyValuePair<string, string>>();

                foreach (var attributeName in ContentAttribute.AllAttributes.Value)
                {
                    var value = attributes[attributeName.ToLower()];

                    if (string.IsNullOrEmpty(value)) continue;
                    if (StringUtils.EqualsIgnoreCase(ContentAttribute.ImageUrl, attributeName))
                    {
                        var imageUrl = PageUtils.GetUrlByBaseUrl(value, contentUrl);
                        list.Add(new KeyValuePair<string, string>(attributeName, imageUrl));
                    }
                    else
                    {
                        list.Add(new KeyValuePair<string, string>(attributeName, value));
                    }
                }
                
                return Ok(new
                {
                    Value = list
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    
    }
}
