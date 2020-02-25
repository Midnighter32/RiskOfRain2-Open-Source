using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI.MainMenu
{
	// Token: 0x0200065F RID: 1631
	public class MultiplayerMenuController : BaseMainMenuScreen
	{
		// Token: 0x170003E5 RID: 997
		// (get) Token: 0x06002643 RID: 9795 RVA: 0x000A634E File Offset: 0x000A454E
		// (set) Token: 0x06002644 RID: 9796 RVA: 0x000A6355 File Offset: 0x000A4555
		public static MultiplayerMenuController instance { get; private set; }

		// Token: 0x170003E6 RID: 998
		// (get) Token: 0x06002645 RID: 9797 RVA: 0x000A635D File Offset: 0x000A455D
		public bool isInHostingState
		{
			get
			{
				return this.state == MultiplayerMenuController.State.Hosting;
			}
		}

		// Token: 0x06002646 RID: 9798 RVA: 0x000A6368 File Offset: 0x000A4568
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

		// Token: 0x06002647 RID: 9799 RVA: 0x000A63A5 File Offset: 0x000A45A5
		public void OnDisable()
		{
			SteamworksLobbyManager.onLobbyLeave -= this.OnLobbyLeave;
			if (!GameNetworkManager.singleton.isNetworkActive)
			{
				SteamworksLobbyManager.LeaveLobby();
			}
			MultiplayerMenuController.instance = SingletonHelper.Unassign<MultiplayerMenuController>(MultiplayerMenuController.instance, this);
		}

		// Token: 0x06002648 RID: 9800 RVA: 0x000A63D9 File Offset: 0x000A45D9
		private void OnLobbyLeave(ulong lobbyId)
		{
			if (!SteamworksLobbyManager.isInLobby && !SteamworksLobbyManager.awaitingJoin)
			{
				SteamworksLobbyManager.CreateLobby();
			}
		}

		// Token: 0x06002649 RID: 9801 RVA: 0x000A63EE File Offset: 0x000A45EE
		public void Awake()
		{
			this.LerpAllUI(LerpUIRect.LerpState.Entering);
		}

		// Token: 0x0600264A RID: 9802 RVA: 0x000A63F8 File Offset: 0x000A45F8
		public void LerpAllUI(LerpUIRect.LerpState lerpState)
		{
			LerpUIRect[] array = this.uiToLerp;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].lerpState = lerpState;
			}
		}

		// Token: 0x0600264B RID: 9803 RVA: 0x000A6423 File Offset: 0x000A4623
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

		// Token: 0x0600264C RID: 9804 RVA: 0x000A6460 File Offset: 0x000A4660
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

		// Token: 0x0600264D RID: 9805 RVA: 0x000A651D File Offset: 0x000A471D
		private bool ShouldEnableQuickplayButton()
		{
			return SteamworksLobbyManager.ownsLobby || SteamworksLobbyManager.newestLobbyData.quickplayQueued;
		}

		// Token: 0x0600264E RID: 9806 RVA: 0x000A6532 File Offset: 0x000A4732
		private bool ShouldEnableStartPrivateGameButton()
		{
			return !SteamworksLobbyManager.newestLobbyData.quickplayQueued && SteamworksLobbyManager.ownsLobby;
		}

		// Token: 0x0600264F RID: 9807 RVA: 0x000A6547 File Offset: 0x000A4747
		private bool ShouldEnableJoinClipboardLobbyButton()
		{
			return !SteamworksLobbyManager.newestLobbyData.quickplayQueued && this.joinClipboardLobbyButtonController.validClipboardLobbyID;
		}

		// Token: 0x06002650 RID: 9808 RVA: 0x000A6562 File Offset: 0x000A4762
		private bool ShouldEnableInviteButton()
		{
			return !SteamworksLobbyManager.isFull && !SteamworksLobbyManager.newestLobbyData.quickplayQueued;
		}

		// Token: 0x06002651 RID: 9809 RVA: 0x0000B933 File Offset: 0x00009B33
		public override bool IsReadyToLeave()
		{
			return true;
		}

		// Token: 0x0400240C RID: 9228
		private const float titleTransitionDuration = 0.5f;

		// Token: 0x0400240D RID: 9229
		private const float titleTransitionBuffer = 0.1f;

		// Token: 0x0400240E RID: 9230
		public Image fadeImage;

		// Token: 0x0400240F RID: 9231
		public LerpUIRect[] uiToLerp;

		// Token: 0x04002410 RID: 9232
		private float titleStopwatch;

		// Token: 0x04002411 RID: 9233
		private MultiplayerMenuController.State state;

		// Token: 0x04002412 RID: 9234
		public MPButton quickplayButton;

		// Token: 0x04002413 RID: 9235
		public MPButton startPrivateGameButton;

		// Token: 0x04002414 RID: 9236
		public SteamJoinClipboardLobby joinClipboardLobbyButtonController;

		// Token: 0x04002415 RID: 9237
		public MPButton inviteButton;

		// Token: 0x02000660 RID: 1632
		private enum State
		{
			// Token: 0x04002417 RID: 9239
			Idle,
			// Token: 0x04002418 RID: 9240
			Hosting,
			// Token: 0x04002419 RID: 9241
			Waiting
		}
	}
}
