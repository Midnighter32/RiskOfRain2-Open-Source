using System;
using Rewired;
using RoR2.UI;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000452 RID: 1106
	public class LocalUser
	{
		// Token: 0x17000241 RID: 577
		// (get) Token: 0x06001896 RID: 6294 RVA: 0x00076841 File Offset: 0x00074A41
		// (set) Token: 0x06001897 RID: 6295 RVA: 0x0007684C File Offset: 0x00074A4C
		public Player inputPlayer
		{
			get
			{
				return this._inputPlayer;
			}
			set
			{
				if (this._inputPlayer == value)
				{
					return;
				}
				if (this._inputPlayer != null)
				{
					this.OnRewiredPlayerLost(this._inputPlayer);
				}
				this._inputPlayer = value;
				this.eventSystem = MPEventSystemManager.FindEventSystem(this._inputPlayer);
				if (this._inputPlayer != null)
				{
					this.OnRewiredPlayerDiscovered(this._inputPlayer);
				}
			}
		}

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x06001898 RID: 6296 RVA: 0x000768A3 File Offset: 0x00074AA3
		// (set) Token: 0x06001899 RID: 6297 RVA: 0x000768AB File Offset: 0x00074AAB
		public MPEventSystem eventSystem { get; private set; }

		// Token: 0x17000243 RID: 579
		// (get) Token: 0x0600189A RID: 6298 RVA: 0x000768B4 File Offset: 0x00074AB4
		// (set) Token: 0x0600189B RID: 6299 RVA: 0x000768BC File Offset: 0x00074ABC
		public UserProfile userProfile
		{
			get
			{
				return this._userProfile;
			}
			set
			{
				this._userProfile = value;
				this.ApplyUserProfileBindingsToRewiredPlayer();
			}
		}

		// Token: 0x0600189C RID: 6300 RVA: 0x000768CB File Offset: 0x00074ACB
		static LocalUser()
		{
			ReInput.ControllerConnectedEvent += LocalUser.OnControllerConnected;
			ReInput.ControllerDisconnectedEvent += LocalUser.OnControllerDisconnected;
		}

		// Token: 0x0600189D RID: 6301 RVA: 0x000768F0 File Offset: 0x00074AF0
		private static void OnControllerConnected(ControllerStatusChangedEventArgs args)
		{
			foreach (LocalUser localUser in LocalUserManager.readOnlyLocalUsersList)
			{
				if (localUser.inputPlayer.controllers.ContainsController(args.controllerType, args.controllerId))
				{
					localUser.OnControllerDiscovered(ReInput.controllers.GetController(args.controllerType, args.controllerId));
				}
			}
		}

		// Token: 0x0600189E RID: 6302 RVA: 0x00076970 File Offset: 0x00074B70
		private static void OnControllerDisconnected(ControllerStatusChangedEventArgs args)
		{
			foreach (LocalUser localUser in LocalUserManager.readOnlyLocalUsersList)
			{
				if (localUser.inputPlayer.controllers.ContainsController(args.controllerType, args.controllerId))
				{
					localUser.OnControllerLost(ReInput.controllers.GetController(args.controllerType, args.controllerId));
				}
			}
		}

		// Token: 0x0600189F RID: 6303 RVA: 0x000769F0 File Offset: 0x00074BF0
		private void OnRewiredPlayerDiscovered(Player player)
		{
			foreach (Controller controller in player.controllers.Controllers)
			{
				this.OnControllerDiscovered(controller);
			}
		}

		// Token: 0x060018A0 RID: 6304 RVA: 0x00076A44 File Offset: 0x00074C44
		private void OnRewiredPlayerLost(Player player)
		{
			foreach (Controller controller in player.controllers.Controllers)
			{
				this.OnControllerLost(controller);
			}
		}

		// Token: 0x060018A1 RID: 6305 RVA: 0x00004507 File Offset: 0x00002707
		private void OnControllerDiscovered(Controller controller)
		{
		}

		// Token: 0x060018A2 RID: 6306 RVA: 0x00076A98 File Offset: 0x00074C98
		private void OnControllerLost(Controller controller)
		{
			this.inputPlayer.controllers.maps.ClearMapsForController(controller.type, controller.id, true);
		}

		// Token: 0x060018A3 RID: 6307 RVA: 0x00076ABC File Offset: 0x00074CBC
		private void ApplyUserProfileBindingsToRewiredPlayer()
		{
			if (this.inputPlayer == null)
			{
				return;
			}
			if (this.userProfile != null)
			{
				this.inputPlayer.controllers.maps.ClearAllMaps(false);
				foreach (Controller controller in this.inputPlayer.controllers.Controllers)
				{
					this.inputPlayer.controllers.maps.LoadMap(controller.type, controller.id, 2, 0);
					this.<ApplyUserProfileBindingsToRewiredPlayer>g__ApplyUserProfileBindingstoRewiredController|20_0(controller);
				}
				this.inputPlayer.controllers.maps.SetAllMapsEnabled(true);
			}
		}

		// Token: 0x17000244 RID: 580
		// (get) Token: 0x060018A4 RID: 6308 RVA: 0x00076B78 File Offset: 0x00074D78
		public bool isUIFocused
		{
			get
			{
				return this.eventSystem.currentSelectedGameObject;
			}
		}

		// Token: 0x17000245 RID: 581
		// (get) Token: 0x060018A5 RID: 6309 RVA: 0x00076B8A File Offset: 0x00074D8A
		// (set) Token: 0x060018A6 RID: 6310 RVA: 0x00076B92 File Offset: 0x00074D92
		public NetworkUser currentNetworkUser { get; private set; }

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x060018A7 RID: 6311 RVA: 0x00076B9B File Offset: 0x00074D9B
		// (set) Token: 0x060018A8 RID: 6312 RVA: 0x00076BA3 File Offset: 0x00074DA3
		public PlayerCharacterMasterController cachedMasterController { get; private set; }

		// Token: 0x17000247 RID: 583
		// (get) Token: 0x060018A9 RID: 6313 RVA: 0x00076BAC File Offset: 0x00074DAC
		// (set) Token: 0x060018AA RID: 6314 RVA: 0x00076BB4 File Offset: 0x00074DB4
		public GameObject cachedMasterObject { get; private set; }

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x060018AB RID: 6315 RVA: 0x00076BBD File Offset: 0x00074DBD
		// (set) Token: 0x060018AC RID: 6316 RVA: 0x00076BC5 File Offset: 0x00074DC5
		public CharacterBody cachedBody { get; private set; }

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x060018AD RID: 6317 RVA: 0x00076BCE File Offset: 0x00074DCE
		// (set) Token: 0x060018AE RID: 6318 RVA: 0x00076BD6 File Offset: 0x00074DD6
		public GameObject cachedBodyObject { get; private set; }

		// Token: 0x060018AF RID: 6319 RVA: 0x00076BE0 File Offset: 0x00074DE0
		public void RebuildControlChain()
		{
			UnityEngine.Object cachedMasterController = this.cachedMasterController;
			this.cachedMasterController = null;
			this.cachedMasterObject = null;
			UnityEngine.Object cachedBody = this.cachedBody;
			this.cachedBody = null;
			this.cachedBodyObject = null;
			if (this.currentNetworkUser)
			{
				this.cachedMasterObject = this.currentNetworkUser.masterObject;
				if (this.cachedMasterObject)
				{
					this.cachedMasterController = this.cachedMasterObject.GetComponent<PlayerCharacterMasterController>();
				}
				if (this.cachedMasterController)
				{
					this.cachedBody = this.cachedMasterController.master.GetBody();
					if (this.cachedBody)
					{
						this.cachedBodyObject = this.cachedBody.gameObject;
					}
				}
			}
			if (cachedBody != this.cachedBody)
			{
				Action action = this.onBodyChanged;
				if (action != null)
				{
					action();
				}
			}
			if (cachedMasterController != this.cachedMasterController)
			{
				Action action2 = this.onMasterChanged;
				if (action2 == null)
				{
					return;
				}
				action2();
			}
		}

		// Token: 0x14000034 RID: 52
		// (add) Token: 0x060018B0 RID: 6320 RVA: 0x00076CD0 File Offset: 0x00074ED0
		// (remove) Token: 0x060018B1 RID: 6321 RVA: 0x00076D08 File Offset: 0x00074F08
		public event Action onBodyChanged;

		// Token: 0x14000035 RID: 53
		// (add) Token: 0x060018B2 RID: 6322 RVA: 0x00076D40 File Offset: 0x00074F40
		// (remove) Token: 0x060018B3 RID: 6323 RVA: 0x00076D78 File Offset: 0x00074F78
		public event Action onMasterChanged;

		// Token: 0x14000036 RID: 54
		// (add) Token: 0x060018B4 RID: 6324 RVA: 0x00076DB0 File Offset: 0x00074FB0
		// (remove) Token: 0x060018B5 RID: 6325 RVA: 0x00076DE8 File Offset: 0x00074FE8
		public event Action<NetworkUser> onNetworkUserFound;

		// Token: 0x14000037 RID: 55
		// (add) Token: 0x060018B6 RID: 6326 RVA: 0x00076E20 File Offset: 0x00075020
		// (remove) Token: 0x060018B7 RID: 6327 RVA: 0x00076E58 File Offset: 0x00075058
		public event Action<NetworkUser> onNetworkUserLost;

		// Token: 0x060018B8 RID: 6328 RVA: 0x00076E8D File Offset: 0x0007508D
		public void LinkNetworkUser(NetworkUser newNetworkUser)
		{
			if (this.currentNetworkUser)
			{
				return;
			}
			this.currentNetworkUser = newNetworkUser;
			newNetworkUser.localUser = this;
			Action<NetworkUser> action = this.onNetworkUserFound;
			if (action == null)
			{
				return;
			}
			action(newNetworkUser);
		}

		// Token: 0x060018B9 RID: 6329 RVA: 0x00076EBC File Offset: 0x000750BC
		public void UnlinkNetworkUser()
		{
			Action<NetworkUser> action = this.onNetworkUserLost;
			if (action != null)
			{
				action(this.currentNetworkUser);
			}
			this.currentNetworkUser.localUser = null;
			this.currentNetworkUser = null;
			this.cachedMasterController = null;
			this.cachedMasterObject = null;
			this.cachedBody = null;
			this.cachedBodyObject = null;
		}

		// Token: 0x04001C3B RID: 7227
		private Player _inputPlayer;

		// Token: 0x04001C3D RID: 7229
		private UserProfile _userProfile;

		// Token: 0x04001C3E RID: 7230
		public int id;

		// Token: 0x04001C44 RID: 7236
		public CameraRigController cameraRigController;
	}
}
