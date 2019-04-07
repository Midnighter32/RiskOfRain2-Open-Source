using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002E1 RID: 737
	public class EnableOnMecanimFloat : MonoBehaviour
	{
		// Token: 0x06000EC7 RID: 3783 RVA: 0x00048B60 File Offset: 0x00046D60
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

		// Token: 0x040012D0 RID: 4816
		public Animator animator;

		// Token: 0x040012D1 RID: 4817
		[Tooltip("The name of the mecanim variable to compare against")]
		public string animatorString;

		// Token: 0x040012D2 RID: 4818
		[Tooltip("The minimum value at which the objects are enabled")]
		public float minFloatValue;

		// Token: 0x040012D3 RID: 4819
		[Tooltip("The maximum value at which the objects are enabled")]
		public float maxFloatValue;

		// Token: 0x040012D4 RID: 4820
		public GameObject[] objectsToEnable;

		// Token: 0x040012D5 RID: 4821
		public GameObject[] objectsToDisable;

		// Token: 0x040012D6 RID: 4822
		private bool wasWithinRange;
	}
}
