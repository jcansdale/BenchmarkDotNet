using System.Runtime.CompilerServices;

namespace BenchmarkDotNet.Engine
{
    public class Consumer
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Consume<T>(T x)
        {
            // TODO
        }
    }
}