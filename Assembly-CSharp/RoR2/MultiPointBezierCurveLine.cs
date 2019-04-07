using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000360 RID: 864
	[ExecuteAlways]
	[RequireComponent(typeof(LineRenderer))]
	public class MultiPointBezierCurveLine : MonoBehaviour
	{
		// Token: 0x060011C1 RID: 4545 RVA: 0x00057D81 File Offset: 0x00055F81
		private void Start()
		{
			this.lineRenderer = base.GetComponent<LineRenderer>();
		}

		// Token: 0x060011C2 RID: 4546 RVA: 0x00057D90 File Offset: 0x00055F90
		private void LateUpdate()
		{
			for (int i = 0; i < this.linePositionList.Length; i++)
			{
				float globalT = (float)i / (float)(this.linePositionList.Length - 1);
				this.linePositionList[i] = this.EvaluateBezier(globalT);
			}
			this.lineRenderer.SetPositions(this.linePositionList);
		}

		// Token: 0x060011C3 RID: 4547 RVA: 0x00057DE4 File Offset: 0x00055FE4
		private Vector3 EvaluateBezier(float globalT)
		{
			int num = this.vertexList.Length - 1;
			int num3;
			int num2 = Mathf.Min((num3 = Mathf.FloorToInt((float)num * globalT)) + 1, num);
			MultiPointBezierCurveLine.Vertex vertex = this.vertexList[num3];
			MultiPointBezierCurveLine.Vertex vertex2 = this.vertexList[num2];
			Vector3 vector = vertex.vertexTransform ? vertex.vertexTransform.position : vertex.position;
			Vector3 a = vertex2.vertexTransform ? vertex2.vertexTransform.position : vertex2.position;
			Vector3 b = vertex.vertexTransform ? vertex.vertexTransform.TransformVector(vertex.localVelocity) : vertex.localVelocity;
			Vector3 b2 = vertex2.vertexTransform ? vertex2.vertexTransform.TransformVector(vertex2.localVelocity) : vertex2.localVelocity;
			if (num3 == num2)
			{
				return vector;
			}
			float inMin = (float)num3 / (float)num;
			float inMax = (float)num2 / (float)num;
			float num4 = Util.Remap(globalT, inMin, inMax, 0f, 1f);
			Vector3 a2 = Vector3.Lerp(vector, vector + b, num4);
			Vector3 b3 = Vector3.Lerp(a, a + b2, 1f - num4);
			return Vector3.Lerp(a2, b3, num4);
		}

		// Token: 0x040015DC RID: 5596
		public MultiPointBezierCurveLine.Vertex[] vertexList;

		// Token: 0x040015DD RID: 5597
		public Vector3[] linePositionList;

		// Token: 0x040015DE RID: 5598
		[HideInInspector]
		public LineRenderer lineRenderer;

		// Token: 0x02000361 RID: 865
		[Serializable]
		public struct Vertex
		{
			// Token: 0x040015DF RID: 5599
			public Transform vertexTransform;

			// Token: 0x040015E0 RID: 5600
			public Vector3 position;

			// Token: 0x040015E1 RID: 5601
			public Vector3 localVelocity;
		}
	}
}
