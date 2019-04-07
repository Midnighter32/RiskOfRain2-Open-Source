using System;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI.LogBook
{
	// Token: 0x0200066C RID: 1644
	public class CategoryDef
	{
		// Token: 0x17000327 RID: 807
		// (get) Token: 0x060024AF RID: 9391 RVA: 0x000ABA3E File Offset: 0x000A9C3E
		// (set) Token: 0x060024B0 RID: 9392 RVA: 0x000ABA46 File Offset: 0x000A9C46
		public GameObject iconPrefab
		{
			get
			{
				return this._iconPrefab;
			}
			[NotNull]
			set
			{
				this._iconPrefab = value;
				this.iconSize = ((RectTransform)this._iconPrefab.transform).sizeDelta;
			}
		}

		// Token: 0x060024B1 RID: 9393 RVA: 0x000ABA6C File Offset: 0x000A9C6C
		public static void InitializeDefault(GameObject gameObject, Entry entry, EntryStatus status, UserProfile userProfile)
		{
			Texture texture = null;
			Color color = Color.white;
			Texture texture2;
			switch (status)
			{
			case EntryStatus.Unimplemented:
				texture2 = Resources.Load<Texture2D>("Textures/MiscIcons/texWIPIcon");
				break;
			case EntryStatus.Locked:
				texture2 = Resources.Load<Texture2D>("Textures/MiscIcons/texUnlockIcon");
				color = Color.gray;
				break;
			case EntryStatus.Unencountered:
				texture2 = entry.iconTexture;
				color = Color.black;
				break;
			case EntryStatus.Available:
				texture2 = entry.iconTexture;
				texture = entry.bgTexture;
				color = Color.white;
				break;
			case EntryStatus.New:
				texture2 = entry.iconTexture;
				texture = entry.bgTexture;
				color = new Color(1f, 0.8f, 0.5f, 1f);
				break;
			default:
				throw new ArgumentOutOfRangeException("status", status, null);
			}
			RawImage rawImage = null;
			ChildLocator component = gameObject.GetComponent<ChildLocator>();
			RawImage rawImage2;
			if (component)
			{
				rawImage2 = component.FindChild("Icon").GetComponent<RawImage>();
				rawImage = component.FindChild("BG").GetComponent<RawImage>();
			}
			else
			{
				rawImage2 = gameObject.GetComponentInChildren<RawImage>();
			}
			rawImage2.texture = texture2;
			rawImage2.color = color;
			if (rawImage)
			{
				if (texture != null)
				{
					rawImage.texture = texture;
				}
				else
				{
					rawImage.enabled = false;
				}
			}
			TextMeshProUGUI componentInChildren = gameObject.GetComponentInChildren<TextMeshProUGUI>();
			if (componentInChildren)
			{
				if (status >= EntryStatus.Available)
				{
					componentInChildren.text = Language.GetString(entry.nameToken);
					return;
				}
				componentInChildren.text = Language.GetString("UNIDENTIFIED");
			}
		}

		// Token: 0x060024B2 RID: 9394 RVA: 0x000ABBD4 File Offset: 0x000A9DD4
		public static void InitializeChallenge(GameObject gameObject, Entry entry, EntryStatus status, UserProfile userProfile)
		{
			TextMeshProUGUI textMeshProUGUI = null;
			TextMeshProUGUI textMeshProUGUI2 = null;
			RawImage rawImage = null;
			AchievementDef achievementDef = (AchievementDef)entry.extraData;
			float achievementProgress = AchievementManager.GetUserAchievementManager(LocalUserManager.readOnlyLocalUsersList.FirstOrDefault((LocalUser v) => v.userProfile == userProfile)).GetAchievementProgress(achievementDef);
			ChildLocator component = gameObject.GetComponent<ChildLocator>();
			if (component)
			{
				textMeshProUGUI = component.FindChild("DescriptionLabel").GetComponent<TextMeshProUGUI>();
				textMeshProUGUI2 = component.FindChild("NameLabel").GetComponent<TextMeshProUGUI>();
				rawImage = component.FindChild("RewardImage").GetComponent<RawImage>();
				textMeshProUGUI2.text = Language.GetString(achievementDef.nameToken);
				textMeshProUGUI.text = Language.GetString(achievementDef.descriptionToken);
			}
			Texture texture = null;
			Color color = Color.white;
			switch (status)
			{
			case EntryStatus.None:
				component.FindChild("RewardImageContainer").gameObject.SetActive(true);
				textMeshProUGUI2.text = "";
				textMeshProUGUI.text = "";
				break;
			case EntryStatus.Unimplemented:
				texture = Resources.Load<Texture2D>("Textures/MiscIcons/texWIPIcon");
				break;
			case EntryStatus.Locked:
				texture = Resources.Load<Texture2D>("Textures/MiscIcons/texUnlockIcon");
				color = Color.black;
				textMeshProUGUI2.text = Language.GetString("UNIDENTIFIED");
				textMeshProUGUI.text = Language.GetString("UNIDENTIFIED");
				component.FindChild("CantBeAchieved").gameObject.SetActive(true);
				break;
			case EntryStatus.Unencountered:
				texture = Resources.Load<Texture2D>("Textures/MiscIcons/texUnlockIcon");
				color = Color.gray;
				component.FindChild("ProgressTowardsUnlocking").GetComponent<Image>().fillAmount = achievementProgress;
				break;
			case EntryStatus.Available:
				texture = entry.iconTexture;
				color = Color.white;
				component.FindChild("HasBeenUnlocked").gameObject.SetActive(true);
				break;
			case EntryStatus.New:
				texture = entry.iconTexture;
				color = new Color(1f, 0.8f, 0.5f, 1f);
				component.FindChild("HasBeenUnlocked").gameObject.SetActive(true);
				break;
			default:
				throw new ArgumentOutOfRangeException("status", status, null);
			}
			if (texture != null)
			{
				rawImage.texture = texture;
				rawImage.color = color;
				return;
			}
			rawImage.enabled = false;
		}

		// Token: 0x040027B3 RID: 10163
		[NotNull]
		public string nameToken = string.Empty;

		// Token: 0x040027B4 RID: 10164
		[NotNull]
		public Entry[] entries = Array.Empty<Entry>();

		// Token: 0x040027B5 RID: 10165
		private GameObject _iconPrefab;

		// Token: 0x040027B6 RID: 10166
		public Vector2 iconSize = Vector2.one;

		// Token: 0x040027B7 RID: 10167
		public bool fullWidth;

		// Token: 0x040027B8 RID: 10168
		public Action<GameObject, Entry, EntryStatus, UserProfile> initializeElementGraphics = new Action<GameObject, Entry, EntryStatus, UserProfile>(CategoryDef.InitializeDefault);

		// Token: 0x040027B9 RID: 10169
		public ViewablesCatalog.Node viewableNode;
	}
}
