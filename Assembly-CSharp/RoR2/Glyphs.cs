﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Rewired;
using RoR2.UI;

namespace RoR2
{
	// Token: 0x02000393 RID: 915
	public static class Glyphs
	{
		// Token: 0x0600163C RID: 5692 RVA: 0x0005F930 File Offset: 0x0005DB30
		private static void AddGlyph(string controllerName, int elementIndex, string assetName, string glyphName)
		{
			Glyphs.glyphMap[new Glyphs.GlyphKey(controllerName, elementIndex)] = string.Format(CultureInfo.InvariantCulture, "<sprite=\"{0}\" name=\"{1}\">", assetName, glyphName);
		}

		// Token: 0x0600163D RID: 5693 RVA: 0x0005F954 File Offset: 0x0005DB54
		private static void RegisterXBoxController(string controllerName)
		{
			Glyphs.AddGlyph(controllerName, 4, "tmpsprXboxOneGlyphs", "texXBoxOneGlyphs_5");
			Glyphs.AddGlyph(controllerName, 5, "tmpsprXboxOneGlyphs", "texXBoxOneGlyphs_9");
			Glyphs.AddGlyph(controllerName, 10, "tmpsprXboxOneGlyphs", "texXBoxOneGlyphs_2");
			Glyphs.AddGlyph(controllerName, 11, "tmpsprXboxOneGlyphs", "texXBoxOneGlyphs_6");
			Glyphs.AddGlyph(controllerName, 6, "tmpsprXboxOneGlyphs", "texXBoxOneGlyphs_0");
			Glyphs.AddGlyph(controllerName, 7, "tmpsprXboxOneGlyphs", "texXBoxOneGlyphs_1");
			Glyphs.AddGlyph(controllerName, 8, "tmpsprXboxOneGlyphs", "texXBoxOneGlyphs_7");
			Glyphs.AddGlyph(controllerName, 9, "tmpsprXboxOneGlyphs", "texXBoxOneGlyphs_11");
			Glyphs.AddGlyph(controllerName, 12, "tmpsprXboxOneGlyphs", "texXBoxOneGlyphs_10");
			Glyphs.AddGlyph(controllerName, 13, "tmpsprXboxOneGlyphs", "texXBoxOneGlyphs_3");
			Glyphs.AddGlyph(controllerName, 14, "tmpsprXboxOneGlyphs", "texXBoxOneGlyphs_4");
			Glyphs.AddGlyph(controllerName, 15, "tmpsprXboxOneGlyphs", "texXBoxOneGlyphs_8");
		}

		// Token: 0x0600163E RID: 5694 RVA: 0x0005FA34 File Offset: 0x0005DC34
		private static void RegisterDS4Controller(string controllerName)
		{
			Glyphs.AddGlyph(controllerName, 4, "tmpsprSteamGlyphs", "texSteamGlyphs_110");
			Glyphs.AddGlyph(controllerName, 5, "tmpsprSteamGlyphs", "texSteamGlyphs_112");
			Glyphs.AddGlyph(controllerName, 10, "tmpsprSteamGlyphs", "texSteamGlyphs_84");
			Glyphs.AddGlyph(controllerName, 11, "tmpsprSteamGlyphs", "texSteamGlyphs_85");
			Glyphs.AddGlyph(controllerName, 6, "tmpsprSteamGlyphs", "texSteamGlyphs_49");
			Glyphs.AddGlyph(controllerName, 7, "tmpsprSteamGlyphs", "texSteamGlyphs_39");
			Glyphs.AddGlyph(controllerName, 8, "tmpsprSteamGlyphs", "texSteamGlyphs_46");
			Glyphs.AddGlyph(controllerName, 9, "tmpsprSteamGlyphs", "texSteamGlyphs_47");
		}

		// Token: 0x0600163F RID: 5695 RVA: 0x0005FACC File Offset: 0x0005DCCC
		private static void RegisterMouse(string controllerName)
		{
			Glyphs.AddGlyph(controllerName, 3, "tmpsprSteamGlyphs", "texSteamGlyphs_17");
			Glyphs.AddGlyph(controllerName, 4, "tmpsprSteamGlyphs", "texSteamGlyphs_18");
			Glyphs.AddGlyph(controllerName, 5, "tmpsprSteamGlyphs", "texSteamGlyphs_19");
		}

		// Token: 0x06001640 RID: 5696 RVA: 0x0000409B File Offset: 0x0000229B
		private static void RegisterKeyboard(string controllerName)
		{
		}

