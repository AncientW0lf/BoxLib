namespace BoxLib.Containers
{
	/// <summary>
	/// A simple container class that contains another object <see cref="T"/> as a reference.
	/// </summary>
	/// <typeparam name="T">The type of the object to reference.</typeparam>
	public class Ref<T>
	{
		/// <summary>
		/// The underlying object of this class.
		/// </summary>
		public T Value { get; set; }

		/// <summary>
		/// Ctor. Sets the value.
		/// </summary>
		/// <param name="value">The object to reference.</param>
		public Ref(ref T value)
		{
			Value = value;
		}
	}
}
