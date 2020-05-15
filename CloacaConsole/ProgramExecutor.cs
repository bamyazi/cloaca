using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CloacaInterpreter;
using CloacaNative;
using CloacaNative.IO;
using LanguageImplementation;

namespace CloacaConsole
{
  public class ProgramExecutor
  {
    private NativeResourceManager _nativeResourceManager;
    private readonly Scheduler _scheduler;
    private readonly Interpreter _interpreter;
    private readonly Stream _stdout;

    public ProgramExecutor(InputOutputStream stdout)
    {
      _stdout = stdout;
      _scheduler = new Scheduler();
      _interpreter = new Interpreter(_scheduler);
      _nativeResourceManager = new NativeResourceManager();
      _nativeResourceManager.RegisterProvider<INativeFileProvider>(new DefaultFileProvider());
      _nativeResourceManager.RegisterProvider<INativeStdioProvider>(new DefaultNativeStdioProvider(null, stdout));
      _nativeResourceManager.RegisterBuiltins(_interpreter);
      _scheduler.SetInterpreter(_interpreter);
    }

    public async void Execute(string program, Dictionary<string, object> variablesIn)
    {
      CodeObject compiledProgram = null;
      try
      {
        compiledProgram = ByteCodeCompiler.Compile(program, variablesIn);
      }
      catch (Exception ex)
      {
        WriteStdout(ex.Message);
        return;
      }
      var receipt = _scheduler.Schedule(compiledProgram);
      
      foreach (var variableName in variablesIn.Keys)
      {
        receipt.Frame.SetVariable(variableName, variablesIn[variableName]);
      }

      while (!_scheduler.Done)
      {
        await _scheduler.Tick();
      }

      if (receipt.Completed)
      {
        if (receipt.EscapedExceptionInfo != null)
        {
          WriteStdout($"{receipt.EscapedExceptionInfo.SourceException}");
        }
        WriteStdout("Done.");
      }
    }

    private void WriteStdout(string message)
    {
      var buffer = Encoding.UTF8.GetBytes($"{message}\r\n");
      _stdout.Write(buffer, 0, buffer.Length);
    }
  }
}
