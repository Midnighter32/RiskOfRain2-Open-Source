using System;
using System.Collections.Generic;
using RoR2.WwiseUtils;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005AA RID: 1450
	[RequireComponent(typeof(RectTransform))]
	public class CrosshairManager : MonoBehaviour
	{
		// Token: 0x06002279 RID: 8825 RVA: 0x0009538F File Offset: 0x0009358F
		private void OnEnable()
		{
			CrosshairManager.instancesList.Add(this);
			this.rtpcDamageDirection = new RtpcSetter("damageDirection", RoR2Application.instance.gameObject);
		}

		// Token: 0x0600227A RID: 8826 RVA: 0x000953B6 File Offset: 0x000935B6
		private void OnDisable()
		{
			CrosshairManager.instancesList.Remove(this);
		}

		// Token: 0x0600227B RID: 8827 RVA: 0x000953C4 File Offset: 0x000935C4
		private static void StaticLateUpdate()
		{
			for (int i = 0; i < CrosshairManager.instancesList.Count; i++)
			{
				CrosshairManager.instancesList[i].DoLateUpdate();
			}
		}

		// Token: 0x0600227C RID: 8828 RVA: 0x000953F8 File Offset: 0x000935F8
		private void DoLateUpdate()
		{
			if (this.cameraRigController)
			{
				this.UpdateCrosshair(this.cameraRigController.target ? this.cameraRigController.target.GetComponent<CharacterBody>() : null, this.cameraRigController.crosshairWorldPosition, this.cameraRigController.uiCam);
			}
			this.UpdateHitMarker();
		}

		// Token: 0x0600227D RID: 8829 RVA: 0x0009545C File Offset: 0x0009365C
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

		// Token: 0x0600227E RID: 8830 RVA: 0x0009556C File Offset: 0x0009376C
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

		// Token: 0x0600227F RID: 8831 RVA: 0x000955D4 File Offset: 0x000937D4
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

		// Token: 0x06002280 RID: 8832 RVA: 0x0009564C File Offset: 0x0009384C
		private static void HandleHitMarker(DamageDealtMessage damageDealtMessage)
		{
			for (int i = 0; i < CrosshairManager.instancesList.Count; i++)
			{
				CrosshairManager crosshairManager = CrosshairManager.instancesList[i];
				if (crosshairManager.cameraRigController)
				{
					GameObject target = crosshairManager.cameraRigController.target;
					if (damageDealtMessage.attacker == target)
					{
						crosshairManager.RefreshHitmarker(damageDealtMessage.crit);
					}
					else if (damageDealtMessage.victim == target)
					{
						Transform transform = crosshairManager.cameraRigController.transform;
						Vector3 position = transform.position;
						Vector3 forward = transform.forward;
						Vector3 position2 = transform.position;
						Vector3 vector = damageDealtMessage.position - position;
						float num = Vector2.SignedAngle(new Vector2(vector.x, vector.z), new Vector2(forward.x, forward.z));
						if (num < 0f)
						{
							num += 360f;
						}
						crosshairManager.rtpcDamageDirection.value = num;
						Util.PlaySound("Play_UI_takeDamage", RoR2Application.instance.gameObject);
					}
				}
			}
		}

		// Token: 0x06002281 RID: 8833 RVA: 0x0009575C File Offset: 0x0009395C
		static CrosshairManager()
		{
			GlobalEventManager.onClientDamageNotified += CrosshairManager.HandleHitMarker;
			RoR2Application.onLateUpdate += CrosshairManager.StaticLateUpdate;
		}

		// Token: 0x04001FE2 RID: 8162
		[Tooltip("The transform which should act as the container for the crosshair.")]
		public RectTransform container;

		// Token: 0x04001FE3 RID: 8163
		public CameraRigController cameraRigController;

		// Token: 0x04001FE4 RID: 8164
		[Tooltip("The hitmarker image.")]
		public Image hitmarker;

		// Token: 0x04001FE5 RID: 8165
		private float hitmarkerAlpha;

		// Token: 0x04001FE6 RID: 8166
		private float hitmarkerTimer;

		// Token: 0x04001FE7 RID: 8167
		private const float hitmarkerDuration = 0.2f;

		// Token: 0x04001FE8 RID: 8168
		private GameObject currentCrosshairPrefab;

		// Token: 0x04001FE9 RID: 8169
		private CrosshairController crosshairController;

		// Token: 0x04001FEA RID: 8170
		private HudElement crosshairHudElement;

		// Token: 0x04001FEB RID: 8171
		private RtpcSetter rtpcDamageDirection;

		// Token: 0x04001FEC RID: 8172
		private static readonly List<CrosshairManager> instancesList = new List<CrosshairManager>();
	}
}
