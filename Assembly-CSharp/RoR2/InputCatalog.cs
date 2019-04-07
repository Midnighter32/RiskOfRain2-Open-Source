using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Rewired;

namespace RoR2
{
	// Token: 0x02000441 RID: 1089
	public static class InputCatalog
	{
		// Token: 0x0600183C RID: 6204 RVA: 0x00073610 File Offset: 0x00071810
		static InputCatalog()
		{
			InputCatalog.<.cctor>g__Add|2_0("MoveHorizontal", "ACTION_MOVE_HORIZONTAL", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("MoveVertical", "ACTION_MOVE_VERTICAL", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("AimHorizontalMouse", "ACTION_AIM_HORIZONTAL_MOUSE", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("AimVerticalMouse", "ACTION_AIM_VERTICAL_MOUSE", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("AimHorizontalStick", "ACTION_AIM_HORIZONTAL_STICK", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("AimVerticalStick", "ACTION_AIM_VERTICAL_STICK", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("Jump", "ACTION_JUMP", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("Sprint", "ACTION_SPRINT", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("Interact", "ACTION_INTERACT", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("Equipment", "ACTION_EQUIPMENT", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("PrimarySkill", "ACTION_PRIMARY_SKILL", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("SecondarySkill", "ACTION_SECONDARY_SKILL", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("UtilitySkill", "ACTION_UTILITY_SKILL", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("SpecialSkill", "ACTION_SPECIAL_SKILL", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("Info", "ACTION_INFO", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("Ping", "ACTION_PING", AxisRange.Full);
			InputCatalog.<.cctor>g__Add|2_0("MoveHorizontal", "ACTION_MOVE_HORIZONTAL_POSITIVE", AxisRange.Positive);
			InputCatalog.<.cctor>g__Add|2_0("MoveHorizontal", "ACTION_MOVE_HORIZONTAL_NEGATIVE", AxisRange.Negative);
			InputCatalog.<.cctor>g__Add|2_0("MoveVertical", "ACTION_MOVE_VERTICAL_POSITIVE", AxisRange.Positive);
			InputCatalog.<.cctor>g__Add|2_0("MoveVertical", "ACTION_MOVE_VERTICAL_NEGATIVE", AxisRange.Negative);
		}

		// Token: 0x0600183D RID: 6205 RVA: 0x00073768 File Offset: 0x00071968
		public static string GetActionNameToken(string actionName, AxisRange axisRange = AxisRange.Full)
		{
			string result;
			if (InputCatalog.actionToToken.TryGetValue(new InputCatalog.ActionAxisPair(actionName, axisRange), out result))
			{
				return result;
			}
			throw new ArgumentException(string.Format("Bad action/axis pair {0} {1}.", actionName, axisRange));
		}

		// Token: 0x04001B86 RID: 7046
		private static readonly Dictionary<InputCatalog.ActionAxisPair, string> actionToToken = new Dictionary<InputCatalog.ActionAxisPair, string>();

		// Token: 0x02000442 RID: 1090
		private struct ActionAxisPair : IEquatable<InputCatalog.ActionAxisPair>
		{
			// Token: 0x0600183F RID: 6207 RVA: 0x000737B6 File Offset: 0x000719B6
			public ActionAxisPair([NotNull] string actionName, AxisRange axisRange)
			{
				this.actionName = actionName;
				this.axisRange = axisRange;
			}

			// Token: 0x06001840 RID: 6208 RVA: 0x000737C6 File Offset: 0x000719C6
			public bool Equals(InputCatalog.ActionAxisPair other)
			{
				return string.Equals(this.actionName, other.actionName) && this.axisRange == other.axisRange;
			}

			// Token: 0x06001841 RID: 6209 RVA: 0x000737EB File Offset: 0x000719EB
			public override bool Equals(object obj)
			{
				return obj != null && obj is InputCatalog.ActionAxisPair && this.Equals((InputCatalog.ActionAxisPair)obj);
			}

			// Token: 0x06001842 RID: 6210 RVA: 0x00073808 File Offset: 0x00071A08
			public override int GetHashCode()
			{
				return ((-1879861323 * -1521134295 + base.GetHashCode()) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.actionName)) * -1521134295 + this.axisRange.GetHashCode();
			}

			// Token: 0x04001B87 RID: 7047
			[NotNull]
			private readonly string actionName;

			// Token: 0x04001B88 RID: 7048
			private readonly AxisRange axisRange;
		}
	}
}
