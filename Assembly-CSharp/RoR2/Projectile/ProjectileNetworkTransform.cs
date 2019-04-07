using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200055A RID: 1370
	public class ProjectileNetworkTransform : NetworkBehaviour
	{
		// Token: 0x06001E85 RID: 7813 RVA: 0x0008FF49 File Offset: 0x0008E149
		public void SetValuesFromTransform()
		{
			this.NetworkserverPosition = this.transform.position;
			this.NetworkserverRotation = this.transform.rotation;
		}

		// Token: 0x170002A8 RID: 680
		// (get) Token: 0x06001E86 RID: 7814 RVA: 0x0008FF6D File Offset: 0x0008E16D
		private bool isPrediction
		{
			get
			{
				return this.projectileController && this.projectileController.isPrediction;
			}
		}

		// Token: 0x06001E87 RID: 7815 RVA: 0x0008FF8C File Offset: 0x0008E18C
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.transform = base.transform;
			this.NetworkserverPosition = this.transform.position;
			this.NetworkserverRotation = this.transform.rotation;
			this.rb = base.GetComponent<Rigidbody>();
		}

		// Token: 0x06001E88 RID: 7816 RVA: 0x0008FFE0 File Offset: 0x0008E1E0
		private void Start()
		{
			this.interpolatedPosition.interpDelay = this.GetNetworkSendInterval() * this.interpolationFactor;
			this.interpolatedPosition.SetValueImmediate(this.serverPosition);
			this.interpolatedRotation.SetValueImmediate(this.serverRotation);
			if (this.isPrediction)
			{
				base.enabled = false;
			}
			if (this.rb && !this.isPrediction && !NetworkServer.active)
			{
				this.rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
				this.rb.detectCollisions = false;
				this.rb.isKinematic = true;
			}
		}

		// Token: 0x06001E89 RID: 7817 RVA: 0x00090076 File Offset: 0x0008E276
		private void OnSyncPosition(Vector3 newPosition)
		{
			this.interpolatedPosition.PushValue(newPosition);
			this.NetworkserverPosition = newPosition;
		}

		// Token: 0x06001E8A RID: 7818 RVA: 0x0009008B File Offset: 0x0008E28B
		private void OnSyncRotation(Quaternion newRotation)
		{
			this.interpolatedRotation.PushValue(newRotation);
			this.NetworkserverRotation = newRotation;
		}

		// Token: 0x06001E8B RID: 7819 RVA: 0x000900A0 File Offset: 0x0008E2A0
		public override float GetNetworkSendInterval()
		{
			return this.positionTransmitInterval;
		}

		// Token: 0x06001E8C RID: 7820 RVA: 0x000900A8 File Offset: 0x0008E2A8
		private void FixedUpdate()
		{
			if (base.isServer)
			{
				this.interpolatedPosition.interpDelay = this.GetNetworkSendInterval() * this.interpolationFactor;
				this.NetworkserverPosition = this.transform.position;
				this.NetworkserverRotation = this.transform.rotation;
				this.interpolatedPosition.SetValueImmediate(this.serverPosition);
				this.interpolatedRotation.SetValueImmediate(this.serverRotation);
				return;
			}
			Vector3 currentValue = this.interpolatedPosition.GetCurrentValue(false);
			Quaternion currentValue2 = this.interpolatedRotation.GetCurrentValue(false);
			this.ApplyPositionAndRotation(currentValue, currentValue2);
		}

		// Token: 0x06001E8D RID: 7821 RVA: 0x0009013C File Offset: 0x0008E33C
		private void ApplyPositionAndRotation(Vector3 position, Quaternion rotation)
		{
			if (this.rb)
			{
				this.rb.MovePosition(position);
				this.rb.MoveRotation(rotation);
				return;
			}
			this.transform.position = position;
			this.transform.rotation = rotation;
		}

		// Token: 0x06001E8F RID: 7823 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x170002A9 RID: 681
		// (get) Token: 0x06001E90 RID: 7824 RVA: 0x0009019C File Offset: 0x0008E39C
		// (set) Token: 0x06001E91 RID: 7825 RVA: 0x000901AF File Offset: 0x0008E3AF
		public Vector3 NetworkserverPosition
		{
			get
			{
				return this.serverPosition;
			}
			set
			{
				uint dirtyBit = 1u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncPosition(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<Vector3>(value, ref this.serverPosition, dirtyBit);
			}
		}

		// Token: 0x170002AA RID: 682
		// (get) Token: 0x06001E92 RID: 7826 RVA: 0x000901F0 File Offset: 0x0008E3F0
		// (set) Token: 0x06001E93 RID: 7827 RVA: 0x00090203 File Offset: 0x0008E403
		public Quaternion NetworkserverRotation
		{
			get
			{
				return this.serverRotation;
			}
			set
			{
				uint dirtyBit = 2u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncRotation(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<Quaternion>(value, ref this.serverRotation, dirtyBit);
			}
		}

		// Token: 0x06001E94 RID: 7828 RVA: 0x00090244 File Offset: 0x0008E444
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.serverPosition);
				writer.Write(this.serverRotation);
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
				writer.Write(this.serverPosition);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.serverRotation);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001E95 RID: 7829 RVA: 0x000902F0 File Offset: 0x0008E4F0
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.serverPosition = reader.ReadVector3();
				this.serverRotation = reader.ReadQuaternion();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.OnSyncPosition(reader.ReadVector3());
			}
			if ((num & 2) != 0)
			{
				this.OnSyncRotation(reader.ReadQuaternion());
			}
		}

		// Token: 0x04002126 RID: 8486
		private ProjectileController projectileController;

		// Token: 0x04002127 RID: 8487
		private new Transform transform;

		// Token: 0x04002128 RID: 8488
		private Rigidbody rb;

		// Token: 0x04002129 RID: 8489
		[Tooltip("The delay in seconds between position network updates.")]
		public float positionTransmitInterval = 0.033333335f;

		// Token: 0x0400212A RID: 8490
		[Tooltip("The number of packets of buffers to have.")]
		public float interpolationFactor = 1f;

		// Token: 0x0400212B RID: 8491
		[SyncVar(hook = "OnSyncPosition")]
		private Vector3 serverPosition;

		// Token: 0x0400212C RID: 8492
		[SyncVar(hook = "OnSyncRotation")]
		private Quaternion serverRotation;

		// Token: 0x0400212D RID: 8493
		private NetworkLerpedVector3 interpolatedPosition;

		// Token: 0x0400212E RID: 8494
		private NetworkLerpedQuaternion interpolatedRotation;
	}
}
