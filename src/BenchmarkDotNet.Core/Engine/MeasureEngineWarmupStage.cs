using System;
using System.Collections.Generic;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;

namespace BenchmarkDotNet.Engine
{
    internal class MeasureEngineWarmupStage : MeasureEngineStage
    {
        internal const int MinIterationCount = 6;
        internal const int MaxIterationCount = 50;
        internal const int MaxIdleItertaionCount = 10;

        public MeasureEngineWarmupStage(IMeasureEngine engine) : base(engine)
        {
        }

        public List<Measurement> Run(long invokeCount, IterationMode iterationMode, Count iterationCount)
        {
            return iterationCount.IsAuto
                ? RunAuto(invokeCount, iterationMode)
                : RunSpecific(invokeCount, iterationMode, iterationCount);
        }

        public void RunIdle(long invokeCount) => Run(invokeCount, IterationMode.IdleWarmup, Count.Auto);
        public void RunMain(long invokeCount) => Run(invokeCount, IterationMode.MainWarmup, TargetJob.WarmupCount);

        private List<Measurement> RunAuto(long invokeCount, IterationMode iterationMode)
        {
            int iterationCounter = 0;
            var measurements = new List<Measurement>(MaxIterationCount);
            while (true)
            {
                iterationCounter++;
                measurements.Add(RunIteration(iterationMode, iterationCounter, invokeCount));
                if (IsWarmupFinished(measurements, iterationMode))
                    break;
            }
            WriteLine();
            return measurements;
        }

        private List<Measurement> RunSpecific(long invokeCount, IterationMode iterationMode, Count iterationCount)
        {
            var measurements = new List<Measurement>(iterationCount);
            for (int i = 0; i < iterationCount; i++)
                measurements.Add(RunIteration(iterationMode, i + 1, invokeCount));
            WriteLine();
            return measurements;
        }

        private static bool IsWarmupFinished(List<Measurement> measurements, IterationMode iterationMode)
        {
            int n = measurements.Count;
            if (n >= MaxIterationCount || (iterationMode.IsIdle() && n >= MaxIdleItertaionCount))
                return true;
            if (n < MinIterationCount)
                return false;                        

            int dir = -1, changeCount = 0;
            for (int i = 1; i < n; i++)
            {
                int nextDir = Math.Sign(measurements[i].Nanoseconds - measurements[i - 1].Nanoseconds);
                if (nextDir != dir || nextDir == 0)
                {
                    dir = nextDir;
                    changeCount++;
                }
            }

            return changeCount >= 4;
        }
    }
}