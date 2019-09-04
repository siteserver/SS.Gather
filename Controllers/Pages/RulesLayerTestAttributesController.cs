using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Gather.Core;

namespace SS.Gather.Controllers.Pages
{
    [RoutePrefix("pages/rulesLayerTestAttributes")]
    public class RulesLayerTestAttributesController : ApiController
    {
        private const string Route = "";

        [HttpPost, Route(Route)]
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
                var attributesDict = TranslateUtils.JsonDeserialize<Dictionary<string, string>>(ruleInfo.ContentAttributesXml);

                var attributes = GatherUtility.GetContentNameValueCollection(ruleInfo.Charset, contentUrl, ruleInfo.CookieString, regexContentExclude, ruleInfo.ContentHtmlClearCollection, ruleInfo.ContentHtmlClearTagCollection, regexTitle, regexContent, regexContent2, regexContent3, regexNextPage, regexChannel, contentAttributes, attributesDict);

                var list = new List<KeyValuePair<string, string>>();

                //var contentAttributes = Context.ContentApi.GetInputStyles(siteId, ruleInfo.ChannelId);

                foreach (var attributeName in attributes.AllKeys)
                {
                    var value = attributes[attributeName];

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
