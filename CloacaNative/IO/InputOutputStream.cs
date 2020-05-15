using System;
using System.Collections.Concurrent;
using System.IO;

namespace CloacaNative.IO
{
  public class InputOutputStream : Stream
  {
    private readonly ConcurrentQueue<byte> _dataQueue = new ConcurrentQueue<byte>();

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => _dataQueue.Count;
    public override long Position { get; set; }

    public override void Flush()
    {
      // Not available
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      // Not available
      return 0;
    }

    public override void SetLength(long value)
    {
      // Not available
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      while (_dataQueue.Count < 1) ;
      var availableData = _dataQueue.Count;
      //GD.Print($"Read {count} requested, {availableData} bytes available.");
      var toRead = Math.Min(count, availableData);
      var read = 0;
      for (var i = 0; i < toRead; i++)
        if (_dataQueue.TryDequeue(out var data))
        {
          buffer[read] = data;
          read++;
        }
        else
        {
          break;
        }

      return read;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      foreach (var data in buffer) _dataQueue.Enqueue(data);
    }
  }
}