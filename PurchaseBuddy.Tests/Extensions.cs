using System.Diagnostics;

namespace PurchaseBuddy.Tests;
internal static class Extensions
{
	internal static void RecordElapsedTime(string name, Action value)
	{
		var sw = new Stopwatch();
		sw.Start();
		value();
		sw.Stop();
		Console.WriteLine($"[{name}] Processed. Total execution time: {sw.ElapsedMilliseconds}ms");
	}
}
