namespace BenchmarkDotNet.Jobs
{
    public enum Mode
    {
        /// <summary>
        /// A mode without overhead evaluating and warmup, with single invocation
        /// </summary>
        ColdStart,
        Throughput
    }
}