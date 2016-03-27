using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using System;
using System.Collections.Generic;
namespace Implem.Libraries.Utilities
{
    public static class Azures
    {
        private static int RetryCount;

        public static void SetRetryManager(int retryCount, int retryInterval)
        {
            RetryCount = retryCount;
            const string defaultRetryStrategyName = "fixed";
            var fixedInterval = new FixedInterval(
                defaultRetryStrategyName,
                RetryCount,
                TimeSpan.FromSeconds(retryInterval));
            var retryStrategyCollection = new List<RetryStrategy>
            {
                fixedInterval
            };
            var retryManager = new RetryManager(
                retryStrategyCollection,
                defaultRetryStrategyName);
            RetryManager.SetDefault(retryManager);
        }

        public static RetryPolicy RetryPolicy()
        {
            return new RetryPolicy(
                new SqlDatabaseTransientErrorDetectionStrategy(),
                RetryCount);
        }
    }
}