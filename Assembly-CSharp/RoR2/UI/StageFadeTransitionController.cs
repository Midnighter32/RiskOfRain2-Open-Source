using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000641 RID: 1601
	[RequireComponent(typeof(Image))]
	public class StageFadeTransitionController : MonoBehaviour
	{
		// Token: 0x060023D6 RID: 9174 RVA: 0x000A85A8 File Offset: 0x000A67A8
		private void Awake()
		{
			this.fadeImage = base.GetComponent<Image>();
			Color color = this.fadeImage.color;
			color.a = 1f;
			this.fadeImage.color = color;
		}

		// Token: 0x060023D7 RID: 9175 RVA: 0x000A85E8 File Offset: 0x000A67E8
		private void Start()
		{
			Color color = this.fadeImage.color;
			color.a = 1f;
			this.fadeImage.color = color;
			this.fadeImage.CrossFadeColor(Color.black, 0.5f, false, true);
			this.startEngineTime = Time.time;
		}

		// Token: 0x060023D8 RID: 9176 RVA: 0x000A863C File Offset: 0x000A683C
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

		// Token: 0x040026CB RID: 9931
		private Image fadeImage;

		// Token: 0x040026CC RID: 9932
		private float startEngineTime;

		// Token: 0x040026CD RID: 9933
		private const float transitionDuration = 0.5f;
	}
}
