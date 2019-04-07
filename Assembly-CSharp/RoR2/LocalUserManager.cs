using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Rewired;
using RoR2.ConVar;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace RoR2
{
	// Token: 0x02000453 RID: 1107
	public static class LocalUserManager
	{
		// Token: 0x060018BC RID: 6332 RVA: 0x00076F88 File Offset: 0x00075188
		public static bool UserExists(Player inputPlayer)
		{
			for (int i = 0; i < LocalUserManager.localUsersList.Count; i++)
			{
				if (LocalUserManager.localUsersList[i].inputPlayer == inputPlayer)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060018BD RID: 6333 RVA: 0x00076FC0 File Offset: 0x000751C0
		private static int FindUserIndex(int userId)
		{
			for (int i = 0; i < LocalUserManager.localUsersList.Count; i++)
			{
				if (LocalUserManager.localUsersList[i].id == userId)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x060018BE RID: 6334 RVA: 0x00076FF8 File Offset: 0x000751F8
		public static LocalUser FindLocalUser(int userId)
		{
			for (int i = 0; i < LocalUserManager.localUsersList.Count; i++)
			{
				if (LocalUserManager.localUsersList[i].id == userId)
				{
					return LocalUserManager.localUsersList[i];
				}
			}
			return null;
		}

		// Token: 0x060018BF RID: 6335 RVA: 0x0007703C File Offset: 0x0007523C
		public static LocalUser FindLocalUser(Player inputPlayer)
		{
			for (int i = 0; i < LocalUserManager.localUsersList.Count; i++)
			{
				if (LocalUserManager.localUsersList[i].inputPlayer == inputPlayer)
				{
					return LocalUserManager.localUsersList[i];
				}
			}
			return null;
		}

		// Token: 0x060018C0 RID: 6336 RVA: 0x0007707E File Offset: 0x0007527E
		public static LocalUser GetFirstLocalUser()
		{
			if (LocalUserManager.localUsersList.Count <= 0)
			{
				return null;
			}
			return LocalUserManager.localUsersList[0];
		}

		// Token: 0x060018C1 RID: 6337 RVA: 0x0007709C File Offset: 0x0007529C
		private static int FindUserIndex(Player inputPlayer)
		{
			for (int i = 0; i < LocalUserManager.localUsersList.Count; i++)
			{
				if (LocalUserManager.localUsersList[i].inputPlayer == inputPlayer)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x060018C2 RID: 6338 RVA: 0x000770D4 File Offset: 0x000752D4
		private static int GetFirstAvailableId()
		{
			int i;
			for (i = 0; i < LocalUserManager.localUsersList.Count; i++)
			{
				if (LocalUserManager.FindUserIndex(i) == -1)
				{
					return i;
				}
			}
			return i;
		}

		// Token: 0x060018C3 RID: 6339 RVA: 0x00077104 File Offset: 0x00075304
		private static void AddUser(Player inputPlayer, UserProfile userProfile)
		{
			if (LocalUserManager.UserExists(inputPlayer))
			{
				return;
			}
			int firstAvailableId = LocalUserManager.GetFirstAvailableId();
			LocalUser localUser = new LocalUser
			{
				inputPlayer = inputPlayer,
				id = firstAvailableId,
				userProfile = userProfile
			};
			LocalUserManager.localUsersList.Add(localUser);
			userProfile.OnLogin();
			MPEventSystem.FindByPlayer(inputPlayer).localUser = localUser;
			if (LocalUserManager.onUserSignIn != null)
			{
				LocalUserManager.onUserSignIn(localUser);
			}
			if (LocalUserManager.onLocalUsersUpdated != null)
			{
				LocalUserManager.onLocalUsersUpdated();
			}
		}

		// Token: 0x060018C4 RID: 6340 RVA: 0x0007717C File Offset: 0x0007537C
		public static bool IsUserChangeSafe()
		{
			return SceneManager.GetActiveScene().name == "title";
		}

		// Token: 0x060018C5 RID: 6341 RVA: 0x000771A8 File Offset: 0x000753A8
		public static void SetLocalUsers(LocalUserManager.LocalUserInitializationInfo[] initializationInfo)
		{
			if (LocalUserManager.localUsersList.Count > 0)
			{
				Debug.LogError("Cannot call LocalUserManager.SetLocalUsers while users are already signed in!");
				return;
			}
			if (!LocalUserManager.IsUserChangeSafe())
			{
				Debug.LogError("Cannot call LocalUserManager.SetLocalUsers at this time, user login changes are not safe at this time.");
				return;
			}
			if (initializationInfo.Length == 1)
			{
				initializationInfo[0].player = LocalUserManager.GetRewiredMainPlayer();
			}
			for (int i = 0; i < initializationInfo.Length; i++)
			{
				LocalUserManager.AddUser(initializationInfo[i].player, initializationInfo[i].profile);
			}
		}

		// Token: 0x060018C6 RID: 6342 RVA: 0x00077221 File Offset: 0x00075421
		private static Player GetRewiredMainPlayer()
		{
			return ReInput.players.GetPlayer("PlayerMain");
		}

		// Token: 0x060018C7 RID: 6343 RVA: 0x00077232 File Offset: 0x00075432
		private static void AddMainUser(UserProfile userProfile)
		{
			LocalUserManager.AddUser(LocalUserManager.GetRewiredMainPlayer(), userProfile);
		}

		// Token: 0x060018C8 RID: 6344 RVA: 0x00077240 File Offset: 0x00075440
		private static void RemoveUser(Player inputPlayer)
		{
			int num = LocalUserManager.FindUserIndex(inputPlayer);
			if (num != -1)
			{
				LocalUserManager.RemoveUser(num);
			}
		}

		// Token: 0x060018C9 RID: 6345 RVA: 0x00077260 File Offset: 0x00075460
		private static void RemoveUser(int userIndex)
		{
			LocalUser localUser = LocalUserManager.localUsersList[userIndex];
			if (LocalUserManager.onUserSignOut != null)
			{
				LocalUserManager.onUserSignOut(localUser);
			}
			localUser.userProfile.OnLogout();
			MPEventSystem.FindByPlayer(localUser.inputPlayer).localUser = null;
			LocalUserManager.localUsersList.RemoveAt(userIndex);
			if (LocalUserManager.onLocalUsersUpdated != null)
			{
				LocalUserManager.onLocalUsersUpdated();
			}
		}

		// Token: 0x060018CA RID: 6346 RVA: 0x000772C4 File Offset: 0x000754C4
		public static void ClearUsers()
		{
			if (!LocalUserManager.IsUserChangeSafe())
			{
				Debug.LogError("Cannot call LocalUserManager.SetLocalUsers at this time, user login changes are not safe at this time.");
				return;
			}
			for (int i = LocalUserManager.localUsersList.Count - 1; i >= 0; i--)
			{
				LocalUserManager.RemoveUser(i);
			}
		}

		// Token: 0x060018CB RID: 6347 RVA: 0x00077300 File Offset: 0x00075500
		private static Player ListenForStartSignIn()
		{
			IList<Player> players = ReInput.players.Players;
			for (int i = 0; i < players.Count; i++)
			{
				Player player = players[i];
				if (!(player.name == "PlayerMain") && !LocalUserManager.UserExists(player) && player.GetButtonDown("Start"))
				{
					return player;
				}
			}
			return null;
		}

		// Token: 0x060018CC RID: 6348 RVA: 0x0007735B File Offset: 0x0007555B
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			RoR2Application.onUpdate += LocalUserManager.Update;
		}

		// Token: 0x060018CD RID: 6349 RVA: 0x00077370 File Offset: 0x00075570
		private static void Update()
		{
			for (int i = 0; i < LocalUserManager.localUsersList.Count; i++)
			{
				LocalUserManager.localUsersList[i].RebuildControlChain();
			}
		}

		// Token: 0x14000038 RID: 56
		// (add) Token: 0x060018CE RID: 6350 RVA: 0x000773A4 File Offset: 0x000755A4
		// (remove) Token: 0x060018CF RID: 6351 RVA: 0x000773D8 File Offset: 0x000755D8
		public static event Action<LocalUser> onUserSignIn;

		// Token: 0x14000039 RID: 57
		// (add) Token: 0x060018D0 RID: 6352 RVA: 0x0007740C File Offset: 0x0007560C
		// (remove) Token: 0x060018D1 RID: 6353 RVA: 0x00077440 File Offset: 0x00075640
		public static event Action<LocalUser> onUserSignOut;

		// Token: 0x060018D2 RID: 6354 RVA: 0x00077473 File Offset: 0x00075673
		[ConCommand(commandName = "remove_all_local_users", flags = ConVarFlags.None, helpText = "Removes all local users.")]
		private static void CCRemoveAllLocalUsers(ConCommandArgs args)
		{
			LocalUserManager.ClearUsers();
		}

		// Token: 0x060018D3 RID: 6355 RVA: 0x0007747C File Offset: 0x0007567C
		[ConCommand(commandName = "print_local_users", flags = ConVarFlags.None, helpText = "Prints a list of all local users.")]
		private static void CCPrintLocalUsers(ConCommandArgs args)
		{
			string[] array = new string[LocalUserManager.localUsersList.Count];
			for (int i = 0; i < LocalUserManager.localUsersList.Count; i++)
			{
				if (LocalUserManager.localUsersList[i] != null)
				{
					array[i] = string.Format("localUsersList[{0}] id={1} userProfile={2}", i, LocalUserManager.localUsersList[i].id, (LocalUserManager.localUsersList[i].userProfile != null) ? LocalUserManager.localUsersList[i].userProfile.fileName : "null");
				}
				else
				{
					array[i] = string.Format("localUsersList[{0}] null", i);
				}
			}
			Debug.Log(string.Join("\n", array));
		}

		// Token: 0x060018D4 RID: 6356 RVA: 0x0007753C File Offset: 0x0007573C
		[ConCommand(commandName = "test_splitscreen", flags = ConVarFlags.None, helpText = "Logs in the specified number of guest users, or two by default.")]
		private static void CCTestSplitscreen(ConCommandArgs args)
		{
			int num = 2;
			int value;
			if (args.Count >= 1 && TextSerialization.TryParseInvariant(args[0], out value))
			{
				num = Mathf.Clamp(value, 1, 4);
			}
			if (!NetworkClient.active)
			{
				LocalUserManager.ClearUsers();
				LocalUserManager.LocalUserInitializationInfo[] array = new LocalUserManager.LocalUserInitializationInfo[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = new LocalUserManager.LocalUserInitializationInfo
					{
						player = ReInput.players.GetPlayer(2 + i),
						profile = UserProfile.CreateGuestProfile()
					};
				}
				LocalUserManager.SetLocalUsers(array);
			}
		}

		// Token: 0x060018D5 RID: 6357 RVA: 0x000775C8 File Offset: 0x000757C8
		[ConCommand(commandName = "export_controller_maps", flags = ConVarFlags.None, helpText = "Prints all Rewired ControllerMaps of the first player as xml.")]
		public static void CCExportControllerMaps(ConCommandArgs args)
		{
			if (LocalUserManager.localUsersList.Count <= 0)
			{
				return;
			}
			foreach (string message in from v in LocalUserManager.localUsersList[0].inputPlayer.controllers.maps.GetAllMaps()
			select v.ToXmlString())
			{
				Debug.Log(message);
			}
		}

		// Token: 0x1400003A RID: 58
		// (add) Token: 0x060018D6 RID: 6358 RVA: 0x00077660 File Offset: 0x00075860
		// (remove) Token: 0x060018D7 RID: 6359 RVA: 0x00077694 File Offset: 0x00075894
		public static event Action onLocalUsersUpdated;

		// Token: 0x04001C49 RID: 7241
		private static readonly List<LocalUser> localUsersList = new List<LocalUser>();

		// Token: 0x04001C4A RID: 7242
		public static readonly ReadOnlyCollection<LocalUser> readOnlyLocalUsersList = LocalUserManager.localUsersList.AsReadOnly();

		// Token: 0x04001C4B RID: 7243
		public static Player startPlayer;

		// Token: 0x02000454 RID: 1108
		public struct LocalUserInitializationInfo
		{
			// Token: 0x04001C4F RID: 7247
			public Player player;

			// Token: 0x04001C50 RID: 7248
			public UserProfile profile;
		}

		// Token: 0x02000455 RID: 1109
		private class UserProfileMainConVar : BaseConVar
		{
			// Token: 0x060018D9 RID: 6361 RVA: 0x00037E38 File Offset: 0x00036038
			public UserProfileMainConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x060018DA RID: 6362 RVA: 0x000776E4 File Offset: 0x000758E4
			public override void SetString(string newValue)
			{
				if (LocalUserManager.readOnlyLocalUsersList.Count > 0)
				{
					Debug.Log("Can't change user_profile_main while there are users signed in.");
					return;
				}
				UserProfile profile = UserProfile.GetProfile(newValue);
				if (profile != null && !profile.isCorrupted)
				{
					LocalUserManager.AddMainUser(profile);
				}
			}

			// Token: 0x060018DB RID: 6363 RVA: 0x00077724 File Offset: 0x00075924
			public override string GetString()
			{
				int num = LocalUserManager.FindUserIndex(LocalUserManager.GetRewiredMainPlayer());
				if (num == -1)
				{
					return "";
				}
				return LocalUserManager.localUsersList[num].userProfile.fileName;
			}

			// Token: 0x04001C51 RID: 7249
			private static LocalUserManager.UserProfileMainConVar cvClCurrentUserProfile = new LocalUserManager.UserProfileMainConVar("user_profile_main", ConVarFlags.Archive, null, "The current user profile.");
		}
	}
}
