using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003E1 RID: 993
	[RequireComponent(typeof(PurchaseInteraction))]
	public class ShrineRestackBehavior : NetworkBehaviour
	{
		// Token: 0x060015A6 RID: 5542 RVA: 0x00037FB6 File Offset: 0x000361B6
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x060015A7 RID: 5543 RVA: 0x00067B6E File Offset: 0x00065D6E
		private void Start()
		{
			this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
			if (NetworkServer.active)
			{
				this.rng = new Xoroshiro128Plus(Run.instance.stageRng.nextUlong);
			}
		}

		// Token: 0x060015A8 RID: 5544 RVA: 0x00067BA0 File Offset: 0x00065DA0
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

		// Token: 0x060015A9 RID: 5545 RVA: 0x00067C14 File Offset: 0x00065E14
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
						subjectCharacterBodyGameObject = interactor.gameObject,
						baseToken = "SHRINE_RESTACK_USE_MESSAGE"
					});
				}
			}
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
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

		// Token: 0x060015AB RID: 5547 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x060015AC RID: 5548 RVA: 0x00067D34 File Offset: 0x00065F34
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x060015AD RID: 5549 RVA: 0x00004507 File Offset: 0x00002707
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040018FD RID: 6397
		public int maxPurchaseCount;

		// Token: 0x040018FE RID: 6398
		public float costMultiplierPerPurchase;

		// Token: 0x040018FF RID: 6399
		public Transform symbolTransform;

		// Token: 0x04001900 RID: 6400
		private PurchaseInteraction purchaseInteraction;

		// Token: 0x04001901 RID: 6401
		private int purchaseCount;

		// Token: 0x04001902 RID: 6402
		private float refreshTimer;

		// Token: 0x04001903 RID: 6403
		private const float refreshDuration = 2f;

		// Token: 0x04001904 RID: 6404
		private bool waitingForRefresh;

		// Token: 0x04001905 RID: 6405
		private Xoroshiro128Plus rng;
	}
}
