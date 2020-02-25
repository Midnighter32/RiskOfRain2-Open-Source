using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.VagrantMonster
{
	// Token: 0x02000804 RID: 2052
	public class FireTrackingBomb : BaseState
	{
		// Token: 0x06002EA9 RID: 11945 RVA: 0x000C66FC File Offset: 0x000C48FC
		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;
			this.duration = FireTrackingBomb.baseDuration / this.attackSpeedStat;
			base.PlayAnimation("Gesture, Override", "FireTrackingBomb", "FireTrackingBomb.playbackRate", this.duration);
			this.FireBomb();
		}

		// Token: 0x06002EAA RID: 11946 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002EAB RID: 11947 RVA: 0x000C674D File Offset: 0x000C494D
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

		// Token: 0x06002EAC RID: 11948 RVA: 0x000C678C File Offset: 0x000C498C
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
			EffectManager.SimpleMuzzleFlash(FireTrackingBomb.muzzleEffectPrefab, base.gameObject, "TrackingBombMuzzle", false);
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(FireTrackingBomb.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireTrackingBomb.bombDamageCoefficient, FireTrackingBomb.bombForce, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x04002BCF RID: 11215
		public static float baseDuration = 3f;

		// Token: 0x04002BD0 RID: 11216
		public static GameObject projectilePrefab;

		// Token: 0x04002BD1 RID: 11217
		public static GameObject muzzleEffectPrefab;

		// Token: 0x04002BD2 RID: 11218
		public static string fireBombSoundString;

		// Token: 0x04002BD3 RID: 11219
		public static float bombDamageCoefficient;

		// Token: 0x04002BD4 RID: 11220
		public static float bombForce;

		// Token: 0x04002BD5 RID: 11221
		public float novaRadius;

		// Token: 0x04002BD6 RID: 11222
		private float duration;

		// Token: 0x04002BD7 RID: 11223
		private float stopwatch;
	}
}
