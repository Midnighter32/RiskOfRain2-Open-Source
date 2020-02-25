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
	// Token: 0x020003C6 RID: 966
	public static class LocalUserManager
	{
		// Token: 0x06001782 RID: 6018 RVA: 0x000664C0 File Offset: 0x000646C0
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

		// Token: 0x06001783 RID: 6019 RVA: 0x000664F8 File Offset: 0x000646F8
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

		// Token: 0x06001784 RID: 6020 RVA: 0x00066530 File Offset: 0x00064730
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

		// Token: 0x06001785 RID: 6021 RVA: 0x00066574 File Offset: 0x00064774
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

		// Token: 0x06001786 RID: 6022 RVA: 0x000665B6 File Offset: 0x000647B6
		public static LocalUser GetFirstLocalUser()
		{
			if (LocalUserManager.localUsersList.Count <= 0)
			{
				return null;
			}
			return LocalUserManager.localUsersList[0];
		}

		// Token: 0x06001787 RID: 6023 RVA: 0x000665D4 File Offset: 0x000647D4
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

		// Token: 0x06001788 RID: 6024 RVA: 0x0006660C File Offset: 0x0006480C
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

		// Token: 0x06001789 RID: 6025 RVA: 0x0006663C File Offset: 0x0006483C
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

		// Token: 0x0600178A RID: 6026 RVA: 0x000666B4 File Offset: 0x000648B4
		public static bool IsUserChangeSafe()
		{
			return SceneManager.GetActiveScene().name == "title";
		}

		// Token: 0x0600178B RID: 6027 RVA: 0x000666E0 File Offset: 0x000648E0
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

		// Token: 0x0600178C RID: 6028 RVA: 0x00066759 File Offset: 0x00064959
		private static Player GetRewiredMainPlayer()
		{
			return ReInput.players.GetPlayer("PlayerMain");
		}

		// Token: 0x0600178D RID: 6029 RVA: 0x0006676A File Offset: 0x0006496A
		private static void AddMainUser(UserProfile userProfile)
		{
			LocalUserManager.AddUser(LocalUserManager.GetRewiredMainPlayer(), userProfile);
		}

		// Token: 0x0600178E RID: 6030 RVA: 0x00066778 File Offset: 0x00064978
		private static void RemoveUser(Player inputPlayer)
		{
			int num = LocalUserManager.FindUserIndex(inputPlayer);
			if (num != -1)
			{
				LocalUserManager.RemoveUser(num);
			}
		}

		// Token: 0x0600178F RID: 6031 RVA: 0x00066798 File Offset: 0x00064998
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

		// Token: 0x06001790 RID: 6032 RVA: 0x000667FC File Offset: 0x000649FC
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

		// Token: 0x06001791 RID: 6033 RVA: 0x00066838 File Offset: 0x00064A38
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

		// Token: 0x06001792 RID: 6034 RVA: 0x00066893 File Offset: 0x00064A93
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			RoR2Application.onUpdate += LocalUserManager.Update;
		}

		// Token: 0x06001793 RID: 6035 RVA: 0x000668A8 File Offset: 0x00064AA8
		private static void Update()
		{
			for (int i = 0; i < LocalUserManager.localUsersList.Count; i++)
			{
				LocalUserManager.localUsersList[i].RebuildControlChain();
			}
		}

		// Token: 0x14000054 RID: 84
		// (add) Token: 0x06001794 RID: 6036 RVA: 0x000668DC File Offset: 0x00064ADC
		// (remove) Token: 0x06001795 RID: 6037 RVA: 0x00066910 File Offset: 0x00064B10
		public static event Action<LocalUser> onUserSignIn;

		// Token: 0x14000055 RID: 85
		// (add) Token: 0x06001796 RID: 6038 RVA: 0x00066944 File Offset: 0x00064B44
		// (remove) Token: 0x06001797 RID: 6039 RVA: 0x00066978 File Offset: 0x00064B78
		public static event Action<LocalUser> onUserSignOut;

		// Token: 0x06001798 RID: 6040 RVA: 0x000669AB File Offset: 0x00064BAB
		[ConCommand(commandName = "remove_all_local_users", flags = ConVarFlags.None, helpText = "Removes all local users.")]
		private static void CCRemoveAllLocalUsers(ConCommandArgs args)
		{
			LocalUserManager.ClearUsers();
		}

		// Token: 0x06001799 RID: 6041 RVA: 0x000669B4 File Offset: 0x00064BB4
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

		// Token: 0x0600179A RID: 6042 RVA: 0x00066A74 File Offset: 0x00064C74
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

		// Token: 0x0600179B RID: 6043 RVA: 0x00066B00 File Offset: 0x00064D00
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

		// Token: 0x14000056 RID: 86
		// (add) Token: 0x0600179C RID: 6044 RVA: 0x00066B98 File Offset: 0x00064D98
		// (remove) Token: 0x0600179D RID: 6045 RVA: 0x00066BCC File Offset: 0x00064DCC
		public static event Action onLocalUsersUpdated;

		// Token: 0x0400163C RID: 5692
		private static readonly List<LocalUser> localUsersList = new List<LocalUser>();

		// Token: 0x0400163D RID: 5693
		public static readonly ReadOnlyCollection<LocalUser> readOnlyLocalUsersList = LocalUserManager.localUsersList.AsReadOnly();

		// Token: 0x0400163E RID: 5694
		public static Player startPlayer;

		// Token: 0x020003C7 RID: 967
		public struct LocalUserInitializationInfo
		{
			// Token: 0x04001642 RID: 5698
			public Player player;

			// Token: 0x04001643 RID: 5699
			public UserProfile profile;
		}

		// Token: 0x020003C8 RID: 968
		private class UserProfileMainConVar : BaseConVar
		{
			// Token: 0x0600179F RID: 6047 RVA: 0x0000972B File Offset: 0x0000792B
			public UserProfileMainConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x060017A0 RID: 6048 RVA: 0x00066C1C File Offset: 0x00064E1C
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

			// Token: 0x060017A1 RID: 6049 RVA: 0x00066C5C File Offset: 0x00064E5C
			public override string GetString()
			{
				int num = LocalUserManager.FindUserIndex(LocalUserManager.GetRewiredMainPlayer());
				if (num == -1)
				{
					return "";
				}
				return LocalUserManager.localUsersList[num].userProfile.fileName;
			}

			// Token: 0x04001644 RID: 5700
			private static LocalUserManager.UserProfileMainConVar cvClCurrentUserProfile = new LocalUserManager.UserProfileMainConVar("user_profile_main", ConVarFlags.Archive, null, "The current user profile.");
		}
	}
}
