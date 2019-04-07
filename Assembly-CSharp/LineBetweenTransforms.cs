using System;
using UnityEngine;

// Token: 0x02000046 RID: 70
[RequireComponent(typeof(LineRenderer))]
[ExecuteAlways]
public class LineBetweenTransforms : MonoBehaviour
{
	// Token: 0x06000133 RID: 307 RVA: 0x00007709 File Offset: 0x00005909
	private void Start()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
	}

	// Token: 0x06000134 RID: 308 RVA: 0x00007718 File Offset: 0x00005918
	private void LateUpdate()
	{
		for (int i = 0; i < this.vertexList.Length; i++)
		{
			this.vertexList[i] = this.transformNodes[i].position;
		}
		this.lineRenderer.SetPositions(this.vertexList);
	}

	// Token: 0x04000142 RID: 322
	public Vector3[] vertexList;

	// Token: 0x04000143 RID: 323
	public Transform[] transformNodes;

	// Token: 0x04000144 RID: 324
	[HideInInspector]
	public LineRenderer lineRenderer;
}
