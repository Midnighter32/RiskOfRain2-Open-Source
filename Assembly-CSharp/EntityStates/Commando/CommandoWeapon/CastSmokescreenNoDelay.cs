using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020001A3 RID: 419
	public class CastSmokescreenNoDelay : BaseState
	{
		// Token: 0x06000822 RID: 2082 RVA: 0x00028A24 File Offset: 0x00026C24
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

		// Token: 0x06000823 RID: 2083 RVA: 0x00028A78 File Offset: 0x00026C78
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

		// Token: 0x06000824 RID: 2084 RVA: 0x00028B5A File Offset: 0x00026D5A
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

		// Token: 0x06000825 RID: 2085 RVA: 0x00028B98 File Offset: 0x00026D98
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
			EffectManager.instance.SpawnEffect(CastSmokescreenNoDelay.smokescreenEffectPrefab, new EffectData
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

		// Token: 0x06000826 RID: 2086 RVA: 0x00028CBC File Offset: 0x00026EBC
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			if (this.stopwatch <= CastSmokescreenNoDelay.minimumStateDuration)
			{
				return InterruptPriority.PrioritySkill;
			}
			return InterruptPriority.Any;
		}

		// Token: 0x04000AAC RID: 2732
		public static float duration;

		// Token: 0x04000AAD RID: 2733
		public static float minimumStateDuration = 3f;

		// Token: 0x04000AAE RID: 2734
		public static string startCloakSoundString;

		// Token: 0x04000AAF RID: 2735
		public static string stopCloakSoundString;

		// Token: 0x04000AB0 RID: 2736
		public static GameObject smokescreenEffectPrefab;

		// Token: 0x04000AB1 RID: 2737
		public static Material destealthMaterial;

		// Token: 0x04000AB2 RID: 2738
		public static float damageCoefficient = 1.3f;

		// Token: 0x04000AB3 RID: 2739
		public static float radius = 4f;

		// Token: 0x04000AB4 RID: 2740
		public static float forceMagnitude = 100f;

		// Token: 0x04000AB5 RID: 2741
		private float stopwatch;

		// Token: 0x04000AB6 RID: 2742
		private bool hasCastSmoke;

		// Token: 0x04000AB7 RID: 2743
		private Animator animator;
	}
}
