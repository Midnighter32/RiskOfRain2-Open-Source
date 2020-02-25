using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001F3 RID: 499
	[DisallowMultipleComponent]
	public class EntityLocator : MonoBehaviour
	{
		// Token: 0x06000A6A RID: 2666 RVA: 0x0002DB1C File Offset: 0x0002BD1C
		public static GameObject GetEntity(GameObject gameObject)
		{
			if (gameObject == null)
			{
				return null;
			}
			EntityLocator component = gameObject.GetComponent<EntityLocator>();
			if (!component)
			{
				return null;
			}
			return component.entity;
		}

		// Token: 0x04000ACF RID: 2767
		[Tooltip("The root gameobject of the entity.")]
		public GameObject entity;
	}
}
