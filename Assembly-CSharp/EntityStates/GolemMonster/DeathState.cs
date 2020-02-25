using System;
using UnityEngine;

namespace EntityStates.GolemMonster
{
	// Token: 0x0200085E RID: 2142
	public class DeathState : GenericCharacterDeath
	{
		// Token: 0x0600305E RID: 12382 RVA: 0x000D04AC File Offset: 0x000CE6AC
		public override void OnEnter()
		{
			base.OnEnter();
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild("Head");
					if (transform && DeathState.initialDeathExplosionPrefab)
					{
						UnityEngine.Object.Instantiate<GameObject>(DeathState.initialDeathExplosionPrefab, transform.position, Quaternion.identity).transform.parent = transform;
					}
				}
			}
		}

		// Token: 0x04002E92 RID: 11922
		public static GameObject initialDeathExplosionPrefab;
	}
}
