using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000291 RID: 657
	[RequireComponent(typeof(LineRenderer))]
	[ExecuteAlways]
	public class MultiPointBezierCurveLine : MonoBehaviour
	{
		// Token: 0x06000E9B RID: 3739 RVA: 0x00040C05 File Offset: 0x0003EE05
		private void Start()
		{
			this.lineRenderer = base.GetComponent<LineRenderer>();
		}

		// Token: 0x06000E9C RID: 3740 RVA: 0x00040C14 File Offset: 0x0003EE14
		private void LateUpdate()
		{
			for (int i = 0; i < this.linePositionList.Length; i++)
			{
				float globalT = (float)i / (float)(this.linePositionList.Length - 1);
				this.linePositionList[i] = this.EvaluateBezier(globalT);
			}
			this.lineRenderer.SetPositions(this.linePositionList);
		}

		// Token: 0x06000E9D RID: 3741 RVA: 0x00040C68 File Offset: 0x0003EE68
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

		// Token: 0x04000E83 RID: 3715
		public MultiPointBezierCurveLine.Vertex[] vertexList;

		// Token: 0x04000E84 RID: 3716
		public Vector3[] linePositionList;

		// Token: 0x04000E85 RID: 3717
		[HideInInspector]
		public LineRenderer lineRenderer;

		// Token: 0x02000292 RID: 658
		[Serializable]
		public struct Vertex
		{
			// Token: 0x04000E86 RID: 3718
			public Transform vertexTransform;

			// Token: 0x04000E87 RID: 3719
			public Vector3 position;

			// Token: 0x04000E88 RID: 3720
			public Vector3 localVelocity;
		}
	}
}
