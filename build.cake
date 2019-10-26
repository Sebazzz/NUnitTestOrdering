//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var verbosity = Argument<Verbosity>("verbosity", Verbosity.Minimal);

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var baseName = "NUnitTestOrdering";
var buildDir = Directory("./build") + Directory(configuration);
var assemblyInfoFile = Directory($"./src/{baseName}/Properties") + File("AssemblyInfo.cs");
var dotCoverResultFile = buildDir + File("CoverageResults.dcvr");
var nuspecPath = File($"./nuget/{baseName}.nuspec");
var testResultsFile = buildDir + File("NUnitTestResults.trx");
var mainAssemblyVersion = (string) null;

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() => {
    CleanDirectory(buildDir);
	CleanDirectories("./src/**/bin");
	CleanDirectories("./src/**/obj");
});

Task("Rebuild")
	.IsDependentOn("Clean")
	.IsDependentOn("Build");

Task("Restore-NuGet-Packages")
    .Does(() => {
    NuGetRestore($"./{baseName}.sln");
    
    Information("Running NuGet restore for tests");
    
    // For tests, we require additional packages
    NuGetRestore($"./src/{baseName}.Tests/Support/NUnitTestVersions/packages.config", new NuGetRestoreSettings {
        PackagesDirectory = Directory("./packages") // Required when not restoring solution
    });
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() => {
        DotNetCoreBuild($"./{baseName}.sln");
});

Task("Test")
    .IsDependentOn("Build")
	.Description("Run all unit tests - under code coverage")
    .Does(() =>
		DotNetCoreTest($"./{baseName}.sln", new DotNetCoreTestSettings {
			ArgumentCustomization = (args) => args.AppendQuoted($"--logger:trx;LogFileName={testResultsFile}")
												  .Append("--logger:\"console;verbosity=normal;noprogress=true\"")
		}));

Task("NuGet-Test")
	.IsDependentOn("Test")
	.Description("Run all unit tests in preperation nupack");

Task("NuGet-Git-UpdateAssemblyInfo")
    .Does(() =>  {
    GitVersion(); // TODO: use this for nuspec
});

Task("NuGet-Get-Assembly-Version")
	.IsDependentOn("NuGet-Git-UpdateAssemblyInfo")
	.Does(() => {
		mainAssemblyVersion = ParseAssemblyInfo(assemblyInfoFile).AssemblyInformationalVersion;
		
		if (mainAssemblyVersion == null){
			throw new CakeException($"Unable to find version for assembly via {assemblyInfoFile}");
		}
	});

Task("NuGet-Pack")
	.IsDependentOn("Build")
	.IsDependentOn("NuGet-Get-Assembly-Version")
	.Description("Packs up a NuGet package")
	.Does(() => {
		NuGetPack(
			nuspecPath,
			new NuGetPackSettings() {
				OutputDirectory = buildDir,
				Version = mainAssemblyVersion
			}
		);
	});
	
Task("AppVeyor-Test")
	.IsDependentOn("Clean")
	.IsDependentOn("Test")
	.Does(() => {
		var jobId = EnvironmentVariable("APPVEYOR_JOB_ID");
		var resultsType = "nunit3";
		
		var wc = new System.Net.WebClient();
		var url = $"https://ci.appveyor.com/api/testresults/{resultsType}/{jobId}";
		var fullTestResultsPath = MakeAbsolute(testResultsFile).FullPath;
		
		Information("Uploading test results from {0} to {1}", fullTestResultsPath, url);
		wc.UploadFile(url, fullTestResultsPath);
	});

Task("AppVeyor")
	.IsDependentOn("AppVeyor-Test")
	.IsDependentOn("NuGet-Pack")
	;

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("None");

Task("Default")
    .IsDependentOn("Test");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
