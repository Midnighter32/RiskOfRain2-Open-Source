using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.ClayBoss.ClayBossWeapon
{
	// Token: 0x020001BF RID: 447
	internal class FireBombardment : BaseState
	{
		// Token: 0x060008BD RID: 2237 RVA: 0x0002BE08 File Offset: 0x0002A008
		private void FireGrenade(string targetMuzzle)
		{
			base.PlayCrossfade("Gesture, Bombardment", "FireBombardment", 0.1f);
			Util.PlaySound(FireBombardment.shootSoundString, base.gameObject);
			this.aimRay = base.GetAimRay();
			Vector3 vector = this.aimRay.origin;
			if (this.modelTransform)
			{
				ChildLocator component = this.modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild(targetMuzzle);
					if (transform)
					{
						vector = transform.position;
					}
				}
			}
			base.AddRecoil(-1f * FireBombardment.recoilAmplitude, -2f * FireBombardment.recoilAmplitude, -1f * FireBombardment.recoilAmplitude, 1f * FireBombardment.recoilAmplitude);
			if (FireBombardment.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireBombardment.effectPrefab, base.gameObject, targetMuzzle, false);
			}
			if (base.isAuthority)
			{
				float num = -1f;
				RaycastHit raycastHit;
				if (Util.CharacterRaycast(base.gameObject, this.aimRay, out raycastHit, float.PositiveInfinity, LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.Ignore))
				{
					Vector3 point = raycastHit.point;
					float velocity = FireBombardment.projectilePrefab.GetComponent<ProjectileSimple>().velocity;
					Vector3 vector2 = point - vector;
					Vector2 vector3 = new Vector2(vector2.x, vector2.z);
					float magnitude = vector3.magnitude;
					float y = Trajectory.CalculateInitialYSpeed(magnitude / velocity, vector2.y);
					Vector3 a = new Vector3(vector3.x / magnitude * velocity, y, vector3.y / magnitude * velocity);
					num = a.magnitude;
					this.aimRay.direction = a / num;
				}
				float x = UnityEngine.Random.Range(0f, base.characterBody.spreadBloomAngle);
				float z = UnityEngine.Random.Range(0f, 360f);
				Vector3 up = Vector3.up;
				Vector3 axis = Vector3.Cross(up, this.aimRay.direction);
				Vector3 vector4 = Quaternion.Euler(0f, 0f, z) * (Quaternion.Euler(x, 0f, 0f) * Vector3.forward);
				float y2 = vector4.y;
				vector4.y = 0f;
				float angle = Mathf.Atan2(vector4.z, vector4.x) * 57.29578f - 90f;
				float angle2 = Mathf.Atan2(y2, vector4.magnitude) * 57.29578f;
				Vector3 forward = Quaternion.AngleAxis(angle, up) * (Quaternion.AngleAxis(angle2, axis) * this.aimRay.direction);
				ProjectileManager.instance.FireProjectile(FireBombardment.projectilePrefab, vector, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * FireBombardment.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, num);
			}
			base.characterBody.AddSpreadBloom(FireBombardment.spreadBloomValue);
		}

		// Token: 0x060008BE RID: 2238 RVA: 0x0002C108 File Offset: 0x0002A308
		public override void OnEnter()
		{
			base.OnEnter();
			this.timeBetweenShots = FireBombardment.baseTimeBetweenShots / this.attackSpeedStat;
			this.duration = (FireBombardment.baseTimeBetweenShots * (float)this.grenadeCount + FireBombardment.cooldownDuration) / this.attackSpeedStat;
			base.PlayCrossfade("Gesture, Additive", "BeginBombardment", 0.1f);
			this.modelTransform = base.GetModelTransform();
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(this.duration);
			}
		}

		// Token: 0x060008BF RID: 2239 RVA: 0x0002C18C File Offset: 0x0002A38C
		public override void OnExit()
		{
			base.PlayCrossfade("Gesture, Additive", "EndBombardment", 0.1f);
			base.OnExit();
		}

		// Token: 0x060008C0 RID: 2240 RVA: 0x0002C1AC File Offset: 0x0002A3AC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				this.fireTimer -= Time.fixedDeltaTime;
				if (this.fireTimer <= 0f && this.grenadeCount < this.grenadeCountMax)
				{
					this.fireTimer += this.timeBetweenShots;
					this.FireGrenade("Muzzle");
					this.grenadeCount++;
				}
				if (base.fixedAge >= this.duration && base.isAuthority)
				{
					this.outer.SetNextStateToMain();
					return;
				}
			}
		}

		// Token: 0x060008C1 RID: 2241 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000BBF RID: 3007
		public static GameObject effectPrefab;

		// Token: 0x04000BC0 RID: 3008
		public static GameObject projectilePrefab;

		// Token: 0x04000BC1 RID: 3009
		public int grenadeCountMax = 3;

		// Token: 0x04000BC2 RID: 3010
		public static float damageCoefficient;

		// Token: 0x04000BC3 RID: 3011
		public static float baseTimeBetweenShots = 1f;

		// Token: 0x04000BC4 RID: 3012
		public static float cooldownDuration = 2f;

		// Token: 0x04000BC5 RID: 3013
		public static float arcAngle = 5f;

		// Token: 0x04000BC6 RID: 3014
		public static float recoilAmplitude = 1f;

		// Token: 0x04000BC7 RID: 3015
		public static string shootSoundString;

		// Token: 0x04000BC8 RID: 3016
		public static float spreadBloomValue = 0.3f;

		// Token: 0x04000BC9 RID: 3017
		private Ray aimRay;

		// Token: 0x04000BCA RID: 3018
		private Transform modelTransform;

		// Token: 0x04000BCB RID: 3019
		private float duration;

		// Token: 0x04000BCC RID: 3020
		private float fireTimer;

		// Token: 0x04000BCD RID: 3021
		private int grenadeCount;

		// Token: 0x04000BCE RID: 3022
		private float timeBetweenShots;
	}
}
