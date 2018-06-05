using System;
using Polly;

namespace PollyTester
{
	public class Retries
	{
		public bool TestRetries1()
		{
			// handle ALL exceptions
			var p = Policy
				.Handle<Exception>()
				.Retry(2, (exception, retryCount, context) =>
				{
					Console.WriteLine($"Exception: {exception.Message}");
				});

			short attempt = 0;
			try
			{
				return p.Execute(() => GenerateError(++attempt, "this is silly"));
			}
			catch (Exception e)
			{
				Console.WriteLine($"Attempt failed\nException: {e.Message}");
			}

			return false;
		}

		public bool TestRetries2()
		{
			// retries an operation twice on a specific exception (3 total tries)
			var p = Policy
				.Handle<Exception>(e => e.Message == "this is silly")
				.WaitAndRetry(2, r => TimeSpan.FromMilliseconds(500), (exception, timeSpan, context) => LogRetry(timeSpan, exception));

			short attempt = 0;
			try
			{
				return p.Execute(() => GenerateError(++attempt, "this is silly"));
			}
			catch (Exception e)
			{
				Console.WriteLine($"Cache availability check failed, error:\nException: {e.Message}");
			}

			return false;
		}

		public static void LogRetry(TimeSpan interval, Exception e)
		{
			Console.WriteLine($"Span: {interval}\nException: {e.Message}");
		}

		public static bool GenerateError(short attempt, string somedata)
		{
			Console.WriteLine($"Attempt #{attempt}");
			throw new MyException(somedata);
		}

		public class MyException : ArgumentException
		{
			public MyException(string message) : base(message) { }

			public int Value { get; set; }
		}
	}
}
