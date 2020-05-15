using System;
using System.IO;
using System.Threading.Tasks;
using CloacaInterpreter;
using CloacaNative.IO.DataTypes;
using LanguageImplementation;

namespace CloacaNative.IO
{
  public class DefaultFileProvider : INativeFileProvider
  {
    public async Task<PyIOBase> Open(
      IInterpreter interpreter, FrameContext context,
      Handle handle, string path, string fileMode)
    {
      try
      {
        var nativeStream = File.Open(path, FileMode.Create);
        //PyTextIOWrapper result =
        //    (PyTextIOWrapper)await PyTextIOWrapperClass.Instance.Call(interpreter, context, new object[] { handle, nativeStream });
        //return result;
        var wrapper = new PyTextIOWrapper(handle, nativeStream);
        var result = (PyTextIOWrapper)await PyTextIOWrapperClass.Instance.Call(interpreter, context, new object[] { });
        return result;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public void RegisterBuiltins(Interpreter interpreter)
    {
      throw new NotImplementedException();
    }

    public void Dispose()
    {
      throw new NotImplementedException();
    }
  }
}