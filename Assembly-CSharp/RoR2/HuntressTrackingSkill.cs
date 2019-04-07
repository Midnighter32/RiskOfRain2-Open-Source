using System;

namespace RoR2
{
	// Token: 0x02000319 RID: 793
	public class HuntressTrackingSkill : GenericSkill
	{
		// Token: 0x0600105D RID: 4189 RVA: 0x000523E7 File Offset: 0x000505E7
		protected new void Start()
		{
			base.Start();
			this.huntressTracker = base.GetComponent<HuntressTracker>();
		}

		// Token: 0x0600105E RID: 4190 RVA: 0x000523FB File Offset: 0x000505FB
		public override bool CanExecute()
		{
			return (!this.huntressTracker || !(this.huntressTracker.GetTrackingTarget() == null)) && base.CanExecute();
		}

		// Token: 0x0400147B RID: 5243
		private HuntressTracker huntressTracker;
	}
}
