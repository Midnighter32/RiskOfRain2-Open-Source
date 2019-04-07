using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003E0 RID: 992
	[RequireComponent(typeof(PurchaseInteraction))]
	public class ShrineHealingBehavior : NetworkBehaviour
	{
		// Token: 0x06001599 RID: 5529 RVA: 0x00037FB6 File Offset: 0x000361B6
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x0600159A RID: 5530 RVA: 0x000678AA File Offset: 0x00065AAA
		// (set) Token: 0x0600159B RID: 5531 RVA: 0x000678B2 File Offset: 0x00065AB2
		public int purchaseCount { get; private set; }

		// Token: 0x0600159C RID: 5532 RVA: 0x000678BB File Offset: 0x00065ABB
		private void Awake()
		{
			this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
		}

		// Token: 0x0600159D RID: 5533 RVA: 0x000678CC File Offset: 0x00065ACC
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

		// Token: 0x0600159E RID: 5534 RVA: 0x00067940 File Offset: 0x00065B40
		[Server]
		private void SetWardEnabled(bool enableWard)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ShrineHealingBehavior::SetWardEnabled(System.Boolean)' called on client");
				return;
			}
			if (enableWard != this.wardInstance)
			{
				if (enableWard)
				{
					this.wardInstance = UnityEngine.Object.Instantiate<GameObject>(this.wardPrefab, base.transform.position, base.transform.rotation);
					this.wardInstance.GetComponent<TeamFilter>().teamIndex = TeamIndex.Player;
					this.healingWard = this.wardInstance.GetComponent<HealingWard>();
					NetworkServer.Spawn(this.wardInstance);
					return;
				}
				UnityEngine.Object.Destroy(this.wardInstance);
				this.wardInstance = null;
				this.healingWard = null;
			}
		}

		// Token: 0x0600159F RID: 5535 RVA: 0x000679E4 File Offset: 0x00065BE4
		[Server]
		public void AddShrineStack(Interactor activator)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ShrineHealingBehavior::AddShrineStack(RoR2.Interactor)' called on client");
				return;
			}
			this.SetWardEnabled(true);
			Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
			{
				subjectCharacterBodyGameObject = activator.gameObject,
				baseToken = "SHRINE_HEALING_USE_MESSAGE"
			});
			this.waitingForRefresh = true;
			int purchaseCount = this.purchaseCount;
			this.purchaseCount = purchaseCount + 1;
			float networkradius = this.baseRadius + this.radiusBonusPerPurchase * (float)(this.purchaseCount - 1);
			this.healingWard.Networkradius = networkradius;
			this.refreshTimer = 2f;
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
			{
				origin = base.transform.position,
				rotation = Quaternion.identity,
				scale = 1f,
				color = Color.green
			}, true);
			if (this.purchaseCount >= this.maxPurchaseCount)
			{
				this.symbolTransform.gameObject.SetActive(false);
			}
			Action<ShrineHealingBehavior, Interactor> action = ShrineHealingBehavior.onActivated;
			if (action == null)
			{
				return;
			}
			action(this, activator);
		}

		// Token: 0x1400002B RID: 43
		// (add) Token: 0x060015A0 RID: 5536 RVA: 0x00067AF8 File Offset: 0x00065CF8
		// (remove) Token: 0x060015A1 RID: 5537 RVA: 0x00067B2C File Offset: 0x00065D2C
		public static event Action<ShrineHealingBehavior, Interactor> onActivated;

		// Token: 0x060015A3 RID: 5539 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x060015A4 RID: 5540 RVA: 0x00067B60 File Offset: 0x00065D60
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x060015A5 RID: 5541 RVA: 0x00004507 File Offset: 0x00002707
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040018EF RID: 6383
		public GameObject wardPrefab;

		// Token: 0x040018F0 RID: 6384
		private GameObject wardInstance;

		// Token: 0x040018F1 RID: 6385
		public float baseRadius;

		// Token: 0x040018F2 RID: 6386
		public float radiusBonusPerPurchase;

		// Token: 0x040018F3 RID: 6387
		public int maxPurchaseCount;

		// Token: 0x040018F4 RID: 6388
		public float costMultiplierPerPurchase;

		// Token: 0x040018F5 RID: 6389
		public Transform symbolTransform;

		// Token: 0x040018F6 RID: 6390
		private PurchaseInteraction purchaseInteraction;

		// Token: 0x040018F8 RID: 6392
		private float refreshTimer;

		// Token: 0x040018F9 RID: 6393
		private const float refreshDuration = 2f;

		// Token: 0x040018FA RID: 6394
		private bool waitingForRefresh;

		// Token: 0x040018FB RID: 6395
		private HealingWard healingWard;
	}
}
