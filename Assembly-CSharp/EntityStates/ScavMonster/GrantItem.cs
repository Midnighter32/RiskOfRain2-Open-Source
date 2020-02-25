using System;
using RoR2;
using UnityEngine.Networking;

namespace EntityStates.ScavMonster
{
	// Token: 0x02000792 RID: 1938
	public class GrantItem : BaseState
	{
		// Token: 0x06002C75 RID: 11381 RVA: 0x000BB9CB File Offset: 0x000B9BCB
		public override void OnEnter()
		{
			base.OnEnter();
			if (NetworkServer.active)
			{
				this.GrantPickupServer(this.dropPickup, this.itemsToGrant);
			}
			if (base.isAuthority)
			{
				this.outer.SetNextState(new ExitSit());
			}
		}

		// Token: 0x06002C76 RID: 11382 RVA: 0x000BBA04 File Offset: 0x000B9C04
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write(this.dropPickup);
			writer.WritePackedUInt32((uint)this.itemsToGrant);
		}

		// Token: 0x06002C77 RID: 11383 RVA: 0x000BBA25 File Offset: 0x000B9C25
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			this.dropPickup = reader.ReadPickupIndex();
			this.itemsToGrant = (int)reader.ReadPackedUInt32();
		}

		// Token: 0x06002C78 RID: 11384 RVA: 0x000BBA48 File Offset: 0x000B9C48
		private void GrantPickupServer(PickupIndex pickupIndex, int countToGrant)
		{
			PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
			if (pickupDef == null)
			{
				return;
			}
			ItemIndex itemIndex = pickupDef.itemIndex;
			if (ItemCatalog.GetItemDef(itemIndex) == null)
			{
				return;
			}
			base.characterBody.inventory.GiveItem(itemIndex, countToGrant);
			Chat.SendBroadcastChat(new Chat.PlayerPickupChatMessage
			{
				subjectAsCharacterBody = base.characterBody,
				baseToken = "MONSTER_PICKUP",
				pickupToken = pickupDef.nameToken,
				pickupColor = pickupDef.baseColor,
				pickupQuantity = (uint)this.itemsToGrant
			});
		}

		// Token: 0x04002889 RID: 10377
		public PickupIndex dropPickup;

		// Token: 0x0400288A RID: 10378
		public int itemsToGrant;
	}
}
