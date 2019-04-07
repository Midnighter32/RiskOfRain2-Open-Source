using System;
using System.Collections.ObjectModel;
using RoR2.Networking;
using RoR2.UI;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002FD RID: 765
	public class GenericPickupController : NetworkBehaviour, IInteractable, IDisplayNameProvider
	{
		// Token: 0x06000F80 RID: 3968 RVA: 0x0004C419 File Offset: 0x0004A619
		private void SyncPickupIndex(PickupIndex newPickupIndex)
		{
			this.NetworkpickupIndex = newPickupIndex;
			this.UpdatePickupDisplay();
		}

		// Token: 0x06000F81 RID: 3969 RVA: 0x0004C428 File Offset: 0x0004A628
		[Server]
		private static void SendPickupMessage(CharacterMaster master, PickupIndex pickupIndex)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.GenericPickupController::SendPickupMessage(RoR2.CharacterMaster,RoR2.PickupIndex)' called on client");
				return;
			}
			uint pickupQuantity = 1u;
			if (master.inventory)
			{
				ItemIndex itemIndex = pickupIndex.itemIndex;
				if (itemIndex != ItemIndex.None)
				{
					pickupQuantity = (uint)master.inventory.GetItemCount(itemIndex);
				}
			}
			GenericPickupController.PickupMessage msg = new GenericPickupController.PickupMessage
			{
				masterGameObject = master.gameObject,
				pickupIndex = pickupIndex,
				pickupQuantity = pickupQuantity
			};
			NetworkServer.SendByChannelToAll(57, msg, QosChannelIndex.chat.intVal);
		}

		// Token: 0x06000F82 RID: 3970 RVA: 0x0004C4A8 File Offset: 0x0004A6A8
		[NetworkMessageHandler(msgType = 57, client = true)]
		private static void HandlePickupMessage(NetworkMessage netMsg)
		{
			Debug.Log("GenericPickupController.HandlePickupMessage: Received pickup message.");
			ReadOnlyCollection<NotificationQueue> readOnlyInstancesList = NotificationQueue.readOnlyInstancesList;
			GenericPickupController.PickupMessage pickupMessage = GenericPickupController.pickupMessageInstance;
			netMsg.ReadMessage<GenericPickupController.PickupMessage>(pickupMessage);
			GameObject masterGameObject = pickupMessage.masterGameObject;
			PickupIndex pickupIndex = pickupMessage.pickupIndex;
			uint pickupQuantity = pickupMessage.pickupQuantity;
			pickupMessage.Reset();
			if (!masterGameObject)
			{
				Debug.Log("GenericPickupController.HandlePickupMessage: failed! masterObject is not valid.");
				return;
			}
			CharacterMaster component = masterGameObject.GetComponent<CharacterMaster>();
			if (!component)
			{
				Debug.Log("GenericPickupController.HandlePickupMessage: failed! master component is not valid.");
				return;
			}
			PlayerCharacterMasterController component2 = component.GetComponent<PlayerCharacterMasterController>();
			if (component2)
			{
				NetworkUser networkUser = component2.networkUser;
				if (networkUser)
				{
					LocalUser localUser = networkUser.localUser;
					if (localUser != null)
					{
						localUser.userProfile.DiscoverPickup(pickupIndex);
					}
				}
			}
			for (int i = 0; i < readOnlyInstancesList.Count; i++)
			{
				readOnlyInstancesList[i].OnPickup(component, pickupIndex);
			}
			CharacterBody body = component.GetBody();
			if (!body)
			{
				Debug.Log("GenericPickupController.HandlePickupMessage: failed! characterBody is not valid.");
			}
			ItemDef itemDef = ItemCatalog.GetItemDef(pickupIndex.itemIndex);
			if (itemDef != null && itemDef.hidden)
			{
				Debug.LogFormat("GenericPickupController.HandlePickupMessage: skipped item {0}, marked hidden.", new object[]
				{
					itemDef.nameToken
				});
				return;
			}
			Chat.AddPickupMessage(body, pickupIndex.GetPickupNameToken(), pickupIndex.GetPickupColor(), pickupQuantity);
			if (body)
			{
				Util.PlaySound("Play_UI_item_pickup", body.gameObject);
			}
		}

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x06000F83 RID: 3971 RVA: 0x0004C607 File Offset: 0x0004A807
		private float stopWatch
		{
			get
			{
				return Run.instance.fixedTime - this.waitStartTime;
			}
		}

		// Token: 0x06000F84 RID: 3972 RVA: 0x0004C61C File Offset: 0x0004A81C
		private void OnTriggerStay(Collider other)
		{
			if (NetworkServer.active && this.stopWatch >= this.waitDuration && !this.consumed)
			{
				CharacterBody component = other.GetComponent<CharacterBody>();
				if (component)
				{
					ItemIndex itemIndex = this.pickupIndex.itemIndex;
					if (itemIndex != ItemIndex.None && ItemCatalog.GetItemDef(itemIndex).tier == ItemTier.Lunar)
					{
						return;
					}
					EquipmentIndex equipmentIndex = this.pickupIndex.equipmentIndex;
					if (equipmentIndex != EquipmentIndex.None)
					{
						if (EquipmentCatalog.GetEquipmentDef(equipmentIndex).isLunar)
						{
							return;
						}
						if (component.inventory && component.inventory.currentEquipmentIndex != EquipmentIndex.None)
						{
							return;
						}
					}
					if (this.pickupIndex.coinIndex != -1)
					{
						return;
					}
					if (GenericPickupController.BodyHasPickupPermission(component))
					{
						this.AttemptGrant(component);
					}
				}
			}
		}

		// Token: 0x06000F85 RID: 3973 RVA: 0x0004C6D5 File Offset: 0x0004A8D5
		private static bool BodyHasPickupPermission(CharacterBody body)
		{
			return (body.masterObject ? body.masterObject.GetComponent<PlayerCharacterMasterController>() : null) && body.inventory;
		}

		// Token: 0x06000F86 RID: 3974 RVA: 0x0000A1ED File Offset: 0x000083ED
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x06000F87 RID: 3975 RVA: 0x0004C708 File Offset: 0x0004A908
		public string GetContextString(Interactor activator)
		{
			string token = "";
			if (this.pickupIndex.itemIndex != ItemIndex.None)
			{
				token = "ITEM_PICKUP_CONTEXT";
			}
			if (this.pickupIndex.equipmentIndex != EquipmentIndex.None)
			{
				token = "EQUIPMENT_PICKUP_CONTEXT";
			}
			if (this.pickupIndex.coinIndex != -1)
			{
				token = "LUNAR_COIN_PICKUP_CONTEXT";
			}
			return string.Format(Language.GetString(token), this.GetDisplayName());
		}

		// Token: 0x06000F88 RID: 3976 RVA: 0x0004C768 File Offset: 0x0004A968
		private void UpdatePickupDisplay()
		{
			if (!this.pickupDisplay)
			{
				return;
			}
			this.pickupDisplay.SetPickupIndex(this.pickupIndex, false);
			if (this.pickupDisplay.modelRenderer)
			{
				Highlight component = base.GetComponent<Highlight>();
				if (component)
				{
					component.targetRenderer = this.pickupDisplay.modelRenderer;
				}
			}
		}

		// Token: 0x06000F89 RID: 3977 RVA: 0x0004C7C8 File Offset: 0x0004A9C8
		[Server]
		private void GrantItem(CharacterBody body, Inventory inventory)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.GenericPickupController::GrantItem(RoR2.CharacterBody,RoR2.Inventory)' called on client");
				return;
			}
			inventory.GiveItem(this.pickupIndex.itemIndex, 1);
			GenericPickupController.SendPickupMessage(inventory.GetComponent<CharacterMaster>(), this.pickupIndex);
			UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x06000F8A RID: 3978 RVA: 0x0004C818 File Offset: 0x0004AA18
		[Server]
		private void GrantEquipment(CharacterBody body, Inventory inventory)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.GenericPickupController::GrantEquipment(RoR2.CharacterBody,RoR2.Inventory)' called on client");
				return;
			}
			this.waitStartTime = Run.instance.fixedTime;
			EquipmentIndex currentEquipmentIndex = inventory.currentEquipmentIndex;
			EquipmentIndex equipmentIndex = this.pickupIndex.equipmentIndex;
			inventory.SetEquipmentIndex(equipmentIndex);
			this.NetworkpickupIndex = new PickupIndex(currentEquipmentIndex);
			this.consumed = false;
			GenericPickupController.SendPickupMessage(inventory.GetComponent<CharacterMaster>(), new PickupIndex(equipmentIndex));
			if (this.pickupIndex == PickupIndex.none)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			if (this.selfDestructIfPickupIndexIsNotIdeal && this.pickupIndex != PickupIndex.Find(this.idealPickupIndex.pickupName))
			{
				PickupDropletController.CreatePickupDroplet(this.pickupIndex, base.transform.position, new Vector3(UnityEngine.Random.Range(-4f, 4f), 20f, UnityEngine.Random.Range(-4f, 4f)));
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06000F8B RID: 3979 RVA: 0x0004C914 File Offset: 0x0004AB14
		[Server]
		private void GrantLunarCoin(CharacterBody body, uint count)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.GenericPickupController::GrantLunarCoin(RoR2.CharacterBody,System.UInt32)' called on client");
				return;
			}
			CharacterMaster master = body.master;
			NetworkUser networkUser = Util.LookUpBodyNetworkUser(body);
			if (networkUser)
			{
				if (master)
				{
					GenericPickupController.SendPickupMessage(master, this.pickupIndex);
				}
				networkUser.AwardLunarCoins(count);
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06000F8C RID: 3980 RVA: 0x0004C974 File Offset: 0x0004AB74
		[Server]
		private void AttemptGrant(CharacterBody body)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.GenericPickupController::AttemptGrant(RoR2.CharacterBody)' called on client");
				return;
			}
			TeamComponent component = body.GetComponent<TeamComponent>();
			if (component && component.teamIndex == TeamIndex.Player)
			{
				Inventory inventory = body.inventory;
				if (inventory)
				{
					this.consumed = true;
					if (this.pickupIndex.itemIndex != ItemIndex.None)
					{
						this.GrantItem(body, inventory);
					}
					if (this.pickupIndex.equipmentIndex != EquipmentIndex.None)
					{
						this.GrantEquipment(body, inventory);
					}
					if (this.pickupIndex.coinIndex != -1)
					{
						this.GrantLunarCoin(body, 1u);
					}
				}
			}
		}

		// Token: 0x06000F8D RID: 3981 RVA: 0x0004CA06 File Offset: 0x0004AC06
		private void Start()
		{
			this.waitStartTime = Run.instance.fixedTime;
			this.consumed = false;
			this.UpdatePickupDisplay();
		}

		// Token: 0x06000F8E RID: 3982 RVA: 0x0004CA28 File Offset: 0x0004AC28
		public Interactability GetInteractability(Interactor activator)
		{
			if (!base.enabled)
			{
				return Interactability.Disabled;
			}
			if (this.stopWatch < this.waitDuration || this.consumed)
			{
				return Interactability.Disabled;
			}
			CharacterBody component = activator.GetComponent<CharacterBody>();
			if (!component)
			{
				return Interactability.Disabled;
			}
			if (!GenericPickupController.BodyHasPickupPermission(component))
			{
				return Interactability.Disabled;
			}
			return Interactability.Available;
		}

		// Token: 0x06000F8F RID: 3983 RVA: 0x0004CA73 File Offset: 0x0004AC73
		public void OnInteractionBegin(Interactor activator)
		{
			this.AttemptGrant(activator.GetComponent<CharacterBody>());
		}

		// Token: 0x06000F90 RID: 3984 RVA: 0x0004CA81 File Offset: 0x0004AC81
		public string GetDisplayName()
		{
			return Language.GetString(this.pickupIndex.GetPickupNameToken());
		}

		// Token: 0x06000F91 RID: 3985 RVA: 0x0004CA94 File Offset: 0x0004AC94
		public void SetPickupIndexFromString(string pickupString)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			PickupIndex networkpickupIndex = PickupIndex.Find(pickupString);
			this.NetworkpickupIndex = networkpickupIndex;
		}

		// Token: 0x06000F94 RID: 3988 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x06000F95 RID: 3989 RVA: 0x0004CAE4 File Offset: 0x0004ACE4
		// (set) Token: 0x06000F96 RID: 3990 RVA: 0x0004CAF7 File Offset: 0x0004ACF7
		public PickupIndex NetworkpickupIndex
		{
			get
			{
				return this.pickupIndex;
			}
			set
			{
				uint dirtyBit = 1u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.SyncPickupIndex(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<PickupIndex>(value, ref this.pickupIndex, dirtyBit);
			}
		}

		// Token: 0x06000F97 RID: 3991 RVA: 0x0004CB38 File Offset: 0x0004AD38
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				GeneratedNetworkCode._WritePickupIndex_None(writer, this.pickupIndex);
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
				GeneratedNetworkCode._WritePickupIndex_None(writer, this.pickupIndex);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000F98 RID: 3992 RVA: 0x0004CBA4 File Offset: 0x0004ADA4
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.pickupIndex = GeneratedNetworkCode._ReadPickupIndex_None(reader);
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.SyncPickupIndex(GeneratedNetworkCode._ReadPickupIndex_None(reader));
			}
		}

		// Token: 0x04001395 RID: 5013
		public PickupDisplay pickupDisplay;

		// Token: 0x04001396 RID: 5014
		[SyncVar(hook = "SyncPickupIndex")]
		public PickupIndex pickupIndex = PickupIndex.none;

		// Token: 0x04001397 RID: 5015
		public bool selfDestructIfPickupIndexIsNotIdeal;

		// Token: 0x04001398 RID: 5016
		public SerializablePickupIndex idealPickupIndex;

		// Token: 0x04001399 RID: 5017
		private static readonly GenericPickupController.PickupMessage pickupMessageInstance = new GenericPickupController.PickupMessage();

		// Token: 0x0400139A RID: 5018
		public float waitDuration = 0.5f;

		// Token: 0x0400139B RID: 5019
		private float waitStartTime;

		// Token: 0x0400139C RID: 5020
		private bool consumed;

		// Token: 0x0400139D RID: 5021
		public const string pickupSoundString = "Play_UI_item_pickup";

		// Token: 0x020002FE RID: 766
		private class PickupMessage : MessageBase
		{
			// Token: 0x06000F99 RID: 3993 RVA: 0x0004CBE5 File Offset: 0x0004ADE5
			public void Reset()
			{
				this.masterGameObject = null;
				this.pickupIndex = PickupIndex.none;
				this.pickupQuantity = 0u;
			}

			// Token: 0x06000F9B RID: 3995 RVA: 0x0004CC00 File Offset: 0x0004AE00
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.masterGameObject);
				GeneratedNetworkCode._WritePickupIndex_None(writer, this.pickupIndex);
				writer.WritePackedUInt32(this.pickupQuantity);
			}

			// Token: 0x06000F9C RID: 3996 RVA: 0x0004CC26 File Offset: 0x0004AE26
			public override void Deserialize(NetworkReader reader)
			{
				this.masterGameObject = reader.ReadGameObject();
				this.pickupIndex = GeneratedNetworkCode._ReadPickupIndex_None(reader);
				this.pickupQuantity = reader.ReadPackedUInt32();
			}

			// Token: 0x0400139E RID: 5022
			public GameObject masterGameObject;

			// Token: 0x0400139F RID: 5023
			public PickupIndex pickupIndex;

			// Token: 0x040013A0 RID: 5024
			public uint pickupQuantity;
		}
	}
}
