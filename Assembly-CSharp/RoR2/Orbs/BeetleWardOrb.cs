using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Orbs
{
	// Token: 0x020004C5 RID: 1221
	public class BeetleWardOrb : Orb
	{
		// Token: 0x06001D61 RID: 7521 RVA: 0x0007D1C4 File Offset: 0x0007B3C4
		public override void Begin()
		{
			base.duration = base.distanceToTarget / this.speed;
			EffectData effectData = new EffectData
			{
				origin = this.origin,
				genericFloat = base.duration
			};
			effectData.SetHurtBoxReference(this.target);
			EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/BeetleWardOrbEffect"), effectData, true);
		}

		// Token: 0x06001D62 RID: 7522 RVA: 0x0007D220 File Offset: 0x0007B420
		public override void OnArrival()
		{
			if (this.target)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/BeetleWard"), this.target.transform.position, Quaternion.identity);
				gameObject.GetComponent<TeamFilter>().teamIndex = this.target.teamIndex;
				NetworkServer.Spawn(gameObject);
			}
		}

		// Token: 0x04001A65 RID: 6757
		public float speed;
	}
}
