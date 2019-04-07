using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003E6 RID: 998
	public class SmoothTrailMesh : MonoBehaviour
	{
		// Token: 0x060015BF RID: 5567 RVA: 0x00068270 File Offset: 0x00066470
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

		// Token: 0x060015C0 RID: 5568 RVA: 0x000682D8 File Offset: 0x000664D8
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

		// Token: 0x060015C1 RID: 5569 RVA: 0x00068350 File Offset: 0x00066550
		private void OnEnable()
		{
			this.AddCurrentPoint();
		}

		// Token: 0x060015C2 RID: 5570 RVA: 0x00068358 File Offset: 0x00066558
		private void OnDisable()
		{
			this.pointsQueue.Clear();
			this.mesh.Clear();
		}

		// Token: 0x060015C3 RID: 5571 RVA: 0x00068370 File Offset: 0x00066570
		private void OnDestroy()
		{
			if (this.meshFilter)
			{
				this.meshFilter.mesh = null;
				UnityEngine.Object.Destroy(this.meshFilter.gameObject);
			}
			UnityEngine.Object.Destroy(this.mesh);
		}

		// Token: 0x060015C4 RID: 5572 RVA: 0x000683A8 File Offset: 0x000665A8
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

		// Token: 0x060015C5 RID: 5573 RVA: 0x000684DA File Offset: 0x000666DA
		private void LateUpdate()
		{
			this.Simulate();
			this.GenerateMesh();
		}

		// Token: 0x060015C6 RID: 5574 RVA: 0x000684E8 File Offset: 0x000666E8
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

		// Token: 0x04001922 RID: 6434
		private MeshFilter meshFilter;

		// Token: 0x04001923 RID: 6435
		private MeshRenderer meshRenderer;

		// Token: 0x04001924 RID: 6436
		private Mesh mesh;

		// Token: 0x04001925 RID: 6437
		public float timeStep = 0.0055555557f;

		// Token: 0x04001926 RID: 6438
		public float width = 1f;

		// Token: 0x04001927 RID: 6439
		public Material[] sharedMaterials;

		// Token: 0x04001928 RID: 6440
		public float trailLifetime = 1f;

		// Token: 0x04001929 RID: 6441
		public bool fadeVertexAlpha = true;

		// Token: 0x0400192A RID: 6442
		private Vector3 previousPosition;

		// Token: 0x0400192B RID: 6443
		private Vector3 previousUp;

		// Token: 0x0400192C RID: 6444
		private float previousTime;

		// Token: 0x0400192D RID: 6445
		private Queue<SmoothTrailMesh.Point> pointsQueue = new Queue<SmoothTrailMesh.Point>();

		// Token: 0x020003E7 RID: 999
		[Serializable]
		private struct Point
		{
			// Token: 0x0400192E RID: 6446
			public Vector3 vertex1;

			// Token: 0x0400192F RID: 6447
			public Vector3 vertex2;

			// Token: 0x04001930 RID: 6448
			public float time;
		}
	}
}
