using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace ThreeEyedGames
{
	// Token: 0x02000931 RID: 2353
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[ExecuteAlways]
	public class Decal : MonoBehaviour
	{
		// Token: 0x060034BE RID: 13502 RVA: 0x000E6591 File Offset: 0x000E4791
		private void OnEnable()
		{
			this._deferredShader = Shader.Find("Decalicious/Deferred Decal");
			this._unlitShader = Shader.Find("Decalicious/Unlit Decal");
		}

		// Token: 0x060034BF RID: 13503 RVA: 0x000E65B4 File Offset: 0x000E47B4
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

		// Token: 0x060034C0 RID: 13504 RVA: 0x000E6608 File Offset: 0x000E4808
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

		// Token: 0x060034C1 RID: 13505 RVA: 0x000E66E4 File Offset: 0x000E48E4
		private void OnDrawGizmos()
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.color = this._colorTransparent;
			Gizmos.DrawCube(Vector3.zero, Vector3.one);
			Gizmos.color = Color.white * 0.2f;
			Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
		}

		// Token: 0x060034C2 RID: 13506 RVA: 0x000E673E File Offset: 0x000E493E
		private void OnDrawGizmosSelected()
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.color = Color.white * 0.5f;
			Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
		}

		// Token: 0x0400343C RID: 13372
		private const string _deferredShaderName = "Decalicious/Deferred Decal";

		// Token: 0x0400343D RID: 13373
		private const string _unlitShaderName = "Decalicious/Unlit Decal";

		// Token: 0x0400343E RID: 13374
		private Shader _deferredShader;

		// Token: 0x0400343F RID: 13375
		private Shader _unlitShader;

		// Token: 0x04003440 RID: 13376
		public Decal.DecalRenderMode RenderMode = Decal.DecalRenderMode.Invalid;

		// Token: 0x04003441 RID: 13377
		[Tooltip("Set a Material with a Decalicious shader.")]
		public Material Material;

		// Token: 0x04003442 RID: 13378
		[Tooltip("To which degree should the Decal be drawn? At 1, the Decal will be drawn with full effect. At 0, the Decal will not be drawn. Experiment with values greater than one.")]
		public float Fade = 1f;

		// Token: 0x04003443 RID: 13379
		[Tooltip("Set a GameObject here to only draw this Decal on the MeshRenderer of the GO or any of its children.")]
		public GameObject LimitTo;

		// Token: 0x04003444 RID: 13380
		[Tooltip("Enable to draw the Albedo / Emission pass of the Decal.")]
		public bool DrawAlbedo = true;

		// Token: 0x04003445 RID: 13381
		[Tooltip("Use an interpolated light probe for this decal for indirect light. This breaks instancing for the decal and thus comes with a performance impact, so use with caution.")]
		public bool UseLightProbes = true;

		// Token: 0x04003446 RID: 13382
		[Tooltip("Enable to draw the Normal / SpecGloss pass of the Decal.")]
		public bool DrawNormalAndGloss = true;

		// Token: 0x04003447 RID: 13383
		[Tooltip("Enable perfect Normal / SpecGloss blending between decals. Costly and has no effect when decals don't overlap, so use with caution.")]
		public bool HighQualityBlending;

		// Token: 0x04003448 RID: 13384
		protected Color _colorTransparent = new Color(0f, 0f, 0f, 0f);

		// Token: 0x02000932 RID: 2354
		public enum DecalRenderMode
		{
			// Token: 0x0400344A RID: 13386
			Deferred,
			// Token: 0x0400344B RID: 13387
			Unlit,
			// Token: 0x0400344C RID: 13388
			Invalid
		}
	}
}
