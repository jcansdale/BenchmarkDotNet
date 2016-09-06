using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Mathematics;
using BenchmarkDotNet.Reports;

namespace BenchmarkDotNet.Engine
{
    public class MeasureEngineTargetStage : MeasureEngineStage
    {
        internal const int MinIterationCount = 15;
        internal const int MaxIterationCount = 100;
        internal const int MaxIdleIterationCount = 20;
        private const double MaxIdleStdErrRelative = 0.05;

        public MeasureEngineTargetStage(IMeasureEngine engine) : base(engine)
        {
        }

        public List<Measurement> Run(long invokeCount, IterationMode iterationMode, Count iterationCount)
        {
            return iterationCount.IsAuto
                ? RunAuto(invokeCount, iterationMode)
                : RunSpecific(invokeCount, iterationMode, iterationCount);
        }

        public List<Measurement> RunIdle(long invokeCount) => Run(invokeCount, IterationMode.IdleTarget, Count.Auto);
        public List<Measurement> RunMain(long invokeCount) => Run(invokeCount, IterationMode.MainTarget, TargetJob.TargetCount);

        private List<Measurement> RunAuto(long invokeCount, IterationMode iterationMode)
        {
            var measurements = new List<Measurement>();
            int iterationCounter = 0;
            bool isIdle = iterationMode.IsIdle();
            double maxErrorRelative = isIdle ? MaxIdleStdErrRelative : TargetAccuracy.MaxStdErrRelative;
            while (true)
            {
                iterationCounter++;
                var measurement = RunIteration(iterationMode, iterationCounter, invokeCount);
                measurements.Add(measurement);

                var statistics = new Statistics(measurements.Select(m => m.Nanoseconds));
                if (TargetAccuracy.RemoveOutliers)
                    statistics = new Statistics(statistics.WithoutOutliers());
                double actualError = statistics.StandardError;
                double maxError = maxErrorRelative * statistics.Mean;

                if (iterationCounter >= MinIterationCount && actualError < maxError)
                    break;

                if (iterationCounter >= MaxIterationCount || (isIdle && iterationCounter >= MaxIdleIterationCount))
                    break;
            }
            WriteLine();
            return measurements;
        }

        private List<Measurement> RunSpecific(long invokeCount, IterationMode iterationMode, Count iterationCount)
        {
            var measurements = new List<Measurement>();
            for (int i = 0; i < iterationCount; i++)
                measurements.Add(RunIteration(iterationMode, i + 1, invokeCount));
            WriteLine();
            return measurements;
        }
    }
}