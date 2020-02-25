using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000174 RID: 372
	public class CameraTargetParams : MonoBehaviour
	{
		// Token: 0x06000703 RID: 1795 RVA: 0x0001D8C0 File Offset: 0x0001BAC0
		public void AddRecoil(float verticalMin, float verticalMax, float horizontalMin, float horizontalMax)
		{
			this.targetRecoil += new Vector2(UnityEngine.Random.Range(horizontalMin, horizontalMax), UnityEngine.Random.Range(verticalMin, verticalMax));
		}

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x06000704 RID: 1796 RVA: 0x0001D8E7 File Offset: 0x0001BAE7
		public Vector3 cameraPivotPosition
		{
			get
			{
				return (this.cameraPivotTransform ? this.cameraPivotTransform : base.transform).position + new Vector3(0f, this.currentPivotVerticalOffset, 0f);
			}
		}

		// Token: 0x06000705 RID: 1797 RVA: 0x0001D924 File Offset: 0x0001BB24
		private void Awake()
		{
			CharacterBody component = base.GetComponent<CharacterBody>();
			if (component && this.cameraPivotTransform == null)
			{
				this.cameraPivotTransform = component.aimOriginTransform;
			}
		}

		// Token: 0x06000706 RID: 1798 RVA: 0x0001D95C File Offset: 0x0001BB5C
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

		// Token: 0x0400076D RID: 1901
		public CharacterCameraParams cameraParams;

		// Token: 0x0400076E RID: 1902
		public Transform cameraPivotTransform;

		// Token: 0x0400076F RID: 1903
		public CameraTargetParams.AimType aimMode;

		// Token: 0x04000770 RID: 1904
		[HideInInspector]
		public Vector2 recoil;

		// Token: 0x04000771 RID: 1905
		[HideInInspector]
		public Vector3 idealLocalCameraPos;

		// Token: 0x04000772 RID: 1906
		[HideInInspector]
		[NonSerialized]
		public float fovOverride = -1f;

		// Token: 0x04000773 RID: 1907
		[HideInInspector]
		public bool dontRaycastToPivot;

		// Token: 0x04000774 RID: 1908
		private float currentPivotVerticalOffset;

		// Token: 0x04000775 RID: 1909
		private float currentPivotVerticalOffsetVelocity;

		// Token: 0x04000776 RID: 1910
		private static float targetRecoilDampTime = 0.08f;

		// Token: 0x04000777 RID: 1911
		private static float recoilDampTime = 0.05f;

		// Token: 0x04000778 RID: 1912
		private Vector2 targetRecoil;

		// Token: 0x04000779 RID: 1913
		private Vector2 recoilVelocity;

		// Token: 0x0400077A RID: 1914
		private Vector2 targetRecoilVelocity;

		// Token: 0x0400077B RID: 1915
		private Vector3 aimVelocity;

		// Token: 0x02000175 RID: 373
		public enum AimType
		{
			// Token: 0x0400077D RID: 1917
			Standard,
			// Token: 0x0400077E RID: 1918
			FirstPerson,
			// Token: 0x0400077F RID: 1919
			Aura,
			// Token: 0x04000780 RID: 1920
			Sprinting,
			// Token: 0x04000781 RID: 1921
			AimThrow
		}
	}
}
