using System;
using RoR2;
using UnityEngine;

namespace EntityStates.GolemMonster
{
	// Token: 0x0200085F RID: 2143
	public class FireLaser : BaseState
	{
		// Token: 0x06003060 RID: 12384 RVA: 0x000D0520 File Offset: 0x000CE720
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
				EffectManager.SimpleMuzzleFlash(FireLaser.effectPrefab, base.gameObject, text, false);
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
							EffectManager.SpawnEffect(FireLaser.tracerEffectPrefab, effectData, true);
							EffectManager.SpawnEffect(FireLaser.hitEffectPrefab, effectData, true);
						}
					}
				}
			}
		}

		// Token: 0x06003061 RID: 12385 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06003062 RID: 12386 RVA: 0x000D0775 File Offset: 0x000CE975
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06003063 RID: 12387 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002E93 RID: 11923
		public static GameObject effectPrefab;

		// Token: 0x04002E94 RID: 11924
		public static GameObject hitEffectPrefab;

		// Token: 0x04002E95 RID: 11925
		public static GameObject tracerEffectPrefab;

		// Token: 0x04002E96 RID: 11926
		public static float damageCoefficient;

		// Token: 0x04002E97 RID: 11927
		public static float blastRadius;

		// Token: 0x04002E98 RID: 11928
		public static float force;

		// Token: 0x04002E99 RID: 11929
		public static float minSpread;

		// Token: 0x04002E9A RID: 11930
		public static float maxSpread;

		// Token: 0x04002E9B RID: 11931
		public static int bulletCount;

		// Token: 0x04002E9C RID: 11932
		public static float baseDuration = 2f;

		// Token: 0x04002E9D RID: 11933
		public static string attackSoundString;

		// Token: 0x04002E9E RID: 11934
		public Vector3 laserDirection;

		// Token: 0x04002E9F RID: 11935
		private float duration;

		// Token: 0x04002EA0 RID: 11936
		private Ray modifiedAimRay;
	}
}
