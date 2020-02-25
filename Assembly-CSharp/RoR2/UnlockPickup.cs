using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000368 RID: 872
	public class UnlockPickup : MonoBehaviour
	{
		// Token: 0x06001536 RID: 5430 RVA: 0x0005A5DD File Offset: 0x000587DD
		private void FixedUpdate()
		{
			this.stopWatch += Time.fixedDeltaTime;
		}

		// Token: 0x06001537 RID: 5431 RVA: 0x0005A5F4 File Offset: 0x000587F4
		private void GrantPickup(GameObject activator)
		{
			if (Run.instance)
			{
				Util.PlaySound(UnlockPickup.itemPickupSoundString, activator);
				Run.instance.GrantUnlockToAllParticipatingPlayers(this.unlockableName);
				string pickupToken = "???";
				UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(this.unlockableName);
				if (unlockableDef != null)
				{
					pickupToken = unlockableDef.nameToken;
				}
				Chat.SendBroadcastChat(new Chat.PlayerPickupChatMessage
				{
					subjectAsCharacterBody = activator.GetComponent<CharacterBody>(),
					baseToken = "PLAYER_PICKUP",
					pickupToken = pickupToken,
					pickupColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Unlockable),
					pickupQuantity = 1U
				});
				this.consumed = true;
				UnityEngine.Object.Destroy(base.transform.root.gameObject);
			}
		}

		// Token: 0x06001538 RID: 5432 RVA: 0x00031DC1 File Offset: 0x0002FFC1
		private static bool BodyHasPickupPermission(CharacterBody body)
		{
			return (body.masterObject ? body.masterObject.GetComponent<PlayerCharacterMasterController>() : null) && body.inventory;
		}

		// Token: 0x06001539 RID: 5433 RVA: 0x0005A6A0 File Offset: 0x000588A0
		private void OnTriggerStay(Collider other)
		{
			if (NetworkServer.active && this.stopWatch >= this.waitDuration && !this.consumed)
			{
				CharacterBody component = other.GetComponent<CharacterBody>();
				if (component)
				{
					TeamComponent component2 = component.GetComponent<TeamComponent>();
					if (component2 && component2.teamIndex == TeamIndex.Player && component.inventory)
					{
						this.GrantPickup(component.gameObject);
					}
				}
			}
		}

		// Token: 0x040013C7 RID: 5063
		public static string itemPickupSoundString = "Play_UI_item_pickup";

		// Token: 0x040013C8 RID: 5064
		private bool consumed;

		// Token: 0x040013C9 RID: 5065
		private float stopWatch;

		// Token: 0x040013CA RID: 5066
		public float waitDuration = 0.5f;

		// Token: 0x040013CB RID: 5067
		public string displayNameToken;

		// Token: 0x040013CC RID: 5068
		public string unlockableName;
	}
}
