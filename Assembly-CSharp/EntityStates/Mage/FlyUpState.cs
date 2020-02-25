using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Mage
{
	// Token: 0x020007CC RID: 1996
	public class FlyUpState : MageCharacterMain
	{
		// Token: 0x06002D84 RID: 11652 RVA: 0x000C1058 File Offset: 0x000BF258
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(FlyUpState.beginSoundString, base.gameObject);
			this.modelTransform = base.GetModelTransform();
			this.flyVector = Vector3.up;
			this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
			base.PlayCrossfade("Body", "FlyUp", "FlyUp.playbackRate", FlyUpState.duration, 0.1f);
			base.characterMotor.Motor.ForceUnground();
			base.characterMotor.velocity = Vector3.zero;
			EffectManager.SimpleMuzzleFlash(FlyUpState.muzzleflashEffect, base.gameObject, "MuzzleLeft", false);
			EffectManager.SimpleMuzzleFlash(FlyUpState.muzzleflashEffect, base.gameObject, "MuzzleRight", false);
			if (base.isAuthority)
			{
				this.blastPosition = base.characterBody.corePosition;
			}
			if (NetworkServer.active)
			{
				BlastAttack blastAttack = new BlastAttack();
				blastAttack.radius = FlyUpState.blastAttackRadius;
				blastAttack.procCoefficient = FlyUpState.blastAttackProcCoefficient;
				blastAttack.position = this.blastPosition;
				blastAttack.attacker = base.gameObject;
				blastAttack.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
				blastAttack.baseDamage = base.characterBody.damage * FlyUpState.blastAttackDamageCoefficient;
				blastAttack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
				blastAttack.baseForce = FlyUpState.blastAttackForce;
				blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
				blastAttack.damageType = DamageType.Stun1s;
				blastAttack.Fire();
			}
		}

		// Token: 0x06002D85 RID: 11653 RVA: 0x000C11CE File Offset: 0x000BF3CE
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write(this.blastPosition);
		}

		// Token: 0x06002D86 RID: 11654 RVA: 0x000C11E3 File Offset: 0x000BF3E3
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			this.blastPosition = reader.ReadVector3();
		}

		// Token: 0x06002D87 RID: 11655 RVA: 0x000C11F8 File Offset: 0x000BF3F8
		public override void HandleMovements()
		{
			base.HandleMovements();
			base.characterMotor.rootMotion += this.flyVector * (this.moveSpeedStat * FlyUpState.speedCoefficientCurve.Evaluate(base.fixedAge / FlyUpState.duration) * Time.fixedDeltaTime);
			base.characterMotor.velocity.y = 0f;
		}

		// Token: 0x06002D88 RID: 11656 RVA: 0x000C1264 File Offset: 0x000BF464
		protected override void UpdateAnimationParameters()
		{
			base.UpdateAnimationParameters();
		}

		// Token: 0x06002D89 RID: 11657 RVA: 0x000C126C File Offset: 0x000BF46C
		private void CreateBlinkEffect(Vector3 origin)
		{
			EffectData effectData = new EffectData();
			effectData.rotation = Util.QuaternionSafeLookRotation(this.flyVector);
			effectData.origin = origin;
			EffectManager.SpawnEffect(FlyUpState.blinkPrefab, effectData, false);
		}

		// Token: 0x06002D8A RID: 11658 RVA: 0x000C12A3 File Offset: 0x000BF4A3
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= FlyUpState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002D8B RID: 11659 RVA: 0x000C12CB File Offset: 0x000BF4CB
		public override void OnExit()
		{
			if (!this.outer.destroying)
			{
				Util.PlaySound(FlyUpState.endSoundString, base.gameObject);
			}
			base.OnExit();
		}

		// Token: 0x04002A1B RID: 10779
		public static GameObject blinkPrefab;

		// Token: 0x04002A1C RID: 10780
		public static float duration = 0.3f;

		// Token: 0x04002A1D RID: 10781
		public static string beginSoundString;

		// Token: 0x04002A1E RID: 10782
		public static string endSoundString;

		// Token: 0x04002A1F RID: 10783
		public static AnimationCurve speedCoefficientCurve;

		// Token: 0x04002A20 RID: 10784
		public static GameObject muzzleflashEffect;

		// Token: 0x04002A21 RID: 10785
		public static float blastAttackRadius;

		// Token: 0x04002A22 RID: 10786
		public static float blastAttackDamageCoefficient;

		// Token: 0x04002A23 RID: 10787
		public static float blastAttackProcCoefficient;

		// Token: 0x04002A24 RID: 10788
		public static float blastAttackForce;

		// Token: 0x04002A25 RID: 10789
		private Vector3 flyVector = Vector3.zero;

		// Token: 0x04002A26 RID: 10790
		private Transform modelTransform;

		// Token: 0x04002A27 RID: 10791
		private CharacterModel characterModel;

		// Token: 0x04002A28 RID: 10792
		private HurtBoxGroup hurtboxGroup;

		// Token: 0x04002A29 RID: 10793
		private Vector3 blastPosition;
	}
}
