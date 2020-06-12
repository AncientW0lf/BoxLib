using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxLib.Tests
{
	[TestClass]
	public class SerializerTest
	{
		[TestMethod]
		public void TestXmlRoot()
		{
			string serialized = Static.Serializer.Serialize(new Containers.LineF(), "ROOT");
			Assert.IsTrue(serialized.Contains("<ROOT") && serialized.Contains("</ROOT>"), serialized);
		}
	}
}
