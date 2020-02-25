using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200048B RID: 1163
	[CreateAssetMenu(menuName = "RoR2/SkinDef")]
	public class SkinDef : ScriptableObject
	{
		// Token: 0x06001C63 RID: 7267 RVA: 0x000792D4 File Offset: 0x000774D4
		private void Bake()
		{
			if (this.runtimeSkin != null)
			{
				return;
			}
			this.runtimeSkin = new SkinDef.RuntimeSkin();
			SkinDef.<>c__DisplayClass14_0 CS$<>8__locals1;
			CS$<>8__locals1.rendererInfoTemplates = new List<SkinDef.RendererInfoTemplate>();
			CS$<>8__locals1.gameObjectActivationTemplates = new List<SkinDef.GameObjectActivationTemplate>();
			CS$<>8__locals1.meshReplacementTemplates = new List<SkinDef.MeshReplacementTemplate>();
			foreach (SkinDef skinDef in this.baseSkins)
			{
				skinDef.Bake();
				SkinDef.RendererInfoTemplate[] rendererInfoTemplates = skinDef.runtimeSkin.rendererInfoTemplates;
				for (int j = 0; j < rendererInfoTemplates.Length; j++)
				{
					SkinDef.<Bake>g__AddRendererInfoTemplate|14_0(rendererInfoTemplates[j], ref CS$<>8__locals1);
				}
				SkinDef.GameObjectActivationTemplate[] gameObjectActivationTemplates = skinDef.runtimeSkin.gameObjectActivationTemplates;
				for (int j = 0; j < gameObjectActivationTemplates.Length; j++)
				{
					SkinDef.<Bake>g__AddGameObjectActivationTemplate|14_1(gameObjectActivationTemplates[j], ref CS$<>8__locals1);
				}
				SkinDef.MeshReplacementTemplate[] meshReplacementTemplates = skinDef.runtimeSkin.meshReplacementTemplates;
				for (int j = 0; j < meshReplacementTemplates.Length; j++)
				{
					SkinDef.<Bake>g__AddMeshReplacementTemplate|14_2(meshReplacementTemplates[j], ref CS$<>8__locals1);
				}
			}
			for (int k = 0; k < this.rendererInfos.Length; k++)
			{
				ref CharacterModel.RendererInfo ptr = ref this.rendererInfos[k];
				if (!ptr.renderer)
				{
					Debug.LogErrorFormat("Skin {0} has an empty renderer field in its rendererInfos.", new object[]
					{
						this
					});
				}
				else
				{
					SkinDef.<Bake>g__AddRendererInfoTemplate|14_0(new SkinDef.RendererInfoTemplate
					{
						data = ptr,
						path = Util.BuildPrefabTransformPath(this.rootObject.transform, ptr.renderer.transform, false)
					}, ref CS$<>8__locals1);
				}
			}
			this.runtimeSkin.rendererInfoTemplates = CS$<>8__locals1.rendererInfoTemplates.ToArray();
			for (int l = 0; l < this.gameObjectActivations.Length; l++)
			{
				ref SkinDef.GameObjectActivation ptr2 = ref this.gameObjectActivations[l];
				if (!ptr2.gameObject)
				{
					Debug.LogErrorFormat("Skin {0} has an empty gameObject field in its gameObjectActivations.", new object[]
					{
						this
					});
				}
				else
				{
					SkinDef.<Bake>g__AddGameObjectActivationTemplate|14_1(new SkinDef.GameObjectActivationTemplate
					{
						shouldActivate = ptr2.shouldActivate,
						path = Util.BuildPrefabTransformPath(this.rootObject.transform, ptr2.gameObject.transform, false)
					}, ref CS$<>8__locals1);
				}
			}
			this.runtimeSkin.gameObjectActivationTemplates = CS$<>8__locals1.gameObjectActivationTemplates.ToArray();
			for (int m = 0; m < this.meshReplacements.Length; m++)
			{
				ref SkinDef.MeshReplacement ptr3 = ref this.meshReplacements[m];
				if (!ptr3.renderer)
				{
					Debug.LogErrorFormat("Skin {0} has an empty renderer field in its meshReplacements.", new object[]
					{
						this
					});
				}
				else
				{
					SkinDef.<Bake>g__AddMeshReplacementTemplate|14_2(new SkinDef.MeshReplacementTemplate
					{
						mesh = ptr3.mesh,
						path = Util.BuildPrefabTransformPath(this.rootObject.transform, ptr3.renderer.transform, false)
					}, ref CS$<>8__locals1);
				}
			}
			this.runtimeSkin.meshReplacementTemplates = CS$<>8__locals1.meshReplacementTemplates.ToArray();
		}

		// Token: 0x06001C64 RID: 7268 RVA: 0x000795C0 File Offset: 0x000777C0
		public void Apply(GameObject modelObject)
		{
			this.Bake();
			this.runtimeSkin.Apply(modelObject);
		}

		// Token: 0x06001C65 RID: 7269 RVA: 0x000795D4 File Offset: 0x000777D4
		private void Awake()
		{
			if (Application.IsPlaying(this))
			{
				this.Bake();
			}
		}

		// Token: 0x06001C67 RID: 7271 RVA: 0x00079610 File Offset: 0x00077810
		[CompilerGenerated]
		internal static void <Bake>g__AddRendererInfoTemplate|14_0(SkinDef.RendererInfoTemplate rendererInfoTemplate, ref SkinDef.<>c__DisplayClass14_0 A_1)
		{
			int i = 0;
			int count = A_1.rendererInfoTemplates.Count;
			while (i < count)
			{
				if (A_1.rendererInfoTemplates[i].path == rendererInfoTemplate.path)
				{
					A_1.rendererInfoTemplates[i] = rendererInfoTemplate;
					return;
				}
				i++;
			}
			A_1.rendererInfoTemplates.Add(rendererInfoTemplate);
		}

		// Token: 0x06001C68 RID: 7272 RVA: 0x00079670 File Offset: 0x00077870
		[CompilerGenerated]
		internal static void <Bake>g__AddGameObjectActivationTemplate|14_1(SkinDef.GameObjectActivationTemplate gameObjectActivationTemplate, ref SkinDef.<>c__DisplayClass14_0 A_1)
		{
			int i = 0;
			int count = A_1.gameObjectActivationTemplates.Count;
			while (i < count)
			{
				if (A_1.gameObjectActivationTemplates[i].path == gameObjectActivationTemplate.path)
				{
					A_1.gameObjectActivationTemplates[i] = gameObjectActivationTemplate;
					return;
				}
				i++;
			}
			A_1.gameObjectActivationTemplates.Add(gameObjectActivationTemplate);
		}

		// Token: 0x06001C69 RID: 7273 RVA: 0x000796D0 File Offset: 0x000778D0
		[CompilerGenerated]
		internal static void <Bake>g__AddMeshReplacementTemplate|14_2(SkinDef.MeshReplacementTemplate meshReplacementTemplate, ref SkinDef.<>c__DisplayClass14_0 A_1)
		{
			int i = 0;
			int count = A_1.meshReplacementTemplates.Count;
			while (i < count)
			{
				if (A_1.meshReplacementTemplates[i].path == meshReplacementTemplate.path)
				{
					A_1.meshReplacementTemplates[i] = meshReplacementTemplate;
					return;
				}
				i++;
			}
			A_1.meshReplacementTemplates.Add(meshReplacementTemplate);
		}

		// Token: 0x04001947 RID: 6471
		[Tooltip("The skins which will be applied before this one.")]
		public SkinDef[] baseSkins;

		// Token: 0x04001948 RID: 6472
		[ShowThumbnail]
		public Sprite icon;

		// Token: 0x04001949 RID: 6473
		public string nameToken;

		// Token: 0x0400194A RID: 6474
		public string unlockableName;

		// Token: 0x0400194B RID: 6475
		[PrefabReference]
		public GameObject rootObject;

		// Token: 0x0400194C RID: 6476
		public CharacterModel.RendererInfo[] rendererInfos = Array.Empty<CharacterModel.RendererInfo>();

		// Token: 0x0400194D RID: 6477
		public SkinDef.GameObjectActivation[] gameObjectActivations = Array.Empty<SkinDef.GameObjectActivation>();

		// Token: 0x0400194E RID: 6478
		public SkinDef.MeshReplacement[] meshReplacements = Array.Empty<SkinDef.MeshReplacement>();

		// Token: 0x0400194F RID: 6479
		private SkinDef.RuntimeSkin runtimeSkin;

		// Token: 0x0200048C RID: 1164
		[Serializable]
		public struct GameObjectActivation
		{
			// Token: 0x04001950 RID: 6480
			[PrefabReference]
			public GameObject gameObject;

			// Token: 0x04001951 RID: 6481
			public bool shouldActivate;
		}

		// Token: 0x0200048D RID: 1165
		[Serializable]
		public struct MeshReplacement
		{
			// Token: 0x04001952 RID: 6482
			[PrefabReference]
			public Renderer renderer;

			// Token: 0x04001953 RID: 6483
			public Mesh mesh;
		}

		// Token: 0x0200048E RID: 1166
		private struct RendererInfoTemplate
		{
			// Token: 0x04001954 RID: 6484
			public string path;

			// Token: 0x04001955 RID: 6485
			public CharacterModel.RendererInfo data;
		}

		// Token: 0x0200048F RID: 1167
		private struct GameObjectActivationTemplate
		{
			// Token: 0x04001956 RID: 6486
			public string path;

			// Token: 0x04001957 RID: 6487
			public bool shouldActivate;
		}

		// Token: 0x02000490 RID: 1168
		private struct MeshReplacementTemplate
		{
			// Token: 0x04001958 RID: 6488
			public string path;

			// Token: 0x04001959 RID: 6489
			public Mesh mesh;
		}

		// Token: 0x02000491 RID: 1169
		private class RuntimeSkin
		{
			// Token: 0x06001C6A RID: 7274 RVA: 0x00079730 File Offset: 0x00077930
			public void Apply(GameObject modelObject)
			{
				Transform transform = modelObject.transform;
				for (int i = 0; i < this.rendererInfoTemplates.Length; i++)
				{
					ref SkinDef.RendererInfoTemplate ptr = ref this.rendererInfoTemplates[i];
					CharacterModel.RendererInfo data = ptr.data;
					Transform transform2 = transform.Find(ptr.path);
					if (transform2)
					{
						Renderer component = transform2.GetComponent<Renderer>();
						if (component)
						{
							data.renderer = component;
							SkinDef.RuntimeSkin.rendererInfoBuffer.Add(data);
						}
						else
						{
							Debug.LogWarningFormat("No renderer at {0}/{1}", new object[]
							{
								transform.name,
								ptr.path
							});
						}
					}
					else
					{
						Debug.LogWarningFormat("Could not find transform \"{0}\" relative to \"{1}\".", new object[]
						{
							ptr.path,
							transform.name
						});
					}
				}
				modelObject.GetComponent<CharacterModel>().baseRendererInfos = SkinDef.RuntimeSkin.rendererInfoBuffer.ToArray();
				SkinDef.RuntimeSkin.rendererInfoBuffer.Clear();
				for (int j = 0; j < this.gameObjectActivationTemplates.Length; j++)
				{
					ref SkinDef.GameObjectActivationTemplate ptr2 = ref this.gameObjectActivationTemplates[j];
					bool shouldActivate = ptr2.shouldActivate;
					transform.Find(ptr2.path).gameObject.SetActive(shouldActivate);
				}
				for (int k = 0; k < this.meshReplacementTemplates.Length; k++)
				{
					ref SkinDef.MeshReplacementTemplate ptr3 = ref this.meshReplacementTemplates[k];
					Mesh mesh = ptr3.mesh;
					Renderer component2 = transform.Find(ptr3.path).GetComponent<Renderer>();
					SkinnedMeshRenderer skinnedMeshRenderer;
					if (component2 is MeshRenderer)
					{
						component2.GetComponent<MeshFilter>().sharedMesh = mesh;
					}
					else if ((skinnedMeshRenderer = (component2 as SkinnedMeshRenderer)) != null)
					{
						skinnedMeshRenderer.sharedMesh = mesh;
					}
				}
			}

			// Token: 0x0400195A RID: 6490
			public SkinDef.RendererInfoTemplate[] rendererInfoTemplates;

			// Token: 0x0400195B RID: 6491
			public SkinDef.GameObjectActivationTemplate[] gameObjectActivationTemplates;

			// Token: 0x0400195C RID: 6492
			public SkinDef.MeshReplacementTemplate[] meshReplacementTemplates;

			// Token: 0x0400195D RID: 6493
			private static readonly List<CharacterModel.RendererInfo> rendererInfoBuffer = new List<CharacterModel.RendererInfo>();
		}
	}
}
