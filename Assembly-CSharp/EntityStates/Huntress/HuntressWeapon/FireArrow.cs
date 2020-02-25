using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Huntress.HuntressWeapon
{
	// Token: 0x02000834 RID: 2100
	public class FireArrow : BaseState
	{
		// Token: 0x06002F84 RID: 12164 RVA: 0x000CB5D8 File Offset: 0x000C97D8
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
				EffectManager.SimpleMuzzleFlash(FireArrow.effectPrefab, base.gameObject, targetMuzzle, false);
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

		// Token: 0x06002F85 RID: 12165 RVA: 0x000CB7F0 File Offset: 0x000C99F0
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

		// Token: 0x06002F86 RID: 12166 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002F87 RID: 12167 RVA: 0x000CB86C File Offset: 0x000C9A6C
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

		// Token: 0x06002F88 RID: 12168 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002D2F RID: 11567
		public static GameObject effectPrefab;

		// Token: 0x04002D30 RID: 11568
		public static GameObject hitEffectPrefab;

		// Token: 0x04002D31 RID: 11569
		public static GameObject projectilePrefab;

		// Token: 0x04002D32 RID: 11570
		public static int arrowCountMax = 1;

		// Token: 0x04002D33 RID: 11571
		public float damageCoefficient;

		// Token: 0x04002D34 RID: 11572
		public static float fireDuration = 1f;

		// Token: 0x04002D35 RID: 11573
		public static float baseDuration = 2f;

		// Token: 0x04002D36 RID: 11574
		public static float arcAngle = 5f;

		// Token: 0x04002D37 RID: 11575
		public static float recoilAmplitude = 1f;

		// Token: 0x04002D38 RID: 11576
		public static string attackSoundString;

		// Token: 0x04002D39 RID: 11577
		public static float spreadBloomValue = 0.3f;

		// Token: 0x04002D3A RID: 11578
		public static float smallHopStrength;

		// Token: 0x04002D3B RID: 11579
		private Ray aimRay;

		// Token: 0x04002D3C RID: 11580
		private Transform modelTransform;

		// Token: 0x04002D3D RID: 11581
		private float duration;

		// Token: 0x04002D3E RID: 11582
		private float fireTimer;

		// Token: 0x04002D3F RID: 11583
		private int grenadeCount;
	}
}
