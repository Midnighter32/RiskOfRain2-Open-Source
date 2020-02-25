using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000299 RID: 665
	public sealed class NetworkedBodyAttachment : NetworkBehaviour
	{
		// Token: 0x06000EC2 RID: 3778 RVA: 0x00041B90 File Offset: 0x0003FD90
		private void OnSyncAttachedBodyObject(GameObject value)
		{
			if (NetworkServer.active)
			{
				return;
			}
			this.Network_attachedBodyObject = value;
			this.OnAttachedBodyObjectAssigned();
		}

		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x06000EC3 RID: 3779 RVA: 0x00041BA7 File Offset: 0x0003FDA7
		public GameObject attachedBodyObject
		{
			get
			{
				return this._attachedBodyObject;
			}
		}

		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x06000EC4 RID: 3780 RVA: 0x00041BAF File Offset: 0x0003FDAF
		// (set) Token: 0x06000EC5 RID: 3781 RVA: 0x00041BB7 File Offset: 0x0003FDB7
		public CharacterBody attachedBody { get; private set; }

		// Token: 0x06000EC6 RID: 3782 RVA: 0x00041BC0 File Offset: 0x0003FDC0
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
			if (clientAuthorityOwner == null || this.forceHostAuthority)
			{
				NetworkServer.Spawn(base.gameObject);
				return;
			}
			NetworkServer.SpawnWithClientAuthority(base.gameObject, clientAuthorityOwner);
		}

		// Token: 0x06000EC7 RID: 3783 RVA: 0x00041C50 File Offset: 0x0003FE50
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
				if (this.shouldParentToAttachedBody)
				{
					base.transform.SetParent(this._attachedBodyObject.transform, false);
					base.transform.localPosition = Vector3.zero;
				}
			}
			INetworkedBodyAttachmentListener[] components = base.GetComponents<INetworkedBodyAttachmentListener>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].OnAttachedBodyAssigned(this);
			}
		}

		// Token: 0x06000EC8 RID: 3784 RVA: 0x00041CD8 File Offset: 0x0003FED8
		public override void OnStartClient()
		{
			base.OnStartClient();
			this.OnSyncAttachedBodyObject(this.attachedBodyObject);
		}

		// Token: 0x06000EC9 RID: 3785 RVA: 0x00041CEC File Offset: 0x0003FEEC
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

		// Token: 0x06000ECA RID: 3786 RVA: 0x00041D0E File Offset: 0x0003FF0E
		private void OnValidate()
		{
			if (!base.GetComponent<NetworkIdentity>().localPlayerAuthority && !this.forceHostAuthority)
			{
				Debug.LogWarningFormat("NetworkedBodyAttachment: Object {0} NetworkIdentity needs localPlayerAuthority=true", new object[]
				{
					base.gameObject.name
				});
			}
		}

		// Token: 0x06000ECC RID: 3788 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x06000ECD RID: 3789 RVA: 0x00041D54 File Offset: 0x0003FF54
		// (set) Token: 0x06000ECE RID: 3790 RVA: 0x00041D68 File Offset: 0x0003FF68
		public GameObject Network_attachedBodyObject
		{
			get
			{
				return this._attachedBodyObject;
			}
			[param: In]
			set
			{
				uint dirtyBit = 1U;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncAttachedBodyObject(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVarGameObject(value, ref this._attachedBodyObject, dirtyBit, ref this.____attachedBodyObjectNetId);
			}
		}

		// Token: 0x06000ECF RID: 3791 RVA: 0x00041DB8 File Offset: 0x0003FFB8
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this._attachedBodyObject);
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
				writer.Write(this._attachedBodyObject);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000ED0 RID: 3792 RVA: 0x00041E24 File Offset: 0x00040024
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

		// Token: 0x06000ED1 RID: 3793 RVA: 0x00041E65 File Offset: 0x00040065
		public override void PreStartClient()
		{
			if (!this.____attachedBodyObjectNetId.IsEmpty())
			{
				this.Network_attachedBodyObject = ClientScene.FindLocalObject(this.____attachedBodyObjectNetId);
			}
		}

		// Token: 0x04000EB2 RID: 3762
		[SyncVar(hook = "OnSyncAttachedBodyObject")]
		private GameObject _attachedBodyObject;

		// Token: 0x04000EB4 RID: 3764
		public bool shouldParentToAttachedBody = true;

		// Token: 0x04000EB5 RID: 3765
		public bool forceHostAuthority;

		// Token: 0x04000EB6 RID: 3766
		private bool attached;

		// Token: 0x04000EB7 RID: 3767
		private NetworkInstanceId ____attachedBodyObjectNetId;
	}
}
