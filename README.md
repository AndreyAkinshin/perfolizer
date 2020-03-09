<h3 align="center">

  ![](docs/logo/perfolizer.svg)

</h3>

<h3 align="center">
  
  [![NuGet](https://img.shields.io/nuget/v/Perfolizer)](https://www.nuget.org/packages/Perfolizer/)
  [![MyGet](https://img.shields.io/myget/perfolizer/vpre/Perfolizer?label=myget)](https://www.myget.org/feed/perfolizer/package/nuget/Perfolizer)
  [![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md)
  [![build](https://github.com/AndreyAkinshin/perfolizer/workflows/build/badge.svg?branch=master)](https://github.com/AndreyAkinshin/perfolizer/actions?query=workflow%3Abuild)

</h3>

**Perfolizer** is a collection of useful algorithms for performance analysis.
You can use it as a [NuGet package](https://www.nuget.org/packages/Perfolizer/) or via a [command-line tool](#command-line-tool).

Perfolizer is used as a statistical engine for [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) (the most popular .NET library for benchmarking).

## Algorithms

Currently, the following algorithms are available:

* [Changepoint detection](#changepoint-detection)
* [Multimodal-sensitive histograms](#multimodal-sensitive-histograms)
* [Multimodality detection](#multimodality-detection)
* [Range Quantile Queries](#range-quantile-queries)
* [QuickSelectAdaptive](#quickselectadaptive)

It's only the beginning, a lot of additional algorithms and approaches for performance analysis are coming!

### Changepoint detection

If you have a history of performance measurements for a test, you may want to find changepoints.
They correspond to moments when the underlying distribution of measurements is changed.
It may be a simple performance degradation/acceleration or a more complicated change (e.g., the variance is increased, or one of the modes in a multimodal distribution is shifted).

Perfolizer provides two algorithms for changepoint detection: EdPelt and RqqPelt.
They can be illustrated with the following picture:

![changepoints](https://user-images.githubusercontent.com/2259237/75871421-6ffdb700-5e04-11ea-8f28-5e55550e3b4b.png)

EdPelt is based on [this](https://doi.org/10.1007/s11222-016-9687-5) paper; it's a computationally efficient algorithm with a high level of precision.
However, it doesn't work well when you have a long performance history with many changepoints.
RqqPelt is a modification of EdPelt, which is optimal for long-history cases.
Both algorithms are nonparametric (they support multimodal distributions).

Here is a usage example of these algorithms:

```cs
var random = new RandomDistribution(42);
var data = new List<double>();
const int n = 20;
for (int i = 0; i < n; i++)
    data.AddRange(random.Gaussian(100, mean: 20 * i, stdDev: 5));

var rqqIndexes = RqqPeltChangePointDetector.Instance.GetChangePointIndexes(data.ToArray());
var edIndexes = EdPeltChangePointDetector.Instance.GetChangePointIndexes(data.ToArray());
```

Here we have 20 normal distributions with shifted mean values.
`RqqPeltChangePointDetector` and `EdPeltChangePointDetector` allows getting indexes of the changepoint.
Here is the result of both algorithms:

```md
RqqPelt  EdPelt
     99     99
    199    199
    299    299
    399      -
    499    499
    599      -
    699    701
    799    797
    899      -
    999    999
   1099      -
   1199   1201
   1301   1299
   1399      -
   1499   1499
   1599      -
   1699   1699
   1799   1799
   1899   1900
```

As you can see, both algorithms have pretty good accuracy, but RqqPelt detects more changepoint when where are many of them.

*References:*

* Haynes, Kaylea, Paul Fearnhead, and Idris A. Eckley. "A computationally efficient nonparametric approach for changepoint detection." Statistics and Computing 27, no. 5 (2017): 1293-1305.  
  https://doi.org/10.1007/s11222-016-9687-5
* Killick, Rebecca, Paul Fearnhead, and Idris A. Eckley. "Optimal detection of changepoints with a linear computational cost." Journal of the American Statistical Association 107, no. 500 (2012): 1590-1598.  
  https://arxiv.org/pdf/1101.1438.pdf

### Multimodal-sensitive histograms

It's very important to understand how the distribution of performance measurement looks like.
Sometimes, we want to get an idea of the distribution shape; histograms is a great way to do it.
Unfortunately, the classic approach to build histograms using a fixed bin size may "hide" some important properties of the distribution like multimodality.
Perfolizer provides an adaptive way to build histograms with dynamic bin size: it corrects the borders of each bin in order to highlight situations when the distribution is multimodal.

Here is a usage example:

```cs
var random = new RandomDistribution(42);
var data = new List<double>();
data.AddRange(random.Gaussian(200, mean: 20, stdDev: 1));
data.AddRange(random.Gaussian(200, mean: 22, stdDev: 1));

const double binSize = 0.5;
Console.WriteLine("*** Simple Histogram ***");
Console.WriteLine(SimpleHistogramBuilder.Instance.Build(data, binSize).ToString());
Console.WriteLine("*** Adaptive Histogram ***");
Console.WriteLine(AdaptiveHistogramBuilder.Instance.Build(data, binSize).ToString());
```

Below you can see histograms that built using simple and adaptive approaches:

```
*** Simple Histogram ***
[17.000 ; 17.500) | @
[17.500 ; 18.000) | @@@@@@
[18.000 ; 18.500) | @@@@@@@@@
[18.500 ; 19.000) | @@@@@@@@@@@@@@@@@@@@@@@
[19.000 ; 19.500) | @@@@@@@@@@@@@@@@@@@@@@@@@@@
[19.500 ; 20.000) | @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
[20.000 ; 20.500) | @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
[20.500 ; 21.000) | @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
[21.000 ; 21.500) | @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
[21.500 ; 22.000) | @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
[22.000 ; 22.500) | @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
[22.500 ; 23.000) | @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
[23.000 ; 23.500) | @@@@@@@@@@@@@
[23.500 ; 24.000) | @@@@@@@@@@@@@
[24.000 ; 24.500) | @@@
*** Adaptive Histogram ***
[16.910 ; 17.404) | @
[17.404 ; 17.815) | @@@@
[17.815 ; 18.328) | @@@@@@@@
[18.328 ; 18.879) | @@@@@@@@@@@@
[18.879 ; 19.379) | @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
[19.379 ; 20.149) | @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
[20.149 ; 20.878) | @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
[20.878 ; 21.782) | @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
[21.782 ; 22.373) | @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
[22.373 ; 22.938) | @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
[22.938 ; 23.865) | @@@@@@@@@@@@@@@@@@@@@@@@
[23.865 ; 24.735) | @@@@@@
```

### Multimodality detection

It's very important to know that the performance distribution that you are working with is multimodal.
Perfolizer uses the [mvalue approach](http://www.brendangregg.com/FrequencyTrails/modes.html) to detect such distribution.
The rule of thumb is simple: when the mvalue is higher than 2.8-3.2,
  the distribution is most probably is multimodal.

Here is a usage example which demonstrate mvalue calculations for unimodal, bimodal, and trimodal distributions:

```cs
var random = new RandomDistribution(42);
var data = new List<double>();
data.AddRange(random.Gaussian(200, mean: 20));
data.AddRange(random.Gaussian(200, mean: 22));
List<double>
    unimodal = new List<double>(),
    bimodal = new List<double>(),
    trimodal = new List<double>();
unimodal.AddRange(random.Gaussian(200, mean: 20));
bimodal.AddRange(random.Gaussian(200, mean: 20));
bimodal.AddRange(random.Gaussian(200, mean: 30));
trimodal.AddRange(random.Gaussian(200, mean: 20));
trimodal.AddRange(random.Gaussian(200, mean: 30));
trimodal.AddRange(random.Gaussian(200, mean: 40));
Console.WriteLine("Unimodal : " + MValueCalculator.Calculate(unimodal));
Console.WriteLine("Bimodal  : " + MValueCalculator.Calculate(bimodal));
Console.WriteLine("Trimodal : " + MValueCalculator.Calculate(trimodal));
```

And here is the corresponding mvalues:

```
Unimodal : 2.32
Bimodal  : 3.949748743718593
Trimodal : 5.915343915343915
```

*References:*

* Brendan Gregg, "Frequency Trails: Modes and Modality", 2015  
  http://www.brendangregg.com/FrequencyTrails/modes.html

### Range Quantile Queries

RQQ (Range Quantile Queries) is a data structure based on wavelet trees that allows getting the given quantile in the given range.
It takes O(n*log(n)) to build it, and O(log(n)) to process a single query.

Here is usage example:

```cs
var data = new double[] {6, 2, 0, 7, 9, 3, 1, 8, 5, 4};
var rqq = new Rqq(data);
Console.WriteLine(rqq.DumpTreeAscii());
Console.WriteLine();
for (int i = 0; i < data.Length; i++)
    Console.WriteLine($"sorted[{i}] = {rqq.Select(0, data.Length - 1, i)}");
```

The `Select(int l, int r, int k)` methods returns k-th smallest element on the [l;r] range.
The RQQ implementation can also present the built tree with the help of ASCII graphics:

```md
                     ┌─────────────────────┐               
                     │ 6 2 0 7 9 3 1 8 5 4 │               
                     │ 1 0 0 1 1 0 0 1 1 0 │               
                     └┬───────────────────┬┘               
                      │                   │                
           ┌──────────┴┐                 ┌┴──────────┐     
           │ 2 0 3 1 4 │                 │ 6 7 9 8 5 │     
           │ 0 0 1 0 1 │                 │ 0 0 1 1 0 │     
           └┬─────────┬┘                 └┬─────────┬┘     
            │         │                   │         │      
      ┌─────┴─┐     ┌─┴───┐         ┌─────┴─┐     ┌─┴───┐  
      │ 2 0 1 │     │ 3 4 │         │ 6 7 5 │     │ 9 8 │  
      │ 1 0 0 │     │ 0 1 │         │ 0 1 0 │     │ 1 0 │  
      └┬─────┬┘     └┬───┬┘         └┬─────┬┘     └┬───┬┘  
       │     │       │   │           │     │       │   │   
  ┌────┴┐   ┌┴──┐ ┌──┴┐ ┌┴──┐   ┌────┴┐   ┌┴──┐ ┌──┴┐ ┌┴──┐
  │ 0 1 │   │ 2 │ │ 3 │ │ 4 │   │ 6 5 │   │ 7 │ │ 8 │ │ 9 │
  │ 0 1 │   │   │ │   │ │   │   │ 1 0 │   │   │ │   │ │   │
  └┬───┬┘   └───┘ └───┘ └───┘   └┬───┬┘   └───┘ └───┘ └───┘
   │   │                         │   │                     
┌──┴┐ ┌┴──┐                   ┌──┴┐ ┌┴──┐                   
│ 0 │ │ 1 │                   │ 5 │ │ 6 │                   
│   │ │   │                   │   │ │   │                   
└───┘ └───┘                   └───┘ └───┘

sorted[0] = 0
sorted[1] = 1
sorted[2] = 2
sorted[3] = 3
sorted[4] = 4
sorted[5] = 5
sorted[6] = 6
sorted[7] = 7
sorted[8] = 8
sorted[9] = 9
```

*References:*

* Gagie, Travis, Simon J. Puglisi, and Andrew Turpin. "Range quantile queries: Another virtue of wavelet trees." In International Symposium on String Processing and Information Retrieval, pp. 1-6. Springer, Berlin, Heidelberg, 2009.  
  https://arxiv.org/pdf/0903.4726.pdf

### QuickSelectAdaptive

QuickSelectAdaptive allows getting the k-th smallest element from the given array.
It takes O(n) operations in the worst case, and it works pretty fast in practice using different heuristics
  from [this paper](http://erdani.com/research/sea2017.pdf) by Andrei Alexandrescu.

Here is a usage example:

```cs
var shuffler = new Shuffler(42);
var data = new double[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
shuffler.Shuffle(data);

var selector = new QuickSelectAdaptive();
for (int i = 0; i < data.Length; i++)
    Console.WriteLine($"data[{i}] = {selector.Select(data, i)}");
```

Here is the output:

```
data[0] = 0
data[1] = 1
data[2] = 2
data[3] = 3
data[4] = 4
data[5] = 5
data[6] = 6
data[7] = 7
data[8] = 8
data[9] = 9
```

*References:*

* Alexandrescu, Andrei. "Fast deterministic selection."  
  http://erdani.com/research/sea2017.pdf

## Command-line tool

### Command-line installation

First of all, you should install [.NET Core SDK 3.1](https://dotnet.microsoft.com/download) or newer.
Once installed, run the following command to install [Perfolizer.Tool]((https://www.nuget.org/packages/Perfolizer.Tool/)):

```sh
dotnet tool install -g Perfolizer.Tool
```

If you already have a previous version installed, you can upgrade to the latest version using the following command:

```sh
dotnet tool update -g Perfolizer.Tool
```

If you want to remove the tool:

```sh
dotnet tool uninstall -g Perfolizer.Tool
```

You can find more information about .NET Core Global Tools [here](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools).

### Command-line usage

The basic usage syntax is:

```sh
dotnet perfolizer [arguments] [options]
```

By default, the tool can be found in `$HOME/.dotnet/tools` on Linux/macOS and in `%USERPROFILE%\.dotnet\tools` on Windows
  (you can specify the directory via the `--tool-path` option of `dotnet tool install`).
In order to shortify the command line, you can also introduce a symlink or an alias for this tool.
E.g.,

```sh
sudo ln -s ~/.dotnet/tools/dotnet-perfolizer /usr/bin/perfolizer
```

or

```sh
alias perfolizer='~/.dotnet/tools/dotnet-perfolizer'
```

After that, you can use it as:

```sh
perfolizer [arguments] [options]
```

### Command-line examples

Changepoint detection:

```sh
$ perfolizer cpd --data '0;0;0;0;100;100;100;100;100' --dist 1
3
```

Histograms:

```sh
$ perfolizer hist --data '0;0;0;1;2;3;54;234;234;24;12;21;3;123;3;35;134'
[ -0.180 ;  54.180) | @@@@@@@@@@@@@
[ 54.180 ; 101.320) | 
[101.320 ; 155.679) | @@
[155.679 ; 206.821) | 
[206.821 ; 261.180) | @@
```

Range quantile queries tree:

```sh
$ perfolizer rqq --data '6;2;0;7;9;3;1;8;5;4'
                     ┌─────────────────────┐               
                     │ 6 2 0 7 9 3 1 8 5 4 │               
                     │ 1 0 0 1 1 0 0 1 1 0 │               
                     └┬───────────────────┬┘               
                      │                   │                
           ┌──────────┴┐                 ┌┴──────────┐     
           │ 2 0 3 1 4 │                 │ 6 7 9 8 5 │     
           │ 0 0 1 0 1 │                 │ 0 0 1 1 0 │     
           └┬─────────┬┘                 └┬─────────┬┘     
            │         │                   │         │      
      ┌─────┴─┐     ┌─┴───┐         ┌─────┴─┐     ┌─┴───┐  
      │ 2 0 1 │     │ 3 4 │         │ 6 7 5 │     │ 9 8 │  
      │ 1 0 0 │     │ 0 1 │         │ 0 1 0 │     │ 1 0 │  
      └┬─────┬┘     └┬───┬┘         └┬─────┬┘     └┬───┬┘  
       │     │       │   │           │     │       │   │   
  ┌────┴┐   ┌┴──┐ ┌──┴┐ ┌┴──┐   ┌────┴┐   ┌┴──┐ ┌──┴┐ ┌┴──┐
  │ 0 1 │   │ 2 │ │ 3 │ │ 4 │   │ 6 5 │   │ 7 │ │ 8 │ │ 9 │
  │ 0 1 │   │   │ │   │ │   │   │ 1 0 │   │   │ │   │ │   │
  └┬───┬┘   └───┘ └───┘ └───┘   └┬───┬┘   └───┘ └───┘ └───┘
   │   │                         │   │                     
┌──┴┐ ┌┴──┐                   ┌──┴┐ ┌┴──┐                   
│ 0 │ │ 1 │                   │ 5 │ │ 6 │                   
│   │ │   │                   │   │ │   │                   
└───┘ └───┘                   └───┘ └───┘                   
```

k-th smallest element:

```sh
$ perfolizer select --data '6;2;0;7;9;3;1;8;5;4' -k 3
3
```

mvalue calculations:

```sh
$ perfolizer mvalue --data '0;0;0;0;0;0;0;0'
2
$ perfolizer mvalue --data '0;0;0;0;1;1;1;1'
4
```

## NuGet packages

You can use perfolizer via one of the following NuGet Packages:

* [Perfolizer](https://www.nuget.org/packages/Perfolizer/) (if you want to use Perfolizer from .NET application)
* [Perfolizer.Tool](https://www.nuget.org/packages/Perfolizer.Tool/) (if you want to use Perfolizer from command line)

Stable versions of both packages are available on [nuget.org](https://www.nuget.org/).
If you want to use prerelease packages, you can download them from [myget.org](https://www.myget.org/) via the following feed: `https://www.myget.org/F/perfolizer/api/v3/index.json`.
Here is an example of `NuGet.Config`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="perfolizer-nightly" value="https://www.myget.org/F/perfolizer/api/v3/index.json" />
  </packageSources>
</configuration>
```

## License

Copyright (c) 2020 Andrey Akinshin  
Copyright (c) 2013–2020 .NET Foundation and contributors

Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.