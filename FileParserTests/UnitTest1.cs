using System;
using FileParser.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileParserTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestParse()
        {
            for (int i = 0; i < 5; i++)
            {
                var vm = new MainWindowVM();
                vm.ParseFilesForTestAsync();
            }
        }
    }
}
