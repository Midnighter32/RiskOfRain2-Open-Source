using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.GreaterWispMonster
{
	// Token: 0x0200072D RID: 1837
	public class DeathState : GenericCharacterDeath
	{
		// Token: 0x06002AAD RID: 10925 RVA: 0x000B38C4 File Offset: 0x000B1AC4
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.modelLocator)
			{
				ChildLocator component = base.modelLocator.modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild("Mask");
					transform.gameObject.SetActive(true);
					transform.GetComponent<AnimateShaderAlpha>().timeMax = DeathState.duration;
					if (this.initialEffect)
					{
						this.initialEffectInstance = UnityEngine.Object.Instantiate<GameObject>(this.initialEffect, transform.position, transform.rotation, transform);
					}
				}
			}
		}

		// Token: 0x06002AAE RID: 10926 RVA: 0x000B3950 File Offset: 0x000B1B50
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= DeathState.duration && NetworkServer.active)
			{
				if (this.deathEffect)
				{
					EffectManager.SpawnEffect(this.deathEffect, new EffectData
					{
						origin = base.transform.position
					}, true);
				}
				EntityState.Destroy(base.gameObject);
			}
		}

		// Token: 0x06002AAF RID: 10927 RVA: 0x000B39B1 File Offset: 0x000B1BB1
		public override void OnExit()
		{
			base.OnExit();
			if (this.initialEffectInstance)
			{
				EntityState.Destroy(this.initialEffectInstance);
			}
		}

		// Token: 0x04002689 RID: 9865
		[SerializeField]
		public GameObject initialEffect;

		// Token: 0x0400268A RID: 9866
		[SerializeField]
		public GameObject deathEffect;

		// Token: 0x0400268B RID: 9867
		private static float duration = 2f;

		// Token: 0x0400268C RID: 9868
		private GameObject initialEffectInstance;
	}
}
