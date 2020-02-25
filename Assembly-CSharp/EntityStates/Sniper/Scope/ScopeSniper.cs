using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Sniper.Scope
{
	// Token: 0x02000789 RID: 1929
	public class ScopeSniper : BaseState
	{
		// Token: 0x06002C4B RID: 11339 RVA: 0x000BAF9C File Offset: 0x000B919C
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

		// Token: 0x06002C4C RID: 11340 RVA: 0x000BB05C File Offset: 0x000B925C
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

		// Token: 0x06002C4D RID: 11341 RVA: 0x000BB0E4 File Offset: 0x000B92E4
		public override void FixedUpdate()
		{
			this.charge = Mathf.Min(this.charge + this.attackSpeedStat / ScopeSniper.baseChargeDuration * Time.fixedDeltaTime, 1f);
			if (base.isAuthority && (!base.inputBank || !base.inputBank.skill2.down))
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002C4E RID: 11342 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002853 RID: 10323
		public static float baseChargeDuration = 4f;

		// Token: 0x04002854 RID: 10324
		public static GameObject crosshairPrefab;

		// Token: 0x04002855 RID: 10325
		public float charge;

		// Token: 0x04002856 RID: 10326
		private GameObject originalCrosshairPrefab;

		// Token: 0x04002857 RID: 10327
		private GameObject laserPointerObject;
	}
}
