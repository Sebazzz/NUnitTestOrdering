This tests the behavior of an SetUpFixture in combination with an ordered test and unordered tests. According to the NUnit documentation, an SetUpFixture runs in the context of the namespace. A SetUpFixture in the global namespace (no namespace) runs before and after all tests.

We don't support a SetUpFixture in a namespace since it doesn't make any sense for ordered tests, but we must support a global SetUpFixture.

Note: While unordered tests have no order, we do execute them before ordered tests.