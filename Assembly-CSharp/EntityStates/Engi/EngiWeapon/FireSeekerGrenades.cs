using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Engi.EngiWeapon
{
	// Token: 0x0200088A RID: 2186
	public class FireSeekerGrenades : BaseState
	{
		// Token: 0x06003129 RID: 12585 RVA: 0x000D38A4 File Offset: 0x000D1AA4
		private void FireGrenade(string targetMuzzle)
		{
			Util.PlaySound(FireSeekerGrenades.attackSoundString, base.gameObject);
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
			base.AddRecoil(-1f * FireSeekerGrenades.recoilAmplitude, -2f * FireSeekerGrenades.recoilAmplitude, -1f * FireSeekerGrenades.recoilAmplitude, 1f * FireSeekerGrenades.recoilAmplitude);
			if (FireSeekerGrenades.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireSeekerGrenades.effectPrefab, base.gameObject, targetMuzzle, false);
			}
			if (base.isAuthority)
			{
				float x = UnityEngine.Random.Range(FireSeekerGrenades.minSpread, FireSeekerGrenades.maxSpread);
				float z = UnityEngine.Random.Range(0f, 360f);
				Vector3 up = Vector3.up;
				Vector3 axis = Vector3.Cross(up, this.aimRay.direction);
				Vector3 vector = Quaternion.Euler(0f, 0f, z) * (Quaternion.Euler(x, 0f, 0f) * Vector3.forward);
				float y = vector.y;
				vector.y = 0f;
				float angle = Mathf.Atan2(vector.z, vector.x) * 57.29578f - 90f;
				float angle2 = Mathf.Atan2(y, vector.magnitude) * 57.29578f + FireSeekerGrenades.arcAngle;
				Vector3 forward = Quaternion.AngleAxis(angle, up) * (Quaternion.AngleAxis(angle2, axis) * this.aimRay.direction);
				ProjectileManager.instance.FireProjectile(FireSeekerGrenades.projectilePrefab, this.aimRay.origin, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * FireSeekerGrenades.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x0600312A RID: 12586 RVA: 0x000D3AA4 File Offset: 0x000D1CA4
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireSeekerGrenades.baseDuration / this.attackSpeedStat;
			this.modelTransform = base.GetModelTransform();
			this.aimRay = base.GetAimRay();
			base.StartAimMode(this.aimRay, 2f, false);
		}

		// Token: 0x0600312B RID: 12587 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x0600312C RID: 12588 RVA: 0x000D3AF4 File Offset: 0x000D1CF4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				this.fireTimer -= Time.fixedDeltaTime;
				float num = FireSeekerGrenades.fireDuration / this.attackSpeedStat / (float)FireSeekerGrenades.grenadeCountMax;
				if (this.fireTimer <= 0f && this.grenadeCount < FireSeekerGrenades.grenadeCountMax)
				{
					this.fireTimer += num;
					if (this.grenadeCount % 2 == 0)
					{
						this.FireGrenade("MuzzleLeft");
						base.PlayCrossfade("Gesture, Left Cannon", "FireGrenadeLeft", 0.1f);
					}
					else
					{
						this.FireGrenade("MuzzleRight");
						base.PlayCrossfade("Gesture, Right Cannon", "FireGrenadeRight", 0.1f);
					}
					this.grenadeCount++;
				}
				if (base.fixedAge >= this.duration)
				{
					this.outer.SetNextStateToMain();
					return;
				}
			}
		}

		// Token: 0x0600312D RID: 12589 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002F6B RID: 12139
		public static GameObject effectPrefab;

		// Token: 0x04002F6C RID: 12140
		public static GameObject hitEffectPrefab;

		// Token: 0x04002F6D RID: 12141
		public static GameObject projectilePrefab;

		// Token: 0x04002F6E RID: 12142
		public static int grenadeCountMax = 3;

		// Token: 0x04002F6F RID: 12143
		public static float damageCoefficient;

		// Token: 0x04002F70 RID: 12144
		public static float fireDuration = 1f;

		// Token: 0x04002F71 RID: 12145
		public static float baseDuration = 2f;

		// Token: 0x04002F72 RID: 12146
		public static float minSpread = 0f;

		// Token: 0x04002F73 RID: 12147
		public static float maxSpread = 5f;

		// Token: 0x04002F74 RID: 12148
		public static float arcAngle = 5f;

		// Token: 0x04002F75 RID: 12149
		public static float recoilAmplitude = 1f;

		// Token: 0x04002F76 RID: 12150
		public static string attackSoundString;

		// Token: 0x04002F77 RID: 12151
		private Ray aimRay;

		// Token: 0x04002F78 RID: 12152
		private Transform modelTransform;

		// Token: 0x04002F79 RID: 12153
		private float duration;

		// Token: 0x04002F7A RID: 12154
		private float fireTimer;

		// Token: 0x04002F7B RID: 12155
		private int grenadeCount;
	}
}
