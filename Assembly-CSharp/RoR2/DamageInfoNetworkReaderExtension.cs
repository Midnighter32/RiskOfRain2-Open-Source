using System;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000236 RID: 566
	internal static class DamageInfoNetworkReaderExtension
	{
		// Token: 0x06000ABD RID: 2749 RVA: 0x00035150 File Offset: 0x00033350
		public static DamageInfo ReadDamageInfo(this NetworkReader reader)
		{
			return new DamageInfo
			{
				damage = reader.ReadSingle(),
				crit = reader.ReadBoolean(),
				attacker = reader.ReadGameObject(),
				inflictor = reader.ReadGameObject(),
				position = reader.ReadVector3(),
				force = reader.ReadVector3(),
				procChainMask = reader.ReadProcChainMask(),
				procCoefficient = reader.ReadSingle(),
				damageType = (DamageType)reader.ReadByte(),
				damageColorIndex = (DamageColorIndex)reader.ReadByte()
			};
		}
	}
}