		// Token: 0x06001641 RID: 5697 RVA: 0x0005FB04 File Offset: 0x0005DD04
		static Glyphs()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["Left Mouse Button"] = "M1";
			dictionary["Right Mouse Button"] = "M2";
			dictionary["Mouse Button 3"] = "M3";
			dictionary["Mouse Button 4"] = "M4";
			dictionary["Mouse Button 5"] = "M5";
			dictionary["Mouse Button 6"] = "M6";
			dictionary["Mouse Button 7"] = "M7";
			dictionary["Mouse Wheel"] = "MW";
			dictionary["Mouse Wheel +"] = "MW+";
			dictionary["Mouse Wheel -"] = "MW-";
			Glyphs.mouseElementRenameMap = dictionary;
			Glyphs.resultsList = new List<ActionElementMap>();
			Glyphs.RegisterXBoxController("Xbox 360 Controller");
			Glyphs.RegisterXBoxController("Xbox One Controller");
			for (int i = 0; i < 4; i++)
			{
				Glyphs.RegisterXBoxController("XInput Gamepad " + i);
			}
			Glyphs.RegisterDS4Controller("Sony DualShock 4");
		}

		// Token: 0x06001642 RID: 5698 RVA: 0x0005FC18 File Offset: 0x0005DE18
		private static string GetKeyboardGlyphString(string actionName)
		{
			string text;
			if (!Glyphs.keyboardRawNameToGlyphName.TryGetValue(actionName, out text))
			{
				if (!(actionName == "Left Shift"))
				{
					if (actionName == "Left Control")
					{
						actionName = "Ctrl";
					}
				}
				else
				{
					actionName = "Shift";
				}
				text = actionName;
				Glyphs.keyboardRawNameToGlyphName[actionName] = text;
			}
			return text;
		}

		// Token: 0x06001643 RID: 5699 RVA: 0x0005FC70 File Offset: 0x0005DE70
		public static string GetGlyphString(MPEventSystemLocator eventSystemLocator, string actionName)
		{
			MPEventSystem eventSystem = eventSystemLocator.eventSystem;
			if (eventSystem)
			{
				return Glyphs.GetGlyphString(eventSystem, actionName, AxisRange.Full);
			}
			return "UNKNOWN";
		}

		// Token: 0x06001644 RID: 5700 RVA: 0x0005FC9A File Offset: 0x0005DE9A
		public static string GetGlyphString(MPEventSystem eventSystem, string actionName, AxisRange axisRange = AxisRange.Full)
		{
			return Glyphs.GetGlyphString(eventSystem, actionName, axisRange, eventSystem.currentInputSource);
		}

		// Token: 0x06001645 RID: 5701 RVA: 0x0005FCAC File Offset: 0x0005DEAC
		public static string GetGlyphString(MPEventSystem eventSystem, string actionName, AxisRange axisRange, MPEventSystem.InputSource currentInputSource)
		{
			Glyphs.<>c__DisplayClass18_0 CS$<>8__locals1;
			CS$<>8__locals1.inputPlayer = eventSystem.player;
			InputAction action = ReInput.mapping.GetAction(actionName);
			CS$<>8__locals1.inputActionId = action.id;
			CS$<>8__locals1.controllerName = "Xbox One Controller";
			CS$<>8__locals1.controllerType = (ControllerType)(-1);
			CS$<>8__locals1.axisContributionMatters = (axisRange > AxisRange.Full);
			CS$<>8__locals1.axisContribution = Pole.Positive;
			if (axisRange == AxisRange.Negative)
			{
				CS$<>8__locals1.axisContribution = Pole.Negative;
			}
			if (currentInputSource != MPEventSystem.InputSource.Keyboard)
			{
				if (currentInputSource != MPEventSystem.InputSource.Gamepad)
				{
					throw new ArgumentOutOfRangeException();
				}
				CS$<>8__locals1.controllerType = ControllerType.Joystick;
				Glyphs.<GetGlyphString>g__SetController|18_0(CS$<>8__locals1.inputPlayer.controllers.GetLastActiveController(ControllerType.Joystick), ref CS$<>8__locals1);
				if (CS$<>8__locals1.actionElementMap == null)
				{
					foreach (Controller controller in CS$<>8__locals1.inputPlayer.controllers.Controllers)
					{
						if (controller.type == ControllerType.Joystick)
						{
							Glyphs.<GetGlyphString>g__SetController|18_0(controller, ref CS$<>8__locals1);
							if (CS$<>8__locals1.actionElementMap != null)
							{
								break;
							}
						}
					}
				}
				if (CS$<>8__locals1.actionElementMap == null && eventSystem.localUser != null)
				{
					using (IEnumerator<ActionElementMap> enumerator2 = eventSystem.localUser.userProfile.joystickMap.ElementMapsWithAction(CS$<>8__locals1.inputActionId).GetEnumerator())
					{
						if (enumerator2.MoveNext())
						{
							ActionElementMap actionElementMap = enumerator2.Current;
							CS$<>8__locals1.actionElementMap = actionElementMap;
						}
					}
				}
				if (CS$<>8__locals1.actionElementMap == null)
				{
					return "[NO GAMEPAD BINDING]";
				}
			}
			else
			{
				Glyphs.<GetGlyphString>g__SetController|18_0(CS$<>8__locals1.inputPlayer.controllers.Keyboard, ref CS$<>8__locals1);
				if (CS$<>8__locals1.actionElementMap == null)
				{
					Glyphs.<GetGlyphString>g__SetController|18_0(CS$<>8__locals1.inputPlayer.controllers.Mouse, ref CS$<>8__locals1);
				}
				if (CS$<>8__locals1.actionElementMap == null)
				{
					return "[NO KB/M BINDING]";
				}
			}
			int elementIdentifierId = CS$<>8__locals1.actionElementMap.elementIdentifierId;
			Glyphs.GlyphKey key = new Glyphs.GlyphKey(CS$<>8__locals1.controllerName, elementIdentifierId);
			string result;
			if (Glyphs.glyphMap.TryGetValue(key, out result))
			{
				return result;
			}
			if (CS$<>8__locals1.controllerType == ControllerType.Keyboard)
			{
				return Glyphs.GetKeyboardGlyphString(CS$<>8__locals1.actionElementMap.elementIdentifierName);
			}
			if (CS$<>8__locals1.controllerType == ControllerType.Mouse)
			{
				string text = CS$<>8__locals1.actionElementMap.elementIdentifierName;
				string text2;
				if (Glyphs.mouseElementRenameMap.TryGetValue(text, out text2))
				{
					text = text2;
				}
				return text;
			}
			return "UNKNOWN";
		}

