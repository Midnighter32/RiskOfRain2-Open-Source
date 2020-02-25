using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using RoR2.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005BD RID: 1469
	[RequireComponent(typeof(MPEventSystemProvider))]
	public class GameEndReportPanelController : MonoBehaviour
	{
		// Token: 0x170003A6 RID: 934
		// (get) Token: 0x060022D1 RID: 8913 RVA: 0x000973B0 File Offset: 0x000955B0
		// (set) Token: 0x060022D2 RID: 8914 RVA: 0x000973B8 File Offset: 0x000955B8
		public GameEndReportPanelController.DisplayData displayData { get; private set; }

		// Token: 0x060022D3 RID: 8915 RVA: 0x000973C1 File Offset: 0x000955C1
		private void Awake()
		{
			this.playerNavigationController.onPageChangeSubmitted += this.OnPlayerNavigationControllerPageChangeSubmitted;
		}

		// Token: 0x060022D4 RID: 8916 RVA: 0x000973DC File Offset: 0x000955DC
		private void SetFlashAnimationValue(float t)
		{
			if (t == this.lastFlashAnimationValue)
			{
				return;
			}
			this.lastFlashAnimationValue = t;
			this.flashOverlay.color = new Color(1f, 1f, 1f, this.flashCurve.Evaluate(t));
			this.canvasGroup.alpha = this.alphaCurve.Evaluate(t);
			if (t >= 1f)
			{
				this.flashOverlay.enabled = false;
			}
		}

		// Token: 0x060022D5 RID: 8917 RVA: 0x00097450 File Offset: 0x00095650
		private void Update()
		{
			this.flashStopwatch += Time.deltaTime;
			this.SetFlashAnimationValue(Mathf.Clamp01(this.flashStopwatch / this.flashDuration));
		}

		// Token: 0x060022D6 RID: 8918 RVA: 0x0009747C File Offset: 0x0009567C
		private void AllocateStatStrips(int count)
		{
			while (this.statStrips.Count > count)
			{
				int index = this.statStrips.Count - 1;
				UnityEngine.Object.Destroy(this.statStrips[index].gameObject);
				this.statStrips.RemoveAt(index);
			}
			while (this.statStrips.Count < count)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.statStripPrefab, this.statContentArea);
				gameObject.SetActive(true);
				this.statStrips.Add(gameObject);
			}
			if (this.statsAnimateImageAlpha)
			{
				Image[] array = new Image[this.statStrips.Count];
				for (int i = 0; i < this.statStrips.Count; i++)
				{
					array[i] = this.statStrips[i].GetComponent<Image>();
				}
				this.statsAnimateImageAlpha.ResetStopwatch();
				this.statsAnimateImageAlpha.images = array;
			}
		}

		// Token: 0x060022D7 RID: 8919 RVA: 0x00097560 File Offset: 0x00095760
		private void AllocateUnlockStrips(int count)
		{
			while (this.unlockStrips.Count > count)
			{
				int index = this.unlockStrips.Count - 1;
				UnityEngine.Object.Destroy(this.unlockStrips[index].gameObject);
				this.unlockStrips.RemoveAt(index);
			}
			while (this.unlockStrips.Count < count)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.unlockStripPrefab, this.unlockContentArea);
				gameObject.SetActive(true);
				this.unlockStrips.Add(gameObject);
			}
		}

		// Token: 0x060022D8 RID: 8920 RVA: 0x000975E4 File Offset: 0x000957E4
		public void SetDisplayData(GameEndReportPanelController.DisplayData newDisplayData)
		{
			if (this.displayData.Equals(newDisplayData))
			{
				return;
			}
			this.displayData = newDisplayData;
			if (this.resultLabel)
			{
				GameResultType gameResultType = GameResultType.Unknown;
				if (this.displayData.runReport != null)
				{
					gameResultType = this.displayData.runReport.gameResultType;
				}
				string token;
				if (gameResultType != GameResultType.Lost)
				{
					if (gameResultType != GameResultType.Won)
					{
						token = "GAME_RESULT_UNKNOWN";
					}
					else
					{
						token = "GAME_RESULT_WON";
					}
				}
				else
				{
					token = "GAME_RESULT_LOST";
				}
				this.resultLabel.text = Language.GetString(token);
			}
			DifficultyIndex difficultyIndex = DifficultyIndex.Invalid;
			if (this.displayData.runReport != null)
			{
				difficultyIndex = this.displayData.runReport.ruleBook.FindDifficulty();
			}
			DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(difficultyIndex);
			if (this.selectedDifficultyImage)
			{
				this.selectedDifficultyImage.sprite = ((difficultyDef != null) ? difficultyDef.GetIconSprite() : null);
			}
			if (this.selectedDifficultyLabel)
			{
				this.selectedDifficultyLabel.token = ((difficultyDef != null) ? difficultyDef.nameToken : null);
			}
			RunReport runReport = this.displayData.runReport;
			RunReport.PlayerInfo playerInfo = (runReport != null) ? runReport.GetPlayerInfoSafe(this.displayData.playerIndex) : null;
			this.SetPlayerInfo(playerInfo);
			RunReport runReport2 = this.displayData.runReport;
			int num = (runReport2 != null) ? runReport2.playerInfoCount : 0;
			this.playerNavigationController.gameObject.SetActive(num > 1);
			this.playerNavigationController.SetDisplayData(new CarouselNavigationController.DisplayData(num, this.displayData.playerIndex));
			ReadOnlyCollection<MPButton> elements = this.playerNavigationController.buttonAllocator.elements;
			for (int i = 0; i < elements.Count; i++)
			{
				MPButton mpbutton = elements[i];
				RunReport.PlayerInfo playerInfo2 = this.displayData.runReport.GetPlayerInfo(i);
				CharacterBody bodyPrefabBodyComponent = BodyCatalog.GetBodyPrefabBodyComponent(playerInfo2.bodyIndex);
				Texture texture = bodyPrefabBodyComponent ? bodyPrefabBodyComponent.portraitIcon : null;
				mpbutton.GetComponentInChildren<RawImage>().texture = texture;
				mpbutton.GetComponent<TooltipProvider>().SetContent(TooltipProvider.GetPlayerNameTooltipContent(playerInfo2.name));
			}
			this.selectedPlayerEffectRoot.transform.SetParent(this.playerNavigationController.buttonAllocator.elements[this.displayData.playerIndex].transform);
			this.selectedPlayerEffectRoot.gameObject.SetActive(false);
			this.selectedPlayerEffectRoot.gameObject.SetActive(true);
			this.selectedPlayerEffectRoot.offsetMin = Vector2.zero;
			this.selectedPlayerEffectRoot.offsetMax = Vector2.zero;
			this.selectedPlayerEffectRoot.localScale = Vector3.one;
		}

		// Token: 0x060022D9 RID: 8921 RVA: 0x00097868 File Offset: 0x00095A68
		private void OnPlayerNavigationControllerPageChangeSubmitted(int newPage)
		{
			GameEndReportPanelController.DisplayData displayData = this.displayData;
			displayData.playerIndex = newPage;
			this.SetDisplayData(displayData);
		}

		// Token: 0x060022DA RID: 8922 RVA: 0x0009788C File Offset: 0x00095A8C
		private void SetPlayerInfo([CanBeNull] RunReport.PlayerInfo playerInfo)
		{
			ulong num = 0UL;
			if (playerInfo != null)
			{
				StatSheet statSheet = playerInfo.statSheet;
				this.AllocateStatStrips(this.statsToDisplay.Length);
				for (int i = 0; i < this.statsToDisplay.Length; i++)
				{
					string text = this.statsToDisplay[i];
					StatDef statDef = StatDef.Find(text);
					if (statDef == null)
					{
						Debug.LogWarningFormat("GameEndReportPanelController.SetStatSheet: Could not find stat def \"{0}\".", new object[]
						{
							text
						});
					}
					else
					{
						this.AssignStatToStrip(statSheet, statDef, this.statStrips[i]);
						num += statSheet.GetStatPointValue(statDef);
					}
				}
				int unlockableCount = statSheet.GetUnlockableCount();
				int num2 = 0;
				for (int j = 0; j < unlockableCount; j++)
				{
					if (!statSheet.GetUnlockable(j).hidden)
					{
						num2++;
					}
				}
				this.AllocateUnlockStrips(num2);
				int num3 = 0;
				for (int k = 0; k < unlockableCount; k++)
				{
					UnlockableDef unlockable = statSheet.GetUnlockable(k);
					if (!unlockable.hidden)
					{
						this.AssignUnlockToStrip(unlockable, this.unlockStrips[num3]);
						num3++;
					}
				}
				if (this.itemInventoryDisplay)
				{
					this.itemInventoryDisplay.SetItems(playerInfo.itemAcquisitionOrder, playerInfo.itemAcquisitionOrder.Length, playerInfo.itemStacks);
					this.itemInventoryDisplay.UpdateDisplay();
				}
			}
			else
			{
				this.AllocateStatStrips(0);
				this.AllocateUnlockStrips(0);
				if (this.itemInventoryDisplay)
				{
					this.itemInventoryDisplay.ResetItems();
				}
			}
			string @string = Language.GetString("STAT_POINTS_FORMAT");
			this.totalPointsLabel.text = string.Format(@string, TextSerialization.ToStringNumeric(num));
			GameObject gameObject = null;
			if (playerInfo != null)
			{
				gameObject = BodyCatalog.GetBodyPrefab(playerInfo.bodyIndex);
			}
			string arg = "";
			Texture texture = null;
			if (gameObject)
			{
				texture = gameObject.GetComponent<CharacterBody>().portraitIcon;
				arg = Language.GetString(gameObject.GetComponent<CharacterBody>().baseNameToken);
			}
			string string2 = Language.GetString("STAT_CLASS_NAME_FORMAT");
			this.playerBodyLabel.text = string.Format(string2, arg);
			this.playerBodyPortraitImage.texture = texture;
			GameObject gameObject2 = null;
			if (playerInfo != null)
			{
				gameObject2 = BodyCatalog.GetBodyPrefab(playerInfo.killerBodyIndex);
			}
			string string3 = Language.GetString("UNIDENTIFIED");
			Texture texture2 = Resources.Load<Texture>("Textures/MiscIcons/texMysteryIcon");
			if (gameObject2)
			{
				Texture portraitIcon = gameObject2.GetComponent<CharacterBody>().portraitIcon;
				string baseNameToken = gameObject2.GetComponent<CharacterBody>().baseNameToken;
				if (portraitIcon != null)
				{
					texture2 = portraitIcon;
				}
				if (!Language.IsTokenInvalid(baseNameToken))
				{
					string3 = Language.GetString(gameObject2.GetComponent<CharacterBody>().baseNameToken);
				}
			}
			string string4 = Language.GetString("STAT_KILLER_NAME_FORMAT");
			this.killerBodyLabel.text = string.Format(string4, string3);
			this.killerBodyPortraitImage.texture = texture2;
			this.killerPanelObject.SetActive(true);
		}

		// Token: 0x060022DB RID: 8923 RVA: 0x00097B3C File Offset: 0x00095D3C
		private void AssignStatToStrip([CanBeNull] StatSheet srcStatSheet, [NotNull] StatDef statDef, GameObject destStatStrip)
		{
			string arg = "0";
			ulong value = 0UL;
			if (srcStatSheet != null)
			{
				arg = srcStatSheet.GetStatDisplayValue(statDef);
				value = srcStatSheet.GetStatPointValue(statDef);
			}
			string @string = Language.GetString(statDef.displayToken);
			string text = string.Format(Language.GetString("STAT_NAME_VALUE_FORMAT"), @string, arg);
			destStatStrip.transform.Find("StatNameLabel").GetComponent<TextMeshProUGUI>().text = text;
			string string2 = Language.GetString("STAT_POINTS_FORMAT");
			destStatStrip.transform.Find("PointValueLabel").GetComponent<TextMeshProUGUI>().text = string.Format(string2, TextSerialization.ToStringNumeric(value));
		}

		// Token: 0x060022DC RID: 8924 RVA: 0x00097BD4 File Offset: 0x00095DD4
		private void AssignUnlockToStrip(UnlockableDef unlockableDef, GameObject destUnlockableStrip)
		{
			AchievementDef achievementDefFromUnlockable = AchievementManager.GetAchievementDefFromUnlockable(unlockableDef.name);
			Texture texture = null;
			string @string = Language.GetString("TOOLTIP_UNLOCK_GENERIC_NAME");
			string string2 = Language.GetString("TOOLTIP_UNLOCK_GENERIC_NAME");
			if (unlockableDef.name.Contains("Items."))
			{
				@string = Language.GetString("TOOLTIP_UNLOCK_ITEM_NAME");
				string2 = Language.GetString("TOOLTIP_UNLOCK_ITEM_DESCRIPTION");
			}
			else if (unlockableDef.name.Contains("Logs."))
			{
				@string = Language.GetString("TOOLTIP_UNLOCK_LOG_NAME");
				string2 = Language.GetString("TOOLTIP_UNLOCK_LOG_DESCRIPTION");
			}
			else if (unlockableDef.name.Contains("Characters."))
			{
				@string = Language.GetString("TOOLTIP_UNLOCK_SURVIVOR_NAME");
				string2 = Language.GetString("TOOLTIP_UNLOCK_SURVIVOR_DESCRIPTION");
			}
			string string3;
			if (achievementDefFromUnlockable != null)
			{
				texture = Resources.Load<Texture>(achievementDefFromUnlockable.iconPath);
				string3 = Language.GetString(achievementDefFromUnlockable.nameToken);
			}
			else
			{
				string3 = Language.GetString(unlockableDef.nameToken);
			}
			if (texture != null)
			{
				destUnlockableStrip.transform.Find("IconImage").GetComponent<RawImage>().texture = texture;
			}
			destUnlockableStrip.transform.Find("NameLabel").GetComponent<TextMeshProUGUI>().text = string3;
			destUnlockableStrip.GetComponent<TooltipProvider>().overrideTitleText = @string;
			destUnlockableStrip.GetComponent<TooltipProvider>().overrideBodyText = string2;
		}

		// Token: 0x0400206E RID: 8302
		[Header("Result")]
		[Tooltip("The TextMeshProUGUI component to use to display the result of the game: Win or Loss")]
		public TextMeshProUGUI resultLabel;

		// Token: 0x0400206F RID: 8303
		[Tooltip("The Image component to use to display the selected difficulty of the run.")]
		[Header("Run Settings")]
		public Image selectedDifficultyImage;

		// Token: 0x04002070 RID: 8304
		[Tooltip("The LanguageTextMeshController component to use to display the selected difficulty of the run.")]
		public LanguageTextMeshController selectedDifficultyLabel;

		// Token: 0x04002071 RID: 8305
		[Tooltip("A list of StatDef names to display in the stats section.")]
		[Header("Stats")]
		public string[] statsToDisplay;

		// Token: 0x04002072 RID: 8306
		[Tooltip("Prefab to be used for stat display.")]
		public GameObject statStripPrefab;

		// Token: 0x04002073 RID: 8307
		[Tooltip("The RectTransform in which to build the stat strips.")]
		public RectTransform statContentArea;

		// Token: 0x04002074 RID: 8308
		[Tooltip("The TextMeshProUGUI component used to display the total points.")]
		public TextMeshProUGUI totalPointsLabel;

		// Token: 0x04002075 RID: 8309
		[Tooltip("The component in charge of swiping over all elements over time.")]
		public AnimateImageAlpha statsAnimateImageAlpha;

		// Token: 0x04002076 RID: 8310
		[Tooltip("Prefab to be used for unlock display.")]
		[Header("Unlocks")]
		public GameObject unlockStripPrefab;

		// Token: 0x04002077 RID: 8311
		[Tooltip("The RectTransform in which to build the unlock strips.")]
		public RectTransform unlockContentArea;

		// Token: 0x04002078 RID: 8312
		[Tooltip("The inventory display controller.")]
		[Header("Items")]
		public ItemInventoryDisplay itemInventoryDisplay;

		// Token: 0x04002079 RID: 8313
		[Header("Intro Flash Animation")]
		[Tooltip("How long the flash animation takes.")]
		public float flashDuration;

		// Token: 0x0400207A RID: 8314
		[Tooltip("A white panel whose alpha will be animated from 0->1->0 when this panel is shown to simulate a flash effect.")]
		public Image flashOverlay;

		// Token: 0x0400207B RID: 8315
		[Tooltip("The alpha curve for flashOverlay")]
		public AnimationCurve flashCurve;

		// Token: 0x0400207C RID: 8316
		[Tooltip("The CanvasGroup which controls the alpha of this entire panel.")]
		public CanvasGroup canvasGroup;

		// Token: 0x0400207D RID: 8317
		[Tooltip("The alpha curve for this panel during its appearance animation.")]
		public AnimationCurve alphaCurve;

		// Token: 0x0400207E RID: 8318
		[Header("Player Info")]
		[Tooltip("The RawImage component to use to display the player character's portrait.")]
		public RawImage playerBodyPortraitImage;

		// Token: 0x0400207F RID: 8319
		[Tooltip("The TextMeshProUGUI component to use to display the player character's body name.")]
		public TextMeshProUGUI playerBodyLabel;

		// Token: 0x04002080 RID: 8320
		[Header("Killer Info")]
		[Tooltip("The RawImage component to use to display the killer character's portrait.")]
		public RawImage killerBodyPortraitImage;

		// Token: 0x04002081 RID: 8321
		[Tooltip("The TextMeshProUGUI component to use to display the killer character's body name.")]
		public TextMeshProUGUI killerBodyLabel;

		// Token: 0x04002082 RID: 8322
		[Tooltip("The GameObject used as the panel for the killer information. This is used to disable the killer panel when the player has won the game.")]
		public GameObject killerPanelObject;

		// Token: 0x04002083 RID: 8323
		[Header("Navigation")]
		public MPButton continueButton;

		// Token: 0x04002084 RID: 8324
		public CarouselNavigationController playerNavigationController;

		// Token: 0x04002085 RID: 8325
		public RectTransform selectedPlayerEffectRoot;

		// Token: 0x04002086 RID: 8326
		private float lastFlashAnimationValue = -1f;

		// Token: 0x04002087 RID: 8327
		private float flashStopwatch;

		// Token: 0x04002088 RID: 8328
		private readonly List<GameObject> statStrips = new List<GameObject>();

		// Token: 0x04002089 RID: 8329
		private readonly List<GameObject> unlockStrips = new List<GameObject>();

		// Token: 0x020005BE RID: 1470
		public struct DisplayData : IEquatable<GameEndReportPanelController.DisplayData>
		{
			// Token: 0x060022DE RID: 8926 RVA: 0x00097D2F File Offset: 0x00095F2F
			public bool Equals(GameEndReportPanelController.DisplayData other)
			{
				return object.Equals(this.runReport, other.runReport) && this.playerIndex == other.playerIndex;
			}

			// Token: 0x060022DF RID: 8927 RVA: 0x00097D54 File Offset: 0x00095F54
			public override bool Equals(object obj)
			{
				if (obj == null)
				{
					return false;
				}
				if (obj is GameEndReportPanelController.DisplayData)
				{
					GameEndReportPanelController.DisplayData other = (GameEndReportPanelController.DisplayData)obj;
					return this.Equals(other);
				}
				return false;
			}

			// Token: 0x060022E0 RID: 8928 RVA: 0x00097D80 File Offset: 0x00095F80
			public override int GetHashCode()
			{
				return ((-1418150836 * -1521134295 + base.GetHashCode()) * -1521134295 + EqualityComparer<RunReport>.Default.GetHashCode(this.runReport)) * -1521134295 + this.playerIndex.GetHashCode();
			}

			// Token: 0x0400208A RID: 8330
			[CanBeNull]
			public RunReport runReport;

			// Token: 0x0400208B RID: 8331
			public int playerIndex;
		}
	}
}
