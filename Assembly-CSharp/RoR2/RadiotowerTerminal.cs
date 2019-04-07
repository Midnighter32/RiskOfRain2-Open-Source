using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace RoR2
{
	// Token: 0x020003A0 RID: 928
	public class RadiotowerTerminal : NetworkBehaviour
	{
		// Token: 0x060013A8 RID: 5032 RVA: 0x00060063 File Offset: 0x0005E263
		private void SetHasBeenPurchased(bool newHasBeenPurchased)
		{
			if (this.hasBeenPurchased != newHasBeenPurchased)
			{
				this.NetworkhasBeenPurchased = newHasBeenPurchased;
			}
		}

		// Token: 0x060013A9 RID: 5033 RVA: 0x00060075 File Offset: 0x0005E275
		public void Start()
		{
			if (NetworkServer.active)
			{
				this.FindStageLogUnlockable();
			}
			bool active = NetworkClient.active;
		}

		// Token: 0x060013AA RID: 5034 RVA: 0x0006008C File Offset: 0x0005E28C
		private void FindStageLogUnlockable()
		{
			this.unlockableName = SceneCatalog.GetUnlockableLogFromSceneName(SceneManager.GetActiveScene().name);
		}

		// Token: 0x060013AB RID: 5035 RVA: 0x000600B4 File Offset: 0x0005E2B4
		[Server]
		public void GrantUnlock(Interactor interactor)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.RadiotowerTerminal::GrantUnlock(RoR2.Interactor)' called on client");
				return;
			}
			EffectManager.instance.SpawnEffect(this.unlockEffect, new EffectData
			{
				origin = base.transform.position
			}, true);
			this.SetHasBeenPurchased(true);
			if (Run.instance)
			{
				Util.PlaySound(this.unlockSoundString, interactor.gameObject);
				Run.instance.GrantUnlockToAllParticipatingPlayers(this.unlockableName);
				string pickupToken = "???";
				UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(this.unlockableName);
				if (unlockableDef != null)
				{
					pickupToken = unlockableDef.nameToken;
				}
				Chat.SendBroadcastChat(new Chat.PlayerPickupChatMessage
				{
					subjectCharacterBodyGameObject = interactor.gameObject,
					baseToken = "PLAYER_PICKUP",
					pickupToken = pickupToken,
					pickupColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Unlockable),
					pickupQuantity = 1u
				});
			}
		}

		// Token: 0x060013AD RID: 5037 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x060013AE RID: 5038 RVA: 0x0006018C File Offset: 0x0005E38C
		// (set) Token: 0x060013AF RID: 5039 RVA: 0x0006019F File Offset: 0x0005E39F
		public bool NetworkhasBeenPurchased
		{
			get
			{
				return this.hasBeenPurchased;
			}
			set
			{
				uint dirtyBit = 1u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.SetHasBeenPurchased(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<bool>(value, ref this.hasBeenPurchased, dirtyBit);
			}
		}

		// Token: 0x060013B0 RID: 5040 RVA: 0x000601E0 File Offset: 0x0005E3E0
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.hasBeenPurchased);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.hasBeenPurchased);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x060013B1 RID: 5041 RVA: 0x0006024C File Offset: 0x0005E44C
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.hasBeenPurchased = reader.ReadBoolean();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.SetHasBeenPurchased(reader.ReadBoolean());
			}
		}

		// Token: 0x04001753 RID: 5971
		[SyncVar(hook = "SetHasBeenPurchased")]
		private bool hasBeenPurchased;

		// Token: 0x04001754 RID: 5972
		private string unlockableName;

		// Token: 0x04001755 RID: 5973
		public string unlockSoundString;

		// Token: 0x04001756 RID: 5974
		public GameObject unlockEffect;
	}
}
