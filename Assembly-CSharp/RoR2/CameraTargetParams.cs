using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200027C RID: 636
	public class CameraTargetParams : MonoBehaviour
	{
		// Token: 0x06000C0A RID: 3082 RVA: 0x0003B946 File Offset: 0x00039B46
		public void AddRecoil(float verticalMin, float verticalMax, float horizontalMin, float horizontalMax)
		{
			this.targetRecoil += new Vector2(UnityEngine.Random.Range(horizontalMin, horizontalMax), UnityEngine.Random.Range(verticalMin, verticalMax));
		}

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x06000C0B RID: 3083 RVA: 0x0003B96D File Offset: 0x00039B6D
		public Vector3 cameraPivotPosition
		{
			get
			{
				return (this.cameraPivotTransform ? this.cameraPivotTransform : base.transform).position + new Vector3(0f, this.currentPivotVerticalOffset, 0f);
			}
		}

		// Token: 0x06000C0C RID: 3084 RVA: 0x0003B9AC File Offset: 0x00039BAC
		private void Awake()
		{
			CharacterBody component = base.GetComponent<CharacterBody>();
			if (component && this.cameraPivotTransform == null)
			{
				this.cameraPivotTransform = component.aimOriginTransform;
			}
		}

		// Token: 0x06000C0D RID: 3085 RVA: 0x0003B9E4 File Offset: 0x00039BE4
		private void Update()
		{
			this.targetRecoil = Vector2.SmoothDamp(this.targetRecoil, Vector2.zero, ref this.targetRecoilVelocity, CameraTargetParams.targetRecoilDampTime, 180f, Time.deltaTime);
			this.recoil = Vector2.SmoothDamp(this.recoil, this.targetRecoil, ref this.recoilVelocity, CameraTargetParams.recoilDampTime, 180f, Time.deltaTime);
			switch (this.aimMode)
			{
			case CameraTargetParams.AimType.Standard:
				this.idealLocalCameraPos = Vector3.SmoothDamp(this.idealLocalCameraPos, this.cameraParams.standardLocalCameraPos, ref this.aimVelocity, 0.5f);
				this.currentPivotVerticalOffset = Mathf.SmoothDamp(this.currentPivotVerticalOffset, this.cameraParams.pivotVerticalOffset, ref this.currentPivotVerticalOffsetVelocity, 0.5f);
				return;
			case CameraTargetParams.AimType.FirstPerson:
				this.idealLocalCameraPos = Vector3.SmoothDamp(this.idealLocalCameraPos, Vector3.zero, ref this.aimVelocity, 0.4f);
				this.currentPivotVerticalOffset = Mathf.SmoothDamp(this.currentPivotVerticalOffset, 0f, ref this.currentPivotVerticalOffsetVelocity, 0.5f);
				return;
			case CameraTargetParams.AimType.Aura:
				this.idealLocalCameraPos = Vector3.SmoothDamp(this.idealLocalCameraPos, this.cameraParams.standardLocalCameraPos + new Vector3(0f, 1.5f, -7f), ref this.aimVelocity, 0.5f);
				this.currentPivotVerticalOffset = Mathf.SmoothDamp(this.currentPivotVerticalOffset, this.cameraParams.pivotVerticalOffset, ref this.currentPivotVerticalOffsetVelocity, 0.5f);
				return;
			case CameraTargetParams.AimType.Sprinting:
				this.idealLocalCameraPos = Vector3.SmoothDamp(this.idealLocalCameraPos, this.cameraParams.standardLocalCameraPos + new Vector3(0f, 0f, 0f), ref this.aimVelocity, 1f);
				this.currentPivotVerticalOffset = Mathf.SmoothDamp(this.currentPivotVerticalOffset, this.cameraParams.pivotVerticalOffset, ref this.currentPivotVerticalOffsetVelocity, 0.1f);
				return;
			case CameraTargetParams.AimType.AimThrow:
			{
				Vector3 standardLocalCameraPos = this.cameraParams.standardLocalCameraPos;
				standardLocalCameraPos.z = -8f;
				standardLocalCameraPos.y = 1f;
				standardLocalCameraPos.x = 1f;
				this.idealLocalCameraPos = Vector3.SmoothDamp(this.idealLocalCameraPos, standardLocalCameraPos, ref this.aimVelocity, 0.4f);
				this.currentPivotVerticalOffset = Mathf.SmoothDamp(this.currentPivotVerticalOffset, 0f, ref this.currentPivotVerticalOffsetVelocity, 0.5f);
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x04001019 RID: 4121
		public CharacterCameraParams cameraParams;

		// Token: 0x0400101A RID: 4122
		public Transform cameraPivotTransform;

		// Token: 0x0400101B RID: 4123
		public CameraTargetParams.AimType aimMode;

		// Token: 0x0400101C RID: 4124
		[HideInInspector]
		public Vector2 recoil;

		// Token: 0x0400101D RID: 4125
		[HideInInspector]
		public Vector3 idealLocalCameraPos;

		// Token: 0x0400101E RID: 4126
		[HideInInspector]
		[NonSerialized]
		public float fovOverride = -1f;

		// Token: 0x0400101F RID: 4127
		[HideInInspector]
		public bool dontRaycastToPivot;

		// Token: 0x04001020 RID: 4128
		private float currentPivotVerticalOffset;

		// Token: 0x04001021 RID: 4129
		private float currentPivotVerticalOffsetVelocity;

		// Token: 0x04001022 RID: 4130
		private static float targetRecoilDampTime = 0.08f;

		// Token: 0x04001023 RID: 4131
		private static float recoilDampTime = 0.05f;

		// Token: 0x04001024 RID: 4132
		private Vector2 targetRecoil;

		// Token: 0x04001025 RID: 4133
		private Vector2 recoilVelocity;

		// Token: 0x04001026 RID: 4134
		private Vector2 targetRecoilVelocity;

		// Token: 0x04001027 RID: 4135
		private Vector3 aimVelocity;

		// Token: 0x0200027D RID: 637
		public enum AimType
		{
			// Token: 0x04001029 RID: 4137
			Standard,
			// Token: 0x0400102A RID: 4138
			FirstPerson,
			// Token: 0x0400102B RID: 4139
			Aura,
			// Token: 0x0400102C RID: 4140
			Sprinting,
			// Token: 0x0400102D RID: 4141
			AimThrow
		}
	}
}
