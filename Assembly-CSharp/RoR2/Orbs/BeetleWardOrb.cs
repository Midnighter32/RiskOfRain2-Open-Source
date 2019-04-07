using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Orbs
{
	// Token: 0x0200050D RID: 1293
	public class BeetleWardOrb : Orb
	{
		// Token: 0x06001D33 RID: 7475 RVA: 0x00087FD8 File Offset: 0x000861D8
		public override void Begin()
		{
			base.duration = base.distanceToTarget / this.speed;
			EffectData effectData = new EffectData
			{
				origin = this.origin,
				genericFloat = base.duration
			};
			effectData.SetHurtBoxReference(this.target);
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/BeetleWardOrbEffect"), effectData, true);
		}

		// Token: 0x06001D34 RID: 7476 RVA: 0x00088038 File Offset: 0x00086238
		public override void OnArrival()
		{
			if (this.target)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/BeetleWard"), this.target.transform.position, Quaternion.identity);
				gameObject.GetComponent<TeamFilter>().teamIndex = this.target.teamIndex;
				NetworkServer.Spawn(gameObject);
			}
		}

		// Token: 0x04001F58 RID: 8024
		public float speed;
	}
}
