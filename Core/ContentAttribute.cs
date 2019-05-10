using System;
using System.Collections.Generic;
using SiteServer.Plugin;

namespace SS.Gather.Core
{
    public static class ContentAttribute
    {
        public const string Id = nameof(IContentInfo.Id);
        public const string ChannelId = nameof(IContentInfo.ChannelId);
        public const string SiteId = nameof(IContentInfo.SiteId);
        public const string AddUserName = nameof(IContentInfo.AddUserName);
        public const string LastEditUserName = nameof(IContentInfo.LastEditUserName);
        public const string LastEditDate = nameof(IContentInfo.LastEditDate);
        public const string AdminId = nameof(IContentInfo.AdminId);
        public const string UserId = nameof(IContentInfo.UserId);
        public const string Taxis = nameof(IContentInfo.Taxis);
        public const string GroupNameCollection = nameof(IContentInfo.GroupNameCollection);
        public const string Tags = nameof(IContentInfo.Tags);
        public const string SourceId = nameof(IContentInfo.SourceId);
        public const string ReferenceId = nameof(IContentInfo.ReferenceId);
        public const string IsChecked = nameof(IsChecked);
        public const string CheckedLevel = nameof(IContentInfo.CheckedLevel);
        public const string Hits = nameof(IContentInfo.Hits);
        public const string HitsByDay = nameof(IContentInfo.HitsByDay);
        public const string HitsByWeek = nameof(IContentInfo.HitsByWeek);
        public const string HitsByMonth = nameof(IContentInfo.HitsByMonth);
        public const string LastHitsDate = nameof(IContentInfo.LastHitsDate);
        public const string Downloads = nameof(Downloads);
        public const string SettingsXml = nameof(SettingsXml);
        public const string Title = nameof(IContentInfo.Title);
        public const string IsTop = nameof(IsTop);
        public const string IsRecommend = nameof(IsRecommend);
        public const string IsHot = nameof(IsHot);
        public const string IsColor = nameof(IsColor);
        public const string LinkUrl = nameof(IContentInfo.LinkUrl);
        public const string AddDate = nameof(IContentInfo.AddDate);

        public const string ImageUrl = nameof(ImageUrl);
        public const string VideoUrl = nameof(VideoUrl);
        public const string FileUrl = nameof(FileUrl);
        public const string Content = nameof(Content);
        public static readonly Lazy<List<string>> AllAttributes = new Lazy<List<string>>(() => new List<string>
        {
            Id,
            ChannelId,
            SiteId,
            AddUserName,
            LastEditUserName,
            LastEditDate,
            AdminId,
            UserId,
            Taxis,
            GroupNameCollection,
            Tags,
            SourceId,
            ReferenceId,
            IsChecked,
            CheckedLevel,
            Hits,
            HitsByDay,
            HitsByWeek,
            HitsByMonth,
            LastHitsDate,
            Downloads,
            SettingsXml,
            Title,
            IsTop,
            IsRecommend,
            IsHot,
            IsColor,
            LinkUrl,
            AddDate,
            ImageUrl,
            VideoUrl,
            FileUrl,
            Content,
        });
    }
}
