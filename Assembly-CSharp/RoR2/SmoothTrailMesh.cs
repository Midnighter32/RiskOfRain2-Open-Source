using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200033A RID: 826
	public class SmoothTrailMesh : MonoBehaviour
	{
		// Token: 0x060013AD RID: 5037 RVA: 0x00054070 File Offset: 0x00052270
		private void Awake()
		{
			this.mesh = new Mesh();
			this.mesh.MarkDynamic();
			GameObject gameObject = new GameObject("SmoothTrailMeshRenderer");
			this.meshFilter = gameObject.AddComponent<MeshFilter>();
			this.meshFilter.mesh = this.mesh;
			this.meshRenderer = gameObject.AddComponent<MeshRenderer>();
			this.meshRenderer.sharedMaterials = this.sharedMaterials;
		}

		// Token: 0x060013AE RID: 5038 RVA: 0x000540D8 File Offset: 0x000522D8
		private void AddCurrentPoint()
		{
			float time = Time.time;
			Vector3 position = base.transform.position;
			Vector3 b = base.transform.up * this.width * 0.5f;
			this.pointsQueue.Enqueue(new SmoothTrailMesh.Point
			{
				vertex1 = position + b,
				vertex2 = position - b,
				time = time
			});
		}

		// Token: 0x060013AF RID: 5039 RVA: 0x00054150 File Offset: 0x00052350
		private void OnEnable()
		{
			this.AddCurrentPoint();
		}

		// Token: 0x060013B0 RID: 5040 RVA: 0x00054158 File Offset: 0x00052358
		private void OnDisable()
		{
			this.pointsQueue.Clear();
			this.mesh.Clear();
		}

		// Token: 0x060013B1 RID: 5041 RVA: 0x00054170 File Offset: 0x00052370
		private void OnDestroy()
		{
			if (this.meshFilter)
			{
				this.meshFilter.mesh = null;
				UnityEngine.Object.Destroy(this.meshFilter.gameObject);
			}
			UnityEngine.Object.Destroy(this.mesh);
		}

		// Token: 0x060013B2 RID: 5042 RVA: 0x000541A8 File Offset: 0x000523A8
		private void Simulate()
		{
			float time = Time.time;
			Vector3 position = base.transform.position;
			Vector3 b = base.transform.up * this.width * 0.5f;
			float num = time - this.previousTime;
			if (num > 0f)
			{
				float num2 = 1f / num;
				for (float num3 = this.previousTime; num3 <= time; num3 += this.timeStep)
				{
					float t = (num3 - this.previousTime) * num2;
					Vector3 a = Vector3.LerpUnclamped(this.previousPosition, position, t);
					Vector3 b2 = Vector3.SlerpUnclamped(this.previousUp, b, t);
					this.pointsQueue.Enqueue(new SmoothTrailMesh.Point
					{
						vertex1 = a + b2,
						vertex2 = a - b2,
						time = num3
					});
				}
			}
			float num4 = time - this.trailLifetime;
			while (this.pointsQueue.Count > 0 && this.pointsQueue.Peek().time < num4)
			{
				this.pointsQueue.Dequeue();
			}
			this.previousTime = time;
			this.previousPosition = position;
			this.previousUp = b;
		}

		// Token: 0x060013B3 RID: 5043 RVA: 0x000542DA File Offset: 0x000524DA
		private void LateUpdate()
		{
			this.Simulate();
			this.GenerateMesh();
		}

		// Token: 0x060013B4 RID: 5044 RVA: 0x000542E8 File Offset: 0x000524E8
		private void GenerateMesh()
		{
			Vector3[] array = new Vector3[this.pointsQueue.Count * 2];
			Vector2[] array2 = new Vector2[this.pointsQueue.Count * 2];
			Color[] array3 = new Color[this.pointsQueue.Count * 2];
			float num = 1f / (float)this.pointsQueue.Count;
			int num2 = 0;
			if (this.pointsQueue.Count > 0)
			{
				float time = this.pointsQueue.Peek().time;
				float time2 = Time.time;
				float num3 = time2 - time;
				float num4 = 1f / num3;
				foreach (SmoothTrailMesh.Point point in this.pointsQueue)
				{
					float num5 = (time2 - point.time) * num4;
					array[num2] = point.vertex1;
					array2[num2] = new Vector2(1f, num5);
					array3[num2] = new Color(1f, 1f, 1f, this.fadeVertexAlpha ? (1f - num5) : 1f);
					num2++;
					array[num2] = point.vertex2;
					array2[num2] = new Vector2(0f, num5);
					num2++;
				}
			}
			int num6 = this.pointsQueue.Count - 1;
			int[] array4 = new int[num6 * 2 * 3];
			int num7 = 0;
			int num8 = 0;
			for (int i = 0; i < num6; i++)
			{
				array4[num7] = num8;
				array4[num7 + 1] = num8 + 1;
				array4[num7 + 2] = num8 + 2;
				array4[num7 + 3] = num8 + 3;
				array4[num7 + 4] = num8 + 1;
				array4[num7 + 5] = num8 + 2;
				num7 += 6;
				num8 += 2;
			}
			this.mesh.Clear();
			this.mesh.vertices = array;
			this.mesh.uv = array2;
			this.mesh.triangles = array4;
			this.mesh.colors = array3;
			this.mesh.RecalculateBounds();
			this.mesh.UploadMeshData(false);
		}

		// Token: 0x04001278 RID: 4728
		private MeshFilter meshFilter;

		// Token: 0x04001279 RID: 4729
		private MeshRenderer meshRenderer;

		// Token: 0x0400127A RID: 4730
		private Mesh mesh;

		// Token: 0x0400127B RID: 4731
		public float timeStep = 0.0055555557f;

		// Token: 0x0400127C RID: 4732
		public float width = 1f;

		// Token: 0x0400127D RID: 4733
		public Material[] sharedMaterials;

		// Token: 0x0400127E RID: 4734
		public float trailLifetime = 1f;

		// Token: 0x0400127F RID: 4735
		public bool fadeVertexAlpha = true;

		// Token: 0x04001280 RID: 4736
		private Vector3 previousPosition;

		// Token: 0x04001281 RID: 4737
		private Vector3 previousUp;

		// Token: 0x04001282 RID: 4738
		private float previousTime;

		// Token: 0x04001283 RID: 4739
		private Queue<SmoothTrailMesh.Point> pointsQueue = new Queue<SmoothTrailMesh.Point>();

		// Token: 0x0200033B RID: 827
		[Serializable]
		private struct Point
		{
			// Token: 0x04001284 RID: 4740
			public Vector3 vertex1;

			// Token: 0x04001285 RID: 4741
			public Vector3 vertex2;

			// Token: 0x04001286 RID: 4742
			public float time;
		}
	}
}
