using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003DE RID: 990
	[RequireComponent(typeof(PurchaseInteraction))]
	public class ShrineChanceBehavior : NetworkBehaviour
	{
		// Token: 0x1400002A RID: 42
		// (add) Token: 0x06001585 RID: 5509 RVA: 0x00067208 File Offset: 0x00065408
		// (remove) Token: 0x06001586 RID: 5510 RVA: 0x0006723C File Offset: 0x0006543C
		public static event Action<bool, Interactor> onShrineChancePurchaseGlobal;

		// Token: 0x06001587 RID: 5511 RVA: 0x0006726F File Offset: 0x0006546F
		private void Awake()
		{
			this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
		}

		// Token: 0x06001588 RID: 5512 RVA: 0x0006727D File Offset: 0x0006547D
		public void Start()
		{
			if (NetworkServer.active)
			{
				this.rng = new Xoroshiro128Plus(Run.instance.treasureRng.nextUlong);
			}
		}

		// Token: 0x06001589 RID: 5513 RVA: 0x000672A0 File Offset: 0x000654A0
		public void FixedUpdate()
		{
			if (this.waitingForRefresh)
			{
				this.refreshTimer -= Time.fixedDeltaTime;
				if (this.refreshTimer <= 0f && this.successfulPurchaseCount < this.maxPurchaseCount)
				{
					this.purchaseInteraction.SetAvailable(true);
					this.purchaseInteraction.Networkcost = (int)((float)this.purchaseInteraction.cost * this.costMultiplierPerPurchase);
					this.waitingForRefresh = false;
				}
			}
		}

		// Token: 0x0600158A RID: 5514 RVA: 0x00067314 File Offset: 0x00065514
		[Server]
		public void AddShrineStack(Interactor activator)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ShrineChanceBehavior::AddShrineStack(RoR2.Interactor)' called on client");
				return;
			}
			PickupIndex none = PickupIndex.none;
			PickupIndex value = Run.instance.availableTier1DropList[this.rng.RangeInt(0, Run.instance.availableTier1DropList.Count - 1)];
			PickupIndex value2 = Run.instance.availableTier2DropList[this.rng.RangeInt(0, Run.instance.availableTier2DropList.Count - 1)];
			PickupIndex value3 = Run.instance.availableTier3DropList[this.rng.RangeInt(0, Run.instance.availableTier3DropList.Count - 1)];
			PickupIndex value4 = Run.instance.availableEquipmentDropList[this.rng.RangeInt(0, Run.instance.availableEquipmentDropList.Count - 1)];
			WeightedSelection<PickupIndex> weightedSelection = new WeightedSelection<PickupIndex>(8);
			weightedSelection.AddChoice(none, this.failureWeight);
			weightedSelection.AddChoice(value, this.tier1Weight);
			weightedSelection.AddChoice(value2, this.tier2Weight);
			weightedSelection.AddChoice(value3, this.tier3Weight);
			weightedSelection.AddChoice(value4, this.equipmentWeight);
			PickupIndex pickupIndex = weightedSelection.Evaluate(this.rng.nextNormalizedFloat);
			bool flag = pickupIndex == PickupIndex.none;
			if (flag)
			{
				Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
				{
					subjectCharacterBodyGameObject = activator.gameObject,
					baseToken = "SHRINE_CHANCE_FAIL_MESSAGE"
				});
			}
			else
			{
				this.successfulPurchaseCount++;
				PickupDropletController.CreatePickupDroplet(pickupIndex, this.dropletOrigin.position, this.dropletOrigin.forward * 20f);
				Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
				{
					subjectCharacterBodyGameObject = activator.gameObject,
					baseToken = "SHRINE_CHANCE_SUCCESS_MESSAGE"
				});
			}
			Action<bool, Interactor> action = ShrineChanceBehavior.onShrineChancePurchaseGlobal;
			if (action != null)
			{
				action(flag, activator);
			}
			this.waitingForRefresh = true;
			this.refreshTimer = 2f;
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
			{
				origin = base.transform.position,
				rotation = Quaternion.identity,
				scale = 1f,
				color = this.shrineColor
			}, true);
			if (this.successfulPurchaseCount >= this.maxPurchaseCount)
			{
				this.symbolTransform.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600158C RID: 5516 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x0600158D RID: 5517 RVA: 0x0006756C File Offset: 0x0006576C
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x0600158E RID: 5518 RVA: 0x00004507 File Offset: 0x00002707
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040018CF RID: 6351
		public int maxPurchaseCount;

		// Token: 0x040018D0 RID: 6352
		public float costMultiplierPerPurchase;

		// Token: 0x040018D1 RID: 6353
		public float failureWeight;

		// Token: 0x040018D2 RID: 6354
		public float equipmentWeight;

		// Token: 0x040018D3 RID: 6355
		public float tier1Weight;

		// Token: 0x040018D4 RID: 6356
		public float tier2Weight;

		// Token: 0x040018D5 RID: 6357
		public float tier3Weight;

		// Token: 0x040018D6 RID: 6358
		public Transform symbolTransform;

		// Token: 0x040018D7 RID: 6359
		public Transform dropletOrigin;

		// Token: 0x040018D8 RID: 6360
		public Color shrineColor;

		// Token: 0x040018D9 RID: 6361
		private PurchaseInteraction purchaseInteraction;

		// Token: 0x040018DA RID: 6362
		private int successfulPurchaseCount;

		// Token: 0x040018DB RID: 6363
		private float refreshTimer;

		// Token: 0x040018DC RID: 6364
		private const float refreshDuration = 2f;

		// Token: 0x040018DD RID: 6365
		private bool waitingForRefresh;

		// Token: 0x040018DF RID: 6367
		private Xoroshiro128Plus rng;
	}
}
