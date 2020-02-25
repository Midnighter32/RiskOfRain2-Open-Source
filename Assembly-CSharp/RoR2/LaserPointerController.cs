using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200026E RID: 622
	internal class LaserPointerController : MonoBehaviour
	{
		// Token: 0x06000DCD RID: 3533 RVA: 0x0003DFAC File Offset: 0x0003C1AC
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

		// Token: 0x04000DD1 RID: 3537
		public InputBankTest source;

		// Token: 0x04000DD2 RID: 3538
		public GameObject dotObject;

		// Token: 0x04000DD3 RID: 3539
		public LineRenderer beam;

		// Token: 0x04000DD4 RID: 3540
		public float minDistanceFromStart = 4f;
	}
}
