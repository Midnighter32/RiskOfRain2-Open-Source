using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003DC RID: 988
	[RequireComponent(typeof(PurchaseInteraction))]
	public class ShrineBloodBehavior : NetworkBehaviour
	{
		// Token: 0x06001575 RID: 5493 RVA: 0x00037FB6 File Offset: 0x000361B6
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x06001576 RID: 5494 RVA: 0x00066E63 File Offset: 0x00065063
		private void Start()
		{
			this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
		}

		// Token: 0x06001577 RID: 5495 RVA: 0x00066E74 File Offset: 0x00065074
		public void FixedUpdate()
		{
			if (this.waitingForRefresh)
			{
				this.refreshTimer -= Time.fixedDeltaTime;
				if (this.refreshTimer <= 0f && this.purchaseCount < this.maxPurchaseCount)
				{
					this.purchaseInteraction.SetAvailable(true);
					this.purchaseInteraction.Networkcost = (int)(100f * (1f - Mathf.Pow(1f - (float)this.purchaseInteraction.cost / 100f, this.costMultiplierPerPurchase)));
					this.waitingForRefresh = false;
				}
			}
		}

		// Token: 0x06001578 RID: 5496 RVA: 0x00066F04 File Offset: 0x00065104
		[Server]
		public void AddShrineStack(Interactor interactor)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ShrineBloodBehavior::AddShrineStack(RoR2.Interactor)' called on client");
				return;
			}
			this.waitingForRefresh = true;
			CharacterBody component = interactor.GetComponent<CharacterBody>();
			if (component)
			{
				uint amount = (uint)(component.healthComponent.fullHealth * (float)this.purchaseInteraction.cost / 100f * this.goldToPaidHpRatio);
				if (component.master)
				{
					component.master.GiveMoney(amount);
					Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
					{
						subjectCharacterBodyGameObject = interactor.gameObject,
						baseToken = "SHRINE_BLOOD_USE_MESSAGE",
						paramTokens = new string[]
						{
							amount.ToString()
						}
					});
				}
			}
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
			{
				origin = base.transform.position,
				rotation = Quaternion.identity,
				scale = 1f,
				color = Color.red
			}, true);
			this.purchaseCount++;
			this.refreshTimer = 2f;
			if (this.purchaseCount >= this.maxPurchaseCount)
			{
				this.symbolTransform.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600157A RID: 5498 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x0600157B RID: 5499 RVA: 0x00067054 File Offset: 0x00065254
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x0600157C RID: 5500 RVA: 0x00004507 File Offset: 0x00002707
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040018BE RID: 6334
		public int maxPurchaseCount;

		// Token: 0x040018BF RID: 6335
		public float goldToPaidHpRatio = 0.5f;

		// Token: 0x040018C0 RID: 6336
		public float costMultiplierPerPurchase;

		// Token: 0x040018C1 RID: 6337
		public Transform symbolTransform;

		// Token: 0x040018C2 RID: 6338
		private PurchaseInteraction purchaseInteraction;

		// Token: 0x040018C3 RID: 6339
		private int purchaseCount;

		// Token: 0x040018C4 RID: 6340
		private float refreshTimer;

		// Token: 0x040018C5 RID: 6341
		private const float refreshDuration = 2f;

		// Token: 0x040018C6 RID: 6342
		private bool waitingForRefresh;
	}
}