		// Token: 0x06001646 RID: 5702 RVA: 0x0005FEEC File Offset: 0x0005E0EC
		[CompilerGenerated]
		internal static void <GetGlyphString>g__SetController|18_0(Controller newController, ref Glyphs.<>c__DisplayClass18_0 A_1)
		{
			if (newController != null)
			{
				A_1.controllerName = newController.name;
				A_1.controllerType = newController.type;
			}
			A_1.actionElementMap = null;
			if (newController != null)
			{
				Glyphs.resultsList.Clear();
				A_1.inputPlayer.controllers.maps.GetElementMapsWithAction(newController.type, newController.id, A_1.inputActionId, false, Glyphs.resultsList);
				foreach (ActionElementMap actionElementMap in Glyphs.resultsList)
				{
					if (!A_1.axisContributionMatters || actionElementMap.axisContribution == A_1.axisContribution)
					{
						A_1.actionElementMap = actionElementMap;
						break;
					}
				}
			}
		}

		// Token: 0x040014E8 RID: 5352
		private static readonly Dictionary<Glyphs.GlyphKey, string> glyphMap = new Dictionary<Glyphs.GlyphKey, string>();

		// Token: 0x040014E9 RID: 5353
		private const string xbox360ControllerName = "Xbox 360 Controller";

		// Token: 0x040014EA RID: 5354
		private const string xboxOneControllerName = "Xbox One Controller";

		// Token: 0x040014EB RID: 5355
		private const string dualshock4ControllerName = "Sony DualShock 4";

		// Token: 0x040014EC RID: 5356
		private const string defaultControllerName = "Xbox One Controller";

		// Token: 0x040014ED RID: 5357
		private static readonly Dictionary<string, string> keyboardRawNameToGlyphName = new Dictionary<string, string>();

		// Token: 0x040014EE RID: 5358
		private static readonly Dictionary<string, string> mouseElementRenameMap;

		// Token: 0x040014EF RID: 5359
		private static readonly List<ActionElementMap> resultsList;

		// Token: 0x02000394 RID: 916
		private struct GlyphKey : IEquatable<Glyphs.GlyphKey>
		{
			// Token: 0x06001647 RID: 5703 RVA: 0x0005FFB8 File Offset: 0x0005E1B8
			public GlyphKey(string deviceName, int elementId)
			{
				this.deviceName = deviceName;
				this.elementId = elementId;
			}

			// Token: 0x06001648 RID: 5704 RVA: 0x0005FFC8 File Offset: 0x0005E1C8
			public bool Equals(Glyphs.GlyphKey other)
			{
				return string.Equals(this.deviceName, other.deviceName) && this.elementId == other.elementId;
			}

			// Token: 0x06001649 RID: 5705 RVA: 0x0005FFED File Offset: 0x0005E1ED
			public override bool Equals(object obj)
			{
				return obj != null && obj is Glyphs.GlyphKey && this.Equals((Glyphs.GlyphKey)obj);
			}

			// Token: 0x0600164A RID: 5706 RVA: 0x0006000A File Offset: 0x0005E20A
			public override int GetHashCode()
			{
				return ((this.deviceName != null) ? this.deviceName.GetHashCode() : 0) * 397 ^ this.elementId;
			}

			// Token: 0x040014F0 RID: 5360
			public readonly string deviceName;

			// Token: 0x040014F1 RID: 5361
			public readonly int elementId;
		}
	}
}
