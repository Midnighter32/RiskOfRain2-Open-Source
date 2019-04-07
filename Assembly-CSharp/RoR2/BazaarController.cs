using System;
using TMPro;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000264 RID: 612
	public class BazaarController : MonoBehaviour
	{
		// Token: 0x170000CB RID: 203
		// (get) Token: 0x06000B66 RID: 2918 RVA: 0x0003820D File Offset: 0x0003640D
		// (set) Token: 0x06000B67 RID: 2919 RVA: 0x00038214 File Offset: 0x00036414
		public static BazaarController instance { get; private set; }

		// Token: 0x06000B68 RID: 2920 RVA: 0x0003821C File Offset: 0x0003641C
		private void Awake()
		{
			BazaarController.instance = SingletonHelper.Assign<BazaarController>(BazaarController.instance, this);
		}

		// Token: 0x06000B69 RID: 2921 RVA: 0x00004507 File Offset: 0x00002707
		private void Start()
		{
		}

		// Token: 0x06000B6A RID: 2922 RVA: 0x0003822E File Offset: 0x0003642E
		private void OnDestroy()
		{
			BazaarController.instance = SingletonHelper.Unassign<BazaarController>(BazaarController.instance, this);
		}

		// Token: 0x06000B6B RID: 2923 RVA: 0x00038240 File Offset: 0x00036440
		public void CommentOnAnnoy()
		{
			float percentChance = 20f;
			int max = 6;
			if (Util.CheckRoll(percentChance, 0f, null))
			{
				Chat.SendBroadcastChat(new Chat.NpcChatMessage
				{
					sender = this.shopkeeper,
					baseToken = "NEWT_ANNOY_" + UnityEngine.Random.Range(1, max)
				});
			}
		}

		// Token: 0x06000B6C RID: 2924 RVA: 0x00004507 File Offset: 0x00002707
		public void CommentOnEnter()
		{
		}

		// Token: 0x06000B6D RID: 2925 RVA: 0x00004507 File Offset: 0x00002707
		public void CommentOnLeaving()
		{
		}

		// Token: 0x06000B6E RID: 2926 RVA: 0x00038294 File Offset: 0x00036494
		public void CommentOnLunarPurchase()
		{
			float percentChance = 20f;
			int max = 8;
			if (Util.CheckRoll(percentChance, 0f, null))
			{
				Chat.SendBroadcastChat(new Chat.NpcChatMessage
				{
					sender = this.shopkeeper,
					baseToken = "NEWT_LUNAR_PURCHASE_" + UnityEngine.Random.Range(1, max)
				});
			}
		}

		// Token: 0x06000B6F RID: 2927 RVA: 0x00004507 File Offset: 0x00002707
		public void CommentOnBlueprintPurchase()
		{
		}

		// Token: 0x06000B70 RID: 2928 RVA: 0x00004507 File Offset: 0x00002707
		public void CommentOnDronePurchase()
		{
		}

		// Token: 0x06000B71 RID: 2929 RVA: 0x000382E8 File Offset: 0x000364E8
		public void CommentOnUpgrade()
		{
			float percentChance = 100f;
			int max = 3;
			if (Util.CheckRoll(percentChance, 0f, null))
			{
				Chat.SendBroadcastChat(new Chat.NpcChatMessage
				{
					sender = this.shopkeeper,
					baseToken = "NEWT_UPGRADE_" + UnityEngine.Random.Range(1, max)
				});
			}
		}

		// Token: 0x06000B72 RID: 2930 RVA: 0x0003833C File Offset: 0x0003653C
		private void Update()
		{
			if (this.shopkeeper)
			{
				if (!this.shopkeeperInputBank)
				{
					this.shopkeeperInputBank = this.shopkeeper.GetComponent<InputBankTest>();
					return;
				}
				Ray aimRay = new Ray(this.shopkeeperInputBank.aimOrigin, this.shopkeeper.transform.forward);
				this.shopkeeperTargetBody = Util.GetEnemyEasyTarget(this.shopkeeper.GetComponent<CharacterBody>(), aimRay, this.shopkeeperTrackDistance, this.shopkeeperTrackAngle);
				if (this.shopkeeperTargetBody)
				{
					Vector3 direction = this.shopkeeperTargetBody.mainHurtBox.transform.position - aimRay.origin;
					aimRay.direction = direction;
				}
				this.shopkeeperInputBank.aimDirection = aimRay.direction;
			}
		}

		// Token: 0x04000F63 RID: 3939
		public GameObject shopkeeper;

		// Token: 0x04000F64 RID: 3940
		public TextMeshPro shopkeeperChat;

		// Token: 0x04000F65 RID: 3941
		public float shopkeeperTrackDistance = 250f;

		// Token: 0x04000F66 RID: 3942
		public float shopkeeperTrackAngle = 120f;

		// Token: 0x04000F67 RID: 3943
		[Tooltip("Any PurchaseInteraction objects here will have their activation state set based on whether or not the specified unlockable is available.")]
		public PurchaseInteraction[] unlockableTerminals;

		// Token: 0x04000F68 RID: 3944
		private InputBankTest shopkeeperInputBank;

		// Token: 0x04000F69 RID: 3945
		private CharacterBody shopkeeperTargetBody;
	}
}
