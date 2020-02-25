using System;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000119 RID: 281
	internal static class DamageInfoNetworkWriterExtension
	{
		// Token: 0x06000520 RID: 1312 RVA: 0x000147D8 File Offset: 0x000129D8
		public static void Write(this NetworkWriter writer, DamageInfo damageInfo)
		{
			writer.Write(damageInfo.damage);
			writer.Write(damageInfo.crit);
			writer.Write(damageInfo.attacker);
			writer.Write(damageInfo.inflictor);
			writer.Write(damageInfo.position);
			writer.Write(damageInfo.force);
			writer.Write(damageInfo.procChainMask);
			writer.Write(damageInfo.procCoefficient);
			writer.Write((byte)damageInfo.damageType);
			writer.Write((byte)damageInfo.damageColorIndex);
			writer.Write((byte)(damageInfo.dotIndex + 1));
		}
	}
}
