using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Vulture.Weapon
{
	// Token: 0x0200073D RID: 1853
	public class ChargeWindblade : BaseSkillState
	{
		// Token: 0x06002B04 RID: 11012 RVA: 0x000B5194 File Offset: 0x000B3394
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = ChargeWindblade.baseDuration / this.attackSpeedStat;
			base.PlayAnimation("Gesture, Additive", "ChargeWindblade", "ChargeWindblade.playbackRate", this.duration);
			Util.PlaySound(ChargeWindblade.soundString, base.gameObject);
			base.characterBody.SetAimTimer(3f);
			Transform transform = base.FindModelChild(ChargeWindblade.muzzleString);
			if (transform && ChargeWindblade.chargeEffectPrefab)
			{
				this.chargeEffectInstance = UnityEngine.Object.Instantiate<GameObject>(ChargeWindblade.chargeEffectPrefab, transform.position, transform.rotation);
				this.chargeEffectInstance.transform.parent = transform;
				ScaleParticleSystemDuration component = this.chargeEffectInstance.GetComponent<ScaleParticleSystemDuration>();
				if (component)
				{
					component.newDuration = this.duration;
				}
			}
		}

		// Token: 0x06002B05 RID: 11013 RVA: 0x000B5262 File Offset: 0x000B3462
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration)
			{
				this.outer.SetNextState(new FireWindblade());
			}
		}

		// Token: 0x06002B06 RID: 11014 RVA: 0x000B5288 File Offset: 0x000B3488
		public override void OnExit()
		{
			if (this.chargeEffectInstance)
			{
				EntityState.Destroy(this.chargeEffectInstance);
			}
			base.OnExit();
		}

		// Token: 0x040026DB RID: 9947
		public static float baseDuration;

		// Token: 0x040026DC RID: 9948
		public static string muzzleString;

		// Token: 0x040026DD RID: 9949
		public static GameObject chargeEffectPrefab;

		// Token: 0x040026DE RID: 9950
		public static string soundString;

		// Token: 0x040026DF RID: 9951
		private float duration;

		// Token: 0x040026E0 RID: 9952
		private GameObject chargeEffectInstance;
	}
}
