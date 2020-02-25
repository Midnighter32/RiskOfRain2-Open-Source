using System;
using RoR2;

namespace EntityStates.Barrel
{
	// Token: 0x020008F9 RID: 2297
	public class Opening : EntityState
	{
		// Token: 0x06003350 RID: 13136 RVA: 0x000DE90C File Offset: 0x000DCB0C
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Body", "Opening", "Opening.playbackRate", Opening.duration);
			if (base.sfxLocator)
			{
				Util.PlaySound(base.sfxLocator.openSound, base.gameObject);
			}
		}

		// Token: 0x06003351 RID: 13137 RVA: 0x000DE95D File Offset: 0x000DCB5D
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= Opening.duration)
			{
				this.outer.SetNextState(new Opened());
				return;
			}
		}

		// Token: 0x040032CD RID: 13005
		public static float duration = 1f;
	}
}
