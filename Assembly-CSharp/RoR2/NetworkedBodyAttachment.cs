using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000368 RID: 872
	public sealed class NetworkedBodyAttachment : NetworkBehaviour
	{
		// Token: 0x060011E7 RID: 4583 RVA: 0x00058B54 File Offset: 0x00056D54
		private void OnSyncAttachedBodyObject(GameObject value)
		{
			if (NetworkServer.active)
			{
				return;
			}
			this.Network_attachedBodyObject = value;
			this.OnAttachedBodyObjectAssigned();
		}

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x060011E8 RID: 4584 RVA: 0x00058B6B File Offset: 0x00056D6B
		public GameObject attachedBodyObject
		{
			get
			{
				return this._attachedBodyObject;
			}
		}

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x060011E9 RID: 4585 RVA: 0x00058B73 File Offset: 0x00056D73
		// (set) Token: 0x060011EA RID: 4586 RVA: 0x00058B7B File Offset: 0x00056D7B
		public CharacterBody attachedBody { get; private set; }

		// Token: 0x060011EB RID: 4587 RVA: 0x00058B84 File Offset: 0x00056D84
		[Server]
		public void AttachToGameObjectAndSpawn([NotNull] GameObject newAttachedBodyObject)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.NetworkedBodyAttachment::AttachToGameObjectAndSpawn(UnityEngine.GameObject)' called on client");
				return;
			}
			if (this.attached)
			{
				Debug.LogErrorFormat("Can't attach object '{0}' to object '{1}', it's already been assigned to object '{2}'.", new object[]
				{
					base.gameObject,
					newAttachedBodyObject,
					this.attachedBodyObject
				});
				return;
			}
			this.Network_attachedBodyObject = newAttachedBodyObject;
			this.OnAttachedBodyObjectAssigned();
			NetworkConnection clientAuthorityOwner = newAttachedBodyObject.GetComponent<NetworkIdentity>().clientAuthorityOwner;
			if (clientAuthorityOwner == null)
			{
				NetworkServer.Spawn(base.gameObject);
				return;
			}
			NetworkServer.SpawnWithClientAuthority(base.gameObject, clientAuthorityOwner);
		}

		// Token: 0x060011EC RID: 4588 RVA: 0x00058C0C File Offset: 0x00056E0C
		private void OnAttachedBodyObjectAssigned()
		{
			if (this.attached)
			{
				return;
			}
			this.attached = true;
			if (this._attachedBodyObject)
			{
				this.attachedBody = this._attachedBodyObject.GetComponent<CharacterBody>();
				base.transform.SetParent(this._attachedBodyObject.transform);
				base.transform.localPosition = Vector3.zero;
			}
			INetworkedBodyAttachmentListener[] components = base.GetComponents<INetworkedBodyAttachmentListener>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].OnAttachedBodyAssigned(this);
			}
		}

		// Token: 0x060011ED RID: 4589 RVA: 0x00058C8B File Offset: 0x00056E8B
		public override void OnStartClient()
		{
			base.OnStartClient();
			this.OnSyncAttachedBodyObject(this.attachedBodyObject);
		}

		// Token: 0x060011EE RID: 4590 RVA: 0x00058C9F File Offset: 0x00056E9F
		private void FixedUpdate()
		{
			if (!this.attachedBodyObject)
			{
				if (NetworkServer.active)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
				return;
			}
		}

		// Token: 0x060011EF RID: 4591 RVA: 0x00058CC1 File Offset: 0x00056EC1
		private void OnValidate()
		{
			if (!base.GetComponent<NetworkIdentity>().localPlayerAuthority)
			{
				Debug.LogWarningFormat("NetworkedBodyAttachment: Object {0} NetworkIdentity needs localPlayerAuthority=true", new object[]
				{
					base.gameObject.name
				});
			}
		}

		// Token: 0x060011F1 RID: 4593 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x060011F2 RID: 4594 RVA: 0x00058CF0 File Offset: 0x00056EF0
		// (set) Token: 0x060011F3 RID: 4595 RVA: 0x00058D04 File Offset: 0x00056F04
		public GameObject Network_attachedBodyObject
		{
			get
			{
				return this._attachedBodyObject;
			}
			set
			{
				uint dirtyBit = 1u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncAttachedBodyObject(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVarGameObject(value, ref this._attachedBodyObject, dirtyBit, ref this.____attachedBodyObjectNetId);
			}
		}

		// Token: 0x060011F4 RID: 4596 RVA: 0x00058D54 File Offset: 0x00056F54
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this._attachedBodyObject);
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
				writer.Write(this._attachedBodyObject);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x060011F5 RID: 4597 RVA: 0x00058DC0 File Offset: 0x00056FC0
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.____attachedBodyObjectNetId = reader.ReadNetworkId();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.OnSyncAttachedBodyObject(reader.ReadGameObject());
			}
		}

		// Token: 0x060011F6 RID: 4598 RVA: 0x00058E01 File Offset: 0x00057001
		public override void PreStartClient()
		{
			if (!this.____attachedBodyObjectNetId.IsEmpty())
			{
				this.Network_attachedBodyObject = ClientScene.FindLocalObject(this.____attachedBodyObjectNetId);
			}
		}

		// Token: 0x04001606 RID: 5638
		[SyncVar(hook = "OnSyncAttachedBodyObject")]
		private GameObject _attachedBodyObject;

		// Token: 0x04001608 RID: 5640
		private bool attached;

		// Token: 0x04001609 RID: 5641
		private NetworkInstanceId ____attachedBodyObjectNetId;
	}
}
