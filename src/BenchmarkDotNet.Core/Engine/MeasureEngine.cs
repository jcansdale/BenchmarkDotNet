using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Horology;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Mathematics;
using BenchmarkDotNet.Reports;
using JetBrains.Annotations;

namespace BenchmarkDotNet.Engine
{
    [UsedImplicitly]
    public class MeasureEngine : IMeasureEngine
    {
        public const int MinInvokeCount = 4;
        public const int MinIterationTimeMs = 200;

        public IJob TargetJob { get; set; } = Job.Default;
        public long OperationsPerInvoke { get; set; } = 1;
        public Action SetupAction { get; set; } = null;
        public Action CleanupAction { get; set; } = null;
        public Action<long> MainAction { get; }
        public Action<long> IdleAction { get; }

        private readonly MeasureEnginePilotStage pilotStage;
        private readonly MeasureEngineWarmupStage warmupStage;
        private readonly MeasureEngineTargetStage targetStage;

        public MeasureEngine([NotNull] Action<long> idleAction, [NotNull] Action<long> mainAction)
        {
            IdleAction = idleAction;
            MainAction = mainAction;
            pilotStage = new MeasureEnginePilotStage(this);
            warmupStage = new MeasureEngineWarmupStage(this);
            targetStage = new MeasureEngineTargetStage(this);
        }

        // TODO: return all measurements
        [UsedImplicitly]
        public void Run()
        {
            Jitting();

            long invokeCount = 1;
            IList<Measurement> idle = null;

            if (TargetJob.Mode != Mode.ColdStart)
            {
                invokeCount = pilotStage.Run();

                if (TargetJob.Accuracy.EvaluateOverhead)
                {
                    warmupStage.RunIdle(invokeCount);
                    idle = targetStage.RunIdle(invokeCount);
                }

                warmupStage.RunMain(invokeCount);
            }
            var main = targetStage.RunMain(invokeCount);

            PrintResult(idle, main);
        }

        private void Jitting()
        {
            SetupAction?.Invoke();
            MainAction.Invoke(1);
            IdleAction.Invoke(1);
            CleanupAction?.Invoke();
        }

        private void PrintResult(IList<Measurement> idle, IList<Measurement> main)
        {
            // TODO: use Accuracy.RemoveOutliers
            var overhead = idle == null ? 0.0 : new Statistics(idle.Select(m => m.Nanoseconds)).Median;
            int resultIndex = 0;
            foreach (var measurement in main)
            {
                var resultMeasurement = new Measurement(
                    measurement.LaunchIndex,
                    IterationMode.Result,
                    ++resultIndex,
                    measurement.Operations,
                    Math.Max(0, measurement.Nanoseconds - overhead));
                WriteLine(resultMeasurement.ToOutputLine());
            }
            WriteLine();
        }

        public Measurement RunIteration(IterationData data)
        {
            // Initialization
            long invokeCount = data.InvokeCount;
            var totalOperations = invokeCount * OperationsPerInvoke;
            var action = data.IterationMode.IsIdle() ? IdleAction : MainAction;

            // Setup
            SetupAction?.Invoke();
            GcCollect();

            // Measure
            var clock = (TargetJob.Clock ?? Chronometer.BestClock).Start();
            action(invokeCount);
            var clockSpan = clock.Stop();

            // Cleanup
            CleanupAction?.Invoke();
            GcCollect();

            // Results
            var measurement = new Measurement(0, data.IterationMode, data.Index, totalOperations, clockSpan.GetNanoseconds());
            WriteLine(measurement.ToOutputLine());
            return measurement;
        }

        private void GcCollect() => GcCollect(TargetJob.GcMode);

        private static void GcCollect(GcMode gcMode)
        {
            if (!gcMode.Force)
                return;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        public void WriteLine() => Console.WriteLine();
        public void WriteLine(string line) => Console.WriteLine(line);
    }
}