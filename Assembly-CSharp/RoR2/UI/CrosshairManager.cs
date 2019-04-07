using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005CB RID: 1483
	[RequireComponent(typeof(RectTransform))]
	public class CrosshairManager : MonoBehaviour
	{
		// Token: 0x06002149 RID: 8521 RVA: 0x0009C501 File Offset: 0x0009A701
		private void OnEnable()
		{
			CrosshairManager.instancesList.Add(this);
		}

		// Token: 0x0600214A RID: 8522 RVA: 0x0009C50E File Offset: 0x0009A70E
		private void OnDisable()
		{
			CrosshairManager.instancesList.Remove(this);
		}

		// Token: 0x0600214B RID: 8523 RVA: 0x0009C51C File Offset: 0x0009A71C
		private static void StaticLateUpdate()
		{
			for (int i = 0; i < CrosshairManager.instancesList.Count; i++)
			{
				CrosshairManager.instancesList[i].DoLateUpdate();
			}
		}

		// Token: 0x0600214C RID: 8524 RVA: 0x0009C550 File Offset: 0x0009A750
		private void DoLateUpdate()
		{
			if (this.cameraRigController)
			{
				this.UpdateCrosshair(this.cameraRigController.target ? this.cameraRigController.target.GetComponent<CharacterBody>() : null, this.cameraRigController.crosshairWorldPosition, this.cameraRigController.uiCam);
			}
			this.UpdateHitMarker();
		}

		// Token: 0x0600214D RID: 8525 RVA: 0x0009C5B4 File Offset: 0x0009A7B4
		private void UpdateCrosshair(CharacterBody targetBody, Vector3 crosshairWorldPosition, Camera uiCamera)
		{
			GameObject gameObject = null;
			if (!this.cameraRigController.hasOverride && targetBody && targetBody.healthComponent.alive)
			{
				if (!targetBody.isSprinting)
				{
					gameObject = (targetBody.hideCrosshair ? null : targetBody.crosshairPrefab);
				}
				else
				{
					gameObject = Resources.Load<GameObject>("Prefabs/Crosshair/SprintingCrosshair");
				}
			}
			if (gameObject != this.currentCrosshairPrefab)
			{
				if (this.crosshairController)
				{
					UnityEngine.Object.Destroy(this.crosshairController.gameObject);
					this.crosshairController = null;
				}
				if (gameObject)
				{
					GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject, this.container);
					this.crosshairController = gameObject2.GetComponent<CrosshairController>();
					this.crosshairHudElement = gameObject2.GetComponent<HudElement>();
				}
				this.currentCrosshairPrefab = gameObject;
			}
			if (this.crosshairController)
			{
				((RectTransform)this.crosshairController.gameObject.transform).anchoredPosition = new Vector2(0.5f, 0.5f);
			}
			if (this.crosshairHudElement)
			{
				this.crosshairHudElement.targetCharacterBody = targetBody;
			}
		}

		// Token: 0x0600214E RID: 8526 RVA: 0x0009C6C4 File Offset: 0x0009A8C4
		public void RefreshHitmarker(bool crit)
		{
			this.hitmarkerTimer = 0.2f;
			this.hitmarker.gameObject.SetActive(false);
			this.hitmarker.gameObject.SetActive(true);
			Util.PlaySound("Play_UI_hit", RoR2Application.instance.gameObject);
			if (crit)
			{
				Util.PlaySound("Play_UI_crit", RoR2Application.instance.gameObject);
			}
		}

		// Token: 0x0600214F RID: 8527 RVA: 0x0009C72C File Offset: 0x0009A92C
		private void UpdateHitMarker()
		{
			this.hitmarkerAlpha = Mathf.Pow(this.hitmarkerTimer / 0.2f, 0.75f);
			this.hitmarkerTimer = Mathf.Max(0f, this.hitmarkerTimer - Time.deltaTime);
			if (this.hitmarker)
			{
				Color color = this.hitmarker.color;
				color.a = this.hitmarkerAlpha;
				this.hitmarker.color = color;
			}
		}

		// Token: 0x06002150 RID: 8528 RVA: 0x0009C7A4 File Offset: 0x0009A9A4
		private static void HandleHitMarker(DamageDealtMessage damageDealtMessage)
		{
			for (int i = 0; i < CrosshairManager.instancesList.Count; i++)
			{
				CrosshairManager crosshairManager = CrosshairManager.instancesList[i];
				if (crosshairManager.cameraRigController && damageDealtMessage.attacker == crosshairManager.cameraRigController.target)
				{
					crosshairManager.RefreshHitmarker(damageDealtMessage.crit);
				}
			}
		}

		// Token: 0x06002151 RID: 8529 RVA: 0x0009C803 File Offset: 0x0009AA03
		static CrosshairManager()
		{
			GlobalEventManager.onClientDamageNotified += CrosshairManager.HandleHitMarker;
			RoR2Application.onLateUpdate += CrosshairManager.StaticLateUpdate;
		}

		// Token: 0x040023D5 RID: 9173
		[Tooltip("The transform which should act as the container for the crosshair.")]
		public RectTransform container;

		// Token: 0x040023D6 RID: 9174
		public CameraRigController cameraRigController;

		// Token: 0x040023D7 RID: 9175
		[Tooltip("The hitmarker image.")]
		public Image hitmarker;

		// Token: 0x040023D8 RID: 9176
		private float hitmarkerAlpha;

		// Token: 0x040023D9 RID: 9177
		private float hitmarkerTimer;

		// Token: 0x040023DA RID: 9178
		private const float hitmarkerDuration = 0.2f;

		// Token: 0x040023DB RID: 9179
		private GameObject currentCrosshairPrefab;

		// Token: 0x040023DC RID: 9180
		private CrosshairController crosshairController;

		// Token: 0x040023DD RID: 9181
		private HudElement crosshairHudElement;

		// Token: 0x040023DE RID: 9182
		private static readonly List<CrosshairManager> instancesList = new List<CrosshairManager>();
	}
}
