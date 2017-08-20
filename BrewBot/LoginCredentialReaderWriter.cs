using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace BrewBot
{
	public class LoginCredentialReaderWriter
	{
		/// <summary>
		/// Create a new login credentials reader/writer with the default file path
		/// </summary>
		public LoginCredentialReaderWriter() : this( _defaultLoginCredentialsPath )
		{
		}

		/// <summary>
		/// Create a new login credentials reader/writer with the specified file path
		/// </summary>
		/// <param name="credentialFilePath"></param>
		public LoginCredentialReaderWriter( string credentialFilePath )
		{
			_filePath = credentialFilePath;
		}

		private static readonly string _defaultLoginCredentialsPath = Path.Combine( Environment.CurrentDirectory, "credentials.xml" );

		private string _filePath;
		private object _lock = new object();

		/// <summary>
		/// Read credentials from a file
		/// </summary>
		/// <returns>List of credentials on success, or null on failure. The credentials will contain the username, oauth, channel, and config file name.</returns>
		public IList<string> ReadCredentials()
		{
			lock ( _lock )
			{
				using ( Stream credentialsStream = new FileStream( _filePath, FileMode.Open, FileAccess.Read ) )
				{
					XmlDocument document = new XmlDocument();
					document.Load( credentialsStream );
					XmlNode userNameNode = document.DocumentElement.SelectSingleNode( "/credentials/username" );
					XmlNode oauthTokenNode = document.DocumentElement.SelectSingleNode( "/credentials/oauth" );
					XmlNode chatChannelNode = document.DocumentElement.SelectSingleNode( "/credentials/channel" );
					XmlNode configFileNode = document.DocumentElement.SelectSingleNode( "/credentials/config-file" );

					string userName = ( userNameNode == null ) ? null : userNameNode.InnerText;
					string oauth = ( oauthTokenNode == null ) ? null : oauthTokenNode.InnerText;
					string channel = ( chatChannelNode == null ) ? null : chatChannelNode.InnerText;
					string configFileName = ( configFileNode == null ) ? null : configFileNode.InnerText;

					// Config file name can be null
					if ( !string.IsNullOrEmpty( userName ) && !string.IsNullOrEmpty( oauth ) && !string.IsNullOrEmpty( channel ) )
					{
						return new List<string> { userName, oauth, channel, configFileName };
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Write credentials out to file so we can read them later
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="oauth"></param>
		/// <param name="channel"></param>
		/// <param name="configFilePath"></param>
		public void WriteCredentials( string userName, string oauth, string channel, string configFilePath )
		{
			if ( !string.IsNullOrEmpty( userName ) && !string.IsNullOrEmpty( oauth ) && !string.IsNullOrEmpty( channel ) )
			{
				lock ( _lock )
				{
					using ( Stream stream = new FileStream( _filePath, FileMode.OpenOrCreate ) )
					{
						using ( StreamWriter credentialsStream = new StreamWriter( stream ) )
						{
							credentialsStream.WriteLine( "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" );
							credentialsStream.WriteLine( "<credentials>" );
							credentialsStream.WriteLine( "  <username>{0}</username>", userName );
							credentialsStream.WriteLine( "  <oauth>{0}</oauth>", oauth );
							credentialsStream.WriteLine( "  <channel>{0}</channel>", channel );
							if ( configFilePath != null )
							{
								credentialsStream.WriteLine( "  <config-file>{0}</config-file>", configFilePath );
							}
							credentialsStream.WriteLine( "</credentials>" );
						}
					}
				}
			}
		}
	}
}
