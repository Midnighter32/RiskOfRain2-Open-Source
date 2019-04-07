using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005D4 RID: 1492
	[ExecuteInEditMode]
	public class DifficultyBarController : MonoBehaviour
	{
		// Token: 0x06002170 RID: 8560 RVA: 0x0009CFEC File Offset: 0x0009B1EC
		private static Color ColorMultiplySaturationAndValue(ref Color col, float saturationMultiplier, float valueMultiplier)
		{
			float h;
			float num;
			float num2;
			Color.RGBToHSV(col, out h, out num, out num2);
			return Color.HSVToRGB(h, num * saturationMultiplier, num2 * valueMultiplier);
		}

		// Token: 0x06002171 RID: 8561 RVA: 0x0009D018 File Offset: 0x0009B218
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

		// Token: 0x06002172 RID: 8562 RVA: 0x0009D2E4 File Offset: 0x0009B4E4
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

		// Token: 0x06002173 RID: 8563 RVA: 0x0009D4C7 File Offset: 0x0009B6C7
		private float CalcSegmentStartX(int i)
		{
			return (float)i * this.elementWidth;
		}

		// Token: 0x06002174 RID: 8564 RVA: 0x0009D4D2 File Offset: 0x0009B6D2
		private float CalcSegmentEndX(int i)
		{
			return (float)(i + 1) * this.elementWidth;
		}

		// Token: 0x06002175 RID: 8565 RVA: 0x0009D4E0 File Offset: 0x0009B6E0
		private void SetLabelDefaultDimensions(RectTransform labelRectTransform)
		{
			labelRectTransform.anchorMin = new Vector2(0f, 0f);
			labelRectTransform.anchorMax = new Vector2(1f, 1f);
			labelRectTransform.pivot = new Vector2(0.5f, 0.5f);
			labelRectTransform.offsetMin = new Vector2(0f, 0f);
			labelRectTransform.offsetMax = new Vector2(0f, 0f);
		}

		// Token: 0x06002176 RID: 8566 RVA: 0x0009D558 File Offset: 0x0009B758
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
				UnityEngine.Object.DestroyImmediate(this.segmentContainer.GetChild((int)(num - 1u)).gameObject);
				num -= 1u;
			}
			while (num < desiredCount)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.segmentPrefab, this.segmentContainer);
				gameObject.SetActive(true);
				this.images[i] = gameObject.GetComponent<Image>();
				this.labels[i] = gameObject.transform.Find("Label").GetComponent<TextMeshProUGUI>();
				i++;
				num += 1u;
			}
		}

		// Token: 0x06002177 RID: 8567 RVA: 0x0009D68C File Offset: 0x0009B88C
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

		// Token: 0x06002178 RID: 8568 RVA: 0x0009D720 File Offset: 0x0009B920
		private static void ScaleLabelToWidth(TextMeshProUGUI label, float width)
		{
			RectTransform rectTransform = (RectTransform)label.transform;
			float x = label.textBounds.size.x;
			Vector3 localScale = rectTransform.localScale;
			localScale.x = width / x;
			rectTransform.localScale = localScale;
		}

		// Token: 0x06002179 RID: 8569 RVA: 0x0009D764 File Offset: 0x0009B964
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

		// Token: 0x0600217A RID: 8570 RVA: 0x0009D860 File Offset: 0x0009BA60
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

		// Token: 0x0600217B RID: 8571 RVA: 0x0009D8D6 File Offset: 0x0009BAD6
		private void Awake()
		{
			this.SetupSegments();
		}

		// Token: 0x0600217C RID: 8572 RVA: 0x0009D8E0 File Offset: 0x0009BAE0
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

		// Token: 0x0600217D RID: 8573 RVA: 0x0009D930 File Offset: 0x0009BB30
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

		// Token: 0x0600217E RID: 8574 RVA: 0x0009D99C File Offset: 0x0009BB9C
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

		// Token: 0x04002407 RID: 9223
		[Header("Component References")]
		public RectTransform viewPort;

		// Token: 0x04002408 RID: 9224
		public RectTransform segmentContainer;

		// Token: 0x04002409 RID: 9225
		[Tooltip("How wide each segment should be.")]
		[Header("Layout")]
		public float elementWidth;

		// Token: 0x0400240A RID: 9226
		public float levelsPerSegment;

		// Token: 0x0400240B RID: 9227
		public float debugTime;

		// Token: 0x0400240C RID: 9228
		[Header("Segment Parameters")]
		public DifficultyBarController.SegmentDef[] segmentDefs;

		// Token: 0x0400240D RID: 9229
		[Tooltip("The prefab to instantiate for each segment.")]
		public GameObject segmentPrefab;

		// Token: 0x0400240E RID: 9230
		[Header("Colors")]
		public float pastSaturationMultiplier;

		// Token: 0x0400240F RID: 9231
		public float pastValueMultiplier;

		// Token: 0x04002410 RID: 9232
		public Color pastLabelColor;

		// Token: 0x04002411 RID: 9233
		public float currentSaturationMultiplier;

		// Token: 0x04002412 RID: 9234
		public float currentValueMultiplier;

		// Token: 0x04002413 RID: 9235
		public Color currentLabelColor;

		// Token: 0x04002414 RID: 9236
		public float upcomingSaturationMultiplier;

		// Token: 0x04002415 RID: 9237
		public float upcomingValueMultiplier;

		// Token: 0x04002416 RID: 9238
		public Color upcomingLabelColor;

		// Token: 0x04002417 RID: 9239
		[Header("Animations")]
		public AnimationCurve fadeAnimationCurve;

		// Token: 0x04002418 RID: 9240
		public float fadeAnimationDuration = 1f;

		// Token: 0x04002419 RID: 9241
		public AnimationCurve flashAnimationCurve;

		// Token: 0x0400241A RID: 9242
		public float flashAnimationDuration = 0.5f;

		// Token: 0x0400241B RID: 9243
		private int currentSegmentIndex = -1;

		// Token: 0x0400241C RID: 9244
		private static readonly Color labelFadedColor = Color.Lerp(Color.gray, Color.white, 0.5f);

		// Token: 0x0400241D RID: 9245
		[Header("Final Segment")]
		public Sprite finalSegmentSprite;

		// Token: 0x0400241E RID: 9246
		private float scrollX;

		// Token: 0x0400241F RID: 9247
		private float scrollXRaw;

		// Token: 0x04002420 RID: 9248
		[Tooltip("Do not set this manually. Regenerate the children instead.")]
		public Image[] images;

		// Token: 0x04002421 RID: 9249
		[Tooltip("Do not set this manually. Regenerate the children instead.")]
		public TextMeshProUGUI[] labels;

		// Token: 0x04002422 RID: 9250
		public RawImage[] wormGearImages;

		// Token: 0x04002423 RID: 9251
		public float UVScaleToScrollX;

		// Token: 0x04002424 RID: 9252
		public float gearUVOffset;

		// Token: 0x04002425 RID: 9253
		private readonly List<DifficultyBarController.SegmentImageAnimation> playingAnimations = new List<DifficultyBarController.SegmentImageAnimation>();

		// Token: 0x020005D5 RID: 1493
		[Serializable]
		public struct SegmentDef
		{
			// Token: 0x04002426 RID: 9254
			[Tooltip("The default English string to use for the element at design time.")]
			public string debugString;

			// Token: 0x04002427 RID: 9255
			[Tooltip("The final language token to use for this element at runtime.")]
			public string token;

			// Token: 0x04002428 RID: 9256
			[Tooltip("The color to use for the panel.")]
			public Color color;
		}

		// Token: 0x020005D6 RID: 1494
		private class SegmentImageAnimation
		{
			// Token: 0x06002181 RID: 8577 RVA: 0x0009DA55 File Offset: 0x0009BC55
			public void Update(float t)
			{
				this.segmentImage.color = Color.Lerp(this.color0, this.color1, this.colorCurve.Evaluate(t));
			}

			// Token: 0x04002429 RID: 9257
			public Image segmentImage;

			// Token: 0x0400242A RID: 9258
			public float age;

			// Token: 0x0400242B RID: 9259
			public float duration;

			// Token: 0x0400242C RID: 9260
			public AnimationCurve colorCurve;

			// Token: 0x0400242D RID: 9261
			public Color color0;

			// Token: 0x0400242E RID: 9262
			public Color color1;
		}
	}
}
