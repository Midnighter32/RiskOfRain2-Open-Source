using System;
using System.Globalization;

// Token: 0x02000015 RID: 21
public struct CSteamID : IEquatable<CSteamID>
{
	// Token: 0x0600006D RID: 109 RVA: 0x000042DB File Offset: 0x000024DB
	public CSteamID(ulong value)
	{
		this.value = value;
	}

	// Token: 0x0600006E RID: 110 RVA: 0x000042E4 File Offset: 0x000024E4
	public static bool operator ==(CSteamID a, CSteamID b)
	{
		return a.value == b.value;
	}

	// Token: 0x0600006F RID: 111 RVA: 0x000042F4 File Offset: 0x000024F4
	public static bool operator !=(CSteamID a, CSteamID b)
	{
		return a.value != b.value;
	}

	// Token: 0x06000070 RID: 112 RVA: 0x00004308 File Offset: 0x00002508
	public override bool Equals(object obj)
	{
		if (obj is CSteamID)
		{
			CSteamID csteamID = (CSteamID)obj;
			return csteamID.value == this.value;
		}
		return false;
	}

	// Token: 0x06000071 RID: 113 RVA: 0x000042E4 File Offset: 0x000024E4
	public bool Equals(CSteamID other)
	{
		return this.value == other.value;
	}

	// Token: 0x06000072 RID: 114 RVA: 0x00004338 File Offset: 0x00002538
	public override int GetHashCode()
	{
		return this.value.GetHashCode();
	}

	// Token: 0x06000073 RID: 115 RVA: 0x00004353 File Offset: 0x00002553
	public override string ToString()
	{
		return TextSerialization.ToStringInvariant(this.value);
	}

	// Token: 0x06000074 RID: 116 RVA: 0x00004360 File Offset: 0x00002560
	public static bool TryParse(string str, out CSteamID result)
	{
		if (!string.IsNullOrEmpty(str) && str[0] <= '9')
		{
			ulong num;
			bool flag = TextSerialization.TryParseInvariant(str, out num);
			result = new CSteamID(flag ? num : 0UL);
			return flag;
		}
		result = CSteamID.nil;
		return false;
	}

	// Token: 0x17000009 RID: 9
	// (get) Token: 0x06000075 RID: 117 RVA: 0x000043AA File Offset: 0x000025AA
	public bool isValid
	{
		get
		{
			return this.value > 0UL;
		}
	}

	// Token: 0x06000076 RID: 118 RVA: 0x000043B8 File Offset: 0x000025B8
	private uint GetBitField(int bitOffset, int bitCount)
	{
		uint num = uint.MaxValue >> 32 - bitCount;
		return (uint)(this.value >> bitOffset) & num;
	}

	// Token: 0x1700000A RID: 10
	// (get) Token: 0x06000077 RID: 119 RVA: 0x000043DD File Offset: 0x000025DD
	public uint accountId
	{
		get
		{
			return this.GetBitField(0, 32);
		}
	}

	// Token: 0x1700000B RID: 11
	// (get) Token: 0x06000078 RID: 120 RVA: 0x000043E8 File Offset: 0x000025E8
	public uint accountInstance
	{
		get
		{
			return this.GetBitField(32, 20);
		}
	}

	// Token: 0x1700000C RID: 12
	// (get) Token: 0x06000079 RID: 121 RVA: 0x000043F4 File Offset: 0x000025F4
	public CSteamID.EAccountType accountType
	{
		get
		{
			return (CSteamID.EAccountType)this.GetBitField(52, 4);
		}
	}

	// Token: 0x1700000D RID: 13
	// (get) Token: 0x0600007A RID: 122 RVA: 0x000043FF File Offset: 0x000025FF
	public CSteamID.EUniverse universe
	{
		get
		{
			return (CSteamID.EUniverse)this.GetBitField(56, 8);
		}
	}

