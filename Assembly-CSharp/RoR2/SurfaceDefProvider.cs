using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000347 RID: 839
	public class SurfaceDefProvider : MonoBehaviour
	{
		// Token: 0x060013F9 RID: 5113 RVA: 0x00055808 File Offset: 0x00053A08
		public static SurfaceDef GetObjectSurfaceDef(Collider collider, Vector3 position)
		{
			SurfaceDefProvider component = collider.GetComponent<SurfaceDefProvider>();
			if (!component)
			{
				return null;
			}
			return component.surfaceDef;
		}

		// Token: 0x040012CA RID: 4810
		[Tooltip("The primary surface definition. Use this when not tying to a splatmap.")]
		public SurfaceDef surfaceDef;
	}
}
