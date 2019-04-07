using System;
using UnityEngine;

// Token: 0x02000057 RID: 87
public class SurfaceDefProvider : MonoBehaviour
{
	// Token: 0x0600016C RID: 364 RVA: 0x000083E4 File Offset: 0x000065E4
	public static SurfaceDef GetObjectSurfaceDef(Collider collider, Vector3 position)
	{
		SurfaceDefProvider component = collider.GetComponent<SurfaceDefProvider>();
		if (!component)
		{
			return null;
		}
		return component.surfaceDef;
	}

	// Token: 0x04000181 RID: 385
	[Tooltip("The primary surface definition. Use this when not tying to a splatmap.")]
	public SurfaceDef surfaceDef;
}
