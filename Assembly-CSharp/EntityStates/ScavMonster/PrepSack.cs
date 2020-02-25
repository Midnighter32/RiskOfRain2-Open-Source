using System;
using RoR2;
using UnityEngine;

namespace EntityStates.ScavMonster
{
	// Token: 0x02000797 RID: 1943
	public class PrepSack : SackBaseState
	{
		// Token: 0x06002C89 RID: 11401 RVA: 0x000BBCB8 File Offset: 0x000B9EB8
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = PrepSack.baseDuration / this.attackSpeedStat;
			base.PlayCrossfade("Body", "PrepSack", "PrepSack.playbackRate", this.duration, 0.1f);
			Util.PlaySound(PrepSack.sound, base.gameObject);
			base.StartAimMode(this.duration, false);
			if (this.muzzleTransform && PrepSack.chargeEffectPrefab)
			{
				this.chargeInstance = UnityEngine.Object.Instantiate<GameObject>(PrepSack.chargeEffectPrefab, this.muzzleTransform.position, this.muzzleTransform.rotation);
				this.chargeInstance.transform.parent = this.muzzleTransform;
				ScaleParticleSystemDuration component = this.chargeInstance.GetComponent<ScaleParticleSystemDuration>();
				if (component)
				{
					component.newDuration = this.duration;
				}
			}
		}

		// Token: 0x06002C8A RID: 11402 RVA: 0x000BBD90 File Offset: 0x000B9F90
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextState(new ThrowSack());
			}
		}

		// Token: 0x06002C8B RID: 11403 RVA: 0x000BBDBE File Offset: 0x000B9FBE
		public override void OnExit()
		{
			base.OnExit();
			if (this.chargeInstance)
			{
				EntityState.Destroy(this.chargeInstance);
			}
		}

		// Token: 0x04002895 RID: 10389
		public static float baseDuration;

		// Token: 0x04002896 RID: 10390
		public static string sound;

		// Token: 0x04002897 RID: 10391
		public static GameObject chargeEffectPrefab;

		// Token: 0x04002898 RID: 10392
		private GameObject chargeInstance;

		// Token: 0x04002899 RID: 10393
		private float duration;
	}
}
