using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020000EE RID: 238
	public class CostTypeDef
	{
		// Token: 0x17000093 RID: 147
		// (get) Token: 0x0600049C RID: 1180 RVA: 0x000129B1 File Offset: 0x00010BB1
		// (set) Token: 0x0600049B RID: 1179 RVA: 0x000129A8 File Offset: 0x00010BA8
		public CostTypeDef.BuildCostStringDelegate buildCostString { private get; set; } = new CostTypeDef.BuildCostStringDelegate(CostTypeDef.BuildCostStringDefault);

		// Token: 0x0600049D RID: 1181 RVA: 0x000129BC File Offset: 0x00010BBC
		public void BuildCostString(int cost, [NotNull] StringBuilder stringBuilder)
		{
			this.buildCostString(this, new CostTypeDef.BuildCostStringContext
			{
				cost = cost,
				stringBuilder = stringBuilder
			});
		}

		// Token: 0x0600049E RID: 1182 RVA: 0x000129EE File Offset: 0x00010BEE
		public static void BuildCostStringDefault(CostTypeDef costTypeDef, CostTypeDef.BuildCostStringContext context)
		{
			context.stringBuilder.Append(Language.GetStringFormatted(costTypeDef.costStringFormatToken, new object[]
			{
				context.cost
			}));
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060004A0 RID: 1184 RVA: 0x00012A24 File Offset: 0x00010C24
		// (set) Token: 0x0600049F RID: 1183 RVA: 0x00012A1B File Offset: 0x00010C1B
		public CostTypeDef.GetCostColorDelegate getCostColor { private get; set; } = new CostTypeDef.GetCostColorDelegate(CostTypeDef.GetCostColorDefault);

		// Token: 0x060004A1 RID: 1185 RVA: 0x00012A2C File Offset: 0x00010C2C
		public Color32 GetCostColor(bool forWorldDisplay)
		{
			return this.getCostColor(this, new CostTypeDef.GetCostColorContext
			{
				forWorldDisplay = forWorldDisplay
			});
		}

		// Token: 0x060004A2 RID: 1186 RVA: 0x00012A58 File Offset: 0x00010C58
		public static Color32 GetCostColorDefault(CostTypeDef costTypeDef, CostTypeDef.GetCostColorContext context)
		{
			Color32 color = ColorCatalog.GetColor(costTypeDef.colorIndex);
			if (context.forWorldDisplay)
			{
				float h;
				float num;
				float num2;
				Color.RGBToHSV(color, out h, out num, out num2);
				if (costTypeDef.saturateWorldStyledCostString && num > 0f)
				{
					num = 1f;
				}
				if (costTypeDef.darkenWorldStyledCostString)
				{
					num2 *= 0.5f;
				}
				color = Color.HSVToRGB(h, num, num2);
			}
			return color;
		}

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x060004A4 RID: 1188 RVA: 0x00012AC8 File Offset: 0x00010CC8
		// (set) Token: 0x060004A3 RID: 1187 RVA: 0x00012ABF File Offset: 0x00010CBF
		public CostTypeDef.BuildCostStringStyledDelegate buildCostStringStyled { private get; set; } = new CostTypeDef.BuildCostStringStyledDelegate(CostTypeDef.BuildCostStringStyledDefault);

		// Token: 0x060004A5 RID: 1189 RVA: 0x00012AD0 File Offset: 0x00010CD0
		public void BuildCostStringStyled(int cost, [NotNull] StringBuilder stringBuilder, bool forWorldDisplay, bool includeColor = true)
		{
			this.buildCostStringStyled(this, new CostTypeDef.BuildCostStringStyledContext
			{
				cost = cost,
				forWorldDisplay = forWorldDisplay,
				stringBuilder = stringBuilder,
				includeColor = includeColor
			});
		}

		// Token: 0x060004A6 RID: 1190 RVA: 0x00012B14 File Offset: 0x00010D14
		public static void BuildCostStringStyledDefault(CostTypeDef costTypeDef, CostTypeDef.BuildCostStringStyledContext context)
		{
			StringBuilder stringBuilder = context.stringBuilder;
			stringBuilder.Append("<nobr>");
			if (costTypeDef.costStringStyle != null)
			{
				stringBuilder.Append("<style=");
				stringBuilder.Append(costTypeDef.costStringStyle);
				stringBuilder.Append(">");
			}
			if (context.includeColor)
			{
				Color32 costColor = costTypeDef.GetCostColor(context.forWorldDisplay);
				stringBuilder.Append("<color=#");
				stringBuilder.AppendColor32RGBHexValues(costColor);
				stringBuilder.Append(">");
			}
			costTypeDef.BuildCostString(context.cost, context.stringBuilder);
			if (context.includeColor)
			{
				stringBuilder.Append("</color>");
			}
			if (costTypeDef.costStringStyle != null)
			{
				stringBuilder.Append("</style>");
			}
			stringBuilder.Append("</nobr>");
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x060004A8 RID: 1192 RVA: 0x00012BE5 File Offset: 0x00010DE5
		// (set) Token: 0x060004A7 RID: 1191 RVA: 0x00012BDC File Offset: 0x00010DDC
		public CostTypeDef.IsAffordableDelegate isAffordable { private get; set; }

		// Token: 0x060004A9 RID: 1193 RVA: 0x00012BF0 File Offset: 0x00010DF0
		public bool IsAffordable(int cost, Interactor activator)
		{
			return this.isAffordable(this, new CostTypeDef.IsAffordableContext
			{
				cost = cost,
				activator = activator
			});
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x060004AB RID: 1195 RVA: 0x00012C2B File Offset: 0x00010E2B
		// (set) Token: 0x060004AA RID: 1194 RVA: 0x00012C22 File Offset: 0x00010E22
		public CostTypeDef.PayCostDelegate payCost { private get; set; }

		// Token: 0x060004AC RID: 1196 RVA: 0x00012C34 File Offset: 0x00010E34
		public CostTypeDef.PayCostResults PayCost(int cost, Interactor activator, GameObject purchasedObject, Xoroshiro128Plus rng, ItemIndex avoidedItemIndex)
		{
			CostTypeDef.PayCostResults payCostResults = new CostTypeDef.PayCostResults();
			CharacterBody component = activator.GetComponent<CharacterBody>();
			this.payCost(this, new CostTypeDef.PayCostContext
			{
				cost = cost,
				activator = activator,
				activatorBody = component,
				activatorMaster = (component ? component.master : null),
				purchasedObject = purchasedObject,
				results = payCostResults,
				rng = rng,
				avoidedItemIndex = avoidedItemIndex
			});
			return payCostResults;
		}

		// Token: 0x04000473 RID: 1139
		public string name;

		// Token: 0x04000474 RID: 1140
		public ItemTier itemTier = ItemTier.NoTier;

		// Token: 0x04000475 RID: 1141
		public ColorCatalog.ColorIndex colorIndex = ColorCatalog.ColorIndex.Error;

		// Token: 0x04000476 RID: 1142
		public string costStringFormatToken;

		// Token: 0x04000477 RID: 1143
		public string costStringStyle;

		// Token: 0x04000478 RID: 1144
		public bool saturateWorldStyledCostString = true;

		// Token: 0x04000479 RID: 1145
		public bool darkenWorldStyledCostString = true;

		// Token: 0x020000EF RID: 239
		// (Invoke) Token: 0x060004AF RID: 1199
		public delegate void BuildCostStringDelegate(CostTypeDef costTypeDef, CostTypeDef.BuildCostStringContext context);

		// Token: 0x020000F0 RID: 240
		public struct BuildCostStringContext
		{
			// Token: 0x0400047F RID: 1151
			public StringBuilder stringBuilder;

			// Token: 0x04000480 RID: 1152
			public int cost;
		}

		// Token: 0x020000F1 RID: 241
		// (Invoke) Token: 0x060004B3 RID: 1203
		public delegate Color32 GetCostColorDelegate(CostTypeDef costTypeDef, CostTypeDef.GetCostColorContext context);

		// Token: 0x020000F2 RID: 242
		public struct GetCostColorContext
		{
			// Token: 0x04000481 RID: 1153
			public bool forWorldDisplay;
		}

		// Token: 0x020000F3 RID: 243
		// (Invoke) Token: 0x060004B7 RID: 1207
		public delegate void BuildCostStringStyledDelegate(CostTypeDef costTypeDef, CostTypeDef.BuildCostStringStyledContext context);

		// Token: 0x020000F4 RID: 244
		public struct BuildCostStringStyledContext
		{
			// Token: 0x04000482 RID: 1154
			public StringBuilder stringBuilder;

			// Token: 0x04000483 RID: 1155
			public int cost;

			// Token: 0x04000484 RID: 1156
			public bool forWorldDisplay;

			// Token: 0x04000485 RID: 1157
			public bool includeColor;
		}

		// Token: 0x020000F5 RID: 245
		// (Invoke) Token: 0x060004BB RID: 1211
		public delegate bool IsAffordableDelegate(CostTypeDef costTypeDef, CostTypeDef.IsAffordableContext context);

		// Token: 0x020000F6 RID: 246
		public struct IsAffordableContext
		{
			// Token: 0x04000486 RID: 1158
			public int cost;

			// Token: 0x04000487 RID: 1159
			public Interactor activator;
		}

		// Token: 0x020000F7 RID: 247
		// (Invoke) Token: 0x060004BF RID: 1215
		public delegate void PayCostDelegate(CostTypeDef costTypeDef, CostTypeDef.PayCostContext context);

		// Token: 0x020000F8 RID: 248
		public struct PayCostContext
		{
			// Token: 0x04000488 RID: 1160
			public int cost;

			// Token: 0x04000489 RID: 1161
			public Interactor activator;

			// Token: 0x0400048A RID: 1162
			public CharacterBody activatorBody;

			// Token: 0x0400048B RID: 1163
			public CharacterMaster activatorMaster;

			// Token: 0x0400048C RID: 1164
			public GameObject purchasedObject;

			// Token: 0x0400048D RID: 1165
			public CostTypeDef.PayCostResults results;

			// Token: 0x0400048E RID: 1166
			public Xoroshiro128Plus rng;

			// Token: 0x0400048F RID: 1167
			public ItemIndex avoidedItemIndex;
		}

		// Token: 0x020000F9 RID: 249
		public class PayCostResults
		{
			// Token: 0x04000490 RID: 1168
			public List<ItemIndex> itemsTaken = new List<ItemIndex>();

			// Token: 0x04000491 RID: 1169
			public List<EquipmentIndex> equipmentTaken = new List<EquipmentIndex>();
		}
	}
}