	// Token: 0x1700000E RID: 14
	// (get) Token: 0x0600007B RID: 123 RVA: 0x0000440A File Offset: 0x0000260A
	public bool isLobby
	{
		get
		{
			return this.accountType == CSteamID.EAccountType.k_EAccountTypeChat;
		}
	}

	// Token: 0x0600007C RID: 124 RVA: 0x00004418 File Offset: 0x00002618
	public string ToSteamID()
	{
		uint accountId = this.accountId;
		return string.Format(CultureInfo.InvariantCulture, "STEAM_{0}:{1}:{2}", (uint)this.universe, accountId & 1u, accountId >> 1);
	}

	// Token: 0x0600007D RID: 125 RVA: 0x00004458 File Offset: 0x00002658
	public string ToCommunityID()
	{
		char c = 'I';
		switch (this.accountType)
		{
		case CSteamID.EAccountType.k_EAccountTypeInvalid:
			c = 'I';
			break;
		case CSteamID.EAccountType.k_EAccountTypeIndividual:
			c = 'U';
			break;
		case CSteamID.EAccountType.k_EAccountTypeMultiseat:
			c = 'M';
			break;
		case CSteamID.EAccountType.k_EAccountTypeGameServer:
			c = 'G';
			break;
		case CSteamID.EAccountType.k_EAccountTypeAnonGameServer:
			c = 'A';
			break;
		case CSteamID.EAccountType.k_EAccountTypePending:
			c = 'P';
			break;
		case CSteamID.EAccountType.k_EAccountTypeContentServer:
			c = 'C';
			break;
		case CSteamID.EAccountType.k_EAccountTypeClan:
			c = 'g';
			break;
		case CSteamID.EAccountType.k_EAccountTypeChat:
			c = 'T';
			break;
		case CSteamID.EAccountType.k_EAccountTypeConsoleUser:
			c = 'I';
			break;
		case CSteamID.EAccountType.k_EAccountTypeAnonUser:
			c = 'a';
			break;
		case CSteamID.EAccountType.k_EAccountTypeMax:
			c = 'I';
			break;
		}
		return string.Format(CultureInfo.InvariantCulture, "[{0}:{1}:{2}", c, 1, this.accountId);
	}

	// Token: 0x04000068 RID: 104
	public readonly ulong value;

	// Token: 0x04000069 RID: 105
	public static readonly CSteamID nil;

	// Token: 0x02000016 RID: 22
	public enum EAccountType
	{
		// Token: 0x0400006B RID: 107
		k_EAccountTypeInvalid,
		// Token: 0x0400006C RID: 108
		k_EAccountTypeIndividual,
		// Token: 0x0400006D RID: 109
		k_EAccountTypeMultiseat,
		// Token: 0x0400006E RID: 110
		k_EAccountTypeGameServer,
		// Token: 0x0400006F RID: 111
		k_EAccountTypeAnonGameServer,
		// Token: 0x04000070 RID: 112
		k_EAccountTypePending,
		// Token: 0x04000071 RID: 113
		k_EAccountTypeContentServer,
		// Token: 0x04000072 RID: 114
		k_EAccountTypeClan,
		// Token: 0x04000073 RID: 115
		k_EAccountTypeChat,
		// Token: 0x04000074 RID: 116
		k_EAccountTypeConsoleUser,
		// Token: 0x04000075 RID: 117
		k_EAccountTypeAnonUser,
		// Token: 0x04000076 RID: 118
		k_EAccountTypeMax
	}

	// Token: 0x02000017 RID: 23
	public enum EUniverse
	{
		// Token: 0x04000078 RID: 120
		k_EUniverseInvalid,
		// Token: 0x04000079 RID: 121
		k_EUniversePublic,
		// Token: 0x0400007A RID: 122
		k_EUniverseBeta,
		// Token: 0x0400007B RID: 123
		k_EUniverseInternal,
		// Token: 0x0400007C RID: 124
		k_EUniverseDev,
		// Token: 0x0400007D RID: 125
		k_EUniverseMax
	}
}
