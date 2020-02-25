using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000125 RID: 293
	public static class EffectCatalog
	{
		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x0600053B RID: 1339 RVA: 0x000151E9 File Offset: 0x000133E9
		public static int effectCount
		{
			get
			{
				return EffectCatalog.entries.Length;
			}
		}

		// Token: 0x0600053C RID: 1340 RVA: 0x000151F2 File Offset: 0x000133F2
		[SystemInitializer(new Type[]
		{

		})]
		public static void Init()
		{
			EffectCatalog.SetEntries(EffectCatalog.GetDefaultEffectDefs());
		}

		// Token: 0x0600053D RID: 1341 RVA: 0x00015200 File Offset: 0x00013400
		private static EffectDef[] GetDefaultEffectDefs()
		{
			GameObject[] array = Resources.LoadAll<GameObject>("Prefabs/Effects/");
			List<EffectDef> list = new List<EffectDef>(array.Length);
			foreach (GameObject gameObject in array)
			{
				EffectComponent component = gameObject.GetComponent<EffectComponent>();
				if (!component)
				{
					Debug.LogErrorFormat(gameObject, "Error registering effect \"{0}\": Prefab does not have EffectComponent attached.", new object[]
					{
						gameObject
					});
				}
				else
				{
					EffectDef item = new EffectDef
					{
						prefab = gameObject,
						prefabEffectComponent = component,
						prefabVfxAttributes = gameObject.GetComponent<VFXAttributes>(),
						prefabName = gameObject.name,
						spawnSoundEventName = component.soundName
					};
					list.Add(item);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j].prefabName.Equals("CoinEmitter"))
				{
					list[j].cullMethod = ((EffectData effectData) => SettingsConVars.cvExpAndMoneyEffects.value);
					break;
				}
			}
			return list.ToArray();
		}

		// Token: 0x0600053E RID: 1342 RVA: 0x00015300 File Offset: 0x00013500
		public static void SetEntries(EffectDef[] newEntries)
		{
			Array.Resize<EffectDef>(ref EffectCatalog.entries, newEntries.Length);
			Array.Copy(newEntries, EffectCatalog.entries, newEntries.Length);
			for (int i = 0; i < EffectCatalog.entries.Length; i++)
			{
				ref EffectDef ptr = ref EffectCatalog.entries[i];
				ptr.index = (EffectIndex)i;
				ptr.prefabEffectComponent.effectIndex = ptr.index;
			}
		}

		// Token: 0x0600053F RID: 1343 RVA: 0x00015364 File Offset: 0x00013564
		public static EffectIndex FindEffectIndexFromPrefab(GameObject effectPrefab)
		{
			if (effectPrefab)
			{
				EffectComponent component = effectPrefab.GetComponent<EffectComponent>();
				if (component)
				{
					return component.effectIndex;
				}
			}
			return EffectIndex.Invalid;
		}

		// Token: 0x06000540 RID: 1344 RVA: 0x00015390 File Offset: 0x00013590
		public static EffectDef GetEffectDef(EffectIndex effectIndex)
		{
			if ((ulong)effectIndex < (ulong)((long)EffectCatalog.entries.Length))
			{
				return EffectCatalog.entries[(int)effectIndex];
			}
			return null;
		}

		// Token: 0x06000541 RID: 1345 RVA: 0x000151F2 File Offset: 0x000133F2
		[ConCommand(commandName = "effects_reload", flags = ConVarFlags.Cheat, helpText = "Reloads the effect catalog.")]
		private static void CCEffectsReload(ConCommandArgs args)
		{
			EffectCatalog.SetEntries(EffectCatalog.GetDefaultEffectDefs());
		}

		// Token: 0x04000578 RID: 1400
		private static EffectDef[] entries = Array.Empty<EffectDef>();
	}
}
