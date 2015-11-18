using System;
using Google.Apis.Auth.OAuth2.Responses;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2.Requests;
using System.Threading;
using AppKit;
using Foundation;
using CoreGraphics;
using WebKit;
using System.Net;
using Google.Apis.Auth.Windows;
using System.Net.Sockets;
using System.IO;
using System.Collections.Specialized;
using System.Linq;

namespace Google.Apis.Auth.OAuth2
{
	public class MacBrowserCodeReciever : ICodeReceiver
	{
		/// <summary>The call back format. Expects one port parameter.</summary>
		private const string LoopbackCallback = "http://localhost:{0}/authorize/";

		/// <summary>Close HTML tag to return the browser so it will close itself.</summary>
		private const string ClosePageResponse =
			@"<html>
			  <head><title>OAuth 2.0 Authentication Token Received</title></head>
			  <body>
			    Received verification code. You may now close this window.
			    <script type='text/javascript'>
			      // This doesn't work on every browser.
			      window.setTimeout(function() {
			          window.open('', '_self', ''); 
			          window.close(); 
			        }, 1000);
			      if (window.opener) { window.opener.checkToken(); }
			    </script>
			  </body>
			</html>";

		private string redirectUri;

		public string RedirectUri
		{
			get
			{
				if (!string.IsNullOrEmpty(redirectUri))
				{
					return redirectUri;
				}

				return redirectUri = string.Format(LoopbackCallback, GetRandomUnusedPort());
			}
		}

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Google.Apis.Auth.OAuth2.MacBrowserCodeReciever"/> class.
		/// </summary>
		public MacBrowserCodeReciever ()
		{
			
		}
			
		#endregion
		#region ICodeReceiver implementation

		/// <summary>
		/// Receives the authorization code.
		/// </summary>
		/// <param name="url">The authorization code request URL</param>
		/// <param name="taskCancellationToken">Cancellation token</param>
		/// <returns>The authorization code response</returns>
		public Task<AuthorizationCodeResponseUrl> ReceiveCodeAsync (AuthorizationCodeRequestUrl url, CancellationToken taskCancellationToken)
		{
			var authorizationUrl = url.Build().ToString();

			var tcsk = new TaskCompletionSource<AuthorizationCodeResponseUrl> ();

			new NSObject ().BeginInvokeOnMainThread (async () => {

				var window = new MacAuthWindow(authorizationUrl, RedirectUri);

				var result = await window.ShowDialogAsync(NSApplication.SharedApplication.KeyWindow);

				tcsk.SetResult(result);

			});

			return tcsk.Task;
		}
			

		#endregion

		/// <summary>Returns a random, unused port.</summary>
		private static int GetRandomUnusedPort()
		{
			var listener = new TcpListener(IPAddress.Loopback, 0);
			try
			{
				listener.Start();
				return ((IPEndPoint)listener.LocalEndpoint).Port;
			}
			finally
			{
				listener.Stop();
			}
		}
			
	}
}

