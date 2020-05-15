using System;
using System.Timers;

namespace BoxLib.Objects
{
	/// <summary>
	/// Provides a more accurate alternative to the <see cref="System.Timers.Timer"/> by regularly checking if the time has
	/// elapsed and using <see cref="DateTime"/> objects for calculating the next time to trigger the
	/// <see cref="Elapsed"/> event.
	/// </summary>
	public class DateTimer : IDisposable
	{
		/// <summary>
		/// The underlying <see cref="System.Timers.Timer"/> used to regularly check if the <see cref="Interval"/> elapsed.
		/// </summary>
		protected Timer Timer;

		/// <summary>
		/// The main event that will be triggered once the interval has elapsed.
		/// </summary>
		public event ElapsedEventHandler Elapsed;

		/// <summary>
		/// An optional start time to start this object on for even more precision.
		/// </summary>
		public DateTime? StartTime { get; set; }

		/// <summary>
		/// The interval after which to trigger the <see cref="Elapsed"/> event. Value will be used at the next
		/// calculation, if updated.
		/// </summary>
		public TimeSpan Interval { get; set; }

		/// <summary>
		/// Stores the next time the <see cref="Elapsed"/> should be triggered.
		/// </summary>
		protected DateTime _nextElapsed;

		/// <summary>
		/// Decides whether to automatically calculate the next trigger time once the <see cref="Elapsed"/>
		/// event has been raised.
		/// </summary>
		public bool AutoReset { get; set; }

		/// <summary>
		/// Returns a <see cref="bool"/> that says whether this object is currently raising the
		/// <see cref="Elapsed"/> event or not.
		/// </summary>
		public bool Enabled { get; protected set; }

		/// <summary>
		/// Initializes a new <see cref="DateTimer"/>.
		/// </summary>
		/// <param name="intervalCheckMs">Specifies the interval in milliseconds in which to check if the timer has elapsed.</param>
		public DateTimer(short intervalCheckMs = 1000)
		{
			Timer = new Timer
			{
				Interval = intervalCheckMs,
				AutoReset = true
			};
			Timer.Elapsed += CheckElapsed;
		}

		/// <summary>
		/// Checks whether the <see cref="Interval"/> is elapsed or not.
		/// </summary>
		protected void CheckElapsed(object sender, ElapsedEventArgs e)
		{
			//Ignores any event invokes if this object has been stopped
			if(!Enabled)
				return;

			//Is the calculated trigger time in the past?
			if(_nextElapsed <= DateTime.Now)
			{
				//Raise the event
				Elapsed?.Invoke(sender, e);

				//Calculate the next trigger time
				_nextElapsed += Interval;
			}

			//Stops the timer
			if(!AutoReset)
				Stop();
		}

		/// <summary>
		/// Starts raising the <see cref="Elapsed"/> event after the interval elapses.
		/// </summary>
		public void Start()
		{
			//Sets the next trigger time to either the start time or the calculated next time based on the interval
			_nextElapsed = StartTime ?? DateTime.Now + Interval;

			//Starts the timer
			Timer?.Start();
			Enabled = true;
		}
		
		/// <summary>
		/// Stops raising the <see cref="Elapsed"/> event.
		/// </summary>
		public void Stop()
		{
			Timer.Stop();
			Enabled = false;
		}

		/// <summary>
		/// Disposes the object.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
			{
				Timer?.Stop();
				Timer?.Dispose();
			}
		}

		/// <inheritdoc />
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}