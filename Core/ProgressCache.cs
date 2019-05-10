using System.Collections.Generic;

namespace SS.Gather.Core
{
    public class ProgressCache
    {
        public const string StatusProgress = "progress";
        public const string StatusSuccess = "success";
        public const string StatusFailure = "failure";

        public static ProgressCache Init(string guid, string message)
        {
            if (string.IsNullOrEmpty(guid)) return null;

            var cache = new ProgressCache
            {
                Status = StatusProgress,
                IsSuccess = true,
                Message = message,
                FailureMessages = new List<string>()
            };
            CacheUtils.InsertHours(guid, cache, 1);
            return cache;
        }

        public static ProgressCache GetCache(string guid)
        {
            return CacheUtils.Get(guid) as ProgressCache;
        }

        private ProgressCache()
        {

        }

        public string Status { get; set; }
        public int TotalCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public List<string> FailureMessages { get; set; }
    }
}