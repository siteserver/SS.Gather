using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NDesk.Options;
using SiteServer.Plugin;
using SS.Gather.Core;

namespace SS.Gather.Cli
{
    public class GatherJob
    {
        public const string CommandName = "gather";

        private static List<int> _includes;
        private static List<int> _excludes;
        private static bool _isHelp;

        private static readonly OptionSet Options = new OptionSet() {
            { "includes=", "指定需要备份的表，多个表用英文逗号隔开，默认备份所有表",
                v => _includes = v == null ? null : TranslateUtils.StringCollectionToIntList(v) },
            { "excludes=", "指定需要排除的表，多个表用英文逗号隔开",
                v => _excludes = v == null ? null : TranslateUtils.StringCollectionToIntList(v) },
            { "h|help",  "命令说明",
                v => _isHelp = v != null }
        };

        public static void PrintUsage()
        {
            Console.WriteLine("信息采集: siteserver gather");
            Options.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
        }

        public static async Task Execute(IJobContext context)
        {
            if (!CliUtils.ParseArgs(Options, context.Args)) return;

            if (_isHelp)
            {
                PrintUsage();
                return;
            }

            var ruleList = Main.GatherRuleRepository.GetGatherRuleIdList(_includes, _excludes);
            foreach(var (id, siteId) in ruleList)
            {
                var guid = Guid.NewGuid().ToString();

                await Main.GatherRuleRepository.GatherChannelsAsync(null, siteId, id, guid, true);
            }

            await CliUtils.PrintRowLineAsync();
            await Console.Out.WriteLineAsync("恭喜，采集任务执行成功！");
        }
    }
}
