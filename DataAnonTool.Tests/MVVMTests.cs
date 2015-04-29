using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Input;
using AnonTool.MVVM.Commands;
using AnonTool.MVVM.Updates;
using System.Threading;

namespace DataAnonTool.Tests
{
    [TestClass]
    public class MVVMTests : UpdateBase
    {
        //RelayCommandTest Items
        private bool _invokeMethod = false;
        private ICommand _testCommand;
        public ICommand TestCommand
        {
            get { return _testCommand ?? (_testCommand = new RelayCommand(o => TestMethod(), o => true)); }
        }
        
        private void TestMethod()
        {
            _invokeMethod = true;
        }

        [TestMethod]
        public void RelayCommandTest()
        {
            Assert.IsFalse(_invokeMethod);
            TestCommand.Execute(null);
            Assert.IsTrue(_invokeMethod);
        }

    }
}
