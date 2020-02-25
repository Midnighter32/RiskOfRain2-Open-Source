using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000527 RID: 1319
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(ProjectileNetworkTransform))]
	public class ProjectileStickOnImpact : NetworkBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x17000348 RID: 840
		// (get) Token: 0x06001F2B RID: 7979 RVA: 0x00087791 File Offset: 0x00085991
		// (set) Token: 0x06001F2C RID: 7980 RVA: 0x00087799 File Offset: 0x00085999
		public Transform stuckTransform { get; private set; }

		// Token: 0x17000349 RID: 841
		// (get) Token: 0x06001F2D RID: 7981 RVA: 0x000877A2 File Offset: 0x000859A2
		// (set) Token: 0x06001F2E RID: 7982 RVA: 0x000877AA File Offset: 0x000859AA
		public CharacterBody stuckBody { get; private set; }

		// Token: 0x06001F2F RID: 7983 RVA: 0x000877B3 File Offset: 0x000859B3
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.rigidbody = base.GetComponent<Rigidbody>();
		}

		// Token: 0x06001F30 RID: 7984 RVA: 0x000877CD File Offset: 0x000859CD
		public void FixedUpdate()
		{
			this.UpdateSticking();
		}

		// Token: 0x06001F31 RID: 7985 RVA: 0x000877D5 File Offset: 0x000859D5
		private void OnEnable()
		{
			if (this.wasEverEnabled)
			{
				Collider component = base.GetComponent<Collider>();
				component.enabled = false;
				component.enabled = true;
			}
			this.wasEverEnabled = true;
		}

		// Token: 0x06001F32 RID: 7986 RVA: 0x000877F9 File Offset: 0x000859F9
		private void OnDisable()
		{
			if (NetworkServer.active)
			{
				this.Detach();
			}
		}

		// Token: 0x1700034A RID: 842
		// (get) Token: 0x06001F33 RID: 7987 RVA: 0x00087808 File Offset: 0x00085A08
		// (set) Token: 0x06001F34 RID: 7988 RVA: 0x00087810 File Offset: 0x00085A10
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

		// Token: 0x1700034B RID: 843
		// (get) Token: 0x06001F35 RID: 7989 RVA: 0x00087820 File Offset: 0x00085A20
		public bool stuck
		{
			get
			{
				return this.hitHurtboxIndex != -1;
			}
		}

		// Token: 0x06001F36 RID: 7990 RVA: 0x0008782E File Offset: 0x00085A2E
		[Server]
		public void Detach()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Projectile.ProjectileStickOnImpact::Detach()' called on client");
				return;
			}
			this.victim = null;
			this.stuckTransform = null;
			this.NetworkhitHurtboxIndex = -1;
			this.UpdateSticking();
		}

		// Token: 0x06001F37 RID: 7991 RVA: 0x00087860 File Offset: 0x00085A60
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
			if (!base.enabled)
			{
				return;
			}
			this.TrySticking(impactInfo.collider, impactInfo.estimatedImpactNormal);
		}

		// Token: 0x06001F38 RID: 7992 RVA: 0x00087880 File Offset: 0x00085A80
		private bool TrySticking(Collider hitCollider, Vector3 impactNormal)
		{
			if (this.victim)
			{
				return false;
			}
			GameObject gameObject = null;
			sbyte networkhitHurtboxIndex = -1;
			HurtBox component = hitCollider.GetComponent<HurtBox>();
			if (component)
			{
				HealthComponent healthComponent = component.healthComponent;
				if (healthComponent)
				{
					gameObject = healthComponent.gameObject;
				}
				networkhitHurtboxIndex = (sbyte)component.indexInGroup;
			}
			if (!gameObject && !this.ignoreWorld)
			{
				gameObject = hitCollider.gameObject;
				networkhitHurtboxIndex = -2;
			}
			if (gameObject == this.projectileController.owner || (this.ignoreCharacters && component))
			{
				gameObject = null;
				networkhitHurtboxIndex = -1;
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
				if (this.alignNormals && impactNormal != Vector3.zero)
				{
					base.transform.rotation = Util.QuaternionSafeLookRotation(impactNormal, base.transform.up);
				}
				Transform transform = hitCollider.transform;
				this.NetworklocalPosition = transform.InverseTransformPoint(base.transform.position);
				this.NetworklocalRotation = Quaternion.Inverse(transform.rotation) * base.transform.rotation;
				this.victim = gameObject;
				this.NetworkhitHurtboxIndex = networkhitHurtboxIndex;
				return true;
			}
			return false;
		}

		// Token: 0x06001F39 RID: 7993 RVA: 0x000879F0 File Offset: 0x00085BF0
		private void UpdateSticking()
		{
			bool flag = this.stuckTransform;
			if (flag)
			{
				base.transform.SetPositionAndRotation(this.stuckTransform.TransformPoint(this.localPosition), this.alignNormals ? (this.stuckTransform.rotation * this.localRotation) : base.transform.rotation);
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
						this.stuckBody = this.stuckTransform.GetComponent<CharacterBody>();
						if (this.stuckBody && this.stuckBody.hurtBoxGroup)
						{
							HurtBox hurtBox = this.stuckBody.hurtBoxGroup.hurtBoxes[(int)this.hitHurtboxIndex];
							this.stuckTransform = (hurtBox ? hurtBox.transform : null);
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

		// Token: 0x06001F3B RID: 7995 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x1700034C RID: 844
		// (get) Token: 0x06001F3C RID: 7996 RVA: 0x00087B6C File Offset: 0x00085D6C
		// (set) Token: 0x06001F3D RID: 7997 RVA: 0x00087B7F File Offset: 0x00085D7F
		public GameObject NetworksyncVictim
		{
			get
			{
				return this.syncVictim;
			}
			[param: In]
			set
			{
				base.SetSyncVarGameObject(value, ref this.syncVictim, 1U, ref this.___syncVictimNetId);
			}
		}

		// Token: 0x1700034D RID: 845
		// (get) Token: 0x06001F3E RID: 7998 RVA: 0x00087B9C File Offset: 0x00085D9C
		// (set) Token: 0x06001F3F RID: 7999 RVA: 0x00087BAF File Offset: 0x00085DAF
		public sbyte NetworkhitHurtboxIndex
		{
			get
			{
				return this.hitHurtboxIndex;
			}
			[param: In]
			set
			{
				base.SetSyncVar<sbyte>(value, ref this.hitHurtboxIndex, 2U);
			}
		}

		// Token: 0x1700034E RID: 846
		// (get) Token: 0x06001F40 RID: 8000 RVA: 0x00087BC4 File Offset: 0x00085DC4
		// (set) Token: 0x06001F41 RID: 8001 RVA: 0x00087BD7 File Offset: 0x00085DD7
		public Vector3 NetworklocalPosition
		{
			get
			{
				return this.localPosition;
			}
			[param: In]
			set
			{
				base.SetSyncVar<Vector3>(value, ref this.localPosition, 4U);
			}
		}

		// Token: 0x1700034F RID: 847
		// (get) Token: 0x06001F42 RID: 8002 RVA: 0x00087BEC File Offset: 0x00085DEC
		// (set) Token: 0x06001F43 RID: 8003 RVA: 0x00087BFF File Offset: 0x00085DFF
		public Quaternion NetworklocalRotation
		{
			get
			{
				return this.localRotation;
			}
			[param: In]
			set
			{
				base.SetSyncVar<Quaternion>(value, ref this.localRotation, 8U);
			}
		}

		// Token: 0x06001F44 RID: 8004 RVA: 0x00087C14 File Offset: 0x00085E14
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
			if ((base.syncVarDirtyBits & 1U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.syncVictim);
			}
			if ((base.syncVarDirtyBits & 2U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt32((uint)this.hitHurtboxIndex);
			}
			if ((base.syncVarDirtyBits & 4U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.localPosition);
			}
			if ((base.syncVarDirtyBits & 8U) != 0U)
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

		// Token: 0x06001F45 RID: 8005 RVA: 0x00087D3C File Offset: 0x00085F3C
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

		// Token: 0x06001F46 RID: 8006 RVA: 0x00087DEC File Offset: 0x00085FEC
		public override void PreStartClient()
		{
			if (!this.___syncVictimNetId.IsEmpty())
			{
				this.NetworksyncVictim = ClientScene.FindLocalObject(this.___syncVictimNetId);
			}
		}

		// Token: 0x04001CE1 RID: 7393
		public string stickSoundString;

		// Token: 0x04001CE2 RID: 7394
		public ParticleSystem[] stickParticleSystem;

		// Token: 0x04001CE3 RID: 7395
		public bool ignoreCharacters;

		// Token: 0x04001CE4 RID: 7396
		public bool ignoreWorld;

		// Token: 0x04001CE5 RID: 7397
		public bool alignNormals = true;

		// Token: 0x04001CE6 RID: 7398
		public UnityEvent stickEvent;

		// Token: 0x04001CE7 RID: 7399
		private ProjectileController projectileController;

		// Token: 0x04001CE8 RID: 7400
		private Rigidbody rigidbody;

		// Token: 0x04001CEB RID: 7403
		private bool wasEverEnabled;

		// Token: 0x04001CEC RID: 7404
		private GameObject _victim;

		// Token: 0x04001CED RID: 7405
		[SyncVar]
		private GameObject syncVictim;

		// Token: 0x04001CEE RID: 7406
		[SyncVar]
		private sbyte hitHurtboxIndex = -1;

		// Token: 0x04001CEF RID: 7407
		[SyncVar]
		private Vector3 localPosition;

		// Token: 0x04001CF0 RID: 7408
		[SyncVar]
		private Quaternion localRotation;

		// Token: 0x04001CF1 RID: 7409
		private NetworkInstanceId ___syncVictimNetId;
	}
}
