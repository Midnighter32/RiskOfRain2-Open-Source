using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200064B RID: 1611
	public class TooltipController : MonoBehaviour
	{
		// Token: 0x06002405 RID: 9221 RVA: 0x000A9208 File Offset: 0x000A7408
		private void SetTooltipProvider(TooltipProvider provider)
		{
			this.titleLabel.text = provider.titleText;
			this.titleLabel.richText = !provider.disableTitleRichText;
			this.bodyLabel.text = provider.bodyText;
			this.bodyLabel.richText = !provider.disableBodyRichText;
			this.colorHighlightImage.color = provider.titleColor;
		}

		// Token: 0x06002406 RID: 9222 RVA: 0x000A9270 File Offset: 0x000A7470
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

		// Token: 0x06002407 RID: 9223 RVA: 0x000A92D0 File Offset: 0x000A74D0
		private void Awake()
		{
			TooltipController.instancesList.Add(this);
		}

		// Token: 0x06002408 RID: 9224 RVA: 0x000A92DD File Offset: 0x000A74DD
		private void OnDestroy()
		{
			TooltipController.instancesList.Remove(this);
		}

		// Token: 0x06002409 RID: 9225 RVA: 0x000A92EC File Offset: 0x000A74EC
		private void LateUpdate()
		{
			Vector2 v;
			if (this.owner && this.owner.GetCursorPosition(out v))
			{
				this.tooltipCenterTransform.position = v;
			}
		}

		// Token: 0x0600240A RID: 9226 RVA: 0x000A9328 File Offset: 0x000A7528
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

		// Token: 0x0600240B RID: 9227 RVA: 0x000A937C File Offset: 0x000A757C
		public static void RemoveTooltip(MPEventSystem eventSystem, TooltipProvider tooltipProvider)
		{
			if (eventSystem.currentTooltipProvider == tooltipProvider)
			{
				TooltipController.SetTooltip(eventSystem, null, Vector3.zero);
			}
		}

		// Token: 0x0600240C RID: 9228 RVA: 0x000A93A0 File Offset: 0x000A75A0
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

		// Token: 0x040026EE RID: 9966
		private static readonly List<TooltipController> instancesList = new List<TooltipController>();

		// Token: 0x040026EF RID: 9967
		[NonSerialized]
		public MPEventSystem owner;

		// Token: 0x040026F0 RID: 9968
		public RectTransform tooltipCenterTransform;

		// Token: 0x040026F1 RID: 9969
		public RectTransform tooltipFlipTransform;

		// Token: 0x040026F2 RID: 9970
		public Image colorHighlightImage;

		// Token: 0x040026F3 RID: 9971
		public TextMeshProUGUI titleLabel;

		// Token: 0x040026F4 RID: 9972
		public TextMeshProUGUI bodyLabel;

		// Token: 0x040026F5 RID: 9973
		private UICamera uiCamera;
	}
}
