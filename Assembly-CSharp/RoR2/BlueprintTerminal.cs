using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200015E RID: 350
	public class BlueprintTerminal : NetworkBehaviour
	{
		// Token: 0x06000671 RID: 1649 RVA: 0x0001A596 File Offset: 0x00018796
		private void SetHasBeenPurchased(bool newHasBeenPurchased)
		{
			if (this.hasBeenPurchased != newHasBeenPurchased)
			{
				this.NetworkhasBeenPurchased = newHasBeenPurchased;
				this.Rebuild();
			}
		}

		// Token: 0x06000672 RID: 1650 RVA: 0x0001A5AE File Offset: 0x000187AE
		public void Start()
		{
			if (NetworkServer.active)
			{
				this.RollChoice();
			}
			if (NetworkClient.active)
			{
				this.Rebuild();
			}
		}

		// Token: 0x06000673 RID: 1651 RVA: 0x0001A5CC File Offset: 0x000187CC
		private void RollChoice()
		{
			WeightedSelection<int> weightedSelection = new WeightedSelection<int>(8);
			for (int i = 0; i < this.unlockableOptions.Length; i++)
			{
				weightedSelection.AddChoice(i, this.unlockableOptions[i].weight);
			}
			this.unlockableChoice = weightedSelection.Evaluate(UnityEngine.Random.value);
			this.Rebuild();
		}

		// Token: 0x06000674 RID: 1652 RVA: 0x0001A624 File Offset: 0x00018824
		private void Rebuild()
		{
			BlueprintTerminal.UnlockableOption unlockableOption = this.unlockableOptions[this.unlockableChoice];
			if (this.displayInstance)
			{
				UnityEngine.Object.Destroy(this.displayInstance);
			}
			this.displayBaseTransform.gameObject.SetActive(!this.hasBeenPurchased);
			if (!this.hasBeenPurchased && this.displayBaseTransform)
			{
				Debug.Log("Found base");
				UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(unlockableOption.unlockableName);
				if (unlockableDef != null)
				{
					Debug.Log("Found unlockable");
					GameObject gameObject = Resources.Load<GameObject>(unlockableDef.displayModelPath);
					if (gameObject)
					{
						Debug.Log("Found prefab");
						this.displayInstance = UnityEngine.Object.Instantiate<GameObject>(gameObject, this.displayBaseTransform.position, this.displayBaseTransform.transform.rotation, this.displayBaseTransform);
						Renderer componentInChildren = this.displayInstance.GetComponentInChildren<Renderer>();
						float num = 1f;
						if (componentInChildren)
						{
							this.displayInstance.transform.rotation = Quaternion.identity;
							Vector3 size = componentInChildren.bounds.size;
							float f = size.x * size.y * size.z;
							num *= Mathf.Pow(this.idealDisplayVolume, 0.33333334f) / Mathf.Pow(f, 0.33333334f);
						}
						this.displayInstance.transform.localScale = new Vector3(num, num, num);
					}
				}
			}
			PurchaseInteraction component = base.GetComponent<PurchaseInteraction>();
			if (component)
			{
				component.Networkcost = unlockableOption.cost;
			}
		}

		// Token: 0x06000675 RID: 1653 RVA: 0x0001A7B8 File Offset: 0x000189B8
		[Server]
		public void GrantUnlock(Interactor interactor)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.BlueprintTerminal::GrantUnlock(RoR2.Interactor)' called on client");
				return;
			}
			this.SetHasBeenPurchased(true);
			string unlockableName = this.unlockableOptions[this.unlockableChoice].unlockableName;
			EffectManager.SpawnEffect(this.unlockEffect, new EffectData
			{
				origin = base.transform.position
			}, true);
			if (Run.instance)
			{
				Util.PlaySound(this.unlockSoundString, interactor.gameObject);
				CharacterBody component = interactor.GetComponent<CharacterBody>();
				Run.instance.GrantUnlockToSinglePlayer(unlockableName, component);
				string pickupToken = "???";
				UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(unlockableName);
				if (unlockableDef != null)
				{
					pickupToken = unlockableDef.nameToken;
				}
				Chat.SendBroadcastChat(new Chat.PlayerPickupChatMessage
				{
					subjectAsCharacterBody = component,
					baseToken = "PLAYER_PICKUP",
					pickupToken = pickupToken,
					pickupColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Unlockable),
					pickupQuantity = 1U
				});
			}
		}

		// Token: 0x06000677 RID: 1655 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000678 RID: 1656 RVA: 0x0001A8AC File Offset: 0x00018AAC
		// (set) Token: 0x06000679 RID: 1657 RVA: 0x0001A8BF File Offset: 0x00018ABF
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

		// Token: 0x0600067A RID: 1658 RVA: 0x0001A900 File Offset: 0x00018B00
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

		// Token: 0x0600067B RID: 1659 RVA: 0x0001A96C File Offset: 0x00018B6C
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

		// Token: 0x040006C3 RID: 1731
		[SyncVar(hook = "SetHasBeenPurchased")]
		private bool hasBeenPurchased;

		// Token: 0x040006C4 RID: 1732
		public Transform displayBaseTransform;

		// Token: 0x040006C5 RID: 1733
		[Tooltip("The unlockable string to grant")]
		public BlueprintTerminal.UnlockableOption[] unlockableOptions;

		// Token: 0x040006C6 RID: 1734
		private int unlockableChoice;

		// Token: 0x040006C7 RID: 1735
		public string unlockSoundString;

		// Token: 0x040006C8 RID: 1736
		public float idealDisplayVolume = 1.5f;

		// Token: 0x040006C9 RID: 1737
		public GameObject unlockEffect;

		// Token: 0x040006CA RID: 1738
		private GameObject displayInstance;

		// Token: 0x0200015F RID: 351
		[Serializable]
		public struct UnlockableOption
		{
			// Token: 0x040006CB RID: 1739
			public string unlockableName;

			// Token: 0x040006CC RID: 1740
			public int cost;

			// Token: 0x040006CD RID: 1741
			public float weight;
		}
	}
}
