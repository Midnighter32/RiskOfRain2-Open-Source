using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RoR2
{
	// Token: 0x02000138 RID: 312
	public static class FadeToBlackManager
	{
		// Token: 0x06000598 RID: 1432 RVA: 0x00017300 File Offset: 0x00015500
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Init()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/ScreenTintCanvas"), RoR2Application.instance.mainCanvas.transform);
			FadeToBlackManager.alpha = 0f;
			FadeToBlackManager.image = gameObject.transform.GetChild(0).GetComponent<Image>();
			FadeToBlackManager.UpdateImageAlpha();
			RoR2Application.onUpdate += FadeToBlackManager.Update;
			SceneManager.sceneUnloaded += FadeToBlackManager.OnSceneUnloaded;
		}

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x06000599 RID: 1433 RVA: 0x00017371 File Offset: 0x00015571
		public static bool fullyFaded
		{
			get
			{
				return FadeToBlackManager.alpha == 2f;
			}
		}

		// Token: 0x0600059A RID: 1434 RVA: 0x0001737F File Offset: 0x0001557F
		public static void OnSceneUnloaded(Scene scene)
		{
			FadeToBlackManager.ForceFullBlack();
		}

		// Token: 0x0600059B RID: 1435 RVA: 0x00017386 File Offset: 0x00015586
		public static void ForceFullBlack()
		{
			FadeToBlackManager.alpha = 2f;
		}

		// Token: 0x0600059C RID: 1436 RVA: 0x00017394 File Offset: 0x00015594
		private static void Update()
		{
			float target = 2f;
			float num = 4f;
			if (FadeToBlackManager.fadeCount <= 0)
			{
				target = 0f;
				num *= 0.25f;
			}
			FadeToBlackManager.alpha = Mathf.MoveTowards(FadeToBlackManager.alpha, target, Time.unscaledDeltaTime * num);
			FadeToBlackManager.UpdateImageAlpha();
		}

		// Token: 0x0600059D RID: 1437 RVA: 0x000173E0 File Offset: 0x000155E0
		private static void UpdateImageAlpha()
		{
			Color color = FadeToBlackManager.image.color;
			color.a = FadeToBlackManager.alpha;
			FadeToBlackManager.image.color = color;
		}

		// Token: 0x04000601 RID: 1537
		private static Image image;

		// Token: 0x04000602 RID: 1538
		public static int fadeCount;

		// Token: 0x04000603 RID: 1539
		private static float alpha;

		// Token: 0x04000604 RID: 1540
		private const float fadeDuration = 0.25f;

		// Token: 0x04000605 RID: 1541
		private const float inversefadeDuration = 4f;
	}
}
