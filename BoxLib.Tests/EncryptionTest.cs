using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxLib.Tests
{
	[TestClass]
	public class EncryptionTest
	{
		[TestMethod]
		public void TestSecureKeys()
		{
			const string secret = "This is a secret message!";
			byte[] key1 = Static.AESThenHMAC.NewKey();
			byte[] key2 = Static.AESThenHMAC.NewKey();

			string encrypted = Static.AESThenHMAC.SimpleEncrypt(secret, key1, key2);
			string decrypted = Static.AESThenHMAC.SimpleDecrypt(encrypted, key1, key2);

			Assert.AreEqual(secret, decrypted);
		}

		[TestMethod]
		public void TestPassword()
		{
			const string secret = "This is a secret message!";
			string password = "ThisIsMyPassword";

			string encrypted = Static.AESThenHMAC.SimpleEncryptWithPassword(secret, password);
			string decrypted = Static.AESThenHMAC.SimpleDecryptWithPassword(encrypted, password);

			Assert.AreEqual(secret, decrypted);
		}
	}
}
