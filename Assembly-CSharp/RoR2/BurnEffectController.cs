using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000272 RID: 626
	public class BurnEffectController : MonoBehaviour
	{
		// Token: 0x06000BC9 RID: 3017 RVA: 0x00039A04 File Offset: 0x00037C04
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Init()
		{
			BurnEffectController.normalEffect = new BurnEffectController.EffectParams
			{
				startSound = "Play_item_proc_igniteOnKill_Loop",
				stopSound = "Stop_item_proc_igniteOnKill_Loop",
				overlayMaterial = Resources.Load<Material>("Materials/matOnFire"),
				fireEffectPrefab = Resources.Load<GameObject>("Prefabs/FireEffect")
			};
			BurnEffectController.helfireEffect = new BurnEffectController.EffectParams
			{
				startSound = "Play_item_proc_igniteOnKill_Loop",
				stopSound = "Stop_item_proc_igniteOnKill_Loop",
				overlayMaterial = Resources.Load<Material>("Materials/matOnHelfire"),
				fireEffectPrefab = Resources.Load<GameObject>("Prefabs/HelfireEffect")
			};
		}

		// Token: 0x06000BCA RID: 3018 RVA: 0x00039A94 File Offset: 0x00037C94
		private void Start()
		{
			Util.PlaySound(this.effectType.startSound, base.gameObject);
			this.particles = new List<GameObject>();
			this.temporaryOverlay = base.gameObject.AddComponent<TemporaryOverlay>();
			this.temporaryOverlay.originalMaterial = this.effectType.overlayMaterial;
			CharacterModel component = this.target.GetComponent<CharacterModel>();
			if (component)
			{
				if (this.temporaryOverlay)
				{
					this.temporaryOverlay.AddToCharacerModel(component);
				}
				CharacterModel.RendererInfo[] rendererInfos = component.rendererInfos;
				for (int i = 0; i < rendererInfos.Length; i++)
				{
					if (!rendererInfos[i].ignoreOverlays)
					{
						GameObject gameObject = this.AddFireParticles(rendererInfos[i].renderer, this.target);
						if (gameObject)
						{
							this.particles.Add(gameObject);
						}
					}
				}
			}
		}

		// Token: 0x06000BCB RID: 3019 RVA: 0x00039B68 File Offset: 0x00037D68
		private void OnDestroy()
		{
			Util.PlaySound(this.effectType.stopSound, base.gameObject);
			if (this.temporaryOverlay)
			{
				UnityEngine.Object.Destroy(this.temporaryOverlay);
			}
			for (int i = 0; i < this.particles.Count; i++)
			{
				if (this.particles[i])
				{
					this.particles[i].GetComponent<ParticleSystem>().emission.enabled = false;
					this.particles[i].GetComponent<DestroyOnTimer>().enabled = true;
					this.particles[i].GetComponentInChildren<LightIntensityCurve>().enabled = true;
				}
			}
		}

		// Token: 0x06000BCC RID: 3020 RVA: 0x00039C1C File Offset: 0x00037E1C
		private GameObject AddFireParticles(Renderer modelRenderer, GameObject target)
		{
			if (modelRenderer is MeshRenderer || modelRenderer is SkinnedMeshRenderer)
			{
				GameObject fireEffectPrefab = this.effectType.fireEffectPrefab;
				fireEffectPrefab.GetComponent<ParticleSystem>();
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(fireEffectPrefab, modelRenderer.transform);
				ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
				ParticleSystem.ShapeModule shape = component.shape;
				if (modelRenderer)
				{
					if (modelRenderer is MeshRenderer)
					{
						shape.shapeType = ParticleSystemShapeType.MeshRenderer;
						shape.meshRenderer = (MeshRenderer)modelRenderer;
					}
					else if (modelRenderer is SkinnedMeshRenderer)
					{
						shape.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
						shape.skinnedMeshRenderer = (SkinnedMeshRenderer)modelRenderer;
					}
				}
				ParticleSystem.MainModule main = component.main;
				Vector3 lossyScale = modelRenderer.transform.lossyScale;
				component.gameObject.SetActive(true);
				BoneParticleController component2 = gameObject.GetComponent<BoneParticleController>();
				if (component2 && modelRenderer is SkinnedMeshRenderer)
				{
					component2.skinnedMeshRenderer = (SkinnedMeshRenderer)modelRenderer;
				}
				return gameObject;
			}
			return null;
		}

		// Token: 0x04000FBB RID: 4027
		private List<GameObject> particles;

		// Token: 0x04000FBC RID: 4028
		public GameObject target;

		// Token: 0x04000FBD RID: 4029
		private TemporaryOverlay temporaryOverlay;

		// Token: 0x04000FBE RID: 4030
		private int soundID;

		// Token: 0x04000FBF RID: 4031
		public BurnEffectController.EffectParams effectType = BurnEffectController.normalEffect;

		// Token: 0x04000FC0 RID: 4032
		public static BurnEffectController.EffectParams normalEffect;

		// Token: 0x04000FC1 RID: 4033
		public static BurnEffectController.EffectParams helfireEffect;

		// Token: 0x04000FC2 RID: 4034
		public float fireParticleSize = 5f;

		// Token: 0x02000273 RID: 627
		public class EffectParams
		{
			// Token: 0x04000FC3 RID: 4035
			public string startSound;

			// Token: 0x04000FC4 RID: 4036
			public string stopSound;

			// Token: 0x04000FC5 RID: 4037
			public Material overlayMaterial;

			// Token: 0x04000FC6 RID: 4038
			public GameObject fireEffectPrefab;
		}
	}
}
