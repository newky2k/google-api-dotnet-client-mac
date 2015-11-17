using System;
using System.IO;
using Google.Apis.Drive.v2;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Services;

namespace GoogleApiSample
{
	public class GoogleAuthenticator
	{
		private Stream mCredentialFileStream;

		public string ApplicationName
		{
			get 
			{
				return "GoogleMacAuthSample";
			}
		}
		/// <summary>
		/// Gets the scopes to use to access the drive
		/// </summary>
		/// <value>The scopes.</value>
		public string[] Scopes
		{
			get 
			{
				return new string[] { DriveService.Scope.Drive,  // view and manage your files and documents
					DriveService.Scope.DriveAppdata,  // view and manage its own configuration data
					DriveService.Scope.DriveAppsReadonly,   // view your drive apps
					DriveService.Scope.DriveFile,   // view and manage files created by this app
					DriveService.Scope.DriveMetadataReadonly,   // view metadata for files
					DriveService.Scope.DriveReadonly,   // view files and documents on your drive
					DriveService.Scope.DriveScripts }; 
			}
		}
			
		/// <summary>
		/// Gets or sets the credentials file stream.
		/// </summary>
		/// <value>The credentials file stream.</value>
		public Stream CredentialsFileStream
		{
			get 
			{
				if (mCredentialFileStream == null) 
				{
					using (var stream = new FileStream ("client_secret.json", FileMode.Open, FileAccess.Read)) {

						mCredentialFileStream = stream;
					}
				}

				return mCredentialFileStream;
			}
			set 
			{
				mCredentialFileStream = value;
			}
		}

		public GoogleAuthenticator ()
		{
		}

		public void Authenticate()
		{
			string credPath = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal);
			credPath = Path.Combine (credPath, String.Format(".credentials{0}{1}", Path.DirectorySeparatorChar, ApplicationName.ToLower()));

			var credential = GoogleWebAuthorizationBroker.AuthorizeAsync (
				GoogleClientSecrets.Load (CredentialsFileStream).Secrets,
				Scopes,
				"user",
				CancellationToken.None,
				new FileDataStore (credPath, true)).Result;

			//UpdateProgress ("Credential file saved to: " + credPath);

			// Create Drive API service.
			var GoogleDriveService = new DriveService (new BaseClientService.Initializer () {
				HttpClientInitializer = credential,
				ApplicationName = ApplicationName,
			});
		}
	}
}

