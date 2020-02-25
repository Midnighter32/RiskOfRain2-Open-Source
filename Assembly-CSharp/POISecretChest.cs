using System;
using UnityEngine;

// Token: 0x02000045 RID: 69
public class POISecretChest : MonoBehaviour
{
	// Token: 0x06000126 RID: 294 RVA: 0x0000797C File Offset: 0x00005B7C
	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(0f, 1f, 1f, 0.03f);
		Gizmos.DrawCube(Vector3.zero, base.transform.localScale / 2f);
		Gizmos.color = new Color(0f, 1f, 1f, 0.1f);
		Gizmos.DrawWireCube(Vector3.zero, base.transform.localScale / 2f);
	}

	// Token: 0x04000154 RID: 340
	public float influence = 5f;
}
