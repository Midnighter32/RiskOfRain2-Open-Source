using System;
using System.Globalization;

namespace RoR2.Networking
{
	// Token: 0x02000577 RID: 1399
	public struct AddressPortPair : IEquatable<AddressPortPair>
	{
		// Token: 0x06001F25 RID: 7973 RVA: 0x00092E22 File Offset: 0x00091022
		public AddressPortPair(string address, ushort port)
		{
			this.address = address;
			this.port = port;
		}

		// Token: 0x06001F26 RID: 7974 RVA: 0x00092E34 File Offset: 0x00091034
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

		// Token: 0x170002BA RID: 698
		// (get) Token: 0x06001F27 RID: 7975 RVA: 0x00092EB8 File Offset: 0x000910B8
		public bool isValid
		{
			get
			{
				return !string.IsNullOrEmpty(this.address);
			}
		}

		// Token: 0x06001F28 RID: 7976 RVA: 0x00092EC8 File Offset: 0x000910C8
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", this.address, this.port);
		}

		// Token: 0x06001F29 RID: 7977 RVA: 0x00092EEA File Offset: 0x000910EA
		public bool Equals(AddressPortPair other)
		{
			return string.Equals(this.address, other.address) && this.port == other.port;
		}

		// Token: 0x06001F2A RID: 7978 RVA: 0x00092F10 File Offset: 0x00091110
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

		// Token: 0x06001F2B RID: 7979 RVA: 0x00092F3C File Offset: 0x0009113C
		public override int GetHashCode()
		{
			return ((this.address != null) ? this.address.GetHashCode() : 0) * 397 ^ this.port.GetHashCode();
		}

		// Token: 0x040021D5 RID: 8661
		public string address;

		// Token: 0x040021D6 RID: 8662
		public ushort port;
	}
}
