using BenchmarkDotNet.Toolchains;
using System;
using BenchmarkDotNet.Horology;
using JetBrains.Annotations;

namespace BenchmarkDotNet.Jobs
{
    public interface IJob : IEquatable<IJob>
    {
        Mode Mode { get; }
        Platform Platform { get; }
        Jit Jit { get; }
        IToolchain Toolchain { get; }
        Runtime Runtime { get; }

        /// <summary>
        /// no value means Host
        /// </summary>
        Property<GcMode> GcMode { get; }

        Count LaunchCount { get; }
        Count WarmupCount { get; }
        Count TargetCount { get; }

        /// <summary>
        /// Desired time of execution of an iteration (in ms).
        /// </summary>
        Count IterationTime { get; }

        /// <summary>
        /// ProcessorAffinity for the benchmark process.
        /// <seealso href="https://msdn.microsoft.com/library/system.diagnostics.process.processoraffinity.aspx"/>
        /// </summary>
        Count Affinity { get; }

        /// <summary>
        /// Accuracy requirements.
        /// </summary>
        Accuracy Accuracy { get; }

        /// <summary>
        /// Clock for measurements. null values corresponds to Chronometer (best available clock).
        /// </summary>
        [CanBeNull]
        IClock Clock { get; }

        Property[] AllProperties { get; }
    }
}