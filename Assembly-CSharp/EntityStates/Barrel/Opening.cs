using System;
using RoR2;

namespace EntityStates.Barrel
{
	// Token: 0x020001DE RID: 478
	public class Opening : EntityState
	{
		// Token: 0x06000950 RID: 2384 RVA: 0x0002ECE8 File Offset: 0x0002CEE8
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Body", "Opening", "Opening.playbackRate", Opening.duration);
			if (base.sfxLocator)
			{
				Util.PlaySound(base.sfxLocator.openSound, base.gameObject);
			}
		}

		// Token: 0x06000951 RID: 2385 RVA: 0x0002ED39 File Offset: 0x0002CF39
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= Opening.duration)
			{
				this.outer.SetNextState(new Opened());
				return;
			}
		}

		// Token: 0x04000CA4 RID: 3236
		public static float duration = 1f;
	}
}
