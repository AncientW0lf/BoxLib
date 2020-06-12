using BoxLib.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxLib.Tests
{
	[TestClass]
	public class SerializerTest
	{
		[TestMethod]
		public void TestXmlRootSerialized()
		{
			string serialized = Static.Serializer.Serialize(new LineF(), "ROOT");
			Assert.IsTrue(serialized.Contains("<ROOT") && serialized.Contains("</ROOT>"), serialized);
		}

		[TestMethod]
		public void TestXmlRootDeserialized()
		{
			string serialized = Static.Serializer.Serialize(new LineF(1, 2, 3, 4), "ROOT");
			var deserialized = Static.Serializer.Deserialize<LineF>(serialized, "ROOT");
			Assert.IsTrue(deserialized.StartX.Equals(1) 
			              && deserialized.StartY.Equals(2)
			              && deserialized.EndX.Equals(3) 
			              && deserialized.EndY.Equals(4), serialized);
		}
	}
}
