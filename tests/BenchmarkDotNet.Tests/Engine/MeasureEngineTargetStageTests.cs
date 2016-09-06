using System;
using BenchmarkDotNet.Engine;
using BenchmarkDotNet.Horology;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Tests.Mocks;
using Xunit;
using Xunit.Abstractions;

namespace BenchmarkDotNet.Tests.Engine
{
    public class MeasureEngineTargetStageTests
    {
        private const int MinIterationCount = MeasureEngineTargetStage.MinIterationCount;
        private const int MaxIterationCount = MeasureEngineTargetStage.MaxIterationCount;
        private const int MaxIdleIterationCount = MeasureEngineTargetStage.MaxIdleIterationCount;

        private readonly ITestOutputHelper output;

        public MeasureEngineTargetStageTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void AutoTest_SteadyState() => AutoTest(data => TimeInterval.Second, MinIterationCount);

        [Fact]
        public void AutoTest_InfiniteIncrease() => AutoTest(data => TimeInterval.Second * data.Index, MaxIterationCount);

        [Fact]
        public void AutoTest_InfiniteIncreaseIdle() => AutoTest(data => TimeInterval.Second * data.Index, MaxIdleIterationCount, mode: IterationMode.IdleTarget);

        private void AutoTest(Func<IterationData, TimeInterval> measure, int min, int max = -1, IterationMode mode = IterationMode.MainTarget)
        {
            if (max == -1)
                max = min;
            var job = Job.Default;
            var stage = CreateStage(job, measure);
            var measurements = stage.Run(1, mode, Count.Auto);
            int count = measurements.Count;
            output.WriteLine($"MeasurementCount = {count} (Min= {min}, Max = {max})");
            Assert.InRange(count, min, max);
        }

        private MeasureEngineTargetStage CreateStage(IJob job, Func<IterationData, TimeInterval> measure)
        {
            var engine = new MockMeasureEngine(output, job, measure);
            return new MeasureEngineTargetStage(engine);
        }
    }
}