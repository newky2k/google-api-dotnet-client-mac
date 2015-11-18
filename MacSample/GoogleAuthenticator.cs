using System;
using System.IO;
using Google.Apis.Drive.v2;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using System.Threading.Tasks;
using AppKit;

namespace MacSample
{
	public class GoogleAuthenticator
	{

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
			
	
		public GoogleAuthenticator ()
		{
		}

		public void Authenticate()
		{
			//execute off of the main ui thread so that the auth window can be shown
			Task.Run (() => 
			{
				// Set an output path for the saved authorisation token	
				string credPath = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal);
				credPath = Path.Combine (credPath, String.Format(".credentials{0}{1}", Path.DirectorySeparatorChar, ApplicationName.ToLower()));

				UserCredential credential = null;

				/***
				 * Replace client_id and the client_secret(and any other values) from the client_secret.json file
				 * 
				 * You can also create a new client_secret.json file from the Google Develeopers console
				 * 
				 * https://console.developers.google.com
				 * 
				 */
				using (var stream = new FileStream ("client_secret.json", FileMode.Open, FileAccess.Read)) {

					credential = MacGoogleWebAuthorizationBroker.AuthorizeAsync (
						GoogleClientSecrets.Load (stream).Secrets,
						Scopes,
						"user",
						CancellationToken.None,
						new FileDataStore (credPath, true)).Result;

				}


				// Create Drive API service.
				var GoogleDriveService = new DriveService (new BaseClientService.Initializer () {
					HttpClientInitializer = credential,
					ApplicationName = ApplicationName,
				});
			});

		}
			
	}
}

