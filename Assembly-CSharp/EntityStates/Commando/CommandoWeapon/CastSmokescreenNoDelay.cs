using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020008B4 RID: 2228
	public class CastSmokescreenNoDelay : BaseState
	{
		// Token: 0x060031F4 RID: 12788 RVA: 0x000D7988 File Offset: 0x000D5B88
		public override void OnEnter()
		{
			base.OnEnter();
			this.animator = base.GetModelAnimator();
			this.CastSmoke();
			if (base.characterBody && NetworkServer.active)
			{
				base.characterBody.AddBuff(BuffIndex.Cloak);
				base.characterBody.AddBuff(BuffIndex.CloakSpeed);
			}
		}

		// Token: 0x060031F5 RID: 12789 RVA: 0x000D79DC File Offset: 0x000D5BDC
		public override void OnExit()
		{
			if (base.characterBody && NetworkServer.active)
			{
				if (base.characterBody.HasBuff(BuffIndex.Cloak))
				{
					base.characterBody.RemoveBuff(BuffIndex.Cloak);
				}
				if (base.characterBody.HasBuff(BuffIndex.CloakSpeed))
				{
					base.characterBody.RemoveBuff(BuffIndex.CloakSpeed);
				}
			}
			if (!this.outer.destroying)
			{
				this.CastSmoke();
			}
			if (CastSmokescreenNoDelay.destealthMaterial)
			{
				TemporaryOverlay temporaryOverlay = this.animator.gameObject.AddComponent<TemporaryOverlay>();
				temporaryOverlay.duration = 1f;
				temporaryOverlay.destroyComponentOnEnd = true;
				temporaryOverlay.originalMaterial = CastSmokescreenNoDelay.destealthMaterial;
				temporaryOverlay.inspectorCharacterModel = this.animator.gameObject.GetComponent<CharacterModel>();
				temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
				temporaryOverlay.animateShaderAlpha = true;
			}
			base.OnExit();
		}

		// Token: 0x060031F6 RID: 12790 RVA: 0x000D7ABE File Offset: 0x000D5CBE
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= CastSmokescreenNoDelay.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060031F7 RID: 12791 RVA: 0x000D7AFC File Offset: 0x000D5CFC
		private void CastSmoke()
		{
			if (!this.hasCastSmoke)
			{
				Util.PlaySound(CastSmokescreenNoDelay.startCloakSoundString, base.gameObject);
				this.hasCastSmoke = true;
			}
			else
			{
				Util.PlaySound(CastSmokescreenNoDelay.stopCloakSoundString, base.gameObject);
			}
			EffectManager.SpawnEffect(CastSmokescreenNoDelay.smokescreenEffectPrefab, new EffectData
			{
				origin = base.transform.position
			}, false);
			int layerIndex = this.animator.GetLayerIndex("Impact");
			if (layerIndex >= 0)
			{
				this.animator.SetLayerWeight(layerIndex, 1f);
				this.animator.PlayInFixedTime("LightImpact", layerIndex, 0f);
			}
			if (NetworkServer.active)
			{
				new BlastAttack
				{
					attacker = base.gameObject,
					inflictor = base.gameObject,
					teamIndex = TeamComponent.GetObjectTeam(base.gameObject),
					baseDamage = this.damageStat * CastSmokescreenNoDelay.damageCoefficient,
					baseForce = CastSmokescreenNoDelay.forceMagnitude,
					position = base.transform.position,
					radius = CastSmokescreenNoDelay.radius,
					falloffModel = BlastAttack.FalloffModel.None,
					damageType = DamageType.Stun1s
				}.Fire();
			}
		}

		// Token: 0x060031F8 RID: 12792 RVA: 0x000D7C1C File Offset: 0x000D5E1C
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			if (this.stopwatch <= CastSmokescreenNoDelay.minimumStateDuration)
			{
				return InterruptPriority.PrioritySkill;
			}
			return InterruptPriority.Any;
		}

		// Token: 0x04003094 RID: 12436
		public static float duration;

		// Token: 0x04003095 RID: 12437
		public static float minimumStateDuration = 3f;

		// Token: 0x04003096 RID: 12438
		public static string startCloakSoundString;

		// Token: 0x04003097 RID: 12439
		public static string stopCloakSoundString;

		// Token: 0x04003098 RID: 12440
		public static GameObject smokescreenEffectPrefab;

		// Token: 0x04003099 RID: 12441
		public static Material destealthMaterial;

		// Token: 0x0400309A RID: 12442
		public static float damageCoefficient = 1.3f;

		// Token: 0x0400309B RID: 12443
		public static float radius = 4f;

		// Token: 0x0400309C RID: 12444
		public static float forceMagnitude = 100f;

		// Token: 0x0400309D RID: 12445
		private float stopwatch;

		// Token: 0x0400309E RID: 12446
		private bool hasCastSmoke;

		// Token: 0x0400309F RID: 12447
		private Animator animator;
	}
}
