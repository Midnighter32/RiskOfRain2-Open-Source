using System;
using System.Collections.Generic;
using System.Linq;
using EntityStates;
using EntityStates.Barrel;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000310 RID: 784
	public class ScavBackpackBehavior : NetworkBehaviour
	{
		// Token: 0x06001262 RID: 4706 RVA: 0x00019B5A File Offset: 0x00017D5A
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x06001263 RID: 4707 RVA: 0x0004F3DC File Offset: 0x0004D5DC
		[Server]
		private void PickFromList(List<PickupIndex> dropList)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ScavBackpackBehavior::PickFromList(System.Collections.Generic.List`1<RoR2.PickupIndex>)' called on client");
				return;
			}
			this.dropPickup = PickupIndex.none;
			if (dropList != null && dropList.Count > 0)
			{
				this.dropPickup = Run.instance.treasureRng.NextElementUniform<PickupIndex>(dropList);
			}
		}

		// Token: 0x06001264 RID: 4708 RVA: 0x0004F42C File Offset: 0x0004D62C
		[Server]
		public void RollItem()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ScavBackpackBehavior::RollItem()' called on client");
				return;
			}
			WeightedSelection<List<PickupIndex>> weightedSelection = new WeightedSelection<List<PickupIndex>>(8);
			weightedSelection.AddChoice((from v in Run.instance.availableTier1DropList
			where ItemCatalog.GetItemDef(v.itemIndex) != null && ItemCatalog.GetItemDef(v.itemIndex).ContainsTag(this.requiredItemTag)
			select v).ToList<PickupIndex>(), this.tier1Chance);
			weightedSelection.AddChoice((from v in Run.instance.availableTier2DropList
			where ItemCatalog.GetItemDef(v.itemIndex) != null && ItemCatalog.GetItemDef(v.itemIndex).ContainsTag(this.requiredItemTag)
			select v).ToList<PickupIndex>(), this.tier2Chance);
			weightedSelection.AddChoice((from v in Run.instance.availableTier3DropList
			where ItemCatalog.GetItemDef(v.itemIndex) != null && ItemCatalog.GetItemDef(v.itemIndex).ContainsTag(this.requiredItemTag)
			select v).ToList<PickupIndex>(), this.tier3Chance);
			weightedSelection.AddChoice((from v in Run.instance.availableLunarDropList
			where ItemCatalog.GetItemDef(v.itemIndex) != null && ItemCatalog.GetItemDef(v.itemIndex).ContainsTag(this.requiredItemTag)
			select v).ToList<PickupIndex>(), this.lunarChance);
			List<PickupIndex> dropList = weightedSelection.Evaluate(Run.instance.treasureRng.nextNormalizedFloat);
			this.PickFromList(dropList);
		}

		// Token: 0x06001265 RID: 4709 RVA: 0x0004F520 File Offset: 0x0004D720
		[Server]
		public void RollEquipment()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ScavBackpackBehavior::RollEquipment()' called on client");
				return;
			}
			this.PickFromList(Run.instance.availableEquipmentDropList);
		}

		// Token: 0x06001266 RID: 4710 RVA: 0x0004F547 File Offset: 0x0004D747
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

		// Token: 0x06001267 RID: 4711 RVA: 0x0004F574 File Offset: 0x0004D774
		[Server]
		public void Open()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ScavBackpackBehavior::Open()' called on client");
				return;
			}
			EntityStateMachine component = base.GetComponent<EntityStateMachine>();
			if (component)
			{
				component.SetNextState(EntityState.Instantiate(this.openState));
			}
		}

		// Token: 0x06001268 RID: 4712 RVA: 0x0004F5B8 File Offset: 0x0004D7B8
		[Server]
		public void ItemDrop()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ScavBackpackBehavior::ItemDrop()' called on client");
				return;
			}
			if (this.dropPickup == PickupIndex.none)
			{
				return;
			}
			PickupDropletController.CreatePickupDroplet(this.dropPickup, base.transform.position + Vector3.up * 1.5f, Vector3.up * 20f + base.transform.forward * 2f);
			this.dropPickup = PickupIndex.none;
		}

		// Token: 0x0600126E RID: 4718 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x0600126F RID: 4719 RVA: 0x0004F6CC File Offset: 0x0004D8CC
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06001270 RID: 4720 RVA: 0x0000409B File Offset: 0x0000229B
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04001159 RID: 4441
		private PickupIndex dropPickup = PickupIndex.none;

		// Token: 0x0400115A RID: 4442
		public float tier1Chance = 0.8f;

		// Token: 0x0400115B RID: 4443
		public float tier2Chance = 0.2f;

		// Token: 0x0400115C RID: 4444
		public float tier3Chance = 0.01f;

		// Token: 0x0400115D RID: 4445
		public float lunarChance;

		// Token: 0x0400115E RID: 4446
		public int totalItems;

		// Token: 0x0400115F RID: 4447
		public float delayBetweenItems;

		// Token: 0x04001160 RID: 4448
		public ItemTag requiredItemTag;

		// Token: 0x04001161 RID: 4449
		public UnityEvent dropRoller;

		// Token: 0x04001162 RID: 4450
		public SerializableEntityStateType openState = new SerializableEntityStateType(typeof(Opening));
	}
}
