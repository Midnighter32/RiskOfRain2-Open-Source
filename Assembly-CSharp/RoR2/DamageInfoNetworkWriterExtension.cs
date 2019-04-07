using System;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000235 RID: 565
	internal static class DamageInfoNetworkWriterExtension
	{
		// Token: 0x06000ABC RID: 2748 RVA: 0x000350C8 File Offset: 0x000332C8
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
		}
	}
}
