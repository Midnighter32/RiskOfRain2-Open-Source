using System;
using RoR2;
using UnityEngine;

namespace EntityStates.ClayBruiser.Weapon
{
	// Token: 0x020008C9 RID: 2249
	public class MinigunSpinUp : MinigunState
	{
		// Token: 0x0600326F RID: 12911 RVA: 0x000DA170 File Offset: 0x000D8370
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = MinigunSpinUp.baseDuration / this.attackSpeedStat;
			Util.PlaySound(MinigunSpinUp.sound, base.gameObject);
			base.GetModelAnimator().SetBool("WeaponIsReady", true);
			if (this.muzzleTransform && MinigunSpinUp.chargeEffectPrefab)
			{
				this.chargeInstance = UnityEngine.Object.Instantiate<GameObject>(MinigunSpinUp.chargeEffectPrefab, this.muzzleTransform.position, this.muzzleTransform.rotation);
				this.chargeInstance.transform.parent = this.muzzleTransform;
				ScaleParticleSystemDuration component = this.chargeInstance.GetComponent<ScaleParticleSystemDuration>();
				if (component)
				{
					component.newDuration = this.duration;
				}
			}
		}

		// Token: 0x06003270 RID: 12912 RVA: 0x000DA22C File Offset: 0x000D842C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextState(new MinigunFire());
			}
		}

		// Token: 0x06003271 RID: 12913 RVA: 0x000DA25A File Offset: 0x000D845A
		public override void OnExit()
		{
			base.OnExit();
			if (this.chargeInstance)
			{
				EntityState.Destroy(this.chargeInstance);
			}
		}

		// Token: 0x04003164 RID: 12644
		public static float baseDuration;

		// Token: 0x04003165 RID: 12645
		public static string sound;

		// Token: 0x04003166 RID: 12646
		public static GameObject chargeEffectPrefab;

		// Token: 0x04003167 RID: 12647
		private GameObject chargeInstance;

		// Token: 0x04003168 RID: 12648
		private float duration;
	}
}
