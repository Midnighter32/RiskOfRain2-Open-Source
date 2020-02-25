using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200025E RID: 606
	[RequireComponent(typeof(Interactor))]
	[RequireComponent(typeof(InputBankTest))]
	public class InteractionDriver : MonoBehaviour, ILifeBehavior
	{
		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x06000D38 RID: 3384 RVA: 0x0003B6BA File Offset: 0x000398BA
		// (set) Token: 0x06000D39 RID: 3385 RVA: 0x0003B6C2 File Offset: 0x000398C2
		public Interactor interactor { get; private set; }

		// Token: 0x06000D3A RID: 3386 RVA: 0x0003B6CB File Offset: 0x000398CB
		private void Awake()
		{
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
			this.interactor = base.GetComponent<Interactor>();
			this.inputBank = base.GetComponent<InputBankTest>();
		}

		// Token: 0x06000D3B RID: 3387 RVA: 0x0003B6F4 File Offset: 0x000398F4
		private void FixedUpdate()
		{
			if (this.networkIdentity.hasAuthority)
			{
				this.interactableCooldown -= Time.fixedDeltaTime;
				this.inputReceived = (this.inputBank.interact.justPressed || (this.inputBank.interact.down && this.interactableCooldown <= 0f));
				if (this.inputBank.interact.justReleased)
				{
					this.inputReceived = false;
					this.interactableCooldown = 0f;
				}
			}
			if (this.inputReceived)
			{
				GameObject gameObject = this.FindBestInteractableObject();
				if (gameObject)
				{
					this.interactor.AttemptInteraction(gameObject);
					this.interactableCooldown = 0.25f;
				}
			}
		}

		// Token: 0x06000D3C RID: 3388 RVA: 0x0003B7B4 File Offset: 0x000399B4
		public GameObject FindBestInteractableObject()
		{
			if (this.interactableOverride)
			{
				return this.interactableOverride;
			}
			float num = 0f;
			Ray originalAimRay = new Ray(this.inputBank.aimOrigin, this.inputBank.aimDirection);
			Ray raycastRay = CameraRigController.ModifyAimRayIfApplicable(originalAimRay, base.gameObject, out num);
			return this.interactor.FindBestInteractableObject(raycastRay, this.interactor.maxInteractionDistance + num, originalAimRay.origin, this.interactor.maxInteractionDistance);
		}

		// Token: 0x06000D3D RID: 3389 RVA: 0x0003B832 File Offset: 0x00039A32
		static InteractionDriver()
		{
			OutlineHighlight.onPreRenderOutlineHighlight = (Action<OutlineHighlight>)Delegate.Combine(OutlineHighlight.onPreRenderOutlineHighlight, new Action<OutlineHighlight>(InteractionDriver.OnPreRenderOutlineHighlight));
		}

		// Token: 0x06000D3E RID: 3390 RVA: 0x0003B854 File Offset: 0x00039A54
		private static void OnPreRenderOutlineHighlight(OutlineHighlight outlineHighlight)
		{
			if (!outlineHighlight.sceneCamera)
			{
				return;
			}
			if (!outlineHighlight.sceneCamera.cameraRigController)
			{
				return;
			}
			GameObject target = outlineHighlight.sceneCamera.cameraRigController.target;
			if (!target)
			{
				return;
			}
			InteractionDriver component = target.GetComponent<InteractionDriver>();
			if (!component)
			{
				return;
			}
			GameObject gameObject = component.FindBestInteractableObject();
			if (!gameObject)
			{
				return;
			}
			IInteractable component2 = gameObject.GetComponent<IInteractable>();
			Highlight component3 = gameObject.GetComponent<Highlight>();
			if (!component3)
			{
				return;
			}
			Color a = component3.GetColor();
			if (component2 != null && ((MonoBehaviour)component2).isActiveAndEnabled && component2.GetInteractability(component.interactor) == Interactability.ConditionsNotMet)
			{
				a = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Unaffordable);
			}
			outlineHighlight.highlightQueue.Enqueue(new OutlineHighlight.HighlightInfo
			{
				renderer = component3.targetRenderer,
				color = a * component3.strength
			});
		}

		// Token: 0x06000D3F RID: 3391 RVA: 0x00022B74 File Offset: 0x00020D74
		public void OnDeathStart()
		{
			base.enabled = false;
		}

		// Token: 0x04000D76 RID: 3446
		public bool highlightInteractor;

		// Token: 0x04000D77 RID: 3447
		private bool inputReceived;

		// Token: 0x04000D78 RID: 3448
		private NetworkIdentity networkIdentity;

		// Token: 0x04000D7A RID: 3450
		private InputBankTest inputBank;

		// Token: 0x04000D7B RID: 3451
		[NonSerialized]
		public GameObject interactableOverride;

		// Token: 0x04000D7C RID: 3452
		private const float interactableCooldownDuration = 0.25f;

		// Token: 0x04000D7D RID: 3453
		private float interactableCooldown;
	}
}
