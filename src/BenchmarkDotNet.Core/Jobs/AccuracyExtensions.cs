using System;
using BenchmarkDotNet.Horology;

namespace BenchmarkDotNet.Jobs
{
    public static class AccuracyExtensions
    {
        public static Accuracy WithEvaluateOverhead(this Accuracy accuracy, bool value) => accuracy.With(a => a.EvaluateOverhead = value);
        public static Accuracy MaxStdErrRelative(this Accuracy accuracy, double value) => accuracy.With(a => a.MaxStdErrRelative = value);
        public static Accuracy MinInvokeCount(this Accuracy accuracy, int value) => accuracy.With(a => a.MinInvokeCount = value);
        public static Accuracy MinIterationTime(this Accuracy accuracy, TimeInterval value) => accuracy.With(a => a.MinIterationTime = value);
        public static Accuracy RemoveOutliers(this Accuracy accuracy, bool value) => accuracy.With(a => a.RemoveOutliers = value);

        private static Accuracy With(this Accuracy accuracy, Action<Accuracy> set)
        {
            var newJob = accuracy.Clone();
            set(newJob);
            return newJob;
        }

        public static Accuracy Clone(this Accuracy accuracy) => new Accuracy()
        {
            EvaluateOverhead = accuracy.EvaluateOverhead,
            MaxStdErrRelative = accuracy.MaxStdErrRelative,
            MinInvokeCount = accuracy.MinInvokeCount,
            MinIterationTime = accuracy.MinIterationTime,
            RemoveOutliers = accuracy.RemoveOutliers
        };
    }
}