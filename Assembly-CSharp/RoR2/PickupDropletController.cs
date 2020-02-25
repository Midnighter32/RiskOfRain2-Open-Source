using System;
using System.Runtime.InteropServices;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002BD RID: 701
	public class PickupDropletController : NetworkBehaviour
	{
		// Token: 0x06000FD3 RID: 4051 RVA: 0x00045998 File Offset: 0x00043B98
		public static void CreatePickupDroplet(PickupIndex pickupIndex, Vector3 position, Vector3 velocity)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/PickupDroplet"), position, Quaternion.identity);
			gameObject.GetComponent<PickupDropletController>().NetworkpickupIndex = pickupIndex;
			Rigidbody component = gameObject.GetComponent<Rigidbody>();
			component.velocity = velocity;
			component.AddTorque(UnityEngine.Random.Range(150f, 120f) * UnityEngine.Random.onUnitSphere);
			NetworkServer.Spawn(gameObject);
		}

		// Token: 0x06000FD4 RID: 4052 RVA: 0x000459F8 File Offset: 0x00043BF8
		public void OnCollisionEnter(Collision collision)
		{
			if (NetworkServer.active && this.alive)
			{
				this.alive = false;
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/GenericPickup"), base.transform.position, Quaternion.identity);
				gameObject.GetComponent<GenericPickupController>().NetworkpickupIndex = this.pickupIndex;
				NetworkServer.Spawn(gameObject);
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06000FD5 RID: 4053 RVA: 0x00045A5C File Offset: 0x00043C5C
		private void Start()
		{
			GameObject pickupDropletDisplayPrefab = this.pickupIndex.GetPickupDropletDisplayPrefab();
			if (pickupDropletDisplayPrefab)
			{
				UnityEngine.Object.Instantiate<GameObject>(pickupDropletDisplayPrefab, base.transform);
			}
		}

		// Token: 0x06000FD7 RID: 4055 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x06000FD8 RID: 4056 RVA: 0x00045AA4 File Offset: 0x00043CA4
		// (set) Token: 0x06000FD9 RID: 4057 RVA: 0x00045AB7 File Offset: 0x00043CB7
		public PickupIndex NetworkpickupIndex
		{
			get
			{
				return this.pickupIndex;
			}
			[param: In]
			set
			{
				base.SetSyncVar<PickupIndex>(value, ref this.pickupIndex, 1U);
			}
		}

		// Token: 0x06000FDA RID: 4058 RVA: 0x00045ACC File Offset: 0x00043CCC
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				GeneratedNetworkCode._WritePickupIndex_None(writer, this.pickupIndex);
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
				GeneratedNetworkCode._WritePickupIndex_None(writer, this.pickupIndex);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000FDB RID: 4059 RVA: 0x00045B38 File Offset: 0x00043D38
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.pickupIndex = GeneratedNetworkCode._ReadPickupIndex_None(reader);
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.pickupIndex = GeneratedNetworkCode._ReadPickupIndex_None(reader);
			}
		}

		// Token: 0x04000F5B RID: 3931
		[SyncVar]
		[NonSerialized]
		public PickupIndex pickupIndex = PickupIndex.none;

		// Token: 0x04000F5C RID: 3932
		private bool alive = true;
	}
}
