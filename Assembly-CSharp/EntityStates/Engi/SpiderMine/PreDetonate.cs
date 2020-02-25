using System;

namespace EntityStates.Engi.SpiderMine
{
	// Token: 0x02000871 RID: 2161
	public class PreDetonate : BaseSpiderMineState
	{
		// Token: 0x060030B9 RID: 12473 RVA: 0x000D1D34 File Offset: 0x000CFF34
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = PreDetonate.baseDuration;
			base.rigidbody.isKinematic = true;
		}

		// Token: 0x060030BA RID: 12474 RVA: 0x000D1D53 File Offset: 0x000CFF53
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && this.duration <= base.fixedAge)
			{
				this.outer.SetNextState(new Detonate());
			}
		}

		// Token: 0x1700044F RID: 1103
		// (get) Token: 0x060030BB RID: 12475 RVA: 0x0000AC89 File Offset: 0x00008E89
		protected override bool shouldStick
		{
			get
			{
				return false;
			}
		}

		// Token: 0x04002F03 RID: 12035
		public static float baseDuration;

		// Token: 0x04002F04 RID: 12036
		private float duration;
	}
}
