using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.VagrantMonster
{
	// Token: 0x0200012E RID: 302
	internal class FireTrackingBomb : BaseState
	{
		// Token: 0x060005D2 RID: 1490 RVA: 0x0001AA4C File Offset: 0x00018C4C
		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;
			this.duration = FireTrackingBomb.baseDuration / this.attackSpeedStat;
			base.PlayAnimation("Gesture, Override", "FireTrackingBomb", "FireTrackingBomb.playbackRate", this.duration);
			this.FireBomb();
		}

		// Token: 0x060005D3 RID: 1491 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x060005D4 RID: 1492 RVA: 0x0001AA9D File Offset: 0x00018C9D
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060005D5 RID: 1493 RVA: 0x0001AADC File Offset: 0x00018CDC
		private void FireBomb()
		{
			Ray aimRay = base.GetAimRay();
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					aimRay.origin = component.FindChild("TrackingBombMuzzle").transform.position;
				}
			}
			EffectManager.instance.SimpleMuzzleFlash(FireTrackingBomb.muzzleEffectPrefab, base.gameObject, "TrackingBombMuzzle", false);
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(FireTrackingBomb.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireTrackingBomb.bombDamageCoefficient, FireTrackingBomb.bombForce, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x040006AB RID: 1707
		public static float baseDuration = 3f;

		// Token: 0x040006AC RID: 1708
		public static GameObject projectilePrefab;

		// Token: 0x040006AD RID: 1709
		public static GameObject muzzleEffectPrefab;

		// Token: 0x040006AE RID: 1710
		public static string fireBombSoundString;

		// Token: 0x040006AF RID: 1711
		public static float bombDamageCoefficient;

		// Token: 0x040006B0 RID: 1712
		public static float bombForce;

		// Token: 0x040006B1 RID: 1713
		public float novaRadius;

		// Token: 0x040006B2 RID: 1714
		private float duration;

		// Token: 0x040006B3 RID: 1715
		private float stopwatch;
	}
}
