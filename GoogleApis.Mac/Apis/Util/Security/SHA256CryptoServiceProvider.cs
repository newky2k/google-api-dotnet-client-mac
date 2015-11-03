using System;
using System.Security.Cryptography;
using System.Security;

namespace Google.Apis.Util.Security
{
	public sealed class SHA256CryptoServiceProvider : SHA256 {

		static byte[] Empty = new byte [0];

		private SHA256 hash;

		[SecurityCritical]
		public SHA256CryptoServiceProvider ()
		{
			// note: we don't use SHA256.Create since CryptoConfig could, 
			// if set to use this class, result in a endless recursion
			hash = new SHA256Managed ();
		}

		[SecurityCritical]
		public override void Initialize ()
		{
			hash.Initialize ();
		}

		[SecurityCritical]
		protected override void HashCore (byte[] array, int ibStart, int cbSize)
		{
			hash.TransformBlock (array, ibStart, cbSize, null, 0);
		}

		[SecurityCritical]
		protected override byte[] HashFinal ()
		{
			hash.TransformFinalBlock (Empty, 0, 0);
			HashValue = hash.Hash;
			return HashValue;
		}

		[SecurityCritical]
		protected override void Dispose (bool disposing)
		{
			(hash as IDisposable).Dispose ();
			base.Dispose (disposing);
		}
	}
}

