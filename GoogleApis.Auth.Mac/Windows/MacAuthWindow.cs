using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using CoreGraphics;
using WebKit;

namespace Google.Apis.Auth.Windows
{
	public partial class MacAuthWindow : AppKit.NSWindow
	{
		private WebView mWebView;
		private String mUrl;
		#region Constructors

		public MacAuthWindow (String url) 
			: base(new CGRect(0,0,480,640), NSWindowStyle.Closable | NSWindowStyle.Titled | NSWindowStyle.Utility, NSBackingStore.Buffered, false)
		{
			mUrl = url;

			Initialize ();
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
		void Initialize ()
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
