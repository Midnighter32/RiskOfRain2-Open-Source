using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000239 RID: 569
	public static class DevCommands
	{
		// Token: 0x06000AC7 RID: 2759 RVA: 0x000353A5 File Offset: 0x000335A5
		private static void AddTokenIfDefault(List<string> lines, string token)
		{
			if (!string.IsNullOrEmpty(token) && Language.GetString(token) == token)
			{
				lines.Add(string.Format("\t\t\"{0}\": \"{0}\",", token));
			}
		}

		// Token: 0x06000AC8 RID: 2760 RVA: 0x000353CC File Offset: 0x000335CC
		[ConCommand(commandName = "language_generate_tokens", flags = ConVarFlags.None, helpText = "Generates default token definitions to be inserted into a JSON language file.")]
		private static void CCLanguageGenerateTokens(ConCommandArgs args)
		{
			List<string> list = new List<string>();
			foreach (ItemDef itemDef in ItemCatalog.allItems.Select(new Func<ItemIndex, ItemDef>(ItemCatalog.GetItemDef)))
			{
				DevCommands.AddTokenIfDefault(list, itemDef.nameToken);
				DevCommands.AddTokenIfDefault(list, itemDef.pickupToken);
				DevCommands.AddTokenIfDefault(list, itemDef.descriptionToken);
			}
			list.Add("\r\n");
			foreach (EquipmentDef equipmentDef in EquipmentCatalog.allEquipment.Select(new Func<EquipmentIndex, EquipmentDef>(EquipmentCatalog.GetEquipmentDef)))
			{
				DevCommands.AddTokenIfDefault(list, equipmentDef.nameToken);
				DevCommands.AddTokenIfDefault(list, equipmentDef.pickupToken);
				DevCommands.AddTokenIfDefault(list, equipmentDef.descriptionToken);
			}
			Debug.Log(string.Join("\r\n", list));
		}
	}
}
