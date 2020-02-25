using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Engi.SpiderMine
{
	// Token: 0x0200086A RID: 2154
	public class BaseSpiderMineState : BaseState
	{
		// Token: 0x17000445 RID: 1093
		// (get) Token: 0x06003093 RID: 12435 RVA: 0x000D174A File Offset: 0x000CF94A
		// (set) Token: 0x06003094 RID: 12436 RVA: 0x000D1752 File Offset: 0x000CF952
		private protected ProjectileStickOnImpact projectileStickOnImpact { protected get; private set; }

		// Token: 0x17000446 RID: 1094
		// (get) Token: 0x06003095 RID: 12437 RVA: 0x000D175B File Offset: 0x000CF95B
		// (set) Token: 0x06003096 RID: 12438 RVA: 0x000D1763 File Offset: 0x000CF963
		private protected ProjectileTargetComponent projectileTargetComponent { protected get; private set; }

		// Token: 0x06003097 RID: 12439 RVA: 0x000D176C File Offset: 0x000CF96C
		public override void OnEnter()
		{
			base.OnEnter();
			this.projectileStickOnImpact = base.GetComponent<ProjectileStickOnImpact>();
			this.projectileTargetComponent = base.GetComponent<ProjectileTargetComponent>();
			if (this.projectileStickOnImpact.enabled != this.shouldStick)
			{
				this.projectileStickOnImpact.enabled = this.shouldStick;
			}
			Transform transform = base.FindModelChild(this.childLocatorStringToEnable);
			if (transform)
			{
				transform.gameObject.SetActive(true);
			}
			Util.PlaySound(this.enterSoundString, base.gameObject);
		}

		// Token: 0x17000447 RID: 1095
		// (get) Token: 0x06003098 RID: 12440 RVA: 0x0000AC89 File Offset: 0x00008E89
		protected virtual bool shouldStick
		{
			get
			{
				return false;
			}
		}

		// Token: 0x04002EF5 RID: 12021
		[SerializeField]
		public string enterSoundString;

		// Token: 0x04002EF6 RID: 12022
		[SerializeField]
		public string childLocatorStringToEnable;
	}
}
