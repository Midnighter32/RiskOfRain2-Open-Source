using System;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Toolbot
{
	// Token: 0x020000E7 RID: 231
	public class ToolbotStanceA : ToolbotStanceBase
	{
		// Token: 0x0600047D RID: 1149 RVA: 0x00012D40 File Offset: 0x00010F40
		public override void OnEnter()
		{
			base.OnEnter();
			this.swapStateType = typeof(ToolbotStanceB);
			base.SetPrimarySkill(this.primarySkillName);
			base.SetCrosshairParameters(ToolbotStanceA.crosshairPrefab, ToolbotStanceA.spreadCurve);
			if (NetworkServer.active)
			{
				base.SetEquipmentSlot(0);
			}
		}

		// Token: 0x04000443 RID: 1091
		[SerializeField]
		public string primarySkillName;

		// Token: 0x04000444 RID: 1092
		[SerializeField]
		public string secondarySkillName;

		// Token: 0x04000445 RID: 1093
		public static GameObject crosshairPrefab;

		// Token: 0x04000446 RID: 1094
		public static AnimationCurve spreadCurve;
	}
}
