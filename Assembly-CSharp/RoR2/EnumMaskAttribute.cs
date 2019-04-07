using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000241 RID: 577
	public class EnumMaskAttribute : PropertyAttribute
	{
		// Token: 0x06000AE0 RID: 2784 RVA: 0x00035AB8 File Offset: 0x00033CB8
		public EnumMaskAttribute(Type enumType)
		{
			this.enumType = enumType;
		}

		// Token: 0x04000EAA RID: 3754
		public Type enumType;
	}
}
