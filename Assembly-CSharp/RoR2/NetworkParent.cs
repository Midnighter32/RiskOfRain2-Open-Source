using System;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200036A RID: 874
	public class NetworkParent : NetworkBehaviour
	{
		// Token: 0x060011F8 RID: 4600 RVA: 0x00058E25 File Offset: 0x00057025
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x060011F9 RID: 4601 RVA: 0x00058E33 File Offset: 0x00057033
		public override void OnStartServer()
		{
			this.ServerUpdateParent();
		}

		// Token: 0x060011FA RID: 4602 RVA: 0x00058E3B File Offset: 0x0005703B
		private void OnTransformParentChanged()
		{
			if (NetworkServer.active)
			{
				this.ServerUpdateParent();
			}
			this.transform.localPosition = Vector3.zero;
			this.transform.localRotation = Quaternion.identity;
			this.transform.localScale = Vector3.one;
		}

		// Token: 0x060011FB RID: 4603 RVA: 0x00058E7C File Offset: 0x0005707C
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

		// Token: 0x060011FC RID: 4604 RVA: 0x00058ED1 File Offset: 0x000570D1
		private void SetParentIdentifier(NetworkParent.ParentIdentifier newParentIdentifier)
		{
			this.NetworkparentIdentifier = newParentIdentifier;
			if (!NetworkServer.active)
			{
				this.transform.parent = this.parentIdentifier.Resolve();
			}
		}

		// Token: 0x060011FE RID: 4606 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x060011FF RID: 4607 RVA: 0x00058EF8 File Offset: 0x000570F8
		// (set) Token: 0x06001200 RID: 4608 RVA: 0x00058F0B File Offset: 0x0005710B
		public NetworkParent.ParentIdentifier NetworkparentIdentifier
		{
			get
			{
				return this.parentIdentifier;
			}
			set
			{
				uint dirtyBit = 1u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.SetParentIdentifier(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<NetworkParent.ParentIdentifier>(value, ref this.parentIdentifier, dirtyBit);
			}
		}

		// Token: 0x06001201 RID: 4609 RVA: 0x00058F4C File Offset: 0x0005714C
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				GeneratedNetworkCode._WriteParentIdentifier_NetworkParent(writer, this.parentIdentifier);
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
				GeneratedNetworkCode._WriteParentIdentifier_NetworkParent(writer, this.parentIdentifier);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001202 RID: 4610 RVA: 0x00058FB8 File Offset: 0x000571B8
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

		// Token: 0x0400160A RID: 5642
		private Transform cachedServerParentTransform;

		// Token: 0x0400160B RID: 5643
		private new Transform transform;

		// Token: 0x0400160C RID: 5644
		[SyncVar(hook = "SetParentIdentifier")]
		private NetworkParent.ParentIdentifier parentIdentifier;

		// Token: 0x0200036B RID: 875
		[Serializable]
		private struct ParentIdentifier : IEquatable<NetworkParent.ParentIdentifier>
		{
			// Token: 0x1700018E RID: 398
			// (get) Token: 0x06001203 RID: 4611 RVA: 0x00058FF9 File Offset: 0x000571F9
			// (set) Token: 0x06001204 RID: 4612 RVA: 0x00059003 File Offset: 0x00057203
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

			// Token: 0x06001205 RID: 4613 RVA: 0x00059010 File Offset: 0x00057210
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

			// Token: 0x06001206 RID: 4614 RVA: 0x00059048 File Offset: 0x00057248
			public ParentIdentifier(Transform parent)
			{
				this.parentNetworkIdentity = null;
				this.indexInParentChildLocatorPlusOne = 0;
				if (!parent)
				{
					return;
				}
				this.parentNetworkIdentity = parent.GetComponentInParent<NetworkIdentity>();
				if (!this.parentNetworkIdentity)
				{
					Debug.LogWarningFormat("NetworkParent cannot accept a non-null parent without a NetworkIdentity! parent={0}", new object[]
					{
						parent
					});
					return;
				}
				if (this.parentNetworkIdentity.gameObject == parent.gameObject)
				{
					return;
				}
				ChildLocator childLocator = NetworkParent.ParentIdentifier.LookUpChildLocator(this.parentNetworkIdentity.transform);
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

			// Token: 0x06001207 RID: 4615 RVA: 0x00059106 File Offset: 0x00057306
			public bool Equals(NetworkParent.ParentIdentifier other)
			{
				return object.Equals(this.parentNetworkIdentity, other.parentNetworkIdentity) && this.indexInParentChildLocatorPlusOne == other.indexInParentChildLocatorPlusOne;
			}

			// Token: 0x06001208 RID: 4616 RVA: 0x0005912B File Offset: 0x0005732B
			public override bool Equals(object obj)
			{
				return obj != null && obj is NetworkParent.ParentIdentifier && this.Equals((NetworkParent.ParentIdentifier)obj);
			}

			// Token: 0x06001209 RID: 4617 RVA: 0x00059148 File Offset: 0x00057348
			public override int GetHashCode()
			{
				return ((this.parentNetworkIdentity != null) ? this.parentNetworkIdentity.GetHashCode() : 0) * 397 ^ this.indexInParentChildLocatorPlusOne.GetHashCode();
			}

			// Token: 0x0600120A RID: 4618 RVA: 0x00059178 File Offset: 0x00057378
			public Transform Resolve()
			{
				if (!this.parentNetworkIdentity)
				{
					return null;
				}
				if (this.indexInParentChildLocatorPlusOne == 0)
				{
					return this.parentNetworkIdentity.transform;
				}
				ChildLocator childLocator = NetworkParent.ParentIdentifier.LookUpChildLocator(this.parentNetworkIdentity.transform);
				if (childLocator)
				{
					return childLocator.FindChild(this.indexInParentChildLocator);
				}
				return null;
			}

			// Token: 0x0400160D RID: 5645
			public NetworkIdentity parentNetworkIdentity;

			// Token: 0x0400160E RID: 5646
			public byte indexInParentChildLocatorPlusOne;
		}
	}
}
