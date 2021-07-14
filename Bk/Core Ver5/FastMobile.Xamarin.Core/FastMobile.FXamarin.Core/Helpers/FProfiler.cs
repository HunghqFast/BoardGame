#if DEBUG

using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace FastMobile.FXamarin.Core
{
    public static class FProfiler
    {
        private static readonly ConcurrentDictionary<string, Stopwatch> watches = new ConcurrentDictionary<string, Stopwatch>();

        public static void Start(object view)
        {
            Start(view.GetType().Name);
        }

        public static string Stop(object view)
        {
            return Stop(view.GetType().Name);
        }

        public static void Start(string tag)
        {
            Console.WriteLine("START {0}", tag);
            var watch = watches[tag] = new Stopwatch();
            watch.Start();
        }

        public static string Stop(string tag)
        {
            if (watches.TryGetValue(tag, out Stopwatch watch))
            {
                Console.WriteLine("STOP {0} TOOK {1}", tag, watch.Elapsed);
                return $"STOP {tag} TOOK {watch.Elapsed}";
            }
            return "";
        }
    }
}

#endif