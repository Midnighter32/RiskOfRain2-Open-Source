using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000409 RID: 1033
	[DisallowMultipleComponent]
	public class PositionIndicator : MonoBehaviour
	{
		// Token: 0x1700021E RID: 542
		// (get) Token: 0x06001701 RID: 5889 RVA: 0x0006D83B File Offset: 0x0006BA3B
		// (set) Token: 0x06001702 RID: 5890 RVA: 0x0006D843 File Offset: 0x0006BA43
		public Vector3 defaultPosition { get; set; }

		// Token: 0x06001703 RID: 5891 RVA: 0x0006D84C File Offset: 0x0006BA4C
		private void Start()
		{
			this.transform = base.transform;
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

		// Token: 0x06001704 RID: 5892 RVA: 0x0006D8AC File Offset: 0x0006BAAC
		private static float CalcHeadOffset(Transform transform)
		{
			Collider component = transform.GetComponent<Collider>();
			if (component)
			{
				return component.bounds.extents.y;
			}
			return 0f;
		}

		// Token: 0x06001705 RID: 5893 RVA: 0x0006D8E1 File Offset: 0x0006BAE1
		private void OnEnable()
		{
			PositionIndicator.instancesList.Add(this);
		}

		// Token: 0x06001706 RID: 5894 RVA: 0x0006D8EE File Offset: 0x0006BAEE
		private void OnDisable()
		{
			PositionIndicator.instancesList.Remove(this);
		}

		// Token: 0x06001707 RID: 5895 RVA: 0x0006D8FC File Offset: 0x0006BAFC
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

		// Token: 0x06001708 RID: 5896 RVA: 0x0006D969 File Offset: 0x0006BB69
		static PositionIndicator()
		{
			UICamera.onUICameraPreCull += PositionIndicator.UpdatePositions;
		}

		// Token: 0x06001709 RID: 5897 RVA: 0x0006D988 File Offset: 0x0006BB88
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
					positionIndicator.transform.position = camera.ScreenToWorldPoint(vector);
					positionIndicator.transform.rotation = rotation;
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

		// Token: 0x04001A3B RID: 6715
		public Transform targetTransform;

		// Token: 0x04001A3C RID: 6716
		private new Transform transform;

		// Token: 0x04001A3D RID: 6717
		private static readonly List<PositionIndicator> instancesList = new List<PositionIndicator>();

		// Token: 0x04001A3E RID: 6718
		[Tooltip("The child object to enable when the target is within the frame.")]
		public GameObject insideViewObject;

		// Token: 0x04001A3F RID: 6719
		[Tooltip("The child object to enable when the target is outside the frame.")]
		public GameObject outsideViewObject;

		// Token: 0x04001A40 RID: 6720
		[Tooltip("The child object to ALWAYS enable, IF its not my own position indicator.")]
		public GameObject alwaysVisibleObject;

		// Token: 0x04001A41 RID: 6721
		[Tooltip("Whether or not outsideViewObject should be rotated to point to the target.")]
		public bool shouldRotateOutsideViewObject;

		// Token: 0x04001A42 RID: 6722
		[Tooltip("The offset to apply to the rotation of the outside view object when shouldRotateOutsideViewObject is set.")]
		public float outsideViewRotationOffset;

		// Token: 0x04001A43 RID: 6723
		private float yOffset;

		// Token: 0x04001A44 RID: 6724
		private bool generateDefaultPosition;
	}
}
