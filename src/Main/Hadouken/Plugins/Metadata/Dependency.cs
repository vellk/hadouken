﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Plugins.Metadata
{
    public sealed class Dependency
    {
        public string Name { get; set; }

        public SemanticVersionRange VersionRange { get; set; }
    }
}
