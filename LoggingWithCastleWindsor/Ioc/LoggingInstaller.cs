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
            public CounterSnapshotInformation(string path, Func<float, string> convert = null)
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
                new CounterSnapshotInformation(@"Memory\Available Mbytes"),
                new CounterSnapshotInformation(@"Memory\Page Reads/sec"),
                new CounterSnapshotInformation(@"Memory\Pages/sec"),
                new CounterSnapshotInformation(@"Processor\% Processor Time\_Total"),
                new CounterSnapshotInformation(@"Process\% Processor Time\" + processName),
                new CounterSnapshotInformation(@"System\Context Switches/sec"),
                new CounterSnapshotInformation(@"Process\Private Bytes\" + processName),
                new CounterSnapshotInformation(@".NET CLR Memory\% Time in GC\" + processName),
                new CounterSnapshotInformation(@".NET CLR Memory\# Bytes in all Heaps\" + processName),
                new CounterSnapshotInformation(@".NET CLR Memory\# of Pinned Objects\" + processName),
                new CounterSnapshotInformation(@".NET CLR Memory\Large Object Heap Size\" + processName),
                new CounterSnapshotInformation(@"Process\Working Set\" + processName),
                new CounterSnapshotInformation(@".NET CLR Exceptions\# of Exceps Thrown / sec\" + processName)
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
