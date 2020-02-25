using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200048A RID: 1162
	[RequireComponent(typeof(VehicleSeat))]
	[RequireComponent(typeof(Rigidbody))]
	public class ZiplineVehicle : NetworkBehaviour
	{
		// Token: 0x1700031E RID: 798
		// (get) Token: 0x06001C55 RID: 7253 RVA: 0x00078F54 File Offset: 0x00077154
		// (set) Token: 0x06001C56 RID: 7254 RVA: 0x00078F5C File Offset: 0x0007715C
		public VehicleSeat vehicleSeat { get; private set; }

		// Token: 0x06001C57 RID: 7255 RVA: 0x00078F68 File Offset: 0x00077168
		private void Awake()
		{
			this.vehicleSeat = base.GetComponent<VehicleSeat>();
			this.rigidbody = base.GetComponent<Rigidbody>();
			this.vehicleSeat.onPassengerEnter += this.OnPassengerEnter;
			this.vehicleSeat.onPassengerExit += this.OnPassengerExit;
		}

		// Token: 0x06001C58 RID: 7256 RVA: 0x00078FBB File Offset: 0x000771BB
		private void OnPassengerEnter(GameObject passenger)
		{
			this.currentPassenger = passenger;
			this.startPoint = base.transform.position;
			this.startTravelFixedTime = Run.FixedTimeStamp.now;
			this.startTravelTime = Run.TimeStamp.now;
		}

		// Token: 0x06001C59 RID: 7257 RVA: 0x00078FEC File Offset: 0x000771EC
		private void SetTravelDistance(float time)
		{
			Vector3 a = this.endPoint - this.startPoint;
			float magnitude = a.magnitude;
			Vector3 a2 = a / magnitude;
			float num = HGPhysics.CalculateDistance(0f, this.acceleration, time);
			bool flag = false;
			if (num > magnitude)
			{
				num = magnitude;
				flag = true;
			}
			this.rigidbody.MovePosition(this.startPoint + a2 * num);
			this.rigidbody.velocity = a2 * (this.acceleration * time);
			if (NetworkServer.active && flag)
			{
				this.vehicleSeat.EjectPassenger(this.currentPassenger);
				return;
			}
		}

		// Token: 0x06001C5A RID: 7258 RVA: 0x0007908B File Offset: 0x0007728B
		private void Update()
		{
			bool hasPassed = this.startTravelTime.hasPassed;
		}

		// Token: 0x06001C5B RID: 7259 RVA: 0x0007909C File Offset: 0x0007729C
		private void FixedUpdate()
		{
			if (this.startTravelFixedTime.hasPassed)
			{
				this.SetTravelDistance(this.startTravelFixedTime.timeSince);
			}
			if (NetworkServer.active && this.currentPassenger)
			{
				Vector3 normalized = (this.endPoint - base.transform.position).normalized;
				if (Vector3.Dot(normalized, this.travelDirection) < 0f)
				{
					this.vehicleSeat.EjectPassenger(this.currentPassenger);
					return;
				}
				float fixedDeltaTime = Time.fixedDeltaTime;
				Vector3 vector = this.rigidbody.velocity;
				vector += this.travelDirection * (this.acceleration * fixedDeltaTime);
				float sqrMagnitude = vector.sqrMagnitude;
				if (sqrMagnitude > this.maxSpeed * this.maxSpeed)
				{
					float num = Mathf.Sqrt(sqrMagnitude);
					vector *= this.maxSpeed / num;
				}
				this.rigidbody.velocity = vector;
				this.travelDirection = normalized;
			}
		}

		// Token: 0x06001C5C RID: 7260 RVA: 0x00079197 File Offset: 0x00077397
		private void OnPassengerExit(GameObject passenger)
		{
			this.currentPassenger = null;
			this.vehicleSeat.enabled = false;
			if (NetworkServer.active)
			{
				base.gameObject.AddComponent<DestroyOnTimer>().duration = 0.1f;
			}
		}

		// Token: 0x06001C5E RID: 7262 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x1700031F RID: 799
		// (get) Token: 0x06001C5F RID: 7263 RVA: 0x000791FC File Offset: 0x000773FC
		// (set) Token: 0x06001C60 RID: 7264 RVA: 0x0007920F File Offset: 0x0007740F
		public Vector3 NetworkendPoint
		{
			get
			{
				return this.endPoint;
			}
			[param: In]
			set
			{
				base.SetSyncVar<Vector3>(value, ref this.endPoint, 1U);
			}
		}

		// Token: 0x06001C61 RID: 7265 RVA: 0x00079224 File Offset: 0x00077424
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.endPoint);
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
				writer.Write(this.endPoint);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001C62 RID: 7266 RVA: 0x00079290 File Offset: 0x00077490
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.endPoint = reader.ReadVector3();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.endPoint = reader.ReadVector3();
			}
		}

		// Token: 0x0400193D RID: 6461
		public float maxSpeed = 30f;

		// Token: 0x0400193E RID: 6462
		public float acceleration = 2f;

		// Token: 0x04001940 RID: 6464
		private Rigidbody rigidbody;

		// Token: 0x04001941 RID: 6465
		private Vector3 startPoint;

		// Token: 0x04001942 RID: 6466
		[SyncVar]
		public Vector3 endPoint;

		// Token: 0x04001943 RID: 6467
		private Vector3 travelDirection;

		// Token: 0x04001944 RID: 6468
		private GameObject currentPassenger;

		// Token: 0x04001945 RID: 6469
		private Run.FixedTimeStamp startTravelFixedTime = Run.FixedTimeStamp.positiveInfinity;

		// Token: 0x04001946 RID: 6470
		private Run.TimeStamp startTravelTime = Run.TimeStamp.positiveInfinity;
	}
}
