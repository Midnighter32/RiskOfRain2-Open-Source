using System;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.Hologram
{
	// Token: 0x02000538 RID: 1336
	public class HologramProjector : MonoBehaviour
	{
		// Token: 0x06001F79 RID: 8057 RVA: 0x00088904 File Offset: 0x00086B04
		private void Awake()
		{
			this.contentProvider = base.GetComponent<IHologramContentProvider>();
		}

		// Token: 0x06001F7A RID: 8058 RVA: 0x00088914 File Offset: 0x00086B14
		private Transform FindViewer(Vector3 position)
		{
			if (this.viewerReselectTimer > 0f)
			{
				return this.cachedViewer;
			}
			this.viewerReselectTimer = this.viewerReselectInterval;
			this.cachedViewer = null;
			float num = float.PositiveInfinity;
			ReadOnlyCollection<PlayerCharacterMasterController> instances = PlayerCharacterMasterController.instances;
			int i = 0;
			int count = instances.Count;
			while (i < count)
			{
				GameObject bodyObject = instances[i].master.GetBodyObject();
				if (bodyObject)
				{
					float sqrMagnitude = (bodyObject.transform.position - position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						num = sqrMagnitude;
						this.cachedViewer = bodyObject.transform;
					}
				}
				i++;
			}
			return this.cachedViewer;
		}

		// Token: 0x06001F7B RID: 8059 RVA: 0x000889BC File Offset: 0x00086BBC
		private void Update()
		{
			this.viewerReselectTimer -= Time.deltaTime;
			Vector3 vector = this.hologramPivot ? this.hologramPivot.position : base.transform.position;
			this.viewer = this.FindViewer(vector);
			Vector3 b = this.viewer ? this.viewer.position : base.transform.position;
			bool flag = false;
			Vector3 forward = Vector3.zero;
			if (this.viewer)
			{
				forward = vector - b;
				if (forward.sqrMagnitude <= this.displayDistance * this.displayDistance)
				{
					flag = true;
				}
			}
			if (flag)
			{
				flag = this.contentProvider.ShouldDisplayHologram(this.viewer.gameObject);
			}
			if (flag)
			{
				if (!this.hologramContentInstance)
				{
					this.BuildHologram();
				}
				if (this.hologramContentInstance && this.contentProvider != null)
				{
					this.contentProvider.UpdateHologramContent(this.hologramContentInstance);
					if (!this.disableHologramRotation)
					{
						this.hologramContentInstance.transform.rotation = Util.SmoothDampQuaternion(this.hologramContentInstance.transform.rotation, Util.QuaternionSafeLookRotation(forward), ref this.transformDampVelocity, 0.2f);
						return;
					}
				}
			}
			else
			{
				this.DestroyHologram();
			}
		}

		// Token: 0x06001F7C RID: 8060 RVA: 0x00088B04 File Offset: 0x00086D04
		private void BuildHologram()
		{
			this.DestroyHologram();
			if (this.contentProvider != null)
			{
				GameObject hologramContentPrefab = this.contentProvider.GetHologramContentPrefab();
				if (hologramContentPrefab)
				{
					this.hologramContentInstance = UnityEngine.Object.Instantiate<GameObject>(hologramContentPrefab);
					this.hologramContentInstance.transform.parent = (this.hologramPivot ? this.hologramPivot : base.transform);
					this.hologramContentInstance.transform.localPosition = Vector3.zero;
					this.hologramContentInstance.transform.localRotation = Quaternion.identity;
					this.hologramContentInstance.transform.localScale = Vector3.one;
					if (this.viewer && !this.disableHologramRotation)
					{
						Vector3 a = this.hologramPivot ? this.hologramPivot.position : base.transform.position;
						Vector3 position = this.viewer.position;
						Vector3 forward = a - this.viewer.position;
						this.hologramContentInstance.transform.rotation = Util.QuaternionSafeLookRotation(forward);
					}
					this.contentProvider.UpdateHologramContent(this.hologramContentInstance);
				}
			}
		}

		// Token: 0x06001F7D RID: 8061 RVA: 0x00088C2D File Offset: 0x00086E2D
		private void DestroyHologram()
		{
			if (this.hologramContentInstance)
			{
				UnityEngine.Object.Destroy(this.hologramContentInstance);
			}
			this.hologramContentInstance = null;
		}

		// Token: 0x04001D2C RID: 7468
		[Tooltip("The range in meters at which the hologram begins to display.")]
		public float displayDistance = 15f;

		// Token: 0x04001D2D RID: 7469
		[Tooltip("The position at which to display the hologram.")]
		public Transform hologramPivot;

		// Token: 0x04001D2E RID: 7470
		[Tooltip("Whether or not the hologram will pivot to the player")]
		public bool disableHologramRotation;

		// Token: 0x04001D2F RID: 7471
		private float transformDampVelocity;

		// Token: 0x04001D30 RID: 7472
		private IHologramContentProvider contentProvider;

		// Token: 0x04001D31 RID: 7473
		private float viewerReselectTimer;

		// Token: 0x04001D32 RID: 7474
		private float viewerReselectInterval = 0.25f;

		// Token: 0x04001D33 RID: 7475
		private Transform cachedViewer;

		// Token: 0x04001D34 RID: 7476
		private Transform viewer;

		// Token: 0x04001D35 RID: 7477
		private GameObject hologramContentInstance;
	}
}
