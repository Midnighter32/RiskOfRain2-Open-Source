using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000333 RID: 819
	[RequireComponent(typeof(PurchaseInteraction))]
	public class ShrineRestackBehavior : NetworkBehaviour
	{
		// Token: 0x0600137B RID: 4987 RVA: 0x00019B5A File Offset: 0x00017D5A
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x0600137C RID: 4988 RVA: 0x00053556 File Offset: 0x00051756
		private void Start()
		{
			this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
			if (NetworkServer.active)
			{
				this.rng = new Xoroshiro128Plus(Run.instance.stageRng.nextUlong);
			}
		}

		// Token: 0x0600137D RID: 4989 RVA: 0x00053588 File Offset: 0x00051788
		public void FixedUpdate()
		{
			if (this.waitingForRefresh)
			{
				this.refreshTimer -= Time.fixedDeltaTime;
				if (this.refreshTimer <= 0f && this.purchaseCount < this.maxPurchaseCount)
				{
					this.purchaseInteraction.SetAvailable(true);
					this.purchaseInteraction.Networkcost = (int)((float)this.purchaseInteraction.cost * this.costMultiplierPerPurchase);
					this.waitingForRefresh = false;
				}
			}
		}

		// Token: 0x0600137E RID: 4990 RVA: 0x000535FC File Offset: 0x000517FC
		[Server]
		public void AddShrineStack(Interactor interactor)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ShrineRestackBehavior::AddShrineStack(RoR2.Interactor)' called on client");
				return;
			}
			this.waitingForRefresh = true;
			CharacterBody component = interactor.GetComponent<CharacterBody>();
			if (component && component.master)
			{
				Inventory inventory = component.master.inventory;
				if (inventory)
				{
					inventory.ShrineRestackInventory(this.rng);
					Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
					{
						subjectAsCharacterBody = component,
						baseToken = "SHRINE_RESTACK_USE_MESSAGE"
					});
				}
			}
			EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
			{
				origin = base.transform.position,
				rotation = Quaternion.identity,
				scale = 1f,
				color = new Color(1f, 0.23f, 0.6337214f)
			}, true);
			this.purchaseCount++;
			this.refreshTimer = 2f;
			if (this.purchaseCount >= this.maxPurchaseCount)
			{
				this.symbolTransform.gameObject.SetActive(false);
			}
		}

		// Token: 0x06001380 RID: 4992 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x06001381 RID: 4993 RVA: 0x00053710 File Offset: 0x00051910
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06001382 RID: 4994 RVA: 0x0000409B File Offset: 0x0000229B
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04001240 RID: 4672
		public int maxPurchaseCount;

		// Token: 0x04001241 RID: 4673
		public float costMultiplierPerPurchase;

		// Token: 0x04001242 RID: 4674
		public Transform symbolTransform;

		// Token: 0x04001243 RID: 4675
		private PurchaseInteraction purchaseInteraction;

		// Token: 0x04001244 RID: 4676
		private int purchaseCount;

		// Token: 0x04001245 RID: 4677
		private float refreshTimer;

		// Token: 0x04001246 RID: 4678
		private const float refreshDuration = 2f;

		// Token: 0x04001247 RID: 4679
		private bool waitingForRefresh;

		// Token: 0x04001248 RID: 4680
		private Xoroshiro128Plus rng;
	}
}
