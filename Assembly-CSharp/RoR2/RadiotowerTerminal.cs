using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002DD RID: 733
	public class RadiotowerTerminal : NetworkBehaviour
	{
		// Token: 0x060010CF RID: 4303 RVA: 0x00049A3C File Offset: 0x00047C3C
		private void SetHasBeenPurchased(bool newHasBeenPurchased)
		{
			if (this.hasBeenPurchased != newHasBeenPurchased)
			{
				this.NetworkhasBeenPurchased = newHasBeenPurchased;
			}
		}

		// Token: 0x060010D0 RID: 4304 RVA: 0x00049A4E File Offset: 0x00047C4E
		public void Start()
		{
			if (NetworkServer.active)
			{
				this.FindStageLogUnlockable();
			}
			bool active = NetworkClient.active;
		}

		// Token: 0x060010D1 RID: 4305 RVA: 0x00049A64 File Offset: 0x00047C64
		private void FindStageLogUnlockable()
		{
			SceneDef mostRecentSceneDef = SceneCatalog.mostRecentSceneDef;
			if (mostRecentSceneDef)
			{
				this.unlockableName = SceneCatalog.GetUnlockableLogFromSceneName(mostRecentSceneDef.baseSceneName);
			}
		}

		// Token: 0x060010D2 RID: 4306 RVA: 0x00049A90 File Offset: 0x00047C90
		[Server]
		public void GrantUnlock(Interactor interactor)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.RadiotowerTerminal::GrantUnlock(RoR2.Interactor)' called on client");
				return;
			}
			EffectManager.SpawnEffect(this.unlockEffect, new EffectData
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
					subjectAsCharacterBody = interactor.GetComponent<CharacterBody>(),
					baseToken = "PLAYER_PICKUP",
					pickupToken = pickupToken,
					pickupColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Unlockable),
					pickupQuantity = 1U
				});
			}
		}

		// Token: 0x060010D4 RID: 4308 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x1700020E RID: 526
		// (get) Token: 0x060010D5 RID: 4309 RVA: 0x00049B64 File Offset: 0x00047D64
		// (set) Token: 0x060010D6 RID: 4310 RVA: 0x00049B77 File Offset: 0x00047D77
		public bool NetworkhasBeenPurchased
		{
			get
			{
				return this.hasBeenPurchased;
			}
			[param: In]
			set
			{
				uint dirtyBit = 1U;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.SetHasBeenPurchased(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<bool>(value, ref this.hasBeenPurchased, dirtyBit);
			}
		}

		// Token: 0x060010D7 RID: 4311 RVA: 0x00049BB8 File Offset: 0x00047DB8
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.hasBeenPurchased);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1U) != 0U)
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

		// Token: 0x060010D8 RID: 4312 RVA: 0x00049C24 File Offset: 0x00047E24
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

		// Token: 0x04001022 RID: 4130
		[SyncVar(hook = "SetHasBeenPurchased")]
		private bool hasBeenPurchased;

		// Token: 0x04001023 RID: 4131
		private string unlockableName;

		// Token: 0x04001024 RID: 4132
		public string unlockSoundString;

		// Token: 0x04001025 RID: 4133
		public GameObject unlockEffect;
	}
}
