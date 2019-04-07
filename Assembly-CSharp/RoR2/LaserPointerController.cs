using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000348 RID: 840
	internal class LaserPointerController : MonoBehaviour
	{
		// Token: 0x0600116D RID: 4461 RVA: 0x00056968 File Offset: 0x00054B68
		private void LateUpdate()
		{
			bool enabled = false;
			bool active = false;
			if (this.source)
			{
				Ray ray = new Ray(this.source.aimOrigin, this.source.aimDirection);
				RaycastHit raycastHit;
				if (Physics.Raycast(ray, out raycastHit, float.PositiveInfinity, LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal))
				{
					base.transform.position = raycastHit.point;
					base.transform.forward = -ray.direction;
					float num = raycastHit.distance - this.minDistanceFromStart;
					if (num >= 0.1f)
					{
						this.beam.SetPosition(1, new Vector3(0f, 0f, num));
						enabled = true;
					}
					active = true;
				}
			}
			this.dotObject.SetActive(active);
			this.beam.enabled = enabled;
		}

		// Token: 0x04001575 RID: 5493
		public InputBankTest source;

		// Token: 0x04001576 RID: 5494
		public GameObject dotObject;

		// Token: 0x04001577 RID: 5495
		public LineRenderer beam;

		// Token: 0x04001578 RID: 5496
		public float minDistanceFromStart = 4f;
	}
}
