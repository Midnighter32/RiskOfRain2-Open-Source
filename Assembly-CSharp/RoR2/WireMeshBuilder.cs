using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020004EC RID: 1260
	public class WireMeshBuilder : IDisposable
	{
		// Token: 0x06001C89 RID: 7305 RVA: 0x000851AC File Offset: 0x000833AC
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

		// Token: 0x06001C8A RID: 7306 RVA: 0x0008520C File Offset: 0x0008340C
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

		// Token: 0x06001C8B RID: 7307 RVA: 0x00085278 File Offset: 0x00083478
		public Mesh GenerateMesh()
		{
			Mesh mesh = new Mesh();
			mesh.SetVertices(this.positions);
			mesh.SetColors(this.colors);
			mesh.SetIndices(this.indices.ToArray(), MeshTopology.Lines, 0);
			return mesh;
		}

		// Token: 0x06001C8C RID: 7308 RVA: 0x000852AA File Offset: 0x000834AA
		public void Dispose()
		{
			this.uniqueVertexToIndex = null;
			this.indices = null;
			this.positions = null;
			this.colors = null;
		}

		// Token: 0x04001E98 RID: 7832
		private int uniqueVertexCount;

		// Token: 0x04001E99 RID: 7833
		private Dictionary<WireMeshBuilder.LineVertex, int> uniqueVertexToIndex = new Dictionary<WireMeshBuilder.LineVertex, int>();

		// Token: 0x04001E9A RID: 7834
		private List<int> indices = new List<int>();

		// Token: 0x04001E9B RID: 7835
		private List<Vector3> positions = new List<Vector3>();

		// Token: 0x04001E9C RID: 7836
		private List<Color> colors = new List<Color>();

		// Token: 0x020004ED RID: 1261
		private struct LineVertex
		{
			// Token: 0x04001E9D RID: 7837
			public Vector3 position;

			// Token: 0x04001E9E RID: 7838
			public Color color;
		}
	}
}
