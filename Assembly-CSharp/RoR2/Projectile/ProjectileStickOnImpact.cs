using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000560 RID: 1376
	[RequireComponent(typeof(ProjectileNetworkTransform))]
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileStickOnImpact : NetworkBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001EB2 RID: 7858 RVA: 0x00090F97 File Offset: 0x0008F197
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.rigidbody = base.GetComponent<Rigidbody>();
		}

		// Token: 0x170002AC RID: 684
		// (get) Token: 0x06001EB3 RID: 7859 RVA: 0x00090FB1 File Offset: 0x0008F1B1
		// (set) Token: 0x06001EB4 RID: 7860 RVA: 0x00090FB9 File Offset: 0x0008F1B9
		public GameObject victim
		{
			get
			{
				return this._victim;
			}
			private set
			{
				this._victim = value;
				this.NetworksyncVictim = value;
			}
		}

		// Token: 0x170002AD RID: 685
		// (get) Token: 0x06001EB5 RID: 7861 RVA: 0x00090FC9 File Offset: 0x0008F1C9
		public bool stuck
		{
			get
			{
				return this.hitHurtboxIndex != -1;
			}
		}

		// Token: 0x06001EB6 RID: 7862 RVA: 0x00090FD8 File Offset: 0x0008F1D8
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
			if (!this.victim)
			{
				bool flag = false;
				this.NetworkhitHurtboxIndex = -1;
				HurtBox component = impactInfo.collider.GetComponent<HurtBox>();
				GameObject gameObject = null;
				if (component)
				{
					flag = true;
					HealthComponent healthComponent = component.healthComponent;
					if (healthComponent)
					{
						gameObject = healthComponent.gameObject;
					}
					this.NetworkhitHurtboxIndex = (sbyte)component.indexInGroup;
				}
				if (!gameObject && !this.ignoreWorld)
				{
					gameObject = impactInfo.collider.gameObject;
					this.NetworkhitHurtboxIndex = -2;
				}
				if (gameObject == this.projectileController.owner)
				{
					this.victim = null;
					this.NetworkhitHurtboxIndex = -1;
					return;
				}
				if (this.ignoreCharacters && flag)
				{
					gameObject = null;
					this.NetworkhitHurtboxIndex = -1;
				}
				if (gameObject)
				{
					this.stickEvent.Invoke();
					ParticleSystem[] array = this.stickParticleSystem;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].Play();
					}
					if (this.stickSoundString.Length > 0)
					{
						Util.PlaySound(this.stickSoundString, base.gameObject);
					}
				}
				Vector3 estimatedImpactNormal = impactInfo.estimatedImpactNormal;
				if (estimatedImpactNormal != Vector3.zero)
				{
					base.transform.rotation = Util.QuaternionSafeLookRotation(estimatedImpactNormal, base.transform.up);
				}
				Transform transform = impactInfo.collider.transform;
				this.NetworklocalPosition = transform.InverseTransformPoint(base.transform.position);
				this.NetworklocalRotation = Quaternion.Inverse(transform.rotation) * base.transform.rotation;
				this.victim = gameObject;
			}
		}

		// Token: 0x06001EB7 RID: 7863 RVA: 0x0009116C File Offset: 0x0008F36C
		public void FixedUpdate()
		{
			bool flag = this.stuckTransform;
			if (flag)
			{
				base.transform.SetPositionAndRotation(this.stuckTransform.TransformPoint(this.localPosition), this.stuckTransform.rotation * this.localRotation);
			}
			else
			{
				GameObject gameObject = NetworkServer.active ? this.victim : this.syncVictim;
				if (gameObject)
				{
					this.stuckTransform = gameObject.transform;
					flag = true;
					if (this.hitHurtboxIndex >= 0)
					{
						CharacterBody component = this.stuckTransform.GetComponent<CharacterBody>();
						if (component)
						{
							this.stuckTransform = component.hurtBoxGroup.hurtBoxes[(int)this.hitHurtboxIndex].transform;
						}
					}
				}
				else if (this.hitHurtboxIndex == -2 && !NetworkServer.active)
				{
					flag = true;
				}
			}
			if (NetworkServer.active)
			{
				if (this.rigidbody.isKinematic != flag)
				{
					if (flag)
					{
						this.rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
						this.rigidbody.isKinematic = true;
					}
					else
					{
						this.rigidbody.isKinematic = false;
						this.rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
					}
				}
				if (!flag)
				{
					this.NetworkhitHurtboxIndex = -1;
				}
			}
		}

		// Token: 0x06001EB9 RID: 7865 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x170002AE RID: 686
		// (get) Token: 0x06001EBA RID: 7866 RVA: 0x0009129C File Offset: 0x0008F49C
		// (set) Token: 0x06001EBB RID: 7867 RVA: 0x000912AF File Offset: 0x0008F4AF
		public GameObject NetworksyncVictim
		{
			get
			{
				return this.syncVictim;
			}
			set
			{
				base.SetSyncVarGameObject(value, ref this.syncVictim, 1u, ref this.___syncVictimNetId);
			}
		}

		// Token: 0x170002AF RID: 687
		// (get) Token: 0x06001EBC RID: 7868 RVA: 0x000912CC File Offset: 0x0008F4CC
		// (set) Token: 0x06001EBD RID: 7869 RVA: 0x000912DF File Offset: 0x0008F4DF
		public sbyte NetworkhitHurtboxIndex
		{
			get
			{
				return this.hitHurtboxIndex;
			}
			set
			{
				base.SetSyncVar<sbyte>(value, ref this.hitHurtboxIndex, 2u);
			}
		}

		// Token: 0x170002B0 RID: 688
		// (get) Token: 0x06001EBE RID: 7870 RVA: 0x000912F4 File Offset: 0x0008F4F4
		// (set) Token: 0x06001EBF RID: 7871 RVA: 0x00091307 File Offset: 0x0008F507
		public Vector3 NetworklocalPosition
		{
			get
			{
				return this.localPosition;
			}
			set
			{
				base.SetSyncVar<Vector3>(value, ref this.localPosition, 4u);
			}
		}

		// Token: 0x170002B1 RID: 689
		// (get) Token: 0x06001EC0 RID: 7872 RVA: 0x0009131C File Offset: 0x0008F51C
		// (set) Token: 0x06001EC1 RID: 7873 RVA: 0x0009132F File Offset: 0x0008F52F
		public Quaternion NetworklocalRotation
		{
			get
			{
				return this.localRotation;
			}
			set
			{
				base.SetSyncVar<Quaternion>(value, ref this.localRotation, 8u);
			}
		}

		// Token: 0x06001EC2 RID: 7874 RVA: 0x00091344 File Offset: 0x0008F544
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.syncVictim);
				writer.WritePackedUInt32((uint)this.hitHurtboxIndex);
				writer.Write(this.localPosition);
				writer.Write(this.localRotation);
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
				writer.Write(this.syncVictim);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt32((uint)this.hitHurtboxIndex);
			}
			if ((base.syncVarDirtyBits & 4u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.localPosition);
			}
			if ((base.syncVarDirtyBits & 8u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.localRotation);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001EC3 RID: 7875 RVA: 0x0009146C File Offset: 0x0008F66C
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.___syncVictimNetId = reader.ReadNetworkId();
				this.hitHurtboxIndex = (sbyte)reader.ReadPackedUInt32();
				this.localPosition = reader.ReadVector3();
				this.localRotation = reader.ReadQuaternion();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.syncVictim = reader.ReadGameObject();
			}
			if ((num & 2) != 0)
			{
				this.hitHurtboxIndex = (sbyte)reader.ReadPackedUInt32();
			}
			if ((num & 4) != 0)
			{
				this.localPosition = reader.ReadVector3();
			}
			if ((num & 8) != 0)
			{
				this.localRotation = reader.ReadQuaternion();
			}
		}

		// Token: 0x06001EC4 RID: 7876 RVA: 0x0009151C File Offset: 0x0008F71C
		public override void PreStartClient()
		{
			if (!this.___syncVictimNetId.IsEmpty())
			{
				this.NetworksyncVictim = ClientScene.FindLocalObject(this.___syncVictimNetId);
			}
		}

		// Token: 0x04002163 RID: 8547
		public string stickSoundString;

		// Token: 0x04002164 RID: 8548
		public ParticleSystem[] stickParticleSystem;

		// Token: 0x04002165 RID: 8549
		private ProjectileController projectileController;

		// Token: 0x04002166 RID: 8550
		private Rigidbody rigidbody;

		// Token: 0x04002167 RID: 8551
		private Transform stuckTransform;

		// Token: 0x04002168 RID: 8552
		public bool ignoreCharacters;

		// Token: 0x04002169 RID: 8553
		public bool ignoreWorld;

		// Token: 0x0400216A RID: 8554
		public UnityEvent stickEvent;

		// Token: 0x0400216B RID: 8555
		private GameObject _victim;

		// Token: 0x0400216C RID: 8556
		[SyncVar]
		private GameObject syncVictim;

		// Token: 0x0400216D RID: 8557
		[SyncVar]
		private sbyte hitHurtboxIndex = -1;

		// Token: 0x0400216E RID: 8558
		[SyncVar]
		private Vector3 localPosition;

		// Token: 0x0400216F RID: 8559
		[SyncVar]
		private Quaternion localRotation;

		// Token: 0x04002170 RID: 8560
		private NetworkInstanceId ___syncVictimNetId;
	}
}
