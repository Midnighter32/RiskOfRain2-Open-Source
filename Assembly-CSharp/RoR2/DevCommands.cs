using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200011F RID: 287
	public static class DevCommands
	{
		// Token: 0x06000531 RID: 1329 RVA: 0x00014F20 File Offset: 0x00013120
		private static void AddTokenIfDefault(List<string> lines, string token)
		{
			if (!string.IsNullOrEmpty(token) && Language.GetString(token) == token)
			{
				lines.Add(string.Format("\t\t\"{0}\": \"{0}\",", token));
			}
		}

		// Token: 0x06000532 RID: 1330 RVA: 0x00014F44 File Offset: 0x00013144
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

		// Token: 0x06000533 RID: 1331 RVA: 0x00015058 File Offset: 0x00013258
		[ConCommand(commandName = "rng_test_roll", flags = ConVarFlags.None, helpText = "Tests the RNG. First argument is a percent chance, second argument is a number of rolls to perform. Result is the average number of rolls that passed.")]
		private static void CCTestRng(ConCommandArgs args)
		{
			float argFloat = args.GetArgFloat(0);
			ulong argULong = args.GetArgULong(1);
			ulong num = 0UL;
			for (ulong num2 = 0UL; num2 < argULong; num2 += 1UL)
			{
				if (RoR2Application.rng.RangeFloat(0f, 100f) < argFloat)
				{
					num += 1UL;
				}
			}
			Debug.Log(num / argULong * 100.0);
		}
	}
}
