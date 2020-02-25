using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Engi.EngiBubbleShield
{
	// Token: 0x02000893 RID: 2195
	public class Deployed : EntityState
	{
		// Token: 0x06003143 RID: 12611 RVA: 0x000D41C2 File Offset: 0x000D23C2
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(Deployed.initialSoundString, base.gameObject);
		}

		// Token: 0x06003144 RID: 12612 RVA: 0x000D41DC File Offset: 0x000D23DC
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

		// Token: 0x06003145 RID: 12613 RVA: 0x000D4250 File Offset: 0x000D2450
		public override void OnExit()
		{
			base.OnExit();
			EffectManager.SpawnEffect(this.destroyEffectPrefab, new EffectData
			{
				origin = base.transform.position,
				rotation = base.transform.rotation,
				scale = this.destroyEffectRadius
			}, false);
			Util.PlaySound(Deployed.destroySoundString, base.gameObject);
		}

		// Token: 0x04002F94 RID: 12180
		public static string childLocatorString;

		// Token: 0x04002F95 RID: 12181
		public static string initialSoundString;

		// Token: 0x04002F96 RID: 12182
		public static string destroySoundString;

		// Token: 0x04002F97 RID: 12183
		public static float delayToDeploy;

		// Token: 0x04002F98 RID: 12184
		public static float lifetime;

		// Token: 0x04002F99 RID: 12185
		[SerializeField]
		public GameObject destroyEffectPrefab;

		// Token: 0x04002F9A RID: 12186
		[SerializeField]
		public float destroyEffectRadius;

		// Token: 0x04002F9B RID: 12187
		private bool hasDeployed;
	}
}
