using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloacaInterpreter;

namespace CloacaNative.IO
{
  public class DefaultNativeStdioProvider : INativeStdioProvider
  {
    public Stream Stdin { get; }
    public Stream StdOut { get; }

    public DefaultNativeStdioProvider(Stream stdin, Stream stdout)
    {
      Stdin = stdin;
      StdOut = stdout;
    }

    public void RegisterBuiltins(Interpreter interpreter)
    {
      throw new NotImplementedException();
    }
  }
}
