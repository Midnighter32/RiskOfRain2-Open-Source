using System;
using System.Collections.Generic;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002DF RID: 735
	public class EffectManager : MonoBehaviour
	{
		// Token: 0x1700013E RID: 318
		// (get) Token: 0x06000EB1 RID: 3761 RVA: 0x000486D3 File Offset: 0x000468D3
		// (set) Token: 0x06000EB0 RID: 3760 RVA: 0x000486CB File Offset: 0x000468CB
		public static EffectManager instance { get; private set; }

		// Token: 0x06000EB2 RID: 3762 RVA: 0x000486DA File Offset: 0x000468DA
		private void OnEnable()
		{
			if (EffectManager.instance)
			{
				Debug.LogError("Only one EffectManager can exist at a time.");
				return;
			}
			EffectManager.instance = this;
		}

		// Token: 0x06000EB3 RID: 3763 RVA: 0x000486F9 File Offset: 0x000468F9
		private void OnDisable()
		{
			if (EffectManager.instance == this)
			{
				EffectManager.instance = null;
			}
		}

		// Token: 0x06000EB4 RID: 3764 RVA: 0x00048710 File Offset: 0x00046910
		private void Awake()
		{
			this.effectPrefabsList = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs/Effects/"));
			uint num = 0u;
			while ((ulong)num < (ulong)((long)this.effectPrefabsList.Count))
			{
				this.effectPrefabToIndexMap[this.effectPrefabsList[(int)num]] = num;
				num += 1u;
			}
		}

		// Token: 0x06000EB5 RID: 3765 RVA: 0x00048762 File Offset: 0x00046962
		[NetworkMessageHandler(msgType = 52, server = true)]
		private static void HandleEffectServer(NetworkMessage netMsg)
		{
			if (EffectManager.instance)
			{
				EffectManager.instance.HandleEffectServerInternal(netMsg);
			}
		}

		// Token: 0x06000EB6 RID: 3766 RVA: 0x0004877B File Offset: 0x0004697B
		[NetworkMessageHandler(msgType = 52, client = true)]
		private static void HandleEffectClient(NetworkMessage netMsg)
		{
			if (EffectManager.instance)
			{
				EffectManager.instance.HandleEffectClientInternal(netMsg);
			}
		}

		// Token: 0x06000EB7 RID: 3767 RVA: 0x00048794 File Offset: 0x00046994
		public void SpawnEffect(GameObject effectPrefab, EffectData effectData, bool transmit)
		{
			if (transmit)
			{
				this.TransmitEffect(effectPrefab, effectData, null);
				if (NetworkServer.active)
				{
					return;
				}
			}
			if (NetworkClient.active)
			{
				if (!VFXBudget.CanAffordSpawn(effectPrefab))
				{
					return;
				}
				EffectData effectData2 = effectData.Clone();
				EffectComponent component = UnityEngine.Object.Instantiate<GameObject>(effectPrefab, effectData2.origin, effectData2.rotation).GetComponent<EffectComponent>();
				if (component)
				{
					component.effectData = effectData2.Clone();
				}
			}
		}

		// Token: 0x06000EB8 RID: 3768 RVA: 0x000487F8 File Offset: 0x000469F8
		private void TransmitEffect(GameObject effectPrefab, EffectData effectData, NetworkConnection netOrigin = null)
		{
			uint effectPrefabIndex;
			if (!this.LookupEffectPrefabIndex(effectPrefab, out effectPrefabIndex))
			{
				return;
			}
			this.TransmitEffect(effectPrefabIndex, effectData, netOrigin);
		}

		// Token: 0x06000EB9 RID: 3769 RVA: 0x0004881C File Offset: 0x00046A1C
		private void TransmitEffect(uint effectPrefabIndex, EffectData effectData, NetworkConnection netOrigin = null)
		{
			EffectManager.outgoingEffectMessage.effectPrefabIndex = effectPrefabIndex;
			EffectData.Copy(effectData, EffectManager.outgoingEffectMessage.effectData);
			if (NetworkServer.active)
			{
				using (IEnumerator<NetworkConnection> enumerator = NetworkServer.connections.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						NetworkConnection networkConnection = enumerator.Current;
						if (networkConnection != null && networkConnection != netOrigin)
						{
							networkConnection.SendByChannel(52, EffectManager.outgoingEffectMessage, QosChannelIndex.effects.intVal);
						}
					}
					return;
				}
			}
			if (ClientScene.readyConnection != null)
			{
				ClientScene.readyConnection.SendByChannel(52, EffectManager.outgoingEffectMessage, QosChannelIndex.effects.intVal);
			}
		}

		// Token: 0x06000EBA RID: 3770 RVA: 0x000488C8 File Offset: 0x00046AC8
		private bool LookupEffectPrefabIndex(GameObject effectPrefab, out uint effectPrefabIndex)
		{
			if (!this.effectPrefabToIndexMap.TryGetValue(effectPrefab, out effectPrefabIndex))
			{
				Debug.LogErrorFormat("Attempted to find effect index for prefab \"{0}\" which is not in Resources/Prefabs/Effects.", new object[]
				{
					effectPrefab
				});
				return false;
			}
			return true;
		}

		// Token: 0x06000EBB RID: 3771 RVA: 0x000488F0 File Offset: 0x00046AF0
		private GameObject LookupEffectPrefab(uint effectPrefabIndex)
		{
			if ((ulong)effectPrefabIndex >= (ulong)((long)this.effectPrefabsList.Count))
			{
				Debug.LogErrorFormat("Attempted to find effect prefab for bad index #{0}.", new object[]
				{
					effectPrefabIndex
				});
				return null;
			}
			return this.effectPrefabsList[(int)effectPrefabIndex];
		}

		// Token: 0x06000EBC RID: 3772 RVA: 0x0004892C File Offset: 0x00046B2C
		private void HandleEffectClientInternal(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<EffectManager.EffectMessage>(EffectManager.incomingEffectMessage);
			GameObject gameObject = this.LookupEffectPrefab(EffectManager.incomingEffectMessage.effectPrefabIndex);
			if (gameObject == null)
			{
				return;
			}
			this.SpawnEffect(gameObject, EffectManager.incomingEffectMessage.effectData, false);
		}

		// Token: 0x06000EBD RID: 3773 RVA: 0x00048974 File Offset: 0x00046B74
		private void HandleEffectServerInternal(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<EffectManager.EffectMessage>(EffectManager.incomingEffectMessage);
			GameObject gameObject = this.LookupEffectPrefab(EffectManager.incomingEffectMessage.effectPrefabIndex);
			if (gameObject == null)
			{
				return;
			}
			this.TransmitEffect(gameObject, EffectManager.incomingEffectMessage.effectData, netMsg.conn);
		}

		// Token: 0x06000EBE RID: 3774 RVA: 0x000489C0 File Offset: 0x00046BC0
		public void SimpleMuzzleFlash(GameObject effectPrefab, GameObject obj, string muzzleName, bool transmit)
		{
			if (!obj)
			{
				return;
			}
			ModelLocator component = obj.GetComponent<ModelLocator>();
			if (component && component.modelTransform)
			{
				ChildLocator component2 = component.modelTransform.GetComponent<ChildLocator>();
				if (component2)
				{
					int childIndex = component2.FindChildIndex(muzzleName);
					Transform transform = component2.FindChild(childIndex);
					if (transform)
					{
						EffectData effectData = new EffectData
						{
							origin = transform.position
						};
						effectData.SetChildLocatorTransformReference(obj, childIndex);
						EffectManager.instance.SpawnEffect(effectPrefab, effectData, transmit);
					}
				}
			}
		}

		// Token: 0x06000EBF RID: 3775 RVA: 0x00048A49 File Offset: 0x00046C49
		public void SimpleImpactEffect(GameObject effectPrefab, Vector3 hitPos, Vector3 normal, bool transmit)
		{
			this.SpawnEffect(effectPrefab, new EffectData
			{
				origin = hitPos,
				rotation = ((normal == Vector3.zero) ? Quaternion.identity : Util.QuaternionSafeLookRotation(normal))
			}, transmit);
		}

		// Token: 0x06000EC0 RID: 3776 RVA: 0x00048A80 File Offset: 0x00046C80
		public void SimpleImpactEffect(GameObject effectPrefab, Vector3 hitPos, Vector3 normal, Color color, bool transmit)
		{
			this.SpawnEffect(effectPrefab, new EffectData
			{
				origin = hitPos,
				rotation = ((normal == Vector3.zero) ? Quaternion.identity : Util.QuaternionSafeLookRotation(normal)),
				color = color
			}, transmit);
		}

		// Token: 0x06000EC1 RID: 3777 RVA: 0x00048ACF File Offset: 0x00046CCF
		public void SimpleEffect(GameObject effectPrefab, Vector3 position, Quaternion rotation, bool transmit)
		{
			this.SpawnEffect(effectPrefab, new EffectData
			{
				origin = position,
				rotation = rotation
			}, transmit);
		}

		// Token: 0x040012CA RID: 4810
		private List<GameObject> effectPrefabsList;

		// Token: 0x040012CB RID: 4811
		private readonly Dictionary<GameObject, uint> effectPrefabToIndexMap = new Dictionary<GameObject, uint>();

		// Token: 0x040012CC RID: 4812
		private static readonly EffectManager.EffectMessage outgoingEffectMessage = new EffectManager.EffectMessage();

		// Token: 0x040012CD RID: 4813
		private static readonly EffectManager.EffectMessage incomingEffectMessage = new EffectManager.EffectMessage();

		// Token: 0x020002E0 RID: 736
		private class EffectMessage : MessageBase
		{
			// Token: 0x06000EC4 RID: 3780 RVA: 0x00048B16 File Offset: 0x00046D16
			public override void Serialize(NetworkWriter writer)
			{
				writer.WritePackedUInt32(this.effectPrefabIndex);
				writer.Write(this.effectData);
			}

			// Token: 0x06000EC5 RID: 3781 RVA: 0x00048B30 File Offset: 0x00046D30
			public override void Deserialize(NetworkReader reader)
			{
				this.effectPrefabIndex = reader.ReadPackedUInt32();
				reader.ReadEffectData(this.effectData);
			}

			// Token: 0x040012CE RID: 4814
			public uint effectPrefabIndex;

			// Token: 0x040012CF RID: 4815
			public readonly EffectData effectData = new EffectData();
		}
	}
}
