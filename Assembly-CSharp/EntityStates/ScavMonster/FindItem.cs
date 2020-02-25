using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.ScavMonster
{
	// Token: 0x02000790 RID: 1936
	public class FindItem : BaseState
	{
		// Token: 0x06002C6A RID: 11370 RVA: 0x000BB74C File Offset: 0x000B994C
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FindItem.baseDuration / this.attackSpeedStat;
			base.PlayCrossfade("Body", "SitRummage", "Sit.playbackRate", this.duration, 0.1f);
			Util.PlaySound(FindItem.sound, base.gameObject);
			if (base.isAuthority)
			{
				WeightedSelection<List<PickupIndex>> weightedSelection = new WeightedSelection<List<PickupIndex>>(8);
				weightedSelection.AddChoice((from v in Run.instance.availableTier1DropList
				where ItemCatalog.GetItemDef(v.itemIndex) != null && ItemCatalog.GetItemDef(v.itemIndex).DoesNotContainTag(ItemTag.AIBlacklist)
				select v).ToList<PickupIndex>(), FindItem.tier1Chance);
				weightedSelection.AddChoice((from v in Run.instance.availableTier2DropList
				where ItemCatalog.GetItemDef(v.itemIndex) != null && ItemCatalog.GetItemDef(v.itemIndex).DoesNotContainTag(ItemTag.AIBlacklist)
				select v).ToList<PickupIndex>(), FindItem.tier2Chance);
				weightedSelection.AddChoice((from v in Run.instance.availableTier3DropList
				where ItemCatalog.GetItemDef(v.itemIndex) != null && ItemCatalog.GetItemDef(v.itemIndex).DoesNotContainTag(ItemTag.AIBlacklist)
				select v).ToList<PickupIndex>(), FindItem.tier3Chance);
				List<PickupIndex> list = weightedSelection.Evaluate(UnityEngine.Random.value);
				this.dropPickup = list[UnityEngine.Random.Range(0, list.Count)];
				PickupDef pickupDef = PickupCatalog.GetPickupDef(this.dropPickup);
				if (pickupDef != null)
				{
					ItemDef itemDef = ItemCatalog.GetItemDef(pickupDef.itemIndex);
					if (itemDef != null)
					{
						this.itemsToGrant = 0;
						switch (itemDef.tier)
						{
						case ItemTier.Tier1:
							this.itemsToGrant = FindItem.tier1Count;
							break;
						case ItemTier.Tier2:
							this.itemsToGrant = FindItem.tier2Count;
							break;
						case ItemTier.Tier3:
							this.itemsToGrant = FindItem.tier3Count;
							break;
						default:
							this.itemsToGrant = 1;
							break;
						}
					}
				}
			}
			Transform transform = base.FindModelChild("PickupDisplay");
			this.pickupDisplay = transform.GetComponent<PickupDisplay>();
			this.pickupDisplay.SetPickupIndex(this.dropPickup, false);
		}

		// Token: 0x06002C6B RID: 11371 RVA: 0x000BB92A File Offset: 0x000B9B2A
		public override void OnExit()
		{
			this.pickupDisplay.SetPickupIndex(PickupIndex.none, false);
			base.OnExit();
		}

		// Token: 0x06002C6C RID: 11372 RVA: 0x000BB944 File Offset: 0x000B9B44
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextState(new GrantItem
				{
					dropPickup = this.dropPickup,
					itemsToGrant = this.itemsToGrant
				});
			}
		}

		// Token: 0x06002C6D RID: 11373 RVA: 0x000BB995 File Offset: 0x000B9B95
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write(this.dropPickup);
		}

		// Token: 0x06002C6E RID: 11374 RVA: 0x000BB9AA File Offset: 0x000B9BAA
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			this.dropPickup = reader.ReadPickupIndex();
		}

		// Token: 0x04002879 RID: 10361
		public static float baseDuration;

		// Token: 0x0400287A RID: 10362
		public static float tier1Chance;

		// Token: 0x0400287B RID: 10363
		public static int tier1Count;

		// Token: 0x0400287C RID: 10364
		public static float tier2Chance;

		// Token: 0x0400287D RID: 10365
		public static int tier2Count;

		// Token: 0x0400287E RID: 10366
		public static float tier3Chance;

		// Token: 0x0400287F RID: 10367
		public static int tier3Count;

		// Token: 0x04002880 RID: 10368
		public static string sound;

		// Token: 0x04002881 RID: 10369
		private float duration;

		// Token: 0x04002882 RID: 10370
		private PickupIndex dropPickup;

		// Token: 0x04002883 RID: 10371
		private int itemsToGrant;

		// Token: 0x04002884 RID: 10372
		private PickupDisplay pickupDisplay;
	}
}
