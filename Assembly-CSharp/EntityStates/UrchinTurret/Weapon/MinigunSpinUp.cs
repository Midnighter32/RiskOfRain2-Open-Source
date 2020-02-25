using System;
using RoR2;
using UnityEngine;

namespace EntityStates.UrchinTurret.Weapon
{
	// Token: 0x02000906 RID: 2310
	public class MinigunSpinUp : MinigunState
	{
		// Token: 0x0600338D RID: 13197 RVA: 0x000DFC24 File Offset: 0x000DDE24
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = MinigunSpinUp.baseDuration / this.attackSpeedStat;
			Util.PlaySound(MinigunSpinUp.sound, base.gameObject);
			base.PlayCrossfade("Gesture, Additive", "EnterShootLoop", 0.2f);
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

		// Token: 0x0600338E RID: 13198 RVA: 0x000DFCE4 File Offset: 0x000DDEE4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextState(new MinigunFire());
			}
		}

		// Token: 0x0600338F RID: 13199 RVA: 0x000DFD12 File Offset: 0x000DDF12
		public override void OnExit()
		{
			base.OnExit();
			if (this.chargeInstance)
			{
				EntityState.Destroy(this.chargeInstance);
			}
		}

		// Token: 0x04003309 RID: 13065
		public static float baseDuration;

		// Token: 0x0400330A RID: 13066
		public static string sound;

		// Token: 0x0400330B RID: 13067
		public static GameObject chargeEffectPrefab;

		// Token: 0x0400330C RID: 13068
		private GameObject chargeInstance;

		// Token: 0x0400330D RID: 13069
		private float duration;
	}
}
