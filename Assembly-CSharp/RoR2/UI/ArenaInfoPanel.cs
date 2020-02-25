using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000589 RID: 1417
	[RequireComponent(typeof(HudElement))]
	public class ArenaInfoPanel : MonoBehaviour
	{
		// Token: 0x060021B6 RID: 8630 RVA: 0x00091DD4 File Offset: 0x0008FFD4
		private void Awake()
		{
			this.monsterBodyIconAllocator = new UIElementAllocator<RawImage>(this.monsterBodyIconContainer, this.iconPrefab);
			this.pickupIconAllocator = new UIElementAllocator<RawImage>(this.pickupIconContainer, this.iconPrefab);
		}

		// Token: 0x060021B7 RID: 8631 RVA: 0x00091E04 File Offset: 0x00090004
		private void OnEnable()
		{
			InstanceTracker.Add<ArenaInfoPanel>(this);
		}

		// Token: 0x060021B8 RID: 8632 RVA: 0x00091E0C File Offset: 0x0009000C
		private void OnDisable()
		{
			InstanceTracker.Remove<ArenaInfoPanel>(this);
		}

		// Token: 0x060021B9 RID: 8633 RVA: 0x00091E14 File Offset: 0x00090014
		private void FixedUpdate()
		{
			ArenaMissionController instance = ArenaMissionController.instance;
			if (instance)
			{
				bool flag = false;
				if (this.currentMonsterBodyIndices.Length != instance.syncActiveMonsterBodies.Count)
				{
					Array.Resize<int>(ref this.currentMonsterBodyIndices, instance.syncActiveMonsterBodies.Count);
					flag = true;
				}
				for (int i = 0; i < instance.syncActiveMonsterBodies.Count; i++)
				{
					if (this.currentMonsterBodyIndices[i] != instance.syncActiveMonsterBodies[i])
					{
						this.currentMonsterBodyIndices[i] = instance.syncActiveMonsterBodies[i];
						flag = true;
					}
				}
				if (flag)
				{
					this.SetMonsterBodies(this.currentMonsterBodyIndices);
				}
				bool flag2 = false;
				if (this.currentPickupIndices.Length != instance.syncActivePickups.Count)
				{
					Array.Resize<PickupIndex>(ref this.currentPickupIndices, instance.syncActivePickups.Count);
					flag2 = true;
				}
				for (int j = 0; j < instance.syncActivePickups.Count; j++)
				{
					PickupIndex pickupIndex = new PickupIndex(instance.syncActivePickups[j]);
					if (pickupIndex != this.currentPickupIndices[j])
					{
						this.currentPickupIndices[j] = pickupIndex;
						flag2 = true;
					}
				}
				if (flag2)
				{
					this.SetPickups(this.currentPickupIndices);
				}
			}
		}

		// Token: 0x060021BA RID: 8634 RVA: 0x00091F48 File Offset: 0x00090148
		private void SetMonsterBodies(int[] bodyIndices)
		{
			this.monsterBodyIconAllocator.AllocateElements(bodyIndices.Length);
			for (int i = 0; i < bodyIndices.Length; i++)
			{
				CharacterBody bodyPrefabBodyComponent = BodyCatalog.GetBodyPrefabBodyComponent(bodyIndices[i]);
				this.monsterBodyIconAllocator.elements[i].texture = ((bodyPrefabBodyComponent != null) ? bodyPrefabBodyComponent.portraitIcon : null);
			}
		}

		// Token: 0x060021BB RID: 8635 RVA: 0x00091F9C File Offset: 0x0009019C
		private void SetPickups(PickupIndex[] pickupIndices)
		{
			this.pickupIconAllocator.AllocateElements(pickupIndices.Length);
			for (int i = 0; i < pickupIndices.Length; i++)
			{
				PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndices[i]);
				this.pickupIconAllocator.elements[i].texture = ((pickupDef != null) ? pickupDef.iconTexture : null);
			}
		}

		// Token: 0x060021BC RID: 8636 RVA: 0x00091FF4 File Offset: 0x000901F4
		private static void SetHudDisplayingArenaInfoPanel(HUD hud, bool shouldDisplay)
		{
			List<ArenaInfoPanel> instancesList = InstanceTracker.GetInstancesList<ArenaInfoPanel>();
			ArenaInfoPanel arenaInfoPanel = null;
			for (int i = 0; i < instancesList.Count; i++)
			{
				ArenaInfoPanel arenaInfoPanel2 = instancesList[i];
				if (arenaInfoPanel2.hud == hud)
				{
					arenaInfoPanel = arenaInfoPanel2;
					break;
				}
			}
			if (arenaInfoPanel != shouldDisplay)
			{
				if (!arenaInfoPanel)
				{
					Transform parent = (RectTransform)hud.GetComponent<ChildLocator>().FindChild("RightInfoBar");
					UnityEngine.Object.Instantiate<GameObject>(ArenaInfoPanel.panelPrefab, parent).GetComponent<ArenaInfoPanel>().hud = hud;
					return;
				}
				UnityEngine.Object.Destroy(arenaInfoPanel.gameObject);
			}
		}

		// Token: 0x060021BD RID: 8637 RVA: 0x0009207A File Offset: 0x0009027A
		[SystemInitializer(new Type[]
		{

		})]
		private static void Init()
		{
			ArenaMissionController.onInstanceChangedGlobal += ArenaInfoPanel.ArenaMissionControllerOnOnInstanceChangedGlobal;
			ArenaInfoPanel.panelPrefab = Resources.Load<GameObject>("Prefabs/UI/ArenaInfoPanel");
		}

		// Token: 0x060021BE RID: 8638 RVA: 0x0009209C File Offset: 0x0009029C
		private static void ArenaMissionControllerOnOnInstanceChangedGlobal()
		{
			bool flag = ArenaMissionController.instance;
			if (ArenaInfoPanel.subscribedToHudTargetChanged != flag)
			{
				if (!ArenaInfoPanel.subscribedToHudTargetChanged)
				{
					HUD.onHudTargetChangedGlobal += ArenaInfoPanel.OnHudTargetChangedGlobal;
				}
				else
				{
					HUD.onHudTargetChangedGlobal -= ArenaInfoPanel.OnHudTargetChangedGlobal;
				}
				ArenaInfoPanel.subscribedToHudTargetChanged = !ArenaInfoPanel.subscribedToHudTargetChanged;
				for (int i = 0; i < HUD.readOnlyInstanceList.Count; i++)
				{
					ArenaInfoPanel.SetHudDisplayingArenaInfoPanel(HUD.readOnlyInstanceList[i], flag);
				}
			}
		}

		// Token: 0x060021BF RID: 8639 RVA: 0x0009211C File Offset: 0x0009031C
		private static void OnHudTargetChangedGlobal(HUD hud)
		{
			bool shouldDisplay = ArenaMissionController.instance;
			ArenaInfoPanel.SetHudDisplayingArenaInfoPanel(hud, shouldDisplay);
		}

		// Token: 0x04001F1A RID: 7962
		public RectTransform monsterBodyIconContainer;

		// Token: 0x04001F1B RID: 7963
		public RectTransform pickupIconContainer;

		// Token: 0x04001F1C RID: 7964
		public GameObject iconPrefab;

		// Token: 0x04001F1D RID: 7965
		private UIElementAllocator<RawImage> monsterBodyIconAllocator;

		// Token: 0x04001F1E RID: 7966
		private UIElementAllocator<RawImage> pickupIconAllocator;

		// Token: 0x04001F1F RID: 7967
		private int[] currentMonsterBodyIndices = Array.Empty<int>();

		// Token: 0x04001F20 RID: 7968
		private PickupIndex[] currentPickupIndices = Array.Empty<PickupIndex>();

		// Token: 0x04001F21 RID: 7969
		private HUD hud;

		// Token: 0x04001F22 RID: 7970
		private static GameObject panelPrefab;

		// Token: 0x04001F23 RID: 7971
		private static bool subscribedToHudTargetChanged;
	}
}
