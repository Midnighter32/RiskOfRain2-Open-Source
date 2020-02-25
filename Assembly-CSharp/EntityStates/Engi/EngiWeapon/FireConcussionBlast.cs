using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Engi.EngiWeapon
{
	// Token: 0x02000885 RID: 2181
	public class FireConcussionBlast : BaseState
	{
		// Token: 0x06003113 RID: 12563 RVA: 0x000D30E0 File Offset: 0x000D12E0
		private void FireGrenade(string targetMuzzle)
		{
			Util.PlaySound(FireConcussionBlast.attackSoundString, base.gameObject);
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
			base.AddRecoil(-1f * FireConcussionBlast.recoilAmplitude, -2f * FireConcussionBlast.recoilAmplitude, -1f * FireConcussionBlast.recoilAmplitude, 1f * FireConcussionBlast.recoilAmplitude);
			if (FireConcussionBlast.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireConcussionBlast.effectPrefab, base.gameObject, targetMuzzle, false);
			}
			if (base.isAuthority)
			{
				new BulletAttack
				{
					owner = base.gameObject,
					weapon = base.gameObject,
					origin = this.aimRay.origin,
					aimVector = this.aimRay.direction,
					minSpread = FireConcussionBlast.minSpread,
					maxSpread = FireConcussionBlast.maxSpread,
					damage = FireConcussionBlast.damageCoefficient * this.damageStat,
					force = FireConcussionBlast.force,
					tracerEffectPrefab = FireConcussionBlast.tracerEffectPrefab,
					muzzleName = targetMuzzle,
					hitEffectPrefab = FireConcussionBlast.hitEffectPrefab,
					isCrit = Util.CheckRoll(this.critStat, base.characterBody.master),
					maxDistance = FireConcussionBlast.maxDistance,
					radius = FireConcussionBlast.radius,
					stopperMask = 0
				}.Fire();
			}
		}

		// Token: 0x06003114 RID: 12564 RVA: 0x000D327C File Offset: 0x000D147C
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireConcussionBlast.baseDuration / this.attackSpeedStat;
			this.modelTransform = base.GetModelTransform();
			this.aimRay = base.GetAimRay();
			base.StartAimMode(this.aimRay, 2f, false);
		}

		// Token: 0x06003115 RID: 12565 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06003116 RID: 12566 RVA: 0x000D32CC File Offset: 0x000D14CC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				this.fireTimer -= Time.fixedDeltaTime;
				float num = FireConcussionBlast.fireDuration / this.attackSpeedStat / (float)FireConcussionBlast.grenadeCountMax;
				if (this.fireTimer <= 0f && this.grenadeCount < FireConcussionBlast.grenadeCountMax)
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

		// Token: 0x06003117 RID: 12567 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002F41 RID: 12097
		public static GameObject effectPrefab;

		// Token: 0x04002F42 RID: 12098
		public static GameObject hitEffectPrefab;

		// Token: 0x04002F43 RID: 12099
		public static int grenadeCountMax = 3;

		// Token: 0x04002F44 RID: 12100
		public static float damageCoefficient;

		// Token: 0x04002F45 RID: 12101
		public static float fireDuration = 1f;

		// Token: 0x04002F46 RID: 12102
		public static float baseDuration = 2f;

		// Token: 0x04002F47 RID: 12103
		public static float minSpread = 0f;

		// Token: 0x04002F48 RID: 12104
		public static float maxSpread = 5f;

		// Token: 0x04002F49 RID: 12105
		public static float recoilAmplitude = 1f;

		// Token: 0x04002F4A RID: 12106
		public static string attackSoundString;

		// Token: 0x04002F4B RID: 12107
		public static float force;

		// Token: 0x04002F4C RID: 12108
		public static float maxDistance;

		// Token: 0x04002F4D RID: 12109
		public static float radius;

		// Token: 0x04002F4E RID: 12110
		public static GameObject tracerEffectPrefab;

		// Token: 0x04002F4F RID: 12111
		private Ray aimRay;

		// Token: 0x04002F50 RID: 12112
		private Transform modelTransform;

		// Token: 0x04002F51 RID: 12113
		private float duration;

		// Token: 0x04002F52 RID: 12114
		private float fireTimer;

		// Token: 0x04002F53 RID: 12115
		private int grenadeCount;
	}
}
