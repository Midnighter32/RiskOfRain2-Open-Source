using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200043E RID: 1086
	public class Indicator
	{
		// Token: 0x0600181C RID: 6172 RVA: 0x00073168 File Offset: 0x00071368
		public Indicator(GameObject owner, GameObject visualizerPrefab)
		{
			this.owner = owner;
			this._visualizerPrefab = visualizerPrefab;
			this.visualizerRenderers = Array.Empty<Renderer>();
		}

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x0600181D RID: 6173 RVA: 0x00073190 File Offset: 0x00071390
		// (set) Token: 0x0600181E RID: 6174 RVA: 0x00073198 File Offset: 0x00071398
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

		// Token: 0x1700022F RID: 559
		// (get) Token: 0x0600181F RID: 6175 RVA: 0x000731C3 File Offset: 0x000713C3
		// (set) Token: 0x06001820 RID: 6176 RVA: 0x000731CB File Offset: 0x000713CB
		private protected GameObject visualizerInstance { protected get; private set; }

		// Token: 0x17000230 RID: 560
		// (get) Token: 0x06001821 RID: 6177 RVA: 0x000731D4 File Offset: 0x000713D4
		// (set) Token: 0x06001822 RID: 6178 RVA: 0x000731DC File Offset: 0x000713DC
		private protected Transform visualizerTransform { protected get; private set; }

		// Token: 0x17000231 RID: 561
		// (get) Token: 0x06001823 RID: 6179 RVA: 0x000731E5 File Offset: 0x000713E5
		// (set) Token: 0x06001824 RID: 6180 RVA: 0x000731ED File Offset: 0x000713ED
		private protected Renderer[] visualizerRenderers { protected get; private set; }

		// Token: 0x17000232 RID: 562
		// (get) Token: 0x06001825 RID: 6181 RVA: 0x000731F6 File Offset: 0x000713F6
		public bool hasVisualizer
		{
			get
			{
				return this.visualizerInstance;
			}
		}

		// Token: 0x17000233 RID: 563
		// (get) Token: 0x06001826 RID: 6182 RVA: 0x00073203 File Offset: 0x00071403
		// (set) Token: 0x06001827 RID: 6183 RVA: 0x0007320B File Offset: 0x0007140B
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

		// Token: 0x06001828 RID: 6184 RVA: 0x00073233 File Offset: 0x00071433
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

		// Token: 0x06001829 RID: 6185 RVA: 0x00073253 File Offset: 0x00071453
		private void InstantiateVisualizer()
		{
			this.visualizerInstance = UnityEngine.Object.Instantiate<GameObject>(this.visualizerPrefab);
			this.OnInstantiateVisualizer();
		}

		// Token: 0x0600182A RID: 6186 RVA: 0x0007326C File Offset: 0x0007146C
		private void DestroyVisualizer()
		{
			this.OnDestroyVisualizer();
			UnityEngine.Object.Destroy(this.visualizerInstance);
			this.visualizerInstance = null;
		}

		// Token: 0x0600182B RID: 6187 RVA: 0x00073286 File Offset: 0x00071486
		public void OnInstantiateVisualizer()
		{
			this.visualizerTransform = this.visualizerInstance.transform;
			this.visualizerRenderers = this.visualizerInstance.GetComponentsInChildren<Renderer>();
			this.SetVisibleInternal(this.visible);
		}

		// Token: 0x0600182C RID: 6188 RVA: 0x000732B6 File Offset: 0x000714B6
		public virtual void OnDestroyVisualizer()
		{
			this.visualizerTransform = null;
			this.visualizerRenderers = Array.Empty<Renderer>();
		}

		// Token: 0x0600182D RID: 6189 RVA: 0x00004507 File Offset: 0x00002707
		public virtual void UpdateVisualizer()
		{
		}

		// Token: 0x0600182E RID: 6190 RVA: 0x000732CC File Offset: 0x000714CC
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

		// Token: 0x0600182F RID: 6191 RVA: 0x0007332E File Offset: 0x0007152E
		public void SetVisible(bool newVisible)
		{
			newVisible &= this.targetTransform;
			if (this.visible != newVisible)
			{
				this.SetVisibleInternal(newVisible);
			}
		}

		// Token: 0x06001830 RID: 6192 RVA: 0x00073350 File Offset: 0x00071550
		private void SetVisibleInternal(bool newVisible)
		{
			this.visible = newVisible;
			Renderer[] visualizerRenderers = this.visualizerRenderers;
			for (int i = 0; i < visualizerRenderers.Length; i++)
			{
				visualizerRenderers[i].enabled = newVisible;
			}
		}

		// Token: 0x04001B7C RID: 7036
		private GameObject _visualizerPrefab;

		// Token: 0x04001B7D RID: 7037
		public readonly GameObject owner;

		// Token: 0x04001B7E RID: 7038
		public Transform targetTransform;

		// Token: 0x04001B82 RID: 7042
		private bool _active;

		// Token: 0x04001B83 RID: 7043
		private bool visible = true;

		// Token: 0x0200043F RID: 1087
		private static class IndicatorManager
		{
			// Token: 0x06001831 RID: 6193 RVA: 0x00073382 File Offset: 0x00071582
			public static void AddIndicator([NotNull] Indicator indicator)
			{
				Indicator.IndicatorManager.runningIndicators.Add(indicator);
				Indicator.IndicatorManager.RebuildVisualizer(indicator);
			}

			// Token: 0x06001832 RID: 6194 RVA: 0x00073395 File Offset: 0x00071595
			public static void RemoveIndicator([NotNull] Indicator indicator)
			{
				indicator.SetVisualizerInstantiated(false);
				Indicator.IndicatorManager.runningIndicators.Remove(indicator);
			}

			// Token: 0x06001833 RID: 6195 RVA: 0x000733AC File Offset: 0x000715AC
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

			// Token: 0x06001834 RID: 6196 RVA: 0x0007340C File Offset: 0x0007160C
			private static void RebuildVisualizerForAll()
			{
				foreach (Indicator indicator in Indicator.IndicatorManager.runningIndicators)
				{
					Indicator.IndicatorManager.RebuildVisualizer(indicator);
				}
			}

			// Token: 0x06001835 RID: 6197 RVA: 0x0007345C File Offset: 0x0007165C
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

			// Token: 0x06001836 RID: 6198 RVA: 0x000734B8 File Offset: 0x000716B8
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

			// Token: 0x06001837 RID: 6199 RVA: 0x0007351C File Offset: 0x0007171C
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

			// Token: 0x06001838 RID: 6200 RVA: 0x000735AC File Offset: 0x000717AC
			private static void OnPostRenderUI(UICamera uiCamera)
			{
				foreach (Indicator indicator in Indicator.IndicatorManager.runningIndicators)
				{
					indicator.SetVisible(true);
				}
			}

			// Token: 0x04001B84 RID: 7044
			private static readonly List<Indicator> runningIndicators = new List<Indicator>();
		}
	}
}
