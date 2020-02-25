using System;
using System.Collections.Generic;
using UnityEngine;

namespace JBooth.VertexPainterPro
{
	// Token: 0x0200007E RID: 126
	[ExecuteInEditMode]
	public class VertexInstanceStream : MonoBehaviour
	{
		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000230 RID: 560 RVA: 0x0000A88F File Offset: 0x00008A8F
		// (set) Token: 0x06000231 RID: 561 RVA: 0x0000A897 File Offset: 0x00008A97
		public Color[] colors
		{
			get
			{
				return this._colors;
			}
			set
			{
				this.enforcedColorChannels = (this._colors != null && (value == null || this._colors.Length == value.Length));
				this._colors = value;
				this.Apply(true);
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000232 RID: 562 RVA: 0x0000A8CC File Offset: 0x00008ACC
		// (set) Token: 0x06000233 RID: 563 RVA: 0x0000A8D4 File Offset: 0x00008AD4
		public List<Vector4> uv0
		{
			get
			{
				return this._uv0;
			}
			set
			{
				this._uv0 = value;
				this.Apply(true);
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000234 RID: 564 RVA: 0x0000A8E5 File Offset: 0x00008AE5
		// (set) Token: 0x06000235 RID: 565 RVA: 0x0000A8ED File Offset: 0x00008AED
		public List<Vector4> uv1
		{
			get
			{
				return this._uv1;
			}
			set
			{
				this._uv1 = value;
				this.Apply(true);
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000236 RID: 566 RVA: 0x0000A8FE File Offset: 0x00008AFE
		// (set) Token: 0x06000237 RID: 567 RVA: 0x0000A906 File Offset: 0x00008B06
		public List<Vector4> uv2
		{
			get
			{
				return this._uv2;
			}
			set
			{
				this._uv2 = value;
				this.Apply(true);
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000238 RID: 568 RVA: 0x0000A917 File Offset: 0x00008B17
		// (set) Token: 0x06000239 RID: 569 RVA: 0x0000A91F File Offset: 0x00008B1F
		public List<Vector4> uv3
		{
			get
			{
				return this._uv3;
			}
			set
			{
				this._uv3 = value;
				this.Apply(true);
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x0600023A RID: 570 RVA: 0x0000A930 File Offset: 0x00008B30
		// (set) Token: 0x0600023B RID: 571 RVA: 0x0000A938 File Offset: 0x00008B38
		public Vector3[] positions
		{
			get
			{
				return this._positions;
			}
			set
			{
				this._positions = value;
				this.Apply(true);
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x0600023C RID: 572 RVA: 0x0000A949 File Offset: 0x00008B49
		// (set) Token: 0x0600023D RID: 573 RVA: 0x0000A951 File Offset: 0x00008B51
		public Vector3[] normals
		{
			get
			{
				return this._normals;
			}
			set
			{
				this._normals = value;
				this.Apply(true);
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x0600023E RID: 574 RVA: 0x0000A962 File Offset: 0x00008B62
		// (set) Token: 0x0600023F RID: 575 RVA: 0x0000A96A File Offset: 0x00008B6A
		public Vector4[] tangents
		{
			get
			{
				return this._tangents;
			}
			set
			{
				this._tangents = value;
				this.Apply(true);
			}
		}

		// Token: 0x06000240 RID: 576 RVA: 0x0000A97C File Offset: 0x00008B7C
		private void Start()
		{
			this.Apply(!this.keepRuntimeData);
			if (this.keepRuntimeData)
			{
				MeshFilter component = base.GetComponent<MeshFilter>();
				this._positions = component.sharedMesh.vertices;
			}
		}

		// Token: 0x06000241 RID: 577 RVA: 0x0000A9BC File Offset: 0x00008BBC
		private void OnDestroy()
		{
			if (!Application.isPlaying)
			{
				MeshRenderer component = base.GetComponent<MeshRenderer>();
				if (component != null)
				{
					component.additionalVertexStreams = null;
				}
			}
		}

		// Token: 0x06000242 RID: 578 RVA: 0x0000A9E8 File Offset: 0x00008BE8
		private void EnforceOriginalMeshHasColors(Mesh stream)
		{
			if (this.enforcedColorChannels)
			{
				return;
			}
			this.enforcedColorChannels = true;
			MeshFilter component = base.GetComponent<MeshFilter>();
			Color[] colors = component.sharedMesh.colors;
			if (stream != null && stream.colors.Length != 0 && (colors == null || colors.Length == 0))
			{
				component.sharedMesh.colors = stream.colors;
			}
		}

		// Token: 0x06000243 RID: 579 RVA: 0x0000AA44 File Offset: 0x00008C44
		public Mesh Apply(bool markNoLongerReadable = true)
		{
			MeshRenderer component = base.GetComponent<MeshRenderer>();
			MeshFilter component2 = base.GetComponent<MeshFilter>();
			if (component != null && component2 != null && component2.sharedMesh != null)
			{
				int vertexCount = component2.sharedMesh.vertexCount;
				Mesh mesh = this.meshStream;
				if (mesh == null || vertexCount != mesh.vertexCount)
				{
					if (mesh != null)
					{
						UnityEngine.Object.DestroyImmediate(mesh);
					}
					mesh = new Mesh();
					mesh.vertices = new Vector3[component2.sharedMesh.vertexCount];
					mesh.vertices = component2.sharedMesh.vertices;
					mesh.MarkDynamic();
					mesh.triangles = component2.sharedMesh.triangles;
					this.meshStream = mesh;
					mesh.hideFlags = HideFlags.HideAndDontSave;
				}
				if (this._positions != null && this._positions.Length == vertexCount)
				{
					mesh.vertices = this._positions;
				}
				if (this._normals != null && this._normals.Length == vertexCount)
				{
					mesh.normals = this._normals;
				}
				else
				{
					mesh.normals = null;
				}
				if (this._tangents != null && this._tangents.Length == vertexCount)
				{
					mesh.tangents = this._tangents;
				}
				else
				{
					mesh.tangents = null;
				}
				if (this._colors != null && this._colors.Length == vertexCount)
				{
					mesh.colors = this._colors;
				}
				else
				{
					mesh.colors = null;
				}
				if (this._uv0 != null && this._uv0.Count == vertexCount)
				{
					mesh.SetUVs(0, this._uv0);
				}
				else
				{
					mesh.uv = null;
				}
				if (this._uv1 != null && this._uv1.Count == vertexCount)
				{
					mesh.SetUVs(1, this._uv1);
				}
				else
				{
					mesh.uv2 = null;
				}
				if (this._uv2 != null && this._uv2.Count == vertexCount)
				{
					mesh.SetUVs(2, this._uv2);
				}
				else
				{
					mesh.uv3 = null;
				}
				if (this._uv3 != null && this._uv3.Count == vertexCount)
				{
					mesh.SetUVs(3, this._uv3);
				}
				else
				{
					mesh.uv4 = null;
				}
				this.EnforceOriginalMeshHasColors(mesh);
				if (!Application.isPlaying || Application.isEditor)
				{
					markNoLongerReadable = false;
				}
				mesh.UploadMeshData(markNoLongerReadable);
				component.additionalVertexStreams = mesh;
				return mesh;
			}
			return null;
		}

		// Token: 0x04000211 RID: 529
		public bool keepRuntimeData;

		// Token: 0x04000212 RID: 530
		[SerializeField]
		[HideInInspector]
		private Color[] _colors;

		// Token: 0x04000213 RID: 531
		[SerializeField]
		[HideInInspector]
		private List<Vector4> _uv0;

		// Token: 0x04000214 RID: 532
		[SerializeField]
		[HideInInspector]
		private List<Vector4> _uv1;

		// Token: 0x04000215 RID: 533
		[HideInInspector]
		[SerializeField]
		private List<Vector4> _uv2;

		// Token: 0x04000216 RID: 534
		[HideInInspector]
		[SerializeField]
		private List<Vector4> _uv3;

		// Token: 0x04000217 RID: 535
		[SerializeField]
		[HideInInspector]
		private Vector3[] _positions;

		// Token: 0x04000218 RID: 536
		[HideInInspector]
		[SerializeField]
		private Vector3[] _normals;

		// Token: 0x04000219 RID: 537
		[SerializeField]
		[HideInInspector]
		private Vector4[] _tangents;

		// Token: 0x0400021A RID: 538
		private bool enforcedColorChannels;

		// Token: 0x0400021B RID: 539
		private Mesh meshStream;
	}
}
