using System;
using RoR2;
using UnityEngine.Networking;

namespace EntityStates.ScavMonster
{
	// Token: 0x02000795 RID: 1941
	public class ExitSit : BaseState
	{
		// Token: 0x06002C80 RID: 11392 RVA: 0x000BBBE4 File Offset: 0x000B9DE4
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = ExitSit.baseDuration / this.attackSpeedStat;
			Util.PlaySound(ExitSit.soundString, base.gameObject);
			base.PlayCrossfade("Body", "ExitSit", "Sit.playbackRate", this.duration, 0.1f);
			base.modelLocator.normalizeToFloor = false;
		}

		// Token: 0x06002C81 RID: 11393 RVA: 0x000BBC46 File Offset: 0x000B9E46
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002C82 RID: 11394 RVA: 0x000BBC67 File Offset: 0x000B9E67
		public override void OnExit()
		{
			if (NetworkServer.active && base.characterBody && base.characterBody.HasBuff(BuffIndex.ArmorBoost))
			{
				base.characterBody.RemoveBuff(BuffIndex.ArmorBoost);
			}
			base.OnExit();
		}

		// Token: 0x04002890 RID: 10384
		public static float baseDuration;

		// Token: 0x04002891 RID: 10385
		public static string soundString;

		// Token: 0x04002892 RID: 10386
		private float duration;
	}
}
