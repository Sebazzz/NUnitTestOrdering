This tests the behavior of the same test fixture specified multiple times in a test suite.

By default, NUnit will run all fixtures in the assembly. To simply run one of the fixtures, call NUnit like this:

    nunit3-console.exe <assembly> --where "test==Ordered.RootOrderedTestFixture.OrderedTestFixture1"