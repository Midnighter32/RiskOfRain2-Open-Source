using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200062A RID: 1578
	public class SettingsSlider : BaseSettingsControl
	{
		// Token: 0x06002547 RID: 9543 RVA: 0x000A2468 File Offset: 0x000A0668
		protected new void Awake()
		{
			base.Awake();
			if (this.slider)
			{
				this.slider.minValue = this.minValue;
				this.slider.maxValue = this.maxValue;
				this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderValueChanged));
			}
			if (this.inputField)
			{
				this.inputField.onSubmit.AddListener(new UnityAction<string>(this.OnInputFieldSubmit));
			}
		}

		// Token: 0x06002548 RID: 9544 RVA: 0x000A24EF File Offset: 0x000A06EF
		private void OnSliderValueChanged(float newValue)
		{
			if (base.inUpdateControls)
			{
				return;
			}
			base.SubmitSetting(TextSerialization.ToStringInvariant(newValue));
		}

		// Token: 0x06002549 RID: 9545 RVA: 0x000A2508 File Offset: 0x000A0708
		private void OnInputFieldSubmit(string newString)
		{
			if (base.inUpdateControls)
			{
				return;
			}
			float value;
			if (TextSerialization.TryParseInvariant(newString, out value))
			{
				base.SubmitSetting(TextSerialization.ToStringInvariant(value));
			}
		}

		// Token: 0x0600254A RID: 9546 RVA: 0x000A2534 File Offset: 0x000A0734
		protected override void OnUpdateControls()
		{
			base.OnUpdateControls();
			float value;
			if (TextSerialization.TryParseInvariant(base.GetCurrentValue(), out value))
			{
				float num = Mathf.Clamp(value, this.minValue, this.maxValue);
				if (this.slider)
				{
					this.slider.value = num;
				}
				if (this.inputField)
				{
					this.inputField.text = string.Format(CultureInfo.InvariantCulture, this.formatString, num);
				}
			}
		}

		// Token: 0x040022FA RID: 8954
		public Slider slider;

		// Token: 0x040022FB RID: 8955
		public TMP_InputField inputField;

		// Token: 0x040022FC RID: 8956
		public float minValue;

		// Token: 0x040022FD RID: 8957
		public float maxValue;

		// Token: 0x040022FE RID: 8958
		public string formatString = "{0:0.00}";
	}
}
