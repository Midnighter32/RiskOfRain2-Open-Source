using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Toolbot
{
	// Token: 0x020000DC RID: 220
	public class ChargeSpear : BaseState
	{
		// Token: 0x0600044E RID: 1102 RVA: 0x00011D04 File Offset: 0x0000FF04
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

		// Token: 0x0600044F RID: 1103 RVA: 0x00011DC0 File Offset: 0x0000FFC0
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

		// Token: 0x06000450 RID: 1104 RVA: 0x00011E11 File Offset: 0x00010011
		public override void Update()
		{
			base.Update();
			base.characterBody.SetSpreadBloom(base.age / this.chargeDuration, true);
		}

		// Token: 0x06000451 RID: 1105 RVA: 0x00011E34 File Offset: 0x00010034
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

		// Token: 0x06000452 RID: 1106 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x0400040B RID: 1035
		public static float baseMinChargeDuration;

		// Token: 0x0400040C RID: 1036
		public static float baseChargeDuration;

		// Token: 0x0400040D RID: 1037
		public static float perfectChargeWindow;

		// Token: 0x0400040E RID: 1038
		public static string muzzleName;

		// Token: 0x0400040F RID: 1039
		public static GameObject chargeupVfxPrefab;

		// Token: 0x04000410 RID: 1040
		public static GameObject holdChargeVfxPrefab;

		// Token: 0x04000411 RID: 1041
		private float minChargeDuration;

		// Token: 0x04000412 RID: 1042
		private float chargeDuration;

		// Token: 0x04000413 RID: 1043
		private bool released;

		// Token: 0x04000414 RID: 1044
		private GameObject chargeupVfxGameObject;

		// Token: 0x04000415 RID: 1045
		private GameObject holdChargeVfxGameObject;

		// Token: 0x04000416 RID: 1046
		private Transform muzzleTransform;
	}
}
