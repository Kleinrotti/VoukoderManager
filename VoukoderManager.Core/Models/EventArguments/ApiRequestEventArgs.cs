using Octokit;
using System;

namespace VoukoderManager.Core
{
    public class ApiRequestEventArgs
    {
        public int RateLimit { get; }
        public int Remaining { get; }
        public DateTime Reset { get; }

        public ApiRequestEventArgs(ApiInfo apiInfo)
        {
            RateLimit = apiInfo.RateLimit.Limit;
            Remaining = apiInfo.RateLimit.Remaining;
            Reset = apiInfo.RateLimit.Reset.LocalDateTime;
        }
    }
}