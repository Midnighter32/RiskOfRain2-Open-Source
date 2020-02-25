using System;
using RoR2;
using UnityEngine.Networking;

namespace EntityStates.ScavMonster
{
	// Token: 0x02000793 RID: 1939
	public class EnterSit : BaseState
	{
		// Token: 0x06002C7A RID: 11386 RVA: 0x000BBACC File Offset: 0x000B9CCC
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = EnterSit.baseDuration / this.attackSpeedStat;
			Util.PlaySound(EnterSit.soundString, base.gameObject);
			base.PlayCrossfade("Body", "EnterSit", "Sit.playbackRate", this.duration, 0.1f);
			base.modelLocator.normalizeToFloor = true;
			base.modelLocator.modelTransform.GetComponent<AimAnimator>().enabled = true;
			if (NetworkServer.active && base.characterBody)
			{
				base.characterBody.AddBuff(BuffIndex.ArmorBoost);
			}
		}

		// Token: 0x06002C7B RID: 11387 RVA: 0x000BBB64 File Offset: 0x000B9D64
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration)
			{
				this.outer.SetNextState(new FindItem());
			}
		}

		// Token: 0x0400288B RID: 10379
		public static float baseDuration;

		// Token: 0x0400288C RID: 10380
		public static string soundString;

		// Token: 0x0400288D RID: 10381
		private float duration;
	}
}
