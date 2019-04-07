using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2
{
	// Token: 0x02000247 RID: 583
	public static class FadeToBlackManager
	{
		// Token: 0x06000AF2 RID: 2802 RVA: 0x00036818 File Offset: 0x00034A18
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/ScreenTintCanvas"));
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			FadeToBlackManager.alpha = 0f;
			FadeToBlackManager.image = gameObject.transform.GetChild(0).GetComponent<Image>();
			FadeToBlackManager.UpdateImageAlpha();
			RoR2Application.onUpdate += FadeToBlackManager.Update;
		}

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06000AF3 RID: 2803 RVA: 0x0003686F File Offset: 0x00034A6F
		public static bool fullyFaded
		{
			get
			{
				return FadeToBlackManager.alpha == 2f;
			}
		}

		// Token: 0x06000AF4 RID: 2804 RVA: 0x0003687D File Offset: 0x00034A7D
		private static void Update()
		{
			FadeToBlackManager.alpha = Mathf.MoveTowards(FadeToBlackManager.alpha, (FadeToBlackManager.fadeCount > 0) ? 2f : 0f, Time.deltaTime * 4f);
			FadeToBlackManager.UpdateImageAlpha();
		}

		// Token: 0x06000AF5 RID: 2805 RVA: 0x000368B4 File Offset: 0x00034AB4
		private static void UpdateImageAlpha()
		{
			Color color = FadeToBlackManager.image.color;
			color.a = FadeToBlackManager.alpha;
			FadeToBlackManager.image.color = color;
		}

		// Token: 0x04000EE4 RID: 3812
		private static Image image;

		// Token: 0x04000EE5 RID: 3813
		public static int fadeCount;

		// Token: 0x04000EE6 RID: 3814
		private static float alpha;

		// Token: 0x04000EE7 RID: 3815
		private const float fadeDuration = 0.25f;

		// Token: 0x04000EE8 RID: 3816
		private const float inversefadeDuration = 4f;
	}
}
