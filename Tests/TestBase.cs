using Microsoft.VisualStudio.TestTools.UnitTesting;
[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]

namespace PDFtoZPL.Tests
{
    public abstract class TestBase
    {
        public TestContext? TestContext { get; set; }
    }
}