using System;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200011A RID: 282
	internal static class DamageInfoNetworkReaderExtension
	{
		// Token: 0x06000521 RID: 1313 RVA: 0x00014870 File Offset: 0x00012A70
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
				damageColorIndex = (DamageColorIndex)reader.ReadByte(),
				dotIndex = (DotController.DotIndex)(reader.ReadByte() - 1)
			};
		}
	}
}
