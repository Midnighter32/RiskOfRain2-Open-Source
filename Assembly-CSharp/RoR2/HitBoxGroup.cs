using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000231 RID: 561
	public class HitBoxGroup : MonoBehaviour
	{
		// Token: 0x04000C7B RID: 3195
		[Tooltip("The name of this hitbox group.")]
		public string groupName;

		// Token: 0x04000C7C RID: 3196
		[Tooltip("The hitbox objects in this group.")]
		public HitBox[] hitBoxes;
	}
}
