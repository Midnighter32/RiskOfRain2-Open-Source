using System;
using EntityStates;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002A4 RID: 676
	public class ComboSkill : GenericSkill
	{
		// Token: 0x06000DC7 RID: 3527 RVA: 0x00043F70 File Offset: 0x00042170
		protected new void Start()
		{
			base.Start();
		}

		// Token: 0x06000DC8 RID: 3528 RVA: 0x00043F78 File Offset: 0x00042178
		protected override void OnExecute()
		{
			this.activationState = this.comboList[this.comboCounter].comboActivationState;
			base.OnExecute();
			if (this.hasExecutedSuccessfully)
			{
				this.comboCounter++;
				if (this.comboCounter >= this.comboList.Length)
				{
					this.comboCounter = 0;
				}
			}
		}

		// Token: 0x06000DC9 RID: 3529 RVA: 0x00043FD4 File Offset: 0x000421D4
		private void Update()
		{
			this.icon = this.comboList[this.comboCounter].comboIcon;
		}

		// Token: 0x040011CA RID: 4554
		public ComboSkill.Combo[] comboList;

		// Token: 0x040011CB RID: 4555
		private int comboCounter;

		// Token: 0x020002A5 RID: 677
		[Serializable]
		public struct Combo
		{
			// Token: 0x040011CC RID: 4556
			public SerializableEntityStateType comboActivationState;

			// Token: 0x040011CD RID: 4557
			public Sprite comboIcon;
		}
	}
}
