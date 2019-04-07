using System;
using UnityEngine;

namespace EntityStates.GolemMonster
{
	// Token: 0x02000178 RID: 376
	public class DeathState : GenericCharacterDeath
	{
		// Token: 0x06000743 RID: 1859 RVA: 0x000236AC File Offset: 0x000218AC
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

		// Token: 0x04000925 RID: 2341
		public static GameObject initialDeathExplosionPrefab;
	}
}
