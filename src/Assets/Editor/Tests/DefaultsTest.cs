using NUnit.Framework;
using Assert = UnityEngine.Assertions.Assert;

namespace Editor.Tests
{
    public class Tests
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            //このクラスが作られるときに一度だけ実行される。
        }
        
        [Test]
        public void ResolveTest()
        {

            Assert.AreEqual("hoge", "hoge");
        }

        [Test]
        public void ResolveAllTest()
        {
            Assert.AreEqual("hoge", "hoge");
        }
        
        [OneTimeTearDown]
        public void TearDown()
        {
            //このクラスが破棄されるときに一度だけ実行される。
        }
    }
}