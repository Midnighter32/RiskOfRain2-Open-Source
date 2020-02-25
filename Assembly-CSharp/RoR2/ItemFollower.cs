using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000267 RID: 615
	[RequireComponent(typeof(ItemDisplay))]
	public class ItemFollower : MonoBehaviour
	{
		// Token: 0x06000D9E RID: 3486 RVA: 0x0003D14A File Offset: 0x0003B34A
		private void Start()
		{
			this.itemDisplay = base.GetComponent<ItemDisplay>();
			this.followerLineRenderer = base.GetComponent<LineRenderer>();
			this.Rebuild();
		}

		// Token: 0x06000D9F RID: 3487 RVA: 0x0003D16C File Offset: 0x0003B36C
		private void Rebuild()
		{
			if (this.itemDisplay.GetVisibilityLevel() == VisibilityLevel.Invisible)
			{
				if (this.followerInstance)
				{
					UnityEngine.Object.Destroy(this.followerInstance);
				}
				if (this.followerLineRenderer)
				{
					this.followerLineRenderer.enabled = false;
					return;
				}
			}
			else
			{
				if (!this.followerInstance)
				{
					this.followerInstance = UnityEngine.Object.Instantiate<GameObject>(this.followerPrefab, this.targetObject.transform.position, Quaternion.identity);
					this.followerInstance.transform.localScale = base.transform.localScale;
					if (this.followerCurve)
					{
						this.v0 = this.followerCurve.v0;
						this.v1 = this.followerCurve.v1;
					}
				}
				if (this.followerLineRenderer)
				{
					this.followerLineRenderer.enabled = true;
				}
			}
		}

		// Token: 0x06000DA0 RID: 3488 RVA: 0x0003D254 File Offset: 0x0003B454
		private void Update()
		{
			this.Rebuild();
			if (this.followerInstance)
			{
				Transform transform = this.followerInstance.transform;
				Transform transform2 = this.targetObject.transform;
				transform.position = Vector3.SmoothDamp(transform.position, transform2.position, ref this.velocityDistance, this.distanceDampTime);
				transform.rotation = transform2.rotation;
				if (this.followerCurve)
				{
					this.followerCurve.v0 = base.transform.TransformVector(this.v0);
					this.followerCurve.v1 = transform.TransformVector(this.v1);
					this.followerCurve.p1 = transform.position;
				}
			}
		}

		// Token: 0x06000DA1 RID: 3489 RVA: 0x0003D30F File Offset: 0x0003B50F
		private void OnDestroy()
		{
			if (this.followerInstance)
			{
				UnityEngine.Object.Destroy(this.followerInstance);
			}
		}

		// Token: 0x04000DA1 RID: 3489
		public GameObject followerPrefab;

		// Token: 0x04000DA2 RID: 3490
		public GameObject targetObject;

		// Token: 0x04000DA3 RID: 3491
		public BezierCurveLine followerCurve;

		// Token: 0x04000DA4 RID: 3492
		public LineRenderer followerLineRenderer;

		// Token: 0x04000DA5 RID: 3493
		public float distanceDampTime;

		// Token: 0x04000DA6 RID: 3494
		public float distanceMaxSpeed;

		// Token: 0x04000DA7 RID: 3495
		private ItemDisplay itemDisplay;

		// Token: 0x04000DA8 RID: 3496
		private Vector3 velocityDistance;

		// Token: 0x04000DA9 RID: 3497
		private Vector3 v0;

		// Token: 0x04000DAA RID: 3498
		private Vector3 v1;

		// Token: 0x04000DAB RID: 3499
		[HideInInspector]
		public GameObject followerInstance;
	}
}
