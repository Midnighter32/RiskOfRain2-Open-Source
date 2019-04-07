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
	// Token: 0x020005DE RID: 1502
	[RequireComponent(typeof(MPEventSystemProvider))]
	public class GameEndReportPanelController : MonoBehaviour
	{
		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x060021A1 RID: 8609 RVA: 0x0009E40C File Offset: 0x0009C60C
		// (set) Token: 0x060021A2 RID: 8610 RVA: 0x0009E414 File Offset: 0x0009C614
		public GameEndReportPanelController.DisplayData displayData { get; private set; }

		// Token: 0x060021A3 RID: 8611 RVA: 0x0009E41D File Offset: 0x0009C61D
		private void Awake()
		{
			this.playerNavigationController.onPageChangeSubmitted += this.OnPlayerNavigationControllerPageChangeSubmitted;
		}

		// Token: 0x060021A4 RID: 8612 RVA: 0x0009E438 File Offset: 0x0009C638
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

		// Token: 0x060021A5 RID: 8613 RVA: 0x0009E4AC File Offset: 0x0009C6AC
		private void Update()
		{
			this.flashStopwatch += Time.deltaTime;
			this.SetFlashAnimationValue(Mathf.Clamp01(this.flashStopwatch / this.flashDuration));
		}

		// Token: 0x060021A6 RID: 8614 RVA: 0x0009E4D8 File Offset: 0x0009C6D8
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

		// Token: 0x060021A7 RID: 8615 RVA: 0x0009E5BC File Offset: 0x0009C7BC
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

		// Token: 0x060021A8 RID: 8616 RVA: 0x0009E640 File Offset: 0x0009C840
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

		// Token: 0x060021A9 RID: 8617 RVA: 0x0009E84C File Offset: 0x0009CA4C
		private void OnPlayerNavigationControllerPageChangeSubmitted(int newPage)
		{
			GameEndReportPanelController.DisplayData displayData = this.displayData;
			displayData.playerIndex = newPage;
			this.SetDisplayData(displayData);
		}

		// Token: 0x060021AA RID: 8618 RVA: 0x0009E870 File Offset: 0x0009CA70
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
			this.totalPointsLabel.text = string.Format(@string, num);
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
			string arg2 = "";
			Texture texture2 = null;
			if (gameObject2)
			{
				texture2 = gameObject2.GetComponent<CharacterBody>().portraitIcon;
				arg2 = Language.GetString(gameObject2.GetComponent<CharacterBody>().baseNameToken);
			}
			string string3 = Language.GetString("STAT_KILLER_NAME_FORMAT");
			this.killerBodyLabel.text = string.Format(string3, arg2);
			this.killerBodyPortraitImage.texture = texture2;
			this.killerPanelObject.SetActive(gameObject2);
		}

		// Token: 0x060021AB RID: 8619 RVA: 0x0009EAF8 File Offset: 0x0009CCF8
		private void AssignStatToStrip([CanBeNull] StatSheet srcStatSheet, [NotNull] StatDef statDef, GameObject destStatStrip)
		{
			string arg = "0";
			ulong num = 0UL;
			if (srcStatSheet != null)
			{
				arg = srcStatSheet.GetStatDisplayValue(statDef);
				num = srcStatSheet.GetStatPointValue(statDef);
			}
			string @string = Language.GetString(statDef.displayToken);
			string text = string.Format(Language.GetString("STAT_NAME_VALUE_FORMAT"), @string, arg);
			destStatStrip.transform.Find("StatNameLabel").GetComponent<TextMeshProUGUI>().text = text;
			string string2 = Language.GetString("STAT_POINTS_FORMAT");
			destStatStrip.transform.Find("PointValueLabel").GetComponent<TextMeshProUGUI>().text = string.Format(string2, num);
		}

		// Token: 0x060021AC RID: 8620 RVA: 0x0009EB90 File Offset: 0x0009CD90
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

		// Token: 0x0400245F RID: 9311
		[Tooltip("The TextMeshProUGUI component to use to display the result of the game: Win or Loss")]
		[Header("Result")]
		public TextMeshProUGUI resultLabel;

		// Token: 0x04002460 RID: 9312
		[Tooltip("A list of StatDef names to display in the stats section.")]
		[Header("Stats")]
		public string[] statsToDisplay;

		// Token: 0x04002461 RID: 9313
		[Tooltip("Prefab to be used for stat display.")]
		public GameObject statStripPrefab;

		// Token: 0x04002462 RID: 9314
		[Tooltip("The RectTransform in which to build the stat strips.")]
		public RectTransform statContentArea;

		// Token: 0x04002463 RID: 9315
		[Tooltip("The TextMeshProUGUI component used to display the total points.")]
		public TextMeshProUGUI totalPointsLabel;

		// Token: 0x04002464 RID: 9316
		[Tooltip("The component in charge of swiping over all elements over time.")]
		public AnimateImageAlpha statsAnimateImageAlpha;

		// Token: 0x04002465 RID: 9317
		[Header("Unlocks")]
		[Tooltip("Prefab to be used for unlock display.")]
		public GameObject unlockStripPrefab;

		// Token: 0x04002466 RID: 9318
		[Tooltip("The RectTransform in which to build the unlock strips.")]
		public RectTransform unlockContentArea;

		// Token: 0x04002467 RID: 9319
		[Tooltip("The inventory display controller.")]
		[Header("Items")]
		public ItemInventoryDisplay itemInventoryDisplay;

		// Token: 0x04002468 RID: 9320
		[Tooltip("How long the flash animation takes.")]
		[Header("Intro Flash Animation")]
		public float flashDuration;

		// Token: 0x04002469 RID: 9321
		[Tooltip("A white panel whose alpha will be animated from 0->1->0 when this panel is shown to simulate a flash effect.")]
		public Image flashOverlay;

		// Token: 0x0400246A RID: 9322
		[Tooltip("The alpha curve for flashOverlay")]
		public AnimationCurve flashCurve;

		// Token: 0x0400246B RID: 9323
		[Tooltip("The CanvasGroup which controls the alpha of this entire panel.")]
		public CanvasGroup canvasGroup;

		// Token: 0x0400246C RID: 9324
		[Tooltip("The alpha curve for this panel during its appearance animation.")]
		public AnimationCurve alphaCurve;

		// Token: 0x0400246D RID: 9325
		[Header("Player Info")]
		[Tooltip("The RawImage component to use to display the player character's portrait.")]
		public RawImage playerBodyPortraitImage;

		// Token: 0x0400246E RID: 9326
		[Tooltip("The TextMeshProUGUI component to use to display the player character's body name.")]
		public TextMeshProUGUI playerBodyLabel;

		// Token: 0x0400246F RID: 9327
		[Tooltip("The RawImage component to use to display the killer character's portrait.")]
		[Header("Killer Info")]
		public RawImage killerBodyPortraitImage;

		// Token: 0x04002470 RID: 9328
		[Tooltip("The TextMeshProUGUI component to use to display the killer character's body name.")]
		public TextMeshProUGUI killerBodyLabel;

		// Token: 0x04002471 RID: 9329
		[Tooltip("The GameObject used as the panel for the killer information. This is used to disable the killer panel when the player has won the game.")]
		public GameObject killerPanelObject;

		// Token: 0x04002472 RID: 9330
		[Header("Navigation")]
		public MPButton continueButton;

		// Token: 0x04002473 RID: 9331
		public CarouselNavigationController playerNavigationController;

		// Token: 0x04002474 RID: 9332
		public RectTransform selectedPlayerEffectRoot;

		// Token: 0x04002475 RID: 9333
		private float lastFlashAnimationValue = -1f;

		// Token: 0x04002476 RID: 9334
		private float flashStopwatch;

		// Token: 0x04002477 RID: 9335
		private readonly List<GameObject> statStrips = new List<GameObject>();

		// Token: 0x04002478 RID: 9336
		private readonly List<GameObject> unlockStrips = new List<GameObject>();

		// Token: 0x020005DF RID: 1503
		public struct DisplayData : IEquatable<GameEndReportPanelController.DisplayData>
		{
			// Token: 0x060021AE RID: 8622 RVA: 0x0009ECEB File Offset: 0x0009CEEB
			public bool Equals(GameEndReportPanelController.DisplayData other)
			{
				return object.Equals(this.runReport, other.runReport) && this.playerIndex == other.playerIndex;
			}

			// Token: 0x060021AF RID: 8623 RVA: 0x0009ED10 File Offset: 0x0009CF10
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

			// Token: 0x060021B0 RID: 8624 RVA: 0x0009ED3C File Offset: 0x0009CF3C
			public override int GetHashCode()
			{
				return ((-1418150836 * -1521134295 + base.GetHashCode()) * -1521134295 + EqualityComparer<RunReport>.Default.GetHashCode(this.runReport)) * -1521134295 + this.playerIndex.GetHashCode();
			}

			// Token: 0x04002479 RID: 9337
			[CanBeNull]
			public RunReport runReport;

			// Token: 0x0400247A RID: 9338
			public int playerIndex;
		}
	}
}
