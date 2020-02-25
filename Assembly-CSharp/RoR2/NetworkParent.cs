using System;
using System.Runtime.InteropServices;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200029B RID: 667
	public class NetworkParent : NetworkBehaviour
	{
		// Token: 0x06000ED3 RID: 3795 RVA: 0x00041E89 File Offset: 0x00040089
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x06000ED4 RID: 3796 RVA: 0x00041E97 File Offset: 0x00040097
		public override void OnStartServer()
		{
			this.ServerUpdateParent();
		}

		// Token: 0x06000ED5 RID: 3797 RVA: 0x00041EA0 File Offset: 0x000400A0
		private void OnTransformParentChanged()
		{
			if (NetworkServer.active)
			{
				this.ServerUpdateParent();
			}
			if (this.transform.parent)
			{
				this.transform.localPosition = Vector3.zero;
				this.transform.localRotation = Quaternion.identity;
				this.transform.localScale = Vector3.one;
			}
		}

		// Token: 0x06000ED6 RID: 3798 RVA: 0x00041EFC File Offset: 0x000400FC
		[Server]
		private void ServerUpdateParent()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.NetworkParent::ServerUpdateParent()' called on client");
				return;
			}
			Transform transform = this.transform.parent;
			if (transform == this.cachedServerParentTransform)
			{
				return;
			}
			if (!transform)
			{
				transform = null;
			}
			this.cachedServerParentTransform = transform;
			this.SetParentIdentifier(new NetworkParent.ParentIdentifier(transform));
		}

		// Token: 0x06000ED7 RID: 3799 RVA: 0x00041F51 File Offset: 0x00040151
		public override void OnStartClient()
		{
			base.OnStartClient();
			this.SetParentIdentifier(this.parentIdentifier);
		}

		// Token: 0x06000ED8 RID: 3800 RVA: 0x00041F65 File Offset: 0x00040165
		private void SetParentIdentifier(NetworkParent.ParentIdentifier newParentIdentifier)
		{
			this.NetworkparentIdentifier = newParentIdentifier;
			if (!NetworkServer.active)
			{
				this.transform.parent = this.parentIdentifier.Resolve();
			}
		}

		// Token: 0x06000EDA RID: 3802 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x170001DA RID: 474
		// (get) Token: 0x06000EDB RID: 3803 RVA: 0x00041F8C File Offset: 0x0004018C
		// (set) Token: 0x06000EDC RID: 3804 RVA: 0x00041F9F File Offset: 0x0004019F
		public NetworkParent.ParentIdentifier NetworkparentIdentifier
		{
			get
			{
				return this.parentIdentifier;
			}
			[param: In]
			set
			{
				uint dirtyBit = 1U;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.SetParentIdentifier(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<NetworkParent.ParentIdentifier>(value, ref this.parentIdentifier, dirtyBit);
			}
		}

		// Token: 0x06000EDD RID: 3805 RVA: 0x00041FE0 File Offset: 0x000401E0
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				GeneratedNetworkCode._WriteParentIdentifier_NetworkParent(writer, this.parentIdentifier);
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
				GeneratedNetworkCode._WriteParentIdentifier_NetworkParent(writer, this.parentIdentifier);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000EDE RID: 3806 RVA: 0x0004204C File Offset: 0x0004024C
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.parentIdentifier = GeneratedNetworkCode._ReadParentIdentifier_NetworkParent(reader);
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.SetParentIdentifier(GeneratedNetworkCode._ReadParentIdentifier_NetworkParent(reader));
			}
		}

		// Token: 0x04000EB8 RID: 3768
		private Transform cachedServerParentTransform;

		// Token: 0x04000EB9 RID: 3769
		private new Transform transform;

		// Token: 0x04000EBA RID: 3770
		[SyncVar(hook = "SetParentIdentifier")]
		private NetworkParent.ParentIdentifier parentIdentifier;

		// Token: 0x0200029C RID: 668
		[Serializable]
		private struct ParentIdentifier : IEquatable<NetworkParent.ParentIdentifier>
		{
			// Token: 0x170001DB RID: 475
			// (get) Token: 0x06000EDF RID: 3807 RVA: 0x0004208D File Offset: 0x0004028D
			// (set) Token: 0x06000EE0 RID: 3808 RVA: 0x00042097 File Offset: 0x00040297
			public int indexInParentChildLocator
			{
				get
				{
					return (int)(this.indexInParentChildLocatorPlusOne - 1);
				}
				set
				{
					this.indexInParentChildLocatorPlusOne = (byte)(value + 1);
				}
			}

			// Token: 0x06000EE1 RID: 3809 RVA: 0x000420A4 File Offset: 0x000402A4
			private static ChildLocator LookUpChildLocator(Transform rootObject)
			{
				ModelLocator component = rootObject.GetComponent<ModelLocator>();
				if (!component)
				{
					return null;
				}
				Transform modelTransform = component.modelTransform;
				if (!modelTransform)
				{
					return null;
				}
				return modelTransform.GetComponent<ChildLocator>();
			}

			// Token: 0x06000EE2 RID: 3810 RVA: 0x000420DC File Offset: 0x000402DC
			public ParentIdentifier(Transform parent)
			{
				this.parentNetworkInstanceId = NetworkInstanceId.Invalid;
				this.indexInParentChildLocatorPlusOne = 0;
				if (!parent)
				{
					return;
				}
				NetworkIdentity componentInParent = parent.GetComponentInParent<NetworkIdentity>();
				if (!componentInParent)
				{
					Debug.LogWarningFormat("NetworkParent cannot accept a non-null parent without a NetworkIdentity! parent={0}", new object[]
					{
						parent
					});
					return;
				}
				this.parentNetworkInstanceId = componentInParent.netId;
				if (componentInParent.gameObject == parent.gameObject)
				{
					return;
				}
				ChildLocator childLocator = NetworkParent.ParentIdentifier.LookUpChildLocator(componentInParent.transform);
				if (!childLocator)
				{
					Debug.LogWarningFormat("NetworkParent can only be parented directly to another object with a NetworkIdentity or an object registered in the ChildLocator of a a model of an object with a NetworkIdentity. parent={0}", new object[]
					{
						parent
					});
					return;
				}
				this.indexInParentChildLocator = childLocator.FindChildIndex(parent);
				if (this.indexInParentChildLocatorPlusOne == 0)
				{
					Debug.LogWarningFormat("NetowrkParent parent={0} is not registered in a ChildLocator.", new object[]
					{
						parent
					});
					return;
				}
			}

			// Token: 0x06000EE3 RID: 3811 RVA: 0x00042196 File Offset: 0x00040396
			public bool Equals(NetworkParent.ParentIdentifier other)
			{
				return object.Equals(this.parentNetworkInstanceId, other.parentNetworkInstanceId) && this.indexInParentChildLocatorPlusOne == other.indexInParentChildLocatorPlusOne;
			}

			// Token: 0x06000EE4 RID: 3812 RVA: 0x000421C5 File Offset: 0x000403C5
			public override bool Equals(object obj)
			{
				return obj != null && obj is NetworkParent.ParentIdentifier && this.Equals((NetworkParent.ParentIdentifier)obj);
			}

			// Token: 0x06000EE5 RID: 3813 RVA: 0x000421E4 File Offset: 0x000403E4
			public Transform Resolve()
			{
				GameObject gameObject = Util.FindNetworkObject(this.parentNetworkInstanceId);
				NetworkIdentity networkIdentity = gameObject ? gameObject.GetComponent<NetworkIdentity>() : null;
				if (!networkIdentity)
				{
					return null;
				}
				if (this.indexInParentChildLocatorPlusOne == 0)
				{
					return networkIdentity.transform;
				}
				ChildLocator childLocator = NetworkParent.ParentIdentifier.LookUpChildLocator(networkIdentity.transform);
				if (childLocator)
				{
					return childLocator.FindChild(this.indexInParentChildLocator);
				}
				return null;
			}

			// Token: 0x04000EBB RID: 3771
			public byte indexInParentChildLocatorPlusOne;

			// Token: 0x04000EBC RID: 3772
			public NetworkInstanceId parentNetworkInstanceId;
		}
	}
}
