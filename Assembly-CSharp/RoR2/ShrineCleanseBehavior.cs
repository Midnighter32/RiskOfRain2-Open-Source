using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000330 RID: 816
	public class ShrineCleanseBehavior : MonoBehaviour, IInteractable
	{
		// Token: 0x06001355 RID: 4949 RVA: 0x00052CBA File Offset: 0x00050EBA
		public string GetContextString(Interactor activator)
		{
			return Language.GetString(this.contextToken);
		}

		// Token: 0x06001356 RID: 4950 RVA: 0x00052CC8 File Offset: 0x00050EC8
		private static bool InventoryIsCleansable(Inventory inventory)
		{
			for (int i = 0; i < ShrineCleanseBehavior.cleansableItems.Length; i++)
			{
				if (inventory.GetItemCount(ShrineCleanseBehavior.cleansableItems[i]) > 0)
				{
					return true;
				}
			}
			int j = 0;
			int equipmentSlotCount = inventory.GetEquipmentSlotCount();
			while (j < equipmentSlotCount)
			{
				EquipmentState equipment = inventory.GetEquipment((uint)j);
				for (int k = 0; k < ShrineCleanseBehavior.cleansableEquipments.Length; k++)
				{
					if (equipment.equipmentIndex == ShrineCleanseBehavior.cleansableEquipments[k])
					{
						return true;
					}
				}
				j++;
			}
			return false;
		}

		// Token: 0x06001357 RID: 4951 RVA: 0x00052D40 File Offset: 0x00050F40
		private static int CleanseInventoryServer(Inventory inventory)
		{
			int num = 0;
			for (int i = 0; i < ShrineCleanseBehavior.cleansableItems.Length; i++)
			{
				ItemIndex itemIndex = ShrineCleanseBehavior.cleansableItems[i];
				int itemCount = inventory.GetItemCount(itemIndex);
				if (itemCount != 0)
				{
					inventory.RemoveItem(itemIndex, itemCount);
					num += itemCount;
				}
			}
			int num2 = 0;
			int j = 0;
			int equipmentSlotCount = inventory.GetEquipmentSlotCount();
			while (j < equipmentSlotCount)
			{
				EquipmentState equipment = inventory.GetEquipment((uint)j);
				for (int k = 0; k < ShrineCleanseBehavior.cleansableEquipments.Length; k++)
				{
					if (equipment.equipmentIndex == ShrineCleanseBehavior.cleansableEquipments[k])
					{
						inventory.SetEquipment(EquipmentState.empty, (uint)j);
						num2++;
					}
				}
				j++;
			}
			return num + num2;
		}

		// Token: 0x06001358 RID: 4952 RVA: 0x00052DE4 File Offset: 0x00050FE4
		public Interactability GetInteractability(Interactor activator)
		{
			CharacterBody component = activator.GetComponent<CharacterBody>();
			if (component)
			{
				Inventory inventory = component.inventory;
				if (inventory && ShrineCleanseBehavior.InventoryIsCleansable(inventory))
				{
					return Interactability.Available;
				}
			}
			return Interactability.ConditionsNotMet;
		}

		// Token: 0x06001359 RID: 4953 RVA: 0x00052E1C File Offset: 0x0005101C
		public void OnInteractionBegin(Interactor activator)
		{
			CharacterBody component = activator.GetComponent<CharacterBody>();
			if (component)
			{
				Inventory inventory = component.inventory;
				if (inventory)
				{
					ShrineCleanseBehavior.CleanseInventoryServer(inventory);
					EffectManager.SimpleEffect(this.activationEffectPrefab, base.transform.position, base.transform.rotation, true);
				}
			}
		}

		// Token: 0x0600135A RID: 4954 RVA: 0x0000AC89 File Offset: 0x00008E89
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x0600135B RID: 4955 RVA: 0x0000B933 File Offset: 0x00009B33
		public bool ShouldShowOnScanner()
		{
			return true;
		}

		// Token: 0x0600135C RID: 4956 RVA: 0x00052E70 File Offset: 0x00051070
		[SystemInitializer(new Type[]
		{
			typeof(ItemCatalog),
			typeof(EquipmentCatalog)
		})]
		private static void Init()
		{
			List<ItemIndex> list = new List<ItemIndex>();
			List<EquipmentIndex> list2 = new List<EquipmentIndex>();
			ItemIndex itemIndex = ItemIndex.Syringe;
			ItemIndex itemCount = (ItemIndex)ItemCatalog.itemCount;
			while (itemIndex < itemCount)
			{
				ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
				if (itemDef.tier == ItemTier.Lunar || itemDef.ContainsTag(ItemTag.Cleansable))
				{
					list.Add(itemIndex);
				}
				itemIndex++;
			}
			EquipmentIndex equipmentIndex = EquipmentIndex.CommandMissile;
			EquipmentIndex equipmentCount = (EquipmentIndex)EquipmentCatalog.equipmentCount;
			while (equipmentIndex < equipmentCount)
			{
				if (EquipmentCatalog.GetEquipmentDef(equipmentIndex).isLunar)
				{
					list2.Add(equipmentIndex);
				}
				equipmentIndex++;
			}
			ShrineCleanseBehavior.cleansableItems = list.ToArray();
			ShrineCleanseBehavior.cleansableEquipments = list2.ToArray();
		}

		// Token: 0x0400121E RID: 4638
		public string contextToken;

		// Token: 0x0400121F RID: 4639
		public GameObject activationEffectPrefab;

		// Token: 0x04001220 RID: 4640
		private static ItemIndex[] cleansableItems = Array.Empty<ItemIndex>();

		// Token: 0x04001221 RID: 4641
		private static EquipmentIndex[] cleansableEquipments = Array.Empty<EquipmentIndex>();
	}
}
