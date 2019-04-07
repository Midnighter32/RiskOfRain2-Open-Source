using System;
using RoR2;
using UnityEngine;

namespace EntityStates.GreaterWispMonster
{
	// Token: 0x020000CE RID: 206
	public class DeathState : GenericCharacterDeath
	{
		// Token: 0x06000405 RID: 1029 RVA: 0x0001089C File Offset: 0x0000EA9C
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
					if (DeathState.initialEffect)
					{
						UnityEngine.Object.Instantiate<GameObject>(DeathState.initialEffect, transform.position, transform.rotation);
					}
				}
			}
		}

		// Token: 0x06000406 RID: 1030 RVA: 0x00010920 File Offset: 0x0000EB20
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= DeathState.duration)
			{
				if (DeathState.deathEffect)
				{
					EffectManager.instance.SpawnEffect(DeathState.deathEffect, new EffectData
					{
						origin = base.transform.position
					}, true);
				}
				EntityState.Destroy(base.gameObject);
			}
		}

		// Token: 0x040003C4 RID: 964
		public static GameObject initialEffect;

		// Token: 0x040003C5 RID: 965
		public static GameObject deathEffect;

		// Token: 0x040003C6 RID: 966
		private static float duration = 2f;
	}
}
