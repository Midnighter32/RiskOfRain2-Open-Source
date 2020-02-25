using System;
using RoR2.ConVar;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2.UI
{
	// Token: 0x0200058C RID: 1420
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class BaseSettingsControl : MonoBehaviour
	{
		// Token: 0x17000394 RID: 916
		// (get) Token: 0x060021C9 RID: 8649 RVA: 0x00092202 File Offset: 0x00090402
		public bool hasBeenChanged
		{
			get
			{
				return this.originalValue != null;
			}
		}

		// Token: 0x060021CA RID: 8650 RVA: 0x00092210 File Offset: 0x00090410
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

		// Token: 0x060021CB RID: 8651 RVA: 0x0009227D File Offset: 0x0009047D
		protected void Start()
		{
			this.Initialize();
		}

		// Token: 0x060021CC RID: 8652 RVA: 0x00092285 File Offset: 0x00090485
		protected void OnEnable()
		{
			this.UpdateControls();
		}

		// Token: 0x060021CD RID: 8653 RVA: 0x0000409B File Offset: 0x0000229B
		public virtual void Initialize()
		{
		}

		// Token: 0x060021CE RID: 8654 RVA: 0x0009228D File Offset: 0x0009048D
		public void SubmitSetting(string newValue)
		{
			if (this.useConfirmationDialog)
			{
				this.SubmitSettingTemporary(newValue);
				return;
			}
			this.SubmitSettingInternal(newValue);
		}

		// Token: 0x060021CF RID: 8655 RVA: 0x000922A8 File Offset: 0x000904A8
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
					conVar.AttemptSetString(newValue);
				}
			}
			RoR2Application.onNextUpdate += this.UpdateControls;
		}

		// Token: 0x060021D0 RID: 8656 RVA: 0x00092340 File Offset: 0x00090540
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

		// Token: 0x060021D1 RID: 8657 RVA: 0x00092448 File Offset: 0x00090648
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

		// Token: 0x060021D2 RID: 8658 RVA: 0x0009249E File Offset: 0x0009069E
		protected BaseConVar GetConVar()
		{
			return Console.instance.FindConVar(this.settingName);
		}

		// Token: 0x060021D3 RID: 8659 RVA: 0x000924B0 File Offset: 0x000906B0
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

		// Token: 0x060021D4 RID: 8660 RVA: 0x000924D3 File Offset: 0x000906D3
		public void Revert()
		{
			if (this.hasBeenChanged)
			{
				this.SubmitSetting(this.originalValue);
				this.originalValue = null;
			}
		}

		// Token: 0x17000395 RID: 917
		// (get) Token: 0x060021D5 RID: 8661 RVA: 0x000924F0 File Offset: 0x000906F0
		// (set) Token: 0x060021D6 RID: 8662 RVA: 0x000924F8 File Offset: 0x000906F8
		private protected bool inUpdateControls { protected get; private set; }

		// Token: 0x060021D7 RID: 8663 RVA: 0x00092501 File Offset: 0x00090701
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

		// Token: 0x060021D8 RID: 8664 RVA: 0x0000409B File Offset: 0x0000229B
		protected virtual void OnUpdateControls()
		{
		}

		// Token: 0x04001F28 RID: 7976
		public BaseSettingsControl.SettingSource settingSource;

		// Token: 0x04001F29 RID: 7977
		[FormerlySerializedAs("convarName")]
		public string settingName;

		// Token: 0x04001F2A RID: 7978
		public string nameToken;

		// Token: 0x04001F2B RID: 7979
		public LanguageTextMeshController nameLabel;

		// Token: 0x04001F2C RID: 7980
		[Tooltip("Whether or not this setting requires a confirmation dialog. This is mainly for video options.")]
		public bool useConfirmationDialog;

		// Token: 0x04001F2D RID: 7981
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04001F2E RID: 7982
		private string originalValue;

		// Token: 0x0200058D RID: 1421
		public enum SettingSource
		{
			// Token: 0x04001F31 RID: 7985
			ConVar,
			// Token: 0x04001F32 RID: 7986
			UserProfilePref
		}
	}
}
