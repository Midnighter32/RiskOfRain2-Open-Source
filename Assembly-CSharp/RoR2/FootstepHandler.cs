using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002F6 RID: 758
	[RequireComponent(typeof(ChildLocator))]
	public class FootstepHandler : MonoBehaviour
	{
		// Token: 0x06000F4E RID: 3918 RVA: 0x0004B9AC File Offset: 0x00049BAC
		private void Start()
		{
			this.childLocator = base.GetComponent<ChildLocator>();
			this.body = base.GetComponent<CharacterModel>().body;
			CharacterMaster master = this.body.master;
			this.bodyInventory = ((master != null) ? master.inventory : null);
			this.animator = base.GetComponent<Animator>();
			if (this.enableFootstepDust)
			{
				this.footstepDustInstanceTransform = UnityEngine.Object.Instantiate<GameObject>(this.footstepDustPrefab, base.transform).transform;
				this.footstepDustInstanceParticleSystem = this.footstepDustInstanceTransform.GetComponent<ParticleSystem>();
				this.footstepDustInstanceShakeEmitter = this.footstepDustInstanceTransform.GetComponent<ShakeEmitter>();
			}
		}

		// Token: 0x06000F4F RID: 3919 RVA: 0x0004BA48 File Offset: 0x00049C48
		public void Footstep(AnimationEvent animationEvent)
		{
			if ((double)animationEvent.animatorClipInfo.weight > 0.5)
			{
				this.Footstep(animationEvent.stringParameter, (GameObject)animationEvent.objectReferenceParameter);
			}
		}

		// Token: 0x06000F50 RID: 3920 RVA: 0x0004BA88 File Offset: 0x00049C88
		public void Footstep(string childName, GameObject footstepEffect)
		{
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
						Util.PlaySound("Play_item_proc_hoof", transform.gameObject);
					}
					if (footstepEffect)
					{
						EffectManager.instance.SimpleImpactEffect(footstepEffect, raycastHit.point, raycastHit.normal, false);
					}
					SurfaceDef objectSurfaceDef = SurfaceDefProvider.GetObjectSurfaceDef(raycastHit.collider, raycastHit.point);
					bool flag = false;
					if (objectSurfaceDef)
					{
						color = objectSurfaceDef.approximateColor;
						if (objectSurfaceDef.footstepEffectPrefab)
						{
							EffectManager.instance.SpawnEffect(objectSurfaceDef.footstepEffectPrefab, new EffectData
							{
								origin = raycastHit.point,
								scale = this.body.radius
							}, false);
							flag = true;
						}
						if (!string.IsNullOrEmpty(objectSurfaceDef.materialSwitchString))
						{
							AkSoundEngine.SetSwitch("material", objectSurfaceDef.materialSwitchString, transform.gameObject);
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
				Util.PlaySound(this.baseFootstepString, transform.gameObject);
				return;
			}
			Debug.LogWarningFormat("Object {0} lacks ChildLocator entry \"{1}\" to handle Footstep event!", new object[]
			{
				base.gameObject.name,
				childName
			});
		}

		// Token: 0x04001376 RID: 4982
		public string baseFootstepString;

		// Token: 0x04001377 RID: 4983
		public bool enableFootstepDust;

		// Token: 0x04001378 RID: 4984
		public GameObject footstepDustPrefab;

		// Token: 0x04001379 RID: 4985
		private ChildLocator childLocator;

		// Token: 0x0400137A RID: 4986
		private Inventory bodyInventory;

		// Token: 0x0400137B RID: 4987
		private Animator animator;

		// Token: 0x0400137C RID: 4988
		private Transform footstepDustInstanceTransform;

		// Token: 0x0400137D RID: 4989
		private ParticleSystem footstepDustInstanceParticleSystem;

		// Token: 0x0400137E RID: 4990
		private ShakeEmitter footstepDustInstanceShakeEmitter;

		// Token: 0x0400137F RID: 4991
		private CharacterBody body;
	}
}
