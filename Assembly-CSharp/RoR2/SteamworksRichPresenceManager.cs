using System;
using System.Collections.Generic;
using System.Globalization;
using Facepunch.Steamworks;
using JetBrains.Annotations;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace RoR2
{
	// Token: 0x0200043C RID: 1084
	internal static class SteamworksRichPresenceManager
	{
		// Token: 0x06001A58 RID: 6744 RVA: 0x000705A3 File Offset: 0x0006E7A3
		private static void SetKeyValue([NotNull] string key, [CanBeNull] string value)
		{
			Client.Instance.User.SetRichPresence(key, value);
		}

		// Token: 0x06001A59 RID: 6745 RVA: 0x000705B8 File Offset: 0x0006E7B8
		private static void OnNetworkStart()
		{
			string text = null;
			CSteamID csteamID = CSteamID.nil;
			CSteamID csteamID2 = CSteamID.nil;
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			for (int i = 0; i < commandLineArgs.Length - 1; i++)
			{
				string a = commandLineArgs[i].ToLower(CultureInfo.InvariantCulture);
				CSteamID csteamID4;
				if (a == "+connect")
				{
					AddressPortPair addressPortPair;
					if (AddressPortPair.TryParse(commandLineArgs[i + 1], out addressPortPair))
					{
						text = addressPortPair.address + ":" + addressPortPair.port;
					}
				}
				else if (a == "+connect_steamworks_p2p")
				{
					CSteamID csteamID3;
					if (CSteamID.TryParse(commandLineArgs[i + 1], out csteamID3))
					{
						csteamID = csteamID3;
					}
				}
				else if (a == "+steam_lobby_join" && CSteamID.TryParse(commandLineArgs[i + 1], out csteamID4))
				{
					csteamID2 = csteamID4;
				}
			}
			if (csteamID2 != CSteamID.nil)
			{
				Console.instance.SubmitCmd(null, "steam_lobby_join " + csteamID2.value, false);
				return;
			}
			if (csteamID != CSteamID.nil)
			{
				Console.instance.SubmitCmd(null, "connect_steamworks_p2p " + csteamID.value, false);
				return;
			}
			if (text != null)
			{
				Console.instance.SubmitCmd(null, "connect " + text, false);
			}
		}

		// Token: 0x06001A5A RID: 6746 RVA: 0x000706FC File Offset: 0x0006E8FC
		private static void OnLobbyChanged()
		{
			if (Client.Instance.Lobby.IsValid)
			{
				SteamworksRichPresenceManager.SetKeyValue("connect", "+steam_lobby_join " + Client.Instance.Lobby.CurrentLobby);
				return;
			}
			SteamworksRichPresenceManager.SetKeyValue("connect", null);
		}

		// Token: 0x06001A5B RID: 6747 RVA: 0x00070750 File Offset: 0x0006E950
		private static void OnInvitedToGame(SteamFriend steamFriend, string connectString)
		{
			Debug.LogFormat("OnGameRichPresenceJoinRequested connectString=\"{0}\" steamFriend=\"{1}\"", new object[]
			{
				connectString,
				steamFriend.Name
			});
			string[] array = connectString.Split(new char[]
			{
				' '
			});
			if (array.Length >= 2)
			{
				CSteamID csteamID;
				if (array[0] == "+connect_steamworks_p2p" && CSteamID.TryParse(array[1], out csteamID))
				{
					if (!SteamworksLobbyManager.ownsLobby)
					{
						SteamworksLobbyManager.LeaveLobby();
					}
					QuitConfirmationHelper.IssueQuitCommand(null, "connect_steamworks_p2p " + csteamID.value);
				}
				CSteamID csteamID2;
				if (array[0] == "+steam_lobby_join" && CSteamID.TryParse(array[1], out csteamID2))
				{
					if (!SteamworksLobbyManager.ownsLobby)
					{
						SteamworksLobbyManager.LeaveLobby();
					}
					QuitConfirmationHelper.IssueQuitCommand(null, "steam_lobby_join " + csteamID2.value);
				}
			}
		}

		// Token: 0x06001A5C RID: 6748 RVA: 0x0007081C File Offset: 0x0006EA1C
		private static void OnGameServerChangeRequested(string address, string password)
		{
			Debug.LogFormat("OnGameServerChangeRequested address=\"{0}\"", new object[]
			{
				address
			});
			if (!SteamworksLobbyManager.ownsLobby)
			{
				SteamworksLobbyManager.LeaveLobby();
			}
			string consoleCmd = string.Format("cl_password \"{0}\"; connect \"{1}\"", Util.EscapeQuotes(password), Util.EscapeQuotes(address));
			QuitConfirmationHelper.IssueQuitCommand(null, consoleCmd);
		}

		// Token: 0x06001A5D RID: 6749 RVA: 0x00070868 File Offset: 0x0006EA68
		private static void SetupCallbacks()
		{
			GameNetworkManager.onStartGlobal += SteamworksRichPresenceManager.OnNetworkStart;
			SteamworksLobbyManager.onLobbyChanged += SteamworksRichPresenceManager.OnLobbyChanged;
			if (Client.Instance != null)
			{
				Client.Instance.Friends.OnInvitedToGame += SteamworksRichPresenceManager.OnInvitedToGame;
				Client.Instance.Friends.OnGameServerChangeRequested = new Action<string, string>(SteamworksRichPresenceManager.OnGameServerChangeRequested);
			}
			new SteamworksRichPresenceManager.DifficultyField().Install();
			new SteamworksRichPresenceManager.GameModeField().Install();
			new SteamworksRichPresenceManager.ParticipationField().Install();
			new SteamworksRichPresenceManager.MinutesField().Install();
			new SteamworksRichPresenceManager.SteamPlayerGroupField().Install();
			new SteamworksRichPresenceManager.SteamDisplayField().Install();
			RoR2Application.onUpdate += SteamworksRichPresenceManager.BaseRichPresenceField.ProcessDirtyFields;
		}

		// Token: 0x06001A5E RID: 6750 RVA: 0x00070921 File Offset: 0x0006EB21
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			SteamworksClientManager.onLoaded += SteamworksRichPresenceManager.SetupCallbacks;
		}

		// Token: 0x04001823 RID: 6179
		private const string rpConnect = "connect";

		// Token: 0x04001824 RID: 6180
		private const string rpStatus = "status";

		// Token: 0x04001825 RID: 6181
		private const string rpSteamDisplay = "steam_display";

		// Token: 0x04001826 RID: 6182
		private const string rpSteamPlayerGroup = "steam_player_group";

		// Token: 0x04001827 RID: 6183
		private const string rpSteamPlayerGroupSize = "steam_player_group_size";

		// Token: 0x04001828 RID: 6184
		private const string rpDifficulty = "difficulty";

		// Token: 0x04001829 RID: 6185
		private const string rpGameMode = "gamemode";

		// Token: 0x0400182A RID: 6186
		private const string rpParticipationType = "participation_type";

		// Token: 0x0400182B RID: 6187
		private const string rpMinutes = "minutes";

		// Token: 0x0200043D RID: 1085
		private abstract class BaseRichPresenceField
		{
			// Token: 0x06001A5F RID: 6751 RVA: 0x00070934 File Offset: 0x0006EB34
			public static void ProcessDirtyFields()
			{
				while (SteamworksRichPresenceManager.BaseRichPresenceField.dirtyFields.Count > 0)
				{
					SteamworksRichPresenceManager.BaseRichPresenceField.dirtyFields.Dequeue().UpdateIfNecessary();
				}
			}

			// Token: 0x17000304 RID: 772
			// (get) Token: 0x06001A60 RID: 6752
			protected abstract string key { get; }

			// Token: 0x06001A61 RID: 6753
			[CanBeNull]
			protected abstract string RebuildValue();

			// Token: 0x06001A62 RID: 6754 RVA: 0x0000409B File Offset: 0x0000229B
			protected virtual void OnChanged()
			{
			}

			// Token: 0x06001A63 RID: 6755 RVA: 0x00070954 File Offset: 0x0006EB54
			public void SetDirty()
			{
				if (!this.isDirty)
				{
					this.isDirty = true;
					SteamworksRichPresenceManager.BaseRichPresenceField.dirtyFields.Enqueue(this);
				}
			}

			// Token: 0x06001A64 RID: 6756 RVA: 0x00070970 File Offset: 0x0006EB70
			private void UpdateIfNecessary()
			{
				if (!this.installed)
				{
					return;
				}
				this.isDirty = false;
				string a = this.RebuildValue();
				if (a != this.currentValue)
				{
					this.currentValue = a;
					SteamworksRichPresenceManager.SetKeyValue(this.key, this.currentValue);
					this.OnChanged();
				}
			}

			// Token: 0x06001A65 RID: 6757 RVA: 0x0000409B File Offset: 0x0000229B
			protected virtual void OnInstall()
			{
			}

			// Token: 0x06001A66 RID: 6758 RVA: 0x0000409B File Offset: 0x0000229B
			protected virtual void OnUninstall()
			{
			}

			// Token: 0x06001A67 RID: 6759 RVA: 0x000709C0 File Offset: 0x0006EBC0
			public void Install()
			{
				if (!this.installed)
				{
					this.OnInstall();
					this.SetDirty();
					this.installed = true;
				}
			}

			// Token: 0x06001A68 RID: 6760 RVA: 0x000709DD File Offset: 0x0006EBDD
			public void Uninstall()
			{
				if (this.installed)
				{
					this.OnUninstall();
					this.installed = false;
					SteamworksRichPresenceManager.SetKeyValue(this.key, null);
				}
			}

			// Token: 0x06001A69 RID: 6761 RVA: 0x00070A00 File Offset: 0x0006EC00
			protected void SetDirtyableValue<T>(ref T field, T value) where T : struct, IEquatable<T>
			{
				if (!field.Equals(value))
				{
					field = value;
					this.SetDirty();
				}
			}

			// Token: 0x06001A6A RID: 6762 RVA: 0x00070A1E File Offset: 0x0006EC1E
			protected void SetDirtyableReference<T>(ref T field, T value) where T : class
			{
				if (field != value)
				{
					field = value;
					this.SetDirty();
				}
			}

			// Token: 0x0400182C RID: 6188
			private static readonly Queue<SteamworksRichPresenceManager.BaseRichPresenceField> dirtyFields = new Queue<SteamworksRichPresenceManager.BaseRichPresenceField>();

			// Token: 0x0400182D RID: 6189
			private bool isDirty;

			// Token: 0x0400182E RID: 6190
			[CanBeNull]
			private string currentValue;

			// Token: 0x0400182F RID: 6191
			private bool installed;
		}

		// Token: 0x0200043E RID: 1086
		private sealed class DifficultyField : SteamworksRichPresenceManager.BaseRichPresenceField
		{
			// Token: 0x17000305 RID: 773
			// (get) Token: 0x06001A6D RID: 6765 RVA: 0x00070A4C File Offset: 0x0006EC4C
			protected override string key
			{
				get
				{
					return "difficulty";
				}
			}

			// Token: 0x06001A6E RID: 6766 RVA: 0x00070A54 File Offset: 0x0006EC54
			protected override string RebuildValue()
			{
				if (!Run.instance)
				{
					return null;
				}
				switch (Run.instance.selectedDifficulty)
				{
				case DifficultyIndex.Easy:
					return "Easy";
				case DifficultyIndex.Normal:
					return "Normal";
				case DifficultyIndex.Hard:
					return "Hard";
				default:
					return null;
				}
			}

			// Token: 0x06001A6F RID: 6767 RVA: 0x00070AA1 File Offset: 0x0006ECA1
			private void SetDirty(Run run)
			{
				base.SetDirty();
			}

			// Token: 0x06001A70 RID: 6768 RVA: 0x00070AA9 File Offset: 0x0006ECA9
			protected override void OnInstall()
			{
				base.OnInstall();
				Run.onRunStartGlobal += this.SetDirty;
				Run.onRunDestroyGlobal += this.SetDirty;
			}

			// Token: 0x06001A71 RID: 6769 RVA: 0x00070AD3 File Offset: 0x0006ECD3
			protected override void OnUninstall()
			{
				Run.onRunStartGlobal -= this.SetDirty;
				Run.onRunDestroyGlobal -= this.SetDirty;
				base.OnUninstall();
			}
		}

		// Token: 0x0200043F RID: 1087
		private sealed class GameModeField : SteamworksRichPresenceManager.BaseRichPresenceField
		{
			// Token: 0x17000306 RID: 774
			// (get) Token: 0x06001A73 RID: 6771 RVA: 0x00070B05 File Offset: 0x0006ED05
			protected override string key
			{
				get
				{
					return "gamemode";
				}
			}

			// Token: 0x06001A74 RID: 6772 RVA: 0x00070B0C File Offset: 0x0006ED0C
			protected override string RebuildValue()
			{
				if (!Run.instance)
				{
					return null;
				}
				Run run = GameModeCatalog.FindGameModePrefabComponent(Run.instance.name);
				if (run == null)
				{
					return null;
				}
				return run.name;
			}

			// Token: 0x06001A75 RID: 6773 RVA: 0x00070AA1 File Offset: 0x0006ECA1
			private void SetDirty(Run run)
			{
				base.SetDirty();
			}

			// Token: 0x06001A76 RID: 6774 RVA: 0x00070B36 File Offset: 0x0006ED36
			protected override void OnInstall()
			{
				base.OnInstall();
				Run.onRunStartGlobal += this.SetDirty;
				Run.onRunDestroyGlobal += this.SetDirty;
			}

			// Token: 0x06001A77 RID: 6775 RVA: 0x00070B60 File Offset: 0x0006ED60
			protected override void OnUninstall()
			{
				Run.onRunStartGlobal -= this.SetDirty;
				Run.onRunDestroyGlobal -= this.SetDirty;
				base.OnUninstall();
			}
		}

		// Token: 0x02000440 RID: 1088
		private sealed class ParticipationField : SteamworksRichPresenceManager.BaseRichPresenceField
		{
			// Token: 0x17000307 RID: 775
			// (get) Token: 0x06001A79 RID: 6777 RVA: 0x00070B8A File Offset: 0x0006ED8A
			protected override string key
			{
				get
				{
					return "participation_type";
				}
			}

			// Token: 0x06001A7A RID: 6778 RVA: 0x00070B91 File Offset: 0x0006ED91
			private void SetParticipationType(SteamworksRichPresenceManager.ParticipationField.ParticipationType newParticipationType)
			{
				if (this.participationType != newParticipationType)
				{
					this.participationType = newParticipationType;
					base.SetDirty();
				}
			}

			// Token: 0x06001A7B RID: 6779 RVA: 0x00070BAC File Offset: 0x0006EDAC
			protected override string RebuildValue()
			{
				switch (this.participationType)
				{
				case SteamworksRichPresenceManager.ParticipationField.ParticipationType.Alive:
					return "Alive";
				case SteamworksRichPresenceManager.ParticipationField.ParticipationType.Dead:
					return "Dead";
				case SteamworksRichPresenceManager.ParticipationField.ParticipationType.Spectator:
					return "Spectator";
				default:
					return null;
				}
			}

			// Token: 0x06001A7C RID: 6780 RVA: 0x00070BEC File Offset: 0x0006EDEC
			protected override void OnInstall()
			{
				base.OnInstall();
				LocalUserManager.onUserSignIn += this.OnLocalUserDiscovered;
				LocalUserManager.onUserSignOut += this.OnLocalUserLost;
				Run.onRunStartGlobal += this.OnRunStart;
				Run.onRunDestroyGlobal += this.OnRunDestroy;
			}

			// Token: 0x06001A7D RID: 6781 RVA: 0x00070C44 File Offset: 0x0006EE44
			protected override void OnUninstall()
			{
				LocalUserManager.onUserSignIn -= this.OnLocalUserDiscovered;
				LocalUserManager.onUserSignOut -= this.OnLocalUserLost;
				Run.onRunStartGlobal -= this.OnRunStart;
				Run.onRunDestroyGlobal -= this.OnRunDestroy;
				this.SetCurrentMaster(null);
			}

			// Token: 0x06001A7E RID: 6782 RVA: 0x00070C9C File Offset: 0x0006EE9C
			private void SetTrackedUser(LocalUser newTrackedUser)
			{
				if (this.trackedUser != null)
				{
					this.trackedUser.onMasterChanged -= this.OnMasterChanged;
				}
				this.trackedUser = newTrackedUser;
				if (this.trackedUser != null)
				{
					this.trackedUser.onMasterChanged += this.OnMasterChanged;
				}
			}

			// Token: 0x06001A7F RID: 6783 RVA: 0x00070CEE File Offset: 0x0006EEEE
			private void OnLocalUserDiscovered(LocalUser localUser)
			{
				if (this.trackedUser == null)
				{
					this.SetTrackedUser(localUser);
				}
			}

			// Token: 0x06001A80 RID: 6784 RVA: 0x00070CFF File Offset: 0x0006EEFF
			private void OnLocalUserLost(LocalUser localUser)
			{
				if (this.trackedUser == localUser)
				{
					this.SetTrackedUser(null);
				}
			}

			// Token: 0x06001A81 RID: 6785 RVA: 0x00070D11 File Offset: 0x0006EF11
			private void OnRunStart(Run run)
			{
				if (this.trackedUser != null && !this.trackedUser.cachedMasterObject)
				{
					this.SetParticipationType(SteamworksRichPresenceManager.ParticipationField.ParticipationType.Spectator);
				}
			}

			// Token: 0x06001A82 RID: 6786 RVA: 0x00070D34 File Offset: 0x0006EF34
			private void OnRunDestroy(Run run)
			{
				if (this.trackedUser != null)
				{
					this.SetParticipationType(SteamworksRichPresenceManager.ParticipationField.ParticipationType.None);
				}
			}

			// Token: 0x06001A83 RID: 6787 RVA: 0x00070D48 File Offset: 0x0006EF48
			private void OnMasterChanged()
			{
				PlayerCharacterMasterController cachedMasterController = this.trackedUser.cachedMasterController;
				this.SetCurrentMaster(cachedMasterController ? cachedMasterController.master : null);
			}

			// Token: 0x06001A84 RID: 6788 RVA: 0x00070D78 File Offset: 0x0006EF78
			private void SetCurrentMaster(CharacterMaster newMaster)
			{
				if (this.currentMaster != null)
				{
					this.currentMaster.onBodyDeath.RemoveListener(new UnityAction(this.OnBodyDeath));
					this.currentMaster.onBodyStart -= this.OnBodyStart;
				}
				this.currentMaster = newMaster;
				if (this.currentMaster != null)
				{
					this.currentMaster.onBodyDeath.AddListener(new UnityAction(this.OnBodyDeath));
					this.currentMaster.onBodyStart += this.OnBodyStart;
				}
			}

			// Token: 0x06001A85 RID: 6789 RVA: 0x00070E02 File Offset: 0x0006F002
			private void OnBodyDeath()
			{
				this.SetParticipationType(SteamworksRichPresenceManager.ParticipationField.ParticipationType.Dead);
			}

			// Token: 0x06001A86 RID: 6790 RVA: 0x00070E0B File Offset: 0x0006F00B
			private void OnBodyStart(CharacterBody body)
			{
				this.SetParticipationType(SteamworksRichPresenceManager.ParticipationField.ParticipationType.Alive);
			}

			// Token: 0x04001830 RID: 6192
			private SteamworksRichPresenceManager.ParticipationField.ParticipationType participationType;

			// Token: 0x04001831 RID: 6193
			private LocalUser trackedUser;

			// Token: 0x04001832 RID: 6194
			private CharacterMaster currentMaster;

			// Token: 0x02000441 RID: 1089
			private enum ParticipationType
			{
				// Token: 0x04001834 RID: 6196
				None,
				// Token: 0x04001835 RID: 6197
				Alive,
				// Token: 0x04001836 RID: 6198
				Dead,
				// Token: 0x04001837 RID: 6199
				Spectator
			}
		}

		// Token: 0x02000442 RID: 1090
		private sealed class MinutesField : SteamworksRichPresenceManager.BaseRichPresenceField
		{
			// Token: 0x17000308 RID: 776
			// (get) Token: 0x06001A88 RID: 6792 RVA: 0x00070E14 File Offset: 0x0006F014
			protected override string key
			{
				get
				{
					return "minutes";
				}
			}

			// Token: 0x06001A89 RID: 6793 RVA: 0x00070E1B File Offset: 0x0006F01B
			protected override string RebuildValue()
			{
				return TextSerialization.ToStringInvariant(this.minutes);
			}

			// Token: 0x06001A8A RID: 6794 RVA: 0x00070E28 File Offset: 0x0006F028
			private void FixedUpdate()
			{
				uint value = 0U;
				if (Run.instance)
				{
					value = (uint)Mathf.FloorToInt(Run.instance.GetRunStopwatch() / 60f);
				}
				base.SetDirtyableValue<uint>(ref this.minutes, value);
			}

			// Token: 0x06001A8B RID: 6795 RVA: 0x00070E66 File Offset: 0x0006F066
			protected override void OnInstall()
			{
				base.OnInstall();
				RoR2Application.onFixedUpdate += this.FixedUpdate;
			}

			// Token: 0x06001A8C RID: 6796 RVA: 0x00070E7F File Offset: 0x0006F07F
			protected override void OnUninstall()
			{
				RoR2Application.onFixedUpdate -= this.FixedUpdate;
				base.OnUninstall();
			}

			// Token: 0x04001838 RID: 6200
			private uint minutes;
		}

		// Token: 0x02000443 RID: 1091
		private sealed class SteamPlayerGroupField : SteamworksRichPresenceManager.BaseRichPresenceField
		{
			// Token: 0x17000309 RID: 777
			// (get) Token: 0x06001A8E RID: 6798 RVA: 0x00070E98 File Offset: 0x0006F098
			protected override string key
			{
				get
				{
					return "steam_player_group";
				}
			}

			// Token: 0x06001A8F RID: 6799 RVA: 0x00070E9F File Offset: 0x0006F09F
			private void SetLobbyId(CSteamID newLobbyId)
			{
				if (this.lobbyId != newLobbyId)
				{
					this.lobbyId = newLobbyId;
					this.UpdateGroupID();
				}
			}

			// Token: 0x06001A90 RID: 6800 RVA: 0x00070EBC File Offset: 0x0006F0BC
			private void SetHostId(CSteamID newHostId)
			{
				if (this.hostId != newHostId)
				{
					this.hostId = newHostId;
					this.UpdateGroupID();
				}
			}

			// Token: 0x06001A91 RID: 6801 RVA: 0x00070ED9 File Offset: 0x0006F0D9
			private void SetGroupId(CSteamID newGroupId)
			{
				if (this.groupId != newGroupId)
				{
					this.groupId = newGroupId;
					base.SetDirty();
				}
			}

			// Token: 0x06001A92 RID: 6802 RVA: 0x00070EF8 File Offset: 0x0006F0F8
			private void UpdateGroupID()
			{
				if (this.hostId != CSteamID.nil)
				{
					this.SetGroupId(this.hostId);
					if (!(this.groupSizeField is SteamworksRichPresenceManager.SteamPlayerGroupSizeFieldGame))
					{
						SteamworksRichPresenceManager.SteamPlayerGroupSizeField steamPlayerGroupSizeField = this.groupSizeField;
						if (steamPlayerGroupSizeField != null)
						{
							steamPlayerGroupSizeField.Uninstall();
						}
						this.groupSizeField = new SteamworksRichPresenceManager.SteamPlayerGroupSizeFieldGame();
						this.groupSizeField.Install();
						return;
					}
				}
				else
				{
					this.SetGroupId(this.lobbyId);
					if (!(this.groupSizeField is SteamworksRichPresenceManager.SteamPlayerGroupSizeFieldLobby))
					{
						SteamworksRichPresenceManager.SteamPlayerGroupSizeField steamPlayerGroupSizeField2 = this.groupSizeField;
						if (steamPlayerGroupSizeField2 != null)
						{
							steamPlayerGroupSizeField2.Uninstall();
						}
						this.groupSizeField = new SteamworksRichPresenceManager.SteamPlayerGroupSizeFieldLobby();
						this.groupSizeField.Install();
					}
				}
			}

			// Token: 0x06001A93 RID: 6803 RVA: 0x00070F98 File Offset: 0x0006F198
			protected override void OnInstall()
			{
				base.OnInstall();
				GameNetworkManager.onClientConnectGlobal += this.OnClientConnectGlobal;
				GameNetworkManager.onClientDisconnectGlobal += this.OnClientDisconnectGlobal;
				GameNetworkManager.onStartServerGlobal += this.OnStartServerGlobal;
				GameNetworkManager.onStopServerGlobal += this.OnStopServerGlobal;
				SteamworksLobbyManager.onLobbyChanged += this.OnLobbyChanged;
			}

			// Token: 0x06001A94 RID: 6804 RVA: 0x00071000 File Offset: 0x0006F200
			protected override void OnUninstall()
			{
				GameNetworkManager.onClientConnectGlobal -= this.OnClientConnectGlobal;
				GameNetworkManager.onClientDisconnectGlobal -= this.OnClientDisconnectGlobal;
				GameNetworkManager.onStartServerGlobal -= this.OnStartServerGlobal;
				GameNetworkManager.onStopServerGlobal -= this.OnStopServerGlobal;
				SteamworksLobbyManager.onLobbyChanged -= this.OnLobbyChanged;
				SteamworksRichPresenceManager.SteamPlayerGroupSizeField steamPlayerGroupSizeField = this.groupSizeField;
				if (steamPlayerGroupSizeField != null)
				{
					steamPlayerGroupSizeField.Uninstall();
				}
				this.groupSizeField = null;
				base.OnUninstall();
			}

			// Token: 0x06001A95 RID: 6805 RVA: 0x00071080 File Offset: 0x0006F280
			protected override string RebuildValue()
			{
				if (this.groupId == CSteamID.nil)
				{
					return null;
				}
				return TextSerialization.ToStringInvariant(this.groupId.value);
			}

			// Token: 0x06001A96 RID: 6806 RVA: 0x000710A8 File Offset: 0x0006F2A8
			private void OnClientConnectGlobal(NetworkConnection conn)
			{
				SteamNetworkConnection steamNetworkConnection;
				if ((steamNetworkConnection = (conn as SteamNetworkConnection)) != null)
				{
					this.hostId = steamNetworkConnection.steamId;
				}
			}

			// Token: 0x06001A97 RID: 6807 RVA: 0x000710CB File Offset: 0x0006F2CB
			private void OnClientDisconnectGlobal(NetworkConnection conn)
			{
				this.hostId = CSteamID.nil;
			}

			// Token: 0x06001A98 RID: 6808 RVA: 0x000710D8 File Offset: 0x0006F2D8
			private void OnStartServerGlobal()
			{
				this.hostId = new CSteamID(Client.Instance.SteamId);
			}

			// Token: 0x06001A99 RID: 6809 RVA: 0x000710CB File Offset: 0x0006F2CB
			private void OnStopServerGlobal()
			{
				this.hostId = CSteamID.nil;
			}

			// Token: 0x06001A9A RID: 6810 RVA: 0x000710EF File Offset: 0x0006F2EF
			private void OnLobbyChanged()
			{
				this.SetLobbyId(new CSteamID(Client.Instance.Lobby.CurrentLobby));
			}

			// Token: 0x04001839 RID: 6201
			private CSteamID lobbyId = CSteamID.nil;

			// Token: 0x0400183A RID: 6202
			private CSteamID hostId = CSteamID.nil;

			// Token: 0x0400183B RID: 6203
			private CSteamID groupId = CSteamID.nil;

			// Token: 0x0400183C RID: 6204
			private SteamworksRichPresenceManager.SteamPlayerGroupSizeField groupSizeField;
		}

		// Token: 0x02000444 RID: 1092
		private abstract class SteamPlayerGroupSizeField : SteamworksRichPresenceManager.BaseRichPresenceField
		{
			// Token: 0x1700030A RID: 778
			// (get) Token: 0x06001A9C RID: 6812 RVA: 0x00071134 File Offset: 0x0006F334
			protected override string key
			{
				get
				{
					return "steam_player_group_size";
				}
			}

			// Token: 0x06001A9D RID: 6813 RVA: 0x0007113B File Offset: 0x0006F33B
			protected override string RebuildValue()
			{
				return TextSerialization.ToStringInvariant(this.groupSize);
			}

			// Token: 0x0400183D RID: 6205
			protected int groupSize;
		}

		// Token: 0x02000445 RID: 1093
		private sealed class SteamPlayerGroupSizeFieldLobby : SteamworksRichPresenceManager.SteamPlayerGroupSizeField
		{
			// Token: 0x06001A9F RID: 6815 RVA: 0x00071148 File Offset: 0x0006F348
			protected override void OnInstall()
			{
				base.OnInstall();
				SteamworksLobbyManager.onPlayerCountUpdated += this.UpdateGroupSize;
				this.UpdateGroupSize();
			}

			// Token: 0x06001AA0 RID: 6816 RVA: 0x00071167 File Offset: 0x0006F367
			protected override void OnUninstall()
			{
				SteamworksLobbyManager.onPlayerCountUpdated -= this.UpdateGroupSize;
				base.OnUninstall();
			}

			// Token: 0x06001AA1 RID: 6817 RVA: 0x00071180 File Offset: 0x0006F380
			private void UpdateGroupSize()
			{
				base.SetDirtyableValue<int>(ref this.groupSize, SteamworksLobbyManager.calculatedTotalPlayerCount);
			}
		}

		// Token: 0x02000446 RID: 1094
		private sealed class SteamPlayerGroupSizeFieldGame : SteamworksRichPresenceManager.SteamPlayerGroupSizeField
		{
			// Token: 0x06001AA3 RID: 6819 RVA: 0x0007119B File Offset: 0x0006F39B
			protected override void OnInstall()
			{
				base.OnInstall();
				NetworkUser.onNetworkUserDiscovered += this.OnNetworkUserDiscovered;
				NetworkUser.onNetworkUserLost += this.OnNetworkUserLost;
				this.UpdateGroupSize();
			}

			// Token: 0x06001AA4 RID: 6820 RVA: 0x000711CB File Offset: 0x0006F3CB
			protected override void OnUninstall()
			{
				NetworkUser.onNetworkUserDiscovered -= this.OnNetworkUserDiscovered;
				NetworkUser.onNetworkUserLost -= this.OnNetworkUserLost;
				base.OnUninstall();
			}

			// Token: 0x06001AA5 RID: 6821 RVA: 0x000711F5 File Offset: 0x0006F3F5
			private void UpdateGroupSize()
			{
				base.SetDirtyableValue<int>(ref this.groupSize, NetworkUser.readOnlyInstancesList.Count);
			}

			// Token: 0x06001AA6 RID: 6822 RVA: 0x0007120D File Offset: 0x0006F40D
			private void OnNetworkUserLost(NetworkUser networkuser)
			{
				this.UpdateGroupSize();
			}

			// Token: 0x06001AA7 RID: 6823 RVA: 0x0007120D File Offset: 0x0006F40D
			private void OnNetworkUserDiscovered(NetworkUser networkUser)
			{
				this.UpdateGroupSize();
			}
		}

		// Token: 0x02000447 RID: 1095
		private sealed class SteamDisplayField : SteamworksRichPresenceManager.BaseRichPresenceField
		{
			// Token: 0x1700030B RID: 779
			// (get) Token: 0x06001AA9 RID: 6825 RVA: 0x00071215 File Offset: 0x0006F415
			protected override string key
			{
				get
				{
					return "steam_display";
				}
			}

			// Token: 0x06001AAA RID: 6826 RVA: 0x0007121C File Offset: 0x0006F41C
			protected override string RebuildValue()
			{
				Scene activeScene = SceneManager.GetActiveScene();
				if (Run.instance)
				{
					if (GameOverController.instance)
					{
						return "#Display_GameOver";
					}
					return "#Display_InGame";
				}
				else
				{
					if (NetworkSession.instance)
					{
						return "#Display_PreGame";
					}
					if (SteamLobbyFinder.running)
					{
						return "#Display_Quickplay";
					}
					if (SteamworksLobbyManager.isInLobby)
					{
						return "#Display_InLobby";
					}
					if (activeScene.name == "logbook")
					{
						return "#Display_Logbook";
					}
					return "#Display_MainMenu";
				}
			}

			// Token: 0x06001AAB RID: 6827 RVA: 0x0007129D File Offset: 0x0006F49D
			protected override void OnInstall()
			{
				base.OnInstall();
				RoR2Application.onUpdate += base.SetDirty;
			}

			// Token: 0x06001AAC RID: 6828 RVA: 0x000712B6 File Offset: 0x0006F4B6
			protected override void OnUninstall()
			{
				RoR2Application.onUpdate -= base.SetDirty;
				base.OnUninstall();
			}
		}
	}
}
