using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000630 RID: 1584
	[RequireComponent(typeof(HudElement))]
	public class SniperRangeIndicator : MonoBehaviour
	{
		// Token: 0x06002563 RID: 9571 RVA: 0x000A2C50 File Offset: 0x000A0E50
		private void Awake()
		{
			this.hudElement = base.GetComponent<HudElement>();
		}

		// Token: 0x06002564 RID: 9572 RVA: 0x000A2C60 File Offset: 0x000A0E60
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

		// Token: 0x0400231C RID: 8988
		public TextMeshProUGUI label;

		// Token: 0x0400231D RID: 8989
		private HudElement hudElement;
	}
}
