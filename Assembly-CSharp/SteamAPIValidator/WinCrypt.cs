using System;
using System.Runtime.InteropServices;

namespace SteamAPIValidator
{
	// Token: 0x02000093 RID: 147
	internal static class WinCrypt
	{
		// Token: 0x06000335 RID: 821
		[DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool CryptQueryObject(int dwObjectType, IntPtr pvObject, int dwExpectedContentTypeFlags, int dwExpectedFormatTypeFlags, int dwFlags, out int pdwMsgAndCertEncodingType, out int pdwContentType, out int pdwFormatType, ref IntPtr phCertStore, ref IntPtr phMsg, ref IntPtr ppvContext);

		// Token: 0x06000336 RID: 822
		[DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool CryptMsgGetParam(IntPtr hCryptMsg, int dwParamType, int dwIndex, IntPtr pvData, ref int pcbData);

		// Token: 0x06000337 RID: 823
		[DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool CryptMsgGetParam(IntPtr hCryptMsg, int dwParamType, int dwIndex, [In] [Out] byte[] vData, ref int pcbData);

		// Token: 0x06000338 RID: 824
		[DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CryptDecodeObject(uint CertEncodingType, UIntPtr lpszStructType, byte[] pbEncoded, uint cbEncoded, uint flags, [In] [Out] byte[] pvStructInfo, ref uint cbStructInfo);

		// Token: 0x04000255 RID: 597
		public const int CRYPT_ASN_ENCODING = 1;

		// Token: 0x04000256 RID: 598
		public const int CRYPT_NDR_ENCODING = 2;

		// Token: 0x04000257 RID: 599
		public const int X509_ASN_ENCODING = 1;

		// Token: 0x04000258 RID: 600
		public const int X509_NDR_ENCODING = 2;

		// Token: 0x04000259 RID: 601
		public const int PKCS_7_ASN_ENCODING = 65536;

		// Token: 0x0400025A RID: 602
		public const int PKCS_7_NDR_ENCODING = 131072;

		// Token: 0x0400025B RID: 603
		public static UIntPtr PKCS7_SIGNER_INFO = new UIntPtr(500U);

		// Token: 0x0400025C RID: 604
		public static UIntPtr CMS_SIGNER_INFO = new UIntPtr(501U);

		// Token: 0x0400025D RID: 605
		public static string szOID_RSA_signingTime = "1.2.840.113549.1.9.5";

		// Token: 0x0400025E RID: 606
		public static string szOID_RSA_counterSign = "1.2.840.113549.1.9.6";

		// Token: 0x0400025F RID: 607
		public const int CMSG_TYPE_PARAM = 1;

		// Token: 0x04000260 RID: 608
		public const int CMSG_CONTENT_PARAM = 2;

		// Token: 0x04000261 RID: 609
		public const int CMSG_BARE_CONTENT_PARAM = 3;

		// Token: 0x04000262 RID: 610
		public const int CMSG_INNER_CONTENT_TYPE_PARAM = 4;

		// Token: 0x04000263 RID: 611
		public const int CMSG_SIGNER_COUNT_PARAM = 5;

		// Token: 0x04000264 RID: 612
		public const int CMSG_SIGNER_INFO_PARAM = 6;

		// Token: 0x04000265 RID: 613
		public const int CMSG_SIGNER_CERT_INFO_PARAM = 7;

		// Token: 0x04000266 RID: 614
		public const int CMSG_SIGNER_HASH_ALGORITHM_PARAM = 8;

		// Token: 0x04000267 RID: 615
		public const int CMSG_SIGNER_AUTH_ATTR_PARAM = 9;

		// Token: 0x04000268 RID: 616
		public const int CMSG_SIGNER_UNAUTH_ATTR_PARAM = 10;

		// Token: 0x04000269 RID: 617
		public const int CMSG_CERT_COUNT_PARAM = 11;

		// Token: 0x0400026A RID: 618
		public const int CMSG_CERT_PARAM = 12;

		// Token: 0x0400026B RID: 619
		public const int CMSG_CRL_COUNT_PARAM = 13;

		// Token: 0x0400026C RID: 620
		public const int CMSG_CRL_PARAM = 14;

		// Token: 0x0400026D RID: 621
		public const int CMSG_ENVELOPE_ALGORITHM_PARAM = 15;

		// Token: 0x0400026E RID: 622
		public const int CMSG_RECIPIENT_COUNT_PARAM = 17;

		// Token: 0x0400026F RID: 623
		public const int CMSG_RECIPIENT_INDEX_PARAM = 18;

		// Token: 0x04000270 RID: 624
		public const int CMSG_RECIPIENT_INFO_PARAM = 19;

		// Token: 0x04000271 RID: 625
		public const int CMSG_HASH_ALGORITHM_PARAM = 20;

		// Token: 0x04000272 RID: 626
		public const int CMSG_HASH_DATA_PARAM = 21;

		// Token: 0x04000273 RID: 627
		public const int CMSG_COMPUTED_HASH_PARAM = 22;

		// Token: 0x04000274 RID: 628
		public const int CMSG_ENCRYPT_PARAM = 26;

		// Token: 0x04000275 RID: 629
		public const int CMSG_ENCRYPTED_DIGEST = 27;

		// Token: 0x04000276 RID: 630
		public const int CMSG_ENCODED_SIGNER = 28;

		// Token: 0x04000277 RID: 631
		public const int CMSG_ENCODED_MESSAGE = 29;

		// Token: 0x04000278 RID: 632
		public const int CMSG_VERSION_PARAM = 30;

		// Token: 0x04000279 RID: 633
		public const int CMSG_ATTR_CERT_COUNT_PARAM = 31;

		// Token: 0x0400027A RID: 634
		public const int CMSG_ATTR_CERT_PARAM = 32;

		// Token: 0x0400027B RID: 635
		public const int CMSG_CMS_RECIPIENT_COUNT_PARAM = 33;

		// Token: 0x0400027C RID: 636
		public const int CMSG_CMS_RECIPIENT_INDEX_PARAM = 34;

		// Token: 0x0400027D RID: 637
		public const int CMSG_CMS_RECIPIENT_ENCRYPTED_KEY_INDEX_PARAM = 35;

		// Token: 0x0400027E RID: 638
		public const int CMSG_CMS_RECIPIENT_INFO_PARAM = 36;

		// Token: 0x0400027F RID: 639
		public const int CMSG_UNPROTECTED_ATTR_PARAM = 37;

		// Token: 0x04000280 RID: 640
		public const int CMSG_SIGNER_CERT_ID_PARAM = 38;

		// Token: 0x04000281 RID: 641
		public const int CMSG_CMS_SIGNER_INFO_PARAM = 39;

		// Token: 0x04000282 RID: 642
		public const int CERT_QUERY_OBJECT_FILE = 1;

		// Token: 0x04000283 RID: 643
		public const int CERT_QUERY_OBJECT_BLOB = 2;

		// Token: 0x04000284 RID: 644
		public const int CERT_QUERY_CONTENT_CERT = 1;

		// Token: 0x04000285 RID: 645
		public const int CERT_QUERY_CONTENT_CTL = 2;

		// Token: 0x04000286 RID: 646
		public const int CERT_QUERY_CONTENT_CRL = 3;

		// Token: 0x04000287 RID: 647
		public const int CERT_QUERY_CONTENT_SERIALIZED_STORE = 4;

		// Token: 0x04000288 RID: 648
		public const int CERT_QUERY_CONTENT_SERIALIZED_CERT = 5;

		// Token: 0x04000289 RID: 649
		public const int CERT_QUERY_CONTENT_SERIALIZED_CTL = 6;

		// Token: 0x0400028A RID: 650
		public const int CERT_QUERY_CONTENT_SERIALIZED_CRL = 7;

		// Token: 0x0400028B RID: 651
		public const int CERT_QUERY_CONTENT_PKCS7_SIGNED = 8;

		// Token: 0x0400028C RID: 652
		public const int CERT_QUERY_CONTENT_PKCS7_UNSIGNED = 9;

		// Token: 0x0400028D RID: 653
		public const int CERT_QUERY_CONTENT_PKCS7_SIGNED_EMBED = 10;

		// Token: 0x0400028E RID: 654
		public const int CERT_QUERY_CONTENT_PKCS10 = 11;

		// Token: 0x0400028F RID: 655
		public const int CERT_QUERY_CONTENT_PFX = 12;

		// Token: 0x04000290 RID: 656
		public const int CERT_QUERY_CONTENT_CERT_PAIR = 13;

		// Token: 0x04000291 RID: 657
		public const int CERT_QUERY_CONTENT_FLAG_CERT = 2;

		// Token: 0x04000292 RID: 658
		public const int CERT_QUERY_CONTENT_FLAG_CTL = 4;

		// Token: 0x04000293 RID: 659
		public const int CERT_QUERY_CONTENT_FLAG_CRL = 8;

		// Token: 0x04000294 RID: 660
		public const int CERT_QUERY_CONTENT_FLAG_SERIALIZED_STORE = 16;

		// Token: 0x04000295 RID: 661
		public const int CERT_QUERY_CONTENT_FLAG_SERIALIZED_CERT = 32;

		// Token: 0x04000296 RID: 662
		public const int CERT_QUERY_CONTENT_FLAG_SERIALIZED_CTL = 64;

		// Token: 0x04000297 RID: 663
		public const int CERT_QUERY_CONTENT_FLAG_SERIALIZED_CRL = 128;

		// Token: 0x04000298 RID: 664
		public const int CERT_QUERY_CONTENT_FLAG_PKCS7_SIGNED = 256;

		// Token: 0x04000299 RID: 665
		public const int CERT_QUERY_CONTENT_FLAG_PKCS7_UNSIGNED = 512;

		// Token: 0x0400029A RID: 666
		public const int CERT_QUERY_CONTENT_FLAG_PKCS7_SIGNED_EMBED = 1024;

		// Token: 0x0400029B RID: 667
		public const int CERT_QUERY_CONTENT_FLAG_PKCS10 = 2048;

		// Token: 0x0400029C RID: 668
		public const int CERT_QUERY_CONTENT_FLAG_PFX = 4096;

		// Token: 0x0400029D RID: 669
		public const int CERT_QUERY_CONTENT_FLAG_CERT_PAIR = 8192;

		// Token: 0x0400029E RID: 670
		public const int CERT_QUERY_CONTENT_FLAG_ALL = 16382;

		// Token: 0x0400029F RID: 671
		public const int CERT_QUERY_FORMAT_BINARY = 1;

		// Token: 0x040002A0 RID: 672
		public const int CERT_QUERY_FORMAT_BASE64_ENCODED = 2;

		// Token: 0x040002A1 RID: 673
		public const int CERT_QUERY_FORMAT_ASN_ASCII_HEX_ENCODED = 3;

		// Token: 0x040002A2 RID: 674
		public const int CERT_QUERY_FORMAT_FLAG_BINARY = 2;

		// Token: 0x040002A3 RID: 675
		public const int CERT_QUERY_FORMAT_FLAG_BASE64_ENCODED = 4;

		// Token: 0x040002A4 RID: 676
		public const int CERT_QUERY_FORMAT_FLAG_ASN_ASCII_HEX_ENCODED = 8;

		// Token: 0x040002A5 RID: 677
		public const int CERT_QUERY_FORMAT_FLAG_ALL = 14;

		// Token: 0x02000094 RID: 148
		public struct BLOB
		{
			// Token: 0x040002A6 RID: 678
			public int cbData;

			// Token: 0x040002A7 RID: 679
			public IntPtr pbData;
		}

		// Token: 0x02000095 RID: 149
		public struct CRYPT_ALGORITHM_IDENTIFIER
		{
			// Token: 0x040002A8 RID: 680
			public string pszObjId;

			// Token: 0x040002A9 RID: 681
			private WinCrypt.BLOB Parameters;
		}

		// Token: 0x02000096 RID: 150
		public struct CERT_ID
		{
			// Token: 0x040002AA RID: 682
			public int dwIdChoice;

			// Token: 0x040002AB RID: 683
			public WinCrypt.BLOB IssuerSerialNumberOrKeyIdOrHashId;
		}

		// Token: 0x02000097 RID: 151
		public struct SIGNER_SUBJECT_INFO
		{
			// Token: 0x040002AC RID: 684
			public uint cbSize;

			// Token: 0x040002AD RID: 685
			public IntPtr pdwIndex;

			// Token: 0x040002AE RID: 686
			public uint dwSubjectChoice;

			// Token: 0x040002AF RID: 687
			public WinCrypt.SubjectChoiceUnion Union1;
		}

		// Token: 0x02000098 RID: 152
		[StructLayout(LayoutKind.Explicit)]
		public struct SubjectChoiceUnion
		{
			// Token: 0x040002B0 RID: 688
			[FieldOffset(0)]
			public IntPtr pSignerFileInfo;

			// Token: 0x040002B1 RID: 689
			[FieldOffset(0)]
			public IntPtr pSignerBlobInfo;
		}

		// Token: 0x02000099 RID: 153
		public struct CERT_NAME_BLOB
		{
			// Token: 0x040002B2 RID: 690
			public uint cbData;

			// Token: 0x040002B3 RID: 691
			[MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)]
			public byte[] pbData;
		}

		// Token: 0x0200009A RID: 154
		public struct CRYPT_INTEGER_BLOB
		{
			// Token: 0x040002B4 RID: 692
			public uint cbData;

			// Token: 0x040002B5 RID: 693
			public IntPtr pbData;
		}

		// Token: 0x0200009B RID: 155
		public struct CRYPT_ATTR_BLOB
		{
			// Token: 0x040002B6 RID: 694
			public uint cbData;

			// Token: 0x040002B7 RID: 695
			[MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)]
			public byte[] pbData;
		}

		// Token: 0x0200009C RID: 156
		public struct CRYPT_ATTRIBUTE
		{
			// Token: 0x040002B8 RID: 696
			[MarshalAs(UnmanagedType.LPStr)]
			public string pszObjId;

			// Token: 0x040002B9 RID: 697
			public uint cValue;

			// Token: 0x040002BA RID: 698
			[MarshalAs(UnmanagedType.LPStruct)]
			public WinCrypt.CRYPT_ATTR_BLOB rgValue;
		}

		// Token: 0x0200009D RID: 157
		public struct CMSG_SIGNER_INFO
		{
			// Token: 0x040002BB RID: 699
			public int dwVersion;

			// Token: 0x040002BC RID: 700
			private WinCrypt.CERT_NAME_BLOB Issuer;

			// Token: 0x040002BD RID: 701
			private WinCrypt.CRYPT_INTEGER_BLOB SerialNumber;

			// Token: 0x040002BE RID: 702
			private WinCrypt.CRYPT_ALGORITHM_IDENTIFIER HashAlgorithm;

			// Token: 0x040002BF RID: 703
			private WinCrypt.CRYPT_ALGORITHM_IDENTIFIER HashEncryptionAlgorithm;

			// Token: 0x040002C0 RID: 704
			private WinCrypt.BLOB EncryptedHash;

			// Token: 0x040002C1 RID: 705
			private WinCrypt.CRYPT_ATTRIBUTE[] AuthAttrs;

			// Token: 0x040002C2 RID: 706
			private WinCrypt.CRYPT_ATTRIBUTE[] UnauthAttrs;
		}
	}
}
