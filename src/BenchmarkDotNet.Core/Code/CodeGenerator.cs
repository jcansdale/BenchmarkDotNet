using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Extensions;
using BenchmarkDotNet.Helpers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace BenchmarkDotNet.Code
{
    internal static class CodeGenerator
    {
        internal static string Generate(Benchmark benchmark)
        {
            var provider = GetDeclarationsProvider(benchmark.Target);

            return new SmartStringBuilder(ResourceHelper.LoadTemplate("BenchmarkProgram.txt")).
                Replace("$OperationsPerInvoke$", provider.OperationsPerInvoke).
                Replace("$TargetTypeNamespace$", provider.TargetTypeNamespace).
                Replace("$TargetMethodReturnTypeNamespace$", provider.TargetMethodReturnTypeNamespace).
                Replace("$TargetTypeName$", provider.TargetTypeName).
                Replace("$TargetMethodDelegate$", provider.TargetMethodDelegate).
                Replace("$TargetMethodDelegateType$", provider.TargetMethodDelegateType).
                Replace("$IdleMethodDelegateType$", provider.IdleMethodDelegateType).
                Replace("$IdleMethodReturnType$", provider.IdleMethodReturnType).
                Replace("$SetupMethodName$", provider.SetupMethodName).
                Replace("$CleanupMethodName$", provider.CleanupMethodName).
                Replace("$IdleImplementation$", provider.IdleImplementation).
                Replace("$HasReturnValue$", provider.HasReturnValue).
                Replace("$AdditionalLogic$", benchmark.Target.AdditionalLogic).
                Replace("$TargetBenchmarkTaskArguments$", benchmark.Job.GenerateWithDefinitions()).
                Replace("$ParamsContent$", GetParamsContent(benchmark)).
                ToString();
        }

        private static DeclarationsProvider GetDeclarationsProvider(Target target)
        {
            var method = target.Method;

            if (method.ReturnType == typeof(Task))
            {
                return new TaskDeclarationsProvider(target);
            }
            if (method.ReturnType.GetTypeInfo().IsGenericType 
                && method.ReturnType.GetTypeInfo().GetGenericTypeDefinition() == typeof(Task<>))
            {
                return new GenericTaskDeclarationsProvider(target, typeof(TaskMethodInvoker<>));
            }
            if (method.ReturnType.GetTypeInfo().IsGenericType 
                && method.ReturnType.GetTypeInfo().GetGenericTypeDefinition() == typeof(ValueTask<>))
            {
                return new GenericTaskDeclarationsProvider(target, typeof(ValueTaskMethodInvoker<>));
            }

            if (method.ReturnType == typeof(void))
            {
                var isUsingAsyncKeyword = method.HasAttribute<AsyncStateMachineAttribute>();
                if (isUsingAsyncKeyword)
                {
                    throw new NotSupportedException("async void is not supported by design");
                }

                return new VoidDeclarationsProvider(target);
            }
            return new NonVoidDeclarationsProvider(target);
        }

        private static string GetParamsContent(Benchmark benchmark)
        {
            return string.Join(
                string.Empty,
                benchmark.Parameters.Items.Select(
                    parameter =>
                        $"{(parameter.IsStatic ? "" : "instance.")}{parameter.Name} = {GetParameterValue(parameter.Value)};"));
        }

        private static string GetParameterValue(object value)
        {
            if (value is bool)
                return ((bool)value).ToLowerCase();
            if (value is string)
                return $"\"{value}\"";
            if (value is char)
                return $"'{value}'";
            if (value is float)
                return ((float)value).ToString("G", CultureInfo.InvariantCulture) + "f";
            if (value is double)
                return ((double)value).ToString("G", CultureInfo.InvariantCulture) + "d";
            if (value is decimal)
                return ((decimal)value).ToString("G", CultureInfo.InvariantCulture) + "m";
            if (value.GetType().GetTypeInfo().IsEnum)
                return value.GetType().GetCorrectTypeName() + "." + value;
            if (value is Type)
                return "typeof(" + ((Type)value).GetCorrectTypeName() + ")";
            return value.ToString();
        }

        private class SmartStringBuilder
        {
            private readonly StringBuilder builder;

            public SmartStringBuilder(string text)
            {
                builder = new StringBuilder(text);
            }

            public SmartStringBuilder Replace(string oldValue, string newValue)
            {
                if (builder.ToString().Contains(oldValue))
                    builder.Replace(oldValue, newValue);
                else
                    builder.Append($"\n// '{oldValue}' not found");
                return this;
            }

            public override string ToString() => builder.ToString();
        }
    }
}