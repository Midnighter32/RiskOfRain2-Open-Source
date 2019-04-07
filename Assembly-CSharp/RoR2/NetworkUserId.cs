using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200045B RID: 1115
	[Serializable]
	public struct NetworkUserId : IEquatable<NetworkUserId>
	{
		// Token: 0x060018EC RID: 6380 RVA: 0x0007793B File Offset: 0x00075B3B
		private NetworkUserId(ulong value, byte subId)
		{
			this.value = value;
			this.subId = subId;
		}

		// Token: 0x060018ED RID: 6381 RVA: 0x0007794B File Offset: 0x00075B4B
		public static NetworkUserId FromIp(string ip, byte subId)
		{
			return new NetworkUserId((ulong)((long)ip.GetHashCode()), subId);
		}

		// Token: 0x060018EE RID: 6382 RVA: 0x0007795A File Offset: 0x00075B5A
		public static NetworkUserId FromSteamId(ulong steamId, byte subId)
		{
			return new NetworkUserId(steamId, subId);
		}

		// Token: 0x060018EF RID: 6383 RVA: 0x00077963 File Offset: 0x00075B63
		public bool Equals(NetworkUserId other)
		{
			return this.value == other.value && this.subId == other.subId;
		}

		// Token: 0x060018F0 RID: 6384 RVA: 0x00077983 File Offset: 0x00075B83
		public override bool Equals(object obj)
		{
			return obj != null && obj is NetworkUserId && this.Equals((NetworkUserId)obj);
		}

		// Token: 0x060018F1 RID: 6385 RVA: 0x000779A0 File Offset: 0x00075BA0
		public override int GetHashCode()
		{
			return this.value.GetHashCode() * 397 ^ this.subId.GetHashCode();
		}

		// Token: 0x04001C63 RID: 7267
		[SerializeField]
		public readonly ulong value;

		// Token: 0x04001C64 RID: 7268
		[SerializeField]
		public readonly byte subId;
	}
}
