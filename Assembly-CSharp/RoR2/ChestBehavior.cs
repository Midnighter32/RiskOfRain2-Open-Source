using System;
using System.Collections.Generic;
using EntityStates;
using EntityStates.Barrel;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200019F RID: 415
	public class ChestBehavior : NetworkBehaviour
	{
		// Token: 0x060008E5 RID: 2277 RVA: 0x00019B5A File Offset: 0x00017D5A
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x060008E6 RID: 2278 RVA: 0x000269BC File Offset: 0x00024BBC
		[Server]
		private void PickFromList(List<PickupIndex> dropList)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ChestBehavior::PickFromList(System.Collections.Generic.List`1<RoR2.PickupIndex>)' called on client");
				return;
			}
			this.dropPickup = PickupIndex.none;
			if (dropList != null && dropList.Count > 0)
			{
				this.dropPickup = Run.instance.treasureRng.NextElementUniform<PickupIndex>(dropList);
			}
		}

		// Token: 0x060008E7 RID: 2279 RVA: 0x00026A0C File Offset: 0x00024C0C
		[Server]
		public void RollItem()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ChestBehavior::RollItem()' called on client");
				return;
			}
			ChestBehavior.<>c__DisplayClass14_0 CS$<>8__locals1 = new ChestBehavior.<>c__DisplayClass14_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.selector = new WeightedSelection<List<PickupIndex>>(8);
			List<PickupIndex> list = new List<PickupIndex>();
			list.Add(PickupCatalog.FindPickupIndex("LunarCoin.Coin0"));
			CS$<>8__locals1.<RollItem>g__Add|1(Run.instance.availableTier1DropList, this.tier1Chance);
			CS$<>8__locals1.<RollItem>g__Add|1(Run.instance.availableTier2DropList, this.tier2Chance);
			CS$<>8__locals1.<RollItem>g__Add|1(Run.instance.availableTier3DropList, this.tier3Chance);
			CS$<>8__locals1.<RollItem>g__Add|1(Run.instance.availableLunarDropList, this.lunarChance);
			CS$<>8__locals1.<RollItem>g__Add|1(list, this.lunarCoinChance);
			List<PickupIndex> dropList = CS$<>8__locals1.selector.Evaluate(Run.instance.treasureRng.nextNormalizedFloat);
			this.PickFromList(dropList);
		}

		// Token: 0x060008E8 RID: 2280 RVA: 0x00026AE2 File Offset: 0x00024CE2
		[Server]
		public void RollEquipment()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ChestBehavior::RollEquipment()' called on client");
				return;
			}
			this.PickFromList(Run.instance.availableEquipmentDropList);
		}

		// Token: 0x060008E9 RID: 2281 RVA: 0x00026B09 File Offset: 0x00024D09
		private void Awake()
		{
			if (this.dropTransform == null)
			{
				this.dropTransform = base.transform;
			}
		}

		// Token: 0x060008EA RID: 2282 RVA: 0x00026B25 File Offset: 0x00024D25
		private void Start()
		{
			if (NetworkServer.active)
			{
				if (this.dropRoller != null)
				{
					this.dropRoller.Invoke();
					return;
				}
				Debug.LogFormat("Chest {0} has no item roller assigned!", Array.Empty<object>());
			}
		}

		// Token: 0x060008EB RID: 2283 RVA: 0x00026B54 File Offset: 0x00024D54
		[Server]
		public void Open()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ChestBehavior::Open()' called on client");
				return;
			}
			EntityStateMachine component = base.GetComponent<EntityStateMachine>();
			if (component)
			{
				component.SetNextState(EntityState.Instantiate(this.openState));
			}
		}

		// Token: 0x060008EC RID: 2284 RVA: 0x00026B98 File Offset: 0x00024D98
		[Server]
		public void ItemDrop()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ChestBehavior::ItemDrop()' called on client");
				return;
			}
			if (this.dropPickup == PickupIndex.none)
			{
				return;
			}
			PickupDropletController.CreatePickupDroplet(this.dropPickup, this.dropTransform.position + Vector3.up * 1.5f, Vector3.up * this.dropUpVelocityStrength + this.dropTransform.forward * this.dropForwardVelocityStrength);
			this.dropPickup = PickupIndex.none;
		}

		// Token: 0x060008EE RID: 2286 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x060008EF RID: 2287 RVA: 0x00026C9C File Offset: 0x00024E9C
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x060008F0 RID: 2288 RVA: 0x0000409B File Offset: 0x0000229B
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04000939 RID: 2361
		private PickupIndex dropPickup = PickupIndex.none;

		// Token: 0x0400093A RID: 2362
		public float tier1Chance = 0.8f;

		// Token: 0x0400093B RID: 2363
		public float tier2Chance = 0.2f;

		// Token: 0x0400093C RID: 2364
		public float tier3Chance = 0.01f;

		// Token: 0x0400093D RID: 2365
		public float lunarChance;

		// Token: 0x0400093E RID: 2366
		public float lunarCoinChance;

		// Token: 0x0400093F RID: 2367
		public ItemTag requiredItemTag;

		// Token: 0x04000940 RID: 2368
		public Transform dropTransform;

		// Token: 0x04000941 RID: 2369
		public float dropUpVelocityStrength = 20f;

		// Token: 0x04000942 RID: 2370
		public float dropForwardVelocityStrength = 2f;

		// Token: 0x04000943 RID: 2371
		public UnityEvent dropRoller;

		// Token: 0x04000944 RID: 2372
		public SerializableEntityStateType openState = new SerializableEntityStateType(typeof(Opening));
	}
}
