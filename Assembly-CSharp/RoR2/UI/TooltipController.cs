using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000640 RID: 1600
	public class TooltipController : MonoBehaviour
	{
		// Token: 0x060025A4 RID: 9636 RVA: 0x000A3CD0 File Offset: 0x000A1ED0
		private void SetTooltipProvider(TooltipProvider provider)
		{
			this.titleLabel.text = provider.titleText;
			this.titleLabel.richText = !provider.disableTitleRichText;
			this.bodyLabel.text = provider.bodyText;
			this.bodyLabel.richText = !provider.disableBodyRichText;
			this.colorHighlightImage.color = provider.titleColor;
		}

		// Token: 0x060025A5 RID: 9637 RVA: 0x000A3D38 File Offset: 0x000A1F38
		private static UICamera FindUICamera(MPEventSystem mpEventSystem)
		{
			foreach (UICamera uicamera in UICamera.readOnlyInstancesList)
			{
				if (uicamera.GetAssociatedEventSystem() as MPEventSystem == mpEventSystem)
				{
					return uicamera;
				}
			}
			return null;
		}

		// Token: 0x060025A6 RID: 9638 RVA: 0x000A3D98 File Offset: 0x000A1F98
		private void Awake()
		{
			TooltipController.instancesList.Add(this);
		}

		// Token: 0x060025A7 RID: 9639 RVA: 0x000A3DA5 File Offset: 0x000A1FA5
		private void OnDestroy()
		{
			TooltipController.instancesList.Remove(this);
		}

		// Token: 0x060025A8 RID: 9640 RVA: 0x000A3DB4 File Offset: 0x000A1FB4
		private void LateUpdate()
		{
			Vector2 v;
			if (this.owner && this.owner.GetCursorPosition(out v))
			{
				this.tooltipCenterTransform.position = v;
			}
		}

		// Token: 0x060025A9 RID: 9641 RVA: 0x000A3DF0 File Offset: 0x000A1FF0
		public static void RemoveTooltip(TooltipProvider tooltipProvider)
		{
			if (tooltipProvider.userCount > 0)
			{
				foreach (MPEventSystem eventSystem in MPEventSystem.readOnlyInstancesList)
				{
					TooltipController.RemoveTooltip(eventSystem, tooltipProvider);
				}
			}
		}

		// Token: 0x060025AA RID: 9642 RVA: 0x000A3E44 File Offset: 0x000A2044
		public static void RemoveTooltip(MPEventSystem eventSystem, TooltipProvider tooltipProvider)
		{
			if (eventSystem.currentTooltipProvider == tooltipProvider)
			{
				TooltipController.SetTooltip(eventSystem, null, Vector3.zero);
			}
		}

		// Token: 0x060025AB RID: 9643 RVA: 0x000A3E68 File Offset: 0x000A2068
		public static void SetTooltip(MPEventSystem eventSystem, TooltipProvider newTooltipProvider, Vector2 tooltipPosition)
		{
			if (eventSystem.currentTooltipProvider != newTooltipProvider)
			{
				if (eventSystem.currentTooltip)
				{
					UnityEngine.Object.Destroy(eventSystem.currentTooltip.gameObject);
					eventSystem.currentTooltip = null;
				}
				if (eventSystem.currentTooltipProvider)
				{
					eventSystem.currentTooltipProvider.userCount--;
				}
				eventSystem.currentTooltipProvider = newTooltipProvider;
				if (newTooltipProvider)
				{
					newTooltipProvider.userCount++;
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/Tooltip"));
					eventSystem.currentTooltip = gameObject.GetComponent<TooltipController>();
					eventSystem.currentTooltip.owner = eventSystem;
					eventSystem.currentTooltip.uiCamera = TooltipController.FindUICamera(eventSystem);
					eventSystem.currentTooltip.SetTooltipProvider(eventSystem.currentTooltipProvider);
					Canvas component = gameObject.GetComponent<Canvas>();
					UICamera uicamera = eventSystem.currentTooltip.uiCamera;
					component.worldCamera = ((uicamera != null) ? uicamera.camera : null);
				}
			}
			if (eventSystem.currentTooltip)
			{
				Vector2 zero = Vector2.zero;
				UICamera uicamera2 = eventSystem.currentTooltip.uiCamera;
				Camera camera = Camera.main;
				if (uicamera2)
				{
					camera = uicamera2.camera;
				}
				if (camera)
				{
					Vector3 vector = camera.ScreenToViewportPoint(new Vector3(tooltipPosition.x, tooltipPosition.y, 0f));
					zero = new Vector2(vector.x, vector.y);
				}
				Vector2 vector2 = new Vector2(0f, 0f);
				vector2.x = ((zero.x > 0.5f) ? 1f : 0f);
				vector2.y = ((zero.y > 0.5f) ? 1f : 0f);
				eventSystem.currentTooltip.tooltipFlipTransform.anchorMin = vector2;
				eventSystem.currentTooltip.tooltipFlipTransform.anchorMax = vector2;
				eventSystem.currentTooltip.tooltipFlipTransform.pivot = vector2;
			}
		}

		// Token: 0x04002353 RID: 9043
		private static readonly List<TooltipController> instancesList = new List<TooltipController>();

		// Token: 0x04002354 RID: 9044
		[NonSerialized]
		public MPEventSystem owner;

		// Token: 0x04002355 RID: 9045
		public RectTransform tooltipCenterTransform;

		// Token: 0x04002356 RID: 9046
		public RectTransform tooltipFlipTransform;

		// Token: 0x04002357 RID: 9047
		public Image colorHighlightImage;

		// Token: 0x04002358 RID: 9048
		public TextMeshProUGUI titleLabel;

		// Token: 0x04002359 RID: 9049
		public TextMeshProUGUI bodyLabel;

		// Token: 0x0400235A RID: 9050
		private UICamera uiCamera;
	}
}
