using System;
using System.Collections.Generic;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000362 RID: 866
	[DisallowMultipleComponent]
	public class PositionIndicator : MonoBehaviour
	{
		// Token: 0x17000283 RID: 643
		// (get) Token: 0x06001507 RID: 5383 RVA: 0x00059BEB File Offset: 0x00057DEB
		// (set) Token: 0x06001508 RID: 5384 RVA: 0x00059BF3 File Offset: 0x00057DF3
		public Vector3 defaultPosition { get; set; }

		// Token: 0x06001509 RID: 5385 RVA: 0x00059BFC File Offset: 0x00057DFC
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x0600150A RID: 5386 RVA: 0x00059C0C File Offset: 0x00057E0C
		private void Start()
		{
			if (!this.generateDefaultPosition)
			{
				this.generateDefaultPosition = true;
				this.defaultPosition = base.transform.position;
			}
			if (this.targetTransform)
			{
				this.yOffset = PositionIndicator.CalcHeadOffset(this.targetTransform) + 1f;
			}
		}

		// Token: 0x0600150B RID: 5387 RVA: 0x00059C60 File Offset: 0x00057E60
		private static float CalcHeadOffset(Transform transform)
		{
			Collider component = transform.GetComponent<Collider>();
			if (component)
			{
				return component.bounds.extents.y;
			}
			return 0f;
		}

		// Token: 0x0600150C RID: 5388 RVA: 0x00059C95 File Offset: 0x00057E95
		private void OnEnable()
		{
			PositionIndicator.instancesList.Add(this);
		}

		// Token: 0x0600150D RID: 5389 RVA: 0x00059CA2 File Offset: 0x00057EA2
		private void OnDisable()
		{
			PositionIndicator.instancesList.Remove(this);
		}

		// Token: 0x0600150E RID: 5390 RVA: 0x00059CB0 File Offset: 0x00057EB0
		private void OnValidate()
		{
			if (this.insideViewObject && this.insideViewObject.GetComponentInChildren<PositionIndicator>())
			{
				Debug.LogError("insideViewObject may not be assigned another object with another PositionIndicator in its heirarchy!");
				this.insideViewObject = null;
			}
			if (this.outsideViewObject && this.outsideViewObject.GetComponentInChildren<PositionIndicator>())
			{
				Debug.LogError("outsideViewObject may not be assigned another object with another PositionIndicator in its heirarchy!");
				this.outsideViewObject = null;
			}
		}

		// Token: 0x0600150F RID: 5391 RVA: 0x00059D1D File Offset: 0x00057F1D
		static PositionIndicator()
		{
			UICamera.onUICameraPreCull += PositionIndicator.UpdatePositions;
		}

		// Token: 0x06001510 RID: 5392 RVA: 0x00059D54 File Offset: 0x00057F54
		private static void UpdatePositions(UICamera uiCamera)
		{
			Camera sceneCam = uiCamera.cameraRigController.sceneCam;
			Camera camera = uiCamera.camera;
			Rect pixelRect = camera.pixelRect;
			Vector2 center = pixelRect.center;
			pixelRect.size *= 0.95f;
			pixelRect.center = center;
			Vector2 center2 = pixelRect.center;
			float num = 1f / (pixelRect.width * 0.5f);
			float num2 = 1f / (pixelRect.height * 0.5f);
			Quaternion rotation = uiCamera.transform.rotation;
			CameraRigController cameraRigController = uiCamera.cameraRigController;
			Transform y = null;
			if (cameraRigController && cameraRigController.target)
			{
				CharacterBody component = cameraRigController.target.GetComponent<CharacterBody>();
				if (component)
				{
					y = component.coreTransform;
				}
				else
				{
					y = cameraRigController.target.transform;
				}
			}
			for (int i = 0; i < PositionIndicator.instancesList.Count; i++)
			{
				PositionIndicator positionIndicator = PositionIndicator.instancesList[i];
				bool flag = false;
				bool flag2 = false;
				bool active = false;
				float num3 = 0f;
				if (PositionIndicator.cvPositionIndicatorsEnable.value)
				{
					Vector3 a = positionIndicator.targetTransform ? positionIndicator.targetTransform.position : positionIndicator.defaultPosition;
					if (!positionIndicator.targetTransform || (positionIndicator.targetTransform && positionIndicator.targetTransform != y))
					{
						active = true;
						Vector3 vector = sceneCam.WorldToScreenPoint(a + new Vector3(0f, positionIndicator.yOffset, 0f));
						bool flag3 = vector.z <= 0f;
						bool flag4 = !flag3 && pixelRect.Contains(vector);
						if (!flag4)
						{
							Vector2 vector2 = vector - center2;
							float a2 = Mathf.Abs(vector2.x * num);
							float b = Mathf.Abs(vector2.y * num2);
							float d = Mathf.Max(a2, b);
							vector2 /= d;
							vector2 *= (flag3 ? -1f : 1f);
							vector = vector2 + center2;
							if (positionIndicator.shouldRotateOutsideViewObject)
							{
								num3 = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
							}
						}
						flag = flag4;
						flag2 = !flag4;
						vector.z = 1f;
						Vector3 position = camera.ScreenToWorldPoint(vector);
						positionIndicator.transform.SetPositionAndRotation(position, rotation);
					}
				}
				if (positionIndicator.alwaysVisibleObject)
				{
					positionIndicator.alwaysVisibleObject.SetActive(active);
				}
				if (positionIndicator.insideViewObject == positionIndicator.outsideViewObject)
				{
					if (positionIndicator.insideViewObject)
					{
						positionIndicator.insideViewObject.SetActive(flag || flag2);
					}
				}
				else
				{
					if (positionIndicator.insideViewObject)
					{
						positionIndicator.insideViewObject.SetActive(flag);
					}
					if (positionIndicator.outsideViewObject)
					{
						positionIndicator.outsideViewObject.SetActive(flag2);
						if (flag2 && positionIndicator.shouldRotateOutsideViewObject)
						{
							positionIndicator.outsideViewObject.transform.localEulerAngles = new Vector3(0f, 0f, num3 + positionIndicator.outsideViewRotationOffset);
						}
					}
				}
			}
		}

		// Token: 0x040013AB RID: 5035
		public Transform targetTransform;

		// Token: 0x040013AC RID: 5036
		private new Transform transform;

		// Token: 0x040013AD RID: 5037
		private static readonly List<PositionIndicator> instancesList = new List<PositionIndicator>();

		// Token: 0x040013AE RID: 5038
		[Tooltip("The child object to enable when the target is within the frame.")]
		public GameObject insideViewObject;

		// Token: 0x040013AF RID: 5039
		[Tooltip("The child object to enable when the target is outside the frame.")]
		public GameObject outsideViewObject;

		// Token: 0x040013B0 RID: 5040
		[Tooltip("The child object to ALWAYS enable, IF its not my own position indicator.")]
		public GameObject alwaysVisibleObject;

		// Token: 0x040013B1 RID: 5041
		[Tooltip("Whether or not outsideViewObject should be rotated to point to the target.")]
		public bool shouldRotateOutsideViewObject;

		// Token: 0x040013B2 RID: 5042
		[Tooltip("The offset to apply to the rotation of the outside view object when shouldRotateOutsideViewObject is set.")]
		public float outsideViewRotationOffset;

		// Token: 0x040013B3 RID: 5043
		private float yOffset;

		// Token: 0x040013B4 RID: 5044
		private bool generateDefaultPosition;

		// Token: 0x040013B6 RID: 5046
		private static BoolConVar cvPositionIndicatorsEnable = new BoolConVar("position_indicators_enable", ConVarFlags.None, "1", "Enables/Disables position indicators for allies, bosses, pings, etc.");
	}
}
