using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Mage.Weapon
{
	// Token: 0x020007DD RID: 2013
	public class PrepWall : BaseState
	{
		// Token: 0x06002DD6 RID: 11734 RVA: 0x000C3008 File Offset: 0x000C1208
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = PrepWall.baseDuration / this.attackSpeedStat;
			base.characterBody.SetAimTimer(this.duration + 2f);
			this.cachedCrosshairPrefab = base.characterBody.crosshairPrefab;
			base.PlayAnimation("Gesture, Additive", "PrepWall", "PrepWall.playbackRate", this.duration);
			Util.PlaySound(PrepWall.prepWallSoundString, base.gameObject);
			this.areaIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(PrepWall.areaIndicatorPrefab);
			this.UpdateAreaIndicator();
		}

		// Token: 0x06002DD7 RID: 11735 RVA: 0x000C3098 File Offset: 0x000C1298
		private void UpdateAreaIndicator()
		{
			this.goodPlacement = false;
			this.areaIndicatorInstance.SetActive(true);
			if (this.areaIndicatorInstance)
			{
				float num = PrepWall.maxDistance;
				float num2 = 0f;
				Ray aimRay = base.GetAimRay();
				RaycastHit raycastHit;
				if (Physics.Raycast(CameraRigController.ModifyAimRayIfApplicable(aimRay, base.gameObject, out num2), out raycastHit, num + num2, LayerIndex.world.mask))
				{
					this.areaIndicatorInstance.transform.position = raycastHit.point;
					this.areaIndicatorInstance.transform.up = raycastHit.normal;
					this.areaIndicatorInstance.transform.forward = -aimRay.direction;
					this.goodPlacement = (Vector3.Angle(Vector3.up, raycastHit.normal) < PrepWall.maxSlopeAngle);
				}
				base.characterBody.crosshairPrefab = (this.goodPlacement ? PrepWall.goodCrosshairPrefab : PrepWall.badCrosshairPrefab);
			}
			this.areaIndicatorInstance.SetActive(this.goodPlacement);
		}

		// Token: 0x06002DD8 RID: 11736 RVA: 0x000C31A0 File Offset: 0x000C13A0
		public override void Update()
		{
			base.Update();
			this.UpdateAreaIndicator();
		}

		// Token: 0x06002DD9 RID: 11737 RVA: 0x000C31B0 File Offset: 0x000C13B0
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.duration && !base.inputBank.skill3.down && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002DDA RID: 11738 RVA: 0x000C3208 File Offset: 0x000C1408
		public override void OnExit()
		{
			if (!this.outer.destroying)
			{
				if (this.goodPlacement)
				{
					base.PlayAnimation("Gesture, Additive", "FireWall");
					Util.PlaySound(PrepWall.fireSoundString, base.gameObject);
					if (this.areaIndicatorInstance && base.isAuthority)
					{
						EffectManager.SimpleMuzzleFlash(PrepWall.muzzleflashEffect, base.gameObject, "MuzzleLeft", true);
						EffectManager.SimpleMuzzleFlash(PrepWall.muzzleflashEffect, base.gameObject, "MuzzleRight", true);
						Vector3 forward = this.areaIndicatorInstance.transform.forward;
						forward.y = 0f;
						forward.Normalize();
						Vector3 vector = Vector3.Cross(Vector3.up, forward);
						bool crit = Util.CheckRoll(this.critStat, base.characterBody.master);
						ProjectileManager.instance.FireProjectile(PrepWall.projectilePrefab, this.areaIndicatorInstance.transform.position + Vector3.up, Util.QuaternionSafeLookRotation(vector), base.gameObject, this.damageStat * PrepWall.damageCoefficient, 0f, crit, DamageColorIndex.Default, null, -1f);
						ProjectileManager.instance.FireProjectile(PrepWall.projectilePrefab, this.areaIndicatorInstance.transform.position + Vector3.up, Util.QuaternionSafeLookRotation(-vector), base.gameObject, this.damageStat * PrepWall.damageCoefficient, 0f, crit, DamageColorIndex.Default, null, -1f);
					}
				}
				else
				{
					base.skillLocator.utility.AddOneStock();
					base.PlayCrossfade("Gesture, Additive", "BufferEmpty", 0.2f);
				}
			}
			EntityState.Destroy(this.areaIndicatorInstance.gameObject);
			base.characterBody.crosshairPrefab = this.cachedCrosshairPrefab;
			base.OnExit();
		}

		// Token: 0x06002DDB RID: 11739 RVA: 0x0000C5D3 File Offset: 0x0000A7D3
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Pain;
		}

		// Token: 0x04002AD5 RID: 10965
		public static float baseDuration;

		// Token: 0x04002AD6 RID: 10966
		public static GameObject areaIndicatorPrefab;

		// Token: 0x04002AD7 RID: 10967
		public static GameObject projectilePrefab;

		// Token: 0x04002AD8 RID: 10968
		public static float damageCoefficient;

		// Token: 0x04002AD9 RID: 10969
		public static GameObject muzzleflashEffect;

		// Token: 0x04002ADA RID: 10970
		public static GameObject goodCrosshairPrefab;

		// Token: 0x04002ADB RID: 10971
		public static GameObject badCrosshairPrefab;

		// Token: 0x04002ADC RID: 10972
		public static string prepWallSoundString;

		// Token: 0x04002ADD RID: 10973
		public static float maxDistance;

		// Token: 0x04002ADE RID: 10974
		public static string fireSoundString;

		// Token: 0x04002ADF RID: 10975
		public static float maxSlopeAngle;

		// Token: 0x04002AE0 RID: 10976
		private float duration;

		// Token: 0x04002AE1 RID: 10977
		private float stopwatch;

		// Token: 0x04002AE2 RID: 10978
		private bool goodPlacement;

		// Token: 0x04002AE3 RID: 10979
		private GameObject areaIndicatorInstance;

		// Token: 0x04002AE4 RID: 10980
		private GameObject cachedCrosshairPrefab;
	}
}
