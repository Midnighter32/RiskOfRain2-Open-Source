using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200015A RID: 346
	public class BazaarController : MonoBehaviour
	{
		// Token: 0x170000BC RID: 188
		// (get) Token: 0x0600063F RID: 1599 RVA: 0x00019DC5 File Offset: 0x00017FC5
		// (set) Token: 0x06000640 RID: 1600 RVA: 0x00019DCC File Offset: 0x00017FCC
		public static BazaarController instance { get; private set; }

		// Token: 0x06000641 RID: 1601 RVA: 0x00019DD4 File Offset: 0x00017FD4
		private void Awake()
		{
			BazaarController.instance = SingletonHelper.Assign<BazaarController>(BazaarController.instance, this);
		}

		// Token: 0x06000642 RID: 1602 RVA: 0x00019DE6 File Offset: 0x00017FE6
		private void Start()
		{
			if (NetworkServer.active)
			{
				this.OnStartServer();
			}
		}

		// Token: 0x06000643 RID: 1603 RVA: 0x00019DF5 File Offset: 0x00017FF5
		private void OnStartServer()
		{
			this.rng = new Xoroshiro128Plus(Run.instance.stageRng.nextUlong);
			this.SetUpSeerStations();
		}

		// Token: 0x06000644 RID: 1604 RVA: 0x00019E18 File Offset: 0x00018018
		private void SetUpSeerStations()
		{
			SceneDef nextStageScene = Run.instance.nextStageScene;
			List<SceneDef> list = new List<SceneDef>();
			if (nextStageScene != null)
			{
				int stageOrder = nextStageScene.stageOrder;
				foreach (SceneDef sceneDef in SceneCatalog.allSceneDefs)
				{
					if (sceneDef.stageOrder == stageOrder)
					{
						list.Add(sceneDef);
					}
				}
			}
			foreach (SeerStationController seerStationController in this.seerStations)
			{
				if (list.Count == 0)
				{
					seerStationController.GetComponent<PurchaseInteraction>().SetAvailable(false);
				}
				else
				{
					Util.ShuffleList<SceneDef>(list, this.rng);
					int index = list.Count - 1;
					SceneDef targetScene = list[index];
					list.RemoveAt(index);
					if (this.rng.nextNormalizedFloat < 0.05f)
					{
						targetScene = SceneCatalog.GetSceneDefFromSceneName("goldshores");
					}
					seerStationController.SetTargetScene(targetScene);
				}
			}
		}

		// Token: 0x06000645 RID: 1605 RVA: 0x00019F1C File Offset: 0x0001811C
		private void OnDestroy()
		{
			BazaarController.instance = SingletonHelper.Unassign<BazaarController>(BazaarController.instance, this);
		}

		// Token: 0x06000646 RID: 1606 RVA: 0x00019F30 File Offset: 0x00018130
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

		// Token: 0x06000647 RID: 1607 RVA: 0x0000409B File Offset: 0x0000229B
		public void CommentOnEnter()
		{
		}

		// Token: 0x06000648 RID: 1608 RVA: 0x0000409B File Offset: 0x0000229B
		public void CommentOnLeaving()
		{
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x00019F84 File Offset: 0x00018184
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

		// Token: 0x0600064A RID: 1610 RVA: 0x0000409B File Offset: 0x0000229B
		public void CommentOnBlueprintPurchase()
		{
		}

		// Token: 0x0600064B RID: 1611 RVA: 0x0000409B File Offset: 0x0000229B
		public void CommentOnDronePurchase()
		{
		}

		// Token: 0x0600064C RID: 1612 RVA: 0x00019FD8 File Offset: 0x000181D8
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

		// Token: 0x0600064D RID: 1613 RVA: 0x0001A02C File Offset: 0x0001822C
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

		// Token: 0x040006A9 RID: 1705
		public GameObject shopkeeper;

		// Token: 0x040006AA RID: 1706
		public TextMeshPro shopkeeperChat;

		// Token: 0x040006AB RID: 1707
		public float shopkeeperTrackDistance = 250f;

		// Token: 0x040006AC RID: 1708
		public float shopkeeperTrackAngle = 120f;

		// Token: 0x040006AD RID: 1709
		[Tooltip("Any PurchaseInteraction objects here will have their activation state set based on whether or not the specified unlockable is available.")]
		public PurchaseInteraction[] unlockableTerminals;

		// Token: 0x040006AE RID: 1710
		public SeerStationController[] seerStations;

		// Token: 0x040006AF RID: 1711
		private InputBankTest shopkeeperInputBank;

		// Token: 0x040006B0 RID: 1712
		private CharacterBody shopkeeperTargetBody;

		// Token: 0x040006B1 RID: 1713
		private Xoroshiro128Plus rng;
	}
}
