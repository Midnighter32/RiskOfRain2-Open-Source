using System;

namespace EntityStates.Toolbot
{
	// Token: 0x0200075C RID: 1884
	public abstract class AimGrenade : AimThrowableBase
	{
		// Token: 0x06002B91 RID: 11153 RVA: 0x000B7FFE File Offset: 0x000B61FE
		public override void OnEnter()
		{
			base.OnEnter();
			this.detonationRadius = 7f;
		}
	}
}
