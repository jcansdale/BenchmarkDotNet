using System;
using BenchmarkDotNet.Engine;
using BenchmarkDotNet.Horology;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using Xunit.Abstractions;

namespace BenchmarkDotNet.Tests.Mocks
{
    public class MockMeasureEngine : IMeasureEngine
    {
        private readonly ITestOutputHelper output;
        private readonly Func<IterationData, TimeInterval> measure;

        public MockMeasureEngine(ITestOutputHelper output, IJob job, Func<IterationData, TimeInterval> measure)
        {
            this.output = output;
            this.measure = measure;
            TargetJob = job;
        }

        public IJob TargetJob { get; set; }
        public long OperationsPerInvoke { get; set; } = 1;
        public Action SetupAction { get; set; }
        public Action CleanupAction { get; set; }
        public Action<long> MainAction { get;  } = _ => { };
        public Action<long> IdleAction { get; } = _ => { };

        public Measurement RunIteration(IterationData data)
        {
            double nanoseconds = measure(data).Nanoseconds;
            var measurement = new Measurement(1, data.IterationMode, data.Index, data.InvokeCount * OperationsPerInvoke, nanoseconds);
            WriteLine(measurement.ToOutputLine());
            return measurement;
        }

        public void WriteLine()
        {
            output.WriteLine("");
        }

        public void WriteLine(string line)
        {
            output.WriteLine(line);
        }
    }
}