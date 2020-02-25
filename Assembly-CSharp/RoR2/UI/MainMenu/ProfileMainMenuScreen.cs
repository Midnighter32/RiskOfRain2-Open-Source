using System;
using System.Collections.Generic;
using Facepunch.Steamworks;
using TMPro;
using UnityEngine;

namespace RoR2.UI.MainMenu
{
	// Token: 0x0200065D RID: 1629
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class ProfileMainMenuScreen : BaseMainMenuScreen
	{
		// Token: 0x06002633 RID: 9779 RVA: 0x000A5FA0 File Offset: 0x000A41A0
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

		// Token: 0x06002634 RID: 9780 RVA: 0x000A5FCE File Offset: 0x000A41CE
		protected void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			this.existingProfileListController.onProfileSelected += this.SetMainProfile;
			this.existingProfileListController.onListRebuilt += this.OnListRebuilt;
		}

		// Token: 0x06002635 RID: 9781 RVA: 0x000A600C File Offset: 0x000A420C
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

		// Token: 0x06002636 RID: 9782 RVA: 0x000A6098 File Offset: 0x000A4298
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

		// Token: 0x06002637 RID: 9783 RVA: 0x000A60E8 File Offset: 0x000A42E8
		private void OnListRebuilt()
		{
			this.existingProfileListController.GetReadOnlyElementsList();
		}

		// Token: 0x06002638 RID: 9784 RVA: 0x0000409B File Offset: 0x0000229B
		protected void OnDisable()
		{
		}

		// Token: 0x06002639 RID: 9785 RVA: 0x000A60F8 File Offset: 0x000A42F8
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

		// Token: 0x0600263A RID: 9786 RVA: 0x000A613E File Offset: 0x000A433E
		private static bool IsProfileCustom(UserProfile profile)
		{
			return profile.fileName != "default";
		}

		// Token: 0x0600263B RID: 9787 RVA: 0x000A6150 File Offset: 0x000A4350
		private static bool IsNewProfileNameAcceptable(string newProfileName)
		{
			return UserProfile.GetProfile(newProfileName) == null && !(newProfileName == "");
		}

		// Token: 0x0600263C RID: 9788 RVA: 0x000A616C File Offset: 0x000A436C
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

		// Token: 0x0600263D RID: 9789 RVA: 0x000A61EC File Offset: 0x000A43EC
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

		// Token: 0x040023FF RID: 9215
		public GameObject createProfilePanel;

		// Token: 0x04002400 RID: 9216
		public TMP_InputField createProfileNameInputField;

		// Token: 0x04002401 RID: 9217
		public MPButton submitProfileNameButton;

		// Token: 0x04002402 RID: 9218
		public GameObject gotoSelectProfilePanelButtonContainer;

		// Token: 0x04002403 RID: 9219
		public GameObject selectProfilePanel;

		// Token: 0x04002404 RID: 9220
		public MPButton gotoCreateProfilePanelButton;

		// Token: 0x04002405 RID: 9221
		public UserProfileListController existingProfileListController;

		// Token: 0x04002406 RID: 9222
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04002407 RID: 9223
		private bool firstTimeConfiguration;

		// Token: 0x04002408 RID: 9224
		private const string defaultName = "Nameless Survivor";
	}
}
