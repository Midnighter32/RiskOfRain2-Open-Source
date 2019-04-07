using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.ClaymanMonster
{
	// Token: 0x020001B5 RID: 437
	public class SwipeForward : BaseState
	{
		// Token: 0x0600088E RID: 2190 RVA: 0x0002ADE8 File Offset: 0x00028FE8
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = SwipeForward.baseDuration / this.attackSpeedStat;
			this.modelAnimator = base.GetModelAnimator();
			Transform modelTransform = base.GetModelTransform();
			this.attack = new OverlapAttack();
			this.attack.attacker = base.gameObject;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = TeamComponent.GetObjectTeam(this.attack.attacker);
			this.attack.damage = SwipeForward.damageCoefficient * this.damageStat;
			this.attack.hitEffectPrefab = SwipeForward.hitEffectPrefab;
			this.attack.isCrit = Util.CheckRoll(this.critStat, base.characterBody.master);
			Util.PlaySound(SwipeForward.attackString, base.gameObject);
			if (modelTransform)
			{
				this.attack.hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Sword");
			}
			if (this.modelAnimator)
			{
				base.PlayAnimation("Gesture, Override", "SwipeForward", "SwipeForward.playbackRate", this.duration);
				base.PlayAnimation("Gesture, Additive", "SwipeForward", "SwipeForward.playbackRate", this.duration);
			}
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(2f);
			}
		}

		// Token: 0x0600088F RID: 2191 RVA: 0x0002AF60 File Offset: 0x00029160
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && this.modelAnimator && this.modelAnimator.GetFloat("SwipeForward.hitBoxActive") > 0.1f)
			{
				if (!this.hasSlashed)
				{
					EffectManager.instance.SimpleMuzzleFlash(SwipeForward.swingEffectPrefab, base.gameObject, "SwingCenter", true);
					HealthComponent healthComponent = base.characterBody.healthComponent;
					CharacterDirection component = base.characterBody.GetComponent<CharacterDirection>();
					if (healthComponent)
					{
						healthComponent.TakeDamageForce(SwipeForward.selfForceMagnitude * component.forward, true);
					}
					this.hasSlashed = true;
				}
				this.attack.forceVector = base.transform.forward * SwipeForward.forceMagnitude;
				this.attack.Fire(null);
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06000890 RID: 2192 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04000B6C RID: 2924
		public static float baseDuration = 3.5f;

		// Token: 0x04000B6D RID: 2925
		public static float damageCoefficient = 4f;

		// Token: 0x04000B6E RID: 2926
		public static float forceMagnitude = 16f;

		// Token: 0x04000B6F RID: 2927
		public static float selfForceMagnitude;

		// Token: 0x04000B70 RID: 2928
		public static float radius = 3f;

		// Token: 0x04000B71 RID: 2929
		public static GameObject hitEffectPrefab;

		// Token: 0x04000B72 RID: 2930
		public static GameObject swingEffectPrefab;

		// Token: 0x04000B73 RID: 2931
		public static string attackString;

		// Token: 0x04000B74 RID: 2932
		private OverlapAttack attack;

		// Token: 0x04000B75 RID: 2933
		private Animator modelAnimator;

		// Token: 0x04000B76 RID: 2934
		private float duration;

		// Token: 0x04000B77 RID: 2935
		private bool hasSlashed;
	}
}
