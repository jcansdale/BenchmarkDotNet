using System;
using BenchmarkDotNet.Engine;
using BenchmarkDotNet.Extensions;
using BenchmarkDotNet.Horology;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Tests.Mocks;
using Xunit;
using Xunit.Abstractions;

namespace BenchmarkDotNet.Tests.Engine
{
    public class MeasureEnginePilotStageTests
    {
        private const long MaxPossibleInvokeCount = MeasureEnginePilotStage.MaxInvokeCount;
        private readonly ITestOutputHelper output;

        public MeasureEnginePilotStageTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void AutoTest_BigResolution() => AutoTest(
            TimeInterval.Millisecond.ToFrequency(),
            TimeInterval.Millisecond,
            Accuracy.Default.MaxStdErrRelative(0.01),
            200);

        [Fact]
        public void AutoTest_ImpossibleResolution() => AutoTest(
            TimeInterval.Second.ToFrequency(),
            TimeInterval.Millisecond,
            Accuracy.Default.MaxStdErrRelative(0),
            MeasureEnginePilotStage.MaxInvokeCount);

        [Fact]
        public void SpecificTest_Simple() => SpecificTest(
            TimeInterval.Millisecond * 100,
            TimeInterval.Millisecond,
            64,
            128);

        private void AutoTest(Frequency clockFrequency, TimeInterval operationTime, Accuracy accuracy, long minInvokeCount)
        {
            var job = Job.Default.
                With(new MockClock(clockFrequency)).
                With(accuracy);
            var stage = CreateStage(job, data => data.InvokeCount * operationTime);
            long invokeCount = stage.Run();
            output.WriteLine($"InvokeCount = {invokeCount} (Min= {minInvokeCount}, Max = {MaxPossibleInvokeCount})");
            Assert.InRange(invokeCount, minInvokeCount, MaxPossibleInvokeCount);
        }

        private void SpecificTest(TimeInterval iterationTime, TimeInterval operationTime, long minInvokeCount, long maxInvokeCount)
        {
            var job = Job.Default.
                With(new MockClock(Frequency.MHz)).
                WithIterationTime(iterationTime.ToMilliseconds().RoundToInt());
            var stage = CreateStage(job, data => data.InvokeCount * operationTime);
            long invokeCount = stage.Run();
            output.WriteLine($"InvokeCount = {invokeCount} (Min= {minInvokeCount}, Max = {maxInvokeCount})");
            Assert.InRange(invokeCount, minInvokeCount, maxInvokeCount);

        }

        private MeasureEnginePilotStage CreateStage(IJob job, Func<IterationData, TimeInterval> measure)
        {
            var engine = new MockMeasureEngine(output, job, measure);
            return new MeasureEnginePilotStage(engine);
        }
    }
}