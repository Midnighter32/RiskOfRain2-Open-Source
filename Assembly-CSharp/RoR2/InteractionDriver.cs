using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200033A RID: 826
	[RequireComponent(typeof(Interactor))]
	[RequireComponent(typeof(InputBankTest))]
	public class InteractionDriver : MonoBehaviour, ILifeBehavior
	{
		// Token: 0x17000176 RID: 374
		// (get) Token: 0x060010E3 RID: 4323 RVA: 0x00054446 File Offset: 0x00052646
		// (set) Token: 0x060010E4 RID: 4324 RVA: 0x0005444E File Offset: 0x0005264E
		public Interactor interactor { get; private set; }

		// Token: 0x060010E5 RID: 4325 RVA: 0x00054457 File Offset: 0x00052657
		private void Awake()
		{
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
			this.interactor = base.GetComponent<Interactor>();
			this.inputBank = base.GetComponent<InputBankTest>();
		}

		// Token: 0x060010E6 RID: 4326 RVA: 0x00054480 File Offset: 0x00052680
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

		// Token: 0x060010E7 RID: 4327 RVA: 0x00054540 File Offset: 0x00052740
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

		// Token: 0x060010E8 RID: 4328 RVA: 0x000545BE File Offset: 0x000527BE
		static InteractionDriver()
		{
			OutlineHighlight.onPreRenderOutlineHighlight = (Action<OutlineHighlight>)Delegate.Combine(OutlineHighlight.onPreRenderOutlineHighlight, new Action<OutlineHighlight>(InteractionDriver.OnPreRenderOutlineHighlight));
		}

		// Token: 0x060010E9 RID: 4329 RVA: 0x000545E0 File Offset: 0x000527E0
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
			if (component2 != null && component2.GetInteractability(component.interactor) == Interactability.ConditionsNotMet)
			{
				a = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Unaffordable);
			}
			outlineHighlight.highlightQueue.Enqueue(new OutlineHighlight.HighlightInfo
			{
				renderer = component3.targetRenderer,
				color = a * component3.strength
			});
		}

		// Token: 0x060010EA RID: 4330 RVA: 0x0003F5D8 File Offset: 0x0003D7D8
		public void OnDeathStart()
		{
			base.enabled = false;
		}

		// Token: 0x04001527 RID: 5415
		public bool highlightInteractor;

		// Token: 0x04001528 RID: 5416
		private bool inputReceived;

		// Token: 0x04001529 RID: 5417
		private NetworkIdentity networkIdentity;

		// Token: 0x0400152B RID: 5419
		private InputBankTest inputBank;

		// Token: 0x0400152C RID: 5420
		[NonSerialized]
		public GameObject interactableOverride;

		// Token: 0x0400152D RID: 5421
		private const float interactableCooldownDuration = 0.25f;

		// Token: 0x0400152E RID: 5422
		private float interactableCooldown;
	}
}
