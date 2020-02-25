using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200032F RID: 815
	[RequireComponent(typeof(PurchaseInteraction))]
	public class ShrineChanceBehavior : NetworkBehaviour
	{
		// Token: 0x1400003F RID: 63
		// (add) Token: 0x0600134B RID: 4939 RVA: 0x000529BC File Offset: 0x00050BBC
		// (remove) Token: 0x0600134C RID: 4940 RVA: 0x000529F0 File Offset: 0x00050BF0
		public static event Action<bool, Interactor> onShrineChancePurchaseGlobal;

		// Token: 0x0600134D RID: 4941 RVA: 0x00052A23 File Offset: 0x00050C23
		private void Awake()
		{
			this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
		}

		// Token: 0x0600134E RID: 4942 RVA: 0x00052A31 File Offset: 0x00050C31
		public void Start()
		{
			if (NetworkServer.active)
			{
				this.rng = new Xoroshiro128Plus(Run.instance.treasureRng.nextUlong);
			}
		}

		// Token: 0x0600134F RID: 4943 RVA: 0x00052A54 File Offset: 0x00050C54
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

		// Token: 0x06001350 RID: 4944 RVA: 0x00052AC8 File Offset: 0x00050CC8
		[Server]
		public void AddShrineStack(Interactor activator)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ShrineChanceBehavior::AddShrineStack(RoR2.Interactor)' called on client");
				return;
			}
			PickupIndex none = PickupIndex.none;
			PickupIndex value = this.rng.NextElementUniform<PickupIndex>(Run.instance.availableTier1DropList);
			PickupIndex value2 = this.rng.NextElementUniform<PickupIndex>(Run.instance.availableTier2DropList);
			PickupIndex value3 = this.rng.NextElementUniform<PickupIndex>(Run.instance.availableTier3DropList);
			PickupIndex value4 = this.rng.NextElementUniform<PickupIndex>(Run.instance.availableEquipmentDropList);
			WeightedSelection<PickupIndex> weightedSelection = new WeightedSelection<PickupIndex>(8);
			weightedSelection.AddChoice(none, this.failureWeight);
			weightedSelection.AddChoice(value, this.tier1Weight);
			weightedSelection.AddChoice(value2, this.tier2Weight);
			weightedSelection.AddChoice(value3, this.tier3Weight);
			weightedSelection.AddChoice(value4, this.equipmentWeight);
			PickupIndex pickupIndex = weightedSelection.Evaluate(this.rng.nextNormalizedFloat);
			bool flag = pickupIndex == PickupIndex.none;
			string baseToken;
			if (flag)
			{
				baseToken = "SHRINE_CHANCE_FAIL_MESSAGE";
			}
			else
			{
				baseToken = "SHRINE_CHANCE_SUCCESS_MESSAGE";
				this.successfulPurchaseCount++;
				PickupDropletController.CreatePickupDroplet(pickupIndex, this.dropletOrigin.position, this.dropletOrigin.forward * 20f);
			}
			Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
			{
				subjectAsCharacterBody = activator.GetComponent<CharacterBody>(),
				baseToken = baseToken
			});
			Action<bool, Interactor> action = ShrineChanceBehavior.onShrineChancePurchaseGlobal;
			if (action != null)
			{
				action(flag, activator);
			}
			this.waitingForRefresh = true;
			this.refreshTimer = 2f;
			EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
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

		// Token: 0x06001352 RID: 4946 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x06001353 RID: 4947 RVA: 0x00052CAC File Offset: 0x00050EAC
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06001354 RID: 4948 RVA: 0x0000409B File Offset: 0x0000229B
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x0400120D RID: 4621
		public int maxPurchaseCount;

		// Token: 0x0400120E RID: 4622
		public float costMultiplierPerPurchase;

		// Token: 0x0400120F RID: 4623
		public float failureWeight;

		// Token: 0x04001210 RID: 4624
		public float equipmentWeight;

		// Token: 0x04001211 RID: 4625
		public float tier1Weight;

		// Token: 0x04001212 RID: 4626
		public float tier2Weight;

		// Token: 0x04001213 RID: 4627
		public float tier3Weight;

		// Token: 0x04001214 RID: 4628
		public Transform symbolTransform;

		// Token: 0x04001215 RID: 4629
		public Transform dropletOrigin;

		// Token: 0x04001216 RID: 4630
		public Color shrineColor;

		// Token: 0x04001217 RID: 4631
		private PurchaseInteraction purchaseInteraction;

		// Token: 0x04001218 RID: 4632
		private int successfulPurchaseCount;

		// Token: 0x04001219 RID: 4633
		private float refreshTimer;

		// Token: 0x0400121A RID: 4634
		private const float refreshDuration = 2f;

		// Token: 0x0400121B RID: 4635
		private bool waitingForRefresh;

		// Token: 0x0400121D RID: 4637
		private Xoroshiro128Plus rng;
	}
}
