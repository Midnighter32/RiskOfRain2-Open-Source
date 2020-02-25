using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x020004FE RID: 1278
	[RequireComponent(typeof(TeamFilter))]
	public class ProjectileController : NetworkBehaviour
	{
		// Token: 0x17000338 RID: 824
		// (get) Token: 0x06001E4B RID: 7755 RVA: 0x00082C19 File Offset: 0x00080E19
		// (set) Token: 0x06001E4C RID: 7756 RVA: 0x00082C21 File Offset: 0x00080E21
		public TeamFilter teamFilter { get; private set; }

		// Token: 0x17000339 RID: 825
		// (get) Token: 0x06001E4D RID: 7757 RVA: 0x00082C2A File Offset: 0x00080E2A
		// (set) Token: 0x06001E4E RID: 7758 RVA: 0x00082C32 File Offset: 0x00080E32
		public NetworkConnection clientAuthorityOwner { get; private set; }

		// Token: 0x1700033A RID: 826
		// (get) Token: 0x06001E4F RID: 7759 RVA: 0x00082C3B File Offset: 0x00080E3B
		// (set) Token: 0x06001E50 RID: 7760 RVA: 0x00082C43 File Offset: 0x00080E43
		public GameObject authorityEffect { get; private set; }

		// Token: 0x1700033B RID: 827
		// (get) Token: 0x06001E51 RID: 7761 RVA: 0x00082C4C File Offset: 0x00080E4C
		// (set) Token: 0x06001E52 RID: 7762 RVA: 0x00082C54 File Offset: 0x00080E54
		public GameObject predictionEffect { get; private set; }

		// Token: 0x06001E53 RID: 7763 RVA: 0x00082C60 File Offset: 0x00080E60
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

		// Token: 0x06001E54 RID: 7764 RVA: 0x00082CB4 File Offset: 0x00080EB4
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

		// Token: 0x06001E55 RID: 7765 RVA: 0x00082DCC File Offset: 0x00080FCC
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

		// Token: 0x06001E56 RID: 7766 RVA: 0x00082E05 File Offset: 0x00081005
		private void OnEnable()
		{
			InstanceTracker.Add<ProjectileController>(this);
			this.IgnoreCollisionsWithOwner();
		}

		// Token: 0x06001E57 RID: 7767 RVA: 0x00082E13 File Offset: 0x00081013
		private void OnDisable()
		{
			InstanceTracker.Remove<ProjectileController>(this);
		}

		// Token: 0x06001E58 RID: 7768 RVA: 0x00082E1C File Offset: 0x0008101C
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

		// Token: 0x06001E59 RID: 7769 RVA: 0x00082ED7 File Offset: 0x000810D7
		private static Vector3 EstimateContactPoint(ContactPoint[] contacts, Collider collider)
		{
			if (contacts.Length == 0)
			{
				return collider.transform.position;
			}
			return contacts[0].point;
		}

		// Token: 0x06001E5A RID: 7770 RVA: 0x00082EF5 File Offset: 0x000810F5
		private static Vector3 EstimateContactNormal(ContactPoint[] contacts)
		{
			if (contacts.Length == 0)
			{
				return Vector3.zero;
			}
			return contacts[0].normal;
		}

		// Token: 0x06001E5B RID: 7771 RVA: 0x00082F10 File Offset: 0x00081110
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

		// Token: 0x06001E5C RID: 7772 RVA: 0x00082F8D File Offset: 0x0008118D
		private void OnValidate()
		{
			if (!base.GetComponent<NetworkIdentity>().localPlayerAuthority)
			{
				Debug.LogWarningFormat(this, "ProjectileController: {0} does not have localPlayerAuthority=true", new object[]
				{
					base.gameObject
				});
			}
		}

		// Token: 0x06001E5E RID: 7774 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x1700033C RID: 828
		// (get) Token: 0x06001E5F RID: 7775 RVA: 0x00082FD8 File Offset: 0x000811D8
		// (set) Token: 0x06001E60 RID: 7776 RVA: 0x00082FEB File Offset: 0x000811EB
		public ushort NetworkpredictionId
		{
			get
			{
				return this.predictionId;
			}
			[param: In]
			set
			{
				base.SetSyncVar<ushort>(value, ref this.predictionId, 1U);
			}
		}

		// Token: 0x1700033D RID: 829
		// (get) Token: 0x06001E61 RID: 7777 RVA: 0x00083000 File Offset: 0x00081200
		// (set) Token: 0x06001E62 RID: 7778 RVA: 0x00083013 File Offset: 0x00081213
		public GameObject Networkowner
		{
			get
			{
				return this.owner;
			}
			[param: In]
			set
			{
				base.SetSyncVarGameObject(value, ref this.owner, 2U, ref this.___ownerNetId);
			}
		}

		// Token: 0x06001E63 RID: 7779 RVA: 0x00083030 File Offset: 0x00081230
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.WritePackedUInt32((uint)this.predictionId);
				writer.Write(this.owner);
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
				writer.WritePackedUInt32((uint)this.predictionId);
			}
			if ((base.syncVarDirtyBits & 2U) != 0U)
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

		// Token: 0x06001E64 RID: 7780 RVA: 0x000830DC File Offset: 0x000812DC
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

		// Token: 0x06001E65 RID: 7781 RVA: 0x00083142 File Offset: 0x00081342
		public override void PreStartClient()
		{
			if (!this.___ownerNetId.IsEmpty())
			{
				this.Networkowner = ClientScene.FindLocalObject(this.___ownerNetId);
			}
		}

		// Token: 0x04001B98 RID: 7064
		[HideInInspector]
		[Tooltip("This is assigned to the prefab automatically by ProjectileCatalog at runtime. Do not set this value manually.")]
		public int catalogIndex = -1;

		// Token: 0x04001B99 RID: 7065
		[Tooltip("The prefab to instantiate as the visual representation of this projectile. The prefab must have a ProjectileGhostController attached.")]
		public GameObject ghostPrefab;

		// Token: 0x04001B9A RID: 7066
		private Rigidbody rigidbody;

		// Token: 0x04001B9C RID: 7068
		[HideInInspector]
		public ProjectileGhostController ghost;

		// Token: 0x04001B9D RID: 7069
		[HideInInspector]
		public bool isPrediction;

		// Token: 0x04001B9E RID: 7070
		public bool allowPrediction = true;

		// Token: 0x04001B9F RID: 7071
		[SyncVar]
		[NonSerialized]
		public ushort predictionId;

		// Token: 0x04001BA0 RID: 7072
		[SyncVar]
		[HideInInspector]
		public GameObject owner;

		// Token: 0x04001BA1 RID: 7073
		[HideInInspector]
		public ProcChainMask procChainMask;

		// Token: 0x04001BA3 RID: 7075
		public float procCoefficient = 1f;

		// Token: 0x04001BA4 RID: 7076
		public GameObject target;

		// Token: 0x04001BA7 RID: 7079
		private Collider[] myColliders;

		// Token: 0x04001BA8 RID: 7080
		private NetworkInstanceId ___ownerNetId;
	}
}
