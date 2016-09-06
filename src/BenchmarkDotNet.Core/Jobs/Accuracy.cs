using System;
using BenchmarkDotNet.Horology;

namespace BenchmarkDotNet.Jobs
{
    public sealed class Accuracy
    {
        public double MaxStdErrRelative { get; set; } = 0.01;
        public TimeInterval MinIterationTime { get; set; } = TimeInterval.Millisecond * 200;
        public int MinInvokeCount { get; set; }= 4;
        public bool EvaluateOverhead { get; set; } = true;
        public bool RemoveOutliers { get; set; } = true;

        public static readonly Accuracy Default = new Accuracy();
    }
}