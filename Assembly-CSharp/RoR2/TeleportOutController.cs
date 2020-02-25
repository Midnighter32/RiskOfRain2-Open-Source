using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000352 RID: 850
	public class TeleportOutController : NetworkBehaviour
	{
		// Token: 0x060014A9 RID: 5289 RVA: 0x0005837C File Offset: 0x0005657C
		public static void AddTPOutEffect(CharacterModel characterModel, float beginAlpha, float endAlpha, float duration)
		{
			if (characterModel)
			{
				TemporaryOverlay temporaryOverlay = characterModel.gameObject.AddComponent<TemporaryOverlay>();
				temporaryOverlay.duration = duration;
				temporaryOverlay.animateShaderAlpha = true;
				temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, beginAlpha, 1f, endAlpha);
				temporaryOverlay.destroyComponentOnEnd = true;
				temporaryOverlay.originalMaterial = Resources.Load<Material>("Materials/matTPInOut");
				temporaryOverlay.AddToCharacerModel(characterModel);
			}
		}

		// Token: 0x060014AA RID: 5290 RVA: 0x000583E0 File Offset: 0x000565E0
		public override void OnStartClient()
		{
			base.OnStartClient();
			if (this.target)
			{
				ModelLocator component = this.target.GetComponent<ModelLocator>();
				if (component)
				{
					Transform modelTransform = component.modelTransform;
					if (modelTransform)
					{
						CharacterModel component2 = modelTransform.GetComponent<CharacterModel>();
						if (component2)
						{
							TeleportOutController.AddTPOutEffect(component2, 0f, 1f, 2f);
							if (component2.baseRendererInfos.Length != 0)
							{
								Renderer renderer = component2.baseRendererInfos[component2.baseRendererInfos.Length - 1].renderer;
								if (renderer)
								{
									ParticleSystem.ShapeModule shape = this.bodyGlowParticles.shape;
									if (renderer is MeshRenderer)
									{
										shape.shapeType = ParticleSystemShapeType.MeshRenderer;
										shape.meshRenderer = (renderer as MeshRenderer);
									}
									else if (renderer is SkinnedMeshRenderer)
									{
										shape.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
										shape.skinnedMeshRenderer = (renderer as SkinnedMeshRenderer);
									}
								}
							}
						}
					}
				}
			}
			this.bodyGlowParticles.Play();
		}

		// Token: 0x060014AB RID: 5291 RVA: 0x000584D8 File Offset: 0x000566D8
		public void FixedUpdate()
		{
			this.fixedAge += Time.fixedDeltaTime;
			if (this.fixedAge >= this.delayBeforePlayingSFX && !this.hasPlayedSFX)
			{
				this.hasPlayedSFX = true;
				Util.PlaySound(TeleportOutController.tpOutSoundString, this.target);
			}
			if (NetworkServer.active && this.fixedAge >= 2f && this.target)
			{
				GameObject teleportEffectPrefab = Run.instance.GetTeleportEffectPrefab(this.target);
				if (teleportEffectPrefab)
				{
					EffectManager.SpawnEffect(teleportEffectPrefab, new EffectData
					{
						origin = this.target.transform.position
					}, true);
				}
				UnityEngine.Object.Destroy(this.target);
			}
		}

		// Token: 0x060014AE RID: 5294 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x1700027C RID: 636
		// (get) Token: 0x060014AF RID: 5295 RVA: 0x000585AC File Offset: 0x000567AC
		// (set) Token: 0x060014B0 RID: 5296 RVA: 0x000585BF File Offset: 0x000567BF
		public GameObject Networktarget
		{
			get
			{
				return this.target;
			}
			[param: In]
			set
			{
				base.SetSyncVarGameObject(value, ref this.target, 1U, ref this.___targetNetId);
			}
		}

		// Token: 0x060014B1 RID: 5297 RVA: 0x000585DC File Offset: 0x000567DC
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.target);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.target);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x060014B2 RID: 5298 RVA: 0x00058648 File Offset: 0x00056848
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.___targetNetId = reader.ReadNetworkId();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.target = reader.ReadGameObject();
			}
		}

		// Token: 0x060014B3 RID: 5299 RVA: 0x00058689 File Offset: 0x00056889
		public override void PreStartClient()
		{
			if (!this.___targetNetId.IsEmpty())
			{
				this.Networktarget = ClientScene.FindLocalObject(this.___targetNetId);
			}
		}

		// Token: 0x04001335 RID: 4917
		[SyncVar]
		[NonSerialized]
		public GameObject target;

		// Token: 0x04001336 RID: 4918
		public ParticleSystem bodyGlowParticles;

		// Token: 0x04001337 RID: 4919
		public static string tpOutSoundString = "Play_UI_teleport_off_map";

		// Token: 0x04001338 RID: 4920
		private float fixedAge;

		// Token: 0x04001339 RID: 4921
		private const float warmupDuration = 2f;

		// Token: 0x0400133A RID: 4922
		public float delayBeforePlayingSFX = 1f;

		// Token: 0x0400133B RID: 4923
		private bool hasPlayedSFX;

		// Token: 0x0400133C RID: 4924
		private NetworkInstanceId ___targetNetId;
	}
}
