using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200016A RID: 362
	public class BurnEffectController : MonoBehaviour
	{
		// Token: 0x060006BE RID: 1726 RVA: 0x0001B89C File Offset: 0x00019A9C
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

		// Token: 0x060006BF RID: 1727 RVA: 0x0001B92C File Offset: 0x00019B2C
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
				CharacterModel.RendererInfo[] baseRendererInfos = component.baseRendererInfos;
				for (int i = 0; i < baseRendererInfos.Length; i++)
				{
					if (!baseRendererInfos[i].ignoreOverlays)
					{
						GameObject gameObject = this.AddFireParticles(baseRendererInfos[i].renderer, this.target);
						if (gameObject)
						{
							this.particles.Add(gameObject);
						}
					}
				}
			}
		}

		// Token: 0x060006C0 RID: 1728 RVA: 0x0001BA00 File Offset: 0x00019C00
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

		// Token: 0x060006C1 RID: 1729 RVA: 0x0001BAB4 File Offset: 0x00019CB4
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

		// Token: 0x0400070D RID: 1805
		private List<GameObject> particles;

		// Token: 0x0400070E RID: 1806
		public GameObject target;

		// Token: 0x0400070F RID: 1807
		private TemporaryOverlay temporaryOverlay;

		// Token: 0x04000710 RID: 1808
		private int soundID;

		// Token: 0x04000711 RID: 1809
		public BurnEffectController.EffectParams effectType = BurnEffectController.normalEffect;

		// Token: 0x04000712 RID: 1810
		public static BurnEffectController.EffectParams normalEffect;

		// Token: 0x04000713 RID: 1811
		public static BurnEffectController.EffectParams helfireEffect;

		// Token: 0x04000714 RID: 1812
		public float fireParticleSize = 5f;

		// Token: 0x0200016B RID: 363
		public class EffectParams
		{
			// Token: 0x04000715 RID: 1813
			public string startSound;

			// Token: 0x04000716 RID: 1814
			public string stopSound;

			// Token: 0x04000717 RID: 1815
			public Material overlayMaterial;

			// Token: 0x04000718 RID: 1816
			public GameObject fireEffectPrefab;
		}
	}
}
