using System;
using EntityStates.SurvivorPod;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.RoboCratePod
{
	// Token: 0x02000779 RID: 1913
	public class Descent : Descent
	{
		// Token: 0x06002C06 RID: 11270 RVA: 0x000BA0C2 File Offset: 0x000B82C2
		protected override void TransitionIntoNextState()
		{
			base.TransitionIntoNextState();
			EffectManager.SimpleMuzzleFlash(Descent.effectPrefab, base.gameObject, "Base", true);
		}

		// Token: 0x06002C07 RID: 11271 RVA: 0x000BA0E0 File Offset: 0x000B82E0
		public override void OnExit()
		{
			VehicleSeat component = base.GetComponent<VehicleSeat>();
			if (component)
			{
				component.EjectPassenger();
			}
			if (NetworkServer.active)
			{
				EntityState.Destroy(base.gameObject);
			}
			base.OnExit();
		}

		// Token: 0x04002824 RID: 10276
		public static GameObject effectPrefab;
	}
}
