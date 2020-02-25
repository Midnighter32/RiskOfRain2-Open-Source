using System;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Croco
{
	// Token: 0x020008AC RID: 2220
	public class Slash : BasicMeleeAttack, SteppedSkillDef.IStepSetter
	{
		// Token: 0x17000461 RID: 1121
		// (get) Token: 0x060031BF RID: 12735 RVA: 0x000D6653 File Offset: 0x000D4853
		private bool isComboFinisher
		{
			get
			{
				return this.step == 2;
			}
		}

		// Token: 0x17000462 RID: 1122
		// (get) Token: 0x060031C0 RID: 12736 RVA: 0x000D665E File Offset: 0x000D485E
		protected override bool allowExitFire
		{
			get
			{
				return base.characterBody && !base.characterBody.isSprinting;
			}
		}

		// Token: 0x060031C1 RID: 12737 RVA: 0x000D667D File Offset: 0x000D487D
		void SteppedSkillDef.IStepSetter.SetStep(int i)
		{
			this.step = i;
		}

		// Token: 0x060031C2 RID: 12738 RVA: 0x000D6688 File Offset: 0x000D4888
		public override void OnEnter()
		{
			if (this.isComboFinisher)
			{
				this.swingEffectPrefab = Slash.comboFinisherSwingEffectPrefab;
				this.hitPauseDuration = Slash.comboFinisherhitPauseDuration;
				this.damageCoefficient = Slash.comboFinisherDamageCoefficient;
				this.bloom = Slash.comboFinisherBloom;
			}
			base.OnEnter();
			base.characterDirection.forward = base.GetAimRay().direction;
			this.durationBeforeInterruptable = (this.isComboFinisher ? (Slash.comboFinisherBaseDurationBeforeInterruptable / this.attackSpeedStat) : (Slash.baseDurationBeforeInterruptable / this.attackSpeedStat));
		}

		// Token: 0x060031C3 RID: 12739 RVA: 0x000D6710 File Offset: 0x000D4910
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x060031C4 RID: 12740 RVA: 0x000D6718 File Offset: 0x000D4918
		protected override void AuthorityModifyOverlapAttack(OverlapAttack overlapAttack)
		{
			base.AuthorityModifyOverlapAttack(overlapAttack);
		}

		// Token: 0x060031C5 RID: 12741 RVA: 0x000D6724 File Offset: 0x000D4924
		protected override void PlayAnimation()
		{
			this.animationStateName = "";
			string soundString = null;
			switch (this.step)
			{
			case 0:
				this.animationStateName = "Slash1";
				soundString = Slash.slash1Sound;
				break;
			case 1:
				this.animationStateName = "Slash2";
				soundString = Slash.slash1Sound;
				break;
			case 2:
				this.animationStateName = "Slash3";
				soundString = Slash.slash3Sound;
				break;
			}
			float duration = Mathf.Max(this.duration, 0.2f);
			base.PlayCrossfade("Gesture, Additive", this.animationStateName, "Slash.playbackRate", duration, 0.05f);
			base.PlayCrossfade("Gesture, Override", this.animationStateName, "Slash.playbackRate", duration, 0.05f);
			Util.PlaySound(soundString, base.gameObject);
		}

		// Token: 0x060031C6 RID: 12742 RVA: 0x000D67E6 File Offset: 0x000D49E6
		protected override void OnMeleeHitAuthority()
		{
			base.OnMeleeHitAuthority();
			base.characterBody.AddSpreadBloom(this.bloom);
		}

		// Token: 0x060031C7 RID: 12743 RVA: 0x000D6800 File Offset: 0x000D4A00
		protected override void BeginMeleeAttack()
		{
			this.swingEffectMuzzleString = this.animationStateName;
			base.AddRecoil(-0.1f * Slash.recoilAmplitude, 0.1f * Slash.recoilAmplitude, -1f * Slash.recoilAmplitude, 1f * Slash.recoilAmplitude);
			base.BeginMeleeAttack();
		}

		// Token: 0x060031C8 RID: 12744 RVA: 0x000D6851 File Offset: 0x000D4A51
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write((byte)this.step);
		}

		// Token: 0x060031C9 RID: 12745 RVA: 0x000D6867 File Offset: 0x000D4A67
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			this.step = (int)reader.ReadByte();
		}

		// Token: 0x060031CA RID: 12746 RVA: 0x000D687C File Offset: 0x000D4A7C
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			if (base.fixedAge >= this.durationBeforeInterruptable && base.authorityHasFiredAtAll)
			{
				return InterruptPriority.Skill;
			}
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x0400304E RID: 12366
		public int step;

		// Token: 0x0400304F RID: 12367
		public static float recoilAmplitude;

		// Token: 0x04003050 RID: 12368
		public static float baseDurationBeforeInterruptable;

		// Token: 0x04003051 RID: 12369
		[SerializeField]
		public float bloom;

		// Token: 0x04003052 RID: 12370
		public static GameObject comboFinisherSwingEffectPrefab;

		// Token: 0x04003053 RID: 12371
		public static float comboFinisherhitPauseDuration;

		// Token: 0x04003054 RID: 12372
		public static float comboFinisherDamageCoefficient;

		// Token: 0x04003055 RID: 12373
		public static float comboFinisherBloom;

		// Token: 0x04003056 RID: 12374
		public static float comboFinisherBaseDurationBeforeInterruptable;

		// Token: 0x04003057 RID: 12375
		public static string slash1Sound;

		// Token: 0x04003058 RID: 12376
		public static string slash3Sound;

		// Token: 0x04003059 RID: 12377
		private string animationStateName;

		// Token: 0x0400305A RID: 12378
		private float durationBeforeInterruptable;
	}
}
