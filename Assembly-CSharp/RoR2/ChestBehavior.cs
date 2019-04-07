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
	// Token: 0x02000297 RID: 663
	public class ChestBehavior : NetworkBehaviour
	{
		// Token: 0x06000D7E RID: 3454 RVA: 0x00037FB6 File Offset: 0x000361B6
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x06000D7F RID: 3455 RVA: 0x000428C0 File Offset: 0x00040AC0
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
				this.dropPickup = dropList[Run.instance.treasureRng.RangeInt(0, dropList.Count)];
			}
		}

		// Token: 0x06000D80 RID: 3456 RVA: 0x0004291C File Offset: 0x00040B1C
		[Server]
		public void RollItem()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ChestBehavior::RollItem()' called on client");
				return;
			}
			WeightedSelection<List<PickupIndex>> weightedSelection = new WeightedSelection<List<PickupIndex>>(8);
			weightedSelection.AddChoice(Run.instance.availableTier1DropList, this.tier1Chance);
			weightedSelection.AddChoice(Run.instance.availableTier2DropList, this.tier2Chance);
			weightedSelection.AddChoice(Run.instance.availableTier3DropList, this.tier3Chance);
			weightedSelection.AddChoice(Run.instance.availableLunarDropList, this.lunarChance);
			List<PickupIndex> dropList = weightedSelection.Evaluate(Run.instance.treasureRng.nextNormalizedFloat);
			this.PickFromList(dropList);
		}

		// Token: 0x06000D81 RID: 3457 RVA: 0x000429B8 File Offset: 0x00040BB8
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

		// Token: 0x06000D82 RID: 3458 RVA: 0x000429DF File Offset: 0x00040BDF
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

		// Token: 0x06000D83 RID: 3459 RVA: 0x00042A0C File Offset: 0x00040C0C
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

		// Token: 0x06000D84 RID: 3460 RVA: 0x00042A50 File Offset: 0x00040C50
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
			PickupDropletController.CreatePickupDroplet(this.dropPickup, base.transform.position + Vector3.up * 1.5f, Vector3.up * 20f + base.transform.forward * 2f);
			this.dropPickup = PickupIndex.none;
		}

		// Token: 0x06000D86 RID: 3462 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x06000D87 RID: 3463 RVA: 0x00042B38 File Offset: 0x00040D38
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06000D88 RID: 3464 RVA: 0x00004507 File Offset: 0x00002707
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04001178 RID: 4472
		private PickupIndex dropPickup = PickupIndex.none;

		// Token: 0x04001179 RID: 4473
		public float tier1Chance = 0.8f;

		// Token: 0x0400117A RID: 4474
		public float tier2Chance = 0.2f;

		// Token: 0x0400117B RID: 4475
		public float tier3Chance = 0.01f;

		// Token: 0x0400117C RID: 4476
		public float lunarChance;

		// Token: 0x0400117D RID: 4477
		public UnityEvent dropRoller;

		// Token: 0x0400117E RID: 4478
		public SerializableEntityStateType openState = new SerializableEntityStateType(typeof(Opening));
	}
}
