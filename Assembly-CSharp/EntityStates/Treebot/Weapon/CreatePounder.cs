using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Treebot.Weapon
{
	// Token: 0x02000750 RID: 1872
	public class CreatePounder : BaseState
	{
		// Token: 0x06002B59 RID: 11097 RVA: 0x000B69F4 File Offset: 0x000B4BF4
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = CreatePounder.baseDuration / this.attackSpeedStat;
			this.cachedCrosshairPrefab = base.characterBody.crosshairPrefab;
			base.PlayAnimation("Gesture, Additive", "PrepWall", "PrepWall.playbackRate", this.duration);
			Util.PlaySound(CreatePounder.prepSoundString, base.gameObject);
			this.areaIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(CreatePounder.areaIndicatorPrefab);
			this.UpdateAreaIndicator();
		}

		// Token: 0x06002B5A RID: 11098 RVA: 0x000B6A6C File Offset: 0x000B4C6C
		private void UpdateAreaIndicator()
		{
			this.goodPlacement = false;
			this.areaIndicatorInstance.SetActive(true);
			if (this.areaIndicatorInstance)
			{
				float num = CreatePounder.maxDistance;
				float num2 = 0f;
				RaycastHit raycastHit;
				if (Physics.Raycast(CameraRigController.ModifyAimRayIfApplicable(base.GetAimRay(), base.gameObject, out num2), out raycastHit, num + num2, LayerIndex.world.mask))
				{
					this.areaIndicatorInstance.transform.position = raycastHit.point;
					this.areaIndicatorInstance.transform.up = raycastHit.normal;
					this.goodPlacement = (Vector3.Angle(Vector3.up, raycastHit.normal) < CreatePounder.maxSlopeAngle);
				}
				base.characterBody.crosshairPrefab = (this.goodPlacement ? CreatePounder.goodCrosshairPrefab : CreatePounder.badCrosshairPrefab);
			}
			this.areaIndicatorInstance.SetActive(this.goodPlacement);
		}

		// Token: 0x06002B5B RID: 11099 RVA: 0x000B6B55 File Offset: 0x000B4D55
		public override void Update()
		{
			base.Update();
			this.UpdateAreaIndicator();
		}

		// Token: 0x06002B5C RID: 11100 RVA: 0x000B6B64 File Offset: 0x000B4D64
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.duration && !base.inputBank.skill4.down && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002B5D RID: 11101 RVA: 0x000B6BBC File Offset: 0x000B4DBC
		public override void OnExit()
		{
			if (this.goodPlacement)
			{
				Util.PlaySound(CreatePounder.fireSoundString, base.gameObject);
				if (this.areaIndicatorInstance && base.isAuthority)
				{
					bool crit = Util.CheckRoll(this.critStat, base.characterBody.master);
					ProjectileManager.instance.FireProjectile(CreatePounder.projectilePrefab, this.areaIndicatorInstance.transform.position, Quaternion.identity, base.gameObject, this.damageStat * CreatePounder.damageCoefficient, 0f, crit, DamageColorIndex.Default, null, -1f);
				}
			}
			else
			{
				base.skillLocator.special.AddOneStock();
			}
			EntityState.Destroy(this.areaIndicatorInstance.gameObject);
			base.characterBody.crosshairPrefab = this.cachedCrosshairPrefab;
			base.OnExit();
		}

		// Token: 0x06002B5E RID: 11102 RVA: 0x0000C5D3 File Offset: 0x0000A7D3
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Pain;
		}

		// Token: 0x04002745 RID: 10053
		public static float baseDuration;

		// Token: 0x04002746 RID: 10054
		public static GameObject areaIndicatorPrefab;

		// Token: 0x04002747 RID: 10055
		public static GameObject projectilePrefab;

		// Token: 0x04002748 RID: 10056
		public static float damageCoefficient;

		// Token: 0x04002749 RID: 10057
		public static GameObject muzzleflashEffect;

		// Token: 0x0400274A RID: 10058
		public static GameObject goodCrosshairPrefab;

		// Token: 0x0400274B RID: 10059
		public static GameObject badCrosshairPrefab;

		// Token: 0x0400274C RID: 10060
		public static string prepSoundString;

		// Token: 0x0400274D RID: 10061
		public static float maxDistance;

		// Token: 0x0400274E RID: 10062
		public static string fireSoundString;

		// Token: 0x0400274F RID: 10063
		public static float maxSlopeAngle;

		// Token: 0x04002750 RID: 10064
		private float duration;

		// Token: 0x04002751 RID: 10065
		private float stopwatch;

		// Token: 0x04002752 RID: 10066
		private bool goodPlacement;

		// Token: 0x04002753 RID: 10067
		private GameObject areaIndicatorInstance;

		// Token: 0x04002754 RID: 10068
		private GameObject cachedCrosshairPrefab;
	}
}
