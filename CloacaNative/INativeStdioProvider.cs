using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloacaNative
{
  public interface INativeStdioProvider : INativeResourceProvider
  {
    Stream Stdin { get; }
    Stream StdOut { get; }
  }
}
