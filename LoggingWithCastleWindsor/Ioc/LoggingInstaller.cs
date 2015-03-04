using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace LoggingWithCastleWindsor.Ioc
{
    class LoggingInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            AddGlobalProperties();

            container.AddLoggingFacility("log4net.config");

            container.Install(
                new InterceptorInstaller<LoggingInterceptor>(),
                new InterceptorInstaller<ExceptionInterceptor>(),
                new InterceptorInstaller<TimingInterceptor>()
                );
        }

        class CounterSnapshotInformation
        {
            public CounterSnapshotInformation(string path, Func<float, string> convert)
            {
                Path = path;
                Convert = convert;
            }
            public string Path { get; private set; }
            public Func<float, string> Convert { get; private set; }
            public string KeyFromPath()
            {
                var parts = Path.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                return string.Join(":", parts.Take(2));
            }
        }

        private static void AddGlobalProperties()
        {
            var processName = Process.GetCurrentProcess().ProcessName;

            var paths = new List<CounterSnapshotInformation>
            {
                new CounterSnapshotInformation(@"Memory\Available Mbytes", value => value.AsGbytes("0.0", 1024f)),
                new CounterSnapshotInformation(@"Processor\% Processor Time\_Total", value => value.AsFormattedValue("0.00")),
                new CounterSnapshotInformation(@"Process\% Processor Time\" + processName, value => value.AsProcessTime("0.00")),
                new CounterSnapshotInformation(@"System\Context Switches/sec", value => value.AsFormattedValue("##,#")),
                new CounterSnapshotInformation(@"Process\Private Bytes\" + processName, value => value.AsMbytes("0.0")),
                new CounterSnapshotInformation(@".NET CLR Memory\# Bytes in all Heaps\" + processName, value => value.AsFormattedValue("##,#"))
            };

            paths.ForEach(AddGlobalProperty);
        }

        private static void AddGlobalProperty(CounterSnapshotInformation info)
        {
            var path = info.Path;
            var convert = info.Convert;
            var key = info.KeyFromPath();
            
            log4net.GlobalContext.Properties[key] = new PerformanceCounterSnapshot(path, convert);
        }
    }
}
