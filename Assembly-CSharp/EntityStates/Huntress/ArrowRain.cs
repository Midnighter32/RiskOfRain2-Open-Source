using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Huntress
{
	// Token: 0x0200014D RID: 333
	public class ArrowRain : BaseState
	{
		// Token: 0x06000664 RID: 1636 RVA: 0x0001DC68 File Offset: 0x0001BE68
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("FullBody, Override", "LoopArrowRain");
			Util.PlaySound(ArrowRain.beginLoopSoundString, base.gameObject);
			this.huntressTracker = base.GetComponent<HuntressTracker>();
			this.areaIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(ArrowRain.areaIndicatorPrefab);
			this.areaIndicatorInstance.transform.localScale = new Vector3(ArrowRain.arrowRainRadius, ArrowRain.arrowRainRadius, ArrowRain.arrowRainRadius);
			if (this.huntressTracker)
			{
				this.huntressTracker.enabled = false;
			}
		}

		// Token: 0x06000665 RID: 1637 RVA: 0x0001DCF8 File Offset: 0x0001BEF8
		private void UpdateAreaIndicator()
		{
			if (this.areaIndicatorInstance)
			{
				float maxDistance = 1000f;
				RaycastHit raycastHit;
				if (Physics.Raycast(base.GetAimRay(), out raycastHit, maxDistance, LayerIndex.world.mask))
				{
					this.areaIndicatorInstance.transform.position = raycastHit.point;
					this.areaIndicatorInstance.transform.up = raycastHit.normal;
				}
			}
		}

		// Token: 0x06000666 RID: 1638 RVA: 0x0001DD68 File Offset: 0x0001BF68
		public override void Update()
		{
			base.Update();
			this.UpdateAreaIndicator();
		}

		// Token: 0x06000667 RID: 1639 RVA: 0x0001DD78 File Offset: 0x0001BF78
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (base.characterMotor)
			{
				base.characterMotor.velocity = Vector3.zero;
			}
			if (base.skillLocator && base.skillLocator.utility.CanExecute() && base.inputBank.skill3.justPressed && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
			if ((this.stopwatch >= ArrowRain.maxDuration || base.inputBank.skill1.justPressed || base.inputBank.skill4.justPressed) && base.isAuthority)
			{
				this.fireArrowRain = true;
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06000668 RID: 1640 RVA: 0x0001DE4C File Offset: 0x0001C04C
		public override void OnExit()
		{
			base.PlayAnimation("FullBody, Override", "FireArrowRain");
			Util.PlaySound(ArrowRain.endLoopSoundString, base.gameObject);
			Util.PlaySound(ArrowRain.fireSoundString, base.gameObject);
			EffectManager.instance.SimpleMuzzleFlash(ArrowRain.muzzleflashEffect, base.gameObject, "Muzzle", false);
			if (base.cameraTargetParams)
			{
				base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Standard;
			}
			if (this.huntressTracker)
			{
				this.huntressTracker.enabled = true;
			}
			if (this.areaIndicatorInstance)
			{
				if (this.fireArrowRain)
				{
					ProjectileManager.instance.FireProjectile(ArrowRain.projectilePrefab, this.areaIndicatorInstance.transform.position, this.areaIndicatorInstance.transform.rotation, base.gameObject, this.damageStat * ArrowRain.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
				}
				EntityState.Destroy(this.areaIndicatorInstance.gameObject);
			}
			base.OnExit();
		}

		// Token: 0x04000795 RID: 1941
		public static float maxDuration;

		// Token: 0x04000796 RID: 1942
		public static GameObject areaIndicatorPrefab;

		// Token: 0x04000797 RID: 1943
		public static float arrowRainRadius;

		// Token: 0x04000798 RID: 1944
		public static GameObject projectilePrefab;

		// Token: 0x04000799 RID: 1945
		public static float damageCoefficient;

		// Token: 0x0400079A RID: 1946
		public static GameObject muzzleflashEffect;

		// Token: 0x0400079B RID: 1947
		public static string beginLoopSoundString;

		// Token: 0x0400079C RID: 1948
		public static string endLoopSoundString;

		// Token: 0x0400079D RID: 1949
		public static string fireSoundString;

		// Token: 0x0400079E RID: 1950
		private float stopwatch;

		// Token: 0x0400079F RID: 1951
		private HuntressTracker huntressTracker;

		// Token: 0x040007A0 RID: 1952
		private GameObject areaIndicatorInstance;

		// Token: 0x040007A1 RID: 1953
		private bool fireArrowRain;
	}
}
