﻿using System.Runtime.CompilerServices;
using Perfolizer.Properties;

[assembly: CLSCompliant(true)]

[assembly: InternalsVisibleTo("Perfolizer.Tests,PublicKey=" + PerfolizerInfo.PublicKey)]
[assembly: InternalsVisibleTo("Perfolizer.SimulationTests,PublicKey=" + PerfolizerInfo.PublicKey)]
[assembly: InternalsVisibleTo("Perfolizer.Simulations,PublicKey=" + PerfolizerInfo.PublicKey)]