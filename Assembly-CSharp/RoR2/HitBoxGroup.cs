using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000311 RID: 785
	public class HitBoxGroup : MonoBehaviour
	{
		// Token: 0x0400143D RID: 5181
		[Tooltip("The name of this hitbox group.")]
		public string groupName;

		// Token: 0x0400143E RID: 5182
		[Tooltip("The hitbox objects in this group.")]
		public HitBox[] hitBoxes;
	}
}
