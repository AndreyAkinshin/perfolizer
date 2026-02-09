<h3 align="center">

  ![](docs/logo/perfolizer.svg)

</h3>

<h3 align="center">
  
  [![NuGet](https://img.shields.io/nuget/v/Perfolizer)](https://www.nuget.org/packages/Perfolizer/)
  [![Downloads](https://img.shields.io/nuget/dt/perfolizer.svg)](https://www.nuget.org/packages/Perfolizer/)
  [![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md)

</h3>

**Perfolizer** is an experimental project providing a smart analytical API for performance measurement analysis.

The project grew out of [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet), extracting and refining its analytical approaches into a dedicated NuGet package for performance data analysis independent of any benchmarking framework.

Perfolizer covers a wide range of analytical features, but the core statistical engine is now migrating to [Pragmastat](https://github.com/AndreyAkinshin/pragmastat).
This separation across dedicated repositories establishes cleaner boundaries, sharper focus for each project, and better long-term maintainability.