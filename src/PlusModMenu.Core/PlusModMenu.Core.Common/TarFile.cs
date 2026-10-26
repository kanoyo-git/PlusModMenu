using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PlusModMenu.Core.Common;

public class TarFile : IEnumerable<TarFile.FileInfo>, IEnumerable, IDisposable
{
	public class FileInfo
	{
		private readonly Stream _stream;

		private byte[]? _data;

		public string Name { get; set; }

		public long Position { get; set; }

		public long Length { get; set; }

		public byte[] Data
		{
			get
			{
				if (_data != null)
				{
					return _data;
				}
				_stream.Seek(Position, SeekOrigin.Begin);
				byte[] array = new byte[Length];
				_stream.Read(array, 0, array.Length);
				return _data = array;
			}
		}

		public FileInfo(string name, long position, long length, Stream stream)
		{
			Name = name;
			Position = position;
			Length = length;
			_stream = stream;
		}

		public override string ToString()
		{
			return Name;
		}
	}

	private readonly List<FileInfo> _files = new List<FileInfo>();

	private readonly Stream _stream;

	public TarFile(byte[] tarData)
	{
		_stream = new MemoryStream(tarData);
		ReadMetaData();
	}

	public TarFile(Stream tarStream)
	{
		_stream = tarStream;
		ReadMetaData();
	}

	private void ReadMetaData()
	{
		while (true)
		{
			byte[] array = new byte[100];
			_stream.Read(array, 0, array.Length);
			string text = Encoding.ASCII.GetString(array).Trim(default(char));
			if (string.IsNullOrWhiteSpace(text))
			{
				break;
			}
			byte[] array2 = new byte[12];
			_stream.Seek(24L, SeekOrigin.Current);
			_stream.Read(array2, 0, array2.Length);
			long num = Convert.ToInt64(Encoding.UTF8.GetString(array2).Trim(default(char)).Trim(), 8);
			_stream.Seek(376L, SeekOrigin.Current);
			if (!text.Equals("./", StringComparison.InvariantCulture))
			{
				if (num > 0)
				{
					_files.Add(new FileInfo(text, _stream.Position, num, _stream));
				}
				_stream.Seek(num, SeekOrigin.Current);
			}
			long position = _stream.Position;
			long num2 = 512 - position % 512;
			if (num2 == 512)
			{
				num2 = 0L;
			}
			_stream.Seek(num2, SeekOrigin.Current);
		}
	}

	public IEnumerator<FileInfo> GetEnumerator()
	{
		return _files.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void Dispose()
	{
		_stream.Dispose();
	}
}
