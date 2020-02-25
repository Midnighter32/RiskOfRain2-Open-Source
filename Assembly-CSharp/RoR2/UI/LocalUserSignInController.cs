using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Rewired;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005EB RID: 1515
	public class LocalUserSignInController : MonoBehaviour
	{
		// Token: 0x060023C1 RID: 9153 RVA: 0x0009C2B0 File Offset: 0x0009A4B0
		private void Start()
		{
			LocalUserSignInCardController[] componentsInChildren = base.GetComponentsInChildren<LocalUserSignInCardController>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				this.cards.Add(componentsInChildren[i]);
			}
		}

		// Token: 0x060023C2 RID: 9154 RVA: 0x0009C2E0 File Offset: 0x0009A4E0
		public bool AreAllCardsReady()
		{
			return this.cards.Any((LocalUserSignInCardController v) => v.rewiredPlayer != null && v.requestedUserProfile == null);
		}

		// Token: 0x060023C3 RID: 9155 RVA: 0x0009C30C File Offset: 0x0009A50C
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

		// Token: 0x060023C4 RID: 9156 RVA: 0x0009C3C0 File Offset: 0x0009A5C0
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

		// Token: 0x060023C5 RID: 9157 RVA: 0x0009C408 File Offset: 0x0009A608
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

		// Token: 0x060023C6 RID: 9158 RVA: 0x0009C44C File Offset: 0x0009A64C
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

		// Token: 0x060023C7 RID: 9159 RVA: 0x0009C56C File Offset: 0x0009A76C
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

		// Token: 0x040021BB RID: 8635
		public GameObject localUserCardPrefab;

		// Token: 0x040021BC RID: 8636
		private readonly List<LocalUserSignInCardController> cards = new List<LocalUserSignInCardController>();
	}
}
