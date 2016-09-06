using System;
using BenchmarkDotNet.Engine;
using BenchmarkDotNet.Horology;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Tests.Mocks;
using Xunit;
using Xunit.Abstractions;

namespace BenchmarkDotNet.Tests.Engine
{
    public class MeasureEngineWarmupStageTests
    {
        private const int MinIterationCount = MeasureEngineWarmupStage.MinIterationCount;
        private const int MaxIterationCount = MeasureEngineWarmupStage.MaxIterationCount;
        private const int MaxIdleItertaionCount = MeasureEngineWarmupStage.MaxIdleItertaionCount;

        private readonly ITestOutputHelper output;

        public MeasureEngineWarmupStageTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void AutoTest_SteadyState()
        {
            AutoTest(data => TimeInterval.Millisecond, MinIterationCount);
        }

        [Fact]
        public void AutoTest_InfiniteIncrease()
        {
            AutoTest(data => TimeInterval.Millisecond * data.Index, MaxIterationCount);
        }

        [Fact]
        public void AutoTest_Alternation()
        {
            AutoTest(data => TimeInterval.Millisecond * (data.Index % 2), MinIterationCount, MaxIterationCount);
        }

        [Fact]
        public void AutoTest_TenSteps()
        {
            AutoTest(data => TimeInterval.Millisecond * Math.Max(0, 10 - data.Index), 10, MaxIterationCount);
        }

        [Fact]
        public void AutoTest_WithoutSteadyStateIdle()
        {
            AutoTest(data => TimeInterval.Millisecond * data.Index, MaxIdleItertaionCount, mode: IterationMode.IdleWarmup);
        }

        private void AutoTest(Func<IterationData, TimeInterval> measure, int min, int max = -1, IterationMode mode = IterationMode.MainWarmup)
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

        private MeasureEngineWarmupStage CreateStage(IJob job, Func<IterationData, TimeInterval> measure)
        {
            var engine = new MockMeasureEngine(output, job, measure);
            return new MeasureEngineWarmupStage(engine);
        }
    }
}