using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003A1 RID: 929
	public class Indicator
	{
		// Token: 0x0600167E RID: 5758 RVA: 0x00060C24 File Offset: 0x0005EE24
		public Indicator(GameObject owner, GameObject visualizerPrefab)
		{
			this.owner = owner;
			this._visualizerPrefab = visualizerPrefab;
			this.visualizerRenderers = Array.Empty<Renderer>();
		}

		// Token: 0x1700029D RID: 669
		// (get) Token: 0x0600167F RID: 5759 RVA: 0x00060C4C File Offset: 0x0005EE4C
		// (set) Token: 0x06001680 RID: 5760 RVA: 0x00060C54 File Offset: 0x0005EE54
		public GameObject visualizerPrefab
		{
			get
			{
				return this._visualizerPrefab;
			}
			set
			{
				if (this._visualizerPrefab == value)
				{
					return;
				}
				if (this.visualizerInstance)
				{
					this.DestroyVisualizer();
				}
				this._visualizerPrefab = value;
			}
		}

		// Token: 0x1700029E RID: 670
		// (get) Token: 0x06001681 RID: 5761 RVA: 0x00060C7F File Offset: 0x0005EE7F
		// (set) Token: 0x06001682 RID: 5762 RVA: 0x00060C87 File Offset: 0x0005EE87
		private protected GameObject visualizerInstance { protected get; private set; }

		// Token: 0x1700029F RID: 671
		// (get) Token: 0x06001683 RID: 5763 RVA: 0x00060C90 File Offset: 0x0005EE90
		// (set) Token: 0x06001684 RID: 5764 RVA: 0x00060C98 File Offset: 0x0005EE98
		private protected Transform visualizerTransform { protected get; private set; }

		// Token: 0x170002A0 RID: 672
		// (get) Token: 0x06001685 RID: 5765 RVA: 0x00060CA1 File Offset: 0x0005EEA1
		// (set) Token: 0x06001686 RID: 5766 RVA: 0x00060CA9 File Offset: 0x0005EEA9
		private protected Renderer[] visualizerRenderers { protected get; private set; }

		// Token: 0x170002A1 RID: 673
		// (get) Token: 0x06001687 RID: 5767 RVA: 0x00060CB2 File Offset: 0x0005EEB2
		public bool hasVisualizer
		{
			get
			{
				return this.visualizerInstance;
			}
		}

		// Token: 0x170002A2 RID: 674
		// (get) Token: 0x06001688 RID: 5768 RVA: 0x00060CBF File Offset: 0x0005EEBF
		// (set) Token: 0x06001689 RID: 5769 RVA: 0x00060CC7 File Offset: 0x0005EEC7
		public bool active
		{
			get
			{
				return this._active;
			}
			set
			{
				if (this._active == value)
				{
					return;
				}
				this._active = value;
				if (this.active)
				{
					Indicator.IndicatorManager.AddIndicator(this);
					return;
				}
				Indicator.IndicatorManager.RemoveIndicator(this);
			}
		}

		// Token: 0x0600168A RID: 5770 RVA: 0x00060CEF File Offset: 0x0005EEEF
		public void SetVisualizerInstantiated(bool newVisualizerInstantiated)
		{
			if (this.visualizerInstance != newVisualizerInstantiated)
			{
				if (newVisualizerInstantiated)
				{
					this.InstantiateVisualizer();
					return;
				}
				this.DestroyVisualizer();
			}
		}

		// Token: 0x0600168B RID: 5771 RVA: 0x00060D0F File Offset: 0x0005EF0F
		private void InstantiateVisualizer()
		{
			this.visualizerInstance = UnityEngine.Object.Instantiate<GameObject>(this.visualizerPrefab);
			this.OnInstantiateVisualizer();
		}

		// Token: 0x0600168C RID: 5772 RVA: 0x00060D28 File Offset: 0x0005EF28
		private void DestroyVisualizer()
		{
			this.OnDestroyVisualizer();
			UnityEngine.Object.Destroy(this.visualizerInstance);
			this.visualizerInstance = null;
		}

		// Token: 0x0600168D RID: 5773 RVA: 0x00060D42 File Offset: 0x0005EF42
		public void OnInstantiateVisualizer()
		{
			this.visualizerTransform = this.visualizerInstance.transform;
			this.visualizerRenderers = this.visualizerInstance.GetComponentsInChildren<Renderer>();
			this.SetVisibleInternal(this.visible);
		}

		// Token: 0x0600168E RID: 5774 RVA: 0x00060D72 File Offset: 0x0005EF72
		public virtual void OnDestroyVisualizer()
		{
			this.visualizerTransform = null;
			this.visualizerRenderers = Array.Empty<Renderer>();
		}

		// Token: 0x0600168F RID: 5775 RVA: 0x0000409B File Offset: 0x0000229B
		public virtual void UpdateVisualizer()
		{
		}

		// Token: 0x06001690 RID: 5776 RVA: 0x00060D88 File Offset: 0x0005EF88
		public virtual void PositionForUI(Camera sceneCamera, Camera uiCamera)
		{
			if (this.targetTransform)
			{
				Vector3 position = this.targetTransform.position;
				Vector3 vector = sceneCamera.WorldToScreenPoint(position);
				vector.z = ((vector.z > 0f) ? 1f : -1f);
				Vector3 position2 = uiCamera.ScreenToWorldPoint(vector);
				this.visualizerTransform.position = position2;
			}
		}

		// Token: 0x06001691 RID: 5777 RVA: 0x00060DEA File Offset: 0x0005EFEA
		public void SetVisible(bool newVisible)
		{
			newVisible &= this.targetTransform;
			if (this.visible != newVisible)
			{
				this.SetVisibleInternal(newVisible);
			}
		}

		// Token: 0x06001692 RID: 5778 RVA: 0x00060E0C File Offset: 0x0005F00C
		private void SetVisibleInternal(bool newVisible)
		{
			this.visible = newVisible;
			Renderer[] visualizerRenderers = this.visualizerRenderers;
			for (int i = 0; i < visualizerRenderers.Length; i++)
			{
				visualizerRenderers[i].enabled = newVisible;
			}
		}

		// Token: 0x04001529 RID: 5417
		private GameObject _visualizerPrefab;

		// Token: 0x0400152A RID: 5418
		public readonly GameObject owner;

		// Token: 0x0400152B RID: 5419
		public Transform targetTransform;

		// Token: 0x0400152F RID: 5423
		private bool _active;

		// Token: 0x04001530 RID: 5424
		private bool visible = true;

		// Token: 0x020003A2 RID: 930
		private static class IndicatorManager
		{
			// Token: 0x06001693 RID: 5779 RVA: 0x00060E3E File Offset: 0x0005F03E
			public static void AddIndicator([NotNull] Indicator indicator)
			{
				Indicator.IndicatorManager.runningIndicators.Add(indicator);
				Indicator.IndicatorManager.RebuildVisualizer(indicator);
			}

			// Token: 0x06001694 RID: 5780 RVA: 0x00060E51 File Offset: 0x0005F051
			public static void RemoveIndicator([NotNull] Indicator indicator)
			{
				indicator.SetVisualizerInstantiated(false);
				Indicator.IndicatorManager.runningIndicators.Remove(indicator);
			}

			// Token: 0x06001695 RID: 5781 RVA: 0x00060E68 File Offset: 0x0005F068
			static IndicatorManager()
			{
				CameraRigController.onCameraTargetChanged += delegate(CameraRigController cameraRigController, GameObject target)
				{
					Indicator.IndicatorManager.RebuildVisualizerForAll();
				};
				UICamera.onUICameraPreRender += Indicator.IndicatorManager.OnPreRenderUI;
				UICamera.onUICameraPostRender += Indicator.IndicatorManager.OnPostRenderUI;
				RoR2Application.onUpdate += Indicator.IndicatorManager.Update;
			}

			// Token: 0x06001696 RID: 5782 RVA: 0x00060EC8 File Offset: 0x0005F0C8
			private static void RebuildVisualizerForAll()
			{
				foreach (Indicator indicator in Indicator.IndicatorManager.runningIndicators)
				{
					Indicator.IndicatorManager.RebuildVisualizer(indicator);
				}
			}

			// Token: 0x06001697 RID: 5783 RVA: 0x00060F18 File Offset: 0x0005F118
			private static void Update()
			{
				foreach (Indicator indicator in Indicator.IndicatorManager.runningIndicators)
				{
					if (indicator.hasVisualizer)
					{
						indicator.UpdateVisualizer();
					}
				}
			}

			// Token: 0x06001698 RID: 5784 RVA: 0x00060F74 File Offset: 0x0005F174
			private static void RebuildVisualizer(Indicator indicator)
			{
				bool visualizerInstantiated = false;
				using (IEnumerator<CameraRigController> enumerator = CameraRigController.readOnlyInstancesList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.target == indicator.owner)
						{
							visualizerInstantiated = true;
							break;
						}
					}
				}
				indicator.SetVisualizerInstantiated(visualizerInstantiated);
			}

			// Token: 0x06001699 RID: 5785 RVA: 0x00060FD8 File Offset: 0x0005F1D8
			private static void OnPreRenderUI(UICamera uiCam)
			{
				GameObject target = uiCam.cameraRigController.target;
				Camera sceneCam = uiCam.cameraRigController.sceneCam;
				foreach (Indicator indicator in Indicator.IndicatorManager.runningIndicators)
				{
					bool flag = target == indicator.owner;
					indicator.SetVisible(target == indicator.owner);
					if (flag)
					{
						indicator.PositionForUI(sceneCam, uiCam.camera);
					}
				}
			}

			// Token: 0x0600169A RID: 5786 RVA: 0x00061068 File Offset: 0x0005F268
			private static void OnPostRenderUI(UICamera uiCamera)
			{
				foreach (Indicator indicator in Indicator.IndicatorManager.runningIndicators)
				{
					indicator.SetVisible(true);
				}
			}

			// Token: 0x04001531 RID: 5425
			private static readonly List<Indicator> runningIndicators = new List<Indicator>();
		}
	}
}
