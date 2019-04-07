using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Sniper.Scope
{
	// Token: 0x020000F7 RID: 247
	internal class ScopeSniper : BaseState
	{
		// Token: 0x060004BB RID: 1211 RVA: 0x00013C64 File Offset: 0x00011E64
		public override void OnEnter()
		{
			base.OnEnter();
			this.charge = 0f;
			if (NetworkServer.active && base.characterBody)
			{
				base.characterBody.AddBuff(BuffIndex.Slow50);
			}
			if (base.cameraTargetParams)
			{
				base.cameraTargetParams.aimMode = CameraTargetParams.AimType.FirstPerson;
				base.cameraTargetParams.fovOverride = 20f;
			}
			if (base.characterBody)
			{
				this.originalCrosshairPrefab = base.characterBody.crosshairPrefab;
				base.characterBody.crosshairPrefab = ScopeSniper.crosshairPrefab;
			}
			this.laserPointerObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/LaserPointerBeamEnd"));
			this.laserPointerObject.GetComponent<LaserPointerController>().source = base.inputBank;
		}

		// Token: 0x060004BC RID: 1212 RVA: 0x00013D24 File Offset: 0x00011F24
		public override void OnExit()
		{
			EntityState.Destroy(this.laserPointerObject);
			if (NetworkServer.active && base.characterBody)
			{
				base.characterBody.RemoveBuff(BuffIndex.Slow50);
			}
			if (base.cameraTargetParams)
			{
				base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Standard;
				base.cameraTargetParams.fovOverride = -1f;
			}
			if (base.characterBody)
			{
				base.characterBody.crosshairPrefab = this.originalCrosshairPrefab;
			}
			base.OnExit();
		}

		// Token: 0x060004BD RID: 1213 RVA: 0x00013DAC File Offset: 0x00011FAC
		public override void FixedUpdate()
		{
			this.charge = Mathf.Min(this.charge + this.attackSpeedStat / ScopeSniper.baseChargeDuration * Time.fixedDeltaTime, 1f);
			if (base.isAuthority && (!base.inputBank || !base.inputBank.skill2.down))
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060004BE RID: 1214 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04000480 RID: 1152
		public static float baseChargeDuration = 4f;

		// Token: 0x04000481 RID: 1153
		public static GameObject crosshairPrefab;

		// Token: 0x04000482 RID: 1154
		public float charge;

		// Token: 0x04000483 RID: 1155
		private GameObject originalCrosshairPrefab;

		// Token: 0x04000484 RID: 1156
		private GameObject laserPointerObject;
	}
}
