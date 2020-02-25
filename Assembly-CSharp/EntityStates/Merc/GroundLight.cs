using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Merc
{
	// Token: 0x020007C4 RID: 1988
	public class GroundLight : BaseState
	{
		// Token: 0x06002D6B RID: 11627 RVA: 0x000C03D0 File Offset: 0x000BE5D0
		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;
			this.earlyExitDuration = GroundLight.baseEarlyExitDuration / this.attackSpeedStat;
			this.animator = base.GetModelAnimator();
			bool @bool = this.animator.GetBool("isMoving");
			bool bool2 = this.animator.GetBool("isGrounded");
			switch (this.comboState)
			{
			case GroundLight.ComboState.GroundLight1:
				this.attackDuration = GroundLight.baseComboAttackDuration / this.attackSpeedStat;
				this.overlapAttack = base.InitMeleeOverlap(GroundLight.comboDamageCoefficient, this.hitEffectPrefab, base.GetModelTransform(), "Sword");
				if (@bool || !bool2)
				{
					base.PlayAnimation("Gesture, Additive", "GroundLight1", "GroundLight.playbackRate", this.attackDuration);
					base.PlayAnimation("Gesture, Override", "GroundLight1", "GroundLight.playbackRate", this.attackDuration);
				}
				else
				{
					base.PlayAnimation("FullBody, Override", "GroundLight1", "GroundLight.playbackRate", this.attackDuration);
				}
				this.slashChildName = "GroundLight1Slash";
				this.swingEffectPrefab = GroundLight.comboSwingEffectPrefab;
				this.hitEffectPrefab = GroundLight.comboHitEffectPrefab;
				this.attackSoundString = GroundLight.comboAttackSoundString;
				break;
			case GroundLight.ComboState.GroundLight2:
				this.attackDuration = GroundLight.baseComboAttackDuration / this.attackSpeedStat;
				this.overlapAttack = base.InitMeleeOverlap(GroundLight.comboDamageCoefficient, this.hitEffectPrefab, base.GetModelTransform(), "Sword");
				if (@bool || !bool2)
				{
					base.PlayAnimation("Gesture, Additive", "GroundLight2", "GroundLight.playbackRate", this.attackDuration);
					base.PlayAnimation("Gesture, Override", "GroundLight2", "GroundLight.playbackRate", this.attackDuration);
				}
				else
				{
					base.PlayAnimation("FullBody, Override", "GroundLight2", "GroundLight.playbackRate", this.attackDuration);
				}
				this.slashChildName = "GroundLight2Slash";
				this.swingEffectPrefab = GroundLight.comboSwingEffectPrefab;
				this.hitEffectPrefab = GroundLight.comboHitEffectPrefab;
				this.attackSoundString = GroundLight.comboAttackSoundString;
				break;
			case GroundLight.ComboState.GroundLight3:
				this.attackDuration = GroundLight.baseFinisherAttackDuration / this.attackSpeedStat;
				this.overlapAttack = base.InitMeleeOverlap(GroundLight.finisherDamageCoefficient, this.hitEffectPrefab, base.GetModelTransform(), "SwordLarge");
				if (@bool || !bool2)
				{
					base.PlayAnimation("Gesture, Additive", "GroundLight3", "GroundLight.playbackRate", this.attackDuration);
					base.PlayAnimation("Gesture, Override", "GroundLight3", "GroundLight.playbackRate", this.attackDuration);
				}
				else
				{
					base.PlayAnimation("FullBody, Override", "GroundLight3", "GroundLight.playbackRate", this.attackDuration);
				}
				this.slashChildName = "GroundLight3Slash";
				this.swingEffectPrefab = GroundLight.finisherSwingEffectPrefab;
				this.hitEffectPrefab = GroundLight.finisherHitEffectPrefab;
				this.attackSoundString = GroundLight.finisherAttackSoundString;
				break;
			}
			base.characterBody.SetAimTimer(this.attackDuration + 1f);
			this.overlapAttack.hitEffectPrefab = this.hitEffectPrefab;
		}

		// Token: 0x06002D6C RID: 11628 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002D6D RID: 11629 RVA: 0x000C06AC File Offset: 0x000BE8AC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.hitPauseTimer -= Time.fixedDeltaTime;
			if (base.isAuthority)
			{
				bool flag = base.FireMeleeOverlap(this.overlapAttack, this.animator, "Sword.active", GroundLight.forceMagnitude, true);
				this.hasHit = (this.hasHit || flag);
				if (flag)
				{
					Util.PlaySound(GroundLight.hitSoundString, base.gameObject);
					if (!this.isInHitPause)
					{
						this.hitStopCachedState = base.CreateHitStopCachedState(base.characterMotor, this.animator, "GroundLight.playbackRate");
						this.hitPauseTimer = GroundLight.hitPauseDuration / this.attackSpeedStat;
						this.isInHitPause = true;
					}
				}
				if (this.hitPauseTimer <= 0f && this.isInHitPause)
				{
					base.ConsumeHitStopCachedState(this.hitStopCachedState, base.characterMotor, this.animator);
					this.isInHitPause = false;
				}
			}
			if (this.animator.GetFloat("Sword.active") > 0f && !this.hasSwung)
			{
				Util.PlayScaledSound(this.attackSoundString, base.gameObject, GroundLight.slashPitch);
				HealthComponent healthComponent = base.characterBody.healthComponent;
				CharacterDirection component = base.characterBody.GetComponent<CharacterDirection>();
				if (healthComponent)
				{
					healthComponent.TakeDamageForce(GroundLight.selfForceMagnitude * component.forward, true, false);
				}
				this.hasSwung = true;
				EffectManager.SimpleMuzzleFlash(this.swingEffectPrefab, base.gameObject, this.slashChildName, false);
			}
			if (!this.isInHitPause)
			{
				this.stopwatch += Time.fixedDeltaTime;
			}
			else
			{
				base.characterMotor.velocity = Vector3.zero;
				this.animator.SetFloat("GroundLight.playbackRate", 0f);
			}
			if (base.isAuthority && this.stopwatch >= this.attackDuration - this.earlyExitDuration)
			{
				if (!this.hasSwung)
				{
					this.overlapAttack.Fire(null);
				}
				if (base.inputBank.skill1.down && this.comboState != GroundLight.ComboState.GroundLight3)
				{
					GroundLight groundLight = new GroundLight();
					groundLight.comboState = this.comboState + 1;
					this.outer.SetNextState(groundLight);
					return;
				}
				if (this.stopwatch >= this.attackDuration)
				{
					this.outer.SetNextStateToMain();
					return;
				}
			}
		}

		// Token: 0x06002D6E RID: 11630 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x06002D6F RID: 11631 RVA: 0x000C08E7 File Offset: 0x000BEAE7
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write((byte)this.comboState);
		}

		// Token: 0x06002D70 RID: 11632 RVA: 0x000C08FD File Offset: 0x000BEAFD
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			this.comboState = (GroundLight.ComboState)reader.ReadByte();
		}

		// Token: 0x040029C9 RID: 10697
		public static float baseComboAttackDuration;

		// Token: 0x040029CA RID: 10698
		public static float baseFinisherAttackDuration;

		// Token: 0x040029CB RID: 10699
		public static float baseEarlyExitDuration;

		// Token: 0x040029CC RID: 10700
		public static string comboAttackSoundString;

		// Token: 0x040029CD RID: 10701
		public static string finisherAttackSoundString;

		// Token: 0x040029CE RID: 10702
		public static float comboDamageCoefficient;

		// Token: 0x040029CF RID: 10703
		public static float finisherDamageCoefficient;

		// Token: 0x040029D0 RID: 10704
		public static float forceMagnitude;

		// Token: 0x040029D1 RID: 10705
		public static GameObject comboHitEffectPrefab;

		// Token: 0x040029D2 RID: 10706
		public static GameObject finisherHitEffectPrefab;

		// Token: 0x040029D3 RID: 10707
		public static GameObject comboSwingEffectPrefab;

		// Token: 0x040029D4 RID: 10708
		public static GameObject finisherSwingEffectPrefab;

		// Token: 0x040029D5 RID: 10709
		public static float hitPauseDuration;

		// Token: 0x040029D6 RID: 10710
		public static float selfForceMagnitude;

		// Token: 0x040029D7 RID: 10711
		public static string hitSoundString;

		// Token: 0x040029D8 RID: 10712
		public static float slashPitch;

		// Token: 0x040029D9 RID: 10713
		private float stopwatch;

		// Token: 0x040029DA RID: 10714
		private float attackDuration;

		// Token: 0x040029DB RID: 10715
		private float earlyExitDuration;

		// Token: 0x040029DC RID: 10716
		private Animator animator;

		// Token: 0x040029DD RID: 10717
		private OverlapAttack overlapAttack;

		// Token: 0x040029DE RID: 10718
		private float hitPauseTimer;

		// Token: 0x040029DF RID: 10719
		private bool isInHitPause;

		// Token: 0x040029E0 RID: 10720
		private bool hasSwung;

		// Token: 0x040029E1 RID: 10721
		private bool hasHit;

		// Token: 0x040029E2 RID: 10722
		private GameObject swingEffectInstance;

		// Token: 0x040029E3 RID: 10723
		public GroundLight.ComboState comboState;

		// Token: 0x040029E4 RID: 10724
		private Vector3 characterForward;

		// Token: 0x040029E5 RID: 10725
		private string slashChildName;

		// Token: 0x040029E6 RID: 10726
		private BaseState.HitStopCachedState hitStopCachedState;

		// Token: 0x040029E7 RID: 10727
		private GameObject swingEffectPrefab;

		// Token: 0x040029E8 RID: 10728
		private GameObject hitEffectPrefab;

		// Token: 0x040029E9 RID: 10729
		private string attackSoundString;

		// Token: 0x020007C5 RID: 1989
		public enum ComboState
		{
			// Token: 0x040029EB RID: 10731
			GroundLight1,
			// Token: 0x040029EC RID: 10732
			GroundLight2,
			// Token: 0x040029ED RID: 10733
			GroundLight3
		}

		// Token: 0x020007C6 RID: 1990
		private struct ComboStateInfo
		{
			// Token: 0x040029EE RID: 10734
			private string mecanimStateName;

			// Token: 0x040029EF RID: 10735
			private string mecanimPlaybackRateName;
		}
	}
}
