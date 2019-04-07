using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005E3 RID: 1507
	[RequireComponent(typeof(RectTransform))]
	public class HealthBar : MonoBehaviour
	{
		// Token: 0x060021BD RID: 8637 RVA: 0x0009F109 File Offset: 0x0009D309
		private void Awake()
		{
			this.rectTransform = base.GetComponent<RectTransform>();
			this.healthbarScale = 1f;
			this.fillImage = this.fillRectTransform.GetComponent<Image>();
			this.originalFillColor = this.fillImage.color;
		}

		// Token: 0x060021BE RID: 8638 RVA: 0x0009F144 File Offset: 0x0009D344
		private void Start()
		{
			this.UpdateHealthbar(0f);
		}

		// Token: 0x060021BF RID: 8639 RVA: 0x0009F151 File Offset: 0x0009D351
		public void Update()
		{
			this.UpdateHealthbar(Time.deltaTime);
		}

		// Token: 0x060021C0 RID: 8640 RVA: 0x0009F160 File Offset: 0x0009D360
		private void UpdateHealthbar(float deltaTime)
		{
			float num = 0f;
			float num2 = 1f;
			float num3 = 1f;
			if (this.source)
			{
				CharacterBody component = this.source.GetComponent<CharacterBody>();
				if (component)
				{
					float num4 = component.CalcLunarDaggerPower();
					num3 /= num4;
				}
				float fullHealth = this.source.fullHealth;
				float f = this.source.health + this.source.shield;
				float num5 = this.source.fullHealth + this.source.fullShield;
				num = Mathf.Clamp01(this.source.health / num5 * num3);
				float num6 = Mathf.Clamp01(this.source.shield / num5 * num3);
				if (!this.hasCachedInitialValue)
				{
					this.cachedFractionalValue = num;
					this.hasCachedInitialValue = true;
				}
				if (this.eliteBackdropRectTransform)
				{
					if (component.equipmentSlot && EliteCatalog.IsEquipmentElite(component.equipmentSlot.equipmentIndex) != EliteIndex.None)
					{
						num2 += 1f;
						this.eliteBackdropRectTransform.gameObject.SetActive(true);
					}
					else
					{
						this.eliteBackdropRectTransform.gameObject.SetActive(false);
					}
				}
				if (this.frozenCullThresholdRectTransform)
				{
					this.frozenCullThresholdRectTransform.gameObject.SetActive(this.source.isFrozen);
				}
				bool active = false;
				if (this.source.fullShield > 0f)
				{
					active = true;
				}
				this.shieldFillRectTransform.gameObject.SetActive(active);
				if (this.scaleHealthbarWidth && component)
				{
					float num7 = Util.Remap(Mathf.Clamp((component.baseMaxHealth + component.baseMaxShield) * num2, 0f, this.maxHealthbarHealth), this.minHealthbarHealth, this.maxHealthbarHealth, this.minHealthbarWidth, this.maxHealthbarWidth);
					this.healthbarScale = num7 / this.minHealthbarWidth;
					this.rectTransform.sizeDelta = new Vector2(num7, this.rectTransform.sizeDelta.y);
				}
				Color color = this.originalFillColor;
				CharacterMaster master = component.master;
				if (master && (master.isBoss || master.inventory.GetItemCount(ItemIndex.Infusion) > 0))
				{
					color = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Teleporter);
				}
				this.fillImage.color = color;
				if (this.fillRectTransform)
				{
					this.fillRectTransform.anchorMin = new Vector2(0f, 0f);
					this.fillRectTransform.anchorMax = new Vector2(num, 1f);
					this.fillRectTransform.anchoredPosition = Vector2.zero;
					this.fillRectTransform.sizeDelta = new Vector2(1f, 1f);
				}
				if (this.shieldFillRectTransform)
				{
					this.shieldFillRectTransform.anchorMin = new Vector2(num, 0f);
					this.shieldFillRectTransform.anchorMax = new Vector2(num + num6, 1f);
					this.shieldFillRectTransform.anchoredPosition = Vector2.zero;
					this.shieldFillRectTransform.sizeDelta = new Vector2(1f, 1f);
				}
				if (this.delayfillRectTransform)
				{
					this.delayfillRectTransform.anchorMin = new Vector2(0f, 0f);
					this.delayfillRectTransform.anchorMax = new Vector2(this.cachedFractionalValue, 1f);
					this.delayfillRectTransform.anchoredPosition = Vector2.zero;
					this.delayfillRectTransform.sizeDelta = new Vector2(1f, 1f);
				}
				if (this.flashRectTransform)
				{
					this.flashRectTransform.anchorMin = new Vector2(0f, 0f);
					this.flashRectTransform.anchorMax = new Vector2(num, 1f);
					float num8 = 1f - num;
					float num9 = 2f * num8;
					this.theta += deltaTime * num9;
					if (this.theta > 1f)
					{
						this.theta -= this.theta - this.theta % 1f;
					}
					float num10 = 1f - Mathf.Cos(this.theta * 3.1415927f * 0.5f);
					this.flashRectTransform.sizeDelta = new Vector2(num10 * 20f * num8, num10 * 20f * num8);
					Image component2 = this.flashRectTransform.GetComponent<Image>();
					if (component2)
					{
						Color color2 = component2.color;
						color2.a = (1f - num10) * num8 * 0.7f;
						component2.color = color2;
					}
				}
				if (this.currentHealthText)
				{
					float num11 = Mathf.Ceil(f);
					if (num11 != this.displayStringCurrentHealth)
					{
						this.displayStringCurrentHealth = num11;
						this.currentHealthText.text = num11.ToString();
					}
				}
				if (this.fullHealthText)
				{
					float num12 = Mathf.Ceil(fullHealth);
					if (num12 != this.displayStringFullHealth)
					{
						this.displayStringFullHealth = num12;
						this.fullHealthText.text = num12.ToString();
					}
				}
				if (this.criticallyHurtImage)
				{
					if (num + num6 < HealthBar.criticallyHurtThreshold && this.source.alive)
					{
						this.criticallyHurtImage.enabled = true;
						this.criticallyHurtImage.color = HealthBar.GetCriticallyHurtColor();
						this.fillImage.color = HealthBar.GetCriticallyHurtColor();
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
			this.cachedFractionalValue = Mathf.SmoothDamp(this.cachedFractionalValue, num, ref this.healthFractionVelocity, 0.05f, float.PositiveInfinity, deltaTime);
		}

		// Token: 0x060021C1 RID: 8641 RVA: 0x0009F738 File Offset: 0x0009D938
		public static Color GetCriticallyHurtColor()
		{
			if (Mathf.FloorToInt(Time.fixedTime * 10f) % 2 != 0)
			{
				return ColorCatalog.GetColor(ColorCatalog.ColorIndex.Teleporter);
			}
			return Color.white;
		}

		// Token: 0x0400248A RID: 9354
		public HealthComponent source;

		// Token: 0x0400248B RID: 9355
		public RectTransform fillRectTransform;

		// Token: 0x0400248C RID: 9356
		public RectTransform shieldFillRectTransform;

		// Token: 0x0400248D RID: 9357
		public RectTransform delayfillRectTransform;

		// Token: 0x0400248E RID: 9358
		public RectTransform flashRectTransform;

		// Token: 0x0400248F RID: 9359
		public RectTransform eliteBackdropRectTransform;

		// Token: 0x04002490 RID: 9360
		public RectTransform frozenCullThresholdRectTransform;

		// Token: 0x04002491 RID: 9361
		public TextMeshProUGUI currentHealthText;

		// Token: 0x04002492 RID: 9362
		public TextMeshProUGUI fullHealthText;

		// Token: 0x04002493 RID: 9363
		public Image criticallyHurtImage;

		// Token: 0x04002494 RID: 9364
		public Image deadImage;

		// Token: 0x04002495 RID: 9365
		public float maxLastHitTimer = 1f;

		// Token: 0x04002496 RID: 9366
		public bool scaleHealthbarWidth;

		// Token: 0x04002497 RID: 9367
		public float minHealthbarWidth;

		// Token: 0x04002498 RID: 9368
		public float maxHealthbarWidth;

		// Token: 0x04002499 RID: 9369
		public float minHealthbarHealth;

		// Token: 0x0400249A RID: 9370
		public float maxHealthbarHealth;

		// Token: 0x0400249B RID: 9371
		private float displayStringCurrentHealth;

		// Token: 0x0400249C RID: 9372
		private float displayStringFullHealth;

		// Token: 0x0400249D RID: 9373
		private RectTransform rectTransform;

		// Token: 0x0400249E RID: 9374
		private float lastHitTimer;

		// Token: 0x0400249F RID: 9375
		private float cachedFractionalValue = 1f;

		// Token: 0x040024A0 RID: 9376
		private bool hasCachedInitialValue;

		// Token: 0x040024A1 RID: 9377
		private float healthbarScale = 1f;

		// Token: 0x040024A2 RID: 9378
		private Image fillImage;

		// Token: 0x040024A3 RID: 9379
		private Color originalFillColor;

		// Token: 0x040024A4 RID: 9380
		public static float criticallyHurtThreshold = 0.3f;

		// Token: 0x040024A5 RID: 9381
		private float theta;

		// Token: 0x040024A6 RID: 9382
		private float healthFractionVelocity;
	}
}
