using System;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using JetBrains.Annotations;

namespace BenchmarkDotNet.Engine
{
    public interface IMeasureEngine
    {
        [NotNull]
        IJob TargetJob { get; set; }

        long OperationsPerInvoke { get; set; }

        [CanBeNull]
        Action SetupAction { get; set; }

        [CanBeNull]
        Action CleanupAction { get; set; }

        [NotNull]
        Action<long> MainAction { get; }

        [NotNull]
        Action<long> IdleAction { get; }

        Measurement RunIteration(IterationData data);

        void WriteLine();
        void WriteLine(string line);
    }
}