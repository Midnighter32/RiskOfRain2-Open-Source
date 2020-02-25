using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003D2 RID: 978
	[Serializable]
	public struct NetworkUserId : IEquatable<NetworkUserId>
	{
		// Token: 0x060017C4 RID: 6084 RVA: 0x00067236 File Offset: 0x00065436
		private NetworkUserId(ulong value, byte subId)
		{
			this.value = value;
			this.subId = subId;
		}

		// Token: 0x060017C5 RID: 6085 RVA: 0x00067246 File Offset: 0x00065446
		public static NetworkUserId FromIp(string ip, byte subId)
		{
			return new NetworkUserId((ulong)((long)ip.GetHashCode()), subId);
		}

		// Token: 0x060017C6 RID: 6086 RVA: 0x00067255 File Offset: 0x00065455
		public static NetworkUserId FromSteamId(ulong steamId, byte subId)
		{
			return new NetworkUserId(steamId, subId);
		}

		// Token: 0x060017C7 RID: 6087 RVA: 0x0006725E File Offset: 0x0006545E
		public bool Equals(NetworkUserId other)
		{
			return this.value == other.value && this.subId == other.subId;
		}

		// Token: 0x060017C8 RID: 6088 RVA: 0x0006727E File Offset: 0x0006547E
		public override bool Equals(object obj)
		{
			return obj != null && obj is NetworkUserId && this.Equals((NetworkUserId)obj);
		}

		// Token: 0x060017C9 RID: 6089 RVA: 0x0006729C File Offset: 0x0006549C
		public override int GetHashCode()
		{
			return this.value.GetHashCode() * 397 ^ this.subId.GetHashCode();
		}

		// Token: 0x170002C2 RID: 706
		// (get) Token: 0x060017CA RID: 6090 RVA: 0x000672CC File Offset: 0x000654CC
		public CSteamID steamId
		{
			get
			{
				return new CSteamID(this.value);
			}
		}

		// Token: 0x04001665 RID: 5733
		[SerializeField]
		public readonly ulong value;

		// Token: 0x04001666 RID: 5734
		[SerializeField]
		public readonly byte subId;
	}
}
