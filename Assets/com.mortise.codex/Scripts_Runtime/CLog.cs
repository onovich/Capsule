using System;

namespace MortiseFrame.Capsule {

    public static class CLog {
        public static Action<string> Log = Console.WriteLine;
        public static Action<string> Warning = (msg) => Console.WriteLine($"WARNING: {msg}");
        public static Action<string> Error = (msg) => Console.Error.WriteLine($"ERROR: {msg}");
    }

}