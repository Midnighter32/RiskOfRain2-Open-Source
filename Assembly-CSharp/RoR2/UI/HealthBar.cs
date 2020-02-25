using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005C2 RID: 1474
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Canvas))]
	public class HealthBar : MonoBehaviour
	{
		// Token: 0x170003A7 RID: 935
		// (get) Token: 0x060022ED RID: 8941 RVA: 0x0009814E File Offset: 0x0009634E
		// (set) Token: 0x060022EE RID: 8942 RVA: 0x00098158 File Offset: 0x00096358
		public HealthComponent source
		{
			get
			{
				return this._source;
			}
			set
			{
				if (this._source != value)
				{
					this._source = value;
					this.healthFractionVelocity = 0f;
					this.cachedFractionalValue = (this._source ? (this._source.health / this._source.fullCombinedHealth) : 0f);
				}
			}
		}

		// Token: 0x060022EF RID: 8943 RVA: 0x000981B6 File Offset: 0x000963B6
		private void Awake()
		{
			this.rectTransform = (RectTransform)base.transform;
			this.barAllocator = new UIElementAllocator<Image>(this.barContainer, this.style.barPrefab);
		}

		// Token: 0x060022F0 RID: 8944 RVA: 0x000981E5 File Offset: 0x000963E5
		private void Start()
		{
			this.UpdateHealthbar(0f);
		}

		// Token: 0x060022F1 RID: 8945 RVA: 0x000981F2 File Offset: 0x000963F2
		public void Update()
		{
			this.UpdateHealthbar(Time.deltaTime);
		}

		// Token: 0x060022F2 RID: 8946 RVA: 0x00098200 File Offset: 0x00096400
		private void ApplyBars()
		{
			HealthBar.<>c__DisplayClass31_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.i = 0;
			this.barAllocator.AllocateElements(this.barInfoCollection.GetActiveCount());
			this.<ApplyBars>g__HandleBar|31_1(ref this.barInfoCollection.trailingBarInfo, ref CS$<>8__locals1);
			this.<ApplyBars>g__HandleBar|31_1(ref this.barInfoCollection.healthBarInfo, ref CS$<>8__locals1);
			this.<ApplyBars>g__HandleBar|31_1(ref this.barInfoCollection.shieldBarInfo, ref CS$<>8__locals1);
			this.<ApplyBars>g__HandleBar|31_1(ref this.barInfoCollection.curseBarInfo, ref CS$<>8__locals1);
			this.<ApplyBars>g__HandleBar|31_1(ref this.barInfoCollection.barrierBarInfo, ref CS$<>8__locals1);
			this.<ApplyBars>g__HandleBar|31_1(ref this.barInfoCollection.flashBarInfo, ref CS$<>8__locals1);
			this.<ApplyBars>g__HandleBar|31_1(ref this.barInfoCollection.cullBarInfo, ref CS$<>8__locals1);
		}

		// Token: 0x060022F3 RID: 8947 RVA: 0x000982B8 File Offset: 0x000964B8
		private void UpdateBarInfos()
		{
			if (!this.source)
			{
				return;
			}
			HealthComponent.HealthBarValues healthBarValues = this.source.GetHealthBarValues();
			HealthBar.<>c__DisplayClass34_0 CS$<>8__locals1;
			CS$<>8__locals1.currentBarEnd = 0f;
			float fullCombinedHealth = this.source.fullCombinedHealth;
			float num = 1f / fullCombinedHealth;
			this.cachedFractionalValue = Mathf.SmoothDamp(this.cachedFractionalValue, healthBarValues.healthFraction, ref this.healthFractionVelocity, 0.4f, float.PositiveInfinity, Time.deltaTime);
			ref HealthBar.BarInfo ptr = ref this.barInfoCollection.trailingBarInfo;
			ptr.normalizedXMin = 0f;
			ptr.normalizedXMax = this.cachedFractionalValue;
			ptr.enabled = !ptr.normalizedXMax.Equals(ptr.normalizedXMin);
			HealthBar.<UpdateBarInfos>g__ApplyStyle|34_1(ref ptr, ref this.style.trailingBarStyle);
			ref HealthBar.BarInfo ptr2 = ref this.barInfoCollection.healthBarInfo;
			ptr2.enabled = (healthBarValues.healthFraction > 0f);
			HealthBar.<UpdateBarInfos>g__ApplyStyle|34_1(ref ptr2, ref this.style.healthBarStyle);
			if (healthBarValues.isBoss || healthBarValues.hasInfusion)
			{
				ptr2.color = HealthBar.infusionPanelColor;
			}
			if (this.healthCritical && this.style.flashOnHealthCritical)
			{
				ptr2.color = HealthBar.GetCriticallyHurtColor();
			}
			HealthBar.<UpdateBarInfos>g__AddBar|34_0(ref ptr2, healthBarValues.healthFraction, ref CS$<>8__locals1);
			this.barInfoCollection.shieldBarInfo.enabled = (healthBarValues.shieldFraction > 0f);
			HealthBar.<UpdateBarInfos>g__ApplyStyle|34_1(ref this.barInfoCollection.shieldBarInfo, ref this.style.shieldBarStyle);
			HealthBar.<UpdateBarInfos>g__AddBar|34_0(ref this.barInfoCollection.shieldBarInfo, healthBarValues.shieldFraction, ref CS$<>8__locals1);
			this.barInfoCollection.curseBarInfo.enabled = (healthBarValues.curseFraction > 0f);
			HealthBar.<UpdateBarInfos>g__ApplyStyle|34_1(ref this.barInfoCollection.curseBarInfo, ref this.style.curseBarStyle);
			this.barInfoCollection.curseBarInfo.normalizedXMin = 1f - healthBarValues.curseFraction;
			this.barInfoCollection.curseBarInfo.normalizedXMax = 1f;
			this.barInfoCollection.barrierBarInfo.enabled = (this.source.barrier > 0f);
			HealthBar.<UpdateBarInfos>g__ApplyStyle|34_1(ref this.barInfoCollection.barrierBarInfo, ref this.style.barrierBarStyle);
			this.barInfoCollection.barrierBarInfo.normalizedXMin = 0f;
			this.barInfoCollection.barrierBarInfo.normalizedXMax = healthBarValues.barrierFraction;
			ref HealthBar.BarInfo ptr3 = ref this.barInfoCollection.flashBarInfo;
			ptr3.normalizedXMin = this.barInfoCollection.healthBarInfo.normalizedXMin;
			ptr3.normalizedXMax = this.barInfoCollection.healthBarInfo.normalizedXMax;
			float num2 = (ptr3.normalizedXMin + ptr3.normalizedXMax) * 0.5f;
			float num3 = ptr3.normalizedXMax - num2;
			float num4 = 1f - this.source.health / this.source.fullHealth;
			float num5 = 2f * num4;
			this.theta += Time.deltaTime * num5;
			if (this.theta > 1f)
			{
				this.theta -= this.theta - this.theta % 1f;
			}
			float num6 = 1f - Mathf.Cos(this.theta * 3.1415927f * 0.5f);
			num3 += num6 * 20f * num4;
			ptr3.normalizedXMin = num2 - num3;
			ptr3.normalizedXMax = num2 + num3;
			HealthBar.<UpdateBarInfos>g__ApplyStyle|34_1(ref ptr3, ref this.style.flashBarStyle);
			Color color = ptr3.color;
			color.a = (1f - num6) * num4 * 0.7f;
			ptr3.color = color;
			float num7 = healthBarValues.cullFraction;
			if (healthBarValues.isElite && this.viewerBody)
			{
				num7 = Mathf.Max(num7, this.viewerBody.executeEliteHealthFraction);
			}
			this.barInfoCollection.cullBarInfo.enabled = (num7 > 0f);
			this.barInfoCollection.cullBarInfo.normalizedXMin = 0f;
			this.barInfoCollection.cullBarInfo.normalizedXMax = num7;
			HealthBar.<UpdateBarInfos>g__ApplyStyle|34_1(ref this.barInfoCollection.cullBarInfo, ref this.style.cullBarStyle);
		}

		// Token: 0x060022F4 RID: 8948 RVA: 0x00098684 File Offset: 0x00096884
		private void UpdateHealthbar(float deltaTime)
		{
			float num = 1f;
			float num2 = 1f;
			if (this.source)
			{
				CharacterBody body = this.source.body;
				bool isElite = body.isElite;
				float fullHealth = this.source.fullHealth;
				float num3 = this.source.fullHealth + this.source.fullShield;
				float num4 = Mathf.Clamp01(this.source.health / num3 * num2);
				float num5 = Mathf.Clamp01(this.source.shield / num3 * num2);
				if (this.eliteBackdropRectTransform)
				{
					if (isElite)
					{
						num += 1f;
					}
					this.eliteBackdropRectTransform.gameObject.SetActive(isElite);
				}
				if (this.scaleHealthbarWidth && body)
				{
					float x = Util.Remap(Mathf.Clamp((body.baseMaxHealth + body.baseMaxShield) * num, 0f, this.maxHealthbarHealth), this.minHealthbarHealth, this.maxHealthbarHealth, this.minHealthbarWidth, this.maxHealthbarWidth);
					this.rectTransform.sizeDelta = new Vector2(x, this.rectTransform.sizeDelta.y);
				}
				if (this.currentHealthText)
				{
					float num6 = Mathf.Ceil(this.source.combinedHealth);
					if (num6 != this.displayStringCurrentHealth)
					{
						this.displayStringCurrentHealth = num6;
						this.currentHealthText.text = num6.ToString();
					}
				}
				if (this.fullHealthText)
				{
					float num7 = Mathf.Ceil(fullHealth);
					if (num7 != this.displayStringFullHealth)
					{
						this.displayStringFullHealth = num7;
						this.fullHealthText.text = num7.ToString();
					}
				}
				this.healthCritical = (num4 + num5 < HealthBar.criticallyHurtThreshold && this.source.alive);
				if (this.criticallyHurtImage)
				{
					if (this.healthCritical)
					{
						this.criticallyHurtImage.enabled = true;
						this.criticallyHurtImage.color = HealthBar.GetCriticallyHurtColor();
					}
					else
					{
						this.criticallyHurtImage.enabled = false;
					}
				}
				if (this.deadImage)
				{
					this.deadImage.enabled = !this.source.alive;
				}
			}
			this.UpdateBarInfos();
			this.ApplyBars();
		}

		// Token: 0x060022F5 RID: 8949 RVA: 0x000988C2 File Offset: 0x00096AC2
		public static Color GetCriticallyHurtColor()
		{
			if (Mathf.FloorToInt(Time.fixedTime * 10f) % 2 != 0)
			{
				return ColorCatalog.GetColor(ColorCatalog.ColorIndex.Teleporter);
			}
			return Color.white;
		}

		// Token: 0x060022F8 RID: 8952 RVA: 0x00098930 File Offset: 0x00096B30
		[CompilerGenerated]
		internal static void <ApplyBars>g__SetRectPosition|31_0(RectTransform rectTransform, float xMin, float xMax, float sizeDelta)
		{
			rectTransform.anchorMin = new Vector2(xMin, 0f);
			rectTransform.anchorMax = new Vector2(xMax, 1f);
			rectTransform.anchoredPosition = Vector2.zero;
			rectTransform.sizeDelta = new Vector2(sizeDelta * 0.5f + 1f, sizeDelta + 1f);
		}

		// Token: 0x060022F9 RID: 8953 RVA: 0x0009898C File Offset: 0x00096B8C
		[CompilerGenerated]
		private void <ApplyBars>g__HandleBar|31_1(ref HealthBar.BarInfo barInfo, ref HealthBar.<>c__DisplayClass31_0 A_2)
		{
			if (!barInfo.enabled)
			{
				return;
			}
			Image image = this.barAllocator.elements[A_2.i];
			image.type = barInfo.imageType;
			image.sprite = barInfo.sprite;
			image.color = barInfo.color;
			HealthBar.<ApplyBars>g__SetRectPosition|31_0((RectTransform)image.transform, barInfo.normalizedXMin, barInfo.normalizedXMax, barInfo.sizeDelta);
			int i = A_2.i + 1;
			A_2.i = i;
		}

		// Token: 0x060022FA RID: 8954 RVA: 0x00098A10 File Offset: 0x00096C10
		[CompilerGenerated]
		internal static void <UpdateBarInfos>g__AddBar|34_0(ref HealthBar.BarInfo barInfo, float fraction, ref HealthBar.<>c__DisplayClass34_0 A_2)
		{
			if (barInfo.enabled)
			{
				barInfo.normalizedXMin = A_2.currentBarEnd;
				A_2.currentBarEnd = (barInfo.normalizedXMax = barInfo.normalizedXMin + fraction);
			}
		}

		// Token: 0x060022FB RID: 8955 RVA: 0x00098A48 File Offset: 0x00096C48
		[CompilerGenerated]
		internal static void <UpdateBarInfos>g__ApplyStyle|34_1(ref HealthBar.BarInfo barInfo, ref HealthBarStyle.BarStyle barStyle)
		{
			barInfo.enabled &= barStyle.enabled;
			barInfo.color = barStyle.baseColor;
			barInfo.sprite = barStyle.sprite;
			barInfo.imageType = barStyle.imageType;
			barInfo.sizeDelta = barStyle.sizeDelta;
		}

		// Token: 0x0400209B RID: 8347
		private HealthComponent _source;

		// Token: 0x0400209C RID: 8348
		public HealthBarStyle style;

		// Token: 0x0400209D RID: 8349
		[Tooltip("The container rect for the actual bars.")]
		public RectTransform barContainer;

		// Token: 0x0400209E RID: 8350
		public RectTransform eliteBackdropRectTransform;

		// Token: 0x0400209F RID: 8351
		public Image criticallyHurtImage;

		// Token: 0x040020A0 RID: 8352
		public Image deadImage;

		// Token: 0x040020A1 RID: 8353
		public float maxLastHitTimer = 1f;

		// Token: 0x040020A2 RID: 8354
		public bool scaleHealthbarWidth;

		// Token: 0x040020A3 RID: 8355
		public float minHealthbarWidth;

		// Token: 0x040020A4 RID: 8356
		public float maxHealthbarWidth;

		// Token: 0x040020A5 RID: 8357
		public float minHealthbarHealth;

		// Token: 0x040020A6 RID: 8358
		public float maxHealthbarHealth;

		// Token: 0x040020A7 RID: 8359
		private float displayStringCurrentHealth;

		// Token: 0x040020A8 RID: 8360
		private float displayStringFullHealth;

		// Token: 0x040020A9 RID: 8361
		private RectTransform rectTransform;

		// Token: 0x040020AA RID: 8362
		private float lastHitTimer;

		// Token: 0x040020AB RID: 8363
		private float cachedFractionalValue = 1f;

		// Token: 0x040020AC RID: 8364
		private float healthFractionVelocity;

		// Token: 0x040020AD RID: 8365
		private bool healthCritical;

		// Token: 0x040020AE RID: 8366
		[NonSerialized]
		public CharacterBody viewerBody;

		// Token: 0x040020AF RID: 8367
		private static readonly Color infusionPanelColor = new Color32(231, 84, 58, byte.MaxValue);

		// Token: 0x040020B0 RID: 8368
		public static float criticallyHurtThreshold = 0.3f;

		// Token: 0x040020B1 RID: 8369
		private float theta;

		// Token: 0x040020B2 RID: 8370
		private UIElementAllocator<Image> barAllocator;

		// Token: 0x040020B3 RID: 8371
		private HealthBar.BarInfoCollection barInfoCollection;

		// Token: 0x040020B4 RID: 8372
		public TextMeshProUGUI currentHealthText;

		// Token: 0x040020B5 RID: 8373
		public TextMeshProUGUI fullHealthText;

		// Token: 0x020005C3 RID: 1475
		private struct BarInfo
		{
			// Token: 0x040020B6 RID: 8374
			public bool enabled;

			// Token: 0x040020B7 RID: 8375
			public Color color;

			// Token: 0x040020B8 RID: 8376
			public Sprite sprite;

			// Token: 0x040020B9 RID: 8377
			public Image.Type imageType;

			// Token: 0x040020BA RID: 8378
			public float normalizedXMin;

			// Token: 0x040020BB RID: 8379
			public float normalizedXMax;

			// Token: 0x040020BC RID: 8380
			public float sizeDelta;
		}

		// Token: 0x020005C4 RID: 1476
		private struct BarInfoCollection
		{
			// Token: 0x060022FC RID: 8956 RVA: 0x00098A98 File Offset: 0x00096C98
			public int GetActiveCount()
			{
				HealthBar.BarInfoCollection.<>c__DisplayClass7_0 CS$<>8__locals1;
				CS$<>8__locals1.count = 0;
				HealthBar.BarInfoCollection.<GetActiveCount>g__Check|7_0(ref this.trailingBarInfo, ref CS$<>8__locals1);
				HealthBar.BarInfoCollection.<GetActiveCount>g__Check|7_0(ref this.healthBarInfo, ref CS$<>8__locals1);
				HealthBar.BarInfoCollection.<GetActiveCount>g__Check|7_0(ref this.shieldBarInfo, ref CS$<>8__locals1);
				HealthBar.BarInfoCollection.<GetActiveCount>g__Check|7_0(ref this.curseBarInfo, ref CS$<>8__locals1);
				HealthBar.BarInfoCollection.<GetActiveCount>g__Check|7_0(ref this.barrierBarInfo, ref CS$<>8__locals1);
				HealthBar.BarInfoCollection.<GetActiveCount>g__Check|7_0(ref this.flashBarInfo, ref CS$<>8__locals1);
				HealthBar.BarInfoCollection.<GetActiveCount>g__Check|7_0(ref this.cullBarInfo, ref CS$<>8__locals1);
				return CS$<>8__locals1.count;
			}

			// Token: 0x060022FD RID: 8957 RVA: 0x00098B0E File Offset: 0x00096D0E
			[CompilerGenerated]
			internal static void <GetActiveCount>g__Check|7_0(ref HealthBar.BarInfo field, ref HealthBar.BarInfoCollection.<>c__DisplayClass7_0 A_1)
			{
				A_1.count += (field.enabled ? 1 : 0);
			}

			// Token: 0x040020BD RID: 8381
			public HealthBar.BarInfo trailingBarInfo;

			// Token: 0x040020BE RID: 8382
			public HealthBar.BarInfo healthBarInfo;

			// Token: 0x040020BF RID: 8383
			public HealthBar.BarInfo shieldBarInfo;

			// Token: 0x040020C0 RID: 8384
			public HealthBar.BarInfo curseBarInfo;

			// Token: 0x040020C1 RID: 8385
			public HealthBar.BarInfo barrierBarInfo;

			// Token: 0x040020C2 RID: 8386
			public HealthBar.BarInfo cullBarInfo;

			// Token: 0x040020C3 RID: 8387
			public HealthBar.BarInfo flashBarInfo;
		}
	}
}
