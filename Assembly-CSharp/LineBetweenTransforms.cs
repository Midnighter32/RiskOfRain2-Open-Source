using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000042 RID: 66
[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class LineBetweenTransforms : MonoBehaviour
{
	// Token: 0x1700001D RID: 29
	// (get) Token: 0x06000117 RID: 279 RVA: 0x00007661 File Offset: 0x00005861
	// (set) Token: 0x06000118 RID: 280 RVA: 0x00007669 File Offset: 0x00005869
	public Transform[] transformNodes
	{
		get
		{
			return this._transformNodes;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this._transformNodes = value;
			this.UpdateVertexBufferSize();
		}
	}

	// Token: 0x06000119 RID: 281 RVA: 0x00007688 File Offset: 0x00005888
	private void PushPositionsToLineRenderer()
	{
		Vector3[] array = this.vertexList;
		Transform[] transformNodes = this.transformNodes;
		for (int i = 0; i < array.Length; i++)
		{
			Transform transform = transformNodes[i];
			if (transform)
			{
				array[i] = transform.position;
			}
		}
		this.lineRenderer.SetPositions(array);
	}

	// Token: 0x0600011A RID: 282 RVA: 0x000076D6 File Offset: 0x000058D6
	private void UpdateVertexBufferSize()
	{
		Array.Resize<Vector3>(ref this.vertexList, this.transformNodes.Length);
	}

	// Token: 0x0600011B RID: 283 RVA: 0x000076EB File Offset: 0x000058EB
	private void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
		this.UpdateVertexBufferSize();
	}

	// Token: 0x0600011C RID: 284 RVA: 0x000076FF File Offset: 0x000058FF
	private void LateUpdate()
	{
		this.PushPositionsToLineRenderer();
	}

	// Token: 0x04000147 RID: 327
	[SerializeField]
	[FormerlySerializedAs("transformNodes")]
	[Tooltip("The list of transforms whose positions will drive the vertex positions of the sibling LineRenderer component.")]
	private Transform[] _transformNodes = Array.Empty<Transform>();

	// Token: 0x04000148 RID: 328
	private LineRenderer lineRenderer;

	// Token: 0x04000149 RID: 329
	private Vector3[] vertexList = Array.Empty<Vector3>();
}
