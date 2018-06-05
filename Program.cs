using System;
using Polly;

namespace PollyTester
{
	class Program
	{
		private static void Main(string[] args)
		{
			//var rt = new Retries();
			//var result = rt.TestRetries1();
			//Console.WriteLine($"Result: {result}");

			var cb = new CircuitBreaker();
			cb.TestCircuit();
		}

	}
}
