using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2.UI
{
	// Token: 0x02000628 RID: 1576
	public class ResolutionControl : BaseSettingsControl
	{
		// Token: 0x0600235C RID: 9052 RVA: 0x000A673E File Offset: 0x000A493E
		private static Vector2Int ResolutionToVector2Int(Resolution resolution)
		{
			return new Vector2Int(resolution.width, resolution.height);
		}

		// Token: 0x0600235D RID: 9053 RVA: 0x000A6753 File Offset: 0x000A4953
		private ResolutionControl.ResolutionOption GetCurrentSelectedResolutionOption()
		{
			if (this.resolutionDropdown.value >= 0)
			{
				return this.resolutionOptions[this.resolutionDropdown.value];
			}
			return null;
		}

		// Token: 0x0600235E RID: 9054 RVA: 0x000A6778 File Offset: 0x000A4978
		private void GenerateResolutionOptions()
		{
			Resolution[] array = Screen.resolutions;
			this.resolutionOptions = (from v in array.Select(new Func<Resolution, Vector2Int>(ResolutionControl.ResolutionToVector2Int)).Distinct<Vector2Int>()
			select new ResolutionControl.ResolutionOption
			{
				size = v
			}).ToArray<ResolutionControl.ResolutionOption>();
			foreach (ResolutionControl.ResolutionOption resolutionOption in this.resolutionOptions)
			{
				foreach (Resolution resolution in array)
				{
					if (ResolutionControl.ResolutionToVector2Int(resolution) == resolutionOption.size)
					{
						resolutionOption.supportedRefreshRates.Add(resolution.refreshRate);
					}
				}
			}
			List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
			foreach (ResolutionControl.ResolutionOption resolutionOption2 in this.resolutionOptions)
			{
				list.Add(new TMP_Dropdown.OptionData
				{
					text = resolutionOption2.GenerateDisplayString()
				});
			}
			this.resolutionDropdown.ClearOptions();
			this.resolutionDropdown.AddOptions(list);
			int value = -1;
			Vector2Int lhs = ResolutionControl.ResolutionToVector2Int(Screen.currentResolution);
			for (int k = 0; k < this.resolutionOptions.Length; k++)
			{
				if (lhs == this.resolutionOptions[k].size)
				{
					value = k;
					break;
				}
			}
			this.resolutionDropdown.value = value;
		}

		// Token: 0x0600235F RID: 9055 RVA: 0x000A68E0 File Offset: 0x000A4AE0
		private void GenerateRefreshRateOptions()
		{
			this.refreshRateDropdown.ClearOptions();
			ResolutionControl.ResolutionOption currentSelectedResolutionOption = this.GetCurrentSelectedResolutionOption();
			if (currentSelectedResolutionOption == null)
			{
				return;
			}
			List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
			foreach (int num in currentSelectedResolutionOption.supportedRefreshRates)
			{
				list.Add(new TMP_Dropdown.OptionData(num.ToString() + "Hz"));
			}
			this.refreshRateDropdown.AddOptions(list);
			int num2 = currentSelectedResolutionOption.supportedRefreshRates.IndexOf(Screen.currentResolution.refreshRate);
			if (num2 == -1)
			{
				num2 = currentSelectedResolutionOption.supportedRefreshRates.Count - 1;
			}
			this.refreshRateDropdown.value = num2;
		}

		// Token: 0x06002360 RID: 9056 RVA: 0x000A69AC File Offset: 0x000A4BAC
		protected new void Awake()
		{
			base.Awake();
			this.resolutionDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnResolutionDropdownValueChanged));
			this.refreshRateDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnRefreshRateDropdownValueChanged));
		}

		// Token: 0x06002361 RID: 9057 RVA: 0x000A69EC File Offset: 0x000A4BEC
		protected new void OnEnable()
		{
			base.OnEnable();
			this.GenerateResolutionOptions();
		}

		// Token: 0x06002362 RID: 9058 RVA: 0x000A69FA File Offset: 0x000A4BFA
		private void OnResolutionDropdownValueChanged(int newValue)
		{
			if (newValue < 0)
			{
				return;
			}
			this.GenerateRefreshRateOptions();
		}

		// Token: 0x06002363 RID: 9059 RVA: 0x000A6A07 File Offset: 0x000A4C07
		private void OnRefreshRateDropdownValueChanged(int newValue)
		{
		}

		// Token: 0x06002364 RID: 9060 RVA: 0x000A6A10 File Offset: 0x000A4C10
		public void SubmitCurrentValue()
		{
			if (this.resolutionDropdown.value == -1 || this.refreshRateDropdown.value == -1)
			{
				return;
			}
			ResolutionControl.ResolutionOption resolutionOption = this.resolutionOptions[this.resolutionDropdown.value];
			base.SubmitSetting(string.Format(CultureInfo.InvariantCulture, "{0}x{1}x{2}", resolutionOption.size.x, resolutionOption.size.y, resolutionOption.supportedRefreshRates[this.refreshRateDropdown.value]));
		}

		// Token: 0x04002658 RID: 9816
		public MPDropdown resolutionDropdown;

		// Token: 0x04002659 RID: 9817
		public MPDropdown refreshRateDropdown;

		// Token: 0x0400265A RID: 9818
		private Resolution[] resolutions;

		// Token: 0x0400265B RID: 9819
		private ResolutionControl.ResolutionOption[] resolutionOptions = Array.Empty<ResolutionControl.ResolutionOption>();

		// Token: 0x02000629 RID: 1577
		private class ResolutionOption
		{
			// Token: 0x06002366 RID: 9062 RVA: 0x000A6AB0 File Offset: 0x000A4CB0
			public string GenerateDisplayString()
			{
				return string.Format("{0}x{1} <color=#7F7F7F>({2})</color>", this.size.x, this.size.y, string.Join("|", from v in this.supportedRefreshRates
				select v.ToString()));
			}

			// Token: 0x0400265C RID: 9820
			public Vector2Int size;

			// Token: 0x0400265D RID: 9821
			public readonly List<int> supportedRefreshRates = new List<int>();
		}
	}
}
