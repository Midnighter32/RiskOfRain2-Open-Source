using System;

namespace RoR2
{
	// Token: 0x02000252 RID: 594
	public interface IInteractable
	{
		// Token: 0x06000D0B RID: 3339
		string GetContextString(Interactor activator);

		// Token: 0x06000D0C RID: 3340
		Interactability GetInteractability(Interactor activator);

		// Token: 0x06000D0D RID: 3341
		void OnInteractionBegin(Interactor activator);

		// Token: 0x06000D0E RID: 3342
		bool ShouldIgnoreSpherecastForInteractibility(Interactor activator);

		// Token: 0x06000D0F RID: 3343
		bool ShouldShowOnScanner();
	}
}
