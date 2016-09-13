# NUnit Ordered Testing library

This library allows you to use test ordering with NUnit. The primary use case is to allow integration tests to depend on each other. Don't use this library to order to unit tests, that's bad practice!

## Usage

Include the library into your project and include this in your AssemblyInfo file:

``` C#
[assembly:EnableTestFixtureOrdering]
```

Now you can start writing ordered tests. You can order both test fixtures and the methods within fixtures themselves.

### Test Fixture ordering
In order to set-up fixture ordering, derive a class from `TestOrderingSpecification`. Decorate your new class with the `OrderedTestFixtureAttribute`. Override and implement the `DefineTestOrdering` method.

In the `DefineTestOrdering` method you can call `TestFixture` and `OrderedTestSpecification` to register an `TestFixture` or `OrderedTestSpecification` to run in order.

``` C#
[OrderedTestFixture]
public sealed class MyOrderedTestFixture : TestOrderingSpecification {
    protected override void DefineTestOrdering() {
        TestFixture<Fixture1>();

        OrderedTestSpecification<MyOtherOrderedTestFixture>();

        TestFixture<Fixture2>();
        TestFixture<Fixture3>();
    }
}
```

Above code will run test fixture `Fixture1` first, then it will whatever fixtures in `MyOtherOrderedTestFixture` and then `Fixture2` and `Fixture3`.

Please also view the samples [in the tests](src/NUnitTestOrdering.Tests/TestData) of various use cases showing how to use this library.

### Test Method ordering
Similar to NUnit `OrderAttribute` you can use attributes to explicitly define the dependencies of a test method. Apply a `TestMethodDependencyAttribute` to your test methods which require another test method to run before. Apply a `TestMethodWithoutDependencyAttribute` to test methods which don't have dependencies and may run first.

``` C#
[TestFixture]
public sealed class Test {
    [Test]
    [TestMethodWithoutDependency]
    public void One() {
        this.Log();
    }

    [Test]
    [TestMethodDependency(nameof(Three))]
    public void Four() {
        this.Log();
    }

    [Test]
    [TestMethodDependency(nameof(One))]
    public void Two() {
        this.Log();
    }

    [Test]
    [TestMethodDependency(nameof(Two))]
    public void Three() {
        this.Log();
    }
}
```

The main advantage of this is that you can explicitly name the dependency of your test method instead of using an opaque index number.

## Development
### Building
To build and run the tests in the library use:

```
build
```

Or:

```
build -Target Test
```

To find other targets to run, please run:

```
build /?
```

### Testing
Most of the tests written for the library are "full blown" integration tests. They work by dynamically compiling an assembly and then running it and capturing the output. 

#### Developing integration tests
To develop an integration test, you need to create an directory [in the TestData directory](src/NUnitTestOrdering.Tests/TestData). Creating a directory will yield an TestFixture, and each subdirectory will yield an individual test.

An test consists at least of one or more C# files and `ExpectedTestResult.txt` text file, as an embedded resource, which contains the expected logged test output. Once you have created your test files, regenerate the `TestDataIndex.tt` file. You are now ready to run your test. In case of trouble, add an `StartDebuggerAttribute` to the test method to start the NUnit application under the debugger. 

## Contributions
This project is accepting contributions. Please keep the following guidelines in mind:

- Add tests for new code added
- Keep in line with the existing code style
- Don't reformat existing code
- Propose new features before creating pull requests to prevent disappointment