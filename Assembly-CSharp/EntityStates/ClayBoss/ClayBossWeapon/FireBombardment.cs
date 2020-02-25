using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.ClayBoss.ClayBossWeapon
{
	// Token: 0x020008DA RID: 2266
	public class FireBombardment : BaseState
	{
		// Token: 0x060032BC RID: 12988 RVA: 0x000DBA3C File Offset: 0x000D9C3C
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
				EffectManager.SimpleMuzzleFlash(FireBombardment.effectPrefab, base.gameObject, targetMuzzle, false);
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

		// Token: 0x060032BD RID: 12989 RVA: 0x000DBD38 File Offset: 0x000D9F38
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

		// Token: 0x060032BE RID: 12990 RVA: 0x000DBDBC File Offset: 0x000D9FBC
		public override void OnExit()
		{
			base.PlayCrossfade("Gesture, Additive", "EndBombardment", 0.1f);
			base.OnExit();
		}

		// Token: 0x060032BF RID: 12991 RVA: 0x000DBDDC File Offset: 0x000D9FDC
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

		// Token: 0x060032C0 RID: 12992 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040031E7 RID: 12775
		public static GameObject effectPrefab;

		// Token: 0x040031E8 RID: 12776
		public static GameObject projectilePrefab;

		// Token: 0x040031E9 RID: 12777
		public int grenadeCountMax = 3;

		// Token: 0x040031EA RID: 12778
		public static float damageCoefficient;

		// Token: 0x040031EB RID: 12779
		public static float baseTimeBetweenShots = 1f;

		// Token: 0x040031EC RID: 12780
		public static float cooldownDuration = 2f;

		// Token: 0x040031ED RID: 12781
		public static float arcAngle = 5f;

		// Token: 0x040031EE RID: 12782
		public static float recoilAmplitude = 1f;

		// Token: 0x040031EF RID: 12783
		public static string shootSoundString;

		// Token: 0x040031F0 RID: 12784
		public static float spreadBloomValue = 0.3f;

		// Token: 0x040031F1 RID: 12785
		private Ray aimRay;

		// Token: 0x040031F2 RID: 12786
		private Transform modelTransform;

		// Token: 0x040031F3 RID: 12787
		private float duration;

		// Token: 0x040031F4 RID: 12788
		private float fireTimer;

		// Token: 0x040031F5 RID: 12789
		private int grenadeCount;

		// Token: 0x040031F6 RID: 12790
		private float timeBetweenShots;
	}
}
