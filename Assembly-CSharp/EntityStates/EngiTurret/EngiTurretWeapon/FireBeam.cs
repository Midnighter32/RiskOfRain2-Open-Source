using System;
using RoR2;
using UnityEngine;

namespace EntityStates.EngiTurret.EngiTurretWeapon
{
	// Token: 0x02000180 RID: 384
	internal class FireBeam : BaseState
	{
		// Token: 0x0600075F RID: 1887 RVA: 0x00024190 File Offset: 0x00022390
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(FireBeam.attackSoundString, base.gameObject);
			this.fireTimer = 0f;
			this.aimRay = base.GetAimRay();
			this.modelTransform = base.GetModelTransform();
			if (this.modelTransform)
			{
				ChildLocator component = this.modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild("Muzzle");
					if (transform && FireBeam.laserPrefab)
					{
						this.laserEffect = UnityEngine.Object.Instantiate<GameObject>(FireBeam.laserPrefab, transform.position, transform.rotation);
						this.laserEffect.transform.parent = transform;
						this.laserLineComponent = this.laserEffect.GetComponent<LineRenderer>();
					}
				}
			}
		}

		// Token: 0x06000760 RID: 1888 RVA: 0x00024256 File Offset: 0x00022456
		public override void OnExit()
		{
			if (this.laserEffect)
			{
				EntityState.Destroy(this.laserEffect);
			}
			base.OnExit();
		}

		// Token: 0x06000761 RID: 1889 RVA: 0x00024278 File Offset: 0x00022478
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.aimRay = base.GetAimRay();
			this.fireTimer += Time.fixedDeltaTime;
			if (this.fireTimer > 1f / FireBeam.fireFrequency)
			{
				string targetMuzzle = "Muzzle";
				this.FireBullet(this.modelTransform, this.aimRay, targetMuzzle);
				this.fireTimer = 0f;
			}
			if (this.laserEffect && this.laserLineComponent)
			{
				float distance = FireBeam.maxDistance;
				Vector3 position = this.laserEffect.transform.parent.position;
				Vector3 point = this.aimRay.GetPoint(distance);
				RaycastHit raycastHit;
				if (Physics.Raycast(this.aimRay, out raycastHit, distance, LayerIndex.world.mask | LayerIndex.defaultLayer.mask))
				{
					point = raycastHit.point;
				}
				this.laserLineComponent.SetPosition(0, position);
				this.laserLineComponent.SetPosition(1, point);
			}
			if (base.isAuthority && (!base.inputBank || !base.inputBank.skill1.down))
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06000762 RID: 1890 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x06000763 RID: 1891 RVA: 0x000243B8 File Offset: 0x000225B8
		private void FireBullet(Transform modelTransform, Ray aimRay, string targetMuzzle)
		{
			if (FireBeam.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireBeam.effectPrefab, base.gameObject, targetMuzzle, false);
			}
			if (base.isAuthority)
			{
				BulletAttack bulletAttack = new BulletAttack();
				bulletAttack.owner = base.gameObject;
				bulletAttack.weapon = base.gameObject;
				bulletAttack.origin = aimRay.origin;
				bulletAttack.aimVector = aimRay.direction;
				bulletAttack.minSpread = FireBeam.minSpread;
				bulletAttack.maxSpread = FireBeam.maxSpread;
				bulletAttack.bulletCount = 1u;
				bulletAttack.damage = FireBeam.damageCoefficient * this.damageStat / FireBeam.fireFrequency;
				bulletAttack.force = FireBeam.force;
				bulletAttack.muzzleName = targetMuzzle;
				bulletAttack.hitEffectPrefab = FireBeam.hitEffectPrefab;
				bulletAttack.isCrit = Util.CheckRoll(this.critStat, base.characterBody.master);
				bulletAttack.HitEffectNormal = false;
				bulletAttack.radius = 0f;
				bulletAttack.maxDistance = FireBeam.maxDistance;
				bulletAttack.damageType |= DamageType.SlowOnHit;
				bulletAttack.Fire();
			}
		}

		// Token: 0x0400095A RID: 2394
		public static GameObject effectPrefab;

		// Token: 0x0400095B RID: 2395
		public static GameObject hitEffectPrefab;

		// Token: 0x0400095C RID: 2396
		public static GameObject laserPrefab;

		// Token: 0x0400095D RID: 2397
		public static string attackSoundString;

		// Token: 0x0400095E RID: 2398
		public static float damageCoefficient;

		// Token: 0x0400095F RID: 2399
		public static float force;

		// Token: 0x04000960 RID: 2400
		public static float minSpread;

		// Token: 0x04000961 RID: 2401
		public static float maxSpread;

		// Token: 0x04000962 RID: 2402
		public static int bulletCount;

		// Token: 0x04000963 RID: 2403
		public static float fireFrequency;

		// Token: 0x04000964 RID: 2404
		public static float maxDistance;

		// Token: 0x04000965 RID: 2405
		private float fireTimer;

		// Token: 0x04000966 RID: 2406
		private Ray aimRay;

		// Token: 0x04000967 RID: 2407
		private Transform modelTransform;

		// Token: 0x04000968 RID: 2408
		private GameObject laserEffect;

		// Token: 0x04000969 RID: 2409
		private LineRenderer laserLineComponent;

		// Token: 0x0400096A RID: 2410
		public int bulletCountCurrent = 1;
	}
}
