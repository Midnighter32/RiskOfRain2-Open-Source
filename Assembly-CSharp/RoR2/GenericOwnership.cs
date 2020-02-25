using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200020D RID: 525
	public class GenericOwnership : NetworkBehaviour
	{
		// Token: 0x17000162 RID: 354
		// (get) Token: 0x06000B3D RID: 2877 RVA: 0x00031959 File Offset: 0x0002FB59
		// (set) Token: 0x06000B3C RID: 2876 RVA: 0x000318B4 File Offset: 0x0002FAB4
		public GameObject ownerObject
		{
			get
			{
				return this.cachedOwnerObject;
			}
			[Server]
			set
			{
				if (!NetworkServer.active)
				{
					Debug.LogWarning("[Server] function 'System.Void RoR2.GenericOwnership::set_ownerObject(UnityEngine.GameObject)' called on client");
					return;
				}
				if (!value)
				{
					value = null;
				}
				if (this.cachedOwnerObject == value)
				{
					return;
				}
				this.cachedOwnerObject = value;
				GameObject gameObject = this.cachedOwnerObject;
				NetworkInstanceId? networkInstanceId;
				if (gameObject == null)
				{
					networkInstanceId = null;
				}
				else
				{
					NetworkIdentity component = gameObject.GetComponent<NetworkIdentity>();
					networkInstanceId = ((component != null) ? new NetworkInstanceId?(component.netId) : null);
				}
				this.NetworkownerInstanceId = (networkInstanceId ?? NetworkInstanceId.Invalid);
				Action<GameObject> action = this.onOwnerChanged;
				if (action == null)
				{
					return;
				}
				action(this.cachedOwnerObject);
			}
		}

		// Token: 0x14000017 RID: 23
		// (add) Token: 0x06000B3E RID: 2878 RVA: 0x00031964 File Offset: 0x0002FB64
		// (remove) Token: 0x06000B3F RID: 2879 RVA: 0x0003199C File Offset: 0x0002FB9C
		public event Action<GameObject> onOwnerChanged;

		// Token: 0x06000B40 RID: 2880 RVA: 0x000319D1 File Offset: 0x0002FBD1
		private void SetOwnerClient(NetworkInstanceId id)
		{
			if (NetworkServer.active)
			{
				return;
			}
			this.cachedOwnerObject = ClientScene.FindLocalObject(id);
			Action<GameObject> action = this.onOwnerChanged;
			if (action == null)
			{
				return;
			}
			action(this.cachedOwnerObject);
		}

		// Token: 0x06000B41 RID: 2881 RVA: 0x000319FD File Offset: 0x0002FBFD
		public override void OnStartClient()
		{
			base.OnStartClient();
			this.SetOwnerClient(this.ownerInstanceId);
		}

		// Token: 0x06000B43 RID: 2883 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x06000B44 RID: 2884 RVA: 0x00031A14 File Offset: 0x0002FC14
		// (set) Token: 0x06000B45 RID: 2885 RVA: 0x00031A27 File Offset: 0x0002FC27
		public NetworkInstanceId NetworkownerInstanceId
		{
			get
			{
				return this.ownerInstanceId;
			}
			[param: In]
			set
			{
				uint dirtyBit = 1U;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.SetOwnerClient(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<NetworkInstanceId>(value, ref this.ownerInstanceId, dirtyBit);
			}
		}

		// Token: 0x06000B46 RID: 2886 RVA: 0x00031A68 File Offset: 0x0002FC68
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.ownerInstanceId);
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
				writer.Write(this.ownerInstanceId);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000B47 RID: 2887 RVA: 0x00031AD4 File Offset: 0x0002FCD4
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.ownerInstanceId = reader.ReadNetworkId();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.SetOwnerClient(reader.ReadNetworkId());
			}
		}

		// Token: 0x04000B98 RID: 2968
		[SyncVar(hook = "SetOwnerClient")]
		private NetworkInstanceId ownerInstanceId;

		// Token: 0x04000B99 RID: 2969
		private GameObject cachedOwnerObject;
	}
}
