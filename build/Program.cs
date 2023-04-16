using System.IO;
using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Build;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Common.Tools.DotNet.Pack;
using Cake.Common.Tools.DotNet.Test;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;

public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .UseContext<BuildContext>()
            .Run(args);
    }
}

public class BuildContext : FrostingContext
{
    public string BuildConfiguration { get; }

    public DirectoryPath RootDirectory { get; }
    public DirectoryPath SolutionDirectory { get; }
    public DirectoryPath ArtifactsDirectory { get; }
    public FilePath SolutionFile { get; }
    public FilePath MainProjectFile { get; }
    public FilePath TestsProjectFile { get; }


    public BuildContext(ICakeContext context)
        : base(context)
    {
        BuildConfiguration = context.Argument("Configuration", "Release");

        RootDirectory = new DirectoryPath(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.FullName);
        SolutionDirectory = RootDirectory.Combine("src").Combine("Perfolizer");
        ArtifactsDirectory = RootDirectory.Combine("artifacts");

        SolutionFile = SolutionDirectory.CombineWithFilePath("Perfolizer.sln");
        MainProjectFile = SolutionDirectory.Combine("Perfolizer").CombineWithFilePath("Perfolizer.csproj");
        TestsProjectFile = SolutionDirectory.Combine("Perfolizer.Tests").CombineWithFilePath("Perfolizer.Tests.csproj");
    }
}

[TaskName("Restore")]
public class RestoreTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetRestore(context.SolutionFile.FullPath);
    }
}

[TaskName("Build")]
[IsDependentOn(typeof(RestoreTask))]
public class BuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetBuild(context.SolutionFile.FullPath, new DotNetBuildSettings
        {
            Configuration = context.BuildConfiguration,
            NoRestore = true,
            DiagnosticOutput = true,
            Verbosity = DotNetVerbosity.Minimal
        });
    }
}

[TaskName("Tests")]
[IsDependentOn(typeof(BuildTask))]
public class TestsTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetTest(context.TestsProjectFile.FullPath, new DotNetTestSettings
        {
            Configuration = context.BuildConfiguration,
            NoBuild = true,
            NoRestore = true
        });
    }
}

[TaskName("Pack")]
[IsDependentOn(typeof(BuildTask))]
public class PackTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.CleanDirectory(context.ArtifactsDirectory);

        var settings = new DotNetPackSettings
        {
            Configuration = context.BuildConfiguration,
            OutputDirectory = context.ArtifactsDirectory.FullPath,
            IncludeSource = true,
            IncludeSymbols = true,
            SymbolPackageFormat = "snupkg" 
        };
        var version = context.Argument("Version", "");
        if (!string.IsNullOrEmpty(version))
        {
            if (version.StartsWith("refs/tags/"))
                version = version["refs/tags/".Length..];
            if (version.StartsWith('v'))
                version = version[1..];
            context.Information("Specified version: " + version);
            settings.MSBuildSettings = new DotNetMSBuildSettings
            {
                ArgumentCustomization = args => args
                    .Append($"/p:Version={version}")
                    .Append($"/p:AssemblyVersion={version}")
                    .Append($"/p:AssemblyFileVersion={version}")
                    .Append($"/p:InformationalVersion={version}")
                    .Append($"/p:PackageVersion={version}")
            };
        }

        context.DotNetPack(context.MainProjectFile.FullPath, settings);
    }
}


[TaskName("Default")]
[IsDependentOn(typeof(BuildTask))]
public class DefaultTask : FrostingTask
{
}