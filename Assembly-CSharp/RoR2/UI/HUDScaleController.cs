using System;
using System.Collections.Generic;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005EB RID: 1515
	public class HUDScaleController : MonoBehaviour
	{
		// Token: 0x060021F3 RID: 8691 RVA: 0x000A0A71 File Offset: 0x0009EC71
		public void OnEnable()
		{
			HUDScaleController.instancesList.Add(this);
		}

		// Token: 0x060021F4 RID: 8692 RVA: 0x000A0A7E File Offset: 0x0009EC7E
		public void OnDisable()
		{
			HUDScaleController.instancesList.Remove(this);
		}

		// Token: 0x060021F5 RID: 8693 RVA: 0x000A0A8C File Offset: 0x0009EC8C
		private void Start()
		{
			this.SetScale();
		}

		// Token: 0x060021F6 RID: 8694 RVA: 0x000A0A94 File Offset: 0x0009EC94
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

		// Token: 0x040024FB RID: 9467
		public RectTransform[] rectTransforms;

		// Token: 0x040024FC RID: 9468
		private static List<HUDScaleController> instancesList = new List<HUDScaleController>();

		// Token: 0x020005EC RID: 1516
		private class HUDScaleConVar : BaseConVar
		{
			// Token: 0x060021F9 RID: 8697 RVA: 0x00037E38 File Offset: 0x00036038
			private HUDScaleConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x060021FA RID: 8698 RVA: 0x000A0B10 File Offset: 0x0009ED10
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

			// Token: 0x060021FB RID: 8699 RVA: 0x000A0B74 File Offset: 0x0009ED74
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(this.intValue);
			}

			// Token: 0x040024FD RID: 9469
			public static HUDScaleController.HUDScaleConVar instance = new HUDScaleController.HUDScaleConVar("hud_scale", ConVarFlags.Archive, "100", "Scales the size of HUD elements in-game. Defaults to 100.");

			// Token: 0x040024FE RID: 9470
			private int intValue;
		}
	}
}
