using NUnit.Core.Extensibility;
using FsCheck.NUnit.Addin;

namespace PropertyTests
{
    [NUnitAddin(Description = "FsCheck addin")]
    public class FsCheckAddin : IAddin
    {
        public bool Install(IExtensionHost host)
        {
            var tcBuilder = new FsCheckTestCaseBuider();
            host.GetExtensionPoint("TestCaseBuilders").Install(tcBuilder);
            return true;
        }
    }
}
