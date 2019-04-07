using System;
using UnityEngine;

// Token: 0x0200004A RID: 74
public class POISecretChest : MonoBehaviour
{
	// Token: 0x06000144 RID: 324 RVA: 0x00007ADC File Offset: 0x00005CDC
	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(0f, 1f, 1f, 0.03f);
		Gizmos.DrawCube(Vector3.zero, base.transform.localScale / 2f);
		Gizmos.color = new Color(0f, 1f, 1f, 0.1f);
		Gizmos.DrawWireCube(Vector3.zero, base.transform.localScale / 2f);
	}

	// Token: 0x04000157 RID: 343
	public float influence = 5f;
}
