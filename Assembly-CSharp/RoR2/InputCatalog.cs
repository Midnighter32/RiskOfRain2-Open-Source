using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Rewired;

namespace RoR2
{
	// Token: 0x020003A4 RID: 932
	public static class InputCatalog
	{
		// Token: 0x0600169E RID: 5790 RVA: 0x000610CC File Offset: 0x0005F2CC
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

		// Token: 0x0600169F RID: 5791 RVA: 0x00061224 File Offset: 0x0005F424
		public static string GetActionNameToken(string actionName, AxisRange axisRange = AxisRange.Full)
		{
			string result;
			if (InputCatalog.actionToToken.TryGetValue(new InputCatalog.ActionAxisPair(actionName, axisRange), out result))
			{
				return result;
			}
			throw new ArgumentException(string.Format("Bad action/axis pair {0} {1}.", actionName, axisRange));
		}

		// Token: 0x060016A0 RID: 5792 RVA: 0x0006125E File Offset: 0x0005F45E
		[CompilerGenerated]
		internal static void <.cctor>g__Add|2_0(string actionName, string token, AxisRange axisRange)
		{
			InputCatalog.actionToToken[new InputCatalog.ActionAxisPair(actionName, axisRange)] = token;
		}

		// Token: 0x04001533 RID: 5427
		private static readonly Dictionary<InputCatalog.ActionAxisPair, string> actionToToken = new Dictionary<InputCatalog.ActionAxisPair, string>();

		// Token: 0x020003A5 RID: 933
		private struct ActionAxisPair : IEquatable<InputCatalog.ActionAxisPair>
		{
			// Token: 0x060016A1 RID: 5793 RVA: 0x00061272 File Offset: 0x0005F472
			public ActionAxisPair([NotNull] string actionName, AxisRange axisRange)
			{
				this.actionName = actionName;
				this.axisRange = axisRange;
			}

			// Token: 0x060016A2 RID: 5794 RVA: 0x00061282 File Offset: 0x0005F482
			public bool Equals(InputCatalog.ActionAxisPair other)
			{
				return string.Equals(this.actionName, other.actionName) && this.axisRange == other.axisRange;
			}

			// Token: 0x060016A3 RID: 5795 RVA: 0x000612A7 File Offset: 0x0005F4A7
			public override bool Equals(object obj)
			{
				return obj != null && obj is InputCatalog.ActionAxisPair && this.Equals((InputCatalog.ActionAxisPair)obj);
			}

			// Token: 0x060016A4 RID: 5796 RVA: 0x000612C4 File Offset: 0x0005F4C4
			public override int GetHashCode()
			{
				return ((-1879861323 * -1521134295 + base.GetHashCode()) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.actionName)) * -1521134295 + this.axisRange.GetHashCode();
			}

			// Token: 0x04001534 RID: 5428
			[NotNull]
			private readonly string actionName;

			// Token: 0x04001535 RID: 5429
			private readonly AxisRange axisRange;
		}
	}
}
