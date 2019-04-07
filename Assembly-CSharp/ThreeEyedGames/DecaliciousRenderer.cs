using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ThreeEyedGames
{
	// Token: 0x020006E8 RID: 1768
	[ExecuteInEditMode]
	public class DecaliciousRenderer : MonoBehaviour
	{
		// Token: 0x0600277A RID: 10106 RVA: 0x000B7388 File Offset: 0x000B5588
		private void OnEnable()
		{
			this._deferredDecals = new Dictionary<Material, HashSet<Decal>>();
			this._unlitDecals = new Dictionary<Material, HashSet<Decal>>();
			this._limitToMeshRenderers = new List<MeshRenderer>();
			this._limitToSkinnedMeshRenderers = new List<SkinnedMeshRenderer>();
			this._limitToGameObjects = new HashSet<GameObject>();
			this._decalComponent = new List<Decal>();
			this._meshFilterComponent = new List<MeshFilter>();
			this._matrices = new Matrix4x4[1023];
			this._fadeValues = new float[1023];
			this._limitToValues = new float[1023];
			this._instancedBlock = new MaterialPropertyBlock();
			this._directBlock = new MaterialPropertyBlock();
			this._camera = base.GetComponent<Camera>();
			DecaliciousRenderer._cubeMesh = Resources.Load<Mesh>("DecalCube");
			this._normalRenderTarget = new RenderTargetIdentifier[]
			{
				BuiltinRenderTextureType.GBuffer1,
				BuiltinRenderTextureType.GBuffer2
			};
		}

		// Token: 0x0600277B RID: 10107 RVA: 0x000B746C File Offset: 0x000B566C
		private void OnDisable()
		{
			if (this._bufferDeferred != null)
			{
				base.GetComponent<Camera>().RemoveCommandBuffer(CameraEvent.BeforeReflections, this._bufferDeferred);
				this._bufferDeferred = null;
			}
			if (this._bufferUnlit != null)
			{
				base.GetComponent<Camera>().RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, this._bufferUnlit);
				this._bufferUnlit = null;
			}
			if (this._bufferLimitTo != null)
			{
				base.GetComponent<Camera>().RemoveCommandBuffer(CameraEvent.AfterGBuffer, this._bufferLimitTo);
				this._bufferLimitTo = null;
			}
		}

		// Token: 0x0600277C RID: 10108 RVA: 0x000B74E0 File Offset: 0x000B56E0
		private void OnPreRender()
		{
			if (!SystemInfo.supportsInstancing)
			{
				this.UseInstancing = false;
			}
			if (this._albedoRenderTarget == null || this._camera.allowHDR != this._camLastKnownHDR)
			{
				this._camLastKnownHDR = this._camera.allowHDR;
				this._albedoRenderTarget = new RenderTargetIdentifier[]
				{
					BuiltinRenderTextureType.GBuffer0,
					this._camLastKnownHDR ? BuiltinRenderTextureType.CameraTarget : BuiltinRenderTextureType.GBuffer3
				};
			}
			DecaliciousRenderer.CreateBuffer(ref this._bufferDeferred, this._camera, "Decalicious - Deferred", CameraEvent.BeforeReflections);
			DecaliciousRenderer.CreateBuffer(ref this._bufferUnlit, this._camera, "Decalicious - Unlit", CameraEvent.BeforeImageEffectsOpaque);
			DecaliciousRenderer.CreateBuffer(ref this._bufferLimitTo, this._camera, "Decalicious - Limit To Game Objects", CameraEvent.AfterGBuffer);
			this._bufferLimitTo.Clear();
			this.DrawLimitToGameObjects(this._camera);
			this._bufferDeferred.Clear();
			this.DrawDeferredDecals_Albedo(this._camera);
			this.DrawDeferredDecals_NormSpecSmooth(this._camera);
			this._bufferUnlit.Clear();
			this.DrawUnlitDecals(this._camera);
			foreach (KeyValuePair<Material, HashSet<Decal>> keyValuePair in this._deferredDecals)
			{
				keyValuePair.Value.Clear();
			}
			foreach (KeyValuePair<Material, HashSet<Decal>> keyValuePair in this._unlitDecals)
			{
				keyValuePair.Value.Clear();
			}
			this._limitToGameObjects.Clear();
		}

		// Token: 0x0600277D RID: 10109 RVA: 0x000B7654 File Offset: 0x000B5854
		private void DrawLimitToGameObjects(Camera cam)
		{
			if (this._limitToGameObjects.Count == 0)
			{
				return;
			}
			if (this._materialLimitToGameObjects == null)
			{
				this._materialLimitToGameObjects = new Material(Shader.Find("Hidden/Decalicious Game Object ID"));
			}
			int nameID = Shader.PropertyToID("_DecaliciousLimitToGameObject");
			this._bufferLimitTo.GetTemporaryRT(nameID, -1, -1, 0, FilterMode.Point, RenderTextureFormat.RFloat);
			this._bufferLimitTo.SetRenderTarget(nameID, BuiltinRenderTextureType.CameraTarget);
			this._bufferLimitTo.ClearRenderTarget(false, true, Color.black);
			Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
			foreach (GameObject gameObject in this._limitToGameObjects)
			{
				this._bufferLimitTo.SetGlobalFloat("_ID", (float)gameObject.GetInstanceID());
				this._limitToMeshRenderers.Clear();
				gameObject.GetComponentsInChildren<MeshRenderer>(this._limitToMeshRenderers);
				foreach (MeshRenderer meshRenderer in this._limitToMeshRenderers)
				{
					this._decalComponent.Clear();
					meshRenderer.GetComponents<Decal>(this._decalComponent);
					if (this._decalComponent.Count == 0 && GeometryUtility.TestPlanesAABB(planes, meshRenderer.bounds))
					{
						this._meshFilterComponent.Clear();
						meshRenderer.GetComponents<MeshFilter>(this._meshFilterComponent);
						if (this._meshFilterComponent.Count == 1)
						{
							MeshFilter meshFilter = this._meshFilterComponent[0];
							this._bufferLimitTo.DrawMesh(meshFilter.sharedMesh, meshRenderer.transform.localToWorldMatrix, this._materialLimitToGameObjects);
						}
					}
				}
				this._limitToSkinnedMeshRenderers.Clear();
				gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(this._limitToSkinnedMeshRenderers);
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in this._limitToSkinnedMeshRenderers)
				{
				}
			}
		}

		// Token: 0x0600277E RID: 10110 RVA: 0x000B7898 File Offset: 0x000B5A98
		private static void SetLightProbeOnBlock(SphericalHarmonicsL2 probe, MaterialPropertyBlock block)
		{
			for (int i = 0; i < 3; i++)
			{
				DecaliciousRenderer._avCoeff[i].x = probe[i, 3];
				DecaliciousRenderer._avCoeff[i].y = probe[i, 1];
				DecaliciousRenderer._avCoeff[i].z = probe[i, 2];
				DecaliciousRenderer._avCoeff[i].w = probe[i, 0] - probe[i, 6];
			}
			for (int j = 0; j < 3; j++)
			{
				DecaliciousRenderer._avCoeff[j + 3].x = probe[j, 4];
				DecaliciousRenderer._avCoeff[j + 3].y = probe[j, 5];
				DecaliciousRenderer._avCoeff[j + 3].z = 3f * probe[j, 6];
				DecaliciousRenderer._avCoeff[j + 3].w = probe[j, 7];
			}
			DecaliciousRenderer._avCoeff[6].x = probe[0, 8];
			DecaliciousRenderer._avCoeff[6].y = probe[1, 8];
			DecaliciousRenderer._avCoeff[6].z = probe[2, 8];
			DecaliciousRenderer._avCoeff[6].w = 1f;
			block.SetVector("unity_SHAr", DecaliciousRenderer._avCoeff[0]);
			block.SetVector("unity_SHAg", DecaliciousRenderer._avCoeff[1]);
			block.SetVector("unity_SHAb", DecaliciousRenderer._avCoeff[2]);
			block.SetVector("unity_SHBr", DecaliciousRenderer._avCoeff[3]);
			block.SetVector("unity_SHBg", DecaliciousRenderer._avCoeff[4]);
			block.SetVector("unity_SHBb", DecaliciousRenderer._avCoeff[5]);
			block.SetVector("unity_SHC", DecaliciousRenderer._avCoeff[6]);
		}

		// Token: 0x0600277F RID: 10111 RVA: 0x000B7A98 File Offset: 0x000B5C98
		private void DrawDeferredDecals_Albedo(Camera cam)
		{
			if (this._deferredDecals.Count == 0)
			{
				return;
			}
			this._bufferDeferred.SetRenderTarget(this._albedoRenderTarget, BuiltinRenderTextureType.CameraTarget);
			foreach (KeyValuePair<Material, HashSet<Decal>> keyValuePair in this._deferredDecals)
			{
				Material key = keyValuePair.Key;
				Dictionary<Material, HashSet<Decal>>.Enumerator enumerator;
				keyValuePair = enumerator.Current;
				HashSet<Decal> value = keyValuePair.Value;
				int count = value.Count;
				int num = 0;
				foreach (Decal decal in value)
				{
					if (decal != null && decal.DrawAlbedo)
					{
						if (this.UseInstancing && !decal.UseLightProbes)
						{
							this._matrices[num] = decal.transform.localToWorldMatrix;
							this._fadeValues[num] = decal.Fade;
							this._limitToValues[num] = (decal.LimitTo ? ((float)decal.LimitTo.GetInstanceID()) : float.NaN);
							num++;
							if (num == 1023)
							{
								this._instancedBlock.Clear();
								this._instancedBlock.SetFloatArray("_MaskMultiplier", this._fadeValues);
								this._instancedBlock.SetFloatArray("_LimitTo", this._limitToValues);
								DecaliciousRenderer.SetLightProbeOnBlock(RenderSettings.ambientProbe, this._instancedBlock);
								this._bufferDeferred.DrawMeshInstanced(DecaliciousRenderer._cubeMesh, 0, key, 0, this._matrices, num, this._instancedBlock);
								num = 0;
							}
						}
						else
						{
							this._directBlock.Clear();
							this._directBlock.SetFloat("_MaskMultiplier", decal.Fade);
							this._directBlock.SetFloat("_LimitTo", decal.LimitTo ? ((float)decal.LimitTo.GetInstanceID()) : float.NaN);
							if (decal.UseLightProbes)
							{
								SphericalHarmonicsL2 probe;
								LightProbes.GetInterpolatedProbe(decal.transform.position, decal.GetComponent<MeshRenderer>(), out probe);
								DecaliciousRenderer.SetLightProbeOnBlock(probe, this._directBlock);
							}
							this._bufferDeferred.DrawMesh(DecaliciousRenderer._cubeMesh, decal.transform.localToWorldMatrix, key, 0, 0, this._directBlock);
						}
					}
				}
				if (this.UseInstancing && num > 0)
				{
					this._instancedBlock.Clear();
					this._instancedBlock.SetFloatArray("_MaskMultiplier", this._fadeValues);
					this._instancedBlock.SetFloatArray("_LimitTo", this._limitToValues);
					DecaliciousRenderer.SetLightProbeOnBlock(RenderSettings.ambientProbe, this._instancedBlock);
					this._bufferDeferred.DrawMeshInstanced(DecaliciousRenderer._cubeMesh, 0, key, 0, this._matrices, num, this._instancedBlock);
				}
			}
		}

		// Token: 0x06002780 RID: 10112 RVA: 0x000B7D4C File Offset: 0x000B5F4C
		private void DrawDeferredDecals_NormSpecSmooth(Camera cam)
		{
			if (this._deferredDecals.Count == 0)
			{
				return;
			}
			int nameID = Shader.PropertyToID("_CameraGBufferTexture1Copy");
			this._bufferDeferred.GetTemporaryRT(nameID, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
			int nameID2 = Shader.PropertyToID("_CameraGBufferTexture2Copy");
			this._bufferDeferred.GetTemporaryRT(nameID2, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
			foreach (KeyValuePair<Material, HashSet<Decal>> keyValuePair in this._deferredDecals)
			{
				Material key = keyValuePair.Key;
				Dictionary<Material, HashSet<Decal>>.Enumerator enumerator;
				keyValuePair = enumerator.Current;
				HashSet<Decal> value = keyValuePair.Value;
				int num = 0;
				foreach (Decal decal in value)
				{
					if (decal != null && decal.DrawNormalAndGloss)
					{
						if (decal.HighQualityBlending)
						{
							this._bufferDeferred.Blit(BuiltinRenderTextureType.GBuffer1, nameID);
							this._bufferDeferred.Blit(BuiltinRenderTextureType.GBuffer2, nameID2);
							this._bufferDeferred.SetRenderTarget(this._normalRenderTarget, BuiltinRenderTextureType.CameraTarget);
							this._instancedBlock.Clear();
							this._instancedBlock.SetFloat("_MaskMultiplier", decal.Fade);
							this._instancedBlock.SetFloat("_LimitTo", decal.LimitTo ? ((float)decal.LimitTo.GetInstanceID()) : float.NaN);
							this._bufferDeferred.DrawMesh(DecaliciousRenderer._cubeMesh, decal.transform.localToWorldMatrix, key, 0, 1, this._instancedBlock);
						}
						else if (this.UseInstancing)
						{
							this._matrices[num] = decal.transform.localToWorldMatrix;
							this._fadeValues[num] = decal.Fade;
							this._limitToValues[num] = (decal.LimitTo ? ((float)decal.LimitTo.GetInstanceID()) : float.NaN);
							num++;
							if (num == 1023)
							{
								this._bufferDeferred.Blit(BuiltinRenderTextureType.GBuffer1, nameID);
								this._bufferDeferred.Blit(BuiltinRenderTextureType.GBuffer2, nameID2);
								this._bufferDeferred.SetRenderTarget(this._normalRenderTarget, BuiltinRenderTextureType.CameraTarget);
								this._instancedBlock.Clear();
								this._instancedBlock.SetFloatArray("_MaskMultiplier", this._fadeValues);
								this._instancedBlock.SetFloatArray("_LimitTo", this._limitToValues);
								this._bufferDeferred.DrawMeshInstanced(DecaliciousRenderer._cubeMesh, 0, key, 1, this._matrices, num, this._instancedBlock);
								num = 0;
							}
						}
						else
						{
							if (num == 0)
							{
								this._bufferDeferred.Blit(BuiltinRenderTextureType.GBuffer1, nameID);
								this._bufferDeferred.Blit(BuiltinRenderTextureType.GBuffer2, nameID2);
							}
							this._bufferDeferred.SetRenderTarget(this._normalRenderTarget, BuiltinRenderTextureType.CameraTarget);
							this._instancedBlock.Clear();
							this._instancedBlock.SetFloat("_MaskMultiplier", decal.Fade);
							this._instancedBlock.SetFloat("_LimitTo", decal.LimitTo ? ((float)decal.LimitTo.GetInstanceID()) : float.NaN);
							this._bufferDeferred.DrawMesh(DecaliciousRenderer._cubeMesh, decal.transform.localToWorldMatrix, key, 0, 1, this._instancedBlock);
							num++;
						}
					}
				}
				if (this.UseInstancing && num > 0)
				{
					this._bufferDeferred.Blit(BuiltinRenderTextureType.GBuffer1, nameID);
					this._bufferDeferred.Blit(BuiltinRenderTextureType.GBuffer2, nameID2);
					this._bufferDeferred.SetRenderTarget(this._normalRenderTarget, BuiltinRenderTextureType.CameraTarget);
					this._instancedBlock.Clear();
					this._instancedBlock.SetFloatArray("_MaskMultiplier", this._fadeValues);
					this._instancedBlock.SetFloatArray("_LimitTo", this._limitToValues);
					this._bufferDeferred.DrawMeshInstanced(DecaliciousRenderer._cubeMesh, 0, key, 1, this._matrices, num, this._instancedBlock);
				}
			}
		}

		// Token: 0x06002781 RID: 10113 RVA: 0x000B8180 File Offset: 0x000B6380
		private void DrawUnlitDecals(Camera cam)
		{
			if (this._unlitDecals.Count == 0)
			{
				return;
			}
			this._bufferUnlit.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
			foreach (KeyValuePair<Material, HashSet<Decal>> keyValuePair in this._unlitDecals)
			{
				Material key = keyValuePair.Key;
				Dictionary<Material, HashSet<Decal>>.Enumerator enumerator;
				keyValuePair = enumerator.Current;
				HashSet<Decal> value = keyValuePair.Value;
				int num = 0;
				foreach (Decal decal in value)
				{
					if (decal != null)
					{
						if (this.UseInstancing)
						{
							this._matrices[num] = decal.transform.localToWorldMatrix;
							this._fadeValues[num] = decal.Fade;
							this._limitToValues[num] = (decal.LimitTo ? ((float)decal.LimitTo.GetInstanceID()) : float.NaN);
							num++;
							if (num == 1023)
							{
								this._instancedBlock.Clear();
								this._instancedBlock.SetFloatArray("_MaskMultiplier", this._fadeValues);
								this._instancedBlock.SetFloatArray("_LimitTo", this._limitToValues);
								this._bufferUnlit.DrawMeshInstanced(DecaliciousRenderer._cubeMesh, 0, key, 0, this._matrices, num, this._instancedBlock);
								num = 0;
							}
						}
						else
						{
							this._instancedBlock.Clear();
							this._instancedBlock.SetFloat("_MaskMultiplier", decal.Fade);
							this._instancedBlock.SetFloat("_LimitTo", decal.LimitTo ? ((float)decal.LimitTo.GetInstanceID()) : float.NaN);
							this._bufferUnlit.DrawMesh(DecaliciousRenderer._cubeMesh, decal.transform.localToWorldMatrix, key, 0, 0, this._instancedBlock);
						}
					}
				}
				if (this.UseInstancing && num > 0)
				{
					this._instancedBlock.Clear();
					this._instancedBlock.SetFloatArray("_MaskMultiplier", this._fadeValues);
					this._instancedBlock.SetFloatArray("_LimitTo", this._limitToValues);
					this._bufferUnlit.DrawMeshInstanced(DecaliciousRenderer._cubeMesh, 0, key, 0, this._matrices, num, this._instancedBlock);
				}
			}
		}

		// Token: 0x06002782 RID: 10114 RVA: 0x000B83BC File Offset: 0x000B65BC
		private static void CreateBuffer(ref CommandBuffer buffer, Camera cam, string name, CameraEvent evt)
		{
			if (buffer == null)
			{
				foreach (CommandBuffer commandBuffer in cam.GetCommandBuffers(evt))
				{
					if (commandBuffer.name == name)
					{
						buffer = commandBuffer;
						break;
					}
				}
				if (buffer == null)
				{
					buffer = new CommandBuffer();
					buffer.name = name;
					cam.AddCommandBuffer(evt, buffer);
				}
			}
		}

		// Token: 0x06002783 RID: 10115 RVA: 0x000B8418 File Offset: 0x000B6618
		public void Add(Decal decal, GameObject limitTo)
		{
			if (limitTo)
			{
				this._limitToGameObjects.Add(limitTo);
			}
			Decal.DecalRenderMode renderMode = decal.RenderMode;
			if (renderMode == Decal.DecalRenderMode.Deferred)
			{
				this.AddDeferred(decal);
				return;
			}
			if (renderMode != Decal.DecalRenderMode.Unlit)
			{
				return;
			}
			this.AddUnlit(decal);
		}

		// Token: 0x06002784 RID: 10116 RVA: 0x000B8458 File Offset: 0x000B6658
		protected void AddDeferred(Decal decal)
		{
			if (!this._deferredDecals.ContainsKey(decal.Material))
			{
				this._deferredDecals.Add(decal.Material, new HashSet<Decal>
				{
					decal
				});
				return;
			}
			this._deferredDecals[decal.Material].Add(decal);
		}

		// Token: 0x06002785 RID: 10117 RVA: 0x000B84B0 File Offset: 0x000B66B0
		protected void AddUnlit(Decal decal)
		{
			if (!this._unlitDecals.ContainsKey(decal.Material))
			{
				this._unlitDecals.Add(decal.Material, new HashSet<Decal>
				{
					decal
				});
				return;
			}
			this._unlitDecals[decal.Material].Add(decal);
		}

		// Token: 0x040029B4 RID: 10676
		[HideInInspector]
		public bool UseInstancing = true;

		// Token: 0x040029B5 RID: 10677
		protected CommandBuffer _bufferDeferred;

		// Token: 0x040029B6 RID: 10678
		protected CommandBuffer _bufferUnlit;

		// Token: 0x040029B7 RID: 10679
		protected CommandBuffer _bufferLimitTo;

		// Token: 0x040029B8 RID: 10680
		protected Dictionary<Material, HashSet<Decal>> _deferredDecals;

		// Token: 0x040029B9 RID: 10681
		protected Dictionary<Material, HashSet<Decal>> _unlitDecals;

		// Token: 0x040029BA RID: 10682
		protected List<MeshRenderer> _limitToMeshRenderers;

		// Token: 0x040029BB RID: 10683
		protected List<SkinnedMeshRenderer> _limitToSkinnedMeshRenderers;

		// Token: 0x040029BC RID: 10684
		protected HashSet<GameObject> _limitToGameObjects;

		// Token: 0x040029BD RID: 10685
		protected List<Decal> _decalComponent;

		// Token: 0x040029BE RID: 10686
		protected List<MeshFilter> _meshFilterComponent;

		// Token: 0x040029BF RID: 10687
		protected const string _bufferBaseName = "Decalicious - ";

		// Token: 0x040029C0 RID: 10688
		protected const string _bufferDeferredName = "Decalicious - Deferred";

		// Token: 0x040029C1 RID: 10689
		protected const string _bufferUnlitName = "Decalicious - Unlit";

		// Token: 0x040029C2 RID: 10690
		protected const string _bufferLimitToName = "Decalicious - Limit To Game Objects";

		// Token: 0x040029C3 RID: 10691
		protected const CameraEvent _camEventDeferred = CameraEvent.BeforeReflections;

		// Token: 0x040029C4 RID: 10692
		protected const CameraEvent _camEventUnlit = CameraEvent.BeforeImageEffectsOpaque;

		// Token: 0x040029C5 RID: 10693
		protected const CameraEvent _camEventLimitTo = CameraEvent.AfterGBuffer;

		// Token: 0x040029C6 RID: 10694
		protected Camera _camera;

		// Token: 0x040029C7 RID: 10695
		protected bool _camLastKnownHDR;

		// Token: 0x040029C8 RID: 10696
		protected static Mesh _cubeMesh = null;

		// Token: 0x040029C9 RID: 10697
		protected Matrix4x4[] _matrices;

		// Token: 0x040029CA RID: 10698
		protected float[] _fadeValues;

		// Token: 0x040029CB RID: 10699
		protected float[] _limitToValues;

		// Token: 0x040029CC RID: 10700
		protected MaterialPropertyBlock _instancedBlock;

		// Token: 0x040029CD RID: 10701
		protected MaterialPropertyBlock _directBlock;

		// Token: 0x040029CE RID: 10702
		protected RenderTargetIdentifier[] _albedoRenderTarget;

		// Token: 0x040029CF RID: 10703
		protected RenderTargetIdentifier[] _normalRenderTarget;

		// Token: 0x040029D0 RID: 10704
		protected Material _materialLimitToGameObjects;

		// Token: 0x040029D1 RID: 10705
		protected static Vector4[] _avCoeff = new Vector4[7];
	}
}
