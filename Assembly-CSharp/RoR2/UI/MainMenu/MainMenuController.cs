using System;
using RoR2.ConVar;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.UI.MainMenu
{
	// Token: 0x0200065B RID: 1627
	public sealed class MainMenuController : MonoBehaviour
	{
		// Token: 0x06002628 RID: 9768 RVA: 0x000A5C08 File Offset: 0x000A3E08
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

		// Token: 0x06002629 RID: 9769 RVA: 0x000A5C30 File Offset: 0x000A3E30
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

		// Token: 0x0600262A RID: 9770 RVA: 0x000A5D3A File Offset: 0x000A3F3A
		private static bool IsMainUserSignedIn()
		{
			return LocalUserManager.FindLocalUser(0) != null;
		}

		// Token: 0x0600262B RID: 9771 RVA: 0x000A5D45 File Offset: 0x000A3F45
		private bool IsInLobby()
		{
			return SteamworksLobbyManager.isInLobby;
		}

		// Token: 0x0600262C RID: 9772 RVA: 0x000A5D4C File Offset: 0x000A3F4C
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

		// Token: 0x0600262D RID: 9773 RVA: 0x000A5F4C File Offset: 0x000A414C
		public void SetDesiredMenuScreen(BaseMainMenuScreen newDesiredMenuScreen)
		{
			this.desiredMenuScreen = newDesiredMenuScreen;
		}

		// Token: 0x040023EB RID: 9195
		[NonSerialized]
		public BaseMainMenuScreen desiredMenuScreen;

		// Token: 0x040023EC RID: 9196
		public BaseMainMenuScreen profileMenuScreen;

		// Token: 0x040023ED RID: 9197
		public BaseMainMenuScreen EAwarningProfileMenu;

		// Token: 0x040023EE RID: 9198
		public BaseMainMenuScreen multiplayerMenuScreen;

		// Token: 0x040023EF RID: 9199
		public BaseMainMenuScreen titleMenuScreen;

		// Token: 0x040023F0 RID: 9200
		public BaseMainMenuScreen settingsMenuScreen;

		// Token: 0x040023F1 RID: 9201
		public BaseMainMenuScreen moreMenuScreen;

		// Token: 0x040023F2 RID: 9202
		private BaseMainMenuScreen currentMenuScreen;

		// Token: 0x040023F3 RID: 9203
		public Transform cameraTransform;

		// Token: 0x040023F4 RID: 9204
		public float camRotationSmoothDampTime;

		// Token: 0x040023F5 RID: 9205
		public float camTranslationSmoothDampTime;

		// Token: 0x040023F6 RID: 9206
		private Vector3 camSmoothDampPositionVelocity;

		// Token: 0x040023F7 RID: 9207
		private Vector3 camSmoothDampRotationVelocity;

		// Token: 0x040023F8 RID: 9208
		public float camTransitionDuration;

		// Token: 0x040023F9 RID: 9209
		private float camTransitionTimer;

		// Token: 0x040023FA RID: 9210
		private static bool wasInMultiplayer = false;

		// Token: 0x040023FB RID: 9211
		private static bool eaWarningShown = false;

		// Token: 0x040023FC RID: 9212
		private static BoolConVar eaMessageSkipConVar = new BoolConVar("ea_message_skip", ConVarFlags.None, "0", "Whether or not to skip the early access splash screen.");
	}
}
