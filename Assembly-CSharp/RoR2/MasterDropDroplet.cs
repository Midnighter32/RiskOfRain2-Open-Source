using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000354 RID: 852
	[RequireComponent(typeof(CharacterMaster))]
	public class MasterDropDroplet : MonoBehaviour
	{
		// Token: 0x0600118F RID: 4495 RVA: 0x00057180 File Offset: 0x00055380
		private void Start()
		{
			this.characterMaster = base.GetComponent<CharacterMaster>();
		}

		// Token: 0x06001190 RID: 4496 RVA: 0x00057190 File Offset: 0x00055390
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

		// Token: 0x0400159A RID: 5530
		private CharacterMaster characterMaster;

		// Token: 0x0400159B RID: 5531
		public SerializablePickupIndex[] pickupsToDrop;
	}
}
