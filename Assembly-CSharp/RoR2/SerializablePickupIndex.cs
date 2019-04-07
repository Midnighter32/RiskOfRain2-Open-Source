using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000468 RID: 1128
	[Serializable]
	public struct SerializablePickupIndex
	{
		// Token: 0x06001946 RID: 6470 RVA: 0x0007921B File Offset: 0x0007741B
		public static explicit operator PickupIndex(SerializablePickupIndex serializablePickupIndex)
		{
			return PickupIndex.Find(serializablePickupIndex.pickupName);
		}

		// Token: 0x04001CBA RID: 7354
		[SerializeField]
		public string pickupName;
	}
}
