using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005B3 RID: 1459
	[ExecuteInEditMode]
	public class DifficultyBarController : MonoBehaviour
	{
		// Token: 0x060022A0 RID: 8864 RVA: 0x00095F60 File Offset: 0x00094160
		private static Color ColorMultiplySaturationAndValue(ref Color col, float saturationMultiplier, float valueMultiplier)
		{
			float h;
			float num;
			float num2;
			Color.RGBToHSV(col, out h, out num, out num2);
			return Color.HSVToRGB(h, num * saturationMultiplier, num2 * valueMultiplier);
		}

		// Token: 0x060022A1 RID: 8865 RVA: 0x00095F8C File Offset: 0x0009418C
		private void OnCurrentSegmentIndexChanged(int newSegmentIndex)
		{
			if (!Application.isPlaying)
			{
				return;
			}
			int num = newSegmentIndex - 1;
			float width = this.viewPort.rect.width;
			int i = 0;
			int num2 = this.images.Length - 1;
			while (i < num2)
			{
				Image image = this.images[i];
				RectTransform rectTransform = image.rectTransform;
				bool enabled = rectTransform.offsetMax.x + this.scrollX >= 0f && rectTransform.offsetMin.x + this.scrollX <= width;
				image.enabled = enabled;
				i++;
			}
			int num3 = this.images.Length - 1;
			Image image2 = this.images[num3];
			bool enabled2 = image2.rectTransform.offsetMax.x + this.scrollX >= 0f;
			image2.enabled = enabled2;
			for (int j = 0; j <= num; j++)
			{
				this.images[j].color = DifficultyBarController.ColorMultiplySaturationAndValue(ref this.segmentDefs[j].color, this.pastSaturationMultiplier, this.pastValueMultiplier);
				this.labels[j].color = this.pastLabelColor;
			}
			for (int k = newSegmentIndex + 1; k < this.images.Length; k++)
			{
				this.images[k].color = DifficultyBarController.ColorMultiplySaturationAndValue(ref this.segmentDefs[k].color, this.upcomingSaturationMultiplier, this.upcomingValueMultiplier);
				this.labels[k].color = this.upcomingLabelColor;
			}
			Image image3 = (num != -1) ? this.images[num] : null;
			Image image4 = (newSegmentIndex != -1) ? this.images[newSegmentIndex] : null;
			TextMeshProUGUI textMeshProUGUI = (newSegmentIndex != -1) ? this.labels[newSegmentIndex] : null;
			if (image3)
			{
				this.playingAnimations.Add(new DifficultyBarController.SegmentImageAnimation
				{
					age = 0f,
					duration = this.fadeAnimationDuration,
					segmentImage = image3,
					colorCurve = this.fadeAnimationCurve,
					color0 = this.segmentDefs[num].color,
					color1 = DifficultyBarController.ColorMultiplySaturationAndValue(ref this.segmentDefs[num].color, this.pastSaturationMultiplier, this.pastValueMultiplier)
				});
			}
			if (image4)
			{
				this.playingAnimations.Add(new DifficultyBarController.SegmentImageAnimation
				{
					age = 0f,
					duration = this.flashAnimationDuration,
					segmentImage = image4,
					colorCurve = this.flashAnimationCurve,
					color0 = DifficultyBarController.ColorMultiplySaturationAndValue(ref this.segmentDefs[newSegmentIndex].color, this.currentSaturationMultiplier, this.currentValueMultiplier),
					color1 = Color.white
				});
			}
			if (textMeshProUGUI)
			{
				textMeshProUGUI.color = this.currentLabelColor;
			}
		}

		// Token: 0x060022A2 RID: 8866 RVA: 0x00096258 File Offset: 0x00094458
		private void SetSegmentScroll(float segmentScroll)
		{
			float num = (float)(this.segmentDefs.Length + 2);
			if (segmentScroll > num)
			{
				segmentScroll = num - 1f + (segmentScroll - Mathf.Floor(segmentScroll));
			}
			this.scrollXRaw = (segmentScroll - 1f) * -this.elementWidth;
			this.scrollX = Mathf.Floor(this.scrollXRaw);
			int num2 = this.currentSegmentIndex;
			this.currentSegmentIndex = Mathf.Clamp(Mathf.FloorToInt(segmentScroll), 0, this.segmentContainer.childCount - 1);
			if (num2 != this.currentSegmentIndex)
			{
				this.OnCurrentSegmentIndexChanged(this.currentSegmentIndex);
			}
			Vector2 offsetMin = this.segmentContainer.offsetMin;
			offsetMin.x = this.scrollX;
			this.segmentContainer.offsetMin = offsetMin;
			if (this.segmentContainer && this.segmentContainer.childCount > 0)
			{
				int num3 = this.segmentContainer.childCount - 1;
				RectTransform rectTransform = (RectTransform)this.segmentContainer.GetChild(num3);
				RectTransform rectTransform2 = (RectTransform)rectTransform.Find("Label");
				TextMeshProUGUI textMeshProUGUI = this.labels[num3];
				if (segmentScroll >= (float)(num3 - 1))
				{
					float num4 = this.elementWidth;
					Vector2 offsetMin2 = rectTransform.offsetMin;
					offsetMin2.x = this.CalcSegmentStartX(num3);
					rectTransform.offsetMin = offsetMin2;
					Vector2 offsetMax = rectTransform.offsetMax;
					offsetMax.x = offsetMin2.x + num4;
					rectTransform.offsetMax = offsetMax;
					rectTransform2.anchorMin = new Vector2(0f, 0f);
					rectTransform2.anchorMax = new Vector2(0f, 1f);
					rectTransform2.offsetMin = new Vector2(0f, 0f);
					rectTransform2.offsetMax = new Vector2(this.elementWidth, 0f);
					return;
				}
				rectTransform.offsetMax = rectTransform.offsetMin + new Vector2(this.elementWidth, 0f);
				this.SetLabelDefaultDimensions(rectTransform2);
			}
		}

		// Token: 0x060022A3 RID: 8867 RVA: 0x0009643B File Offset: 0x0009463B
		private float CalcSegmentStartX(int i)
		{
			return (float)i * this.elementWidth;
		}

		// Token: 0x060022A4 RID: 8868 RVA: 0x00096446 File Offset: 0x00094646
		private float CalcSegmentEndX(int i)
		{
			return (float)(i + 1) * this.elementWidth;
		}

		// Token: 0x060022A5 RID: 8869 RVA: 0x00096454 File Offset: 0x00094654
		private void SetLabelDefaultDimensions(RectTransform labelRectTransform)
		{
			labelRectTransform.anchorMin = new Vector2(0f, 0f);
			labelRectTransform.anchorMax = new Vector2(1f, 1f);
			labelRectTransform.pivot = new Vector2(0.5f, 0.5f);
			labelRectTransform.offsetMin = new Vector2(0f, 0f);
			labelRectTransform.offsetMax = new Vector2(0f, 0f);
		}

		// Token: 0x060022A6 RID: 8870 RVA: 0x000964CC File Offset: 0x000946CC
		private void SetSegmentCount(uint desiredCount)
		{
			if (!this.segmentContainer || !this.segmentPrefab)
			{
				return;
			}
			uint num = (uint)this.segmentContainer.childCount;
			if (this.images == null || (long)this.images.Length != (long)((ulong)desiredCount))
			{
				this.images = new Image[desiredCount];
				this.labels = new TextMeshProUGUI[desiredCount];
			}
			int i = 0;
			int num2 = Mathf.Min(this.images.Length, this.segmentContainer.childCount);
			while (i < num2)
			{
				Transform child = this.segmentContainer.GetChild(i);
				this.images[i] = child.GetComponent<Image>();
				this.labels[i] = child.Find("Label").GetComponent<TextMeshProUGUI>();
				i++;
			}
			while (num > desiredCount)
			{
				UnityEngine.Object.DestroyImmediate(this.segmentContainer.GetChild((int)(num - 1U)).gameObject);
				num -= 1U;
			}
			while (num < desiredCount)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.segmentPrefab, this.segmentContainer);
				gameObject.SetActive(true);
				this.images[i] = gameObject.GetComponent<Image>();
				this.labels[i] = gameObject.transform.Find("Label").GetComponent<TextMeshProUGUI>();
				i++;
				num += 1U;
			}
		}

		// Token: 0x060022A7 RID: 8871 RVA: 0x00096600 File Offset: 0x00094800
		private void SetupSegments()
		{
			if (!this.segmentContainer || !this.segmentPrefab)
			{
				return;
			}
			this.SetSegmentCount((uint)this.segmentDefs.Length);
			for (int i = 0; i < this.segmentContainer.childCount; i++)
			{
				this.SetupSegment((RectTransform)this.segmentContainer.GetChild(i), ref this.segmentDefs[i], i);
			}
			this.SetupFinalSegment((RectTransform)this.segmentContainer.GetChild(this.segmentContainer.childCount - 1));
		}

		// Token: 0x060022A8 RID: 8872 RVA: 0x00096694 File Offset: 0x00094894
		private static void ScaleLabelToWidth(TextMeshProUGUI label, float width)
		{
			RectTransform rectTransform = (RectTransform)label.transform;
			float x = label.textBounds.size.x;
			Vector3 localScale = rectTransform.localScale;
			localScale.x = width / x;
			rectTransform.localScale = localScale;
		}

		// Token: 0x060022A9 RID: 8873 RVA: 0x000966D8 File Offset: 0x000948D8
		private void SetupFinalSegment(RectTransform segmentTransform)
		{
			TextMeshProUGUI[] array = segmentTransform.GetComponentsInChildren<TextMeshProUGUI>();
			int num = 4;
			if (array.Length < num)
			{
				TextMeshProUGUI[] array2 = new TextMeshProUGUI[num];
				for (int i = 0; i < array.Length; i++)
				{
					array2[i] = array[i];
				}
				for (int j = array.Length; j < num; j++)
				{
					array2[j] = UnityEngine.Object.Instantiate<GameObject>(array[0].gameObject, segmentTransform).GetComponent<TextMeshProUGUI>();
				}
				array = array2;
			}
			int k = 0;
			int num2 = array.Length;
			while (k < num2)
			{
				TextMeshProUGUI textMeshProUGUI = array[k];
				textMeshProUGUI.enableWordWrapping = false;
				textMeshProUGUI.overflowMode = TextOverflowModes.Overflow;
				textMeshProUGUI.alignment = TextAlignmentOptions.MidlineLeft;
				textMeshProUGUI.text = Language.GetString(this.segmentDefs[this.segmentDefs.Length - 1].token);
				textMeshProUGUI.enableAutoSizing = true;
				Vector3 localPosition = textMeshProUGUI.transform.localPosition;
				localPosition.x = (float)k * this.elementWidth;
				textMeshProUGUI.transform.localPosition = localPosition;
				k++;
			}
			segmentTransform.GetComponent<Image>().sprite = this.finalSegmentSprite;
		}

		// Token: 0x060022AA RID: 8874 RVA: 0x000967D4 File Offset: 0x000949D4
		private void SetupSegment(RectTransform segmentTransform, ref DifficultyBarController.SegmentDef segmentDef, int i)
		{
			Vector2 offsetMin = segmentTransform.offsetMin;
			Vector2 offsetMax = segmentTransform.offsetMax;
			offsetMin.x = this.CalcSegmentStartX(i);
			offsetMax.x = this.CalcSegmentEndX(i);
			segmentTransform.offsetMin = offsetMin;
			segmentTransform.offsetMax = offsetMax;
			segmentTransform.GetComponent<Image>().color = segmentDef.color;
			((RectTransform)segmentTransform.Find("Label")).GetComponent<LanguageTextMeshController>().token = segmentDef.token;
		}

		// Token: 0x060022AB RID: 8875 RVA: 0x0009684A File Offset: 0x00094A4A
		private void Awake()
		{
			this.SetupSegments();
		}

		// Token: 0x060022AC RID: 8876 RVA: 0x00096854 File Offset: 0x00094A54
		private void Update()
		{
			if (Run.instance)
			{
				this.SetSegmentScroll((Run.instance.targetMonsterLevel - 1f) / this.levelsPerSegment);
			}
			if (Application.isPlaying)
			{
				this.RunAnimations(Time.deltaTime);
			}
			this.UpdateGears();
		}

		// Token: 0x060022AD RID: 8877 RVA: 0x000968A4 File Offset: 0x00094AA4
		private void UpdateGears()
		{
			foreach (RawImage rawImage in this.wormGearImages)
			{
				Rect uvRect = rawImage.uvRect;
				float num = Mathf.Sign(uvRect.width);
				uvRect.x = this.scrollXRaw * this.UVScaleToScrollX * num + ((num < 0f) ? this.gearUVOffset : 0f);
				rawImage.uvRect = uvRect;
			}
		}

		// Token: 0x060022AE RID: 8878 RVA: 0x00096910 File Offset: 0x00094B10
		private void RunAnimations(float deltaTime)
		{
			for (int i = this.playingAnimations.Count - 1; i >= 0; i--)
			{
				DifficultyBarController.SegmentImageAnimation segmentImageAnimation = this.playingAnimations[i];
				segmentImageAnimation.age += deltaTime;
				float num = Mathf.Clamp01(segmentImageAnimation.age / segmentImageAnimation.duration);
				segmentImageAnimation.Update(num);
				if (num >= 1f)
				{
					this.playingAnimations.RemoveAt(i);
				}
			}
		}

		// Token: 0x04002016 RID: 8214
		[Header("Component References")]
		public RectTransform viewPort;

		// Token: 0x04002017 RID: 8215
		public RectTransform segmentContainer;

		// Token: 0x04002018 RID: 8216
		[Header("Layout")]
		[Tooltip("How wide each segment should be.")]
		public float elementWidth;

		// Token: 0x04002019 RID: 8217
		public float levelsPerSegment;

		// Token: 0x0400201A RID: 8218
		public float debugTime;

		// Token: 0x0400201B RID: 8219
		[Header("Segment Parameters")]
		public DifficultyBarController.SegmentDef[] segmentDefs;

		// Token: 0x0400201C RID: 8220
		[Tooltip("The prefab to instantiate for each segment.")]
		public GameObject segmentPrefab;

		// Token: 0x0400201D RID: 8221
		[Header("Colors")]
		public float pastSaturationMultiplier;

		// Token: 0x0400201E RID: 8222
		public float pastValueMultiplier;

		// Token: 0x0400201F RID: 8223
		public Color pastLabelColor;

		// Token: 0x04002020 RID: 8224
		public float currentSaturationMultiplier;

		// Token: 0x04002021 RID: 8225
		public float currentValueMultiplier;

		// Token: 0x04002022 RID: 8226
		public Color currentLabelColor;

		// Token: 0x04002023 RID: 8227
		public float upcomingSaturationMultiplier;

		// Token: 0x04002024 RID: 8228
		public float upcomingValueMultiplier;

		// Token: 0x04002025 RID: 8229
		public Color upcomingLabelColor;

		// Token: 0x04002026 RID: 8230
		[Header("Animations")]
		public AnimationCurve fadeAnimationCurve;

		// Token: 0x04002027 RID: 8231
		public float fadeAnimationDuration = 1f;

		// Token: 0x04002028 RID: 8232
		public AnimationCurve flashAnimationCurve;

		// Token: 0x04002029 RID: 8233
		public float flashAnimationDuration = 0.5f;

		// Token: 0x0400202A RID: 8234
		private int currentSegmentIndex = -1;

		// Token: 0x0400202B RID: 8235
		private static readonly Color labelFadedColor = Color.Lerp(Color.gray, Color.white, 0.5f);

		// Token: 0x0400202C RID: 8236
		[Header("Final Segment")]
		public Sprite finalSegmentSprite;

		// Token: 0x0400202D RID: 8237
		private float scrollX;

		// Token: 0x0400202E RID: 8238
		private float scrollXRaw;

		// Token: 0x0400202F RID: 8239
		[Tooltip("Do not set this manually. Regenerate the children instead.")]
		public Image[] images;

		// Token: 0x04002030 RID: 8240
		[Tooltip("Do not set this manually. Regenerate the children instead.")]
		public TextMeshProUGUI[] labels;

		// Token: 0x04002031 RID: 8241
		public RawImage[] wormGearImages;

		// Token: 0x04002032 RID: 8242
		public float UVScaleToScrollX;

		// Token: 0x04002033 RID: 8243
		public float gearUVOffset;

		// Token: 0x04002034 RID: 8244
		private readonly List<DifficultyBarController.SegmentImageAnimation> playingAnimations = new List<DifficultyBarController.SegmentImageAnimation>();

		// Token: 0x020005B4 RID: 1460
		[Serializable]
		public struct SegmentDef
		{
			// Token: 0x04002035 RID: 8245
			[Tooltip("The default English string to use for the element at design time.")]
			public string debugString;

			// Token: 0x04002036 RID: 8246
			[Tooltip("The final language token to use for this element at runtime.")]
			public string token;

			// Token: 0x04002037 RID: 8247
			[Tooltip("The color to use for the panel.")]
			public Color color;
		}

		// Token: 0x020005B5 RID: 1461
		private class SegmentImageAnimation
		{
			// Token: 0x060022B1 RID: 8881 RVA: 0x000969C9 File Offset: 0x00094BC9
			public void Update(float t)
			{
				this.segmentImage.color = Color.Lerp(this.color0, this.color1, this.colorCurve.Evaluate(t));
			}

			// Token: 0x04002038 RID: 8248
			public Image segmentImage;

			// Token: 0x04002039 RID: 8249
			public float age;

			// Token: 0x0400203A RID: 8250
			public float duration;

			// Token: 0x0400203B RID: 8251
			public AnimationCurve colorCurve;

			// Token: 0x0400203C RID: 8252
			public Color color0;

			// Token: 0x0400203D RID: 8253
			public Color color1;
		}
	}
}
