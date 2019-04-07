using System;
using System.Collections.Generic;
using Facepunch.Steamworks;
using TMPro;
using UnityEngine;

namespace RoR2.UI.MainMenu
{
	// Token: 0x02000668 RID: 1640
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class ProfileMainMenuScreen : BaseMainMenuScreen
	{
		// Token: 0x0600248F RID: 9359 RVA: 0x000AB464 File Offset: 0x000A9664
		private string GuessDefaultProfileName()
		{
			Client instance = Client.Instance;
			string text = (instance != null) ? instance.Username : null;
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return "Nameless Survivor";
		}

		// Token: 0x06002490 RID: 9360 RVA: 0x000AB492 File Offset: 0x000A9692
		protected void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			this.existingProfileListController.onProfileSelected += this.SetMainProfile;
			this.existingProfileListController.onListRebuilt += this.OnListRebuilt;
		}

		// Token: 0x06002491 RID: 9361 RVA: 0x000AB4D0 File Offset: 0x000A96D0
		protected void OnEnable()
		{
			this.firstTimeConfiguration = true;
			List<string> availableProfileNames = UserProfile.GetAvailableProfileNames();
			for (int i = 0; i < availableProfileNames.Count; i++)
			{
				if (ProfileMainMenuScreen.IsProfileCustom(UserProfile.GetProfile(availableProfileNames[i])))
				{
					this.firstTimeConfiguration = false;
					break;
				}
			}
			if (this.firstTimeConfiguration)
			{
				Debug.Log("First-Time Profile Configuration");
				this.OpenCreateProfileMenu(true);
				return;
			}
			this.createProfilePanel.SetActive(false);
			this.selectProfilePanel.SetActive(true);
			this.OnListRebuilt();
			this.gotoSelectProfilePanelButtonContainer.SetActive(true);
		}

		// Token: 0x06002492 RID: 9362 RVA: 0x000AB55C File Offset: 0x000A975C
		public void OpenCreateProfileMenu(bool firstTime)
		{
			this.selectProfilePanel.SetActive(false);
			this.createProfilePanel.SetActive(true);
			this.createProfileNameInputField.text = this.GuessDefaultProfileName();
			this.createProfileNameInputField.ActivateInputField();
			if (firstTime)
			{
				this.gotoSelectProfilePanelButtonContainer.SetActive(false);
			}
		}

		// Token: 0x06002493 RID: 9363 RVA: 0x000AB5AC File Offset: 0x000A97AC
		private void OnListRebuilt()
		{
			this.existingProfileListController.GetReadOnlyElementsList();
		}

		// Token: 0x06002494 RID: 9364 RVA: 0x00004507 File Offset: 0x00002707
		protected void OnDisable()
		{
		}

		// Token: 0x06002495 RID: 9365 RVA: 0x000AB5BC File Offset: 0x000A97BC
		private void SetMainProfile(UserProfile profile)
		{
			LocalUserManager.SetLocalUsers(new LocalUserManager.LocalUserInitializationInfo[]
			{
				new LocalUserManager.LocalUserInitializationInfo
				{
					profile = profile
				}
			});
			this.myMainMenuController.desiredMenuScreen = this.myMainMenuController.titleMenuScreen;
		}

		// Token: 0x06002496 RID: 9366 RVA: 0x000AB602 File Offset: 0x000A9802
		private static bool IsProfileCustom(UserProfile profile)
		{
			return profile.fileName != "default";
		}

		// Token: 0x06002497 RID: 9367 RVA: 0x000AB614 File Offset: 0x000A9814
		private static bool IsNewProfileNameAcceptable(string newProfileName)
		{
			return UserProfile.GetProfile(newProfileName) == null && !(newProfileName == "");
		}

		// Token: 0x06002498 RID: 9368 RVA: 0x000AB630 File Offset: 0x000A9830
		public void OnAddProfilePressed()
		{
			if (this.eventSystemLocator.eventSystem.currentSelectedGameObject == this.createProfileNameInputField.gameObject && !Input.GetKeyDown(KeyCode.Return) && !Input.GetKeyDown(KeyCode.KeypadEnter))
			{
				return;
			}
			string text = this.createProfileNameInputField.text;
			if (!ProfileMainMenuScreen.IsNewProfileNameAcceptable(text))
			{
				return;
			}
			this.createProfileNameInputField.text = "";
			UserProfile userProfile = UserProfile.CreateProfile(RoR2Application.cloudStorage, text);
			if (userProfile != null)
			{
				this.SetMainProfile(userProfile);
			}
		}

		// Token: 0x06002499 RID: 9369 RVA: 0x000AB6B0 File Offset: 0x000A98B0
		protected void Update()
		{
			if (Input.GetKeyDown(KeyCode.Delete))
			{
				GameObject currentSelectedGameObject = MPEventSystemManager.combinedEventSystem.currentSelectedGameObject;
				if (currentSelectedGameObject)
				{
					UserProfileListElementController component = currentSelectedGameObject.GetComponent<UserProfileListElementController>();
					if (component)
					{
						if (component.userProfile == null)
						{
							Debug.LogError("!!!???");
							return;
						}
						SimpleDialogBox simpleDialogBox = SimpleDialogBox.Create(null);
						string consoleString = "user_profile_delete \"" + component.userProfile.fileName + "\"";
						simpleDialogBox.headerToken = new SimpleDialogBox.TokenParamsPair
						{
							token = "USER_PROFILE_DELETE_HEADER",
							formatParams = null
						};
						simpleDialogBox.descriptionToken = new SimpleDialogBox.TokenParamsPair
						{
							token = "USER_PROFILE_DELETE_DESCRIPTION",
							formatParams = new object[]
							{
								component.userProfile.name
							}
						};
						simpleDialogBox.AddCommandButton(consoleString, "USER_PROFILE_DELETE_YES", Array.Empty<object>());
						simpleDialogBox.AddCancelButton("USER_PROFILE_DELETE_NO", Array.Empty<object>());
					}
				}
			}
		}

		// Token: 0x04002798 RID: 10136
		public GameObject createProfilePanel;

		// Token: 0x04002799 RID: 10137
		public TMP_InputField createProfileNameInputField;

		// Token: 0x0400279A RID: 10138
		public MPButton submitProfileNameButton;

		// Token: 0x0400279B RID: 10139
		public GameObject gotoSelectProfilePanelButtonContainer;

		// Token: 0x0400279C RID: 10140
		public GameObject selectProfilePanel;

		// Token: 0x0400279D RID: 10141
		public MPButton gotoCreateProfilePanelButton;

		// Token: 0x0400279E RID: 10142
		public UserProfileListController existingProfileListController;

		// Token: 0x0400279F RID: 10143
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x040027A0 RID: 10144
		private bool firstTimeConfiguration;

		// Token: 0x040027A1 RID: 10145
		private const string defaultName = "Nameless Survivor";
	}
}
