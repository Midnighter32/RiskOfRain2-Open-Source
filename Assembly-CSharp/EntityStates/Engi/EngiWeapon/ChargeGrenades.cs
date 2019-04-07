using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Engi.EngiWeapon
{
	// Token: 0x02000182 RID: 386
	internal class ChargeGrenades : BaseState
	{
		// Token: 0x0600076B RID: 1899 RVA: 0x00024678 File Offset: 0x00022878
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

		// Token: 0x0600076C RID: 1900 RVA: 0x000247D4 File Offset: 0x000229D4
		public override void OnExit()
		{
			base.OnExit();
			base.PlayAnimation("Gesture, Additive", "Empty");
			Util.PlaySound(ChargeGrenades.chargeLoopStopSoundString, base.gameObject);
			EntityState.Destroy(this.chargeLeftInstance);
			EntityState.Destroy(this.chargeRightInstance);
		}

		// Token: 0x0600076D RID: 1901 RVA: 0x00024814 File Offset: 0x00022A14
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

		// Token: 0x0600076E RID: 1902 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000977 RID: 2423
		public static float baseTotalDuration;

		// Token: 0x04000978 RID: 2424
		public static float baseMaxChargeTime;

		// Token: 0x04000979 RID: 2425
		public static int maxCharges;

		// Token: 0x0400097A RID: 2426
		public static GameObject chargeEffectPrefab;

		// Token: 0x0400097B RID: 2427
		public static string chargeStockSoundString;

		// Token: 0x0400097C RID: 2428
		public static string chargeLoopStartSoundString;

		// Token: 0x0400097D RID: 2429
		public static string chargeLoopStopSoundString;

		// Token: 0x0400097E RID: 2430
		public static int minGrenadeCount;

		// Token: 0x0400097F RID: 2431
		public static int maxGrenadeCount;

		// Token: 0x04000980 RID: 2432
		public static float minBonusBloom;

		// Token: 0x04000981 RID: 2433
		public static float maxBonusBloom;

		// Token: 0x04000982 RID: 2434
		private GameObject chargeLeftInstance;

		// Token: 0x04000983 RID: 2435
		private GameObject chargeRightInstance;

		// Token: 0x04000984 RID: 2436
		private int charge;

		// Token: 0x04000985 RID: 2437
		private int lastCharge;

		// Token: 0x04000986 RID: 2438
		private float totalDuration;

		// Token: 0x04000987 RID: 2439
		private float maxChargeTime;
	}
}
