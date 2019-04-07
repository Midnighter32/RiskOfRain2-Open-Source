using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Engi.EngiBubbleShield
{
	// Token: 0x02000190 RID: 400
	public class Deployed : EntityState
	{
		// Token: 0x060007B2 RID: 1970 RVA: 0x00026326 File Offset: 0x00024526
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(Deployed.initialSoundString, base.gameObject);
		}

		// Token: 0x060007B3 RID: 1971 RVA: 0x00026340 File Offset: 0x00024540
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (!this.hasDeployed && base.fixedAge >= Deployed.delayToDeploy)
			{
				ChildLocator component = base.GetComponent<ChildLocator>();
				if (component)
				{
					component.FindChild(Deployed.childLocatorString).gameObject.SetActive(true);
					this.hasDeployed = true;
				}
			}
			if (base.fixedAge >= Deployed.lifetime && NetworkServer.active)
			{
				EntityState.Destroy(base.gameObject);
			}
		}

		// Token: 0x060007B4 RID: 1972 RVA: 0x000263B4 File Offset: 0x000245B4
		public override void OnExit()
		{
			base.OnExit();
			EffectManager.instance.SpawnEffect(Deployed.destroyEffectPrefab, new EffectData
			{
				origin = base.transform.position,
				scale = Deployed.destroyEffectRadius
			}, false);
			Util.PlaySound(Deployed.destroySoundString, base.gameObject);
		}

		// Token: 0x040009F8 RID: 2552
		public static string childLocatorString;

		// Token: 0x040009F9 RID: 2553
		public static string initialSoundString;

		// Token: 0x040009FA RID: 2554
		public static string destroySoundString;

		// Token: 0x040009FB RID: 2555
		public static float delayToDeploy;

		// Token: 0x040009FC RID: 2556
		public static float lifetime;

		// Token: 0x040009FD RID: 2557
		public static GameObject destroyEffectPrefab;

		// Token: 0x040009FE RID: 2558
		public static float destroyEffectRadius;

		// Token: 0x040009FF RID: 2559
		private bool hasDeployed;
	}
}
