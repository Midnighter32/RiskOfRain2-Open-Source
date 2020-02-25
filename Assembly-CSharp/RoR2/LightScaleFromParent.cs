using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000271 RID: 625
	public class LightScaleFromParent : MonoBehaviour
	{
		// Token: 0x06000DE9 RID: 3561 RVA: 0x0003E610 File Offset: 0x0003C810
		private void Start()
		{
			Light component = base.GetComponent<Light>();
			if (component)
			{
				float range = component.range;
				Vector3 lossyScale = base.transform.lossyScale;
				component.range = range * Mathf.Max(new float[]
				{
					lossyScale.x,
					lossyScale.y,
					lossyScale.z
				});
			}
		}
	}
}
