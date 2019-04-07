using System;
using RoR2.ConVar;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2.UI
{
	// Token: 0x020005B2 RID: 1458
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class BaseSettingsControl : MonoBehaviour
	{
		// Token: 0x170002E0 RID: 736
		// (get) Token: 0x060020A7 RID: 8359 RVA: 0x00099A26 File Offset: 0x00097C26
		public bool hasBeenChanged
		{
			get
			{
				return this.originalValue != null;
			}
		}

		// Token: 0x060020A8 RID: 8360 RVA: 0x00099A34 File Offset: 0x00097C34
		protected void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			if (this.nameLabel && !string.IsNullOrEmpty(this.nameToken))
			{
				this.nameLabel.token = this.nameToken;
			}
			if (this.settingSource == BaseSettingsControl.SettingSource.ConVar && this.GetConVar() == null)
			{
				Debug.LogErrorFormat("Null convar{0} detected in options", new object[]
				{
					this.settingName
				});
			}
		}

		// Token: 0x060020A9 RID: 8361 RVA: 0x00099AA1 File Offset: 0x00097CA1
		protected void Start()
		{
			this.Initialize();
		}

		// Token: 0x060020AA RID: 8362 RVA: 0x00099AA9 File Offset: 0x00097CA9
		protected void OnEnable()
		{
			this.UpdateControls();
		}

		// Token: 0x060020AB RID: 8363 RVA: 0x00004507 File Offset: 0x00002707
		public virtual void Initialize()
		{
		}

		// Token: 0x060020AC RID: 8364 RVA: 0x00099AB1 File Offset: 0x00097CB1
		public void SubmitSetting(string newValue)
		{
			if (this.useConfirmationDialog)
			{
				this.SubmitSettingTemporary(newValue);
				return;
			}
			this.SubmitSettingInternal(newValue);
		}

		// Token: 0x060020AD RID: 8365 RVA: 0x00099ACC File Offset: 0x00097CCC
		private void SubmitSettingInternal(string newValue)
		{
			if (this.originalValue == null)
			{
				this.originalValue = this.GetCurrentValue();
			}
			if (this.originalValue == newValue)
			{
				this.originalValue = null;
			}
			BaseSettingsControl.SettingSource settingSource = this.settingSource;
			if (settingSource != BaseSettingsControl.SettingSource.ConVar)
			{
				if (settingSource == BaseSettingsControl.SettingSource.UserProfilePref)
				{
					UserProfile currentUserProfile = this.GetCurrentUserProfile();
					if (currentUserProfile != null)
					{
						currentUserProfile.SetSaveFieldString(this.settingName, newValue);
					}
					UserProfile currentUserProfile2 = this.GetCurrentUserProfile();
					if (currentUserProfile2 != null)
					{
						currentUserProfile2.RequestSave(false);
					}
				}
			}
			else
			{
				BaseConVar conVar = this.GetConVar();
				if (conVar != null)
				{
					conVar.SetString(newValue);
				}
			}
			RoR2Application.onNextUpdate += this.UpdateControls;
		}

		// Token: 0x060020AE RID: 8366 RVA: 0x00099B64 File Offset: 0x00097D64
		private void SubmitSettingTemporary(string newValue)
		{
			string oldValue = this.GetCurrentValue();
			if (newValue == oldValue)
			{
				return;
			}
			this.SubmitSettingInternal(newValue);
			SimpleDialogBox dialogBox = SimpleDialogBox.Create(null);
			Action revertFunction = delegate()
			{
				if (dialogBox)
				{
					this.SubmitSettingInternal(oldValue);
				}
			};
			float num = 10f;
			float timeEnd = Time.unscaledTime + num;
			MPButton revertButton = dialogBox.AddActionButton(delegate
			{
				revertFunction();
			}, "OPTION_REVERT", Array.Empty<object>());
			dialogBox.AddActionButton(delegate
			{
			}, "OPTION_ACCEPT", Array.Empty<object>());
			Action updateText = null;
			updateText = delegate()
			{
				if (dialogBox)
				{
					int num2 = Mathf.FloorToInt(timeEnd - Time.unscaledTime);
					if (num2 < 0)
					{
						num2 = 0;
					}
					dialogBox.descriptionToken = new SimpleDialogBox.TokenParamsPair
					{
						token = "OPTION_AUTOREVERT_DIALOG_DESCRIPTION",
						formatParams = new object[]
						{
							num2
						}
					};
					if (num2 > 0)
					{
						RoR2Application.unscaledTimeTimers.CreateTimer(1f, updateText);
					}
				}
			};
			updateText();
			RoR2Application.unscaledTimeTimers.CreateTimer(num, delegate
			{
				if (revertButton)
				{
					revertButton.onClick.Invoke();
				}
			});
		}

		// Token: 0x060020AF RID: 8367 RVA: 0x00099C6C File Offset: 0x00097E6C
		public string GetCurrentValue()
		{
			BaseSettingsControl.SettingSource settingSource = this.settingSource;
			if (settingSource != BaseSettingsControl.SettingSource.ConVar)
			{
				if (settingSource != BaseSettingsControl.SettingSource.UserProfilePref)
				{
					return "";
				}
				UserProfile currentUserProfile = this.GetCurrentUserProfile();
				return ((currentUserProfile != null) ? currentUserProfile.GetSaveFieldString(this.settingName) : null) ?? "";
			}
			else
			{
				BaseConVar conVar = this.GetConVar();
				if (conVar == null)
				{
					return null;
				}
				return conVar.GetString();
			}
		}

		// Token: 0x060020B0 RID: 8368 RVA: 0x00099CC2 File Offset: 0x00097EC2
		protected BaseConVar GetConVar()
		{
			return Console.instance.FindConVar(this.settingName);
		}

		// Token: 0x060020B1 RID: 8369 RVA: 0x00099CD4 File Offset: 0x00097ED4
		public UserProfile GetCurrentUserProfile()
		{
			MPEventSystem eventSystem = this.eventSystemLocator.eventSystem;
			if (eventSystem == null)
			{
				return null;
			}
			LocalUser localUser = eventSystem.localUser;
			if (localUser == null)
			{
				return null;
			}
			return localUser.userProfile;
		}

		// Token: 0x060020B2 RID: 8370 RVA: 0x00099CF7 File Offset: 0x00097EF7
		public void Revert()
		{
			if (this.hasBeenChanged)
			{
				this.SubmitSetting(this.originalValue);
				this.originalValue = null;
			}
		}

		// Token: 0x170002E1 RID: 737
		// (get) Token: 0x060020B3 RID: 8371 RVA: 0x00099D14 File Offset: 0x00097F14
		// (set) Token: 0x060020B4 RID: 8372 RVA: 0x00099D1C File Offset: 0x00097F1C
		private protected bool inUpdateControls { protected get; private set; }

		// Token: 0x060020B5 RID: 8373 RVA: 0x00099D25 File Offset: 0x00097F25
		protected void UpdateControls()
		{
			if (!this)
			{
				return;
			}
			if (this.inUpdateControls)
			{
				return;
			}
			this.inUpdateControls = true;
			this.OnUpdateControls();
			this.inUpdateControls = false;
		}

		// Token: 0x060020B6 RID: 8374 RVA: 0x00004507 File Offset: 0x00002707
		protected virtual void OnUpdateControls()
		{
		}

		// Token: 0x04002332 RID: 9010
		public BaseSettingsControl.SettingSource settingSource;

		// Token: 0x04002333 RID: 9011
		[FormerlySerializedAs("convarName")]
		public string settingName;

		// Token: 0x04002334 RID: 9012
		public string nameToken;

		// Token: 0x04002335 RID: 9013
		public LanguageTextMeshController nameLabel;

		// Token: 0x04002336 RID: 9014
		[Tooltip("Whether or not this setting requires a confirmation dialog. This is mainly for video options.")]
		public bool useConfirmationDialog;

		// Token: 0x04002337 RID: 9015
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04002338 RID: 9016
		private string originalValue;

		// Token: 0x020005B3 RID: 1459
		public enum SettingSource
		{
			// Token: 0x0400233B RID: 9019
			ConVar,
			// Token: 0x0400233C RID: 9020
			UserProfilePref
		}
	}
}
