using System;
using System.Collections.Generic;
using UnityEngine;

namespace JBooth.VertexPainterPro
{
	// Token: 0x02000079 RID: 121
	[ExecuteInEditMode]
	public class VertexInstanceStream : MonoBehaviour
	{
		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060001F4 RID: 500 RVA: 0x00009DF3 File Offset: 0x00007FF3
		// (set) Token: 0x060001F5 RID: 501 RVA: 0x00009DFB File Offset: 0x00007FFB
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

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060001F6 RID: 502 RVA: 0x00009E30 File Offset: 0x00008030
		// (set) Token: 0x060001F7 RID: 503 RVA: 0x00009E38 File Offset: 0x00008038
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

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060001F8 RID: 504 RVA: 0x00009E49 File Offset: 0x00008049
		// (set) Token: 0x060001F9 RID: 505 RVA: 0x00009E51 File Offset: 0x00008051
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

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060001FA RID: 506 RVA: 0x00009E62 File Offset: 0x00008062
		// (set) Token: 0x060001FB RID: 507 RVA: 0x00009E6A File Offset: 0x0000806A
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

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060001FC RID: 508 RVA: 0x00009E7B File Offset: 0x0000807B
		// (set) Token: 0x060001FD RID: 509 RVA: 0x00009E83 File Offset: 0x00008083
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

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060001FE RID: 510 RVA: 0x00009E94 File Offset: 0x00008094
		// (set) Token: 0x060001FF RID: 511 RVA: 0x00009E9C File Offset: 0x0000809C
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

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000200 RID: 512 RVA: 0x00009EAD File Offset: 0x000080AD
		// (set) Token: 0x06000201 RID: 513 RVA: 0x00009EB5 File Offset: 0x000080B5
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

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000202 RID: 514 RVA: 0x00009EC6 File Offset: 0x000080C6
		// (set) Token: 0x06000203 RID: 515 RVA: 0x00009ECE File Offset: 0x000080CE
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

		// Token: 0x06000204 RID: 516 RVA: 0x00009EE0 File Offset: 0x000080E0
		private void Start()
		{
			this.Apply(!this.keepRuntimeData);
			if (this.keepRuntimeData)
			{
				MeshFilter component = base.GetComponent<MeshFilter>();
				this._positions = component.sharedMesh.vertices;
			}
		}

		// Token: 0x06000205 RID: 517 RVA: 0x00009F20 File Offset: 0x00008120
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

		// Token: 0x06000206 RID: 518 RVA: 0x00009F4C File Offset: 0x0000814C
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

		// Token: 0x06000207 RID: 519 RVA: 0x00009FA8 File Offset: 0x000081A8
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

		// Token: 0x04000206 RID: 518
		public bool keepRuntimeData;

		// Token: 0x04000207 RID: 519
		[SerializeField]
		[HideInInspector]
		private Color[] _colors;

		// Token: 0x04000208 RID: 520
		[HideInInspector]
		[SerializeField]
		private List<Vector4> _uv0;

		// Token: 0x04000209 RID: 521
		[HideInInspector]
		[SerializeField]
		private List<Vector4> _uv1;

		// Token: 0x0400020A RID: 522
		[HideInInspector]
		[SerializeField]
		private List<Vector4> _uv2;

		// Token: 0x0400020B RID: 523
		[SerializeField]
		[HideInInspector]
		private List<Vector4> _uv3;

		// Token: 0x0400020C RID: 524
		[SerializeField]
		[HideInInspector]
		private Vector3[] _positions;

		// Token: 0x0400020D RID: 525
		[HideInInspector]
		[SerializeField]
		private Vector3[] _normals;

		// Token: 0x0400020E RID: 526
		[SerializeField]
		[HideInInspector]
		private Vector4[] _tangents;

		// Token: 0x0400020F RID: 527
		private bool enforcedColorChannels;

		// Token: 0x04000210 RID: 528
		private Mesh meshStream;
	}
}
