using System;
using System.Collections.Generic;
using Facepunch.Steamworks;
using JetBrains.Annotations;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace RoR2
{
	// Token: 0x020004AB RID: 1195
	internal static class SteamworksRichPresenceManager
	{
		// Token: 0x06001AF2 RID: 6898 RVA: 0x0007EB23 File Offset: 0x0007CD23
		private static void SetKeyValue([NotNull] string key, [CanBeNull] string value)
		{
			Client.Instance.User.SetRichPresence(key, value);
		}

		// Token: 0x06001AF3 RID: 6899 RVA: 0x0007EB38 File Offset: 0x0007CD38
		private static void OnNetworkStart()
		{
			string text = null;
			CSteamID csteamID = CSteamID.nil;
			CSteamID csteamID2 = CSteamID.nil;
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			for (int i = 0; i < commandLineArgs.Length - 1; i++)
			{
				string a = commandLineArgs[i].ToLower();
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

		// Token: 0x06001AF4 RID: 6900 RVA: 0x0007EC78 File Offset: 0x0007CE78
		private static void OnLobbyChanged()
		{
			if (Client.Instance.Lobby.IsValid)
			{
				SteamworksRichPresenceManager.SetKeyValue("connect", "+steam_lobby_join " + Client.Instance.Lobby.CurrentLobby);
				return;
			}
			SteamworksRichPresenceManager.SetKeyValue("connect", null);
		}

		// Token: 0x06001AF5 RID: 6901 RVA: 0x0007ECCC File Offset: 0x0007CECC
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
					Console.instance.SubmitCmd(null, "connect_steamworks_p2p " + csteamID.value, false);
				}
				CSteamID csteamID2;
				if (array[0] == "+steam_lobby_join" && CSteamID.TryParse(array[1], out csteamID2))
				{
					if (!SteamworksLobbyManager.ownsLobby)
					{
						SteamworksLobbyManager.LeaveLobby();
					}
					Console.instance.SubmitCmd(null, "steam_lobby_join " + csteamID2.value, false);
				}
			}
		}

		// Token: 0x06001AF6 RID: 6902 RVA: 0x0007EDA1 File Offset: 0x0007CFA1
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
			Console.instance.SubmitCmd(null, string.Format("connect \"{0}\"", address), false);
		}

		// Token: 0x06001AF7 RID: 6903 RVA: 0x0007EDDC File Offset: 0x0007CFDC
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

		// Token: 0x06001AF8 RID: 6904 RVA: 0x0007EE95 File Offset: 0x0007D095
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			RoR2Application.onLoad = (Action)Delegate.Combine(RoR2Application.onLoad, new Action(SteamworksRichPresenceManager.SetupCallbacks));
		}

		// Token: 0x04001DBD RID: 7613
		private const string rpConnect = "connect";

		// Token: 0x04001DBE RID: 7614
		private const string rpStatus = "status";

		// Token: 0x04001DBF RID: 7615
		private const string rpSteamDisplay = "steam_display";

		// Token: 0x04001DC0 RID: 7616
		private const string rpSteamPlayerGroup = "steam_player_group";

		// Token: 0x04001DC1 RID: 7617
		private const string rpSteamPlayerGroupSize = "steam_player_group_size";

		// Token: 0x04001DC2 RID: 7618
		private const string rpDifficulty = "difficulty";

		// Token: 0x04001DC3 RID: 7619
		private const string rpGameMode = "gamemode";

		// Token: 0x04001DC4 RID: 7620
		private const string rpParticipationType = "participation_type";

		// Token: 0x04001DC5 RID: 7621
		private const string rpMinutes = "minutes";

		// Token: 0x020004AC RID: 1196
		private abstract class BaseRichPresenceField
		{
			// Token: 0x06001AF9 RID: 6905 RVA: 0x0007EEB7 File Offset: 0x0007D0B7
			public static void ProcessDirtyFields()
			{
				while (SteamworksRichPresenceManager.BaseRichPresenceField.dirtyFields.Count > 0)
				{
					SteamworksRichPresenceManager.BaseRichPresenceField.dirtyFields.Dequeue().UpdateIfNecessary();
				}
			}

			// Token: 0x1700027D RID: 637
			// (get) Token: 0x06001AFA RID: 6906
			protected abstract string key { get; }

			// Token: 0x06001AFB RID: 6907
			[CanBeNull]
			protected abstract string RebuildValue();

			// Token: 0x06001AFC RID: 6908 RVA: 0x00004507 File Offset: 0x00002707
			protected virtual void OnChanged()
			{
			}

			// Token: 0x06001AFD RID: 6909 RVA: 0x0007EED7 File Offset: 0x0007D0D7
			public void SetDirty()
			{
				if (!this.isDirty)
				{
					this.isDirty = true;
					SteamworksRichPresenceManager.BaseRichPresenceField.dirtyFields.Enqueue(this);
				}
			}

			// Token: 0x06001AFE RID: 6910 RVA: 0x0007EEF4 File Offset: 0x0007D0F4
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

			// Token: 0x06001AFF RID: 6911 RVA: 0x00004507 File Offset: 0x00002707
			protected virtual void OnInstall()
			{
			}

			// Token: 0x06001B00 RID: 6912 RVA: 0x00004507 File Offset: 0x00002707
			protected virtual void OnUninstall()
			{
			}

			// Token: 0x06001B01 RID: 6913 RVA: 0x0007EF44 File Offset: 0x0007D144
			public void Install()
			{
				if (!this.installed)
				{
					this.OnInstall();
					this.SetDirty();
					this.installed = true;
				}
			}

			// Token: 0x06001B02 RID: 6914 RVA: 0x0007EF61 File Offset: 0x0007D161
			public void Uninstall()
			{
				if (this.installed)
				{
					this.OnUninstall();
					this.installed = false;
					SteamworksRichPresenceManager.SetKeyValue(this.key, null);
				}
			}

			// Token: 0x06001B03 RID: 6915 RVA: 0x0007EF84 File Offset: 0x0007D184
			protected void SetDirtyableValue<T>(ref T field, T value) where T : struct, IEquatable<T>
			{
				if (!field.Equals(value))
				{
					field = value;
					this.SetDirty();
				}
			}

			// Token: 0x06001B04 RID: 6916 RVA: 0x0007EFA2 File Offset: 0x0007D1A2
			protected void SetDirtyableReference<T>(ref T field, T value) where T : class
			{
				if (field != value)
				{
					field = value;
					this.SetDirty();
				}
			}

			// Token: 0x04001DC6 RID: 7622
			private static readonly Queue<SteamworksRichPresenceManager.BaseRichPresenceField> dirtyFields = new Queue<SteamworksRichPresenceManager.BaseRichPresenceField>();

			// Token: 0x04001DC7 RID: 7623
			private bool isDirty;

			// Token: 0x04001DC8 RID: 7624
			[CanBeNull]
			private string currentValue;

			// Token: 0x04001DC9 RID: 7625
			private bool installed;
		}

		// Token: 0x020004AD RID: 1197
		private sealed class DifficultyField : SteamworksRichPresenceManager.BaseRichPresenceField
		{
			// Token: 0x1700027E RID: 638
			// (get) Token: 0x06001B07 RID: 6919 RVA: 0x0007EFD0 File Offset: 0x0007D1D0
			protected override string key
			{
				get
				{
					return "difficulty";
				}
			}

			// Token: 0x06001B08 RID: 6920 RVA: 0x0007EFD8 File Offset: 0x0007D1D8
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

			// Token: 0x06001B09 RID: 6921 RVA: 0x0007F025 File Offset: 0x0007D225
			private void SetDirty(Run run)
			{
				base.SetDirty();
			}

			// Token: 0x06001B0A RID: 6922 RVA: 0x0007F02D File Offset: 0x0007D22D
			protected override void OnInstall()
			{
				base.OnInstall();
				Run.onRunStartGlobal += this.SetDirty;
				Run.onRunDestroyGlobal += this.SetDirty;
			}

			// Token: 0x06001B0B RID: 6923 RVA: 0x0007F057 File Offset: 0x0007D257
			protected override void OnUninstall()
			{
				Run.onRunStartGlobal -= this.SetDirty;
				Run.onRunDestroyGlobal -= this.SetDirty;
				base.OnUninstall();
			}
		}

		// Token: 0x020004AE RID: 1198
		private sealed class GameModeField : SteamworksRichPresenceManager.BaseRichPresenceField
		{
			// Token: 0x1700027F RID: 639
			// (get) Token: 0x06001B0D RID: 6925 RVA: 0x0007F089 File Offset: 0x0007D289
			protected override string key
			{
				get
				{
					return "gamemode";
				}
			}

			// Token: 0x06001B0E RID: 6926 RVA: 0x0007F090 File Offset: 0x0007D290
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

			// Token: 0x06001B0F RID: 6927 RVA: 0x0007F025 File Offset: 0x0007D225
			private void SetDirty(Run run)
			{
				base.SetDirty();
			}

			// Token: 0x06001B10 RID: 6928 RVA: 0x0007F0BA File Offset: 0x0007D2BA
			protected override void OnInstall()
			{
				base.OnInstall();
				Run.onRunStartGlobal += this.SetDirty;
				Run.onRunDestroyGlobal += this.SetDirty;
			}

			// Token: 0x06001B11 RID: 6929 RVA: 0x0007F0E4 File Offset: 0x0007D2E4
			protected override void OnUninstall()
			{
				Run.onRunStartGlobal -= this.SetDirty;
				Run.onRunDestroyGlobal -= this.SetDirty;
				base.OnUninstall();
			}
		}

		// Token: 0x020004AF RID: 1199
		private sealed class ParticipationField : SteamworksRichPresenceManager.BaseRichPresenceField
		{
			// Token: 0x17000280 RID: 640
			// (get) Token: 0x06001B13 RID: 6931 RVA: 0x0007F10E File Offset: 0x0007D30E
			protected override string key
			{
				get
				{
					return "participation_type";
				}
			}

			// Token: 0x06001B14 RID: 6932 RVA: 0x0007F115 File Offset: 0x0007D315
			private void SetParticipationType(SteamworksRichPresenceManager.ParticipationField.ParticipationType newParticipationType)
			{
				if (this.participationType != newParticipationType)
				{
					this.participationType = newParticipationType;
					base.SetDirty();
				}
			}

			// Token: 0x06001B15 RID: 6933 RVA: 0x0007F130 File Offset: 0x0007D330
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

			// Token: 0x06001B16 RID: 6934 RVA: 0x0007F170 File Offset: 0x0007D370
			protected override void OnInstall()
			{
				base.OnInstall();
				LocalUserManager.onUserSignIn += this.OnLocalUserDiscovered;
				LocalUserManager.onUserSignOut += this.OnLocalUserLost;
				Run.onRunStartGlobal += this.OnRunStart;
				Run.onRunDestroyGlobal += this.OnRunDestroy;
			}

			// Token: 0x06001B17 RID: 6935 RVA: 0x0007F1C8 File Offset: 0x0007D3C8
			protected override void OnUninstall()
			{
				LocalUserManager.onUserSignIn -= this.OnLocalUserDiscovered;
				LocalUserManager.onUserSignOut -= this.OnLocalUserLost;
				Run.onRunStartGlobal -= this.OnRunStart;
				Run.onRunDestroyGlobal -= this.OnRunDestroy;
				this.SetCurrentMaster(null);
			}

			// Token: 0x06001B18 RID: 6936 RVA: 0x0007F220 File Offset: 0x0007D420
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

			// Token: 0x06001B19 RID: 6937 RVA: 0x0007F272 File Offset: 0x0007D472
			private void OnLocalUserDiscovered(LocalUser localUser)
			{
				if (this.trackedUser == null)
				{
					this.SetTrackedUser(localUser);
				}
			}

			// Token: 0x06001B1A RID: 6938 RVA: 0x0007F283 File Offset: 0x0007D483
			private void OnLocalUserLost(LocalUser localUser)
			{
				if (this.trackedUser == localUser)
				{
					this.SetTrackedUser(null);
				}
			}

			// Token: 0x06001B1B RID: 6939 RVA: 0x0007F295 File Offset: 0x0007D495
			private void OnRunStart(Run run)
			{
				if (this.trackedUser != null && !this.trackedUser.cachedMasterObject)
				{
					this.SetParticipationType(SteamworksRichPresenceManager.ParticipationField.ParticipationType.Spectator);
				}
			}

			// Token: 0x06001B1C RID: 6940 RVA: 0x0007F2B8 File Offset: 0x0007D4B8
			private void OnRunDestroy(Run run)
			{
				if (this.trackedUser != null)
				{
					this.SetParticipationType(SteamworksRichPresenceManager.ParticipationField.ParticipationType.None);
				}
			}

			// Token: 0x06001B1D RID: 6941 RVA: 0x0007F2CC File Offset: 0x0007D4CC
			private void OnMasterChanged()
			{
				PlayerCharacterMasterController cachedMasterController = this.trackedUser.cachedMasterController;
				this.SetCurrentMaster(cachedMasterController ? cachedMasterController.master : null);
			}

			// Token: 0x06001B1E RID: 6942 RVA: 0x0007F2FC File Offset: 0x0007D4FC
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

			// Token: 0x06001B1F RID: 6943 RVA: 0x0007F386 File Offset: 0x0007D586
			private void OnBodyDeath()
			{
				this.SetParticipationType(SteamworksRichPresenceManager.ParticipationField.ParticipationType.Dead);
			}

			// Token: 0x06001B20 RID: 6944 RVA: 0x0007F38F File Offset: 0x0007D58F
			private void OnBodyStart(CharacterBody body)
			{
				this.SetParticipationType(SteamworksRichPresenceManager.ParticipationField.ParticipationType.Alive);
			}

			// Token: 0x04001DCA RID: 7626
			private SteamworksRichPresenceManager.ParticipationField.ParticipationType participationType;

			// Token: 0x04001DCB RID: 7627
			private LocalUser trackedUser;

			// Token: 0x04001DCC RID: 7628
			private CharacterMaster currentMaster;

			// Token: 0x020004B0 RID: 1200
			private enum ParticipationType
			{
				// Token: 0x04001DCE RID: 7630
				None,
				// Token: 0x04001DCF RID: 7631
				Alive,
				// Token: 0x04001DD0 RID: 7632
				Dead,
				// Token: 0x04001DD1 RID: 7633
				Spectator
			}
		}

		// Token: 0x020004B1 RID: 1201
		private sealed class MinutesField : SteamworksRichPresenceManager.BaseRichPresenceField
		{
			// Token: 0x17000281 RID: 641
			// (get) Token: 0x06001B22 RID: 6946 RVA: 0x0007F398 File Offset: 0x0007D598
			protected override string key
			{
				get
				{
					return "minutes";
				}
			}

			// Token: 0x06001B23 RID: 6947 RVA: 0x0007F39F File Offset: 0x0007D59F
			protected override string RebuildValue()
			{
				return TextSerialization.ToStringInvariant(this.minutes);
			}

			// Token: 0x06001B24 RID: 6948 RVA: 0x0007F3AC File Offset: 0x0007D5AC
			private void FixedUpdate()
			{
				uint value = 0u;
				if (Run.instance)
				{
					value = (uint)Mathf.FloorToInt(Run.instance.fixedTime / 60f);
				}
				base.SetDirtyableValue<uint>(ref this.minutes, value);
			}

			// Token: 0x06001B25 RID: 6949 RVA: 0x0007F3EA File Offset: 0x0007D5EA
			protected override void OnInstall()
			{
				base.OnInstall();
				RoR2Application.onFixedUpdate += this.FixedUpdate;
			}

			// Token: 0x06001B26 RID: 6950 RVA: 0x0007F403 File Offset: 0x0007D603
			protected override void OnUninstall()
			{
				RoR2Application.onFixedUpdate -= this.FixedUpdate;
				base.OnUninstall();
			}

			// Token: 0x04001DD2 RID: 7634
			private uint minutes;
		}

		// Token: 0x020004B2 RID: 1202
		private sealed class SteamPlayerGroupField : SteamworksRichPresenceManager.BaseRichPresenceField
		{
			// Token: 0x17000282 RID: 642
			// (get) Token: 0x06001B28 RID: 6952 RVA: 0x0007F41C File Offset: 0x0007D61C
			protected override string key
			{
				get
				{
					return "steam_player_group";
				}
			}

			// Token: 0x06001B29 RID: 6953 RVA: 0x0007F423 File Offset: 0x0007D623
			private void SetLobbyId(CSteamID newLobbyId)
			{
				if (this.lobbyId != newLobbyId)
				{
					this.lobbyId = newLobbyId;
					this.UpdateGroupID();
				}
			}

			// Token: 0x06001B2A RID: 6954 RVA: 0x0007F440 File Offset: 0x0007D640
			private void SetHostId(CSteamID newHostId)
			{
				if (this.hostId != newHostId)
				{
					this.hostId = newHostId;
					this.UpdateGroupID();
				}
			}

			// Token: 0x06001B2B RID: 6955 RVA: 0x0007F45D File Offset: 0x0007D65D
			private void SetGroupId(CSteamID newGroupId)
			{
				if (this.groupId != newGroupId)
				{
					this.groupId = newGroupId;
					base.SetDirty();
				}
			}

			// Token: 0x06001B2C RID: 6956 RVA: 0x0007F47C File Offset: 0x0007D67C
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

			// Token: 0x06001B2D RID: 6957 RVA: 0x0007F51C File Offset: 0x0007D71C
			protected override void OnInstall()
			{
				base.OnInstall();
				GameNetworkManager.onClientConnectGlobal += this.OnClientConnectGlobal;
				GameNetworkManager.onClientDisconnectGlobal += this.OnClientDisconnectGlobal;
				GameNetworkManager.onStartServerGlobal += this.OnStartServerGlobal;
				GameNetworkManager.onStopServerGlobal += this.OnStopServerGlobal;
				SteamworksLobbyManager.onLobbyChanged += this.OnLobbyChanged;
			}

			// Token: 0x06001B2E RID: 6958 RVA: 0x0007F584 File Offset: 0x0007D784
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

			// Token: 0x06001B2F RID: 6959 RVA: 0x0007F604 File Offset: 0x0007D804
			protected override string RebuildValue()
			{
				if (this.groupId == CSteamID.nil)
				{
					return null;
				}
				return TextSerialization.ToStringInvariant(this.groupId.value);
			}

			// Token: 0x06001B30 RID: 6960 RVA: 0x0007F62C File Offset: 0x0007D82C
			private void OnClientConnectGlobal(NetworkConnection conn)
			{
				SteamNetworkConnection steamNetworkConnection;
				if ((steamNetworkConnection = (conn as SteamNetworkConnection)) != null)
				{
					this.hostId = steamNetworkConnection.steamId;
				}
			}

			// Token: 0x06001B31 RID: 6961 RVA: 0x0007F64F File Offset: 0x0007D84F
			private void OnClientDisconnectGlobal(NetworkConnection conn)
			{
				this.hostId = CSteamID.nil;
			}

			// Token: 0x06001B32 RID: 6962 RVA: 0x0007F65C File Offset: 0x0007D85C
			private void OnStartServerGlobal()
			{
				this.hostId = new CSteamID(Client.Instance.SteamId);
			}

			// Token: 0x06001B33 RID: 6963 RVA: 0x0007F64F File Offset: 0x0007D84F
			private void OnStopServerGlobal()
			{
				this.hostId = CSteamID.nil;
			}

			// Token: 0x06001B34 RID: 6964 RVA: 0x0007F673 File Offset: 0x0007D873
			private void OnLobbyChanged()
			{
				this.SetLobbyId(new CSteamID(Client.Instance.Lobby.CurrentLobby));
			}

			// Token: 0x04001DD3 RID: 7635
			private CSteamID lobbyId = CSteamID.nil;

			// Token: 0x04001DD4 RID: 7636
			private CSteamID hostId = CSteamID.nil;

			// Token: 0x04001DD5 RID: 7637
			private CSteamID groupId = CSteamID.nil;

			// Token: 0x04001DD6 RID: 7638
			private SteamworksRichPresenceManager.SteamPlayerGroupSizeField groupSizeField;
		}

		// Token: 0x020004B3 RID: 1203
		private abstract class SteamPlayerGroupSizeField : SteamworksRichPresenceManager.BaseRichPresenceField
		{
			// Token: 0x17000283 RID: 643
			// (get) Token: 0x06001B36 RID: 6966 RVA: 0x0007F6B8 File Offset: 0x0007D8B8
			protected override string key
			{
				get
				{
					return "steam_player_group_size";
				}
			}

			// Token: 0x06001B37 RID: 6967 RVA: 0x0007F6BF File Offset: 0x0007D8BF
			protected override string RebuildValue()
			{
				return TextSerialization.ToStringInvariant(this.groupSize);
			}

			// Token: 0x04001DD7 RID: 7639
			protected int groupSize;
		}

		// Token: 0x020004B4 RID: 1204
		private sealed class SteamPlayerGroupSizeFieldLobby : SteamworksRichPresenceManager.SteamPlayerGroupSizeField
		{
			// Token: 0x06001B39 RID: 6969 RVA: 0x0007F6CC File Offset: 0x0007D8CC
			protected override void OnInstall()
			{
				base.OnInstall();
				SteamworksLobbyManager.onPlayerCountUpdated += this.UpdateGroupSize;
				this.UpdateGroupSize();
			}

			// Token: 0x06001B3A RID: 6970 RVA: 0x0007F6EB File Offset: 0x0007D8EB
			protected override void OnUninstall()
			{
				SteamworksLobbyManager.onPlayerCountUpdated -= this.UpdateGroupSize;
				base.OnUninstall();
			}

			// Token: 0x06001B3B RID: 6971 RVA: 0x0007F704 File Offset: 0x0007D904
			private void UpdateGroupSize()
			{
				base.SetDirtyableValue<int>(ref this.groupSize, SteamworksLobbyManager.calculatedTotalPlayerCount);
			}
		}

		// Token: 0x020004B5 RID: 1205
		private sealed class SteamPlayerGroupSizeFieldGame : SteamworksRichPresenceManager.SteamPlayerGroupSizeField
		{
			// Token: 0x06001B3D RID: 6973 RVA: 0x0007F71F File Offset: 0x0007D91F
			protected override void OnInstall()
			{
				base.OnInstall();
				NetworkUser.onNetworkUserDiscovered += this.OnNetworkUserDiscovered;
				NetworkUser.onNetworkUserLost += this.OnNetworkUserLost;
				this.UpdateGroupSize();
			}

			// Token: 0x06001B3E RID: 6974 RVA: 0x0007F74F File Offset: 0x0007D94F
			protected override void OnUninstall()
			{
				NetworkUser.onNetworkUserDiscovered -= this.OnNetworkUserDiscovered;
				NetworkUser.onNetworkUserLost -= this.OnNetworkUserLost;
				base.OnUninstall();
			}

			// Token: 0x06001B3F RID: 6975 RVA: 0x0007F779 File Offset: 0x0007D979
			private void UpdateGroupSize()
			{
				base.SetDirtyableValue<int>(ref this.groupSize, NetworkUser.readOnlyInstancesList.Count);
			}

			// Token: 0x06001B40 RID: 6976 RVA: 0x0007F791 File Offset: 0x0007D991
			private void OnNetworkUserLost(NetworkUser networkuser)
			{
				this.UpdateGroupSize();
			}

			// Token: 0x06001B41 RID: 6977 RVA: 0x0007F791 File Offset: 0x0007D991
			private void OnNetworkUserDiscovered(NetworkUser networkUser)
			{
				this.UpdateGroupSize();
			}
		}

		// Token: 0x020004B6 RID: 1206
		private sealed class SteamDisplayField : SteamworksRichPresenceManager.BaseRichPresenceField
		{
			// Token: 0x17000284 RID: 644
			// (get) Token: 0x06001B43 RID: 6979 RVA: 0x0007F799 File Offset: 0x0007D999
			protected override string key
			{
				get
				{
					return "steam_display";
				}
			}

			// Token: 0x06001B44 RID: 6980 RVA: 0x0007F7A0 File Offset: 0x0007D9A0
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

			// Token: 0x06001B45 RID: 6981 RVA: 0x0007F821 File Offset: 0x0007DA21
			protected override void OnInstall()
			{
				base.OnInstall();
				RoR2Application.onUpdate += base.SetDirty;
			}

			// Token: 0x06001B46 RID: 6982 RVA: 0x0007F83A File Offset: 0x0007DA3A
			protected override void OnUninstall()
			{
				RoR2Application.onUpdate -= base.SetDirty;
				base.OnUninstall();
			}
		}
	}
}
