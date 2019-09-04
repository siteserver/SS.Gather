using System;
using Datory;

namespace SS.Gather.Core
{
    [Table("ss_gather_rule")]
    public class GatherRuleInfo : Entity
    {
        [TableColumn]
        public string GatherRuleName { get; set; }

        [TableColumn]
        public int SiteId { get; set; }

        [TableColumn(Text = true)]
        public string CookieString { get; set; }

        [TableColumn]
        public bool GatherUrlIsCollection { get; set; }

        [TableColumn(Text = true)]
        public string GatherUrlCollection { get; set; }

        [TableColumn]
        public bool GatherUrlIsSerialize { get; set; }

        [TableColumn(Text = true)]
        public string GatherUrlSerialize { get; set; }

        [TableColumn]
        public int SerializeFrom { get; set; }

        [TableColumn]
        public int SerializeTo { get; set; }

        [TableColumn]
        public int SerializeInterval { get; set; }

        [TableColumn]
        public bool SerializeIsOrderByDesc { get; set; }

        [TableColumn]
        public bool SerializeIsAddZero { get; set; }

        [TableColumn]
        public int ChannelId { get; set; }

        [TableColumn]
        public string Charset { get; set; }

        [TableColumn(Text = true)]
        public string UrlInclude { get; set; }

        [TableColumn(Text = true)]
        public string TitleInclude { get; set; }

        [TableColumn(Text = true)]
        public string ContentExclude { get; set; }

        [TableColumn(Text = true)]
        public string ContentHtmlClearCollection { get; set; }

        [TableColumn(Text = true)]
        public string ContentHtmlClearTagCollection { get; set; }

        [TableColumn]
        public DateTime? LastGatherDate { get; set; }

        [TableColumn(Text = true)]
        public string ListAreaStart { get; set; }

        [TableColumn(Text = true)]
        public string ListAreaEnd { get; set; }

        [TableColumn(Text = true)]
        public string ContentChannelStart { get; set; }

        [TableColumn(Text = true)]
        public string ContentChannelEnd { get; set; }

        [TableColumn(Text = true)]
        public string ContentTitleStart { get; set; }

        [TableColumn(Text = true)]
        public string ContentTitleEnd { get; set; }

        [TableColumn(Text = true)]
        public string ContentContentStart { get; set; }

        [TableColumn(Text = true)]
        public string ContentContentEnd { get; set; }

        [TableColumn(Text = true)]
        public string ContentNextPageStart { get; set; }

        [TableColumn(Text = true)]
        public string ContentNextPageEnd { get; set; }

        [TableColumn(Text = true)]
        public string ContentAttributes { get; set; }

        [TableColumn(Text = true)]
        public string ContentAttributesXml { get; set; }

        [TableColumn(Text = true, Extend = true)]
        public string ExtendValues { get; set; }

        public int GatherNum { get; set; }

        public bool IsSaveImage { get; set; }

        public bool IsSetFirstImageAsImageUrl { get; set; }

        public bool IsSaveFiles { get; set; }

        public bool IsEmptyContentAllowed { get; set; }

        public bool IsSameTitleAllowed { get; set; }

        public bool IsChecked { get; set; }

        public bool IsAutoCreate { get; set; }

        public bool IsOrderByDesc { get; set; }

        public string ContentContentStart2 { get; set; }

        public string ContentContentEnd2 { get; set; }

        public string ContentContentStart3 { get; set; }

        public string ContentContentEnd3 { get; set; }

        public string ContentReplaceFrom { get; set; }

        public string ContentReplaceTo { get; set; }
    }
}
