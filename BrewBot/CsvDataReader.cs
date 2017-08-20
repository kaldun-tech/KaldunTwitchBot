using System;
using System.IO;

namespace BrewBot
{
	/// <summary>
	/// Read data from CSV files
	/// </summary>
	class CsvDataReader : IDisposable
	{
		/// <summary>
		///  Create a csv data reader. Creates the file at the given path if it does not exist.
		/// </summary>
		/// <param name="filepath"></param>
		public CsvDataReader( string filepath )
		{
			if ( !File.Exists( filepath ) )
			{
				File.Create( filepath );
			}
			_reader = new StreamReader( filepath );
		}

		StreamReader _reader;

		/// <summary>
		/// Read a line of csv separated by commas
		/// </summary>
		/// <returns>A string array of the contents, or null on failure</returns>
		public string[] ReadLine()
		{
			string line;
			if ( !string.IsNullOrEmpty( line = _reader.ReadLine() )  )
			{
				return line.Split( new char[] { ',' } );
			}

			return null;
		}

		/// <summary>
		/// Dispose the reader
		/// </summary>
		public void Dispose()
		{
			_reader.Dispose();
		}
	}
}
