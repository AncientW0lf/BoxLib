using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace BoxLib.Static
{
	public static class Serializer
	{
		/// <summary>
		/// Deserializes a string into the specified type.
		/// </summary>
		/// <typeparam name="T">The type to deserialize the input into.</typeparam>
		/// <param name="input">The string to be deserialized.</param>
		/// <param name="root">The root element of the serialized data.</param>
		/// <returns>The deserialized object.</returns>
		public static T Deserialize<T>(string input, string root = null)
		{
			var ser = new XmlSerializer(typeof(T), !string.IsNullOrWhiteSpace(root) ? new XmlRootAttribute(root) : null);

			using (var sr = new StringReader(input))
			{
				return (T)ser.Deserialize(sr);
			}
		}

		/// <summary>
		/// Serializes an object with the specified type into a string.
		/// </summary>
		/// <typeparam name="T">The type that will be serialized.</typeparam>
		/// <param name="objectToSerialize">The object to be serialized.</param>
		/// <param name="root">The root element of the serialized data.</param>
		/// <returns>The serialized string.</returns>
		public static string Serialize<T>(T objectToSerialize, string root = null)
		{
			var xmlSerializer = new XmlSerializer(objectToSerialize.GetType(), 
				!string.IsNullOrWhiteSpace(root) ? new XmlRootAttribute(root) : null);

			using (var textWriter = new StringWriter())
			{
				xmlSerializer.Serialize(textWriter, objectToSerialize);
				return textWriter.ToString();
			}
		}

		/// <summary>
		/// Perform a deep Copy of the object.
		/// </summary>
		/// <typeparam name="T">The type of object being copied.</typeparam>
		/// <param name="source">The object instance to copy.</param>
		/// <returns>The copied object.</returns>
		public static T Clone<T>(T source) where T : class
		{
			if (!typeof(T).IsSerializable)
			{
				throw new ArgumentException("The type must be serializable.", nameof(source));
			}

			// Don't serialize a null object, simply return the default for that object
			if (Object.ReferenceEquals(source, null))
			{
				return default(T);
			}

			IFormatter formatter = new BinaryFormatter();
			Stream stream = new MemoryStream();
			using (stream)
			{
				formatter.Serialize(stream, source);
				stream.Seek(0, SeekOrigin.Begin);
				return (T)formatter.Deserialize(stream);
			}
		}
	}
}
