using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000349 RID: 841
	public class LightScaleFromParent : MonoBehaviour
	{
		// Token: 0x0600116F RID: 4463 RVA: 0x00056A6C File Offset: 0x00054C6C
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
