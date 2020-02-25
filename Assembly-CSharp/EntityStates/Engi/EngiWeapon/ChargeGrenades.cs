using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Engi.EngiWeapon
{
	// Token: 0x02000881 RID: 2177
	public class ChargeGrenades : BaseState
	{
		// Token: 0x060030FB RID: 12539 RVA: 0x000D28E0 File Offset: 0x000D0AE0
		public override void OnEnter()
		{
			base.OnEnter();
			this.totalDuration = ChargeGrenades.baseTotalDuration / this.attackSpeedStat;
			this.maxChargeTime = ChargeGrenades.baseMaxChargeTime / this.attackSpeedStat;
			Transform modelTransform = base.GetModelTransform();
			base.PlayAnimation("Gesture, Additive", "ChargeGrenades");
			Util.PlaySound(ChargeGrenades.chargeLoopStartSoundString, base.gameObject);
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild("MuzzleLeft");
					if (transform && ChargeGrenades.chargeEffectPrefab)
					{
						this.chargeLeftInstance = UnityEngine.Object.Instantiate<GameObject>(ChargeGrenades.chargeEffectPrefab, transform.position, transform.rotation);
						this.chargeLeftInstance.transform.parent = transform;
						ScaleParticleSystemDuration component2 = this.chargeLeftInstance.GetComponent<ScaleParticleSystemDuration>();
						if (component2)
						{
							component2.newDuration = this.totalDuration;
						}
					}
					Transform transform2 = component.FindChild("MuzzleRight");
					if (transform2 && ChargeGrenades.chargeEffectPrefab)
					{
						this.chargeRightInstance = UnityEngine.Object.Instantiate<GameObject>(ChargeGrenades.chargeEffectPrefab, transform2.position, transform2.rotation);
						this.chargeRightInstance.transform.parent = transform2;
						ScaleParticleSystemDuration component3 = this.chargeRightInstance.GetComponent<ScaleParticleSystemDuration>();
						if (component3)
						{
							component3.newDuration = this.totalDuration;
						}
					}
				}
			}
		}

		// Token: 0x060030FC RID: 12540 RVA: 0x000D2A3C File Offset: 0x000D0C3C
		public override void OnExit()
		{
			base.OnExit();
			base.PlayAnimation("Gesture, Additive", "Empty");
			Util.PlaySound(ChargeGrenades.chargeLoopStopSoundString, base.gameObject);
			EntityState.Destroy(this.chargeLeftInstance);
			EntityState.Destroy(this.chargeRightInstance);
		}

		// Token: 0x060030FD RID: 12541 RVA: 0x000D2A7C File Offset: 0x000D0C7C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.lastCharge = this.charge;
			this.charge = Mathf.Min((int)(base.fixedAge / this.maxChargeTime * (float)ChargeGrenades.maxCharges), ChargeGrenades.maxCharges);
			float t = (float)this.charge / (float)ChargeGrenades.maxCharges;
			float value = Mathf.Lerp(ChargeGrenades.minBonusBloom, ChargeGrenades.maxBonusBloom, t);
			base.characterBody.SetSpreadBloom(value, true);
			int num = Mathf.FloorToInt(Mathf.Lerp((float)ChargeGrenades.minGrenadeCount, (float)ChargeGrenades.maxGrenadeCount, t));
			if (this.lastCharge < this.charge)
			{
				Util.PlaySound(ChargeGrenades.chargeStockSoundString, base.gameObject, "engiM1_chargePercent", 100f * ((float)(num - 1) / (float)ChargeGrenades.maxGrenadeCount));
			}
			if ((base.fixedAge >= this.totalDuration || !base.inputBank || !base.inputBank.skill1.down) && base.isAuthority)
			{
				FireGrenades fireGrenades = new FireGrenades();
				fireGrenades.grenadeCountMax = num;
				this.outer.SetNextState(fireGrenades);
				return;
			}
		}

		// Token: 0x060030FE RID: 12542 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002F27 RID: 12071
		public static float baseTotalDuration;

		// Token: 0x04002F28 RID: 12072
		public static float baseMaxChargeTime;

		// Token: 0x04002F29 RID: 12073
		public static int maxCharges;

		// Token: 0x04002F2A RID: 12074
		public static GameObject chargeEffectPrefab;

		// Token: 0x04002F2B RID: 12075
		public static string chargeStockSoundString;

		// Token: 0x04002F2C RID: 12076
		public static string chargeLoopStartSoundString;

		// Token: 0x04002F2D RID: 12077
		public static string chargeLoopStopSoundString;

		// Token: 0x04002F2E RID: 12078
		public static int minGrenadeCount;

		// Token: 0x04002F2F RID: 12079
		public static int maxGrenadeCount;

		// Token: 0x04002F30 RID: 12080
		public static float minBonusBloom;

		// Token: 0x04002F31 RID: 12081
		public static float maxBonusBloom;

		// Token: 0x04002F32 RID: 12082
		private GameObject chargeLeftInstance;

		// Token: 0x04002F33 RID: 12083
		private GameObject chargeRightInstance;

		// Token: 0x04002F34 RID: 12084
		private int charge;

		// Token: 0x04002F35 RID: 12085
		private int lastCharge;

		// Token: 0x04002F36 RID: 12086
		private float totalDuration;

		// Token: 0x04002F37 RID: 12087
		private float maxChargeTime;
	}
}
