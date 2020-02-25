using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Fauna
{
	// Token: 0x02000866 RID: 2150
	public class VultureEggDeathState : BirdsharkDeathState
	{
		// Token: 0x0600307D RID: 12413 RVA: 0x000D1074 File Offset: 0x000CF274
		public override void OnEnter()
		{
			base.OnEnter();
			if (NetworkServer.active)
			{
				for (int i = 0; i < VultureEggDeathState.healPackCount; i++)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/HealPack"), base.transform.position, UnityEngine.Random.rotation);
					gameObject.GetComponent<TeamFilter>().teamIndex = TeamIndex.Player;
					gameObject.GetComponentInChildren<HealthPickup>().fractionalHealing = VultureEggDeathState.fractionalHealing;
					gameObject.transform.localScale = new Vector3(VultureEggDeathState.scale, VultureEggDeathState.scale, VultureEggDeathState.scale);
					gameObject.GetComponent<Rigidbody>().AddForce(UnityEngine.Random.insideUnitSphere * VultureEggDeathState.healPackMaxVelocity, ForceMode.VelocityChange);
					NetworkServer.Spawn(gameObject);
				}
			}
		}

		// Token: 0x04002ECD RID: 11981
		public static int healPackCount;

		// Token: 0x04002ECE RID: 11982
		public static float healPackMaxVelocity;

		// Token: 0x04002ECF RID: 11983
		public static float fractionalHealing;

		// Token: 0x04002ED0 RID: 11984
		public static float scale;
	}
}
