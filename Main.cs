using System.Collections.Generic;
using SiteServer.Plugin;
using SS.Gather.Core;

namespace SS.Gather
{
    public class Main : PluginBase
    {
        public static GatherRuleRepository GatherRuleRepository { get; private set; }

        public override void Startup(IService service)
        {
            GatherRuleRepository = new GatherRuleRepository();

            service
                .AddSiteMenu(siteId => new Menu
                {
                    Text = "信息采集",
                    Href = "pages/rules.html",
                    IconClass = "fa fa-download",
                    Menus = new List<Menu>
                    {
                        new Menu
                        {
                            Text = "添加采集规则",
                            Href = "pages/add.html",
                        },
                        new Menu
                        {
                            Text = "采集规则管理",
                            Href = "pages/rules.html",
                        }
                    }
                })
                .AddDatabaseTable(GatherRuleRepository.TableName, GatherRuleRepository.TableColumns)
                ;
        }
    }
}