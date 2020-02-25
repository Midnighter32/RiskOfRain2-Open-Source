using System;
using System.Globalization;

namespace RoR2.Networking
{
	// Token: 0x02000543 RID: 1347
	public struct AddressPortPair : IEquatable<AddressPortPair>
	{
		// Token: 0x06001FC7 RID: 8135 RVA: 0x00089F06 File Offset: 0x00088106
		public AddressPortPair(string address, ushort port)
		{
			this.address = address;
			this.port = port;
		}

		// Token: 0x06001FC8 RID: 8136 RVA: 0x00089F18 File Offset: 0x00088118
		public static bool TryParse(string str, out AddressPortPair addressPortPair)
		{
			if (!string.IsNullOrEmpty(str))
			{
				int num = str.Length - 1;
				while (num >= 0 && str[num] != ':')
				{
					num--;
				}
				if (num >= 0)
				{
					string text = str.Substring(0, num);
					string s = str.Substring(num + 1, str.Length - num - 1);
					addressPortPair.address = text;
					ushort num2;
					addressPortPair.port = (TextSerialization.TryParseInvariant(s, out num2) ? num2 : 0);
					return true;
				}
			}
			addressPortPair.address = "";
			addressPortPair.port = 0;
			return false;
		}

		// Token: 0x17000358 RID: 856
		// (get) Token: 0x06001FC9 RID: 8137 RVA: 0x00089F9C File Offset: 0x0008819C
		public bool isValid
		{
			get
			{
				return !string.IsNullOrEmpty(this.address);
			}
		}

		// Token: 0x06001FCA RID: 8138 RVA: 0x00089FAC File Offset: 0x000881AC
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", this.address, this.port);
		}

		// Token: 0x06001FCB RID: 8139 RVA: 0x00089FCE File Offset: 0x000881CE
		public bool Equals(AddressPortPair other)
		{
			return string.Equals(this.address, other.address) && this.port == other.port;
		}

		// Token: 0x06001FCC RID: 8140 RVA: 0x00089FF4 File Offset: 0x000881F4
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is AddressPortPair)
			{
				AddressPortPair other = (AddressPortPair)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06001FCD RID: 8141 RVA: 0x0008A020 File Offset: 0x00088220
		public override int GetHashCode()
		{
			return ((this.address != null) ? this.address.GetHashCode() : 0) * 397 ^ this.port.GetHashCode();
		}

		// Token: 0x04001D71 RID: 7537
		public string address;

		// Token: 0x04001D72 RID: 7538
		public ushort port;
	}
}
