using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200038B RID: 907
	public class ZiplineController : NetworkBehaviour
	{
		// Token: 0x06001614 RID: 5652 RVA: 0x0005F147 File Offset: 0x0005D347
		public void SetPointAPosition(Vector3 position)
		{
			this.NetworkpointAPosition = position;
			this.pointATransform.position = this.pointAPosition;
			this.pointATransform.LookAt(this.pointBTransform);
			this.pointBTransform.LookAt(this.pointATransform);
		}

		// Token: 0x06001615 RID: 5653 RVA: 0x0005F183 File Offset: 0x0005D383
		public void SetPointBPosition(Vector3 position)
		{
			this.NetworkpointBPosition = position;
			this.pointBTransform.position = this.pointBPosition;
			this.pointATransform.LookAt(this.pointBTransform);
			this.pointBTransform.LookAt(this.pointATransform);
		}

		// Token: 0x06001616 RID: 5654 RVA: 0x0005F1C0 File Offset: 0x0005D3C0
		private void RebuildZiplineVehicle(ref ZiplineVehicle ziplineVehicle, Vector3 startPos, Vector3 endPos)
		{
			if (ziplineVehicle && ziplineVehicle.vehicleSeat.hasPassenger)
			{
				ziplineVehicle = null;
			}
			if (!ziplineVehicle)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.ziplineVehiclePrefab, startPos, Quaternion.identity);
				ziplineVehicle = gameObject.GetComponent<ZiplineVehicle>();
				ziplineVehicle.NetworkendPoint = endPos;
				NetworkServer.Spawn(gameObject);
			}
		}

		// Token: 0x06001617 RID: 5655 RVA: 0x0005F218 File Offset: 0x0005D418
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.RebuildZiplineVehicle(ref this.currentZiplineA, this.pointAPosition, this.pointBPosition);
				this.RebuildZiplineVehicle(ref this.currentZiplineB, this.pointBPosition, this.pointAPosition);
			}
		}

		// Token: 0x06001618 RID: 5656 RVA: 0x0005F254 File Offset: 0x0005D454
		private void OnDestroy()
		{
			if (NetworkServer.active)
			{
				if (this.currentZiplineA)
				{
					UnityEngine.Object.Destroy(this.currentZiplineA.gameObject);
				}
				if (this.currentZiplineB)
				{
					UnityEngine.Object.Destroy(this.currentZiplineB.gameObject);
				}
			}
		}

		// Token: 0x0600161A RID: 5658 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x17000294 RID: 660
		// (get) Token: 0x0600161B RID: 5659 RVA: 0x0005F2A4 File Offset: 0x0005D4A4
		// (set) Token: 0x0600161C RID: 5660 RVA: 0x0005F2B7 File Offset: 0x0005D4B7
		public Vector3 NetworkpointAPosition
		{
			get
			{
				return this.pointAPosition;
			}
			[param: In]
			set
			{
				uint dirtyBit = 1U;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.SetPointAPosition(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<Vector3>(value, ref this.pointAPosition, dirtyBit);
			}
		}

		// Token: 0x17000295 RID: 661
		// (get) Token: 0x0600161D RID: 5661 RVA: 0x0005F2F8 File Offset: 0x0005D4F8
		// (set) Token: 0x0600161E RID: 5662 RVA: 0x0005F30B File Offset: 0x0005D50B
		public Vector3 NetworkpointBPosition
		{
			get
			{
				return this.pointBPosition;
			}
			[param: In]
			set
			{
				uint dirtyBit = 2U;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.SetPointBPosition(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<Vector3>(value, ref this.pointBPosition, dirtyBit);
			}
		}

		// Token: 0x0600161F RID: 5663 RVA: 0x0005F34C File Offset: 0x0005D54C
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.pointAPosition);
				writer.Write(this.pointBPosition);
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
				writer.Write(this.pointAPosition);
			}
			if ((base.syncVarDirtyBits & 2U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.pointBPosition);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001620 RID: 5664 RVA: 0x0005F3F8 File Offset: 0x0005D5F8
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.pointAPosition = reader.ReadVector3();
				this.pointBPosition = reader.ReadVector3();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.SetPointAPosition(reader.ReadVector3());
			}
			if ((num & 2) != 0)
			{
				this.SetPointBPosition(reader.ReadVector3());
			}
		}

		// Token: 0x040014CF RID: 5327
		[SyncVar(hook = "SetPointAPosition")]
		private Vector3 pointAPosition;

		// Token: 0x040014D0 RID: 5328
		[SyncVar(hook = "SetPointBPosition")]
		private Vector3 pointBPosition;

		// Token: 0x040014D1 RID: 5329
		[SerializeField]
		private Transform pointATransform;

		// Token: 0x040014D2 RID: 5330
		[SerializeField]
		private Transform pointBTransform;

		// Token: 0x040014D3 RID: 5331
		public GameObject ziplineVehiclePrefab;

		// Token: 0x040014D4 RID: 5332
		private ZiplineVehicle currentZiplineA;

		// Token: 0x040014D5 RID: 5333
		private ZiplineVehicle currentZiplineB;
	}
}
