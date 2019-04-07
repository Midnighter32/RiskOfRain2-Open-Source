using System;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Toolbot
{
	// Token: 0x020000E8 RID: 232
	public class ToolbotStanceB : ToolbotStanceBase
	{
		// Token: 0x0600047F RID: 1151 RVA: 0x00012D98 File Offset: 0x00010F98
		public override void OnEnter()
		{
			base.OnEnter();
			this.swapStateType = typeof(ToolbotStanceA);
			base.SetPrimarySkill(this.primarySkillName);
			base.SetCrosshairParameters(ToolbotStanceB.crosshairPrefab, ToolbotStanceB.spreadCurve);
			if (NetworkServer.active)
			{
				base.SetEquipmentSlot(1);
			}
		}

		// Token: 0x06000480 RID: 1152 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x04000447 RID: 1095
		[SerializeField]
		public string primarySkillName;

		// Token: 0x04000448 RID: 1096
		[SerializeField]
		public string secondarySkillName;

		// Token: 0x04000449 RID: 1097
		public static GameObject crosshairPrefab;

		// Token: 0x0400044A RID: 1098
		public static AnimationCurve spreadCurve;
	}
}
