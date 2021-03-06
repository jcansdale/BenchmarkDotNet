﻿using System;
using BenchmarkDotNet.Characteristics;
using BenchmarkDotNet.Horology;

namespace BenchmarkDotNet.Engines
{
    internal class EnginePilotStage : EngineStage
    {
        internal const long MaxInvokeCount = (long.MaxValue / 2 + 1) / 2;

        public EnginePilotStage(IEngine engine) : base(engine)
        {
        }

        /// <returns>Perfect invocation count</returns>
        public long Run()
        {
            // If InvocationCount is specified, pilot stage should be skipped
            if (!TargetJob.Run.InvocationCount.IsDefault)
                return TargetJob.Run.InvocationCount.SpecifiedValue;

            // Here we want to guess "perfect" amount of invocation
            return TargetJob.Run.IterationTime.IsDefault ? RunAuto() : RunSpecific();
        }

        /// <summary>
        /// A case where we don't have specific iteration time.
        /// </summary>
        private long RunAuto()
        {
            long invokeCount = TargetAccuracy.MinInvokeCount.Resolve(Resolver);
            double maxError = TargetAccuracy.MaxStdErrRelative.Resolve(Resolver); // TODO: introduce a StdErr factor
            double minIterationTome = TimeUnit.Convert(Engine.MinIterationTimeMs, TimeUnit.Millisecond, TimeUnit.Nanosecond);

            double resolution = TargetClock.GetResolution().Nanoseconds;

            int iterationCounter = 0;
            while (true)
            {
                iterationCounter++;
                var measurement = RunIteration(IterationMode.Pilot, iterationCounter, invokeCount);
                double iterationTime = measurement.Nanoseconds;                
                double operationError = 2.0 * resolution / invokeCount; // An operation error which has arisen due to the Chronometer precision
                double operationMaxError = iterationTime / invokeCount * maxError; // Max acceptable operation error

                bool isFinished = operationError < operationMaxError && iterationTime >= minIterationTome;
                if (isFinished)
                    break;
                if (invokeCount >= MaxInvokeCount)
                    break;

                invokeCount *= 2;
            }
            WriteLine();

            return invokeCount;
        }

        /// <summary>
        /// A case where we have specific iteration time.
        /// </summary>
        private long RunSpecific()
        {
            long invokeCount = Engine.MinInvokeCount;
            double targetIterationTime = TargetJob.Run.IterationTime.Resolve(Resolver).ToNanoseconds();
            int iterationCounter = 0;

            int downCount = 0; // Amount of iterations where newInvokeCount < invokeCount
            while (true)
            {
                iterationCounter++;
                var measurement = RunIteration(IterationMode.Pilot, iterationCounter, invokeCount);
                double actualIterationTime = measurement.Nanoseconds;
                long newInvokeCount = Math.Max(TargetAccuracy.MinInvokeCount.Resolve(Resolver), (long) Math.Round(invokeCount * targetIterationTime / actualIterationTime));

                if (newInvokeCount < invokeCount)
                    downCount++;

                if (Math.Abs(newInvokeCount - invokeCount) <= 1 || downCount >= 3)
                    break;

                invokeCount = newInvokeCount;
            }
            WriteLine();

            return invokeCount;
        }
    }
}