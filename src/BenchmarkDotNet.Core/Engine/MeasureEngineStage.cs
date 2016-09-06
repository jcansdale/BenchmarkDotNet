using BenchmarkDotNet.Horology;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;

namespace BenchmarkDotNet.Engine
{
    public class MeasureEngineStage
    {
        private readonly IMeasureEngine engine;

        protected MeasureEngineStage(IMeasureEngine engine)
        {
            this.engine = engine;
        }

        protected IJob TargetJob => engine.TargetJob;
        protected Accuracy TargetAccuracy => TargetJob.Accuracy;
        protected IClock TargetClock => (TargetJob.Clock ?? Chronometer.BestClock);

        protected Measurement RunIteration(IterationMode mode, int index, long invokeCount)
        {
            return engine.RunIteration(new IterationData(mode, index, invokeCount));
        }

        protected void WriteLine() => engine.WriteLine();

        protected void WriteLine(string line) => engine.WriteLine(line);
    }
}