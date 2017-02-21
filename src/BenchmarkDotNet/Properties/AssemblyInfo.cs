﻿using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Properties;

[assembly: AssemblyTitle(BenchmarkDotNetInfo.Title)]
[assembly: AssemblyProduct(BenchmarkDotNetInfo.Title)]
[assembly: AssemblyDescription(BenchmarkDotNetInfo.Description)]
[assembly: AssemblyCopyright(BenchmarkDotNetInfo.Copyright)]
[assembly: AssemblyVersion(BenchmarkDotNetInfo.Version)]
[assembly: AssemblyFileVersion(BenchmarkDotNetInfo.Version)]

[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]
[assembly: Guid("cbba82d3-e650-407f-a0f0-767891d4f04c")]

[assembly: CLSCompliant(true)]

#if RELEASE
[assembly: InternalsVisibleTo("BenchmarkDotNet.Tests,PublicKey=" + BenchmarkDotNetInfo.PublicKey)]
[assembly: InternalsVisibleTo("BenchmarkDotNet.IntegrationTests,PublicKey=" + BenchmarkDotNetInfo.PublicKey)]
[assembly: InternalsVisibleTo("BenchmarkDotNet.Samples,PublicKey=" + BenchmarkDotNetInfo.PublicKey)]
#else
[assembly: InternalsVisibleTo("BenchmarkDotNet.Tests")]
[assembly: InternalsVisibleTo("BenchmarkDotNet.IntegrationTests")]
[assembly: InternalsVisibleTo("BenchmarkDotNet.Samples")]
#endif