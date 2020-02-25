using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200032D RID: 813
	[RequireComponent(typeof(PurchaseInteraction))]
	public class ShrineBloodBehavior : NetworkBehaviour
	{
		// Token: 0x0600133B RID: 4923 RVA: 0x00019B5A File Offset: 0x00017D5A
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x0600133C RID: 4924 RVA: 0x0005263F File Offset: 0x0005083F
		private void Start()
		{
			this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
		}

		// Token: 0x0600133D RID: 4925 RVA: 0x00052650 File Offset: 0x00050850
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

		// Token: 0x0600133E RID: 4926 RVA: 0x000526E0 File Offset: 0x000508E0
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
				uint amount = (uint)(component.healthComponent.fullCombinedHealth * (float)this.purchaseInteraction.cost / 100f * this.goldToPaidHpRatio);
				if (component.master)
				{
					component.master.GiveMoney(amount);
					Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
					{
						subjectAsCharacterBody = component,
						baseToken = "SHRINE_BLOOD_USE_MESSAGE",
						paramTokens = new string[]
						{
							amount.ToString()
						}
					});
				}
			}
			EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
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

		// Token: 0x06001340 RID: 4928 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x06001341 RID: 4929 RVA: 0x00052824 File Offset: 0x00050A24
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06001342 RID: 4930 RVA: 0x0000409B File Offset: 0x0000229B
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040011FC RID: 4604
		public int maxPurchaseCount;

		// Token: 0x040011FD RID: 4605
		public float goldToPaidHpRatio = 0.5f;

		// Token: 0x040011FE RID: 4606
		public float costMultiplierPerPurchase;

		// Token: 0x040011FF RID: 4607
		public Transform symbolTransform;

		// Token: 0x04001200 RID: 4608
		private PurchaseInteraction purchaseInteraction;

		// Token: 0x04001201 RID: 4609
		private int purchaseCount;

		// Token: 0x04001202 RID: 4610
		private float refreshTimer;

		// Token: 0x04001203 RID: 4611
		private const float refreshDuration = 2f;

		// Token: 0x04001204 RID: 4612
		private bool waitingForRefresh;
	}
}
