using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Rewired;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005FD RID: 1533
	public class LocalUserSignInController : MonoBehaviour
	{
		// Token: 0x06002256 RID: 8790 RVA: 0x000A2354 File Offset: 0x000A0554
		private void Start()
		{
			LocalUserSignInCardController[] componentsInChildren = base.GetComponentsInChildren<LocalUserSignInCardController>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				this.cards.Add(componentsInChildren[i]);
			}
		}

		// Token: 0x06002257 RID: 8791 RVA: 0x000A2384 File Offset: 0x000A0584
		public bool AreAllCardsReady()
		{
			return this.cards.Any((LocalUserSignInCardController v) => v.rewiredPlayer != null && v.requestedUserProfile == null);
		}

		// Token: 0x06002258 RID: 8792 RVA: 0x000A23B0 File Offset: 0x000A05B0
		private void DoSignIn()
		{
			LocalUserManager.LocalUserInitializationInfo[] array = new LocalUserManager.LocalUserInitializationInfo[this.cards.Count((LocalUserSignInCardController v) => v.rewiredPlayer != null)];
			int index = 0;
			for (int i = 0; i < this.cards.Count; i++)
			{
				if (this.cards[i].rewiredPlayer != null)
				{
					array[index++] = new LocalUserManager.LocalUserInitializationInfo
					{
						player = this.cards[index].rewiredPlayer,
						profile = this.cards[index].requestedUserProfile
					};
				}
			}
			LocalUserManager.SetLocalUsers(array);
		}

		// Token: 0x06002259 RID: 8793 RVA: 0x000A2464 File Offset: 0x000A0664
		private LocalUserSignInCardController FindCardAssociatedWithRewiredPlayer(Player rewiredPlayer)
		{
			for (int i = 0; i < this.cards.Count; i++)
			{
				if (this.cards[i].rewiredPlayer == rewiredPlayer)
				{
					return this.cards[i];
				}
			}
			return null;
		}

		// Token: 0x0600225A RID: 8794 RVA: 0x000A24AC File Offset: 0x000A06AC
		private LocalUserSignInCardController FindCardWithoutRewiredPlayer()
		{
			for (int i = 0; i < this.cards.Count; i++)
			{
				if (this.cards[i].rewiredPlayer == null)
				{
					return this.cards[i];
				}
			}
			return null;
		}

		// Token: 0x0600225B RID: 8795 RVA: 0x000A24F0 File Offset: 0x000A06F0
		private void Update()
		{
			IList<Player> players = ReInput.players.Players;
			for (int i = 0; i < players.Count; i++)
			{
				Player player = players[i];
				if (!(player.name == "PlayerMain"))
				{
					LocalUserSignInCardController localUserSignInCardController = this.FindCardAssociatedWithRewiredPlayer(player);
					if (localUserSignInCardController == null)
					{
						if (player.GetButtonDown("Start"))
						{
							LocalUserSignInCardController localUserSignInCardController2 = this.FindCardWithoutRewiredPlayer();
							if (localUserSignInCardController2 != null)
							{
								localUserSignInCardController2.rewiredPlayer = player;
							}
						}
					}
					else if (player.GetButtonDown("UICancel") || !LocalUserSignInController.PlayerHasControllerConnected(player))
					{
						localUserSignInCardController.rewiredPlayer = null;
					}
				}
			}
			ReadOnlyCollection<LocalUser> readOnlyLocalUsersList = LocalUserManager.readOnlyLocalUsersList;
			int num = 4;
			while (this.cards.Count < num)
			{
				this.cards.Add(UnityEngine.Object.Instantiate<GameObject>(this.localUserCardPrefab, base.transform).GetComponent<LocalUserSignInCardController>());
			}
			while (this.cards.Count > num)
			{
				UnityEngine.Object.Destroy(this.cards[this.cards.Count - 1].gameObject);
				this.cards.RemoveAt(this.cards.Count - 1);
			}
		}

		// Token: 0x0600225C RID: 8796 RVA: 0x000A2610 File Offset: 0x000A0810
		private static bool PlayerHasControllerConnected(Player player)
		{
			using (IEnumerator<Controller> enumerator = player.controllers.Controllers.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					Controller controller = enumerator.Current;
					return true;
				}
			}
			return false;
		}

		// Token: 0x04002575 RID: 9589
		public GameObject localUserCardPrefab;

		// Token: 0x04002576 RID: 9590
		private readonly List<LocalUserSignInCardController> cards = new List<LocalUserSignInCardController>();
	}
}
