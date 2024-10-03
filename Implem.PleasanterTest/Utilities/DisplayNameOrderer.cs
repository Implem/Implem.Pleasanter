using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using System.Linq;

[assembly: TestCollectionOrderer(
    ordererTypeName: "Implem.PleasanterTest.Utilities.DisplayNameOrderer",
    ordererAssemblyName: "Implem.PleasanterTest")]

namespace Implem.PleasanterTest.Utilities;


public class DisplayNameOrderer : ITestCollectionOrderer
{
    public IEnumerable<ITestCollection> OrderTestCollections(
        IEnumerable<ITestCollection> testCollections) =>
        testCollections.OrderBy(collection => collection.DisplayName);
}