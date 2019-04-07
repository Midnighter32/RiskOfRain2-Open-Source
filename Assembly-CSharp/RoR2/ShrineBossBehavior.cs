using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003DD RID: 989
	[RequireComponent(typeof(PurchaseInteraction))]
	public class ShrineBossBehavior : NetworkBehaviour
	{
		// Token: 0x0600157D RID: 5501 RVA: 0x00037FB6 File Offset: 0x000361B6
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x0600157E RID: 5502 RVA: 0x00067062 File Offset: 0x00065262
		private void Start()
		{
			this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
		}

		// Token: 0x0600157F RID: 5503 RVA: 0x00067070 File Offset: 0x00065270
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

		// Token: 0x06001580 RID: 5504 RVA: 0x000670E4 File Offset: 0x000652E4
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
			if (component && component.master)
			{
				Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
				{
					subjectCharacterBodyGameObject = interactor.gameObject,
					baseToken = "SHRINE_BOSS_USE_MESSAGE"
				});
			}
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
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

		// Token: 0x06001582 RID: 5506 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x06001583 RID: 5507 RVA: 0x000671F8 File Offset: 0x000653F8
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06001584 RID: 5508 RVA: 0x00004507 File Offset: 0x00002707
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040018C7 RID: 6343
		public int maxPurchaseCount;

		// Token: 0x040018C8 RID: 6344
		public float costMultiplierPerPurchase;

		// Token: 0x040018C9 RID: 6345
		public Transform symbolTransform;

		// Token: 0x040018CA RID: 6346
		private PurchaseInteraction purchaseInteraction;

		// Token: 0x040018CB RID: 6347
		private int purchaseCount;

		// Token: 0x040018CC RID: 6348
		private float refreshTimer;

		// Token: 0x040018CD RID: 6349
		private const float refreshDuration = 2f;

		// Token: 0x040018CE RID: 6350
		private bool waitingForRefresh;
	}
}
