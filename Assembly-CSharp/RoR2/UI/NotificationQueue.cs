using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000600 RID: 1536
	public class NotificationQueue : MonoBehaviour
	{
		// Token: 0x170003CC RID: 972
		// (get) Token: 0x06002477 RID: 9335 RVA: 0x0009F215 File Offset: 0x0009D415
		public static ReadOnlyCollection<NotificationQueue> readOnlyInstancesList
		{
			get
			{
				return NotificationQueue._readOnlyInstancesList;
			}
		}

		// Token: 0x06002478 RID: 9336 RVA: 0x0009F21C File Offset: 0x0009D41C
		private void OnEnable()
		{
			NotificationQueue.instancesList.Add(this);
		}

		// Token: 0x06002479 RID: 9337 RVA: 0x0009F229 File Offset: 0x0009D429
		private void OnDisable()
		{
			NotificationQueue.instancesList.Remove(this);
		}

		// Token: 0x0600247A RID: 9338 RVA: 0x0009F238 File Offset: 0x0009D438
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

		// Token: 0x0600247B RID: 9339 RVA: 0x0009F289 File Offset: 0x0009D489
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

		// Token: 0x0600247C RID: 9340 RVA: 0x0009F2BC File Offset: 0x0009D4BC
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

		// Token: 0x0600247D RID: 9341 RVA: 0x0009F2F4 File Offset: 0x0009D4F4
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

		// Token: 0x0400223B RID: 8763
		private static List<NotificationQueue> instancesList = new List<NotificationQueue>();

		// Token: 0x0400223C RID: 8764
		private static ReadOnlyCollection<NotificationQueue> _readOnlyInstancesList = new ReadOnlyCollection<NotificationQueue>(NotificationQueue.instancesList);

		// Token: 0x0400223D RID: 8765
		public HUD hud;

		// Token: 0x0400223E RID: 8766
		private Queue<NotificationQueue.NotificationInfo> notificationQueue = new Queue<NotificationQueue.NotificationInfo>();

		// Token: 0x0400223F RID: 8767
		private GenericNotification currentNotification;

		// Token: 0x02000601 RID: 1537
		private enum NotificationType
		{
			// Token: 0x04002241 RID: 8769
			ItemPickup,
			// Token: 0x04002242 RID: 8770
			EquipmentPickup
		}

		// Token: 0x02000602 RID: 1538
		private class NotificationInfo
		{
			// Token: 0x04002243 RID: 8771
			public NotificationQueue.NotificationType notificationType;

			// Token: 0x04002244 RID: 8772
			public ItemIndex itemIndex = ItemIndex.None;

			// Token: 0x04002245 RID: 8773
			public EquipmentIndex equipmentIndex = EquipmentIndex.None;
		}
	}
}
