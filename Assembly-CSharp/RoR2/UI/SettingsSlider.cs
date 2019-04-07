using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000637 RID: 1591
	public class SettingsSlider : BaseSettingsControl
	{
		// Token: 0x060023AD RID: 9133 RVA: 0x000A7BE0 File Offset: 0x000A5DE0
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

		// Token: 0x060023AE RID: 9134 RVA: 0x000A7C67 File Offset: 0x000A5E67
		private void OnSliderValueChanged(float newValue)
		{
			if (base.inUpdateControls)
			{
				return;
			}
			base.SubmitSetting(TextSerialization.ToStringInvariant(newValue));
		}

		// Token: 0x060023AF RID: 9135 RVA: 0x000A7C80 File Offset: 0x000A5E80
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

		// Token: 0x060023B0 RID: 9136 RVA: 0x000A7CAC File Offset: 0x000A5EAC
		protected new void OnEnable()
		{
			base.OnEnable();
			base.UpdateControls();
		}

		// Token: 0x060023B1 RID: 9137 RVA: 0x000A7CBC File Offset: 0x000A5EBC
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

		// Token: 0x0400269D RID: 9885
		public Slider slider;

		// Token: 0x0400269E RID: 9886
		public TMP_InputField inputField;

		// Token: 0x0400269F RID: 9887
		public float minValue;

		// Token: 0x040026A0 RID: 9888
		public float maxValue;

		// Token: 0x040026A1 RID: 9889
		public string formatString = "{0:0.00}";
	}
}
