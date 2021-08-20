using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FastEnumToStringGeneratorTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual(CustomEnum1.MyFifthValue.FastToString(), CustomEnum1.MyFifthValue.ToString());
        }
    }
}
