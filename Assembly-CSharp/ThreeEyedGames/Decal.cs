using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace ThreeEyedGames
{
	// Token: 0x020006E6 RID: 1766
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[ExecuteInEditMode]
	public class Decal : MonoBehaviour
	{
		// Token: 0x06002774 RID: 10100 RVA: 0x000B7149 File Offset: 0x000B5349
		private void OnEnable()
		{
			this._deferredShader = Shader.Find("Decalicious/Deferred Decal");
			this._unlitShader = Shader.Find("Decalicious/Unlit Decal");
		}

		// Token: 0x06002775 RID: 10101 RVA: 0x000B716C File Offset: 0x000B536C
		private void Reset()
		{
			base.GetComponent<MeshFilter>().sharedMesh = Resources.Load<Mesh>("DecalCube");
			MeshRenderer component = base.GetComponent<MeshRenderer>();
			component.shadowCastingMode = ShadowCastingMode.Off;
			component.receiveShadows = false;
			component.materials = new Material[0];
			component.lightProbeUsage = LightProbeUsage.BlendProbes;
			component.reflectionProbeUsage = ReflectionProbeUsage.Off;
		}

		// Token: 0x06002776 RID: 10102 RVA: 0x000B71C0 File Offset: 0x000B53C0
		private void OnWillRenderObject()
		{
			if (this.Fade <= 0f)
			{
				return;
			}
			if (this.Material == null)
			{
				return;
			}
			if (Camera.current == null)
			{
				return;
			}
			if (this.Material == null)
			{
				this.RenderMode = Decal.DecalRenderMode.Invalid;
			}
			else if (this.Material.shader == this._deferredShader)
			{
				this.RenderMode = Decal.DecalRenderMode.Deferred;
			}
			else if (this.Material.shader == this._unlitShader)
			{
				this.RenderMode = Decal.DecalRenderMode.Unlit;
			}
			else
			{
				this.RenderMode = Decal.DecalRenderMode.Invalid;
			}
			DecaliciousRenderer decaliciousRenderer = Camera.current.GetComponent<DecaliciousRenderer>();
			if (decaliciousRenderer == null)
			{
				decaliciousRenderer = Camera.current.gameObject.AddComponent<DecaliciousRenderer>();
			}
			this.Material.enableInstancing = decaliciousRenderer.UseInstancing;
			decaliciousRenderer.Add(this, this.LimitTo);
		}

		// Token: 0x06002777 RID: 10103 RVA: 0x000B729C File Offset: 0x000B549C
		private void OnDrawGizmos()
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.color = this._colorTransparent;
			Gizmos.DrawCube(Vector3.zero, Vector3.one);
			Gizmos.color = Color.white * 0.2f;
			Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
		}

		// Token: 0x06002778 RID: 10104 RVA: 0x000B72F6 File Offset: 0x000B54F6
		private void OnDrawGizmosSelected()
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.color = Color.white * 0.5f;
			Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
		}

		// Token: 0x040029A3 RID: 10659
		private const string _deferredShaderName = "Decalicious/Deferred Decal";

		// Token: 0x040029A4 RID: 10660
		private const string _unlitShaderName = "Decalicious/Unlit Decal";

		// Token: 0x040029A5 RID: 10661
		private Shader _deferredShader;

		// Token: 0x040029A6 RID: 10662
		private Shader _unlitShader;

		// Token: 0x040029A7 RID: 10663
		public Decal.DecalRenderMode RenderMode = Decal.DecalRenderMode.Invalid;

		// Token: 0x040029A8 RID: 10664
		[Tooltip("Set a Material with a Decalicious shader.")]
		public Material Material;

		// Token: 0x040029A9 RID: 10665
		[Tooltip("To which degree should the Decal be drawn? At 1, the Decal will be drawn with full effect. At 0, the Decal will not be drawn. Experiment with values greater than one.")]
		public float Fade = 1f;

		// Token: 0x040029AA RID: 10666
		[Tooltip("Set a GameObject here to only draw this Decal on the MeshRenderer of the GO or any of its children.")]
		public GameObject LimitTo;

		// Token: 0x040029AB RID: 10667
		[Tooltip("Enable to draw the Albedo / Emission pass of the Decal.")]
		public bool DrawAlbedo = true;

		// Token: 0x040029AC RID: 10668
		[Tooltip("Use an interpolated light probe for this decal for indirect light. This breaks instancing for the decal and thus comes with a performance impact, so use with caution.")]
		public bool UseLightProbes = true;

		// Token: 0x040029AD RID: 10669
		[Tooltip("Enable to draw the Normal / SpecGloss pass of the Decal.")]
		public bool DrawNormalAndGloss = true;

		// Token: 0x040029AE RID: 10670
		[Tooltip("Enable perfect Normal / SpecGloss blending between decals. Costly and has no effect when decals don't overlap, so use with caution.")]
		public bool HighQualityBlending;

		// Token: 0x040029AF RID: 10671
		protected Color _colorTransparent = new Color(0f, 0f, 0f, 0f);

		// Token: 0x020006E7 RID: 1767
		public enum DecalRenderMode
		{
			// Token: 0x040029B1 RID: 10673
			Deferred,
			// Token: 0x040029B2 RID: 10674
			Unlit,
			// Token: 0x040029B3 RID: 10675
			Invalid
		}
	}
}
