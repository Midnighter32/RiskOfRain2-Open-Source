using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Mage.Weapon
{
	// Token: 0x0200011C RID: 284
	public class PrepWall : BaseState
	{
		// Token: 0x06000575 RID: 1397 RVA: 0x00018CB8 File Offset: 0x00016EB8
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = PrepWall.baseDuration / this.attackSpeedStat;
			base.characterBody.SetAimTimer(this.duration + 2f);
			this.cachedCrosshairPrefab = base.characterBody.crosshairPrefab;
			base.PlayAnimation("Gesture, Additive", "PrepWall", "PrepWall.playbackRate", this.duration);
			Util.PlaySound(PrepWall.prepWallSoundString, base.gameObject);
			this.areaIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(PrepWall.areaIndicatorPrefab);
			MageLastElementTracker component = base.GetComponent<MageLastElementTracker>();
			if (component)
			{
				component.ApplyElement(MageElement.Ice);
			}
			this.UpdateAreaIndicator();
		}

		// Token: 0x06000576 RID: 1398 RVA: 0x00018D60 File Offset: 0x00016F60
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

		// Token: 0x06000577 RID: 1399 RVA: 0x00018E68 File Offset: 0x00017068
		public override void Update()
		{
			base.Update();
			this.UpdateAreaIndicator();
		}

		// Token: 0x06000578 RID: 1400 RVA: 0x00018E78 File Offset: 0x00017078
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.duration && !base.inputBank.skill3.down && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06000579 RID: 1401 RVA: 0x00018ED0 File Offset: 0x000170D0
		public override void OnExit()
		{
			if (this.goodPlacement)
			{
				base.PlayAnimation("Gesture, Additive", "FireWall");
				Util.PlaySound(PrepWall.fireSoundString, base.gameObject);
				if (this.areaIndicatorInstance && base.isAuthority)
				{
					EffectManager.instance.SimpleMuzzleFlash(PrepWall.muzzleflashEffect, base.gameObject, "MuzzleLeft", true);
					EffectManager.instance.SimpleMuzzleFlash(PrepWall.muzzleflashEffect, base.gameObject, "MuzzleRight", true);
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
			EntityState.Destroy(this.areaIndicatorInstance.gameObject);
			base.characterBody.crosshairPrefab = this.cachedCrosshairPrefab;
			base.OnExit();
		}

		// Token: 0x0600057A RID: 1402 RVA: 0x0000BB2B File Offset: 0x00009D2B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Pain;
		}

		// Token: 0x04000618 RID: 1560
		public static float baseDuration;

		// Token: 0x04000619 RID: 1561
		public static GameObject areaIndicatorPrefab;

		// Token: 0x0400061A RID: 1562
		public static GameObject projectilePrefab;

		// Token: 0x0400061B RID: 1563
		public static float damageCoefficient;

		// Token: 0x0400061C RID: 1564
		public static GameObject muzzleflashEffect;

		// Token: 0x0400061D RID: 1565
		public static GameObject goodCrosshairPrefab;

		// Token: 0x0400061E RID: 1566
		public static GameObject badCrosshairPrefab;

		// Token: 0x0400061F RID: 1567
		public static string prepWallSoundString;

		// Token: 0x04000620 RID: 1568
		public static float maxDistance;

		// Token: 0x04000621 RID: 1569
		public static string fireSoundString;

		// Token: 0x04000622 RID: 1570
		public static float maxSlopeAngle;

		// Token: 0x04000623 RID: 1571
		private float duration;

		// Token: 0x04000624 RID: 1572
		private float stopwatch;

		// Token: 0x04000625 RID: 1573
		private bool goodPlacement;

		// Token: 0x04000626 RID: 1574
		private GameObject areaIndicatorInstance;

		// Token: 0x04000627 RID: 1575
		private GameObject cachedCrosshairPrefab;
	}
}
