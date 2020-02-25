using System;
using System.Collections.Generic;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005CF RID: 1487
	public class HUDScaleController : MonoBehaviour
	{
		// Token: 0x06002330 RID: 9008 RVA: 0x00099D88 File Offset: 0x00097F88
		public void OnEnable()
		{
			HUDScaleController.instancesList.Add(this);
		}

		// Token: 0x06002331 RID: 9009 RVA: 0x00099D95 File Offset: 0x00097F95
		public void OnDisable()
		{
			HUDScaleController.instancesList.Remove(this);
		}

		// Token: 0x06002332 RID: 9010 RVA: 0x00099DA3 File Offset: 0x00097FA3
		private void Start()
		{
			this.SetScale();
		}

		// Token: 0x06002333 RID: 9011 RVA: 0x00099DAC File Offset: 0x00097FAC
		private void SetScale()
		{
			BaseConVar baseConVar = Console.instance.FindConVar("hud_scale");
			float num;
			if (baseConVar != null && TextSerialization.TryParseInvariant(baseConVar.GetString(), out num))
			{
				Vector3 localScale = new Vector3(num / 100f, num / 100f, num / 100f);
				RectTransform[] array = this.rectTransforms;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].localScale = localScale;
				}
			}
		}

		// Token: 0x04002118 RID: 8472
		public RectTransform[] rectTransforms;

		// Token: 0x04002119 RID: 8473
		private static List<HUDScaleController> instancesList = new List<HUDScaleController>();

		// Token: 0x020005D0 RID: 1488
		private class HUDScaleConVar : BaseConVar
		{
			// Token: 0x06002336 RID: 9014 RVA: 0x0000972B File Offset: 0x0000792B
			private HUDScaleConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06002337 RID: 9015 RVA: 0x00099E28 File Offset: 0x00098028
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num) && num != 0)
				{
					this.intValue = num;
					foreach (HUDScaleController hudscaleController in HUDScaleController.instancesList)
					{
						hudscaleController.SetScale();
					}
				}
			}

			// Token: 0x06002338 RID: 9016 RVA: 0x00099E8C File Offset: 0x0009808C
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(this.intValue);
			}

			// Token: 0x0400211A RID: 8474
			public static HUDScaleController.HUDScaleConVar instance = new HUDScaleController.HUDScaleConVar("hud_scale", ConVarFlags.Archive, "100", "Scales the size of HUD elements in-game. Defaults to 100.");

			// Token: 0x0400211B RID: 8475
			private int intValue;
		}
	}
}
