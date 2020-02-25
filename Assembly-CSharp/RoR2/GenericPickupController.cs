using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using RoR2.Networking;
using RoR2.UI;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200020E RID: 526
	public sealed class GenericPickupController : NetworkBehaviour, IInteractable, IDisplayNameProvider
	{
		// Token: 0x06000B48 RID: 2888 RVA: 0x00031B15 File Offset: 0x0002FD15
		private void SyncPickupIndex(PickupIndex newPickupIndex)
		{
			this.NetworkpickupIndex = newPickupIndex;
			this.UpdatePickupDisplay();
		}

		// Token: 0x06000B49 RID: 2889 RVA: 0x00031B24 File Offset: 0x0002FD24
		[Server]
		private static void SendPickupMessage(CharacterMaster master, PickupIndex pickupIndex)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.GenericPickupController::SendPickupMessage(RoR2.CharacterMaster,RoR2.PickupIndex)' called on client");
				return;
			}
			uint pickupQuantity = 1U;
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

		// Token: 0x06000B4A RID: 2890 RVA: 0x00031BA4 File Offset: 0x0002FDA4
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

		// Token: 0x06000B4B RID: 2891 RVA: 0x00031D04 File Offset: 0x0002FF04
		private void OnTriggerStay(Collider other)
		{
			if (NetworkServer.active && this.waitStartTime.timeSince >= this.waitDuration && !this.consumed)
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
					if (this.pickupIndex.coinValue != 0U)
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

		// Token: 0x06000B4C RID: 2892 RVA: 0x00031DC1 File Offset: 0x0002FFC1
		private static bool BodyHasPickupPermission(CharacterBody body)
		{
			return (body.masterObject ? body.masterObject.GetComponent<PlayerCharacterMasterController>() : null) && body.inventory;
		}

		// Token: 0x06000B4D RID: 2893 RVA: 0x0000AC89 File Offset: 0x00008E89
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x06000B4E RID: 2894 RVA: 0x00031DF2 File Offset: 0x0002FFF2
		public string GetContextString(Interactor activator)
		{
			return string.Format(Language.GetString(this.pickupIndex.GetInteractContextToken()), this.GetDisplayName());
		}

		// Token: 0x06000B4F RID: 2895 RVA: 0x00031E10 File Offset: 0x00030010
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

		// Token: 0x06000B50 RID: 2896 RVA: 0x00031E70 File Offset: 0x00030070
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

		// Token: 0x06000B51 RID: 2897 RVA: 0x00031EC0 File Offset: 0x000300C0
		[Server]
		private void GrantEquipment(CharacterBody body, Inventory inventory)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.GenericPickupController::GrantEquipment(RoR2.CharacterBody,RoR2.Inventory)' called on client");
				return;
			}
			this.waitStartTime = Run.FixedTimeStamp.now;
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

		// Token: 0x06000B52 RID: 2898 RVA: 0x00031FB8 File Offset: 0x000301B8
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

		// Token: 0x06000B53 RID: 2899 RVA: 0x00032018 File Offset: 0x00030218
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
					if (this.pickupIndex.coinValue != 0U)
					{
						this.GrantLunarCoin(body, this.pickupIndex.coinValue);
					}
				}
			}
		}

		// Token: 0x06000B54 RID: 2900 RVA: 0x000320B3 File Offset: 0x000302B3
		private void Start()
		{
			this.waitStartTime = Run.FixedTimeStamp.now;
			this.consumed = false;
			this.UpdatePickupDisplay();
		}

		// Token: 0x06000B55 RID: 2901 RVA: 0x000320CD File Offset: 0x000302CD
		private void OnEnable()
		{
			InstanceTracker.Add<GenericPickupController>(this);
		}

		// Token: 0x06000B56 RID: 2902 RVA: 0x000320D5 File Offset: 0x000302D5
		private void OnDisable()
		{
			InstanceTracker.Remove<GenericPickupController>(this);
		}

		// Token: 0x06000B57 RID: 2903 RVA: 0x000320E0 File Offset: 0x000302E0
		public Interactability GetInteractability(Interactor activator)
		{
			if (!base.enabled)
			{
				return Interactability.Disabled;
			}
			if (this.waitStartTime.timeSince < this.waitDuration || this.consumed)
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

		// Token: 0x06000B58 RID: 2904 RVA: 0x00032130 File Offset: 0x00030330
		public void OnInteractionBegin(Interactor activator)
		{
			this.AttemptGrant(activator.GetComponent<CharacterBody>());
		}

		// Token: 0x06000B59 RID: 2905 RVA: 0x0000B933 File Offset: 0x00009B33
		public bool ShouldShowOnScanner()
		{
			return true;
		}

		// Token: 0x06000B5A RID: 2906 RVA: 0x0003213E File Offset: 0x0003033E
		public string GetDisplayName()
		{
			return Language.GetString(this.pickupIndex.GetPickupNameToken());
		}

		// Token: 0x06000B5B RID: 2907 RVA: 0x00032150 File Offset: 0x00030350
		public void SetPickupIndexFromString(string pickupString)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			PickupIndex networkpickupIndex = PickupIndex.Find(pickupString);
			this.NetworkpickupIndex = networkpickupIndex;
		}

		// Token: 0x06000B5E RID: 2910 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x06000B5F RID: 2911 RVA: 0x000321A0 File Offset: 0x000303A0
		// (set) Token: 0x06000B60 RID: 2912 RVA: 0x000321B3 File Offset: 0x000303B3
		public PickupIndex NetworkpickupIndex
		{
			get
			{
				return this.pickupIndex;
			}
			[param: In]
			set
			{
				uint dirtyBit = 1U;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.SyncPickupIndex(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<PickupIndex>(value, ref this.pickupIndex, dirtyBit);
			}
		}

		// Token: 0x06000B61 RID: 2913 RVA: 0x000321F4 File Offset: 0x000303F4
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				GeneratedNetworkCode._WritePickupIndex_None(writer, this.pickupIndex);
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
				GeneratedNetworkCode._WritePickupIndex_None(writer, this.pickupIndex);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000B62 RID: 2914 RVA: 0x00032260 File Offset: 0x00030460
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

		// Token: 0x04000B9A RID: 2970
		public PickupDisplay pickupDisplay;

		// Token: 0x04000B9B RID: 2971
		[SyncVar(hook = "SyncPickupIndex")]
		public PickupIndex pickupIndex = PickupIndex.none;

		// Token: 0x04000B9C RID: 2972
		public bool selfDestructIfPickupIndexIsNotIdeal;

		// Token: 0x04000B9D RID: 2973
		public SerializablePickupIndex idealPickupIndex;

		// Token: 0x04000B9E RID: 2974
		private static readonly GenericPickupController.PickupMessage pickupMessageInstance = new GenericPickupController.PickupMessage();

		// Token: 0x04000B9F RID: 2975
		public float waitDuration = 0.5f;

		// Token: 0x04000BA0 RID: 2976
		private Run.FixedTimeStamp waitStartTime;

		// Token: 0x04000BA1 RID: 2977
		private bool consumed;

		// Token: 0x04000BA2 RID: 2978
		public const string pickupSoundString = "Play_UI_item_pickup";

		// Token: 0x0200020F RID: 527
		private class PickupMessage : MessageBase
		{
			// Token: 0x06000B63 RID: 2915 RVA: 0x000322A1 File Offset: 0x000304A1
			public void Reset()
			{
				this.masterGameObject = null;
				this.pickupIndex = PickupIndex.none;
				this.pickupQuantity = 0U;
			}

			// Token: 0x06000B65 RID: 2917 RVA: 0x000322BC File Offset: 0x000304BC
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.masterGameObject);
				GeneratedNetworkCode._WritePickupIndex_None(writer, this.pickupIndex);
				writer.WritePackedUInt32(this.pickupQuantity);
			}

			// Token: 0x06000B66 RID: 2918 RVA: 0x000322E2 File Offset: 0x000304E2
			public override void Deserialize(NetworkReader reader)
			{
				this.masterGameObject = reader.ReadGameObject();
				this.pickupIndex = GeneratedNetworkCode._ReadPickupIndex_None(reader);
				this.pickupQuantity = reader.ReadPackedUInt32();
			}

			// Token: 0x04000BA3 RID: 2979
			public GameObject masterGameObject;

			// Token: 0x04000BA4 RID: 2980
			public PickupIndex pickupIndex;

			// Token: 0x04000BA5 RID: 2981
			public uint pickupQuantity;
		}
	}
}
