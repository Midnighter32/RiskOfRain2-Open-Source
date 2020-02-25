using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001F1 RID: 497
	public class EnableOnMecanimFloat : MonoBehaviour
	{
		// Token: 0x06000A64 RID: 2660 RVA: 0x0002D9F8 File Offset: 0x0002BBF8
		private void Update()
		{
			if (this.animator)
			{
				float @float = this.animator.GetFloat(this.animatorString);
				bool flag = Mathf.Clamp(@float, this.minFloatValue, this.maxFloatValue) == @float;
				if (flag != this.wasWithinRange)
				{
					GameObject[] array = this.objectsToEnable;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].SetActive(flag);
					}
					array = this.objectsToDisable;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].SetActive(!flag);
					}
					this.wasWithinRange = flag;
				}
			}
		}

		// Token: 0x04000AC5 RID: 2757
		public Animator animator;

		// Token: 0x04000AC6 RID: 2758
		[Tooltip("The name of the mecanim variable to compare against")]
		public string animatorString;

		// Token: 0x04000AC7 RID: 2759
		[Tooltip("The minimum value at which the objects are enabled")]
		public float minFloatValue;

		// Token: 0x04000AC8 RID: 2760
		[Tooltip("The maximum value at which the objects are enabled")]
		public float maxFloatValue;

		// Token: 0x04000AC9 RID: 2761
		public GameObject[] objectsToEnable;

		// Token: 0x04000ACA RID: 2762
		public GameObject[] objectsToDisable;

		// Token: 0x04000ACB RID: 2763
		private bool wasWithinRange;
	}
}
