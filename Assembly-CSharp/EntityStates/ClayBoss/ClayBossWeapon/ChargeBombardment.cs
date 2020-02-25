using System;
using RoR2;
using UnityEngine;

namespace EntityStates.ClayBoss.ClayBossWeapon
{
	// Token: 0x020008D9 RID: 2265
	public class ChargeBombardment : BaseState
	{
		// Token: 0x060032B7 RID: 12983 RVA: 0x000DB824 File Offset: 0x000D9A24
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

		// Token: 0x060032B8 RID: 12984 RVA: 0x000DB925 File Offset: 0x000D9B25
		public override void OnExit()
		{
			base.OnExit();
			base.PlayAnimation("Gesture, Additive", "Empty");
			Util.PlaySound(ChargeBombardment.chargeLoopStopSoundString, base.gameObject);
			EntityState.Destroy(this.chargeInstance);
		}

		// Token: 0x060032B9 RID: 12985 RVA: 0x000DB95C File Offset: 0x000D9B5C
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

		// Token: 0x060032BA RID: 12986 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040031D8 RID: 12760
		public static float baseTotalDuration;

		// Token: 0x040031D9 RID: 12761
		public static float baseMaxChargeTime;

		// Token: 0x040031DA RID: 12762
		public static int maxCharges;

		// Token: 0x040031DB RID: 12763
		public static GameObject chargeEffectPrefab;

		// Token: 0x040031DC RID: 12764
		public static string chargeLoopStartSoundString;

		// Token: 0x040031DD RID: 12765
		public static string chargeLoopStopSoundString;

		// Token: 0x040031DE RID: 12766
		public static int minGrenadeCount;

		// Token: 0x040031DF RID: 12767
		public static int maxGrenadeCount;

		// Token: 0x040031E0 RID: 12768
		public static float minBonusBloom;

		// Token: 0x040031E1 RID: 12769
		public static float maxBonusBloom;

		// Token: 0x040031E2 RID: 12770
		private float stopwatch;

		// Token: 0x040031E3 RID: 12771
		private GameObject chargeInstance;

		// Token: 0x040031E4 RID: 12772
		private int charge;

		// Token: 0x040031E5 RID: 12773
		private float totalDuration;

		// Token: 0x040031E6 RID: 12774
		private float maxChargeTime;
	}
}
