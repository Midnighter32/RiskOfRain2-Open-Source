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
	// Token: 0x02000408 RID: 1032
	public class MPEventSystemManager : MonoBehaviour
	{
		// Token: 0x1700021B RID: 539
		// (get) Token: 0x060016F6 RID: 5878 RVA: 0x0006D5F1 File Offset: 0x0006B7F1
		// (set) Token: 0x060016F7 RID: 5879 RVA: 0x0006D5F8 File Offset: 0x0006B7F8
		public static MPEventSystem combinedEventSystem { get; private set; }

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x060016F8 RID: 5880 RVA: 0x0006D600 File Offset: 0x0006B800
		// (set) Token: 0x060016F9 RID: 5881 RVA: 0x0006D607 File Offset: 0x0006B807
		public static MPEventSystem kbmEventSystem { get; private set; }

		// Token: 0x1700021D RID: 541
		// (get) Token: 0x060016FA RID: 5882 RVA: 0x0006D60F File Offset: 0x0006B80F
		// (set) Token: 0x060016FB RID: 5883 RVA: 0x0006D616 File Offset: 0x0006B816
		public static MPEventSystem primaryEventSystem { get; private set; }

		// Token: 0x060016FC RID: 5884 RVA: 0x0006D620 File Offset: 0x0006B820
		public static MPEventSystem FindEventSystem(Player inputPlayer)
		{
			MPEventSystem result;
			MPEventSystemManager.eventSystems.TryGetValue(inputPlayer.id, out result);
			return result;
		}

		// Token: 0x060016FD RID: 5885 RVA: 0x0006D644 File Offset: 0x0006B844
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

		// Token: 0x060016FE RID: 5886 RVA: 0x0006D73C File Offset: 0x0006B93C
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

		// Token: 0x060016FF RID: 5887 RVA: 0x0006D7FE File Offset: 0x0006B9FE
		static MPEventSystemManager()
		{
			LocalUserManager.onLocalUsersUpdated += MPEventSystemManager.RefreshEventSystems;
			RoR2Application.onLoad = (Action)Delegate.Combine(RoR2Application.onLoad, new Action(MPEventSystemManager.Initialize));
		}

		// Token: 0x04001A36 RID: 6710
		private static readonly Dictionary<int, MPEventSystem> eventSystems = new Dictionary<int, MPEventSystem>();

		// Token: 0x04001A3A RID: 6714
		public static ResourceAvailability availability;
	}
}
