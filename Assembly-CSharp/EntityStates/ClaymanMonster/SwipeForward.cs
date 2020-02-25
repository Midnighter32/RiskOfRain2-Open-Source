using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.ClaymanMonster
{
	// Token: 0x020008CF RID: 2255
	public class SwipeForward : BaseState
	{
		// Token: 0x06003289 RID: 12937 RVA: 0x000DA91C File Offset: 0x000D8B1C
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

		// Token: 0x0600328A RID: 12938 RVA: 0x000DAA94 File Offset: 0x000D8C94
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && this.modelAnimator && this.modelAnimator.GetFloat("SwipeForward.hitBoxActive") > 0.1f)
			{
				if (!this.hasSlashed)
				{
					EffectManager.SimpleMuzzleFlash(SwipeForward.swingEffectPrefab, base.gameObject, "SwingCenter", true);
					HealthComponent healthComponent = base.characterBody.healthComponent;
					CharacterDirection component = base.characterBody.GetComponent<CharacterDirection>();
					if (healthComponent)
					{
						healthComponent.TakeDamageForce(SwipeForward.selfForceMagnitude * component.forward, true, false);
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

		// Token: 0x0600328B RID: 12939 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x0400318D RID: 12685
		public static float baseDuration = 3.5f;

		// Token: 0x0400318E RID: 12686
		public static float damageCoefficient = 4f;

		// Token: 0x0400318F RID: 12687
		public static float forceMagnitude = 16f;

		// Token: 0x04003190 RID: 12688
		public static float selfForceMagnitude;

		// Token: 0x04003191 RID: 12689
		public static float radius = 3f;

		// Token: 0x04003192 RID: 12690
		public static GameObject hitEffectPrefab;

		// Token: 0x04003193 RID: 12691
		public static GameObject swingEffectPrefab;

		// Token: 0x04003194 RID: 12692
		public static string attackString;

		// Token: 0x04003195 RID: 12693
		private OverlapAttack attack;

		// Token: 0x04003196 RID: 12694
		private Animator modelAnimator;

		// Token: 0x04003197 RID: 12695
		private float duration;

		// Token: 0x04003198 RID: 12696
		private bool hasSlashed;
	}
}
