using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000611 RID: 1553
	public class NotificationQueue : MonoBehaviour
	{
		// Token: 0x17000316 RID: 790
		// (get) Token: 0x06002307 RID: 8967 RVA: 0x000A50A5 File Offset: 0x000A32A5
		public static ReadOnlyCollection<NotificationQueue> readOnlyInstancesList
		{
			get
			{
				return NotificationQueue._readOnlyInstancesList;
			}
		}

		// Token: 0x06002308 RID: 8968 RVA: 0x000A50AC File Offset: 0x000A32AC
		private void OnEnable()
		{
			NotificationQueue.instancesList.Add(this);
		}

		// Token: 0x06002309 RID: 8969 RVA: 0x000A50B9 File Offset: 0x000A32B9
		private void OnDisable()
		{
			NotificationQueue.instancesList.Remove(this);
		}

		// Token: 0x0600230A RID: 8970 RVA: 0x000A50C8 File Offset: 0x000A32C8
		private void OnItemPickup(CharacterMaster characterMaster, ItemIndex itemIndex)
		{
			ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
			if (itemDef == null || itemDef.hidden)
			{
				return;
			}
			if (this.hud.targetMaster == characterMaster)
			{
				this.notificationQueue.Enqueue(new NotificationQueue.NotificationInfo
				{
					notificationType = NotificationQueue.NotificationType.ItemPickup,
					itemIndex = itemIndex
				});
			}
		}

		// Token: 0x0600230B RID: 8971 RVA: 0x000A5119 File Offset: 0x000A3319
		private void OnEquipmentPickup(CharacterMaster characterMaster, EquipmentIndex equipmentIndex)
		{
			if (this.hud.targetMaster == characterMaster)
			{
				this.notificationQueue.Enqueue(new NotificationQueue.NotificationInfo
				{
					notificationType = NotificationQueue.NotificationType.EquipmentPickup,
					equipmentIndex = equipmentIndex
				});
			}
		}

		// Token: 0x0600230C RID: 8972 RVA: 0x000A514C File Offset: 0x000A334C
		public void OnPickup(CharacterMaster characterMaster, PickupIndex pickupIndex)
		{
			ItemIndex itemIndex = pickupIndex.itemIndex;
			if (itemIndex >= ItemIndex.Syringe)
			{
				this.OnItemPickup(characterMaster, itemIndex);
				return;
			}
			EquipmentIndex equipmentIndex = pickupIndex.equipmentIndex;
			if (equipmentIndex >= EquipmentIndex.CommandMissile)
			{
				this.OnEquipmentPickup(characterMaster, equipmentIndex);
			}
		}

		// Token: 0x0600230D RID: 8973 RVA: 0x000A5184 File Offset: 0x000A3384
		public void Update()
		{
			if (!this.currentNotification && this.notificationQueue.Count > 0)
			{
				NotificationQueue.NotificationInfo notificationInfo = this.notificationQueue.Dequeue();
				this.currentNotification = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NotificationPanel2")).GetComponent<GenericNotification>();
				NotificationQueue.NotificationType notificationType = notificationInfo.notificationType;
				if (notificationType != NotificationQueue.NotificationType.ItemPickup)
				{
					if (notificationType == NotificationQueue.NotificationType.EquipmentPickup)
					{
						this.currentNotification.SetEquipment(notificationInfo.equipmentIndex);
					}
				}
				else
				{
					this.currentNotification.SetItem(notificationInfo.itemIndex);
				}
				this.currentNotification.GetComponent<RectTransform>().SetParent(base.GetComponent<RectTransform>(), false);
			}
		}

		// Token: 0x040025F7 RID: 9719
		private static List<NotificationQueue> instancesList = new List<NotificationQueue>();

		// Token: 0x040025F8 RID: 9720
		private static ReadOnlyCollection<NotificationQueue> _readOnlyInstancesList = new ReadOnlyCollection<NotificationQueue>(NotificationQueue.instancesList);

		// Token: 0x040025F9 RID: 9721
		public HUD hud;

		// Token: 0x040025FA RID: 9722
		private Queue<NotificationQueue.NotificationInfo> notificationQueue = new Queue<NotificationQueue.NotificationInfo>();

		// Token: 0x040025FB RID: 9723
		private GenericNotification currentNotification;

		// Token: 0x02000612 RID: 1554
		private enum NotificationType
		{
			// Token: 0x040025FD RID: 9725
			ItemPickup,
			// Token: 0x040025FE RID: 9726
			EquipmentPickup
		}

		// Token: 0x02000613 RID: 1555
		private class NotificationInfo
		{
			// Token: 0x040025FF RID: 9727
			public NotificationQueue.NotificationType notificationType;

			// Token: 0x04002600 RID: 9728
			public ItemIndex itemIndex = ItemIndex.None;

			// Token: 0x04002601 RID: 9729
			public EquipmentIndex equipmentIndex = EquipmentIndex.None;
		}
	}
}
