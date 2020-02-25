using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000488 RID: 1160
	public class WireMeshBuilder : IDisposable
	{
		// Token: 0x06001C50 RID: 7248 RVA: 0x00078E04 File Offset: 0x00077004
		private int GetVertexIndex(WireMeshBuilder.LineVertex vertex)
		{
			int num;
			if (!this.uniqueVertexToIndex.TryGetValue(vertex, out num))
			{
				int num2 = this.uniqueVertexCount;
				this.uniqueVertexCount = num2 + 1;
				num = num2;
				this.positions.Add(vertex.position);
				this.colors.Add(vertex.color);
				this.uniqueVertexToIndex.Add(vertex, num);
			}
			return num;
		}

		// Token: 0x06001C51 RID: 7249 RVA: 0x00078E64 File Offset: 0x00077064
		public void AddLine(Vector3 p1, Color c1, Vector3 p2, Color c2)
		{
			WireMeshBuilder.LineVertex vertex = new WireMeshBuilder.LineVertex
			{
				position = p1,
				color = c1
			};
			WireMeshBuilder.LineVertex vertex2 = new WireMeshBuilder.LineVertex
			{
				position = p2,
				color = c2
			};
			int vertexIndex = this.GetVertexIndex(vertex);
			int vertexIndex2 = this.GetVertexIndex(vertex2);
			this.indices.Add(vertexIndex);
			this.indices.Add(vertexIndex2);
		}

		// Token: 0x06001C52 RID: 7250 RVA: 0x00078ED0 File Offset: 0x000770D0
		public Mesh GenerateMesh()
		{
			Mesh mesh = new Mesh();
			mesh.SetVertices(this.positions);
			mesh.SetColors(this.colors);
			mesh.SetIndices(this.indices.ToArray(), MeshTopology.Lines, 0);
			return mesh;
		}

		// Token: 0x06001C53 RID: 7251 RVA: 0x00078F02 File Offset: 0x00077102
		public void Dispose()
		{
			this.uniqueVertexToIndex = null;
			this.indices = null;
			this.positions = null;
			this.colors = null;
		}

		// Token: 0x04001936 RID: 6454
		private int uniqueVertexCount;

		// Token: 0x04001937 RID: 6455
		private Dictionary<WireMeshBuilder.LineVertex, int> uniqueVertexToIndex = new Dictionary<WireMeshBuilder.LineVertex, int>();

		// Token: 0x04001938 RID: 6456
		private List<int> indices = new List<int>();

		// Token: 0x04001939 RID: 6457
		private List<Vector3> positions = new List<Vector3>();

		// Token: 0x0400193A RID: 6458
		private List<Color> colors = new List<Color>();

		// Token: 0x02000489 RID: 1161
		private struct LineVertex
		{
			// Token: 0x0400193B RID: 6459
			public Vector3 position;

			// Token: 0x0400193C RID: 6460
			public Color color;
		}
	}
}
