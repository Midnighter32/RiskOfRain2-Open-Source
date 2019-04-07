using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002E5 RID: 741
	[DisallowMultipleComponent]
	public class EntityLocator : MonoBehaviour
	{
		// Token: 0x06000ED1 RID: 3793 RVA: 0x00049108 File Offset: 0x00047308
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

		// Token: 0x040012F1 RID: 4849
		[Tooltip("The root gameobject of the entity.")]
		public GameObject entity;
	}
}
