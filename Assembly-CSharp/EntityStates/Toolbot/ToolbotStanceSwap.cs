using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Toolbot
{
	// Token: 0x020000E9 RID: 233
	public class ToolbotStanceSwap : ToolbotStanceBase
	{
		// Token: 0x06000482 RID: 1154 RVA: 0x00012DE8 File Offset: 0x00010FE8
		public override void OnEnter()
		{
			base.OnEnter();
			this.endTime = Run.FixedTimeStamp.now + this.baseDuration / this.attackSpeedStat;
			base.SetPrimarySkill(null);
			base.SetCrosshairParameters(ToolbotStanceSwap.crosshairPrefab, ToolbotStanceSwap.spreadCurve);
			if (this.nextStanceState == typeof(ToolbotStanceA))
			{
				base.PlayAnimation("Stance, Additive", "SpearToNailgun", "StanceSwap.playbackRate", this.baseDuration);
				Util.PlaySound(ToolbotStanceSwap.spearToNailgunSoundString, base.gameObject);
				return;
			}
			base.PlayAnimation("Stance, Additive", "NailgunToSpear", "StanceSwap.playbackRate", this.baseDuration);
			Util.PlaySound(ToolbotStanceSwap.nailgunToSpearSoundString, base.gameObject);
		}

		// Token: 0x06000483 RID: 1155 RVA: 0x00012E9F File Offset: 0x0001109F
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.hasAuthority && this.endTime.hasPassed)
			{
				this.outer.SetNextState(EntityState.Instantiate(this.nextStanceState));
				return;
			}
		}

		// Token: 0x0400044B RID: 1099
		[SerializeField]
		private float baseDuration = 0.5f;

		// Token: 0x0400044C RID: 1100
		public static string spearToNailgunSoundString;

		// Token: 0x0400044D RID: 1101
		public static string nailgunToSpearSoundString;

		// Token: 0x0400044E RID: 1102
		private Run.FixedTimeStamp endTime;

		// Token: 0x0400044F RID: 1103
		public Type nextStanceState;

		// Token: 0x04000450 RID: 1104
		public static GameObject crosshairPrefab;

		// Token: 0x04000451 RID: 1105
		public static AnimationCurve spreadCurve;
	}
}
