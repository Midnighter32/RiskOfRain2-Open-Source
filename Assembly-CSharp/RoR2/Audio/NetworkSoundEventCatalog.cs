using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Audio
{
	// Token: 0x02000685 RID: 1669
	public static class NetworkSoundEventCatalog
	{
		// Token: 0x06002710 RID: 10000 RVA: 0x000A9DFC File Offset: 0x000A7FFC
		[SystemInitializer(new Type[]
		{

		})]
		private static void Init()
		{
			List<NetworkSoundEventDef> list = new List<NetworkSoundEventDef>();
			list.AddRange(Resources.LoadAll<NetworkSoundEventDef>("NetworkSoundEventDefs/"));
			Action<List<NetworkSoundEventDef>> action = NetworkSoundEventCatalog.getSoundEventDefs;
			if (action != null)
			{
				action(list);
			}
			NetworkSoundEventCatalog.SetNetworkSoundEvents(list);
		}

		// Token: 0x06002711 RID: 10001 RVA: 0x000A9E38 File Offset: 0x000A8038
		public static void SetNetworkSoundEvents(List<NetworkSoundEventDef> newEntriesList)
		{
			NetworkSoundEventCatalog.eventNameToIndexTable.Clear();
			NetworkSoundEventCatalog.eventIdToIndexTable.Clear();
			NetworkSoundEventCatalog.entries = newEntriesList.ToArray();
			Array.Sort<NetworkSoundEventDef>(NetworkSoundEventCatalog.entries, (NetworkSoundEventDef a, NetworkSoundEventDef b) => StringComparer.OrdinalIgnoreCase.Compare(a.eventName, b.eventName));
			for (int i = 0; i < NetworkSoundEventCatalog.entries.Length; i++)
			{
				NetworkSoundEventDef networkSoundEventDef = NetworkSoundEventCatalog.entries[i];
				networkSoundEventDef.index = (NetworkSoundEventIndex)i;
				networkSoundEventDef.akId = AkSoundEngine.GetIDFromString(networkSoundEventDef.eventName);
				if (networkSoundEventDef.akId == 0U)
				{
					Debug.LogErrorFormat("Error during network sound registration: Wwise event \"{0}\" does not exist.", new object[]
					{
						networkSoundEventDef.eventName
					});
				}
			}
			for (int j = 0; j < NetworkSoundEventCatalog.entries.Length; j++)
			{
				NetworkSoundEventDef networkSoundEventDef2 = NetworkSoundEventCatalog.entries[j];
				NetworkSoundEventCatalog.eventNameToIndexTable[networkSoundEventDef2.eventName] = networkSoundEventDef2.index;
				NetworkSoundEventCatalog.eventIdToIndexTable[networkSoundEventDef2.akId] = networkSoundEventDef2.index;
			}
		}

		// Token: 0x1400008A RID: 138
		// (add) Token: 0x06002712 RID: 10002 RVA: 0x000A9F28 File Offset: 0x000A8128
		// (remove) Token: 0x06002713 RID: 10003 RVA: 0x000A9F5C File Offset: 0x000A815C
		public static event Action<List<NetworkSoundEventDef>> getSoundEventDefs;

		// Token: 0x06002714 RID: 10004 RVA: 0x000A9F90 File Offset: 0x000A8190
		public static NetworkSoundEventIndex FindNetworkSoundEventIndex(string eventName)
		{
			NetworkSoundEventIndex result;
			if (NetworkSoundEventCatalog.eventNameToIndexTable.TryGetValue(eventName, out result))
			{
				return result;
			}
			return NetworkSoundEventIndex.Invalid;
		}

		// Token: 0x06002715 RID: 10005 RVA: 0x000A9FB0 File Offset: 0x000A81B0
		public static NetworkSoundEventIndex FindNetworkSoundEventIndex(uint akEventId)
		{
			NetworkSoundEventIndex result;
			if (NetworkSoundEventCatalog.eventIdToIndexTable.TryGetValue(akEventId, out result))
			{
				return result;
			}
			return NetworkSoundEventIndex.Invalid;
		}

		// Token: 0x06002716 RID: 10006 RVA: 0x000A9FCF File Offset: 0x000A81CF
		public static uint GetAkIdFromNetworkSoundEventIndex(NetworkSoundEventIndex eventIndex)
		{
			if (eventIndex == NetworkSoundEventIndex.Invalid)
			{
				return 0U;
			}
			return NetworkSoundEventCatalog.entries[(int)eventIndex].akId;
		}

		// Token: 0x06002717 RID: 10007 RVA: 0x0000D703 File Offset: 0x0000B903
		public static void WriteNetworkSoundEventIndex(this NetworkWriter writer, NetworkSoundEventIndex networkSoundEventIndex)
		{
			writer.WritePackedUInt32((uint)(networkSoundEventIndex + 1));
		}

		// Token: 0x06002718 RID: 10008 RVA: 0x0000D70E File Offset: 0x0000B90E
		public static NetworkSoundEventIndex ReadNetworkSoundEventIndex(this NetworkReader reader)
		{
			return (NetworkSoundEventIndex)(reader.ReadPackedUInt32() - 1U);
		}

		// Token: 0x06002719 RID: 10009 RVA: 0x000A9FE4 File Offset: 0x000A81E4
		public static string GetEventNameFromId(uint akEventId)
		{
			NetworkSoundEventIndex networkSoundEventIndex;
			if (NetworkSoundEventCatalog.eventIdToIndexTable.TryGetValue(akEventId, out networkSoundEventIndex))
			{
				return NetworkSoundEventCatalog.entries[(int)networkSoundEventIndex].eventName;
			}
			return null;
		}

		// Token: 0x0600271A RID: 10010 RVA: 0x000AA00E File Offset: 0x000A820E
		public static string GetEventNameFromNetworkIndex(NetworkSoundEventIndex networkSoundEventIndex)
		{
			NetworkSoundEventDef safe = HGArrayUtilities.GetSafe<NetworkSoundEventDef>(NetworkSoundEventCatalog.entries, (int)networkSoundEventIndex);
			if (safe == null)
			{
				return null;
			}
			return safe.eventName;
		}

		// Token: 0x040024D1 RID: 9425
		private static NetworkSoundEventDef[] entries = Array.Empty<NetworkSoundEventDef>();

		// Token: 0x040024D2 RID: 9426
		private static readonly Dictionary<string, NetworkSoundEventIndex> eventNameToIndexTable = new Dictionary<string, NetworkSoundEventIndex>();

		// Token: 0x040024D3 RID: 9427
		private static readonly Dictionary<uint, NetworkSoundEventIndex> eventIdToIndexTable = new Dictionary<uint, NetworkSoundEventIndex>();
	}
}
