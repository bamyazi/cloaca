using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using CloacaInterpreter;
using CloacaNative.IO.DataTypes;
using LanguageImplementation;

namespace CloacaNative
{
  public class NativeResourceManager
  {
    private readonly List<Handle> _activeHandles = new List<Handle>();

    private readonly Dictionary<Type, INativeResourceProvider> _nativeResourceProviders =
      new Dictionary<Type, INativeResourceProvider>();

    private int _nextDescriptor = 1;


    public void RegisterProvider<T>(INativeResourceProvider provider)
    {
      var providerType = typeof(T);
      if (_nativeResourceProviders.ContainsKey(providerType))
        throw new Exception($"Provider of type '{providerType}' already exists.");
      Debug.WriteLine($"Registered provider '{providerType}'.");
      _nativeResourceProviders.Add(providerType, provider);
    }

    public bool TryGetProvider<T>(out T provider) where T : INativeResourceProvider
    {
      var providerType = typeof(T);
      if (_nativeResourceProviders.ContainsKey(providerType))
      {
        provider = (T) _nativeResourceProviders[providerType];
        return true;
      }

      provider = default;
      return false;
    }

    public void RegisterBuiltins(Interpreter interpreter)
    {
      interpreter.AddBuiltin(
        new WrappedCodeObject("open", typeof(NativeResourceManager).GetMethod("open_func"), this));
      interpreter.AddBuiltin(
          new WrappedCodeObject("print", typeof(NativeResourceManager).GetMethod("print_func"), this));
    }

    public Task<PyIOBase> open_func(IInterpreter interpreter, FrameContext context, string fileName, string fileMode)
    {
      if (TryGetProvider<INativeFileProvider>(out var provider))
      {
        var handle = CreateResourceHandle();
        _activeHandles.Add(handle);
        return provider.Open(interpreter, context, handle, fileName, fileMode);
      }

      throw new Exception("Missing provider.");
    }

    public void print_func(IInterpreter interpreter, FrameContext context, string text)
    {
      if (TryGetProvider<INativeStdioProvider>(out var provider))
      {
        var buffer = Encoding.UTF8.GetBytes($"{text}\n");
        provider.StdOut.Write(buffer, 0, buffer.Length);
        return;
      }

      throw new Exception("Missing provider.");
    }

    private Handle CreateResourceHandle()
    {
      return new Handle(this, _nextDescriptor++);
    }
  }
}