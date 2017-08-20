using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BrewBot
{
	/// <summary>
	/// Write data to CSV files
	/// </summary>
	class CsvDataWriter : IDisposable
	{
		/// <summary>
		/// Create a csv data writer. Creates the file at the given path if it does not exist.
		/// This overwrites existing data.
		/// </summary>
		/// <param name="filepath"></param>
		public CsvDataWriter( string filepath )
		{
			if ( !File.Exists( filepath ) )
			{
				File.Create( filepath );
			}
			_writer = new StreamWriter( filepath );
		}

		StreamWriter _writer;
		
		/// <summary>
		/// Write a line to the csv file
		/// </summary>
		/// <param name="data"></param>
		public void WriteLine( IList<string> data )
		{
			StringBuilder builder = new StringBuilder();
			foreach ( string nextItem in data )
			{
				if ( builder.Length > 0 )
				{
					builder.Append( ',' );
				}
				builder.Append( nextItem );
			}
			_writer.WriteLine( builder );
		}

		/// <summary>
		/// Dispose the writer
		/// </summary>
		public void Dispose()
		{
			_writer.Dispose();
		}
	}
}
