using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200060E RID: 1550
	public class PingIndicator : MonoBehaviour
	{
		// Token: 0x170003CF RID: 975
		// (get) Token: 0x060024B6 RID: 9398 RVA: 0x000A00D3 File Offset: 0x0009E2D3
		// (set) Token: 0x060024B7 RID: 9399 RVA: 0x000A00DB File Offset: 0x0009E2DB
		public Vector3 pingOrigin { get; set; }

		// Token: 0x170003D0 RID: 976
		// (get) Token: 0x060024B8 RID: 9400 RVA: 0x000A00E4 File Offset: 0x0009E2E4
		// (set) Token: 0x060024B9 RID: 9401 RVA: 0x000A00EC File Offset: 0x0009E2EC
		public Vector3 pingNormal { get; set; }

		// Token: 0x170003D1 RID: 977
		// (get) Token: 0x060024BA RID: 9402 RVA: 0x000A00F5 File Offset: 0x0009E2F5
		// (set) Token: 0x060024BB RID: 9403 RVA: 0x000A00FD File Offset: 0x0009E2FD
		public GameObject pingOwner { get; set; }

		// Token: 0x170003D2 RID: 978
		// (get) Token: 0x060024BC RID: 9404 RVA: 0x000A0106 File Offset: 0x0009E306
		// (set) Token: 0x060024BD RID: 9405 RVA: 0x000A010E File Offset: 0x0009E30E
		public GameObject pingTarget { get; set; }

		// Token: 0x060024BE RID: 9406 RVA: 0x000A0117 File Offset: 0x0009E317
		private void OnEnable()
		{
			PingIndicator.instancesList.Add(this);
		}

		// Token: 0x060024BF RID: 9407 RVA: 0x000A0124 File Offset: 0x0009E324
		private void OnDisable()
		{
			PingIndicator.instancesList.Remove(this);
		}

		// Token: 0x060024C0 RID: 9408 RVA: 0x000A0134 File Offset: 0x0009E334
		public void RebuildPing()
		{
			base.transform.rotation = Util.QuaternionSafeLookRotation(this.pingNormal);
			base.transform.position = (this.pingTarget ? this.pingTarget.transform.position : this.pingOrigin);
			base.transform.localScale = Vector3.one;
			this.positionIndicator.targetTransform = (this.pingTarget ? this.pingTarget.transform : null);
			this.positionIndicator.defaultPosition = base.transform.position;
			IDisplayNameProvider displayNameProvider = this.pingTarget ? this.pingTarget.GetComponentInParent<IDisplayNameProvider>() : null;
			ModelLocator modelLocator = null;
			this.pingType = PingIndicator.PingType.Default;
			this.pingObjectScaleCurve.enabled = false;
			this.pingObjectScaleCurve.enabled = true;
			GameObject[] array = this.defaultPingGameObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
			array = this.enemyPingGameObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
			array = this.interactablePingGameObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
			if (this.pingTarget)
			{
				Debug.LogFormat("Ping target {0}", new object[]
				{
					this.pingTarget
				});
				modelLocator = this.pingTarget.GetComponent<ModelLocator>();
				if (displayNameProvider != null)
				{
					CharacterBody component = this.pingTarget.GetComponent<CharacterBody>();
					if (component)
					{
						this.pingType = PingIndicator.PingType.Enemy;
						this.targetTransformToFollow = component.coreTransform;
					}
					else
					{
						this.pingType = PingIndicator.PingType.Interactable;
					}
				}
			}
			string bestMasterName = Util.GetBestMasterName(this.pingOwner.GetComponent<CharacterMaster>());
			string text = ((MonoBehaviour)displayNameProvider) ? Util.GetBestBodyName(((MonoBehaviour)displayNameProvider).gameObject) : "";
			this.pingText.enabled = true;
			this.pingText.text = bestMasterName;
			switch (this.pingType)
			{
			case PingIndicator.PingType.Default:
				this.pingColor = this.defaultPingColor;
				this.pingDuration = this.defaultPingDuration;
				this.pingHighlight.isOn = false;
				array = this.defaultPingGameObjects;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(true);
				}
				Chat.AddMessage(string.Format(Language.GetString("PLAYER_PING_DEFAULT"), bestMasterName));
				break;
			case PingIndicator.PingType.Enemy:
				this.pingColor = this.enemyPingColor;
				this.pingDuration = this.enemyPingDuration;
				array = this.enemyPingGameObjects;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(true);
				}
				if (modelLocator)
				{
					Transform modelTransform = modelLocator.modelTransform;
					if (modelTransform)
					{
						CharacterModel component2 = modelTransform.GetComponent<CharacterModel>();
						if (component2)
						{
							bool flag = false;
							foreach (CharacterModel.RendererInfo rendererInfo in component2.baseRendererInfos)
							{
								if (!rendererInfo.ignoreOverlays && !flag)
								{
									this.pingHighlight.highlightColor = Highlight.HighlightColor.teleporter;
									this.pingHighlight.targetRenderer = rendererInfo.renderer;
									this.pingHighlight.strength = 1f;
									this.pingHighlight.isOn = true;
									flag = true;
								}
							}
						}
					}
					Chat.AddMessage(string.Format(Language.GetString("PLAYER_PING_ENEMY"), bestMasterName, text));
				}
				break;
			case PingIndicator.PingType.Interactable:
			{
				this.pingColor = this.interactablePingColor;
				this.pingDuration = this.interactablePingDuration;
				this.pingTargetPurchaseInteraction = this.pingTarget.GetComponent<PurchaseInteraction>();
				Sprite sprite = Resources.Load<Sprite>("Textures/MiscIcons/texInventoryIconOutlined");
				SpriteRenderer component3 = this.interactablePingGameObjects[0].GetComponent<SpriteRenderer>();
				ShopTerminalBehavior component4 = this.pingTarget.GetComponent<ShopTerminalBehavior>();
				if (component4)
				{
					PickupIndex pickupIndex = component4.CurrentPickupIndex();
					text = string.Format(CultureInfo.InvariantCulture, "{0} ({1})", text, component4.pickupIndexIsHidden ? "?" : Language.GetString(pickupIndex.GetPickupNameToken()));
				}
				else if (this.pingTarget.gameObject.name.Contains("Shrine"))
				{
					sprite = Resources.Load<Sprite>("Textures/MiscIcons/texShrineIconOutlined");
				}
				else if (this.pingTarget.GetComponent<GenericPickupController>())
				{
					sprite = Resources.Load<Sprite>("Textures/MiscIcons/texLootIconOutlined");
					this.pingDuration = 60f;
				}
				else if (this.pingTarget.GetComponent<TeleporterInteraction>())
				{
					sprite = Resources.Load<Sprite>("Textures/MiscIcons/texTeleporterIconOutlined");
					this.pingDuration = 60f;
				}
				else if (this.pingTarget.GetComponent<SummonMasterBehavior>())
				{
					sprite = Resources.Load<Sprite>("Textures/MiscIcons/texDroneIconOutlined");
				}
				array = this.interactablePingGameObjects;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(true);
				}
				Renderer componentInChildren;
				if (modelLocator)
				{
					componentInChildren = modelLocator.modelTransform.GetComponentInChildren<Renderer>();
				}
				else
				{
					componentInChildren = this.pingTarget.GetComponentInChildren<Renderer>();
				}
				if (componentInChildren)
				{
					this.pingHighlight.highlightColor = Highlight.HighlightColor.interactive;
					this.pingHighlight.targetRenderer = componentInChildren;
					this.pingHighlight.strength = 1f;
					this.pingHighlight.isOn = true;
				}
				component3.sprite = sprite;
				if (this.pingTargetPurchaseInteraction && this.pingTargetPurchaseInteraction.costType != CostTypeIndex.None)
				{
					PingIndicator.sharedStringBuilder.Clear();
					CostTypeCatalog.GetCostTypeDef(this.pingTargetPurchaseInteraction.costType).BuildCostStringStyled(this.pingTargetPurchaseInteraction.cost, PingIndicator.sharedStringBuilder, false, true);
					Chat.AddMessage(string.Format(Language.GetString("PLAYER_PING_INTERACTABLE_WITH_COST"), bestMasterName, text, PingIndicator.sharedStringBuilder.ToString()));
				}
				else
				{
					Chat.AddMessage(string.Format(Language.GetString("PLAYER_PING_INTERACTABLE"), bestMasterName, text));
				}
				break;
			}
			}
			this.pingText.color = this.textBaseColor * this.pingColor;
			this.fixedTimer = this.pingDuration;
		}

		// Token: 0x060024C1 RID: 9409 RVA: 0x000A0734 File Offset: 0x0009E934
		private void Update()
		{
			if (this.pingType == PingIndicator.PingType.Interactable && this.pingTargetPurchaseInteraction && !this.pingTargetPurchaseInteraction.available)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			this.fixedTimer -= Time.deltaTime;
			if (this.fixedTimer <= 0f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x060024C2 RID: 9410 RVA: 0x000A079C File Offset: 0x0009E99C
		private void LateUpdate()
		{
			if (!this.pingTarget)
			{
				if (this.pingTarget != null)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
				return;
			}
			if (this.targetTransformToFollow)
			{
				base.transform.SetPositionAndRotation(this.targetTransformToFollow.position, this.targetTransformToFollow.rotation);
			}
		}

		// Token: 0x0400226D RID: 8813
		public PositionIndicator positionIndicator;

		// Token: 0x0400226E RID: 8814
		public TextMeshPro pingText;

		// Token: 0x0400226F RID: 8815
		public Highlight pingHighlight;

		// Token: 0x04002270 RID: 8816
		public ObjectScaleCurve pingObjectScaleCurve;

		// Token: 0x04002271 RID: 8817
		public GameObject positionIndicatorRoot;

		// Token: 0x04002272 RID: 8818
		public Color textBaseColor;

		// Token: 0x04002273 RID: 8819
		public GameObject[] defaultPingGameObjects;

		// Token: 0x04002274 RID: 8820
		public Color defaultPingColor;

		// Token: 0x04002275 RID: 8821
		public float defaultPingDuration;

		// Token: 0x04002276 RID: 8822
		public GameObject[] enemyPingGameObjects;

		// Token: 0x04002277 RID: 8823
		public Color enemyPingColor;

		// Token: 0x04002278 RID: 8824
		public float enemyPingDuration;

		// Token: 0x04002279 RID: 8825
		public GameObject[] interactablePingGameObjects;

		// Token: 0x0400227A RID: 8826
		public Color interactablePingColor;

		// Token: 0x0400227B RID: 8827
		public float interactablePingDuration;

		// Token: 0x0400227C RID: 8828
		public static List<PingIndicator> instancesList = new List<PingIndicator>();

		// Token: 0x0400227D RID: 8829
		private PingIndicator.PingType pingType;

		// Token: 0x0400227E RID: 8830
		private Color pingColor;

		// Token: 0x0400227F RID: 8831
		private float pingDuration;

		// Token: 0x04002280 RID: 8832
		private PurchaseInteraction pingTargetPurchaseInteraction;

		// Token: 0x04002285 RID: 8837
		private Transform targetTransformToFollow;

		// Token: 0x04002286 RID: 8838
		private float fixedTimer;

		// Token: 0x04002287 RID: 8839
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();

		// Token: 0x0200060F RID: 1551
		public enum PingType
		{
			// Token: 0x04002289 RID: 8841
			Default,
			// Token: 0x0400228A RID: 8842
			Enemy,
			// Token: 0x0400228B RID: 8843
			Interactable,
			// Token: 0x0400228C RID: 8844
			Count
		}
	}
}
