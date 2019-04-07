using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI.MainMenu
{
	// Token: 0x0200066A RID: 1642
	public class MultiplayerMenuController : BaseMainMenuScreen
	{
		// Token: 0x17000325 RID: 805
		// (get) Token: 0x0600249F RID: 9375 RVA: 0x000AB812 File Offset: 0x000A9A12
		// (set) Token: 0x060024A0 RID: 9376 RVA: 0x000AB819 File Offset: 0x000A9A19
		public static MultiplayerMenuController instance { get; private set; }

		// Token: 0x17000326 RID: 806
		// (get) Token: 0x060024A1 RID: 9377 RVA: 0x000AB821 File Offset: 0x000A9A21
		public bool isInHostingState
		{
			get
			{
				return this.state == MultiplayerMenuController.State.Hosting;
			}
		}

		// Token: 0x060024A2 RID: 9378 RVA: 0x000AB82C File Offset: 0x000A9A2C
		public void OnEnable()
		{
			this.LerpAllUI(LerpUIRect.LerpState.Entering);
			this.state = MultiplayerMenuController.State.Idle;
			MultiplayerMenuController.instance = SingletonHelper.Assign<MultiplayerMenuController>(MultiplayerMenuController.instance, this);
			if (!SteamworksLobbyManager.isInLobby)
			{
				SteamworksLobbyManager.CreateLobby();
			}
			SteamworksLobbyManager.onLobbyLeave += this.OnLobbyLeave;
		}

		// Token: 0x060024A3 RID: 9379 RVA: 0x000AB869 File Offset: 0x000A9A69
		public void OnDisable()
		{
			SteamworksLobbyManager.onLobbyLeave -= this.OnLobbyLeave;
			if (!GameNetworkManager.singleton.isNetworkActive)
			{
				SteamworksLobbyManager.LeaveLobby();
			}
			MultiplayerMenuController.instance = SingletonHelper.Unassign<MultiplayerMenuController>(MultiplayerMenuController.instance, this);
		}

		// Token: 0x060024A4 RID: 9380 RVA: 0x000AB89D File Offset: 0x000A9A9D
		private void OnLobbyLeave(ulong lobbyId)
		{
			if (!SteamworksLobbyManager.isInLobby && !SteamworksLobbyManager.awaitingJoin)
			{
				SteamworksLobbyManager.CreateLobby();
			}
		}

		// Token: 0x060024A5 RID: 9381 RVA: 0x000AB8B2 File Offset: 0x000A9AB2
		public void Awake()
		{
			this.LerpAllUI(LerpUIRect.LerpState.Entering);
		}

		// Token: 0x060024A6 RID: 9382 RVA: 0x000AB8BC File Offset: 0x000A9ABC
		public void LerpAllUI(LerpUIRect.LerpState lerpState)
		{
			LerpUIRect[] array = this.uiToLerp;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].lerpState = lerpState;
			}
		}

		// Token: 0x060024A7 RID: 9383 RVA: 0x000AB8E7 File Offset: 0x000A9AE7
		public void Host()
		{
			if (this.state == MultiplayerMenuController.State.Idle)
			{
				Console.instance.SubmitCmd(null, "transition_command \"gamemode ClassicRun; host 1;\"", false);
				SteamworksLobbyManager.SetStartingIfOwner(true);
				this.state = MultiplayerMenuController.State.Hosting;
				this.titleStopwatch = 0f;
				this.LerpAllUI(LerpUIRect.LerpState.Leaving);
			}
		}

		// Token: 0x060024A8 RID: 9384 RVA: 0x000AB924 File Offset: 0x000A9B24
		private void Update()
		{
			this.titleStopwatch += Time.deltaTime;
			if (this.titleStopwatch >= 0.6f)
			{
				switch (this.state)
				{
				case MultiplayerMenuController.State.Hosting:
					this.state = MultiplayerMenuController.State.Waiting;
					this.titleStopwatch = -2f;
					break;
				case MultiplayerMenuController.State.Waiting:
					this.state = MultiplayerMenuController.State.Idle;
					this.titleStopwatch = 0.1f;
					this.LerpAllUI(LerpUIRect.LerpState.Entering);
					break;
				}
			}
			this.quickplayButton.interactable = this.ShouldEnableQuickplayButton();
			this.startPrivateGameButton.interactable = this.ShouldEnableStartPrivateGameButton();
			this.joinClipboardLobbyButtonController.mpButton.interactable = this.ShouldEnableJoinClipboardLobbyButton();
			this.inviteButton.interactable = this.ShouldEnableInviteButton();
		}

		// Token: 0x060024A9 RID: 9385 RVA: 0x000AB9E1 File Offset: 0x000A9BE1
		private bool ShouldEnableQuickplayButton()
		{
			return SteamworksLobbyManager.ownsLobby || SteamworksLobbyManager.newestLobbyData.quickplayQueued;
		}

		// Token: 0x060024AA RID: 9386 RVA: 0x000AB9F6 File Offset: 0x000A9BF6
		private bool ShouldEnableStartPrivateGameButton()
		{
			return !SteamworksLobbyManager.newestLobbyData.quickplayQueued && SteamworksLobbyManager.ownsLobby;
		}

		// Token: 0x060024AB RID: 9387 RVA: 0x000ABA0B File Offset: 0x000A9C0B
		private bool ShouldEnableJoinClipboardLobbyButton()
		{
			return !SteamworksLobbyManager.newestLobbyData.quickplayQueued && this.joinClipboardLobbyButtonController.validClipboardLobbyID;
		}

		// Token: 0x060024AC RID: 9388 RVA: 0x000ABA26 File Offset: 0x000A9C26
		private bool ShouldEnableInviteButton()
		{
			return !SteamworksLobbyManager.isFull && !SteamworksLobbyManager.newestLobbyData.quickplayQueued;
		}

		// Token: 0x060024AD RID: 9389 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override bool IsReadyToLeave()
		{
			return true;
		}

		// Token: 0x040027A5 RID: 10149
		private const float titleTransitionDuration = 0.5f;

		// Token: 0x040027A6 RID: 10150
		private const float titleTransitionBuffer = 0.1f;

		// Token: 0x040027A7 RID: 10151
		public Image fadeImage;

		// Token: 0x040027A8 RID: 10152
		public LerpUIRect[] uiToLerp;

		// Token: 0x040027A9 RID: 10153
		private float titleStopwatch;

		// Token: 0x040027AA RID: 10154
		private MultiplayerMenuController.State state;

		// Token: 0x040027AB RID: 10155
		public MPButton quickplayButton;

		// Token: 0x040027AC RID: 10156
		public MPButton startPrivateGameButton;

		// Token: 0x040027AD RID: 10157
		public SteamJoinClipboardLobby joinClipboardLobbyButtonController;

		// Token: 0x040027AE RID: 10158
		public MPButton inviteButton;

		// Token: 0x0200066B RID: 1643
		private enum State
		{
			// Token: 0x040027B0 RID: 10160
			Idle,
			// Token: 0x040027B1 RID: 10161
			Hosting,
			// Token: 0x040027B2 RID: 10162
			Waiting
		}
	}
}
