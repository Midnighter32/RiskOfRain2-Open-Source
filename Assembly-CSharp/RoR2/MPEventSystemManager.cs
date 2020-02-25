using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Rewired;
using Rewired.Integration.UnityUI;
using RoR2.UI;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000361 RID: 865
	public class MPEventSystemManager : MonoBehaviour
	{
		// Token: 0x17000280 RID: 640
		// (get) Token: 0x060014FC RID: 5372 RVA: 0x000599A1 File Offset: 0x00057BA1
		// (set) Token: 0x060014FD RID: 5373 RVA: 0x000599A8 File Offset: 0x00057BA8
		public static MPEventSystem combinedEventSystem { get; private set; }

		// Token: 0x17000281 RID: 641
		// (get) Token: 0x060014FE RID: 5374 RVA: 0x000599B0 File Offset: 0x00057BB0
		// (set) Token: 0x060014FF RID: 5375 RVA: 0x000599B7 File Offset: 0x00057BB7
		public static MPEventSystem kbmEventSystem { get; private set; }

		// Token: 0x17000282 RID: 642
		// (get) Token: 0x06001500 RID: 5376 RVA: 0x000599BF File Offset: 0x00057BBF
		// (set) Token: 0x06001501 RID: 5377 RVA: 0x000599C6 File Offset: 0x00057BC6
		public static MPEventSystem primaryEventSystem { get; private set; }

		// Token: 0x06001502 RID: 5378 RVA: 0x000599D0 File Offset: 0x00057BD0
		public static MPEventSystem FindEventSystem(Player inputPlayer)
		{
			MPEventSystem result;
			MPEventSystemManager.eventSystems.TryGetValue(inputPlayer.id, out result);
			return result;
		}

		// Token: 0x06001503 RID: 5379 RVA: 0x000599F4 File Offset: 0x00057BF4
		private static void Initialize()
		{
			GameObject original = Resources.Load<GameObject>("Prefabs/UI/MPEventSystem");
			IList<Player> players = ReInput.players.Players;
			for (int i = 0; i < players.Count; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(original, RoR2Application.instance.transform);
				gameObject.name = string.Format(CultureInfo.InvariantCulture, "MPEventSystem Player{0}", i);
				MPEventSystem component = gameObject.GetComponent<MPEventSystem>();
				RewiredStandaloneInputModule component2 = gameObject.GetComponent<RewiredStandaloneInputModule>();
				Player player = players[i];
				component2.RewiredPlayerIds = new int[]
				{
					player.id
				};
				gameObject.GetComponent<MPInput>().player = player;
				if (i == 1)
				{
					MPEventSystemManager.kbmEventSystem = component;
					component.allowCursorPush = false;
				}
				component.player = players[i];
				MPEventSystemManager.eventSystems[players[i].id] = component;
			}
			MPEventSystemManager.combinedEventSystem = MPEventSystemManager.eventSystems[0];
			MPEventSystemManager.combinedEventSystem.isCombinedEventSystem = true;
			MPEventSystemManager.RefreshEventSystems();
		}

		// Token: 0x06001504 RID: 5380 RVA: 0x00059AEC File Offset: 0x00057CEC
		private static void RefreshEventSystems()
		{
			int count = LocalUserManager.readOnlyLocalUsersList.Count;
			ReadOnlyCollection<MPEventSystem> readOnlyInstancesList = MPEventSystem.readOnlyInstancesList;
			readOnlyInstancesList[0].enabled = (count <= 1);
			for (int i = 1; i < readOnlyInstancesList.Count; i++)
			{
				readOnlyInstancesList[i].enabled = (readOnlyInstancesList[i].localUser != null);
			}
			int num = 0;
			for (int j = 0; j < readOnlyInstancesList.Count; j++)
			{
				MPEventSystem mpeventSystem = readOnlyInstancesList[j];
				int playerSlot;
				if (!readOnlyInstancesList[j].enabled)
				{
					playerSlot = -1;
				}
				else
				{
					num = (playerSlot = num) + 1;
				}
				mpeventSystem.playerSlot = playerSlot;
			}
			MPEventSystemManager.primaryEventSystem = ((count > 0) ? LocalUserManager.readOnlyLocalUsersList[0].eventSystem : MPEventSystemManager.combinedEventSystem);
			MPEventSystemManager.availability.MakeAvailable();
		}

		// Token: 0x06001505 RID: 5381 RVA: 0x00059BAE File Offset: 0x00057DAE
		static MPEventSystemManager()
		{
			LocalUserManager.onLocalUsersUpdated += MPEventSystemManager.RefreshEventSystems;
			RoR2Application.onLoad = (Action)Delegate.Combine(RoR2Application.onLoad, new Action(MPEventSystemManager.Initialize));
		}

		// Token: 0x040013A6 RID: 5030
		private static readonly Dictionary<int, MPEventSystem> eventSystems = new Dictionary<int, MPEventSystem>();

		// Token: 0x040013AA RID: 5034
		public static ResourceAvailability availability;
	}
}
