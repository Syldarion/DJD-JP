using System;
using System.Threading;
using System.Collections.Generic;

#if NETFX_CORE
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

namespace BeardedManStudios.Threading
{
	public class Task
	{
		private static List<Task> tasks = new List<Task>();
		private static object taskMutex = new Object();

#if !NETFX_CORE
		public Thread TrackedThread { get; private set; }
#endif

		private Task() { }

		private void SetExpression(Action expression)
		{
#if !NETFX_CORE
			TrackedThread = new Thread(new ThreadStart(expression));
#endif
		}

		public void Kill()
		{
			lock (taskMutex)
			{
#if !NETFX_CORE
				TrackedThread.Abort();
#endif
				tasks.Remove(this);
			}
		}

		public void Wait()
		{
#if !NETFX_CORE
			while (TrackedThread.IsAlive) { }
#endif
		}

		public static void KillAll()
		{
			for (int i = tasks.Count - 1; i >= 0; --i)
				tasks[i].Kill();
		}
		
#if NETFX_CORE
		public static System.Threading.Tasks.Task Run(Action expression)
#else
		public static Task Run(Action expression)
#endif
		{
			Task task = new Task();

			Action inline = () =>
			{
				expression();

				lock (taskMutex)
				{
					tasks.Remove(task);
				}
			};

			task.SetExpression(inline);

#if NETFX_CORE
			return System.Threading.Tasks.Task.Run(async () =>
			{
				inline();
			});
#else

			task.TrackedThread.Start();

			lock (taskMutex)
			{
				tasks.Add(task);
			}

			return task;
#endif
		}
	}
}