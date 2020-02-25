using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003E3 RID: 995
	[Serializable]
	public struct SerializablePickupIndex
	{
		// Token: 0x0600183F RID: 6207 RVA: 0x00068FB2 File Offset: 0x000671B2
		public static explicit operator PickupIndex(SerializablePickupIndex serializablePickupIndex)
		{
			return PickupCatalog.FindPickupIndex(serializablePickupIndex.pickupName);
		}

		// Token: 0x040016CE RID: 5838
		[SerializeField]
		public string pickupName;
	}
}
