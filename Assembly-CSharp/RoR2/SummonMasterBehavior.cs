using System;
using RoR2.CharacterAI;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003F0 RID: 1008
	public class SummonMasterBehavior : NetworkBehaviour
	{
		// Token: 0x060015F7 RID: 5623 RVA: 0x00037FB6 File Offset: 0x000361B6
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x060015F8 RID: 5624 RVA: 0x0006968C File Offset: 0x0006788C
		[Server]
		public void OpenSummon(Interactor activator)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.SummonMasterBehavior::OpenSummon(RoR2.Interactor)' called on client");
				return;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.masterPrefab, base.transform.position, base.transform.rotation);
			NetworkServer.Spawn(gameObject);
			CharacterMaster component = gameObject.GetComponent<CharacterMaster>();
			component.SpawnBody(component.bodyPrefab, base.transform.position + Vector3.up * 0.8f, base.transform.rotation);
			AIOwnership component2 = gameObject.GetComponent<AIOwnership>();
			if (component2)
			{
				CharacterBody component3 = activator.GetComponent<CharacterBody>();
				if (component3)
				{
					CharacterMaster master = component3.master;
					if (master)
					{
						component2.ownerMaster = master;
					}
				}
			}
			BaseAI component4 = gameObject.GetComponent<BaseAI>();
			if (component4)
			{
				component4.leader.gameObject = activator.gameObject;
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x060015FA RID: 5626 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x060015FB RID: 5627 RVA: 0x00069770 File Offset: 0x00067970
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x060015FC RID: 5628 RVA: 0x00004507 File Offset: 0x00002707
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x0400196A RID: 6506
		[Tooltip("The master to spawn")]
		public GameObject masterPrefab;
	}
}
