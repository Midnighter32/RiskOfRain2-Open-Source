using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000520 RID: 1312
	public class ProjectileNetworkTransform : NetworkBehaviour
	{
		// Token: 0x06001EF6 RID: 7926 RVA: 0x0008629A File Offset: 0x0008449A
		public void SetValuesFromTransform()
		{
			this.NetworkserverPosition = this.transform.position;
			this.NetworkserverRotation = this.transform.rotation;
		}

		// Token: 0x17000344 RID: 836
		// (get) Token: 0x06001EF7 RID: 7927 RVA: 0x000862BE File Offset: 0x000844BE
		private bool isPrediction
		{
			get
			{
				return this.projectileController && this.projectileController.isPrediction;
			}
		}

		// Token: 0x06001EF8 RID: 7928 RVA: 0x000862DC File Offset: 0x000844DC
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.interpolatedTransform = base.GetComponent<InterpolatedTransform>();
			this.transform = base.transform;
			this.NetworkserverPosition = this.transform.position;
			this.NetworkserverRotation = this.transform.rotation;
			this.rb = base.GetComponent<Rigidbody>();
		}

		// Token: 0x06001EF9 RID: 7929 RVA: 0x0008633C File Offset: 0x0008453C
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
				this.rb.detectCollisions = this.allowClientsideCollision;
				this.rb.isKinematic = true;
			}
		}

		// Token: 0x06001EFA RID: 7930 RVA: 0x000863D7 File Offset: 0x000845D7
		private void OnSyncPosition(Vector3 newPosition)
		{
			this.interpolatedPosition.PushValue(newPosition);
			this.NetworkserverPosition = newPosition;
		}

		// Token: 0x06001EFB RID: 7931 RVA: 0x000863EC File Offset: 0x000845EC
		private void OnSyncRotation(Quaternion newRotation)
		{
			this.interpolatedRotation.PushValue(newRotation);
			this.NetworkserverRotation = newRotation;
		}

		// Token: 0x06001EFC RID: 7932 RVA: 0x00086401 File Offset: 0x00084601
		public override float GetNetworkSendInterval()
		{
			return this.positionTransmitInterval;
		}

		// Token: 0x06001EFD RID: 7933 RVA: 0x0008640C File Offset: 0x0008460C
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

		// Token: 0x06001EFE RID: 7934 RVA: 0x000864A0 File Offset: 0x000846A0
		private void ApplyPositionAndRotation(Vector3 position, Quaternion rotation)
		{
			if (this.rb && !this.interpolatedTransform)
			{
				this.rb.MovePosition(position);
				this.rb.MoveRotation(rotation);
				return;
			}
			this.transform.position = position;
			this.transform.rotation = rotation;
		}

		// Token: 0x06001F00 RID: 7936 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x17000345 RID: 837
		// (get) Token: 0x06001F01 RID: 7937 RVA: 0x00086518 File Offset: 0x00084718
		// (set) Token: 0x06001F02 RID: 7938 RVA: 0x0008652B File Offset: 0x0008472B
		public Vector3 NetworkserverPosition
		{
			get
			{
				return this.serverPosition;
			}
			[param: In]
			set
			{
				uint dirtyBit = 1U;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncPosition(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<Vector3>(value, ref this.serverPosition, dirtyBit);
			}
		}

		// Token: 0x17000346 RID: 838
		// (get) Token: 0x06001F03 RID: 7939 RVA: 0x0008656C File Offset: 0x0008476C
		// (set) Token: 0x06001F04 RID: 7940 RVA: 0x0008657F File Offset: 0x0008477F
		public Quaternion NetworkserverRotation
		{
			get
			{
				return this.serverRotation;
			}
			[param: In]
			set
			{
				uint dirtyBit = 2U;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncRotation(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<Quaternion>(value, ref this.serverRotation, dirtyBit);
			}
		}

		// Token: 0x06001F05 RID: 7941 RVA: 0x000865C0 File Offset: 0x000847C0
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.serverPosition);
				writer.Write(this.serverRotation);
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
				writer.Write(this.serverPosition);
			}
			if ((base.syncVarDirtyBits & 2U) != 0U)
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

		// Token: 0x06001F06 RID: 7942 RVA: 0x0008666C File Offset: 0x0008486C
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

		// Token: 0x04001C86 RID: 7302
		private ProjectileController projectileController;

		// Token: 0x04001C87 RID: 7303
		private new Transform transform;

		// Token: 0x04001C88 RID: 7304
		private Rigidbody rb;

		// Token: 0x04001C89 RID: 7305
		private InterpolatedTransform interpolatedTransform;

		// Token: 0x04001C8A RID: 7306
		[Tooltip("The delay in seconds between position network updates.")]
		public float positionTransmitInterval = 0.033333335f;

		// Token: 0x04001C8B RID: 7307
		[Tooltip("The number of packets of buffers to have.")]
		public float interpolationFactor = 1f;

		// Token: 0x04001C8C RID: 7308
		public bool allowClientsideCollision;

		// Token: 0x04001C8D RID: 7309
		[SyncVar(hook = "OnSyncPosition")]
		private Vector3 serverPosition;

		// Token: 0x04001C8E RID: 7310
		[SyncVar(hook = "OnSyncRotation")]
		private Quaternion serverRotation;

		// Token: 0x04001C8F RID: 7311
		private NetworkLerpedVector3 interpolatedPosition;

		// Token: 0x04001C90 RID: 7312
		private NetworkLerpedQuaternion interpolatedRotation;
	}
}
