using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Assassin.Weapon
{
	// Token: 0x020008FB RID: 2299
	public class SlashCombo : BaseState
	{
		// Token: 0x06003359 RID: 13145 RVA: 0x000DEA7C File Offset: 0x000DCC7C
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

		// Token: 0x0600335A RID: 13146 RVA: 0x000DEC4C File Offset: 0x000DCE4C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && this.modelAnimator && this.modelAnimator.GetFloat("SlashCombo.hitBoxActive") > 0.1f)
			{
				if (!this.hasSlashed)
				{
					EffectManager.SimpleMuzzleFlash(SlashCombo.swingEffectPrefab, base.gameObject, "SwingCenter", true);
					HealthComponent healthComponent = base.characterBody.healthComponent;
					CharacterDirection component = base.characterBody.GetComponent<CharacterDirection>();
					if (healthComponent)
					{
						healthComponent.TakeDamageForce(SlashCombo.selfForceMagnitude * component.forward, true, false);
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

		// Token: 0x0600335B RID: 13147 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x040032CF RID: 13007
		public static float baseDuration = 3.5f;

		// Token: 0x040032D0 RID: 13008
		public static float mecanimDurationCoefficient;

		// Token: 0x040032D1 RID: 13009
		public static float damageCoefficient = 4f;

		// Token: 0x040032D2 RID: 13010
		public static float forceMagnitude = 16f;

		// Token: 0x040032D3 RID: 13011
		public static float selfForceMagnitude;

		// Token: 0x040032D4 RID: 13012
		public static float radius = 3f;

		// Token: 0x040032D5 RID: 13013
		public static GameObject hitEffectPrefab;

		// Token: 0x040032D6 RID: 13014
		public static GameObject swingEffectPrefab;

		// Token: 0x040032D7 RID: 13015
		public static string attackString;

		// Token: 0x040032D8 RID: 13016
		private OverlapAttack attack;

		// Token: 0x040032D9 RID: 13017
		private Animator modelAnimator;

		// Token: 0x040032DA RID: 13018
		private float duration;

		// Token: 0x040032DB RID: 13019
		private bool hasSlashed;

		// Token: 0x040032DC RID: 13020
		public SlashCombo.SlashComboPermutation slashComboPermutation;

		// Token: 0x020008FC RID: 2300
		public enum SlashComboPermutation
		{
			// Token: 0x040032DE RID: 13022
			Slash1,
			// Token: 0x040032DF RID: 13023
			Slash2,
			// Token: 0x040032E0 RID: 13024
			Final
		}
	}
}
