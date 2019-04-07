using System;

namespace RoR2
{
	// Token: 0x02000330 RID: 816
	public interface IInteractable
	{
		// Token: 0x060010BF RID: 4287
		string GetContextString(Interactor activator);

		// Token: 0x060010C0 RID: 4288
		Interactability GetInteractability(Interactor activator);

		// Token: 0x060010C1 RID: 4289
		void OnInteractionBegin(Interactor activator);

		// Token: 0x060010C2 RID: 4290
		bool ShouldIgnoreSpherecastForInteractibility(Interactor activator);
	}
}
