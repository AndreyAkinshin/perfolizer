using System;
using System.Runtime.CompilerServices;
using Perfolizer.Properties;

[assembly: CLSCompliant(true)]

[assembly: InternalsVisibleTo("Perfolizer.Tests,PublicKey=" + PerfolizerInfo.PublicKey)]
[assembly: InternalsVisibleTo("Perfolizer.Simulator.RqqPelt,PublicKey=" + PerfolizerInfo.PublicKey)]