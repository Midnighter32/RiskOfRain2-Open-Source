using System;
using RoR2;
using UnityEngine;

namespace EntityStates.ClayBoss.ClayBossWeapon
{
	// Token: 0x020001BE RID: 446
	internal class ChargeBombardment : BaseState
	{
		// Token: 0x060008B8 RID: 2232 RVA: 0x0002BBF0 File Offset: 0x00029DF0
		public override void OnEnter()
		{
			base.OnEnter();
			this.totalDuration = ChargeBombardment.baseTotalDuration / this.attackSpeedStat;
			this.maxChargeTime = ChargeBombardment.baseMaxChargeTime / this.attackSpeedStat;
			Transform modelTransform = base.GetModelTransform();
			base.PlayAnimation("Gesture, Additive", "ChargeBombardment");
			Util.PlaySound(ChargeBombardment.chargeLoopStartSoundString, base.gameObject);
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild("Muzzle");
					if (transform && ChargeBombardment.chargeEffectPrefab)
					{
						this.chargeInstance = UnityEngine.Object.Instantiate<GameObject>(ChargeBombardment.chargeEffectPrefab, transform.position, transform.rotation);
						this.chargeInstance.transform.parent = transform;
						ScaleParticleSystemDuration component2 = this.chargeInstance.GetComponent<ScaleParticleSystemDuration>();
						if (component2)
						{
							component2.newDuration = this.totalDuration;
						}
					}
				}
			}
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(this.totalDuration);
			}
		}

		// Token: 0x060008B9 RID: 2233 RVA: 0x0002BCF1 File Offset: 0x00029EF1
		public override void OnExit()
		{
			base.OnExit();
			base.PlayAnimation("Gesture, Additive", "Empty");
			Util.PlaySound(ChargeBombardment.chargeLoopStopSoundString, base.gameObject);
			EntityState.Destroy(this.chargeInstance);
		}

		// Token: 0x060008BA RID: 2234 RVA: 0x0002BD28 File Offset: 0x00029F28
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			this.charge = Mathf.Min((int)(this.stopwatch / this.maxChargeTime * (float)ChargeBombardment.maxCharges), ChargeBombardment.maxCharges);
			float t = (float)this.charge / (float)ChargeBombardment.maxCharges;
			float value = Mathf.Lerp(ChargeBombardment.minBonusBloom, ChargeBombardment.maxBonusBloom, t);
			base.characterBody.SetSpreadBloom(value, true);
			int grenadeCountMax = Mathf.FloorToInt(Mathf.Lerp((float)ChargeBombardment.minGrenadeCount, (float)ChargeBombardment.maxGrenadeCount, t));
			if ((this.stopwatch >= this.totalDuration || !base.inputBank || !base.inputBank.skill1.down) && base.isAuthority)
			{
				FireBombardment fireBombardment = new FireBombardment();
				fireBombardment.grenadeCountMax = grenadeCountMax;
				this.outer.SetNextState(fireBombardment);
				return;
			}
		}

		// Token: 0x060008BB RID: 2235 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000BB0 RID: 2992
		public static float baseTotalDuration;

		// Token: 0x04000BB1 RID: 2993
		public static float baseMaxChargeTime;

		// Token: 0x04000BB2 RID: 2994
		public static int maxCharges;

		// Token: 0x04000BB3 RID: 2995
		public static GameObject chargeEffectPrefab;

		// Token: 0x04000BB4 RID: 2996
		public static string chargeLoopStartSoundString;

		// Token: 0x04000BB5 RID: 2997
		public static string chargeLoopStopSoundString;

		// Token: 0x04000BB6 RID: 2998
		public static int minGrenadeCount;

		// Token: 0x04000BB7 RID: 2999
		public static int maxGrenadeCount;

		// Token: 0x04000BB8 RID: 3000
		public static float minBonusBloom;

		// Token: 0x04000BB9 RID: 3001
		public static float maxBonusBloom;

		// Token: 0x04000BBA RID: 3002
		private float stopwatch;

		// Token: 0x04000BBB RID: 3003
		private GameObject chargeInstance;

		// Token: 0x04000BBC RID: 3004
		private int charge;

		// Token: 0x04000BBD RID: 3005
		private float totalDuration;

		// Token: 0x04000BBE RID: 3006
		private float maxChargeTime;
	}
}
