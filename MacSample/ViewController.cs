using System;

using AppKit;
using Foundation;
using GoogleApiSample;

namespace MacSample
{
	public partial class ViewController : NSViewController
	{
		private GoogleAuthenticator mAuthenticator;

		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

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
	}
}
