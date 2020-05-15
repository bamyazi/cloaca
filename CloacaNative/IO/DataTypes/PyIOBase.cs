using System;
using System.IO;
using System.Linq.Expressions;
using System.Numerics;
using LanguageImplementation;
using LanguageImplementation.DataTypes;

namespace CloacaNative.IO.DataTypes
{
  public class PyIOBaseClass : PyClass
  {
    private static PyIOBaseClass __instance;

    protected PyIOBaseClass(CodeObject __init__)
      : base("IOBase", __init__, new PyClass[0])
    {
      __instance = this;

      Expression<Action<PyTypeObject>> expr = instance => DefaultNew<PyIOBase>(null);
      var methodInfo = ((MethodCallExpression) expr.Body).Method;
      __new__ = new WrappedCodeObject("__new__", methodInfo, this);
    }

    public static PyIOBaseClass Instance => __instance ?? (__instance = new PyIOBaseClass(null));

    [ClassMember]
    public static void close(PyIOBase self)
    {
      self.Close();
    }

    [ClassMember]
    public static bool closed(PyIOBase self)
    {
      return self.Closed();
    }

    [ClassMember]
    public static PyInteger fileno(PyIOBase self)
    {
      return PyInteger.Create(self.Fileno());
    }

    [ClassMember]
    public static void flush(PyIOBase self)
    {
      self.Flush();
    }

    [ClassMember]
    public static bool isatty(PyIOBase self)
    {
      return self.IsaTTY();
    }

    [ClassMember]
    public static bool readable(PyIOBase self)
    {
      return PyBool.Create(self.Readable());
    }

    [ClassMember]
    public static PyString readline(PyIOBase self)
    {
      return readline(self, PyInteger.Create(-1));
    }

    [ClassMember]
    public static PyString readline(PyIOBase self, PyInteger size)
    {
      return PyString.Create(self.Readline());
    }

    [ClassMember]
    public static PyList readlines(PyIOBase self, PyInteger size)
    {
      return PyList.Create();
    }

    [ClassMember]
    public static void seek(PyIOBase self, PyInteger offset, PyInteger whence = null)
    {
      self.Seek(offset.number, whence?.number ?? new BigInteger(0));
    }

    [ClassMember]
    public static bool seekable(PyIOBase self)
    {
      return PyBool.Create(self.Seekable());
    }

    [ClassMember]
    public static PyInteger tell(PyIOBase self)
    {
      return PyInteger.Create(self.Tell());
    }

    [ClassMember]
    public static void truncate(PyIOBase self, PyInteger size)
    {
      self.Truncate(size.number);
    }

    [ClassMember]
    public static PyBool writeable(PyIOBase self)
    {
      return PyBool.Create(self.Writeable());
    }

    [ClassMember]
    public static void writelines(PyIOBase self, PyList lines)
    {
    }

    [ClassMember]
    public static void __del__(PyIOBase self)
    {
      self.Dispose();
    }
  }

  public class PyIOBase : PyObject, IDisposable
  {
    private bool _isClosed;

    private readonly Stream _nativeStream;
    private readonly Handle _resourceHandle;

    public PyIOBase()
    {
    }

    public PyIOBase(PyTypeObject fromType, Handle resourceHandle, Stream nativeStream)
      : base(fromType)
    {
      _resourceHandle = resourceHandle;
      _nativeStream = nativeStream;
    }

    public virtual void Dispose()
    {
      _nativeStream.Dispose();
    }

    public virtual void Close()
    {
      _nativeStream?.Close();
      _isClosed = true;
    }

    public virtual bool Closed()
    {
      return _isClosed;
    }

    public virtual BigInteger Fileno()
    {
      return _resourceHandle.Descriptor;
    }

    public virtual void Flush()
    {
      _nativeStream.Flush();
    }

    public virtual bool IsaTTY()
    {
      return false;
    }

    public virtual bool Readable()
    {
      return _nativeStream.CanRead;
    }

    public virtual string Readline()
    {
      return "This appears to be working";
    }

    public virtual bool Seekable()
    {
      return _nativeStream.CanSeek;
    }

    public virtual void Seek(BigInteger offset, BigInteger whence)
    {
      switch ((int) whence)
      {
        case 0:
          _nativeStream.Seek((long) offset, SeekOrigin.Begin);
          break;
        case 1:
          _nativeStream.Seek((long) offset, SeekOrigin.Current);
          break;
        case 2:
          _nativeStream.Seek((long) offset, SeekOrigin.Current);
          break;
      }
    }

    public virtual long Tell()
    {
      return _nativeStream.Position;
    }

    public virtual void Truncate(BigInteger length)
    {
      _nativeStream.SetLength((long) length);
    }

    public virtual bool Writeable()
    {
      return _nativeStream.CanWrite;
    }
  }
}