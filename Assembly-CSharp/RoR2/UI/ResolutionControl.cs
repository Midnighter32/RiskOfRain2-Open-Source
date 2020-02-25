using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2.UI
{
	// Token: 0x02000617 RID: 1559
	public class ResolutionControl : BaseSettingsControl
	{
		// Token: 0x060024DB RID: 9435 RVA: 0x000A0B1E File Offset: 0x0009ED1E
		private static Vector2Int ResolutionToVector2Int(Resolution resolution)
		{
			return new Vector2Int(resolution.width, resolution.height);
		}

		// Token: 0x060024DC RID: 9436 RVA: 0x000A0B33 File Offset: 0x0009ED33
		private ResolutionControl.ResolutionOption GetCurrentSelectedResolutionOption()
		{
			if (this.resolutionDropdown.value >= 0)
			{
				return this.resolutionOptions[this.resolutionDropdown.value];
			}
			return null;
		}

		// Token: 0x060024DD RID: 9437 RVA: 0x000A0B58 File Offset: 0x0009ED58
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

		// Token: 0x060024DE RID: 9438 RVA: 0x000A0CC0 File Offset: 0x0009EEC0
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

		// Token: 0x060024DF RID: 9439 RVA: 0x000A0D8C File Offset: 0x0009EF8C
		protected new void Awake()
		{
			base.Awake();
			this.resolutionDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnResolutionDropdownValueChanged));
			this.refreshRateDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnRefreshRateDropdownValueChanged));
		}

		// Token: 0x060024E0 RID: 9440 RVA: 0x000A0DCC File Offset: 0x0009EFCC
		protected new void OnEnable()
		{
			base.OnEnable();
			this.GenerateResolutionOptions();
		}

		// Token: 0x060024E1 RID: 9441 RVA: 0x000A0DDA File Offset: 0x0009EFDA
		private void OnResolutionDropdownValueChanged(int newValue)
		{
			if (newValue < 0)
			{
				return;
			}
			this.GenerateRefreshRateOptions();
		}

		// Token: 0x060024E2 RID: 9442 RVA: 0x000A0DE7 File Offset: 0x0009EFE7
		private void OnRefreshRateDropdownValueChanged(int newValue)
		{
		}

		// Token: 0x060024E3 RID: 9443 RVA: 0x000A0DF0 File Offset: 0x0009EFF0
		public void SubmitCurrentValue()
		{
			if (this.resolutionDropdown.value == -1 || this.refreshRateDropdown.value == -1)
			{
				return;
			}
			ResolutionControl.ResolutionOption resolutionOption = this.resolutionOptions[this.resolutionDropdown.value];
			base.SubmitSetting(string.Format(CultureInfo.InvariantCulture, "{0}x{1}x{2}", resolutionOption.size.x, resolutionOption.size.y, resolutionOption.supportedRefreshRates[this.refreshRateDropdown.value]));
		}

		// Token: 0x040022A1 RID: 8865
		public MPDropdown resolutionDropdown;

		// Token: 0x040022A2 RID: 8866
		public MPDropdown refreshRateDropdown;

		// Token: 0x040022A3 RID: 8867
		private Resolution[] resolutions;

		// Token: 0x040022A4 RID: 8868
		private ResolutionControl.ResolutionOption[] resolutionOptions = Array.Empty<ResolutionControl.ResolutionOption>();

		// Token: 0x02000618 RID: 1560
		private class ResolutionOption
		{
			// Token: 0x060024E5 RID: 9445 RVA: 0x000A0E90 File Offset: 0x0009F090
			public string GenerateDisplayString()
			{
				return string.Format("{0}x{1}", this.size.x, this.size.y);
			}

			// Token: 0x040022A5 RID: 8869
			public Vector2Int size;

			// Token: 0x040022A6 RID: 8870
			public readonly List<int> supportedRefreshRates = new List<int>();
		}
	}
}
