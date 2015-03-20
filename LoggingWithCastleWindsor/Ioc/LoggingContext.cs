using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace LoggingWithCastleWindsor.Ioc
{
    internal class LoggingContext
    {
        private readonly Action<string, object> _addToContext;
        private readonly Assembly _assembly;

        public LoggingContext(Action<string, object> addToContext, Assembly assembly)
        {
            _addToContext = addToContext;
            _assembly = assembly;
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

        public void AddGlobalProperties()
        {
            AddPerformanceCounters();

            AddAssemblyAttributes();
        }

        private void AddPerformanceCounters()
        {
            var processName = Process.GetCurrentProcess().ProcessName;

            // These could be read from a configuration file ...
            var counters = new List<CounterSnapshotInformation>
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

            counters.ForEach(AddGlobalProperty);
        }

        private void AddGlobalProperty(CounterSnapshotInformation info)
        {
            var performanceCounterSnapshot = new PerformanceCounterSnapshot(info.Path, info.Convert);

            var key = info.KeyFromPath();

            _addToContext(key, performanceCounterSnapshot);
        }

        private void AddAssemblyAttributes()
        {
            var attributes = _assembly.GetCustomAttributes(false);
            var version = _assembly.GetName().Version;

            var values = new List<Tuple<string, object>>
            {
                new Tuple<string, object>("Company Name", MatchAttribute<AssemblyCompanyAttribute>(attributes, attribute => attribute.Company)),
                new Tuple<string, object>("Product Name", MatchAttribute<AssemblyProductAttribute>(attributes, attribute => attribute.Product)),
                new Tuple<string, object>("Product Version", version)
            };

            values.ForEach(tuple => _addToContext(tuple.Item1, tuple.Item2));
        }

        private static string MatchAttribute<TAttributeType>(
            IEnumerable<object> attributes, Func<TAttributeType, string> transformer
            ) where TAttributeType : Attribute
        {
            var match = (TAttributeType)(from attribute in attributes
                                         where attribute is TAttributeType
                                         select attribute).FirstOrDefault();
            return match == null ? "" : transformer(match);
        }
    }
}