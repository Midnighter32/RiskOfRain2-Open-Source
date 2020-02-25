using System;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace EntityStates.Toolbot
{
	// Token: 0x0200076D RID: 1901
	public class ToolbotStanceBase : BaseState
	{
		// Token: 0x06002BD1 RID: 11217 RVA: 0x000B9588 File Offset: 0x000B7788
		protected void SetPrimarySkill()
		{
			base.skillLocator.primary = this.GetCurrentPrimarySkill();
		}

		// Token: 0x06002BD2 RID: 11218 RVA: 0x000B959B File Offset: 0x000B779B
		protected void SetSecondarySkill(string skillName)
		{
			if (base.skillLocator)
			{
				base.skillLocator.secondary = base.skillLocator.FindSkill(skillName);
			}
		}

		// Token: 0x06002BD3 RID: 11219 RVA: 0x000B95C1 File Offset: 0x000B77C1
		protected string GetSkillSlotStance(GenericSkill skillSlot)
		{
			ToolbotWeaponSkillDef toolbotWeaponSkillDef = skillSlot.skillDef as ToolbotWeaponSkillDef;
			return ((toolbotWeaponSkillDef != null) ? toolbotWeaponSkillDef.stanceName : null) ?? string.Empty;
		}

		// Token: 0x06002BD4 RID: 11220 RVA: 0x000B95E3 File Offset: 0x000B77E3
		protected GenericSkill GetPrimarySkill1()
		{
			return base.skillLocator.FindSkillByFamilyName("ToolbotBodyPrimary1");
		}

		// Token: 0x06002BD5 RID: 11221 RVA: 0x000B95F5 File Offset: 0x000B77F5
		protected GenericSkill GetPrimarySkill2()
		{
			return base.skillLocator.FindSkillByFamilyName("ToolbotBodyPrimary2");
		}

		// Token: 0x06002BD6 RID: 11222 RVA: 0x0000AC7F File Offset: 0x00008E7F
		protected virtual GenericSkill GetCurrentPrimarySkill()
		{
			return null;
		}

		// Token: 0x06002BD7 RID: 11223 RVA: 0x000B9608 File Offset: 0x000B7808
		protected void UpdateCrosshairParameters(ToolbotWeaponSkillDef weaponSkillDef)
		{
			GameObject crosshairPrefab = ToolbotStanceBase.emptyCrosshairPrefab;
			AnimationCurve crosshairSpreadCurve = ToolbotStanceBase.emptyCrosshairSpreadCurve;
			if (weaponSkillDef)
			{
				crosshairPrefab = weaponSkillDef.crosshairPrefab;
				crosshairSpreadCurve = weaponSkillDef.crosshairSpreadCurve;
			}
			base.characterBody.crosshairPrefab = crosshairPrefab;
			base.characterBody.spreadBloomCurve = crosshairSpreadCurve;
		}

		// Token: 0x06002BD8 RID: 11224 RVA: 0x000B964F File Offset: 0x000B784F
		protected void SetEquipmentSlot(byte i)
		{
			if (this.inventory)
			{
				this.inventory.SetActiveEquipmentSlot(i);
			}
		}

		// Token: 0x06002BD9 RID: 11225 RVA: 0x000B966C File Offset: 0x000B786C
		public override void OnEnter()
		{
			base.OnEnter();
			this.inventory = (base.characterBody ? base.characterBody.inventory : null);
			this.SetPrimarySkill();
			GenericSkill currentPrimarySkill = this.GetCurrentPrimarySkill();
			ToolbotWeaponSkillDef toolbotWeaponSkillDef = ((currentPrimarySkill != null) ? currentPrimarySkill.skillDef : null) as ToolbotWeaponSkillDef;
			if (toolbotWeaponSkillDef)
			{
				this.SendWeaponStanceToAnimator(toolbotWeaponSkillDef);
			}
			this.UpdateCrosshairParameters(toolbotWeaponSkillDef);
		}

		// Token: 0x06002BDA RID: 11226 RVA: 0x000B96D4 File Offset: 0x000B78D4
		protected void SendWeaponStanceToAnimator(ToolbotWeaponSkillDef weaponSkillDef)
		{
			if (weaponSkillDef)
			{
				base.GetModelAnimator().SetInteger("weaponStance", weaponSkillDef.animatorWeaponIndex);
			}
		}

		// Token: 0x06002BDB RID: 11227 RVA: 0x000B96F4 File Offset: 0x000B78F4
		protected static ToolbotStanceBase.WeaponStance GetSkillStance(GenericSkill skillSlot)
		{
			ToolbotWeaponSkillDef toolbotWeaponSkillDef = ((skillSlot != null) ? skillSlot.skillDef : null) as ToolbotWeaponSkillDef;
			string a = (toolbotWeaponSkillDef != null) ? toolbotWeaponSkillDef.stanceName : null;
			if (a == "Nailgun")
			{
				return ToolbotStanceBase.WeaponStance.Nailgun;
			}
			if (a == "Spear")
			{
				return ToolbotStanceBase.WeaponStance.Spear;
			}
			if (a == "Grenade")
			{
				return ToolbotStanceBase.WeaponStance.Grenade;
			}
			if (!(a == "Buzzsaw"))
			{
				return ToolbotStanceBase.WeaponStance.None;
			}
			return ToolbotStanceBase.WeaponStance.Buzzsaw;
		}

		// Token: 0x040027FB RID: 10235
		public static GameObject emptyCrosshairPrefab;

		// Token: 0x040027FC RID: 10236
		public static AnimationCurve emptyCrosshairSpreadCurve;

		// Token: 0x040027FD RID: 10237
		protected Inventory inventory;

		// Token: 0x040027FE RID: 10238
		public Type swapStateType;

		// Token: 0x0200076E RID: 1902
		protected enum WeaponStance
		{
			// Token: 0x04002800 RID: 10240
			None = -1,
			// Token: 0x04002801 RID: 10241
			Nailgun,
			// Token: 0x04002802 RID: 10242
			Spear,
			// Token: 0x04002803 RID: 10243
			Grenade,
			// Token: 0x04002804 RID: 10244
			Buzzsaw
		}
	}
}
