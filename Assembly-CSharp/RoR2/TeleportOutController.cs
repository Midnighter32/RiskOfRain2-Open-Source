using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003FA RID: 1018
	public class TeleportOutController : NetworkBehaviour
	{
		// Token: 0x060016AA RID: 5802 RVA: 0x0006C0DC File Offset: 0x0006A2DC
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

		// Token: 0x060016AB RID: 5803 RVA: 0x0006C140 File Offset: 0x0006A340
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
							if (component2.rendererInfos.Length != 0)
							{
								Renderer renderer = component2.rendererInfos[component2.rendererInfos.Length - 1].renderer;
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
			Util.PlaySound(TeleportOutController.tpOutSoundString, base.gameObject);
		}

		// Token: 0x060016AC RID: 5804 RVA: 0x0006C248 File Offset: 0x0006A448
		public void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.age += Time.fixedDeltaTime;
				if (this.age >= 2f && this.target)
				{
					GameObject teleportEffectPrefab = Run.instance.GetTeleportEffectPrefab(this.target);
					if (teleportEffectPrefab)
					{
						EffectManager.instance.SpawnEffect(teleportEffectPrefab, new EffectData
						{
							origin = this.target.transform.position
						}, true);
					}
					UnityEngine.Object.Destroy(this.target);
				}
			}
		}

		// Token: 0x060016AF RID: 5807 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x060016B0 RID: 5808 RVA: 0x0006C2E0 File Offset: 0x0006A4E0
		// (set) Token: 0x060016B1 RID: 5809 RVA: 0x0006C2F3 File Offset: 0x0006A4F3
		public GameObject Networktarget
		{
			get
			{
				return this.target;
			}
			set
			{
				base.SetSyncVarGameObject(value, ref this.target, 1u, ref this.___targetNetId);
			}
		}

		// Token: 0x060016B2 RID: 5810 RVA: 0x0006C310 File Offset: 0x0006A510
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.target);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1u) != 0u)
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

		// Token: 0x060016B3 RID: 5811 RVA: 0x0006C37C File Offset: 0x0006A57C
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

		// Token: 0x060016B4 RID: 5812 RVA: 0x0006C3BD File Offset: 0x0006A5BD
		public override void PreStartClient()
		{
			if (!this.___targetNetId.IsEmpty())
			{
				this.Networktarget = ClientScene.FindLocalObject(this.___targetNetId);
			}
		}

		// Token: 0x040019D0 RID: 6608
		[SyncVar]
		[NonSerialized]
		public GameObject target;

		// Token: 0x040019D1 RID: 6609
		public ParticleSystem bodyGlowParticles;

		// Token: 0x040019D2 RID: 6610
		public static string tpOutSoundString = "Play_UI_teleport_off_map";

		// Token: 0x040019D3 RID: 6611
		private float age;

		// Token: 0x040019D4 RID: 6612
		private const float warmupDuration = 2f;

		// Token: 0x040019D5 RID: 6613
		private NetworkInstanceId ___targetNetId;
	}
}
