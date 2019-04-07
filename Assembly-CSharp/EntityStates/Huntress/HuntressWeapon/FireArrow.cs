using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Huntress.HuntressWeapon
{
	// Token: 0x02000152 RID: 338
	internal class FireArrow : BaseState
	{
		// Token: 0x06000683 RID: 1667 RVA: 0x0001EF14 File Offset: 0x0001D114
		private void FireGrenade(string targetMuzzle)
		{
			Util.PlaySound(FireArrow.attackSoundString, base.gameObject);
			this.aimRay = base.GetAimRay();
			if (this.modelTransform)
			{
				ChildLocator component = this.modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild(targetMuzzle);
					if (transform)
					{
						this.aimRay.origin = transform.position;
					}
				}
			}
			base.AddRecoil(-1f * FireArrow.recoilAmplitude, -2f * FireArrow.recoilAmplitude, -1f * FireArrow.recoilAmplitude, 1f * FireArrow.recoilAmplitude);
			if (FireArrow.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireArrow.effectPrefab, base.gameObject, targetMuzzle, false);
			}
			if (base.isAuthority)
			{
				float x = UnityEngine.Random.Range(0f, base.characterBody.spreadBloomAngle);
				float z = UnityEngine.Random.Range(0f, 360f);
				Vector3 up = Vector3.up;
				Vector3 axis = Vector3.Cross(up, this.aimRay.direction);
				Vector3 vector = Quaternion.Euler(0f, 0f, z) * (Quaternion.Euler(x, 0f, 0f) * Vector3.forward);
				float y = vector.y;
				vector.y = 0f;
				float angle = Mathf.Atan2(vector.z, vector.x) * 57.29578f - 90f;
				float angle2 = Mathf.Atan2(y, vector.magnitude) * 57.29578f + FireArrow.arcAngle;
				Vector3 forward = Quaternion.AngleAxis(angle, up) * (Quaternion.AngleAxis(angle2, axis) * this.aimRay.direction);
				ProjectileManager.instance.FireProjectile(FireArrow.projectilePrefab, this.aimRay.origin, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * this.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
			base.characterBody.AddSpreadBloom(FireArrow.spreadBloomValue);
		}

		// Token: 0x06000684 RID: 1668 RVA: 0x0001F130 File Offset: 0x0001D330
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireArrow.baseDuration / this.attackSpeedStat;
			this.modelTransform = base.GetModelTransform();
			if (base.characterMotor && FireArrow.smallHopStrength != 0f)
			{
				base.characterMotor.velocity.y = FireArrow.smallHopStrength;
			}
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(2f);
			}
		}

		// Token: 0x06000685 RID: 1669 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000686 RID: 1670 RVA: 0x0001F1AC File Offset: 0x0001D3AC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				this.fireTimer -= Time.fixedDeltaTime;
				float num = FireArrow.fireDuration / this.attackSpeedStat / (float)FireArrow.arrowCountMax;
				if (this.fireTimer <= 0f && this.grenadeCount < FireArrow.arrowCountMax)
				{
					base.PlayAnimation("Gesture, Additive", "FireArrow", "FireArrow.playbackRate", this.duration - num);
					base.PlayAnimation("Gesture, Override", "FireArrow", "FireArrow.playbackRate", this.duration - num);
					this.FireGrenade("Muzzle");
					this.fireTimer += num;
					this.grenadeCount++;
				}
				if (base.fixedAge >= this.duration)
				{
					this.outer.SetNextStateToMain();
					return;
				}
			}
		}

		// Token: 0x06000687 RID: 1671 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040007E9 RID: 2025
		public static GameObject effectPrefab;

		// Token: 0x040007EA RID: 2026
		public static GameObject hitEffectPrefab;

		// Token: 0x040007EB RID: 2027
		public static GameObject projectilePrefab;

		// Token: 0x040007EC RID: 2028
		public static int arrowCountMax = 1;

		// Token: 0x040007ED RID: 2029
		public float damageCoefficient;

		// Token: 0x040007EE RID: 2030
		public static float fireDuration = 1f;

		// Token: 0x040007EF RID: 2031
		public static float baseDuration = 2f;

		// Token: 0x040007F0 RID: 2032
		public static float arcAngle = 5f;

		// Token: 0x040007F1 RID: 2033
		public static float recoilAmplitude = 1f;

		// Token: 0x040007F2 RID: 2034
		public static string attackSoundString;

		// Token: 0x040007F3 RID: 2035
		public static float spreadBloomValue = 0.3f;

		// Token: 0x040007F4 RID: 2036
		public static float smallHopStrength;

		// Token: 0x040007F5 RID: 2037
		private Ray aimRay;

		// Token: 0x040007F6 RID: 2038
		private Transform modelTransform;

		// Token: 0x040007F7 RID: 2039
		private float duration;

		// Token: 0x040007F8 RID: 2040
		private float fireTimer;

		// Token: 0x040007F9 RID: 2041
		private int grenadeCount;
	}
}
