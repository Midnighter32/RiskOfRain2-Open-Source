using System;
using System.Globalization;

// Token: 0x0200000F RID: 15
public struct CSteamID : IEquatable<CSteamID>
{
	// Token: 0x06000044 RID: 68 RVA: 0x00003E6F File Offset: 0x0000206F
	public CSteamID(ulong value)
	{
		this.value = value;
	}

	// Token: 0x06000045 RID: 69 RVA: 0x00003E78 File Offset: 0x00002078
	public static bool operator ==(CSteamID a, CSteamID b)
	{
		return a.value == b.value;
	}

	// Token: 0x06000046 RID: 70 RVA: 0x00003E88 File Offset: 0x00002088
	public static bool operator !=(CSteamID a, CSteamID b)
	{
		return a.value != b.value;
	}

	// Token: 0x06000047 RID: 71 RVA: 0x00003E9C File Offset: 0x0000209C
	public override bool Equals(object obj)
	{
		if (obj is CSteamID)
		{
			CSteamID csteamID = (CSteamID)obj;
			return csteamID.value == this.value;
		}
		return false;
	}

	// Token: 0x06000048 RID: 72 RVA: 0x00003E78 File Offset: 0x00002078
	public bool Equals(CSteamID other)
	{
		return this.value == other.value;
	}

	// Token: 0x06000049 RID: 73 RVA: 0x00003ECC File Offset: 0x000020CC
	public override int GetHashCode()
	{
		return this.value.GetHashCode();
	}

	// Token: 0x0600004A RID: 74 RVA: 0x00003EE7 File Offset: 0x000020E7
	public override string ToString()
	{
		return TextSerialization.ToStringInvariant(this.value);
	}

	// Token: 0x0600004B RID: 75 RVA: 0x00003EF4 File Offset: 0x000020F4
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
	// (get) Token: 0x0600004C RID: 76 RVA: 0x00003F3E File Offset: 0x0000213E
	public bool isValid
	{
		get
		{
			return this.value > 0UL;
		}
	}

	// Token: 0x0600004D RID: 77 RVA: 0x00003F4C File Offset: 0x0000214C
	private uint GetBitField(int bitOffset, int bitCount)
	{
		uint num = uint.MaxValue >> 32 - bitCount;
		return (uint)(this.value >> bitOffset) & num;
	}

	// Token: 0x1700000A RID: 10
	// (get) Token: 0x0600004E RID: 78 RVA: 0x00003F71 File Offset: 0x00002171
	public uint accountId
	{
		get
		{
			return this.GetBitField(0, 32);
		}
	}

	// Token: 0x1700000B RID: 11
	// (get) Token: 0x0600004F RID: 79 RVA: 0x00003F7C File Offset: 0x0000217C
	public uint accountInstance
	{
		get
		{
			return this.GetBitField(32, 20);
		}
	}

	// Token: 0x1700000C RID: 12
	// (get) Token: 0x06000050 RID: 80 RVA: 0x00003F88 File Offset: 0x00002188
	public CSteamID.EAccountType accountType
	{
		get
		{
			return (CSteamID.EAccountType)this.GetBitField(52, 4);
		}
	}

	// Token: 0x1700000D RID: 13
	// (get) Token: 0x06000051 RID: 81 RVA: 0x00003F93 File Offset: 0x00002193
	public CSteamID.EUniverse universe
	{
		get
		{
			return (CSteamID.EUniverse)this.GetBitField(56, 8);
		}
	}

	// Token: 0x1700000E RID: 14
	// (get) Token: 0x06000052 RID: 82 RVA: 0x00003F9E File Offset: 0x0000219E
	public bool isLobby
	{
		get
		{
			return this.accountType == CSteamID.EAccountType.k_EAccountTypeChat;
		}
	}

	// Token: 0x06000053 RID: 83 RVA: 0x00003FAC File Offset: 0x000021AC
	public string ToSteamID()
	{
		uint accountId = this.accountId;
		return string.Format(CultureInfo.InvariantCulture, "STEAM_{0}:{1}:{2}", (uint)this.universe, accountId & 1U, accountId >> 1);
	}

	// Token: 0x06000054 RID: 84 RVA: 0x00003FEC File Offset: 0x000021EC
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

	// Token: 0x04000063 RID: 99
	public readonly ulong value;

	// Token: 0x04000064 RID: 100
	public static readonly CSteamID nil;

	// Token: 0x02000010 RID: 16
	public enum EAccountType
	{
		// Token: 0x04000066 RID: 102
		k_EAccountTypeInvalid,
		// Token: 0x04000067 RID: 103
		k_EAccountTypeIndividual,
		// Token: 0x04000068 RID: 104
		k_EAccountTypeMultiseat,
		// Token: 0x04000069 RID: 105
		k_EAccountTypeGameServer,
		// Token: 0x0400006A RID: 106
		k_EAccountTypeAnonGameServer,
		// Token: 0x0400006B RID: 107
		k_EAccountTypePending,
		// Token: 0x0400006C RID: 108
		k_EAccountTypeContentServer,
		// Token: 0x0400006D RID: 109
		k_EAccountTypeClan,
		// Token: 0x0400006E RID: 110
		k_EAccountTypeChat,
		// Token: 0x0400006F RID: 111
		k_EAccountTypeConsoleUser,
		// Token: 0x04000070 RID: 112
		k_EAccountTypeAnonUser,
		// Token: 0x04000071 RID: 113
		k_EAccountTypeMax
	}

	// Token: 0x02000011 RID: 17
	public enum EUniverse
	{
		// Token: 0x04000073 RID: 115
		k_EUniverseInvalid,
		// Token: 0x04000074 RID: 116
		k_EUniversePublic,
		// Token: 0x04000075 RID: 117
		k_EUniverseBeta,
		// Token: 0x04000076 RID: 118
		k_EUniverseInternal,
		// Token: 0x04000077 RID: 119
		k_EUniverseDev,
		// Token: 0x04000078 RID: 120
		k_EUniverseMax
	}
}
