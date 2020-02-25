using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200032E RID: 814
	[RequireComponent(typeof(PurchaseInteraction))]
	public class ShrineBossBehavior : NetworkBehaviour
	{
		// Token: 0x06001343 RID: 4931 RVA: 0x00019B5A File Offset: 0x00017D5A
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x06001344 RID: 4932 RVA: 0x00052832 File Offset: 0x00050A32
		private void Start()
		{
			this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
		}

		// Token: 0x06001345 RID: 4933 RVA: 0x00052840 File Offset: 0x00050A40
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

		// Token: 0x06001346 RID: 4934 RVA: 0x000528B4 File Offset: 0x00050AB4
		[Server]
		public void AddShrineStack(Interactor interactor)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ShrineBossBehavior::AddShrineStack(RoR2.Interactor)' called on client");
				return;
			}
			this.waitingForRefresh = true;
			if (TeleporterInteraction.instance)
			{
				TeleporterInteraction.instance.AddShrineStack();
			}
			CharacterBody component = interactor.GetComponent<CharacterBody>();
			Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
			{
				subjectAsCharacterBody = component,
				baseToken = "SHRINE_BOSS_USE_MESSAGE"
			});
			EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
			{
				origin = base.transform.position,
				rotation = Quaternion.identity,
				scale = 1f,
				color = new Color(0.7372549f, 0.90588236f, 0.94509804f)
			}, true);
			this.purchaseCount++;
			this.refreshTimer = 2f;
			if (this.purchaseCount >= this.maxPurchaseCount)
			{
				this.symbolTransform.gameObject.SetActive(false);
			}
		}

		// Token: 0x06001348 RID: 4936 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x06001349 RID: 4937 RVA: 0x000529AC File Offset: 0x00050BAC
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x0600134A RID: 4938 RVA: 0x0000409B File Offset: 0x0000229B
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04001205 RID: 4613
		public int maxPurchaseCount;

		// Token: 0x04001206 RID: 4614
		public float costMultiplierPerPurchase;

		// Token: 0x04001207 RID: 4615
		public Transform symbolTransform;

		// Token: 0x04001208 RID: 4616
		private PurchaseInteraction purchaseInteraction;

		// Token: 0x04001209 RID: 4617
		private int purchaseCount;

		// Token: 0x0400120A RID: 4618
		private float refreshTimer;

		// Token: 0x0400120B RID: 4619
		private const float refreshDuration = 2f;

		// Token: 0x0400120C RID: 4620
		private bool waitingForRefresh;
	}
}
