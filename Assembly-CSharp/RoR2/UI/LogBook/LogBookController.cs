using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EntityStates;
using RoR2.Stats;
using RoR2.UI.SkinControllers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoR2.UI.LogBook
{
	// Token: 0x02000666 RID: 1638
	public class LogBookController : MonoBehaviour
	{
		// Token: 0x06002660 RID: 9824 RVA: 0x000A6A40 File Offset: 0x000A4C40
		private void Awake()
		{
			this.navigationCategoryButtonAllocator = new UIElementAllocator<MPButton>(this.categoryContainer, Resources.Load<GameObject>("Prefabs/UI/Logbook/CategoryButton"));
			this.navigationPageIndicatorAllocator = new UIElementAllocator<MPButton>(this.navigationPageIndicatorContainer, this.navigationPageIndicatorPrefab);
			this.navigationPageIndicatorAllocator.onCreateElement = delegate(int index, MPButton button)
			{
				button.onClick.AddListener(delegate()
				{
					this.desiredPageIndex = index;
				});
			};
			this.previousPageButton.onClick.AddListener(new UnityAction(this.OnLeftButton));
			this.nextPageButton.onClick.AddListener(new UnityAction(this.OnRightButton));
			this.pageViewerBackButton.onClick.AddListener(new UnityAction(this.ReturnToNavigation));
			this.stateMachine = base.gameObject.AddComponent<EntityStateMachine>();
			this.stateMachine.initialStateType = default(SerializableEntityStateType);
			this.categoryHightlightRect = (RectTransform)UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/ButtonSelectionHighlight"), base.transform.parent).transform;
			this.categoryHightlightRect.gameObject.SetActive(false);
		}

		// Token: 0x06002661 RID: 9825 RVA: 0x000A6B47 File Offset: 0x000A4D47
		private void Start()
		{
			this.BuildCategoriesButtons();
			this.GeneratePages();
			this.stateMachine.SetNextState(new LogBookController.ChangeCategoryState
			{
				newCategoryIndex = 0
			});
		}

		// Token: 0x06002662 RID: 9826 RVA: 0x000A6B6C File Offset: 0x000A4D6C
		private void BuildCategoriesButtons()
		{
			this.navigationCategoryButtonAllocator.AllocateElements(LogBookController.categories.Length);
			ReadOnlyCollection<MPButton> elements = this.navigationCategoryButtonAllocator.elements;
			for (int i = 0; i < LogBookController.categories.Length; i++)
			{
				MPButton mpbutton = elements[i];
				CategoryDef categoryDef = LogBookController.categories[i];
				mpbutton.GetComponentInChildren<TextMeshProUGUI>().text = Language.GetString(categoryDef.nameToken);
				mpbutton.onClick.RemoveAllListeners();
				int categoryIndex = i;
				mpbutton.onClick.AddListener(delegate()
				{
					this.OnCategoryClicked(categoryIndex);
				});
				ViewableTag viewableTag = mpbutton.gameObject.GetComponent<ViewableTag>();
				if (!viewableTag)
				{
					viewableTag = mpbutton.gameObject.AddComponent<ViewableTag>();
				}
				viewableTag.viewableName = categoryDef.viewableNode.fullName;
			}
			if (this.categorySpaceFiller)
			{
				for (int j = 0; j < this.categorySpaceFillerCount; j++)
				{
					UnityEngine.Object.Instantiate<GameObject>(this.categorySpaceFiller, this.categoryContainer).gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x06002663 RID: 9827 RVA: 0x000A6C81 File Offset: 0x000A4E81
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			BodyCatalog.availability.CallWhenAvailable(delegate
			{
				SceneCatalog.availability.CallWhenAvailable(new Action(LogBookController.BuildStaticData));
			});
		}

		// Token: 0x06002664 RID: 9828 RVA: 0x000A6CAC File Offset: 0x000A4EAC
		private static EntryStatus GetPickupStatus(UserProfile userProfile, Entry entry)
		{
			PickupIndex pickupIndex = (PickupIndex)entry.extraData;
			ItemIndex itemIndex = pickupIndex.itemIndex;
			EquipmentIndex equipmentIndex = pickupIndex.equipmentIndex;
			string unlockableName;
			if (itemIndex != ItemIndex.None)
			{
				unlockableName = ItemCatalog.GetItemDef(itemIndex).unlockableName;
			}
			else
			{
				if (equipmentIndex == EquipmentIndex.None)
				{
					return EntryStatus.Unimplemented;
				}
				unlockableName = EquipmentCatalog.GetEquipmentDef(equipmentIndex).unlockableName;
			}
			if (!userProfile.HasUnlockable(unlockableName))
			{
				return EntryStatus.Locked;
			}
			if (!userProfile.HasDiscoveredPickup(pickupIndex))
			{
				return EntryStatus.Unencountered;
			}
			return EntryStatus.Available;
		}

		// Token: 0x06002665 RID: 9829 RVA: 0x000A6D18 File Offset: 0x000A4F18
		private static TooltipContent GetPickupTooltipContent(UserProfile userProfile, Entry entry, EntryStatus status)
		{
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(((PickupIndex)entry.extraData).GetUnlockableName());
			TooltipContent result = default(TooltipContent);
			if (status >= EntryStatus.Available)
			{
				result.titleToken = entry.nameToken;
				result.titleColor = entry.color;
				if (unlockableDef != null)
				{
					result.overrideBodyText = unlockableDef.getUnlockedString();
				}
				result.bodyToken = "LOGBOOK_CATEGORY_ITEM";
				result.bodyColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Unlockable);
			}
			else
			{
				result.titleToken = "UNIDENTIFIED";
				result.titleColor = Color.gray;
				if (status == EntryStatus.Unimplemented)
				{
					result.titleToken = "TOOLTIP_WIP_CONTENT_NAME";
					result.bodyToken = "TOOLTIP_WIP_CONTENT_DESCRIPTION";
				}
				else if (status == EntryStatus.Unencountered)
				{
					result.overrideBodyText = Language.GetString("LOGBOOK_UNLOCK_ITEM_LOG");
				}
				else if (status == EntryStatus.Locked)
				{
					result.overrideBodyText = unlockableDef.getHowToUnlockString();
				}
				result.bodyColor = Color.white;
			}
			return result;
		}

		// Token: 0x06002666 RID: 9830 RVA: 0x000A6E0C File Offset: 0x000A500C
		private static TooltipContent GetMonsterTooltipContent(UserProfile userProfile, Entry entry, EntryStatus status)
		{
			TooltipContent result = default(TooltipContent);
			result.titleColor = entry.color;
			if (status >= EntryStatus.Available)
			{
				result.titleToken = entry.nameToken;
				result.titleColor = entry.color;
				result.bodyToken = "LOGBOOK_CATEGORY_MONSTER";
			}
			else
			{
				result.titleToken = "UNIDENTIFIED";
				result.titleColor = Color.gray;
				result.bodyToken = "LOGBOOK_UNLOCK_ITEM_MONSTER";
			}
			return result;
		}

		// Token: 0x06002667 RID: 9831 RVA: 0x000A6E80 File Offset: 0x000A5080
		private static TooltipContent GetStageTooltipContent(UserProfile userProfile, Entry entry, EntryStatus status)
		{
			TooltipContent result = default(TooltipContent);
			result.titleColor = entry.color;
			if (status >= EntryStatus.Available)
			{
				result.titleToken = entry.nameToken;
				result.titleColor = entry.color;
				result.bodyToken = "LOGBOOK_CATEGORY_STAGE";
			}
			else
			{
				result.titleToken = "UNIDENTIFIED";
				result.titleColor = Color.gray;
				result.bodyToken = "LOGBOOK_UNLOCK_ITEM_STAGE";
			}
			return result;
		}

		// Token: 0x06002668 RID: 9832 RVA: 0x000A6EF4 File Offset: 0x000A50F4
		private static TooltipContent GetSurvivorTooltipContent(UserProfile userProfile, Entry entry, EntryStatus status)
		{
			TooltipContent result = default(TooltipContent);
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(SurvivorCatalog.FindSurvivorDefFromBody(((CharacterBody)entry.extraData).gameObject).unlockableName);
			if (status >= EntryStatus.Available)
			{
				result.titleToken = entry.nameToken;
				result.titleColor = entry.color;
				result.bodyToken = "LOGBOOK_CATEGORY_SURVIVOR";
			}
			else
			{
				result.titleToken = "UNIDENTIFIED";
				result.titleColor = Color.gray;
				if (status == EntryStatus.Unencountered)
				{
					result.overrideBodyText = Language.GetString("LOGBOOK_UNLOCK_ITEM_SURVIVOR");
				}
				else if (status == EntryStatus.Locked)
				{
					result.overrideBodyText = unlockableDef.getHowToUnlockString();
				}
			}
			return result;
		}

		// Token: 0x06002669 RID: 9833 RVA: 0x000A6F9C File Offset: 0x000A519C
		private static TooltipContent GetWIPTooltipContent(UserProfile userProfile, Entry entry, EntryStatus status)
		{
			return new TooltipContent
			{
				titleColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.WIP),
				titleToken = "TOOLTIP_WIP_CONTENT_NAME",
				bodyToken = "TOOLTIP_WIP_CONTENT_DESCRIPTION"
			};
		}

		// Token: 0x0600266A RID: 9834 RVA: 0x0000C68F File Offset: 0x0000A88F
		private static EntryStatus GetAlwaysAvailable(UserProfile userProfile, Entry entry)
		{
			return EntryStatus.Available;
		}

		// Token: 0x0600266B RID: 9835 RVA: 0x0000B933 File Offset: 0x00009B33
		private static EntryStatus GetUnimplemented(UserProfile userProfile, Entry entry)
		{
			return EntryStatus.Unimplemented;
		}

		// Token: 0x0600266C RID: 9836 RVA: 0x000A6FE0 File Offset: 0x000A51E0
		private static EntryStatus GetStageStatus(UserProfile userProfile, Entry entry)
		{
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(SceneCatalog.GetUnlockableLogFromSceneName((entry.extraData as SceneDef).baseSceneName));
			if (unlockableDef != null && userProfile.HasUnlockable(unlockableDef))
			{
				return EntryStatus.Available;
			}
			return EntryStatus.Unencountered;
		}

		// Token: 0x0600266D RID: 9837 RVA: 0x000A7018 File Offset: 0x000A5218
		private static EntryStatus GetMonsterStatus(UserProfile userProfile, Entry entry)
		{
			CharacterBody characterBody = (CharacterBody)entry.extraData;
			DeathRewards component = characterBody.GetComponent<DeathRewards>();
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef((component != null) ? component.logUnlockableName : null);
			if (unlockableDef == null)
			{
				return EntryStatus.None;
			}
			if (userProfile.HasUnlockable(unlockableDef))
			{
				return EntryStatus.Available;
			}
			if (userProfile.statSheet.GetStatValueULong(PerBodyStatDef.killsAgainst, characterBody.gameObject.name) > 0UL)
			{
				return EntryStatus.Unencountered;
			}
			return EntryStatus.Locked;
		}

		// Token: 0x0600266E RID: 9838 RVA: 0x000A707C File Offset: 0x000A527C
		private static EntryStatus GetSurvivorStatus(UserProfile userProfile, Entry entry)
		{
			CharacterBody characterBody = (CharacterBody)entry.extraData;
			SurvivorDef survivorDef = SurvivorCatalog.FindSurvivorDefFromBody(characterBody.gameObject);
			if (!userProfile.HasUnlockable(survivorDef.unlockableName))
			{
				return EntryStatus.Locked;
			}
			if (userProfile.statSheet.GetStatValueDouble(PerBodyStatDef.totalTimeAlive, characterBody.gameObject.name) == 0.0)
			{
				return EntryStatus.Unencountered;
			}
			return EntryStatus.Available;
		}

		// Token: 0x0600266F RID: 9839 RVA: 0x000A70DC File Offset: 0x000A52DC
		private static EntryStatus GetAchievementStatus(UserProfile userProfile, Entry entry)
		{
			string identifier = ((AchievementDef)entry.extraData).identifier;
			bool flag = userProfile.HasAchievement(identifier);
			if (!userProfile.CanSeeAchievement(identifier))
			{
				return EntryStatus.Locked;
			}
			if (!flag)
			{
				return EntryStatus.Unencountered;
			}
			return EntryStatus.Available;
		}

		// Token: 0x06002670 RID: 9840 RVA: 0x000A7113 File Offset: 0x000A5313
		private static void BuildStaticData()
		{
			LogBookController.categories = LogBookController.BuildCategories();
			LogBookController.RegisterViewables(LogBookController.categories);
			LogBookController.availability.MakeAvailable();
		}

		// Token: 0x170003E8 RID: 1000
		// (get) Token: 0x06002672 RID: 9842 RVA: 0x000A714A File Offset: 0x000A534A
		// (set) Token: 0x06002673 RID: 9843 RVA: 0x000A7152 File Offset: 0x000A5352
		private LogBookController.NavigationPageInfo[] availableNavigationPages
		{
			get
			{
				return this._availableNavigationPages;
			}
			set
			{
				int num = this._availableNavigationPages.Length;
				this._availableNavigationPages = value;
				if (num != this.availableNavigationPages.Length)
				{
					this.navigationPageIndicatorAllocator.AllocateElements(this.availableNavigationPages.Length);
				}
			}
		}

		// Token: 0x06002674 RID: 9844 RVA: 0x000A7180 File Offset: 0x000A5380
		private LogBookController.NavigationPageInfo[] GetCategoryPages(int categoryIndex)
		{
			return this.navigationPagesByCategory[categoryIndex];
		}

		// Token: 0x06002675 RID: 9845 RVA: 0x000A718A File Offset: 0x000A538A
		private void OnLeftButton()
		{
			this.desiredPageIndex--;
		}

		// Token: 0x06002676 RID: 9846 RVA: 0x000A719A File Offset: 0x000A539A
		private void OnRightButton()
		{
			this.desiredPageIndex++;
		}

		// Token: 0x06002677 RID: 9847 RVA: 0x000A71AA File Offset: 0x000A53AA
		private void OnCategoryClicked(int categoryIndex)
		{
			this.desiredCategoryIndex = categoryIndex;
			this.goToEndOfNextCategory = false;
		}

		// Token: 0x06002678 RID: 9848 RVA: 0x000A71BC File Offset: 0x000A53BC
		private void GeneratePages()
		{
			this.navigationPagesByCategory = new LogBookController.NavigationPageInfo[LogBookController.categories.Length][];
			IEnumerable<LogBookController.NavigationPageInfo> enumerable = Array.Empty<LogBookController.NavigationPageInfo>();
			int num = 0;
			for (int i = 0; i < LogBookController.categories.Length; i++)
			{
				CategoryDef categoryDef = LogBookController.categories[i];
				bool fullWidth = categoryDef.fullWidth;
				Vector2 size = this.entryPageContainer.rect.size;
				if (fullWidth)
				{
					categoryDef.iconSize.x = size.x;
				}
				int num2 = Mathf.FloorToInt(Mathf.Max(size.x / categoryDef.iconSize.x, 1f));
				int num3 = Mathf.FloorToInt(Mathf.Max(size.y / categoryDef.iconSize.y, 1f));
				int num4 = num2 * num3;
				int num5 = Mathf.CeilToInt((float)categoryDef.entries.Length / (float)num4);
				if (num5 <= 0)
				{
					num5 = 1;
				}
				LogBookController.NavigationPageInfo[] array = new LogBookController.NavigationPageInfo[num5];
				for (int j = 0; j < num5; j++)
				{
					int num6 = num4;
					int num7 = j * num4;
					int num8 = categoryDef.entries.Length - num7;
					int num9 = num6;
					if (num9 > num8)
					{
						num9 = num8;
					}
					Entry[] array2 = new Entry[num6];
					Array.Copy(categoryDef.entries, num7, array2, 0, num9);
					array[j] = new LogBookController.NavigationPageInfo
					{
						categoryDef = categoryDef,
						entries = array2,
						index = num++,
						indexInCategory = j
					};
				}
				this.navigationPagesByCategory[i] = array;
				enumerable = enumerable.Concat(array);
			}
			this.allNavigationPages = enumerable.ToArray<LogBookController.NavigationPageInfo>();
		}

		// Token: 0x06002679 RID: 9849 RVA: 0x000A7344 File Offset: 0x000A5544
		private void Update()
		{
			if (this.desiredPageIndex > this.availableNavigationPages.Length - 1)
			{
				this.desiredPageIndex = this.availableNavigationPages.Length - 1;
				this.desiredCategoryIndex++;
				this.goToEndOfNextCategory = false;
			}
			if (this.desiredPageIndex < 0)
			{
				this.desiredCategoryIndex--;
				this.desiredPageIndex = 0;
				this.goToEndOfNextCategory = true;
			}
			if (this.desiredCategoryIndex > LogBookController.categories.Length - 1)
			{
				this.desiredCategoryIndex = LogBookController.categories.Length - 1;
			}
			if (this.desiredCategoryIndex < 0)
			{
				this.desiredCategoryIndex = 0;
			}
			foreach (MPButton mpbutton in this.navigationPageIndicatorAllocator.elements)
			{
				ColorBlock colors = mpbutton.colors;
				colors.colorMultiplier = 1f;
				mpbutton.colors = colors;
			}
			if (this.currentPageIndex < this.navigationPageIndicatorAllocator.elements.Count)
			{
				MPButton mpbutton2 = this.navigationPageIndicatorAllocator.elements[this.currentPageIndex];
				ColorBlock colors2 = mpbutton2.colors;
				colors2.colorMultiplier = 2f;
				mpbutton2.colors = colors2;
			}
			if (this.desiredCategoryIndex != this.currentCategoryIndex)
			{
				if (this.stateMachine.state is Idle)
				{
					int num = (this.desiredCategoryIndex > this.currentCategoryIndex) ? 1 : -1;
					this.stateMachine.SetNextState(new LogBookController.ChangeCategoryState
					{
						newCategoryIndex = this.currentCategoryIndex + num,
						goToLastPage = this.goToEndOfNextCategory
					});
					return;
				}
			}
			else if (this.desiredPageIndex != this.currentPageIndex && this.stateMachine.state is Idle)
			{
				int num2 = (this.desiredPageIndex > this.currentPageIndex) ? 1 : -1;
				this.stateMachine.SetNextState(new LogBookController.ChangeEntriesPageState
				{
					newNavigationPageInfo = this.GetCategoryPages(this.currentCategoryIndex)[this.currentPageIndex + num2],
					moveDirection = new Vector2((float)num2, 0f)
				});
			}
		}

		// Token: 0x0600267A RID: 9850 RVA: 0x000A7550 File Offset: 0x000A5750
		private UserProfile LookUpUserProfile()
		{
			LocalUser localUser = LocalUserManager.readOnlyLocalUsersList.FirstOrDefault((LocalUser v) => v != null);
			if (localUser == null)
			{
				return null;
			}
			return localUser.userProfile;
		}

		// Token: 0x0600267B RID: 9851 RVA: 0x000A7588 File Offset: 0x000A5788
		private GameObject BuildEntriesPage(LogBookController.NavigationPageInfo navigationPageInfo)
		{
			Entry[] entries = navigationPageInfo.entries;
			CategoryDef categoryDef = navigationPageInfo.categoryDef;
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.entryPagePrefab, this.entryPageContainer);
			gameObject.GetComponent<GridLayoutGroup>().cellSize = categoryDef.iconSize;
			UIElementAllocator<RectTransform> uielementAllocator = new UIElementAllocator<RectTransform>((RectTransform)gameObject.transform, categoryDef.iconPrefab);
			uielementAllocator.AllocateElements(entries.Length);
			UserProfile userProfile = this.LookUpUserProfile();
			ReadOnlyCollection<RectTransform> elements = uielementAllocator.elements;
			for (int i = 0; i < elements.Count; i++)
			{
				EntryStatus entryStatus = EntryStatus.None;
				RectTransform rectTransform = elements[i];
				MPButton component = rectTransform.GetComponent<MPButton>();
				Entry entry = (i < entries.Length) ? entries[i] : null;
				if (entry != null)
				{
					entryStatus = entry.getStatus(userProfile, entry);
					rectTransform.gameObject.AddComponent<TooltipProvider>().SetContent(entry.getTooltipContent(userProfile, entry, entryStatus));
					Action<GameObject, Entry, EntryStatus, UserProfile> initializeElementGraphics = categoryDef.initializeElementGraphics;
					if (initializeElementGraphics != null)
					{
						initializeElementGraphics(rectTransform.gameObject, entry, entryStatus, userProfile);
					}
					if (component)
					{
						component.interactable = (entryStatus >= EntryStatus.Available);
					}
					if (entry.viewableNode != null)
					{
						ViewableTag viewableTag = rectTransform.gameObject.GetComponent<ViewableTag>();
						if (!viewableTag)
						{
							viewableTag = rectTransform.gameObject.AddComponent<ViewableTag>();
							viewableTag.viewableVisualStyle = ViewableTag.ViewableVisualStyle.Icon;
						}
						viewableTag.viewableName = entry.viewableNode.fullName;
					}
				}
				if (entryStatus >= EntryStatus.Available && component)
				{
					component.onClick.AddListener(delegate()
					{
						this.ViewEntry(entry);
					});
				}
				if (entryStatus == EntryStatus.None)
				{
					if (component)
					{
						component.enabled = false;
						component.targetGraphic.color = this.spaceFillerColor;
						component.GetComponent<ButtonSkinController>().enabled = false;
					}
					else
					{
						rectTransform.GetComponent<Image>().color = this.spaceFillerColor;
					}
					for (int j = rectTransform.childCount - 1; j >= 0; j--)
					{
						UnityEngine.Object.Destroy(rectTransform.GetChild(j).gameObject);
					}
				}
			}
			gameObject.gameObject.SetActive(true);
			GridLayoutGroup gridLayoutGroup = gameObject.GetComponent<GridLayoutGroup>();
			Action destroyLayoutGroup = null;
			int frameTimer = 2;
			destroyLayoutGroup = delegate()
			{
				int frameTimer;
				frameTimer--;
				frameTimer = frameTimer;
				if (frameTimer <= 0)
				{
					gridLayoutGroup.enabled = false;
					RoR2Application.onLateUpdate -= destroyLayoutGroup;
				}
			};
			RoR2Application.onLateUpdate += destroyLayoutGroup;
			return gameObject;
		}

		// Token: 0x0600267C RID: 9852 RVA: 0x000A781C File Offset: 0x000A5A1C
		private void ViewEntry(Entry entry)
		{
			this.OnViewEntry.Invoke();
			LogBookPage component = this.pageViewerPanel.GetComponent<LogBookPage>();
			component.SetEntry(this.LookUpUserProfile(), entry);
			component.modelPanel.SetAnglesForCharacterThumbnailForSeconds(0.5f, false);
			ViewablesCatalog.Node viewableNode = entry.viewableNode;
			ViewableTrigger.TriggerView((viewableNode != null) ? viewableNode.fullName : null);
		}

		// Token: 0x0600267D RID: 9853 RVA: 0x000A7873 File Offset: 0x000A5A73
		private void ReturnToNavigation()
		{
			this.navigationPanel.SetActive(true);
			this.pageViewerPanel.SetActive(false);
		}

		// Token: 0x0600267E RID: 9854 RVA: 0x000A7890 File Offset: 0x000A5A90
		private static Entry[] BuildPickupEntries()
		{
			Entry entry = new Entry();
			entry.nameToken = "TOOLTIP_WIP_CONTENT_NAME";
			entry.color = Color.white;
			entry.iconTexture = Resources.Load<Texture>("Textures/MiscIcons/texWIPIcon");
			entry.getStatus = new Func<UserProfile, Entry, EntryStatus>(LogBookController.GetUnimplemented);
			entry.getTooltipContent = new Func<UserProfile, Entry, EntryStatus, TooltipContent>(LogBookController.GetWIPTooltipContent);
			IEnumerable<Entry> first = from pickupIndex in PickupIndex.allPickups
			select ItemCatalog.GetItemDef(pickupIndex.itemIndex) into itemDef
			where itemDef != null && itemDef.inDroppableTier
			orderby (int)(itemDef.tier + ((itemDef.tier == ItemTier.Lunar) ? 100 : 0))
			select new Entry
			{
				nameToken = itemDef.nameToken,
				categoryTypeToken = "LOGBOOK_CATEGORY_ITEM",
				color = ColorCatalog.GetColor(itemDef.darkColorIndex),
				iconTexture = itemDef.pickupIconTexture,
				bgTexture = itemDef.bgIconTexture,
				extraData = new PickupIndex(itemDef.itemIndex),
				modelPrefab = Resources.Load<GameObject>(itemDef.pickupModelPath),
				getStatus = new Func<UserProfile, Entry, EntryStatus>(LogBookController.GetPickupStatus),
				getTooltipContent = new Func<UserProfile, Entry, EntryStatus, TooltipContent>(LogBookController.GetPickupTooltipContent),
				addEntries = new Action<PageBuilder>(PageBuilder.SimplePickup),
				isWIP = Language.IsTokenInvalid(itemDef.loreToken)
			};
			IEnumerable<Entry> second = from pickupIndex in PickupIndex.allPickups
			select EquipmentCatalog.GetEquipmentDef(pickupIndex.equipmentIndex) into equipmentDef
			where equipmentDef != null && equipmentDef.canDrop
			orderby !equipmentDef.isLunar
			select new Entry
			{
				nameToken = equipmentDef.nameToken,
				categoryTypeToken = "LOGBOOK_CATEGORY_EQUIPMENT",
				color = ColorCatalog.GetColor(equipmentDef.colorIndex),
				iconTexture = equipmentDef.pickupIconTexture,
				bgTexture = equipmentDef.bgIconTexture,
				extraData = new PickupIndex(equipmentDef.equipmentIndex),
				modelPrefab = Resources.Load<GameObject>(equipmentDef.pickupModelPath),
				getStatus = new Func<UserProfile, Entry, EntryStatus>(LogBookController.GetPickupStatus),
				getTooltipContent = new Func<UserProfile, Entry, EntryStatus, TooltipContent>(LogBookController.GetPickupTooltipContent),
				addEntries = new Action<PageBuilder>(PageBuilder.SimplePickup),
				isWIP = Language.IsTokenInvalid(equipmentDef.loreToken)
			};
			IEnumerable<Entry> enumerable = first.Concat(second);
			int count = Math.Max(110 - enumerable.Count<Entry>(), 0);
			IEnumerable<Entry> second2 = Enumerable.Repeat<Entry>(entry, count);
			enumerable = enumerable.Concat(second2);
			return enumerable.ToArray<Entry>();
		}

		// Token: 0x0600267F RID: 9855 RVA: 0x000A7A50 File Offset: 0x000A5C50
		private static CategoryDef[] BuildCategories()
		{
			CategoryDef[] array = new CategoryDef[5];
			array[0] = new CategoryDef
			{
				nameToken = "LOGBOOK_CATEGORY_ITEMANDEQUIPMENT",
				iconPrefab = Resources.Load<GameObject>("Prefabs/UI/Logbook/ItemEntryIcon"),
				entries = LogBookController.BuildPickupEntries()
			};
			int num = 1;
			CategoryDef categoryDef = new CategoryDef();
			categoryDef.nameToken = "LOGBOOK_CATEGORY_MONSTER";
			categoryDef.iconPrefab = Resources.Load<GameObject>("Prefabs/UI/Logbook/MonsterEntryIcon");
			categoryDef.entries = (from characterBody in BodyCatalog.allBodyPrefabBodyBodyComponents.Where(delegate(CharacterBody characterBody)
			{
				if (characterBody)
				{
					DeathRewards component = characterBody.GetComponent<DeathRewards>();
					return !string.IsNullOrEmpty((component != null) ? component.logUnlockableName : null);
				}
				return false;
			})
			orderby characterBody.baseMaxHealth
			select characterBody).Select(delegate(CharacterBody characterBody)
			{
				Entry entry = new Entry();
				entry.nameToken = characterBody.baseNameToken;
				entry.categoryTypeToken = "LOGBOOK_CATEGORY_MONSTER";
				entry.color = ColorCatalog.GetColor(ColorCatalog.ColorIndex.HardDifficulty);
				entry.iconTexture = characterBody.portraitIcon;
				entry.extraData = characterBody;
				ModelLocator component = characterBody.GetComponent<ModelLocator>();
				GameObject modelPrefab;
				if (component == null)
				{
					modelPrefab = null;
				}
				else
				{
					Transform modelTransform = component.modelTransform;
					modelPrefab = ((modelTransform != null) ? modelTransform.gameObject : null);
				}
				entry.modelPrefab = modelPrefab;
				entry.getStatus = new Func<UserProfile, Entry, EntryStatus>(LogBookController.GetMonsterStatus);
				entry.getTooltipContent = new Func<UserProfile, Entry, EntryStatus, TooltipContent>(LogBookController.GetMonsterTooltipContent);
				entry.addEntries = new Action<PageBuilder>(PageBuilder.MonsterBody);
				entry.bgTexture = (characterBody.isChampion ? Resources.Load<Texture>("Textures/ItemIcons/BG/texTier3BGIcon") : Resources.Load<Texture>("Textures/ItemIcons/BG/texTier1BGIcon"));
				entry.isWIP = false;
				return entry;
			}).ToArray<Entry>();
			array[num] = categoryDef;
			int num2 = 2;
			CategoryDef categoryDef2 = new CategoryDef();
			categoryDef2.nameToken = "LOGBOOK_CATEGORY_STAGE";
			categoryDef2.iconPrefab = Resources.Load<GameObject>("Prefabs/UI/Logbook/StageEntryIcon");
			categoryDef2.entries = (from sceneDef in SceneCatalog.allSceneDefs
			where sceneDef.sceneType == SceneType.Stage || sceneDef.sceneType == SceneType.Intermission
			orderby sceneDef.stageOrder
			select new Entry
			{
				nameToken = sceneDef.nameToken,
				categoryTypeToken = "LOGBOOK_CATEGORY_STAGE",
				iconTexture = sceneDef.previewTexture,
				color = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Tier2ItemDark),
				getStatus = new Func<UserProfile, Entry, EntryStatus>(LogBookController.GetStageStatus),
				modelPrefab = sceneDef.dioramaPrefab,
				getTooltipContent = new Func<UserProfile, Entry, EntryStatus, TooltipContent>(LogBookController.GetStageTooltipContent),
				addEntries = new Action<PageBuilder>(PageBuilder.Stage),
				extraData = sceneDef,
				isWIP = Language.IsTokenInvalid(sceneDef.loreToken)
			}).ToArray<Entry>();
			array[num2] = categoryDef2;
			int num3 = 3;
			CategoryDef categoryDef3 = new CategoryDef();
			categoryDef3.nameToken = "LOGBOOK_CATEGORY_SURVIVOR";
			categoryDef3.iconPrefab = Resources.Load<GameObject>("Prefabs/UI/Logbook/SurvivorEntryIcon");
			categoryDef3.entries = (from characterBody in BodyCatalog.allBodyPrefabBodyBodyComponents
			where SurvivorCatalog.FindSurvivorDefFromBody(characterBody.gameObject) != null
			select characterBody).Select(delegate(CharacterBody characterBody)
			{
				Entry entry = new Entry();
				entry.nameToken = characterBody.baseNameToken;
				entry.categoryTypeToken = "LOGBOOK_CATEGORY_SURVIVOR";
				entry.color = ColorCatalog.GetColor(ColorCatalog.ColorIndex.NormalDifficulty);
				entry.iconTexture = characterBody.portraitIcon;
				entry.extraData = characterBody;
				ModelLocator component = characterBody.GetComponent<ModelLocator>();
				GameObject modelPrefab;
				if (component == null)
				{
					modelPrefab = null;
				}
				else
				{
					Transform modelTransform = component.modelTransform;
					modelPrefab = ((modelTransform != null) ? modelTransform.gameObject : null);
				}
				entry.modelPrefab = modelPrefab;
				entry.getStatus = new Func<UserProfile, Entry, EntryStatus>(LogBookController.GetSurvivorStatus);
				entry.getTooltipContent = new Func<UserProfile, Entry, EntryStatus, TooltipContent>(LogBookController.GetSurvivorTooltipContent);
				entry.addEntries = new Action<PageBuilder>(PageBuilder.SurvivorBody);
				entry.isWIP = false;
				return entry;
			}).ToArray<Entry>();
			array[num3] = categoryDef3;
			int num4 = 4;
			CategoryDef categoryDef4 = new CategoryDef();
			categoryDef4.nameToken = "LOGBOOK_CATEGORY_ACHIEVEMENTS";
			categoryDef4.iconPrefab = Resources.Load<GameObject>("Prefabs/UI/Logbook/AchievementEntryIcon");
			categoryDef4.entries = AchievementManager.allAchievementDefs.Select(delegate(AchievementDef achievementDef)
			{
				Entry entry = new Entry();
				entry.nameToken = achievementDef.nameToken;
				entry.categoryTypeToken = "LOGBOOK_CATEGORY_ACHIEVEMENT";
				entry.color = ColorCatalog.GetColor(ColorCatalog.ColorIndex.NormalDifficulty);
				Sprite achievedIcon = achievementDef.GetAchievedIcon();
				entry.iconTexture = ((achievedIcon != null) ? achievedIcon.texture : null);
				entry.extraData = achievementDef;
				entry.modelPrefab = null;
				entry.getStatus = new Func<UserProfile, Entry, EntryStatus>(LogBookController.GetAchievementStatus);
				return entry;
			}).ToArray<Entry>();
			categoryDef4.initializeElementGraphics = new Action<GameObject, Entry, EntryStatus, UserProfile>(CategoryDef.InitializeChallenge);
			categoryDef4.fullWidth = true;
			array[num4] = categoryDef4;
			return array;
		}

		// Token: 0x06002680 RID: 9856 RVA: 0x000A7CC0 File Offset: 0x000A5EC0
		private static void RegisterViewables(CategoryDef[] categoriesToGenerateFrom)
		{
			ViewablesCatalog.Node node = new ViewablesCatalog.Node("Logbook", true, null);
			foreach (CategoryDef categoryDef in LogBookController.categories)
			{
				ViewablesCatalog.Node node2 = new ViewablesCatalog.Node(categoryDef.nameToken, true, node);
				categoryDef.viewableNode = node2;
				Entry[] entries = categoryDef.entries;
				for (int j = 0; j < entries.Length; j++)
				{
					LogBookController.<>c__DisplayClass69_0 CS$<>8__locals1 = new LogBookController.<>c__DisplayClass69_0();
					CS$<>8__locals1.entry = entries[j];
					LogBookController.<>c__DisplayClass69_1 CS$<>8__locals2 = new LogBookController.<>c__DisplayClass69_1();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					string nameToken = CS$<>8__locals2.CS$<>8__locals1.entry.nameToken;
					if (!CS$<>8__locals2.CS$<>8__locals1.entry.isWIP && !(nameToken == "TOOLTIP_WIP_CONTENT_NAME"))
					{
						CS$<>8__locals2.entryNode = new ViewablesCatalog.Node(nameToken, false, node2);
						if ((CS$<>8__locals2.achievementDef = (CS$<>8__locals2.CS$<>8__locals1.entry.extraData as AchievementDef)) != null)
						{
							LogBookController.<>c__DisplayClass69_2 CS$<>8__locals3 = new LogBookController.<>c__DisplayClass69_2();
							CS$<>8__locals3.CS$<>8__locals2 = CS$<>8__locals2;
							CS$<>8__locals3.hasPrereq = !string.IsNullOrEmpty(CS$<>8__locals3.CS$<>8__locals2.achievementDef.prerequisiteAchievementIdentifier);
							CS$<>8__locals3.CS$<>8__locals2.entryNode.shouldShowUnviewed = new Func<UserProfile, bool>(CS$<>8__locals3.<RegisterViewables>g__Check|1);
						}
						else
						{
							CS$<>8__locals2.entryNode.shouldShowUnviewed = new Func<UserProfile, bool>(CS$<>8__locals2.<RegisterViewables>g__Check|0);
						}
						CS$<>8__locals2.CS$<>8__locals1.entry.viewableNode = CS$<>8__locals2.entryNode;
					}
				}
			}
			ViewablesCatalog.AddNodeToRoot(node);
		}

		// Token: 0x0400243A RID: 9274
		[Header("Navigation")]
		public GameObject navigationPanel;

		// Token: 0x0400243B RID: 9275
		public RectTransform categoryContainer;

		// Token: 0x0400243C RID: 9276
		public GameObject categorySpaceFiller;

		// Token: 0x0400243D RID: 9277
		public int categorySpaceFillerCount;

		// Token: 0x0400243E RID: 9278
		public Color spaceFillerColor;

		// Token: 0x0400243F RID: 9279
		private UIElementAllocator<MPButton> navigationCategoryButtonAllocator;

		// Token: 0x04002440 RID: 9280
		public RectTransform entryPageContainer;

		// Token: 0x04002441 RID: 9281
		public GameObject entryPagePrefab;

		// Token: 0x04002442 RID: 9282
		public RectTransform navigationPageIndicatorContainer;

		// Token: 0x04002443 RID: 9283
		public GameObject navigationPageIndicatorPrefab;

		// Token: 0x04002444 RID: 9284
		private UIElementAllocator<MPButton> navigationPageIndicatorAllocator;

		// Token: 0x04002445 RID: 9285
		public MPButton previousPageButton;

		// Token: 0x04002446 RID: 9286
		public MPButton nextPageButton;

		// Token: 0x04002447 RID: 9287
		public LanguageTextMeshController currentCategoryLabel;

		// Token: 0x04002448 RID: 9288
		private RectTransform categoryHightlightRect;

		// Token: 0x04002449 RID: 9289
		[Header("PageViewer")]
		public UnityEvent OnViewEntry;

		// Token: 0x0400244A RID: 9290
		public GameObject pageViewerPanel;

		// Token: 0x0400244B RID: 9291
		public MPButton pageViewerBackButton;

		// Token: 0x0400244C RID: 9292
		private EntityStateMachine stateMachine;

		// Token: 0x0400244D RID: 9293
		public static CategoryDef[] categories = Array.Empty<CategoryDef>();

		// Token: 0x0400244E RID: 9294
		public static ResourceAvailability availability = default(ResourceAvailability);

		// Token: 0x0400244F RID: 9295
		private LogBookController.NavigationPageInfo[] _availableNavigationPages = Array.Empty<LogBookController.NavigationPageInfo>();

		// Token: 0x04002450 RID: 9296
		private GameObject currentEntriesPageObject;

		// Token: 0x04002451 RID: 9297
		private int currentCategoryIndex;

		// Token: 0x04002452 RID: 9298
		private int desiredCategoryIndex;

		// Token: 0x04002453 RID: 9299
		private int currentPageIndex;

		// Token: 0x04002454 RID: 9300
		private int desiredPageIndex;

		// Token: 0x04002455 RID: 9301
		private bool goToEndOfNextCategory;

		// Token: 0x04002456 RID: 9302
		private LogBookController.NavigationPageInfo[] allNavigationPages;

		// Token: 0x04002457 RID: 9303
		private LogBookController.NavigationPageInfo[][] navigationPagesByCategory;

		// Token: 0x02000667 RID: 1639
		private class NavigationPageInfo
		{
			// Token: 0x04002458 RID: 9304
			public CategoryDef categoryDef;

			// Token: 0x04002459 RID: 9305
			public Entry[] entries;

			// Token: 0x0400245A RID: 9306
			public int index;

			// Token: 0x0400245B RID: 9307
			public int indexInCategory;
		}

		// Token: 0x02000668 RID: 1640
		private class LogBookState : EntityState
		{
			// Token: 0x06002684 RID: 9860 RVA: 0x000A7E90 File Offset: 0x000A6090
			public override void OnEnter()
			{
				base.OnEnter();
				this.logBookController = base.GetComponent<LogBookController>();
			}

			// Token: 0x06002685 RID: 9861 RVA: 0x000A7EA4 File Offset: 0x000A60A4
			public override void Update()
			{
				base.Update();
				this.unscaledAge += Time.unscaledDeltaTime;
			}

			// Token: 0x0400245C RID: 9308
			protected LogBookController logBookController;

			// Token: 0x0400245D RID: 9309
			protected float unscaledAge;
		}

		// Token: 0x02000669 RID: 1641
		private class FadeState : LogBookController.LogBookState
		{
			// Token: 0x06002687 RID: 9863 RVA: 0x000A7EBE File Offset: 0x000A60BE
			public override void OnEnter()
			{
				base.OnEnter();
				this.canvasGroup = base.GetComponent<CanvasGroup>();
				if (this.canvasGroup)
				{
					this.canvasGroup.alpha = 0f;
				}
			}

			// Token: 0x06002688 RID: 9864 RVA: 0x000A7EEF File Offset: 0x000A60EF
			public override void OnExit()
			{
				if (this.canvasGroup)
				{
					this.canvasGroup.alpha = this.endValue;
				}
				base.OnExit();
			}

			// Token: 0x06002689 RID: 9865 RVA: 0x000A7F18 File Offset: 0x000A6118
			public override void Update()
			{
				if (this.canvasGroup)
				{
					this.canvasGroup.alpha = this.unscaledAge / this.duration;
					if (this.canvasGroup.alpha >= 1f)
					{
						this.outer.SetNextState(new Idle());
					}
				}
				base.Update();
			}

			// Token: 0x0400245E RID: 9310
			private CanvasGroup canvasGroup;

			// Token: 0x0400245F RID: 9311
			public float duration = 0.5f;

			// Token: 0x04002460 RID: 9312
			public float endValue;
		}

		// Token: 0x0200066A RID: 1642
		private class ChangeEntriesPageState : LogBookController.LogBookState
		{
			// Token: 0x0600268B RID: 9867 RVA: 0x000A7F88 File Offset: 0x000A6188
			public override void OnEnter()
			{
				base.OnEnter();
				if (this.logBookController)
				{
					this.oldPageIndex = this.logBookController.currentPageIndex;
					this.oldPage = this.logBookController.currentEntriesPageObject;
					this.newPage = this.logBookController.BuildEntriesPage(this.newNavigationPageInfo);
					this.containerSize = this.logBookController.entryPageContainer.rect.size;
				}
				this.SetPagePositions(0f);
			}

			// Token: 0x0600268C RID: 9868 RVA: 0x000A800C File Offset: 0x000A620C
			public override void OnExit()
			{
				base.OnExit();
				EntityState.Destroy(this.oldPage);
				if (this.logBookController)
				{
					this.logBookController.currentEntriesPageObject = this.newPage;
					this.logBookController.currentPageIndex = this.newNavigationPageInfo.indexInCategory;
				}
			}

			// Token: 0x0600268D RID: 9869 RVA: 0x000A8060 File Offset: 0x000A6260
			private void SetPagePositions(float t)
			{
				Vector2 vector = new Vector2(this.containerSize.x * -this.moveDirection.x, this.containerSize.y * this.moveDirection.y);
				Vector2 vector2 = vector * t;
				if (this.oldPage)
				{
					this.oldPage.transform.localPosition = vector2;
				}
				if (this.newPage)
				{
					this.newPage.transform.localPosition = vector2 - vector;
				}
			}

			// Token: 0x0600268E RID: 9870 RVA: 0x000A80F8 File Offset: 0x000A62F8
			public override void Update()
			{
				base.Update();
				float num = Mathf.Clamp01(this.unscaledAge / this.duration);
				this.SetPagePositions(num);
				if (num == 1f)
				{
					this.outer.SetNextState(new Idle());
				}
			}

			// Token: 0x04002461 RID: 9313
			private int oldPageIndex;

			// Token: 0x04002462 RID: 9314
			public LogBookController.NavigationPageInfo newNavigationPageInfo;

			// Token: 0x04002463 RID: 9315
			public float duration = 0.1f;

			// Token: 0x04002464 RID: 9316
			public Vector2 moveDirection;

			// Token: 0x04002465 RID: 9317
			private GameObject oldPage;

			// Token: 0x04002466 RID: 9318
			private GameObject newPage;

			// Token: 0x04002467 RID: 9319
			private Vector2 oldPageTargetPosition;

			// Token: 0x04002468 RID: 9320
			private Vector2 newPageTargetPosition;

			// Token: 0x04002469 RID: 9321
			private Vector2 containerSize = Vector2.zero;
		}

		// Token: 0x0200066B RID: 1643
		private class ChangeCategoryState : LogBookController.LogBookState
		{
			// Token: 0x06002690 RID: 9872 RVA: 0x000A815C File Offset: 0x000A635C
			public override void OnEnter()
			{
				base.OnEnter();
				if (this.logBookController)
				{
					this.oldCategoryIndex = this.logBookController.currentCategoryIndex;
					this.oldPage = this.logBookController.currentEntriesPageObject;
					this.newNavigationPages = this.logBookController.GetCategoryPages(this.newCategoryIndex);
					this.destinationPageIndex = this.newNavigationPages[0].index;
					if (this.goToLastPage)
					{
						this.destinationPageIndex = this.newNavigationPages[this.newNavigationPages.Length - 1].index;
						Debug.LogFormat("goToLastPage=true destinationPageIndex={0}", new object[]
						{
							this.destinationPageIndex
						});
					}
					this.newNavigationPageInfo = this.logBookController.allNavigationPages[this.destinationPageIndex];
					this.newPage = this.logBookController.BuildEntriesPage(this.newNavigationPageInfo);
					this.containerSize = this.logBookController.entryPageContainer.rect.size;
					this.moveDirection = new Vector2(Mathf.Sign((float)(this.newCategoryIndex - this.oldCategoryIndex)), 0f);
				}
				this.SetPagePositions(0f);
			}

			// Token: 0x06002691 RID: 9873 RVA: 0x000A8288 File Offset: 0x000A6488
			public override void OnExit()
			{
				EntityState.Destroy(this.oldPage);
				if (this.logBookController)
				{
					this.logBookController.currentEntriesPageObject = this.newPage;
					this.logBookController.currentPageIndex = this.newNavigationPageInfo.indexInCategory;
					this.logBookController.desiredPageIndex = this.newNavigationPageInfo.indexInCategory;
					this.logBookController.currentCategoryIndex = this.newCategoryIndex;
					this.logBookController.availableNavigationPages = this.newNavigationPages;
					this.logBookController.currentCategoryLabel.token = LogBookController.categories[this.newCategoryIndex].nameToken;
					this.logBookController.categoryHightlightRect.SetParent(this.logBookController.navigationCategoryButtonAllocator.elements[this.newCategoryIndex].transform, false);
					this.logBookController.categoryHightlightRect.gameObject.SetActive(false);
					this.logBookController.categoryHightlightRect.gameObject.SetActive(true);
				}
				base.OnExit();
			}

			// Token: 0x06002692 RID: 9874 RVA: 0x000A8394 File Offset: 0x000A6594
			private void SetPagePositions(float t)
			{
				Vector2 vector = new Vector2(this.containerSize.x * -this.moveDirection.x, this.containerSize.y * this.moveDirection.y);
				Vector2 vector2 = vector * t;
				if (this.oldPage)
				{
					this.oldPage.transform.localPosition = vector2;
				}
				if (this.newPage)
				{
					this.newPage.transform.localPosition = vector2 - vector;
					if (this.frame == 4)
					{
						this.newPage.GetComponent<GridLayoutGroup>().enabled = false;
					}
				}
			}

			// Token: 0x06002693 RID: 9875 RVA: 0x000A8448 File Offset: 0x000A6648
			public override void Update()
			{
				base.Update();
				this.frame++;
				float num = Mathf.Clamp01(this.unscaledAge / this.duration);
				this.SetPagePositions(num);
				if (num == 1f)
				{
					this.outer.SetNextState(new Idle());
				}
			}

			// Token: 0x0400246A RID: 9322
			private int oldCategoryIndex;

			// Token: 0x0400246B RID: 9323
			public int newCategoryIndex;

			// Token: 0x0400246C RID: 9324
			public bool goToLastPage;

			// Token: 0x0400246D RID: 9325
			public float duration = 0.1f;

			// Token: 0x0400246E RID: 9326
			private GameObject oldPage;

			// Token: 0x0400246F RID: 9327
			private GameObject newPage;

			// Token: 0x04002470 RID: 9328
			private Vector2 oldPageTargetPosition;

			// Token: 0x04002471 RID: 9329
			private Vector2 newPageTargetPosition;

			// Token: 0x04002472 RID: 9330
			private Vector2 moveDirection;

			// Token: 0x04002473 RID: 9331
			private Vector2 containerSize = Vector2.zero;

			// Token: 0x04002474 RID: 9332
			private LogBookController.NavigationPageInfo[] newNavigationPages;

			// Token: 0x04002475 RID: 9333
			private int destinationPageIndex;

			// Token: 0x04002476 RID: 9334
			private LogBookController.NavigationPageInfo newNavigationPageInfo;

			// Token: 0x04002477 RID: 9335
			private int frame;
		}

		// Token: 0x0200066C RID: 1644
		private class EnterLogViewState : LogBookController.LogBookState
		{
			// Token: 0x06002695 RID: 9877 RVA: 0x000A84BC File Offset: 0x000A66BC
			public override void OnEnter()
			{
				base.OnEnter();
				this.flyingIcon = new GameObject("FlyingIcon", new Type[]
				{
					typeof(RectTransform),
					typeof(CanvasRenderer),
					typeof(RawImage)
				});
				this.flyingIconTransform = (RectTransform)this.flyingIcon.transform;
				this.flyingIconTransform.SetParent(this.logBookController.transform, false);
				this.flyingIconTransform.localScale = Vector3.one;
				this.flyingIconImage = this.flyingIconTransform.GetComponent<RawImage>();
				this.flyingIconImage.texture = this.iconTexture;
				Vector3[] array = new Vector3[4];
				this.startRectTransform.GetWorldCorners(array);
				this.startRect = this.GetRectRelativeToParent(array);
				this.midRect = new Rect(((RectTransform)this.logBookController.transform).rect.center, this.startRect.size);
				this.endRectTransform.GetWorldCorners(array);
				this.endRect = this.GetRectRelativeToParent(array);
				this.SetIconRect(this.startRect);
			}

			// Token: 0x06002696 RID: 9878 RVA: 0x000A85E3 File Offset: 0x000A67E3
			private void SetIconRect(Rect rect)
			{
				this.flyingIconTransform.position = rect.position;
				this.flyingIconTransform.offsetMin = rect.min;
				this.flyingIconTransform.offsetMax = rect.max;
			}

			// Token: 0x06002697 RID: 9879 RVA: 0x000A8620 File Offset: 0x000A6820
			private Rect GetRectRelativeToParent(Vector3[] corners)
			{
				for (int i = 0; i < 4; i++)
				{
					corners[i] = this.logBookController.transform.InverseTransformPoint(corners[i]);
				}
				return new Rect
				{
					xMin = corners[0].x,
					xMax = corners[2].x,
					yMin = corners[0].y,
					yMax = corners[2].y
				};
			}

			// Token: 0x06002698 RID: 9880 RVA: 0x000A86AC File Offset: 0x000A68AC
			private static Rect RectFromWorldCorners(Vector3[] corners)
			{
				return new Rect
				{
					xMin = corners[0].x,
					xMax = corners[2].x,
					yMin = corners[0].y,
					yMax = corners[2].y
				};
			}

			// Token: 0x06002699 RID: 9881 RVA: 0x000A8710 File Offset: 0x000A6910
			private static Rect LerpRect(Rect a, Rect b, float t)
			{
				return new Rect
				{
					min = Vector2.LerpUnclamped(a.min, b.min, t),
					max = Vector2.LerpUnclamped(a.max, b.max, t)
				};
			}

			// Token: 0x0600269A RID: 9882 RVA: 0x000A875C File Offset: 0x000A695C
			public override void OnExit()
			{
				EntityState.Destroy(this.flyingIcon);
				base.OnExit();
			}

			// Token: 0x0600269B RID: 9883 RVA: 0x000A8770 File Offset: 0x000A6970
			public override void Update()
			{
				base.Update();
				float num = Mathf.Min(this.unscaledAge / this.duration, 1f);
				if (num < 0.1f)
				{
					Util.Remap(num, 0f, 0.1f, 0f, 1f);
					this.SetIconRect(this.startRect);
				}
				if (num < 0.2f)
				{
					float t = Util.Remap(num, 0.1f, 0.2f, 0f, 1f);
					this.SetIconRect(LogBookController.EnterLogViewState.LerpRect(this.startRect, this.midRect, t));
					return;
				}
				if (num < 0.4f)
				{
					Util.Remap(num, 0.2f, 0.4f, 0f, 1f);
					this.SetIconRect(this.midRect);
					return;
				}
				if (num < 0.6f)
				{
					float t2 = Util.Remap(num, 0.4f, 0.6f, 0f, 1f);
					this.SetIconRect(LogBookController.EnterLogViewState.LerpRect(this.midRect, this.endRect, t2));
					return;
				}
				if (num < 1f)
				{
					float num2 = Util.Remap(num, 0.6f, 1f, 0f, 1f);
					this.flyingIconImage.color = new Color(1f, 1f, 1f, 1f - num2);
					this.SetIconRect(this.endRect);
					if (!this.submittedViewEntry)
					{
						this.submittedViewEntry = true;
						this.logBookController.ViewEntry(this.entry);
						return;
					}
				}
				else
				{
					this.outer.SetNextState(new Idle());
				}
			}

			// Token: 0x04002478 RID: 9336
			public Texture iconTexture;

			// Token: 0x04002479 RID: 9337
			public RectTransform startRectTransform;

			// Token: 0x0400247A RID: 9338
			public RectTransform endRectTransform;

			// Token: 0x0400247B RID: 9339
			public Entry entry;

			// Token: 0x0400247C RID: 9340
			private GameObject flyingIcon;

			// Token: 0x0400247D RID: 9341
			private RectTransform flyingIconTransform;

			// Token: 0x0400247E RID: 9342
			private RawImage flyingIconImage;

			// Token: 0x0400247F RID: 9343
			private float duration = 0.75f;

			// Token: 0x04002480 RID: 9344
			private Rect startRect;

			// Token: 0x04002481 RID: 9345
			private Rect midRect;

			// Token: 0x04002482 RID: 9346
			private Rect endRect;

			// Token: 0x04002483 RID: 9347
			private bool submittedViewEntry;
		}
	}
}
