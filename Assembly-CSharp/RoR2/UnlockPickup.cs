using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200040F RID: 1039
	public class UnlockPickup : MonoBehaviour
	{
		// Token: 0x0600172F RID: 5935 RVA: 0x0006E209 File Offset: 0x0006C409
		private void FixedUpdate()
		{
			this.stopWatch += Time.fixedDeltaTime;
		}

		// Token: 0x06001730 RID: 5936 RVA: 0x0006E220 File Offset: 0x0006C420
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
					subjectCharacterBodyGameObject = activator,
					baseToken = "PLAYER_PICKUP",
					pickupToken = pickupToken,
					pickupColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Unlockable),
					pickupQuantity = 1u
				});
				this.consumed = true;
				UnityEngine.Object.Destroy(base.transform.root.gameObject);
			}
		}

		// Token: 0x06001731 RID: 5937 RVA: 0x0004C6D5 File Offset: 0x0004A8D5
		private static bool BodyHasPickupPermission(CharacterBody body)
		{
			return (body.masterObject ? body.masterObject.GetComponent<PlayerCharacterMasterController>() : null) && body.inventory;
		}

		// Token: 0x06001732 RID: 5938 RVA: 0x0006E2C8 File Offset: 0x0006C4C8
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

		// Token: 0x04001A56 RID: 6742
		public static string itemPickupSoundString = "Play_UI_item_pickup";

		// Token: 0x04001A57 RID: 6743
		private bool consumed;

		// Token: 0x04001A58 RID: 6744
		private float stopWatch;

		// Token: 0x04001A59 RID: 6745
		public float waitDuration = 0.5f;

		// Token: 0x04001A5A RID: 6746
		public string displayNameToken;

		// Token: 0x04001A5B RID: 6747
		public string unlockableName;
	}
}
