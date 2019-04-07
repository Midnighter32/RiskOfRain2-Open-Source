using System;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000385 RID: 901
	public class PickupDropletController : NetworkBehaviour
	{
		// Token: 0x060012C7 RID: 4807 RVA: 0x0005C290 File Offset: 0x0005A490
		public static void CreatePickupDroplet(PickupIndex pickupIndex, Vector3 position, Vector3 velocity)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/PickupDroplet"), position, Quaternion.identity);
			gameObject.GetComponent<PickupDropletController>().NetworkpickupIndex = pickupIndex;
			Rigidbody component = gameObject.GetComponent<Rigidbody>();
			component.velocity = velocity;
			component.AddTorque(UnityEngine.Random.Range(150f, 120f) * UnityEngine.Random.onUnitSphere);
			NetworkServer.Spawn(gameObject);
		}

		// Token: 0x060012C8 RID: 4808 RVA: 0x0005C2F0 File Offset: 0x0005A4F0
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

		// Token: 0x060012C9 RID: 4809 RVA: 0x0005C354 File Offset: 0x0005A554
		private void Start()
		{
			GameObject pickupDropletDisplayPrefab = this.pickupIndex.GetPickupDropletDisplayPrefab();
			if (pickupDropletDisplayPrefab)
			{
				UnityEngine.Object.Instantiate<GameObject>(pickupDropletDisplayPrefab, base.transform);
			}
		}

		// Token: 0x060012CB RID: 4811 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x060012CC RID: 4812 RVA: 0x0005C39C File Offset: 0x0005A59C
		// (set) Token: 0x060012CD RID: 4813 RVA: 0x0005C3AF File Offset: 0x0005A5AF
		public PickupIndex NetworkpickupIndex
		{
			get
			{
				return this.pickupIndex;
			}
			set
			{
				base.SetSyncVar<PickupIndex>(value, ref this.pickupIndex, 1u);
			}
		}

		// Token: 0x060012CE RID: 4814 RVA: 0x0005C3C4 File Offset: 0x0005A5C4
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				GeneratedNetworkCode._WritePickupIndex_None(writer, this.pickupIndex);
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
				GeneratedNetworkCode._WritePickupIndex_None(writer, this.pickupIndex);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x060012CF RID: 4815 RVA: 0x0005C430 File Offset: 0x0005A630
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

		// Token: 0x0400169E RID: 5790
		[SyncVar]
		[NonSerialized]
		public PickupIndex pickupIndex = PickupIndex.none;

		// Token: 0x0400169F RID: 5791
		private bool alive = true;
	}
}
