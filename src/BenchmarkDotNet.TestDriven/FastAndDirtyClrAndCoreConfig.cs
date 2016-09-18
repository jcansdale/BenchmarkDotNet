using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Horology;

namespace BenchmarkDotNet.TestDriven
{
    // See https://perfdotnet.github.io/BenchmarkDotNet/faq.htm
    public class FastAndDirtyClrAndCoreConfig : ManualConfig
    {
        public FastAndDirtyClrAndCoreConfig()
        {
            Add(Job.Clr
                .WithLaunchCount(1)     // benchmark process will be launched only once
                .WithIterationTime(TimeInterval.FromMilliseconds(100)) // 100ms per iteration
                .WithWarmupCount(3)     // 3 warmup iteration
                .WithTargetCount(3)     // 3 target iteration
            );

            Add(Job.Core
                .WithLaunchCount(1)     // benchmark process will be launched only once
                .WithIterationTime(TimeInterval.FromMilliseconds(100)) // 100ms per iteration
                .WithWarmupCount(3)     // 3 warmup iteration
                .WithTargetCount(3)     // 3 target iteration
            );
        }
    }
}
