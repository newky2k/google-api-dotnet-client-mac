using System;

using AppKit;
using Foundation;

namespace GoogleApiSample
{
	public partial class ViewController : NSViewController
	{
		//private GoogleAuthenticator mGoogleAuth;

		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			//mGoogleAuth = new GoogleAuthenticator ();

			// Do any additional setup after loading the view.
		}

		public override NSObject RepresentedObject {
			get {
				return base.RepresentedObject;
			}
			set {
				base.RepresentedObject = value;
				// Update the view, if already loaded.
			}
		}

		partial void didClickAuthenticate (NSObject sender)
		{
			//mGoogleAuth.Authenticate();
		}
	}
}
