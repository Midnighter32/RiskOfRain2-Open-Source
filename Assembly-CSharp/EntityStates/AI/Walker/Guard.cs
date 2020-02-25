using System;

namespace EntityStates.AI.Walker
{
	// Token: 0x02000901 RID: 2305
	public class Guard : LookBusy
	{
		// Token: 0x0600337B RID: 13179 RVA: 0x000DF714 File Offset: 0x000DD914
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			base.fixedAge = 0f;
		}

		// Token: 0x0600337C RID: 13180 RVA: 0x0000409B File Offset: 0x0000229B
		protected override void PickNewTargetLookDirection()
		{
		}
	}
}
