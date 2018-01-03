# NUnit Ordered Testing library

This library allows you to use test ordering with NUnit. This is an implementation of ordered tests for NUnit, similar in how [they exist in the MSTest framework](https://msdn.microsoft.com/en-us/library/ms182631.aspx).

[![Build status](https://ci.appveyor.com/api/projects/status/1ugsdmnkcpw5krh4?svg=true)](https://ci.appveyor.com/project/Sebazzz/nunittestordering)
[![NuGet Version and Downloads count](https://buildstats.info/nuget/NUnitTestOrdering?includePreReleases=true)](https://www.nuget.org/packages/nunittestordering)

## Why?

The primary use case of this library is to allow integration tests to depend on each other. You may want to test a certain work flow in the application, but each step of the workflow is a test of its own. When testing a web shop application, you may want to test these steps:

- Manage products
  - Add product in web shop
  - Find the product in the overview
  - Add stock for the product
- Buy product
  - Find product using search
  - Add to basket
  - Buy product
- Manage shipping
  - ... 

Using this library you can model and manage workflow tests such as the above. 

**In essence this library is a replacement and improvement to MSTest Ordered Tests implemented for NUnit.**

**Note:** Don't use this library to order to unit tests, that's bad practice. You want your unit tests to be as independent as possible

## Features
The library offers the following features:

- Build complex test ordering hierarchies.
- Skip subsequent tests if an earlier test fails.
- Order your test methods by dependency instead of integer order.
- Supports usage side-by-side with unordered tests. Unordered tests are executed first.

Please also view the [known issues](#known-issues) below.

## Download
Download the current prerelease from [NuGet](https://www.nuget.org/packages/NUnitTestOrdering/):

    Install-Package NUnitTestOrdering -Pre
    
Or [download and compile](#building) build the binaries yourself.

## Usage

Include the library into your project and include this in your AssemblyInfo file:

``` C#
[assembly:EnableTestFixtureOrdering]
```

Now you can start writing ordered tests. You can order both test fixtures and the methods within fixtures themselves.

**Note:** If you use the NuGet package this has already been arranged for you!

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

		// You can specify a fixture twice
        TestFixture<Fixture3>();

        // You can only run a specific set of methods in a specific order
        TestFixture<Fixture2>(config => 
            config.TestMethod(t => t.MyFirstTest())
                  .TestMethod(nameof(Fixture2.MySecondTest)) // Alternate syntax
                  .TestMethod(t => t.MyThirdTest()));
    }

    protected override bool ContinueOnError => false; // Or true, if you want to continue even if a child test fails
}
```

Above code will run test fixture `Fixture1` first, then it will whatever fixtures in `MyOtherOrderedTestFixture` and then `Fixture2` and `Fixture3`.

You may also add methods decorated with `OneTimeSetupAttribute` and `OneTimeTearDownAttribute` to allow code to run before the fixture executes and after the fixture executes.

Please also view the samples [in the tests](src/NUnitTestOrdering.Tests/TestData) of various use cases showing how to use this library.

#### Using the OrderedTestFixture attribute
When you use the `OrderedTestFixtureAttribute` on a `TestOrderingSpecification`, it will cause the specified test ordering specification to become available "in the root" of the ordered test suite. 

In this way you can multiple suites of ordered tests to run. You can reference the same test fixture in multiple hierarchies of ordered tests. You can also refer the same test ordering specification in multiple hierarchies, as long as you don't cause a cyclic reference.

Note that different multiple suites of ordered tests are meant to be used for different use cases. If you need some test to run before another test, just because the test is setting things up you should probably use NUnit built-in action attributes.

### Test Method ordering
There are two ways you can use to order test methods within a test fixture.

#### By specifying dependencies
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

#### By using an orderer class
Especially useful for auto-generated class fixtures, for example by the great [SpecFlow](http://www.specflow.org/) framework, you can specify an "test orderer" class to apply to your test. Simply derive a class from `TestMethodOrderer` and register it using the `TestMethodOrdererAttribute` to your class. 

Example below:

``` C#
[TestFixture]
[TestMethodOrderer(typeof(Orderer))]
partial class MySpecFlowFeature {
    private sealed class Orderer : TestOrderer<Tests> {
        protected override void DefineOrdering() {
            TestMethod(nameof(TheFirstTest));

            TestMethod(x => x.ShouldExecuteSecond()); // Alternate syntax

            TestMethod(nameof(LastOne));
        }
    }
}
```

Note you shouldn't mix using "orderer classess" and specifying dependencies within the same test fixture!

### Using SetUpFixture
NUnit SetUpFixture allows you to run code before and after all tests (if a SetUpFixture is defined in the global namespace), or run code before or after tests within the same namespace.

The latter is not supported, but you can use a global SetUpFixture. Apply the OrderedTestGlobalSetUpFixtureAttribute to the fixture or your tests may end up running in the wrong order.

## Known issues
Since this library can only work with whatever NUnit allows for extensibility, there are some limitations:

- Only global SetupFixture support: No namespace based SetUpFixture support
- A single test can only be specified once in a test ordering

Related to test runners:
- Some test runners, like the ReSharper test runner use their own way of discovering tests and won't show the tests correctly. They still run correctly, though. Other test runners, like the NUnit console runner or GUI runner work perfectly.

## Development
### Building
To build and run the tests in the library use:

    build

Or:

    build -Target Test

To find other targets to run, please run:

    build /?

**Note:** Before you run the tests in Visual Studio or any other test runner, please restore NuGet packages by running the command below. This has already been done for you if you have run the tests using commands shown above.

    build -Target Restore-NuGet-Packages

### Testing
Part of the tests written for the library are "full blown" integration tests. They work by dynamically compiling an assembly and then running it under NUnit and capturing the output. The tests run for multiple NUnit version, so consistency of behavior is enforced and checked across NUnit versions. This is important because the library extends and depends on several internal NUnit constructs.

#### Developing integration tests
To develop an integration test, you need to create an directory [in the TestData directory](src/NUnitTestOrdering.Tests/TestData). Creating a directory will yield an TestFixture, and each subdirectory will yield an individual test.

An test consists at least of one or more C# files and `ExpectedTestResult.txt` text file, as an embedded resource, which contains the expected logged test output. Once you have created your test files, regenerate the `TestDataIndex.tt` file. You are now ready to run your test. In case of trouble, add an `StartDebuggerAttribute` to the test method to start the NUnit application under the debugger. 

A test is run against several NUnit versions, as stated in the [test packages file](src/NUnitTestOrdering.Tests/Support/NUnitTestVersions/packages.config).

## Contributions
This project is accepting contributions. Please keep the following guidelines in mind:

- Add tests for new code added
- Keep in line with the existing code style
- Don't reformat existing code
- Propose new features before creating pull requests to prevent disappointment
