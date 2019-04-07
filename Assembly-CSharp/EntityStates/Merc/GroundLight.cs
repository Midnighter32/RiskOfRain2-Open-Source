using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Merc
{
	// Token: 0x02000107 RID: 263
	internal class GroundLight : BaseState
	{
		// Token: 0x0600051B RID: 1307 RVA: 0x00016400 File Offset: 0x00014600
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

		// Token: 0x0600051C RID: 1308 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x0600051D RID: 1309 RVA: 0x000166DC File Offset: 0x000148DC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.hitPauseTimer -= Time.fixedDeltaTime;
			if (base.isAuthority)
			{
				bool flag = base.FireMeleeOverlap(this.overlapAttack, this.animator, "Sword.active", GroundLight.forceMagnitude);
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
					healthComponent.TakeDamageForce(GroundLight.selfForceMagnitude * component.forward, true);
				}
				this.hasSwung = true;
				EffectManager.instance.SimpleMuzzleFlash(this.swingEffectPrefab, base.gameObject, this.slashChildName, false);
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

		// Token: 0x0600051E RID: 1310 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x0600051F RID: 1311 RVA: 0x00016902 File Offset: 0x00014B02
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write((byte)this.comboState);
		}

		// Token: 0x06000520 RID: 1312 RVA: 0x00016918 File Offset: 0x00014B18
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			this.comboState = (GroundLight.ComboState)reader.ReadByte();
		}

		// Token: 0x04000525 RID: 1317
		public static float baseComboAttackDuration;

		// Token: 0x04000526 RID: 1318
		public static float baseFinisherAttackDuration;

		// Token: 0x04000527 RID: 1319
		public static float baseEarlyExitDuration;

		// Token: 0x04000528 RID: 1320
		public static string comboAttackSoundString;

		// Token: 0x04000529 RID: 1321
		public static string finisherAttackSoundString;

		// Token: 0x0400052A RID: 1322
		public static float comboDamageCoefficient;

		// Token: 0x0400052B RID: 1323
		public static float finisherDamageCoefficient;

		// Token: 0x0400052C RID: 1324
		public static float forceMagnitude;

		// Token: 0x0400052D RID: 1325
		public static GameObject comboHitEffectPrefab;

		// Token: 0x0400052E RID: 1326
		public static GameObject finisherHitEffectPrefab;

		// Token: 0x0400052F RID: 1327
		public static GameObject comboSwingEffectPrefab;

		// Token: 0x04000530 RID: 1328
		public static GameObject finisherSwingEffectPrefab;

		// Token: 0x04000531 RID: 1329
		public static float hitPauseDuration;

		// Token: 0x04000532 RID: 1330
		public static float selfForceMagnitude;

		// Token: 0x04000533 RID: 1331
		public static string hitSoundString;

		// Token: 0x04000534 RID: 1332
		public static float slashPitch;

		// Token: 0x04000535 RID: 1333
		private float stopwatch;

		// Token: 0x04000536 RID: 1334
		private float attackDuration;

		// Token: 0x04000537 RID: 1335
		private float earlyExitDuration;

		// Token: 0x04000538 RID: 1336
		private Animator animator;

		// Token: 0x04000539 RID: 1337
		private OverlapAttack overlapAttack;

		// Token: 0x0400053A RID: 1338
		private float hitPauseTimer;

		// Token: 0x0400053B RID: 1339
		private bool isInHitPause;

		// Token: 0x0400053C RID: 1340
		private bool hasSwung;

		// Token: 0x0400053D RID: 1341
		private bool hasHit;

		// Token: 0x0400053E RID: 1342
		private GameObject swingEffectInstance;

		// Token: 0x0400053F RID: 1343
		public GroundLight.ComboState comboState;

		// Token: 0x04000540 RID: 1344
		private Vector3 characterForward;

		// Token: 0x04000541 RID: 1345
		private string slashChildName;

		// Token: 0x04000542 RID: 1346
		private BaseState.HitStopCachedState hitStopCachedState;

		// Token: 0x04000543 RID: 1347
		private GameObject swingEffectPrefab;

		// Token: 0x04000544 RID: 1348
		private GameObject hitEffectPrefab;

		// Token: 0x04000545 RID: 1349
		private string attackSoundString;

		// Token: 0x02000108 RID: 264
		public enum ComboState
		{
			// Token: 0x04000547 RID: 1351
			GroundLight1,
			// Token: 0x04000548 RID: 1352
			GroundLight2,
			// Token: 0x04000549 RID: 1353
			GroundLight3
		}

		// Token: 0x02000109 RID: 265
		private struct ComboStateInfo
		{
			// Token: 0x0400054A RID: 1354
			private string mecanimStateName;

			// Token: 0x0400054B RID: 1355
			private string mecanimPlaybackRateName;
		}
	}
}
