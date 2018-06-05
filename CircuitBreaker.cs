using System;
using System.Threading;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace PollyTester
{
	public class CircuitBreaker
	{
		private static int loop = 1;

		public void TestCircuit()
		{
			var policy = StandardBreaker();
			//var policy = StandardRetry();

			for (loop = 1; loop <= 10; loop++)
			{
				try
				{
					if (loop == 6)
						Thread.Sleep(5000);

					Console.WriteLine($"Attempt: {loop}");
					policy.Execute(DoSomething);
				}
				catch (BrokenCircuitException e)
				{
					Console.WriteLine($"\tCircuit broken: {e.Message}");
				}
				catch (Exception e)
				{
					Console.WriteLine($"\t{e.Message}");
				}
			}

			Console.WriteLine($"Complete!");
			Console.ReadKey();
		}

		static void DoSomething()
		{
			if (loop > 2 && loop < 6)
				throw new ArgumentException($"Circuit exception: {loop}");
		}

		public static Policy StandardBreaker()
		{
			return Policy
				.Handle<ArgumentException>()
				.CircuitBreaker(3, TimeSpan.FromSeconds(5));
				//.CircuitBreaker(3, TimeSpan.FromSeconds(15), OnBreak, OnReset);
		}

		public static RetryPolicy StandardRetry()
		{
			return Policy.Handle<Exception>()
				.WaitAndRetry(2, r => TimeSpan.FromMilliseconds(250), (exception, timeSpan, context) => RetryOccurred(timeSpan, exception));
		}

		private static void OnBreak(Exception arg1, TimeSpan arg2)
		{
			Console.WriteLine("Circuit broken");
		}

		public static void OnReset()
		{
			Console.WriteLine("Circuit reset");
		}

		public static void RetryOccurred(TimeSpan interval, Exception e)
		{
			Console.WriteLine($"Retry attempt, Waiting:{interval}\nException:\n{e}");
		}
	}
}
