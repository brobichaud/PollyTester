using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Polly;

namespace ConsoleApplication1
{
	class Program
	{
		private static void Main(string[] args)
		{
			var x = TestPolly();
			Console.WriteLine("Result: {0}", x);
		}

		private static bool TestPolly()
		{
			//var p = Policy
			//	.Handle<Exception>()
			//	.Retry(2, (exception, retryCount, context) =>
			//	{
			//		Console.WriteLine("Count: {0}, Exception: {1}", retryCount, exception);
			//	});

			var p = Policy
				.Handle<Exception>(e => e.Message == "this is silly")
				.WaitAndRetry(2, r => TimeSpan.FromMilliseconds(500), (exception, timeSpan, context) => LogRetry(timeSpan, exception));

			try
			{
				return p.Execute(() => GenerateError("this is silly"));
			}
			catch (Exception e)
			{
				Console.WriteLine("Cache availability check failed, error:\n" + e);
			}

			return false;
		}

		public static void LogRetry(TimeSpan interval, Exception e)
		{
			Console.WriteLine("Span: {0}, Exception: {1}", interval, e);
		}

		public static bool GenerateError(string somedata)
		{
			throw new MyException(somedata);
			return true;
		}

		public class MyException : ArgumentException
		{
			public MyException(string message)
				: base(message)
			{
				
			}

			public int Value { get; set; }
		}
	}
}
