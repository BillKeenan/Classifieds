using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Classifieds.Tests
{
    [TestClass]
    public class SecurityTest
    {
        [TestMethod]
        public void TestSha1IsRepeatable()
        {
            const string plain = "password";
            const string salt = "90sa098asdfjk";

            var crypt = services.data.SHA1.GetSha(plain, salt);

            Assert.AreEqual(crypt,services.data.SHA1.GetSha(plain, salt));
        }

        [TestMethod]
        public void TestSha1IsVerifiable()
        {
            const string plain = "password";
            const string salt = "monkeys";

            var crypt = services.data.SHA1.GetSha(plain, salt);

            Assert.IsTrue(services.data.SHA1.AreEqual(plain, salt,crypt));
        }

        [TestMethod]
        public void GetSalt()
        {
            const string plain = "password";
            const string salt = "90sa098asdfjkbill@bigmojo.net";

            var crypt = services.data.SHA1.GetSha(plain, salt);

            Assert.IsTrue(services.data.SHA1.AreEqual(plain, salt, crypt));
        }
    }
}
