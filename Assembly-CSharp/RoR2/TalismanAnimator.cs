using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003F2 RID: 1010
	[RequireComponent(typeof(ItemFollower))]
	public class TalismanAnimator : MonoBehaviour
	{
		// Token: 0x06001617 RID: 5655 RVA: 0x00069B5C File Offset: 0x00067D5C
		private void Start()
		{
			this.itemFollower = base.GetComponent<ItemFollower>();
			if (this.itemFollower.followerInstance)
			{
				this.killEffects = this.itemFollower.followerInstance.GetComponentsInChildren<ParticleSystem>();
			}
			CharacterModel componentInParent = base.GetComponentInParent<CharacterModel>();
			if (componentInParent)
			{
				CharacterBody body = componentInParent.body;
				if (body)
				{
					this.equipmentSlot = body.equipmentSlot;
				}
			}
		}

		// Token: 0x06001618 RID: 5656 RVA: 0x00069BC8 File Offset: 0x00067DC8
		private void FixedUpdate()
		{
			if (this.equipmentSlot)
			{
				float cooldownTimer = this.equipmentSlot.cooldownTimer;
				if (this.lastCooldownTimer - cooldownTimer >= 0.5f)
				{
					ParticleSystem[] array = this.killEffects;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].Play();
					}
				}
				this.lastCooldownTimer = cooldownTimer;
			}
		}

		// Token: 0x04001975 RID: 6517
		private float lastCooldownTimer;

		// Token: 0x04001976 RID: 6518
		private EquipmentSlot equipmentSlot;

		// Token: 0x04001977 RID: 6519
		private ItemFollower itemFollower;

		// Token: 0x04001978 RID: 6520
		private ParticleSystem[] killEffects;
	}
}
