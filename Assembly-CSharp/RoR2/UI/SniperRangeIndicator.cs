using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200063D RID: 1597
	[RequireComponent(typeof(HudElement))]
	public class SniperRangeIndicator : MonoBehaviour
	{
		// Token: 0x060023CA RID: 9162 RVA: 0x000A835E File Offset: 0x000A655E
		private void Awake()
		{
			this.hudElement = base.GetComponent<HudElement>();
		}

		// Token: 0x060023CB RID: 9163 RVA: 0x000A836C File Offset: 0x000A656C
		private void FixedUpdate()
		{
			float num = float.PositiveInfinity;
			if (this.hudElement.targetCharacterBody)
			{
				InputBankTest component = this.hudElement.targetCharacterBody.GetComponent<InputBankTest>();
				if (component)
				{
					Ray ray = new Ray(component.aimOrigin, component.aimDirection);
					RaycastHit raycastHit = default(RaycastHit);
					if (Util.CharacterRaycast(this.hudElement.targetCharacterBody.gameObject, ray, out raycastHit, float.PositiveInfinity, LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal))
					{
						num = raycastHit.distance;
					}
				}
			}
			this.label.text = "Dis: " + ((num > 999f) ? "999m" : string.Format("{0:D3}m", Mathf.FloorToInt(num)));
		}

		// Token: 0x040026BF RID: 9919
		public TextMeshProUGUI label;

		// Token: 0x040026C0 RID: 9920
		private HudElement hudElement;
	}
}
