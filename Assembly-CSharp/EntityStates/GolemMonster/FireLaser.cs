using System;
using RoR2;
using UnityEngine;

namespace EntityStates.GolemMonster
{
	// Token: 0x02000179 RID: 377
	internal class FireLaser : BaseState
	{
		// Token: 0x06000745 RID: 1861 RVA: 0x00023720 File Offset: 0x00021920
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireLaser.baseDuration / this.attackSpeedStat;
			this.modifiedAimRay = base.GetAimRay();
			this.modifiedAimRay.direction = this.laserDirection;
			base.GetModelAnimator();
			Transform modelTransform = base.GetModelTransform();
			Util.PlaySound(FireLaser.attackSoundString, base.gameObject);
			string text = "MuzzleLaser";
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(2f);
			}
			base.PlayAnimation("Gesture", "FireLaser", "FireLaser.playbackRate", this.duration);
			if (FireLaser.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireLaser.effectPrefab, base.gameObject, text, false);
			}
			if (base.isAuthority)
			{
				float num = 1000f;
				Vector3 vector = this.modifiedAimRay.origin + this.modifiedAimRay.direction * num;
				RaycastHit raycastHit;
				if (Physics.Raycast(this.modifiedAimRay, out raycastHit, num, LayerIndex.world.mask | LayerIndex.defaultLayer.mask | LayerIndex.entityPrecise.mask))
				{
					vector = raycastHit.point;
				}
				new BlastAttack
				{
					attacker = base.gameObject,
					inflictor = base.gameObject,
					teamIndex = TeamComponent.GetObjectTeam(base.gameObject),
					baseDamage = this.damageStat * FireLaser.damageCoefficient,
					baseForce = FireLaser.force * 0.2f,
					position = vector,
					radius = FireLaser.blastRadius,
					falloffModel = BlastAttack.FalloffModel.SweetSpot,
					bonusForce = FireLaser.force * this.modifiedAimRay.direction
				}.Fire();
				Vector3 origin = this.modifiedAimRay.origin;
				if (modelTransform)
				{
					ChildLocator component = modelTransform.GetComponent<ChildLocator>();
					if (component)
					{
						int childIndex = component.FindChildIndex(text);
						if (FireLaser.tracerEffectPrefab)
						{
							EffectData effectData = new EffectData
							{
								origin = vector,
								start = this.modifiedAimRay.origin
							};
							effectData.SetChildLocatorTransformReference(base.gameObject, childIndex);
							EffectManager.instance.SpawnEffect(FireLaser.tracerEffectPrefab, effectData, true);
							EffectManager.instance.SpawnEffect(FireLaser.hitEffectPrefab, effectData, true);
						}
					}
				}
			}
		}

		// Token: 0x06000746 RID: 1862 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000747 RID: 1863 RVA: 0x00023983 File Offset: 0x00021B83
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06000748 RID: 1864 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000926 RID: 2342
		public static GameObject effectPrefab;

		// Token: 0x04000927 RID: 2343
		public static GameObject hitEffectPrefab;

		// Token: 0x04000928 RID: 2344
		public static GameObject tracerEffectPrefab;

		// Token: 0x04000929 RID: 2345
		public static float damageCoefficient;

		// Token: 0x0400092A RID: 2346
		public static float blastRadius;

		// Token: 0x0400092B RID: 2347
		public static float force;

		// Token: 0x0400092C RID: 2348
		public static float minSpread;

		// Token: 0x0400092D RID: 2349
		public static float maxSpread;

		// Token: 0x0400092E RID: 2350
		public static int bulletCount;

		// Token: 0x0400092F RID: 2351
		public static float baseDuration = 2f;

		// Token: 0x04000930 RID: 2352
		public static string attackSoundString;

		// Token: 0x04000931 RID: 2353
		public Vector3 laserDirection;

		// Token: 0x04000932 RID: 2354
		private float duration;

		// Token: 0x04000933 RID: 2355
		private Ray modifiedAimRay;
	}
}
