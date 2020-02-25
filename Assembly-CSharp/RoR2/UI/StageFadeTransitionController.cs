using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000636 RID: 1590
	[RequireComponent(typeof(Image))]
	public class StageFadeTransitionController : MonoBehaviour
	{
		// Token: 0x06002575 RID: 9589 RVA: 0x000A2FB8 File Offset: 0x000A11B8
		private void Awake()
		{
			this.fadeImage = base.GetComponent<Image>();
			Color color = this.fadeImage.color;
			color.a = 1f;
			this.fadeImage.color = color;
		}

		// Token: 0x06002576 RID: 9590 RVA: 0x000A2FF8 File Offset: 0x000A11F8
		private void Start()
		{
			Color color = this.fadeImage.color;
			color.a = 1f;
			this.fadeImage.color = color;
			this.fadeImage.CrossFadeColor(Color.black, 0.5f, false, true);
			this.startEngineTime = Time.time;
		}

		// Token: 0x06002577 RID: 9591 RVA: 0x000A304C File Offset: 0x000A124C
		private void Update()
		{
			if (Stage.instance)
			{
				float stageAdvanceTime = Stage.instance.stageAdvanceTime;
				float num = Time.time - this.startEngineTime;
				float a = 0f;
				float b = 0f;
				if (num < 0.5f)
				{
					a = 1f - Mathf.Clamp01((Time.time - this.startEngineTime) / 0.5f);
				}
				if (!float.IsInfinity(stageAdvanceTime))
				{
					float num2 = Stage.instance.stageAdvanceTime - 0.25f - Run.instance.fixedTime;
					b = 1f - Mathf.Clamp01(num2 / 0.5f);
				}
				Color color = this.fadeImage.color;
				color.a = Mathf.Max(a, b);
				this.fadeImage.color = color;
			}
		}

		// Token: 0x0400232E RID: 9006
		private Image fadeImage;

		// Token: 0x0400232F RID: 9007
		private float startEngineTime;

		// Token: 0x04002330 RID: 9008
		private const float transitionDuration = 0.5f;
	}
}
