using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000349 RID: 841
	[RequireComponent(typeof(ItemFollower))]
	public class TalismanAnimator : MonoBehaviour
	{
		// Token: 0x0600140C RID: 5132 RVA: 0x00055A94 File Offset: 0x00053C94
		private void Start()
		{
			this.itemFollower = base.GetComponent<ItemFollower>();
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

		// Token: 0x0600140D RID: 5133 RVA: 0x00055AD8 File Offset: 0x00053CD8
		private void FixedUpdate()
		{
			if (this.equipmentSlot)
			{
				float cooldownTimer = this.equipmentSlot.cooldownTimer;
				if (this.lastCooldownTimer - cooldownTimer >= 0.5f && this.itemFollower.followerInstance)
				{
					if (this.killEffects == null || this.killEffects.Length == 0 || this.killEffects[0] == null)
					{
						this.killEffects = this.itemFollower.followerInstance.GetComponentsInChildren<ParticleSystem>();
					}
					ParticleSystem[] array = this.killEffects;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].Play();
					}
				}
				this.lastCooldownTimer = cooldownTimer;
			}
		}

		// Token: 0x040012D3 RID: 4819
		private float lastCooldownTimer;

		// Token: 0x040012D4 RID: 4820
		private EquipmentSlot equipmentSlot;

		// Token: 0x040012D5 RID: 4821
		private ItemFollower itemFollower;

		// Token: 0x040012D6 RID: 4822
		private ParticleSystem[] killEffects;
	}
}
