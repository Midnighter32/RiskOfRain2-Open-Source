using System;
using RoR2.ConVar;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.UI.MainMenu
{
	// Token: 0x02000666 RID: 1638
	public sealed class MainMenuController : MonoBehaviour
	{
		// Token: 0x06002484 RID: 9348 RVA: 0x000AB0CC File Offset: 0x000A92CC
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			GameNetworkManager.onStartClientGlobal += delegate(NetworkClient client)
			{
				if (!NetworkServer.active || !NetworkServer.dontListen)
				{
					MainMenuController.wasInMultiplayer = true;
				}
			};
		}

		// Token: 0x06002485 RID: 9349 RVA: 0x000AB0F4 File Offset: 0x000A92F4
		private void Start()
		{
			MainMenuController.wasInMultiplayer = false;
			this.titleMenuScreen.gameObject.SetActive(false);
			this.multiplayerMenuScreen.gameObject.SetActive(false);
			this.settingsMenuScreen.gameObject.SetActive(false);
			this.moreMenuScreen.gameObject.SetActive(false);
			this.desiredMenuScreen = (MainMenuController.wasInMultiplayer ? this.multiplayerMenuScreen : this.titleMenuScreen);
			if (!MainMenuController.eaWarningShown || !MainMenuController.IsMainUserSignedIn())
			{
				MainMenuController.eaWarningShown = true;
				if (!MainMenuController.eaMessageSkipConVar.value)
				{
					this.desiredMenuScreen = this.EAwarningProfileMenu;
				}
			}
			this.currentMenuScreen = this.desiredMenuScreen;
			this.currentMenuScreen.gameObject.SetActive(true);
			this.cameraTransform.position = this.currentMenuScreen.desiredCameraTransform.position;
			this.cameraTransform.rotation = this.currentMenuScreen.desiredCameraTransform.rotation;
			if (this.currentMenuScreen)
			{
				this.currentMenuScreen.OnEnter(this);
			}
		}

		// Token: 0x06002486 RID: 9350 RVA: 0x000AB1FE File Offset: 0x000A93FE
		private static bool IsMainUserSignedIn()
		{
			return LocalUserManager.FindLocalUser(0) != null;
		}

		// Token: 0x06002487 RID: 9351 RVA: 0x000AB209 File Offset: 0x000A9409
		private bool IsInLobby()
		{
			return SteamworksLobbyManager.isInLobby;
		}

		// Token: 0x06002488 RID: 9352 RVA: 0x000AB210 File Offset: 0x000A9410
		private void Update()
		{
			if (this.IsInLobby() && this.currentMenuScreen != this.multiplayerMenuScreen)
			{
				this.desiredMenuScreen = this.multiplayerMenuScreen;
			}
			if (!MainMenuController.IsMainUserSignedIn() && this.currentMenuScreen != this.EAwarningProfileMenu)
			{
				this.desiredMenuScreen = this.profileMenuScreen;
			}
			if (this.desiredMenuScreen != this.currentMenuScreen)
			{
				this.currentMenuScreen.shouldDisplay = false;
				if (this.currentMenuScreen.IsReadyToLeave())
				{
					this.currentMenuScreen.OnExit(this);
					this.currentMenuScreen.gameObject.SetActive(false);
					this.currentMenuScreen = this.desiredMenuScreen;
					this.camTransitionTimer = this.camTransitionDuration;
					this.currentMenuScreen.OnEnter(this);
					return;
				}
			}
			else
			{
				this.camTransitionTimer -= Time.deltaTime;
				this.cameraTransform.position = Vector3.SmoothDamp(this.cameraTransform.position, this.currentMenuScreen.desiredCameraTransform.position, ref this.camSmoothDampPositionVelocity, this.camTranslationSmoothDampTime);
				Vector3 eulerAngles = this.cameraTransform.eulerAngles;
				Vector3 eulerAngles2 = this.currentMenuScreen.desiredCameraTransform.eulerAngles;
				eulerAngles.x = Mathf.SmoothDampAngle(eulerAngles.x, eulerAngles2.x, ref this.camSmoothDampRotationVelocity.x, this.camRotationSmoothDampTime, float.PositiveInfinity, Time.unscaledDeltaTime);
				eulerAngles.y = Mathf.SmoothDampAngle(eulerAngles.y, eulerAngles2.y, ref this.camSmoothDampRotationVelocity.y, this.camRotationSmoothDampTime, float.PositiveInfinity, Time.unscaledDeltaTime);
				eulerAngles.z = Mathf.SmoothDampAngle(eulerAngles.z, eulerAngles2.z, ref this.camSmoothDampRotationVelocity.z, this.camRotationSmoothDampTime, float.PositiveInfinity, Time.unscaledDeltaTime);
				this.cameraTransform.eulerAngles = eulerAngles;
				if (this.camTransitionTimer <= 0f)
				{
					this.currentMenuScreen.gameObject.SetActive(true);
					this.currentMenuScreen.shouldDisplay = true;
				}
			}
		}

		// Token: 0x06002489 RID: 9353 RVA: 0x000AB410 File Offset: 0x000A9610
		public void SetDesiredMenuScreen(BaseMainMenuScreen newDesiredMenuScreen)
		{
			this.desiredMenuScreen = newDesiredMenuScreen;
		}

		// Token: 0x04002784 RID: 10116
		[NonSerialized]
		public BaseMainMenuScreen desiredMenuScreen;

		// Token: 0x04002785 RID: 10117
		public BaseMainMenuScreen profileMenuScreen;

		// Token: 0x04002786 RID: 10118
		public BaseMainMenuScreen EAwarningProfileMenu;

		// Token: 0x04002787 RID: 10119
		public BaseMainMenuScreen multiplayerMenuScreen;

		// Token: 0x04002788 RID: 10120
		public BaseMainMenuScreen titleMenuScreen;

		// Token: 0x04002789 RID: 10121
		public BaseMainMenuScreen settingsMenuScreen;

		// Token: 0x0400278A RID: 10122
		public BaseMainMenuScreen moreMenuScreen;

		// Token: 0x0400278B RID: 10123
		private BaseMainMenuScreen currentMenuScreen;

		// Token: 0x0400278C RID: 10124
		public Transform cameraTransform;

		// Token: 0x0400278D RID: 10125
		public float camRotationSmoothDampTime;

		// Token: 0x0400278E RID: 10126
		public float camTranslationSmoothDampTime;

		// Token: 0x0400278F RID: 10127
		private Vector3 camSmoothDampPositionVelocity;

		// Token: 0x04002790 RID: 10128
		private Vector3 camSmoothDampRotationVelocity;

		// Token: 0x04002791 RID: 10129
		public float camTransitionDuration;

		// Token: 0x04002792 RID: 10130
		private float camTransitionTimer;

		// Token: 0x04002793 RID: 10131
		private static bool wasInMultiplayer = false;

		// Token: 0x04002794 RID: 10132
		private static bool eaWarningShown = false;

		// Token: 0x04002795 RID: 10133
		private static BoolConVar eaMessageSkipConVar = new BoolConVar("ea_message_skip", ConVarFlags.None, "0", "Whether or not to skip the early access splash screen.");
	}
}
