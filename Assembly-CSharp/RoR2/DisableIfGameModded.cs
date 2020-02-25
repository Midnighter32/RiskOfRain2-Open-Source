using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001E9 RID: 489
	public class DisableIfGameModded : MonoBehaviour
	{
		// Token: 0x06000A36 RID: 2614 RVA: 0x0002C95E File Offset: 0x0002AB5E
		public void OnEnable()
		{
			if (RoR2Application.isModded)
			{
				base.gameObject.SetActive(false);
			}
		}
	}
}
