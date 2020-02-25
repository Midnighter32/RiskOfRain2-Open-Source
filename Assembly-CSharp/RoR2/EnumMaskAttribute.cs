using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200012F RID: 303
	public class EnumMaskAttribute : PropertyAttribute
	{
		// Token: 0x06000575 RID: 1397 RVA: 0x000161AD File Offset: 0x000143AD
		public EnumMaskAttribute(Type enumType)
		{
			this.enumType = enumType;
		}

		// Token: 0x040005B5 RID: 1461
		public Type enumType;
	}
}
