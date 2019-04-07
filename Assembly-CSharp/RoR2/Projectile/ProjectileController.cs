using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000541 RID: 1345
	[RequireComponent(typeof(TeamFilter))]
	public class ProjectileController : NetworkBehaviour
	{
		// Token: 0x170002A1 RID: 673
		// (get) Token: 0x06001E06 RID: 7686 RVA: 0x0008D750 File Offset: 0x0008B950
		// (set) Token: 0x06001E07 RID: 7687 RVA: 0x0008D758 File Offset: 0x0008B958
		public TeamFilter teamFilter { get; private set; }

		// Token: 0x170002A2 RID: 674
		// (get) Token: 0x06001E08 RID: 7688 RVA: 0x0008D761 File Offset: 0x0008B961
		// (set) Token: 0x06001E09 RID: 7689 RVA: 0x0008D769 File Offset: 0x0008B969
		public NetworkConnection clientAuthorityOwner { get; private set; }

		// Token: 0x06001E0A RID: 7690 RVA: 0x0008D774 File Offset: 0x0008B974
		private void Awake()
		{
			this.rigidbody = base.GetComponent<Rigidbody>();
			this.teamFilter = base.GetComponent<TeamFilter>();
			this.myColliders = base.GetComponents<Collider>();
			for (int i = 0; i < this.myColliders.Length; i++)
			{
				this.myColliders[i].enabled = false;
			}
		}

		// Token: 0x06001E0B RID: 7691 RVA: 0x0008D7C8 File Offset: 0x0008B9C8
		private void Start()
		{
			for (int i = 0; i < this.myColliders.Length; i++)
			{
				this.myColliders[i].enabled = true;
			}
			this.IgnoreCollisionsWithOwner();
			if (!this.isPrediction && !NetworkServer.active)
			{
				ProjectileManager.instance.OnClientProjectileReceived(this);
			}
			if (this.ghostPrefab && (this.isPrediction || !this.allowPrediction || !base.hasAuthority))
			{
				this.ghost = UnityEngine.Object.Instantiate<GameObject>(this.ghostPrefab, base.transform.position, base.transform.rotation).GetComponent<ProjectileGhostController>();
				if (this.isPrediction)
				{
					this.ghost.predictionTransform = base.transform;
				}
				else
				{
					this.ghost.authorityTransform = base.transform;
				}
			}
			if (this.isPrediction)
			{
				if (this.predictionEffect)
				{
					this.predictionEffect.SetActive(true);
				}
			}
			else if (this.authorityEffect)
			{
				this.authorityEffect.SetActive(true);
			}
			this.clientAuthorityOwner = base.GetComponent<NetworkIdentity>().clientAuthorityOwner;
		}

		// Token: 0x06001E0C RID: 7692 RVA: 0x0008D8E0 File Offset: 0x0008BAE0
		private void OnDestroy()
		{
			if (NetworkServer.active)
			{
				ProjectileManager.instance.OnServerProjectileDestroyed(this);
			}
			if (this.ghost && this.isPrediction)
			{
				UnityEngine.Object.Destroy(this.ghost.gameObject);
			}
		}

		// Token: 0x06001E0D RID: 7693 RVA: 0x0008D919 File Offset: 0x0008BB19
		private void OnEnable()
		{
			this.IgnoreCollisionsWithOwner();
		}

		// Token: 0x06001E0E RID: 7694 RVA: 0x0008D924 File Offset: 0x0008BB24
		private void IgnoreCollisionsWithOwner()
		{
			if (this.owner)
			{
				ModelLocator component = this.owner.GetComponent<ModelLocator>();
				if (component)
				{
					Transform modelTransform = component.modelTransform;
					if (modelTransform)
					{
						HurtBoxGroup component2 = modelTransform.GetComponent<HurtBoxGroup>();
						if (component2)
						{
							HurtBox[] hurtBoxes = component2.hurtBoxes;
							for (int i = 0; i < hurtBoxes.Length; i++)
							{
								foreach (Collider collider in hurtBoxes[i].GetComponents<Collider>())
								{
									for (int k = 0; k < this.myColliders.Length; k++)
									{
										Collider collider2 = this.myColliders[k];
										Physics.IgnoreCollision(collider, collider2);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06001E0F RID: 7695 RVA: 0x0008D9DF File Offset: 0x0008BBDF
		private static Vector3 EstimateContactPoint(ContactPoint[] contacts, Collider collider)
		{
			if (contacts.Length == 0)
			{
				return collider.transform.position;
			}
			return contacts[0].point;
		}

		// Token: 0x06001E10 RID: 7696 RVA: 0x0008D9FD File Offset: 0x0008BBFD
		private static Vector3 EstimateContactNormal(ContactPoint[] contacts)
		{
			if (contacts.Length == 0)
			{
				return Vector3.zero;
			}
			return contacts[0].normal;
		}

		// Token: 0x06001E11 RID: 7697 RVA: 0x0008DA18 File Offset: 0x0008BC18
		public void OnCollisionEnter(Collision collision)
		{
			if (NetworkServer.active || this.isPrediction)
			{
				ContactPoint[] contacts = collision.contacts;
				ProjectileImpactInfo impactInfo = new ProjectileImpactInfo
				{
					collider = collision.collider,
					estimatedPointOfImpact = ProjectileController.EstimateContactPoint(contacts, collision.collider),
					estimatedImpactNormal = ProjectileController.EstimateContactNormal(contacts)
				};
				IProjectileImpactBehavior[] components = base.GetComponents<IProjectileImpactBehavior>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].OnProjectileImpact(impactInfo);
				}
			}
		}

		// Token: 0x06001E13 RID: 7699 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x170002A3 RID: 675
		// (get) Token: 0x06001E14 RID: 7700 RVA: 0x0008DAB0 File Offset: 0x0008BCB0
		// (set) Token: 0x06001E15 RID: 7701 RVA: 0x0008DAC3 File Offset: 0x0008BCC3
		public ushort NetworkpredictionId
		{
			get
			{
				return this.predictionId;
			}
			set
			{
				base.SetSyncVar<ushort>(value, ref this.predictionId, 1u);
			}
		}

		// Token: 0x170002A4 RID: 676
		// (get) Token: 0x06001E16 RID: 7702 RVA: 0x0008DAD8 File Offset: 0x0008BCD8
		// (set) Token: 0x06001E17 RID: 7703 RVA: 0x0008DAEB File Offset: 0x0008BCEB
		public GameObject Networkowner
		{
			get
			{
				return this.owner;
			}
			set
			{
				base.SetSyncVarGameObject(value, ref this.owner, 2u, ref this.___ownerNetId);
			}
		}

		// Token: 0x06001E18 RID: 7704 RVA: 0x0008DB08 File Offset: 0x0008BD08
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.WritePackedUInt32((uint)this.predictionId);
				writer.Write(this.owner);
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
				writer.WritePackedUInt32((uint)this.predictionId);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.owner);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001E19 RID: 7705 RVA: 0x0008DBB4 File Offset: 0x0008BDB4
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.predictionId = (ushort)reader.ReadPackedUInt32();
				this.___ownerNetId = reader.ReadNetworkId();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.predictionId = (ushort)reader.ReadPackedUInt32();
			}
			if ((num & 2) != 0)
			{
				this.owner = reader.ReadGameObject();
			}
		}

		// Token: 0x06001E1A RID: 7706 RVA: 0x0008DC1A File Offset: 0x0008BE1A
		public override void PreStartClient()
		{
			if (!this.___ownerNetId.IsEmpty())
			{
				this.Networkowner = ClientScene.FindLocalObject(this.___ownerNetId);
			}
		}

		// Token: 0x04002072 RID: 8306
		[Tooltip("The prefab to instantiate as the visual representation of this projectile. The prefab must have a ProjectileGhostController attached.")]
		public GameObject ghostPrefab;

		// Token: 0x04002073 RID: 8307
		private Rigidbody rigidbody;

		// Token: 0x04002075 RID: 8309
		[HideInInspector]
		public ProjectileGhostController ghost;

		// Token: 0x04002076 RID: 8310
		[HideInInspector]
		public bool isPrediction;

		// Token: 0x04002077 RID: 8311
		public bool allowPrediction = true;

		// Token: 0x04002078 RID: 8312
		[SyncVar]
		[NonSerialized]
		public ushort predictionId;

		// Token: 0x04002079 RID: 8313
		[SyncVar]
		[HideInInspector]
		public GameObject owner;

		// Token: 0x0400207A RID: 8314
		[HideInInspector]
		public ProcChainMask procChainMask;

		// Token: 0x0400207C RID: 8316
		public float procCoefficient = 1f;

		// Token: 0x0400207D RID: 8317
		public GameObject authorityEffect;

		// Token: 0x0400207E RID: 8318
		public GameObject predictionEffect;

		// Token: 0x0400207F RID: 8319
		private Collider[] myColliders;

		// Token: 0x04002080 RID: 8320
		private NetworkInstanceId ___ownerNetId;
	}
}
