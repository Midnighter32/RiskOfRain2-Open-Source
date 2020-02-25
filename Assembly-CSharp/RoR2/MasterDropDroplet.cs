using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200027E RID: 638
	[RequireComponent(typeof(CharacterMaster))]
	public class MasterDropDroplet : MonoBehaviour
	{
		// Token: 0x06000E2A RID: 3626 RVA: 0x0003F2BA File Offset: 0x0003D4BA
		private void Start()
		{
			this.characterMaster = base.GetComponent<CharacterMaster>();
		}

		// Token: 0x06000E2B RID: 3627 RVA: 0x0003F2C8 File Offset: 0x0003D4C8
		public void DropItems()
		{
			CharacterBody body = this.characterMaster.GetBody();
			if (body)
			{
				SerializablePickupIndex[] array = this.pickupsToDrop;
				for (int i = 0; i < array.Length; i++)
				{
					PickupDropletController.CreatePickupDroplet(PickupIndex.Find(array[i].pickupName), body.coreTransform.position, new Vector3(UnityEngine.Random.Range(-4f, 4f), 20f, UnityEngine.Random.Range(-4f, 4f)));
				}
			}
		}

		// Token: 0x04000E17 RID: 3607
		private CharacterMaster characterMaster;

		// Token: 0x04000E18 RID: 3608
		public SerializablePickupIndex[] pickupsToDrop;
	}
}
