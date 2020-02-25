using System;
using RoR2;
using UnityEngine;

namespace EntityStates.ScavMonster
{
	// Token: 0x0200078E RID: 1934
	public class PrepEnergyCannon : EnergyCannonState
	{
		// Token: 0x06002C62 RID: 11362 RVA: 0x000BB3FC File Offset: 0x000B95FC
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = PrepEnergyCannon.baseDuration / this.attackSpeedStat;
			base.PlayCrossfade("Body", "PrepEnergyCannon", "PrepEnergyCannon.playbackRate", this.duration, 0.1f);
			Util.PlaySound(PrepEnergyCannon.sound, base.gameObject);
			if (this.muzzleTransform && PrepEnergyCannon.chargeEffectPrefab)
			{
				this.chargeInstance = UnityEngine.Object.Instantiate<GameObject>(PrepEnergyCannon.chargeEffectPrefab, this.muzzleTransform.position, this.muzzleTransform.rotation);
				this.chargeInstance.transform.parent = this.muzzleTransform;
				ScaleParticleSystemDuration component = this.chargeInstance.GetComponent<ScaleParticleSystemDuration>();
				if (component)
				{
					component.newDuration = this.duration;
				}
			}
		}

		// Token: 0x06002C63 RID: 11363 RVA: 0x000BB4C7 File Offset: 0x000B96C7
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			base.StartAimMode(0.5f, false);
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextState(new FireEnergyCannon());
			}
		}

		// Token: 0x06002C64 RID: 11364 RVA: 0x000BB501 File Offset: 0x000B9701
		public override void OnExit()
		{
			base.OnExit();
			if (this.chargeInstance)
			{
				EntityState.Destroy(this.chargeInstance);
			}
		}

		// Token: 0x04002863 RID: 10339
		public static float baseDuration;

		// Token: 0x04002864 RID: 10340
		public static string sound;

		// Token: 0x04002865 RID: 10341
		public static GameObject chargeEffectPrefab;

		// Token: 0x04002866 RID: 10342
		private GameObject chargeInstance;

		// Token: 0x04002867 RID: 10343
		private float duration;
	}
}
