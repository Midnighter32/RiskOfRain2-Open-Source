using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Engi.EngiWeapon
{
	// Token: 0x02000886 RID: 2182
	public class FireGrenades : BaseState
	{
		// Token: 0x0600311A RID: 12570 RVA: 0x000D33E8 File Offset: 0x000D15E8
		private void FireGrenade(string targetMuzzle)
		{
			Util.PlaySound(FireGrenades.attackSoundString, base.gameObject);
			this.projectileRay = base.GetAimRay();
			if (this.modelTransform)
			{
				ChildLocator component = this.modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild(targetMuzzle);
					if (transform)
					{
						this.projectileRay.origin = transform.position;
					}
				}
			}
			base.AddRecoil(-1f * FireGrenades.recoilAmplitude, -2f * FireGrenades.recoilAmplitude, -1f * FireGrenades.recoilAmplitude, 1f * FireGrenades.recoilAmplitude);
			if (FireGrenades.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireGrenades.effectPrefab, base.gameObject, targetMuzzle, false);
			}
			if (base.isAuthority)
			{
				float x = UnityEngine.Random.Range(0f, base.characterBody.spreadBloomAngle);
				float z = UnityEngine.Random.Range(0f, 360f);
				Vector3 up = Vector3.up;
				Vector3 axis = Vector3.Cross(up, this.projectileRay.direction);
				Vector3 vector = Quaternion.Euler(0f, 0f, z) * (Quaternion.Euler(x, 0f, 0f) * Vector3.forward);
				float y = vector.y;
				vector.y = 0f;
				float angle = Mathf.Atan2(vector.z, vector.x) * 57.29578f - 90f;
				float angle2 = Mathf.Atan2(y, vector.magnitude) * 57.29578f + FireGrenades.arcAngle;
				Vector3 forward = Quaternion.AngleAxis(angle, up) * (Quaternion.AngleAxis(angle2, axis) * this.projectileRay.direction);
				ProjectileManager.instance.FireProjectile(FireGrenades.projectilePrefab, this.projectileRay.origin, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * FireGrenades.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
			base.characterBody.AddSpreadBloom(FireGrenades.spreadBloomValue);
		}

		// Token: 0x0600311B RID: 12571 RVA: 0x000D35FD File Offset: 0x000D17FD
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireGrenades.baseDuration / this.attackSpeedStat;
			this.modelTransform = base.GetModelTransform();
			base.StartAimMode(2f, false);
		}

		// Token: 0x0600311C RID: 12572 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x0600311D RID: 12573 RVA: 0x000D3630 File Offset: 0x000D1830
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				this.fireTimer -= Time.fixedDeltaTime;
				float num = FireGrenades.fireDuration / this.attackSpeedStat / (float)this.grenadeCountMax;
				if (this.fireTimer <= 0f && this.grenadeCount < this.grenadeCountMax)
				{
					this.fireTimer += num;
					if (this.grenadeCount % 2 == 0)
					{
						this.FireGrenade("MuzzleLeft");
						base.PlayCrossfade("Gesture Left Cannon, Additive", "FireGrenadeLeft", 0.1f);
					}
					else
					{
						this.FireGrenade("MuzzleRight");
						base.PlayCrossfade("Gesture Right Cannon, Additive", "FireGrenadeRight", 0.1f);
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

		// Token: 0x0600311E RID: 12574 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002F54 RID: 12116
		public static GameObject effectPrefab;

		// Token: 0x04002F55 RID: 12117
		public static GameObject projectilePrefab;

		// Token: 0x04002F56 RID: 12118
		public int grenadeCountMax = 3;

		// Token: 0x04002F57 RID: 12119
		public static float damageCoefficient;

		// Token: 0x04002F58 RID: 12120
		public static float fireDuration = 1f;

		// Token: 0x04002F59 RID: 12121
		public static float baseDuration = 2f;

		// Token: 0x04002F5A RID: 12122
		public static float arcAngle = 5f;

		// Token: 0x04002F5B RID: 12123
		public static float recoilAmplitude = 1f;

		// Token: 0x04002F5C RID: 12124
		public static string attackSoundString;

		// Token: 0x04002F5D RID: 12125
		public static float spreadBloomValue = 0.3f;

		// Token: 0x04002F5E RID: 12126
		private Ray projectileRay;

		// Token: 0x04002F5F RID: 12127
		private Transform modelTransform;

		// Token: 0x04002F60 RID: 12128
		private float duration;

		// Token: 0x04002F61 RID: 12129
		private float fireTimer;

		// Token: 0x04002F62 RID: 12130
		private int grenadeCount;
	}
}
