using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000205 RID: 517
	[RequireComponent(typeof(ChildLocator))]
	public class FootstepHandler : MonoBehaviour
	{
		// Token: 0x06000B05 RID: 2821 RVA: 0x00030DCC File Offset: 0x0002EFCC
		private void Start()
		{
			this.childLocator = base.GetComponent<ChildLocator>();
			this.body = base.GetComponent<CharacterModel>().body;
			this.bodyInventory = (this.body ? this.body.inventory : null);
			this.animator = base.GetComponent<Animator>();
			if (this.enableFootstepDust)
			{
				this.footstepDustInstanceTransform = UnityEngine.Object.Instantiate<GameObject>(this.footstepDustPrefab, base.transform).transform;
				this.footstepDustInstanceParticleSystem = this.footstepDustInstanceTransform.GetComponent<ParticleSystem>();
				this.footstepDustInstanceShakeEmitter = this.footstepDustInstanceTransform.GetComponent<ShakeEmitter>();
			}
		}

		// Token: 0x06000B06 RID: 2822 RVA: 0x00030E6C File Offset: 0x0002F06C
		public void Footstep(AnimationEvent animationEvent)
		{
			if ((double)animationEvent.animatorClipInfo.weight > 0.5)
			{
				this.Footstep(animationEvent.stringParameter, (GameObject)animationEvent.objectReferenceParameter);
			}
		}

		// Token: 0x06000B07 RID: 2823 RVA: 0x00030EAC File Offset: 0x0002F0AC
		public void Footstep(string childName, GameObject footstepEffect)
		{
			if (!this.body)
			{
				return;
			}
			Transform transform = this.childLocator.FindChild(childName);
			if (transform)
			{
				Color color = Color.gray;
				RaycastHit raycastHit = default(RaycastHit);
				Vector3 position = transform.position;
				position.y += 1.5f;
				Debug.DrawRay(position, Vector3.down);
				if (Physics.Raycast(new Ray(position, Vector3.down), out raycastHit, 4f, LayerIndex.world.mask | LayerIndex.water.mask, QueryTriggerInteraction.Collide))
				{
					if (this.bodyInventory && this.bodyInventory.GetItemCount(ItemIndex.Hoof) > 0 && childName == "FootR")
					{
						Util.PlaySound("Play_item_proc_hoof", this.body.gameObject);
					}
					if (footstepEffect)
					{
						EffectManager.SimpleImpactEffect(footstepEffect, raycastHit.point, raycastHit.normal, false);
					}
					SurfaceDef objectSurfaceDef = SurfaceDefProvider.GetObjectSurfaceDef(raycastHit.collider, raycastHit.point);
					bool flag = false;
					if (objectSurfaceDef)
					{
						color = objectSurfaceDef.approximateColor;
						if (objectSurfaceDef.footstepEffectPrefab)
						{
							EffectManager.SpawnEffect(objectSurfaceDef.footstepEffectPrefab, new EffectData
							{
								origin = raycastHit.point,
								scale = this.body.radius
							}, false);
							flag = true;
						}
						if (!string.IsNullOrEmpty(objectSurfaceDef.materialSwitchString))
						{
							AkSoundEngine.SetSwitch("material", objectSurfaceDef.materialSwitchString, this.body.gameObject);
						}
					}
					else
					{
						Debug.LogFormat("{0} is missing surface def", new object[]
						{
							raycastHit.collider.gameObject
						});
					}
					if (this.footstepDustInstanceTransform && !flag)
					{
						this.footstepDustInstanceTransform.position = raycastHit.point;
						this.footstepDustInstanceParticleSystem.main.startColor = color;
						this.footstepDustInstanceParticleSystem.Play();
						if (this.footstepDustInstanceShakeEmitter)
						{
							this.footstepDustInstanceShakeEmitter.StartShake();
						}
					}
				}
				Util.PlaySound((!string.IsNullOrEmpty(this.sprintFootstepOverrideString) && this.body.isSprinting) ? this.sprintFootstepOverrideString : this.baseFootstepString, this.body.gameObject);
				return;
			}
			Debug.LogWarningFormat("Object {0} lacks ChildLocator entry \"{1}\" to handle Footstep event!", new object[]
			{
				base.gameObject.name,
				childName
			});
		}

		// Token: 0x04000B75 RID: 2933
		public string baseFootstepString;

		// Token: 0x04000B76 RID: 2934
		public string sprintFootstepOverrideString;

		// Token: 0x04000B77 RID: 2935
		public bool enableFootstepDust;

		// Token: 0x04000B78 RID: 2936
		public GameObject footstepDustPrefab;

		// Token: 0x04000B79 RID: 2937
		private ChildLocator childLocator;

		// Token: 0x04000B7A RID: 2938
		private Inventory bodyInventory;

		// Token: 0x04000B7B RID: 2939
		private Animator animator;

		// Token: 0x04000B7C RID: 2940
		private Transform footstepDustInstanceTransform;

		// Token: 0x04000B7D RID: 2941
		private ParticleSystem footstepDustInstanceParticleSystem;

		// Token: 0x04000B7E RID: 2942
		private ShakeEmitter footstepDustInstanceShakeEmitter;

		// Token: 0x04000B7F RID: 2943
		private CharacterBody body;
	}
}
