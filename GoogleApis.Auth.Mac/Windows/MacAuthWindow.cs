using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using CoreGraphics;
using WebKit;
using System.Net;
using System.Net.Sockets;
using System.Collections.Specialized;
using System.IO;
using Google.Apis.Auth.OAuth2.Responses;
using System.Threading.Tasks;

namespace Google.Apis.Auth.Windows
{
	public partial class MacAuthWindow : AppKit.NSWindow
	{
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
	

		#region  Fields
		private WebView mWebView;
		private String mUrl;
		private String mRedirectUrl;
		private HttpListener mlistener;
		private TaskCompletionSource<AuthorizationCodeResponseUrl> mTask;
		private AuthorizationCodeResponseUrl mItem;
		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Google.Apis.Auth.Windows.MacAuthWindow"/> class.
		/// </summary>
		/// <param name="url">URL.</param>
		public MacAuthWindow (String url, String redirectUrl) 
			: base(new CGRect(0,0,480,640), NSWindowStyle.Closable | NSWindowStyle.Titled | NSWindowStyle.Utility, NSBackingStore.Buffered, false)
		{
			mUrl = url;
			mRedirectUrl = redirectUrl;

			Initialize ();

			this.WeakDelegate = this;
		}


		// Called when created from unmanaged code
		public MacAuthWindow (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MacAuthWindow (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Shared initialization code
		private void Initialize ()
		{
			mWebView = new WebView(new CGRect(0,0,480,640),"Gps","Gps");
			this.ContentView.AddSubview(mWebView);

			var aUri = new Uri(mUrl);

			mWebView.ReceivedTitle += (object sender, WebFrameTitleEventArgs e) => 
			{
				this.Title = e.Title;
			};

			mWebView.WillPerformClientRedirect += (object sender, WebFrameClientRedirectEventArgs e) => 
			{
				Console.WriteLine("");
			};
				
			//NSUrl aNsUrl = (NSUrl)aUri;
			mWebView.MainFrame.LoadRequest(new NSUrlRequest((NSUrl)aUri));

			SetupListener ();
		}

		[Export("windowWillClose:")]
		public void WindowWillClose(NSNotification notif)
		{
			CloseListener ();

			mTask.SetResult (mItem);
		}

		private async void SetupListener()
		{
			mlistener = new HttpListener();
			mlistener.Prefixes.Add(mRedirectUrl);
			mlistener.Start();

			var context = await mlistener.GetContextAsync().ConfigureAwait(false);
			NameValueCollection coll = context.Request.QueryString;

			// Write a "close" response.
			using (var writer = new StreamWriter(context.Response.OutputStream))
			{
				writer.WriteLine(ClosePageResponse);
				writer.Flush();
			}

			context.Response.OutputStream.Close();

			mItem = new AuthorizationCodeResponseUrl(coll.AllKeys.ToDictionary(k => k, k => coll[k]));
		}

		private void CloseListener()
		{
			mlistener.Close ();

			mlistener = null;
		}

		/// <summary>
		/// Shows the dialog asyncronously
		/// </summary>
		/// <returns>The dialog async.</returns>
		/// <param name="owner">Owner.</param>
		public Task<AuthorizationCodeResponseUrl> ShowDialogAsync (NSWindow owner)
		{
			mTask = new TaskCompletionSource<AuthorizationCodeResponseUrl> ();

			owner.BeginInvokeOnMainThread (() => {
					
				this.ParentWindow = owner;
				this.MakeKeyAndOrderFront(owner);
				this.Center ();

			});


			return mTask.Task;
		}
			
		#endregion

	}

	public class FrameInterceptor : IWebFrameLoadDelegate
	{
		#region IDisposable implementation
		public void Dispose ()
		{
			throw new NotImplementedException ();
		}
		#endregion
		#region INativeObject implementation
		public IntPtr Handle {
			get {
				throw new NotImplementedException ();
			}
		}
		#endregion
		
	}
}
