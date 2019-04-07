using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Assassin.Weapon
{
	// Token: 0x020001E0 RID: 480
	public class SlashCombo : BaseState
	{
		// Token: 0x06000959 RID: 2393 RVA: 0x0002EE58 File Offset: 0x0002D058
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = SlashCombo.baseDuration / this.attackSpeedStat;
			this.modelAnimator = base.GetModelAnimator();
			Transform modelTransform = base.GetModelTransform();
			this.attack = new OverlapAttack();
			this.attack.attacker = base.gameObject;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = TeamComponent.GetObjectTeam(this.attack.attacker);
			this.attack.damage = SlashCombo.damageCoefficient * this.damageStat;
			this.attack.hitEffectPrefab = SlashCombo.hitEffectPrefab;
			this.attack.isCrit = Util.CheckRoll(this.critStat, base.characterBody.master);
			Util.PlaySound(SlashCombo.attackString, base.gameObject);
			string hitboxGroupName = "";
			string animationStateName = "";
			switch (this.slashComboPermutation)
			{
			case SlashCombo.SlashComboPermutation.Slash1:
				hitboxGroupName = "DaggerLeft";
				animationStateName = "SlashP1";
				break;
			case SlashCombo.SlashComboPermutation.Slash2:
				hitboxGroupName = "DaggerLeft";
				animationStateName = "SlashP2";
				break;
			case SlashCombo.SlashComboPermutation.Final:
				hitboxGroupName = "DaggerLeft";
				animationStateName = "SlashP3";
				break;
			}
			if (modelTransform)
			{
				this.attack.hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == hitboxGroupName);
			}
			if (this.modelAnimator)
			{
				base.PlayAnimation("Gesture, Override", animationStateName, "SlashCombo.playbackRate", this.duration * SlashCombo.mecanimDurationCoefficient);
				base.PlayAnimation("Gesture, Additive", animationStateName, "SlashCombo.playbackRate", this.duration * SlashCombo.mecanimDurationCoefficient);
			}
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(2f);
			}
		}

		// Token: 0x0600095A RID: 2394 RVA: 0x0002F028 File Offset: 0x0002D228
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && this.modelAnimator && this.modelAnimator.GetFloat("SlashCombo.hitBoxActive") > 0.1f)
			{
				if (!this.hasSlashed)
				{
					EffectManager.instance.SimpleMuzzleFlash(SlashCombo.swingEffectPrefab, base.gameObject, "SwingCenter", true);
					HealthComponent healthComponent = base.characterBody.healthComponent;
					CharacterDirection component = base.characterBody.GetComponent<CharacterDirection>();
					if (healthComponent)
					{
						healthComponent.TakeDamageForce(SlashCombo.selfForceMagnitude * component.forward, true);
					}
					this.hasSlashed = true;
				}
				this.attack.forceVector = base.transform.forward * SlashCombo.forceMagnitude;
				this.attack.Fire(null);
			}
			if (base.fixedAge < this.duration || !base.isAuthority)
			{
				return;
			}
			if (base.inputBank && base.inputBank.skill1.down)
			{
				SlashCombo slashCombo = new SlashCombo();
				switch (this.slashComboPermutation)
				{
				case SlashCombo.SlashComboPermutation.Slash1:
					slashCombo.slashComboPermutation = SlashCombo.SlashComboPermutation.Slash2;
					break;
				case SlashCombo.SlashComboPermutation.Slash2:
					slashCombo.slashComboPermutation = SlashCombo.SlashComboPermutation.Slash1;
					break;
				}
				this.outer.SetNextState(slashCombo);
				return;
			}
			this.outer.SetNextStateToMain();
		}

		// Token: 0x0600095B RID: 2395 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04000CA6 RID: 3238
		public static float baseDuration = 3.5f;

		// Token: 0x04000CA7 RID: 3239
		public static float mecanimDurationCoefficient;

		// Token: 0x04000CA8 RID: 3240
		public static float damageCoefficient = 4f;

		// Token: 0x04000CA9 RID: 3241
		public static float forceMagnitude = 16f;

		// Token: 0x04000CAA RID: 3242
		public static float selfForceMagnitude;

		// Token: 0x04000CAB RID: 3243
		public static float radius = 3f;

		// Token: 0x04000CAC RID: 3244
		public static GameObject hitEffectPrefab;

		// Token: 0x04000CAD RID: 3245
		public static GameObject swingEffectPrefab;

		// Token: 0x04000CAE RID: 3246
		public static string attackString;

		// Token: 0x04000CAF RID: 3247
		private OverlapAttack attack;

		// Token: 0x04000CB0 RID: 3248
		private Animator modelAnimator;

		// Token: 0x04000CB1 RID: 3249
		private float duration;

		// Token: 0x04000CB2 RID: 3250
		private bool hasSlashed;

		// Token: 0x04000CB3 RID: 3251
		public SlashCombo.SlashComboPermutation slashComboPermutation;

		// Token: 0x020001E1 RID: 481
		public enum SlashComboPermutation
		{
			// Token: 0x04000CB5 RID: 3253
			Slash1,
			// Token: 0x04000CB6 RID: 3254
			Slash2,
			// Token: 0x04000CB7 RID: 3255
			Final
		}
	}
}
