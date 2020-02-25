using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000332 RID: 818
	[RequireComponent(typeof(PurchaseInteraction))]
	public class ShrineHealingBehavior : NetworkBehaviour
	{
		// Token: 0x0600136E RID: 4974 RVA: 0x00019B5A File Offset: 0x00017D5A
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x17000254 RID: 596
		// (get) Token: 0x0600136F RID: 4975 RVA: 0x00053292 File Offset: 0x00051492
		// (set) Token: 0x06001370 RID: 4976 RVA: 0x0005329A File Offset: 0x0005149A
		public int purchaseCount { get; private set; }

		// Token: 0x06001371 RID: 4977 RVA: 0x000532A3 File Offset: 0x000514A3
		private void Awake()
		{
			this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
		}

		// Token: 0x06001372 RID: 4978 RVA: 0x000532B4 File Offset: 0x000514B4
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

		// Token: 0x06001373 RID: 4979 RVA: 0x00053328 File Offset: 0x00051528
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

		// Token: 0x06001374 RID: 4980 RVA: 0x000533CC File Offset: 0x000515CC
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
				subjectAsCharacterBody = activator.gameObject.GetComponent<CharacterBody>(),
				baseToken = "SHRINE_HEALING_USE_MESSAGE"
			});
			this.waitingForRefresh = true;
			int purchaseCount = this.purchaseCount;
			this.purchaseCount = purchaseCount + 1;
			float networkradius = this.baseRadius + this.radiusBonusPerPurchase * (float)(this.purchaseCount - 1);
			this.healingWard.Networkradius = networkradius;
			this.refreshTimer = 2f;
			EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
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

		// Token: 0x14000041 RID: 65
		// (add) Token: 0x06001375 RID: 4981 RVA: 0x000534E0 File Offset: 0x000516E0
		// (remove) Token: 0x06001376 RID: 4982 RVA: 0x00053514 File Offset: 0x00051714
		public static event Action<ShrineHealingBehavior, Interactor> onActivated;

		// Token: 0x06001378 RID: 4984 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x06001379 RID: 4985 RVA: 0x00053548 File Offset: 0x00051748
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x0600137A RID: 4986 RVA: 0x0000409B File Offset: 0x0000229B
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04001232 RID: 4658
		public GameObject wardPrefab;

		// Token: 0x04001233 RID: 4659
		private GameObject wardInstance;

		// Token: 0x04001234 RID: 4660
		public float baseRadius;

		// Token: 0x04001235 RID: 4661
		public float radiusBonusPerPurchase;

		// Token: 0x04001236 RID: 4662
		public int maxPurchaseCount;

		// Token: 0x04001237 RID: 4663
		public float costMultiplierPerPurchase;

		// Token: 0x04001238 RID: 4664
		public Transform symbolTransform;

		// Token: 0x04001239 RID: 4665
		private PurchaseInteraction purchaseInteraction;

		// Token: 0x0400123B RID: 4667
		private float refreshTimer;

		// Token: 0x0400123C RID: 4668
		private const float refreshDuration = 2f;

		// Token: 0x0400123D RID: 4669
		private bool waitingForRefresh;

		// Token: 0x0400123E RID: 4670
		private HealingWard healingWard;
	}
}
