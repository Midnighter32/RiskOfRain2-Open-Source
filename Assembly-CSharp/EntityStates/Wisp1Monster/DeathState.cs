using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Wisp1Monster
{
	// Token: 0x02000723 RID: 1827
	public class DeathState : GenericCharacterDeath
	{
		// Token: 0x06002A87 RID: 10887 RVA: 0x000B3040 File Offset: 0x000B1240
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.modelLocator)
			{
				if (base.modelLocator.modelBaseTransform)
				{
					EntityState.Destroy(base.modelLocator.modelBaseTransform.gameObject);
				}
				if (base.modelLocator.modelTransform)
				{
					EntityState.Destroy(base.modelLocator.modelTransform.gameObject);
				}
			}
			if (NetworkServer.active)
			{
				EffectManager.SimpleEffect(DeathState.initialExplosion, base.transform.position, base.transform.rotation, true);
				EntityState.Destroy(base.gameObject);
			}
		}

		// Token: 0x04002666 RID: 9830
		public static GameObject initialExplosion;
	}
}
