using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Toolbot
{
	// Token: 0x02000760 RID: 1888
	public class ChargeSpear : BaseState
	{
		// Token: 0x06002B9D RID: 11165 RVA: 0x000B8150 File Offset: 0x000B6350
		public override void OnEnter()
		{
			base.OnEnter();
			this.minChargeDuration = ChargeSpear.baseMinChargeDuration / this.attackSpeedStat;
			this.chargeDuration = ChargeSpear.baseChargeDuration / this.attackSpeedStat;
			base.PlayAnimation("Gesture, Additive", "ChargeSpear", "ChargeSpear.playbackRate", this.chargeDuration);
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					this.muzzleTransform = component.FindChild(ChargeSpear.muzzleName);
					if (this.muzzleTransform)
					{
						this.chargeupVfxGameObject = UnityEngine.Object.Instantiate<GameObject>(ChargeSpear.chargeupVfxPrefab, this.muzzleTransform);
						this.chargeupVfxGameObject.GetComponent<ScaleParticleSystemDuration>().newDuration = this.chargeDuration;
					}
				}
			}
		}

		// Token: 0x06002B9E RID: 11166 RVA: 0x000B820C File Offset: 0x000B640C
		public override void OnExit()
		{
			if (this.chargeupVfxGameObject)
			{
				EntityState.Destroy(this.chargeupVfxGameObject);
				this.chargeupVfxGameObject = null;
			}
			if (this.holdChargeVfxGameObject)
			{
				EntityState.Destroy(this.holdChargeVfxGameObject);
				this.holdChargeVfxGameObject = null;
			}
			base.OnExit();
		}

		// Token: 0x06002B9F RID: 11167 RVA: 0x000B825D File Offset: 0x000B645D
		public override void Update()
		{
			base.Update();
			base.characterBody.SetSpreadBloom(base.age / this.chargeDuration, true);
		}

		// Token: 0x06002BA0 RID: 11168 RVA: 0x000B8280 File Offset: 0x000B6480
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			float num = base.fixedAge - this.chargeDuration;
			if (num >= 0f)
			{
				float num2 = ChargeSpear.perfectChargeWindow;
			}
			float charge = Mathf.Clamp01(base.fixedAge / this.chargeDuration);
			if (base.fixedAge >= this.chargeDuration)
			{
				if (this.chargeupVfxGameObject)
				{
					EntityState.Destroy(this.chargeupVfxGameObject);
					this.chargeupVfxGameObject = null;
				}
				if (!this.holdChargeVfxGameObject && this.muzzleTransform)
				{
					this.holdChargeVfxGameObject = UnityEngine.Object.Instantiate<GameObject>(ChargeSpear.holdChargeVfxPrefab, this.muzzleTransform);
				}
			}
			if (base.isAuthority)
			{
				if (!this.released && (!base.inputBank || !base.inputBank.skill1.down))
				{
					this.released = true;
				}
				if (this.released && base.fixedAge >= this.minChargeDuration)
				{
					this.outer.SetNextState(new FireSpear
					{
						charge = charge
					});
				}
			}
		}

		// Token: 0x06002BA1 RID: 11169 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040027B1 RID: 10161
		public static float baseMinChargeDuration;

		// Token: 0x040027B2 RID: 10162
		public static float baseChargeDuration;

		// Token: 0x040027B3 RID: 10163
		public static float perfectChargeWindow;

		// Token: 0x040027B4 RID: 10164
		public static string muzzleName;

		// Token: 0x040027B5 RID: 10165
		public static GameObject chargeupVfxPrefab;

		// Token: 0x040027B6 RID: 10166
		public static GameObject holdChargeVfxPrefab;

		// Token: 0x040027B7 RID: 10167
		private float minChargeDuration;

		// Token: 0x040027B8 RID: 10168
		private float chargeDuration;

		// Token: 0x040027B9 RID: 10169
		private bool released;

		// Token: 0x040027BA RID: 10170
		private GameObject chargeupVfxGameObject;

		// Token: 0x040027BB RID: 10171
		private GameObject holdChargeVfxGameObject;

		// Token: 0x040027BC RID: 10172
		private Transform muzzleTransform;
	}
}
