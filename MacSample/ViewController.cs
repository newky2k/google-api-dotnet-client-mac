using System;

using AppKit;
using Foundation;
using System.Threading.Tasks;

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
			mAuthenticator = new GoogleAuthenticator();
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

		partial void OnClickedAuthorise (AppKit.NSButton sender)
		{
			
			mAuthenticator.Authenticate();


		}
	}
}
