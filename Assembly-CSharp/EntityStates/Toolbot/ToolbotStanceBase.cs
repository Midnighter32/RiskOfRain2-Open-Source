using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Toolbot
{
	// Token: 0x020000E6 RID: 230
	public abstract class ToolbotStanceBase : BaseState
	{
		// Token: 0x06000477 RID: 1143 RVA: 0x00012C94 File Offset: 0x00010E94
		protected void SetPrimarySkill(string skillName)
		{
			if (base.skillLocator)
			{
				base.skillLocator.primary = base.skillLocator.FindSkill(skillName);
			}
		}

		// Token: 0x06000478 RID: 1144 RVA: 0x00012CBA File Offset: 0x00010EBA
		protected void SetSecondarySkill(string skillName)
		{
			if (base.skillLocator)
			{
				base.skillLocator.secondary = base.skillLocator.FindSkill(skillName);
			}
		}

		// Token: 0x06000479 RID: 1145 RVA: 0x00012CE0 File Offset: 0x00010EE0
		protected void SetCrosshairParameters(GameObject crosshairPrefab, AnimationCurve spreadCurve)
		{
			base.characterBody.crosshairPrefab = crosshairPrefab;
			base.characterBody.spreadBloomCurve = spreadCurve;
		}

		// Token: 0x0600047A RID: 1146 RVA: 0x00012CFA File Offset: 0x00010EFA
		protected void SetEquipmentSlot(byte i)
		{
			if (this.inventory)
			{
				this.inventory.SetActiveEquipmentSlot(i);
			}
		}

		// Token: 0x0600047B RID: 1147 RVA: 0x00012D15 File Offset: 0x00010F15
		public override void OnEnter()
		{
			base.OnEnter();
			this.inventory = (base.characterBody ? base.characterBody.inventory : null);
		}

		// Token: 0x04000441 RID: 1089
		protected Inventory inventory;

		// Token: 0x04000442 RID: 1090
		public Type swapStateType;
	}
}
