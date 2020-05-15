using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Windows.Input;
using System.Windows.Threading;
using CloacaNative.IO;
using LanguageImplementation;

namespace CloacaConsole
{
  public class MainWindowViewModel : BaseViewModel
  {
    private ProgramExecutor _programExecutor;
    public ICommand ExecuteCommand { get; }
    public InputOutputStream _stdOutStream;

    public MainWindowViewModel()
      : base()
    {
      _stdOutStream = new InputOutputStream();
      _programExecutor = new ProgramExecutor(_stdOutStream);
      ExecuteCommand = new DelegateCommand(DoExecute);
      Task.Factory.StartNew(OutputReader);
    }

    public void DoExecute(object param)
    {
      var variablesIn = new Dictionary<string, object>();
      _programExecutor.Execute(ProgramText, variablesIn );
    }

    private string _programText = "";

    public string ProgramText
    {
      get => _programText;
      set
      {
        if (_programText != value)
        {
          _programText = value;
          OnPropertyChanged();
        }
      }
    }

    private string _outputText = "";

    public string OutputText
    {
      get => _outputText;
      set
      {
        if (_outputText != value)
        {
          _outputText = value;
          OnPropertyChanged();
        }
      }
    }

    private void OutputReader()
    {
      using (var streamReader = new StreamReader(_stdOutStream))
      {
        while (true)
        {
          var line = streamReader.ReadLine();
          Dispatcher.CurrentDispatcher.Invoke(() =>
          {
            OutputText += line;
          });
        }
      }
    }
  }
}
