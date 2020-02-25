using System;
using System.Runtime.CompilerServices;
using Rewired;
using RoR2.UI;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003C5 RID: 965
	public class LocalUser
	{
		// Token: 0x170002B5 RID: 693
		// (get) Token: 0x0600175C RID: 5980 RVA: 0x00065D85 File Offset: 0x00063F85
		// (set) Token: 0x0600175D RID: 5981 RVA: 0x00065D90 File Offset: 0x00063F90
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

		// Token: 0x170002B6 RID: 694
		// (get) Token: 0x0600175E RID: 5982 RVA: 0x00065DE7 File Offset: 0x00063FE7
		// (set) Token: 0x0600175F RID: 5983 RVA: 0x00065DEF File Offset: 0x00063FEF
		public MPEventSystem eventSystem { get; private set; }

		// Token: 0x170002B7 RID: 695
		// (get) Token: 0x06001760 RID: 5984 RVA: 0x00065DF8 File Offset: 0x00063FF8
		// (set) Token: 0x06001761 RID: 5985 RVA: 0x00065E00 File Offset: 0x00064000
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

		// Token: 0x06001762 RID: 5986 RVA: 0x00065E0F File Offset: 0x0006400F
		static LocalUser()
		{
			ReInput.ControllerConnectedEvent += LocalUser.OnControllerConnected;
			ReInput.ControllerDisconnectedEvent += LocalUser.OnControllerDisconnected;
		}

		// Token: 0x06001763 RID: 5987 RVA: 0x00065E34 File Offset: 0x00064034
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

		// Token: 0x06001764 RID: 5988 RVA: 0x00065EB4 File Offset: 0x000640B4
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

		// Token: 0x06001765 RID: 5989 RVA: 0x00065F34 File Offset: 0x00064134
		private void OnRewiredPlayerDiscovered(Player player)
		{
			foreach (Controller controller in player.controllers.Controllers)
			{
				this.OnControllerDiscovered(controller);
			}
		}

		// Token: 0x06001766 RID: 5990 RVA: 0x00065F88 File Offset: 0x00064188
		private void OnRewiredPlayerLost(Player player)
		{
			foreach (Controller controller in player.controllers.Controllers)
			{
				this.OnControllerLost(controller);
			}
		}

		// Token: 0x06001767 RID: 5991 RVA: 0x0000409B File Offset: 0x0000229B
		private void OnControllerDiscovered(Controller controller)
		{
		}

		// Token: 0x06001768 RID: 5992 RVA: 0x00065FDC File Offset: 0x000641DC
		private void OnControllerLost(Controller controller)
		{
			this.inputPlayer.controllers.maps.ClearMapsForController(controller.type, controller.id, true);
		}

		// Token: 0x06001769 RID: 5993 RVA: 0x00066000 File Offset: 0x00064200
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

		// Token: 0x170002B8 RID: 696
		// (get) Token: 0x0600176A RID: 5994 RVA: 0x000660BC File Offset: 0x000642BC
		public bool isUIFocused
		{
			get
			{
				return this.eventSystem.currentSelectedGameObject;
			}
		}

		// Token: 0x170002B9 RID: 697
		// (get) Token: 0x0600176B RID: 5995 RVA: 0x000660CE File Offset: 0x000642CE
		// (set) Token: 0x0600176C RID: 5996 RVA: 0x000660D6 File Offset: 0x000642D6
		public NetworkUser currentNetworkUser { get; private set; }

		// Token: 0x170002BA RID: 698
		// (get) Token: 0x0600176D RID: 5997 RVA: 0x000660DF File Offset: 0x000642DF
		// (set) Token: 0x0600176E RID: 5998 RVA: 0x000660E7 File Offset: 0x000642E7
		public PlayerCharacterMasterController cachedMasterController { get; private set; }

		// Token: 0x170002BB RID: 699
		// (get) Token: 0x0600176F RID: 5999 RVA: 0x000660F0 File Offset: 0x000642F0
		// (set) Token: 0x06001770 RID: 6000 RVA: 0x000660F8 File Offset: 0x000642F8
		public GameObject cachedMasterObject { get; private set; }

		// Token: 0x170002BC RID: 700
		// (get) Token: 0x06001771 RID: 6001 RVA: 0x00066101 File Offset: 0x00064301
		// (set) Token: 0x06001772 RID: 6002 RVA: 0x00066109 File Offset: 0x00064309
		public CharacterBody cachedBody { get; private set; }

		// Token: 0x170002BD RID: 701
		// (get) Token: 0x06001773 RID: 6003 RVA: 0x00066112 File Offset: 0x00064312
		// (set) Token: 0x06001774 RID: 6004 RVA: 0x0006611A File Offset: 0x0006431A
		public GameObject cachedBodyObject { get; private set; }

		// Token: 0x06001775 RID: 6005 RVA: 0x00066124 File Offset: 0x00064324
		public void RebuildControlChain()
		{
			PlayerCharacterMasterController cachedMasterController = this.cachedMasterController;
			this.cachedMasterController = null;
			this.cachedMasterObject = null;
			CharacterBody cachedBody = this.cachedBody;
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

		// Token: 0x14000050 RID: 80
		// (add) Token: 0x06001776 RID: 6006 RVA: 0x00066208 File Offset: 0x00064408
		// (remove) Token: 0x06001777 RID: 6007 RVA: 0x00066240 File Offset: 0x00064440
		public event Action onBodyChanged;

		// Token: 0x14000051 RID: 81
		// (add) Token: 0x06001778 RID: 6008 RVA: 0x00066278 File Offset: 0x00064478
		// (remove) Token: 0x06001779 RID: 6009 RVA: 0x000662B0 File Offset: 0x000644B0
		public event Action onMasterChanged;

		// Token: 0x14000052 RID: 82
		// (add) Token: 0x0600177A RID: 6010 RVA: 0x000662E8 File Offset: 0x000644E8
		// (remove) Token: 0x0600177B RID: 6011 RVA: 0x00066320 File Offset: 0x00064520
		public event Action<NetworkUser> onNetworkUserFound;

		// Token: 0x14000053 RID: 83
		// (add) Token: 0x0600177C RID: 6012 RVA: 0x00066358 File Offset: 0x00064558
		// (remove) Token: 0x0600177D RID: 6013 RVA: 0x00066390 File Offset: 0x00064590
		public event Action<NetworkUser> onNetworkUserLost;

		// Token: 0x0600177E RID: 6014 RVA: 0x000663C5 File Offset: 0x000645C5
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

		// Token: 0x0600177F RID: 6015 RVA: 0x000663F4 File Offset: 0x000645F4
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

		// Token: 0x06001781 RID: 6017 RVA: 0x00066448 File Offset: 0x00064648
		[CompilerGenerated]
		private void <ApplyUserProfileBindingsToRewiredPlayer>g__ApplyUserProfileBindingstoRewiredController|20_0(Controller controller)
		{
			if (this.userProfile == null)
			{
				return;
			}
			ControllerMap controllerMap = null;
			switch (controller.type)
			{
			case ControllerType.Keyboard:
				controllerMap = this.userProfile.keyboardMap;
				break;
			case ControllerType.Mouse:
				controllerMap = this.userProfile.mouseMap;
				break;
			case ControllerType.Joystick:
				controllerMap = this.userProfile.joystickMap;
				break;
			}
			if (controllerMap != null)
			{
				this.inputPlayer.controllers.maps.AddMap(controller, controllerMap);
			}
		}

		// Token: 0x0400162E RID: 5678
		private Player _inputPlayer;

		// Token: 0x04001630 RID: 5680
		private UserProfile _userProfile;

		// Token: 0x04001631 RID: 5681
		public int id;

		// Token: 0x04001637 RID: 5687
		public CameraRigController cameraRigController;
	}
}
