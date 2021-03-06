﻿using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Horology;

namespace BenchmarkDotNet.TestDriven
{
    // See https://perfdotnet.github.io/BenchmarkDotNet/faq.htm
    public class FastAndDirtyConfig : ManualConfig
    {
        public FastAndDirtyConfig()
        {
            Add(Job.Default
                .WithLaunchCount(1)     // benchmark process will be launched only once
                .WithIterationTime(TimeInterval.FromMilliseconds(100)) // 100ms per iteration
                .WithWarmupCount(3)     // 3 warmup iteration
                .WithTargetCount(3)     // 3 target iteration
            );
        }
    }
}
