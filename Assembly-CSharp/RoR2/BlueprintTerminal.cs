using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000268 RID: 616
	public class BlueprintTerminal : NetworkBehaviour
	{
		// Token: 0x06000B93 RID: 2963 RVA: 0x0003888E File Offset: 0x00036A8E
		private void SetHasBeenPurchased(bool newHasBeenPurchased)
		{
			if (this.hasBeenPurchased != newHasBeenPurchased)
			{
				this.NetworkhasBeenPurchased = newHasBeenPurchased;
				this.Rebuild();
			}
		}

		// Token: 0x06000B94 RID: 2964 RVA: 0x000388A6 File Offset: 0x00036AA6
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

		// Token: 0x06000B95 RID: 2965 RVA: 0x000388C4 File Offset: 0x00036AC4
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

		// Token: 0x06000B96 RID: 2966 RVA: 0x0003891C File Offset: 0x00036B1C
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

		// Token: 0x06000B97 RID: 2967 RVA: 0x00038AB0 File Offset: 0x00036CB0
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
			EffectManager.instance.SpawnEffect(this.unlockEffect, new EffectData
			{
				origin = base.transform.position
			}, true);
			if (Run.instance)
			{
				Util.PlaySound(this.unlockSoundString, interactor.gameObject);
				Run.instance.GrantUnlockToSinglePlayer(unlockableName, interactor.GetComponent<CharacterBody>());
				string pickupToken = "???";
				UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(unlockableName);
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

		// Token: 0x06000B99 RID: 2969 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x06000B9A RID: 2970 RVA: 0x00038BAC File Offset: 0x00036DAC
		// (set) Token: 0x06000B9B RID: 2971 RVA: 0x00038BBF File Offset: 0x00036DBF
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

		// Token: 0x06000B9C RID: 2972 RVA: 0x00038C00 File Offset: 0x00036E00
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

		// Token: 0x06000B9D RID: 2973 RVA: 0x00038C6C File Offset: 0x00036E6C
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

		// Token: 0x04000F7B RID: 3963
		[SyncVar(hook = "SetHasBeenPurchased")]
		private bool hasBeenPurchased;

		// Token: 0x04000F7C RID: 3964
		public Transform displayBaseTransform;

		// Token: 0x04000F7D RID: 3965
		[Tooltip("The unlockable string to grant")]
		public BlueprintTerminal.UnlockableOption[] unlockableOptions;

		// Token: 0x04000F7E RID: 3966
		private int unlockableChoice;

		// Token: 0x04000F7F RID: 3967
		public string unlockSoundString;

		// Token: 0x04000F80 RID: 3968
		public float idealDisplayVolume = 1.5f;

		// Token: 0x04000F81 RID: 3969
		public GameObject unlockEffect;

		// Token: 0x04000F82 RID: 3970
		private GameObject displayInstance;

		// Token: 0x02000269 RID: 617
		[Serializable]
		public struct UnlockableOption
		{
			// Token: 0x04000F83 RID: 3971
			public string unlockableName;

			// Token: 0x04000F84 RID: 3972
			public int cost;

			// Token: 0x04000F85 RID: 3973
			public float weight;
		}
	}
}
