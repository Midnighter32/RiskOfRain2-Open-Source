using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200061F RID: 1567
	public class PingIndicator : MonoBehaviour
	{
		// Token: 0x06002340 RID: 9024 RVA: 0x000A5DF0 File Offset: 0x000A3FF0
		private void OnEnable()
		{
			PingIndicator.instancesList.Add(this);
		}

		// Token: 0x06002341 RID: 9025 RVA: 0x000A5DFD File Offset: 0x000A3FFD
		private void OnDisable()
		{
			PingIndicator.instancesList.Remove(this);
		}

		// Token: 0x06002342 RID: 9026 RVA: 0x000A5E0C File Offset: 0x000A400C
		public void RebuildPing()
		{
			base.transform.rotation = Util.QuaternionSafeLookRotation(this.pingNormal);
			base.transform.parent = (this.pingTarget ? this.pingTarget.transform : null);
			base.transform.position = (this.pingTarget ? this.pingTarget.transform.position : this.pingOrigin);
			this.positionIndicator.targetTransform = (this.pingTarget ? this.pingTarget.transform : null);
			this.positionIndicator.defaultPosition = base.transform.position;
			IDisplayNameProvider componentInParent = base.GetComponentInParent<IDisplayNameProvider>();
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
				if (componentInParent != null)
				{
					CharacterBody component = this.pingTarget.GetComponent<CharacterBody>();
					if (component)
					{
						this.pingType = PingIndicator.PingType.Enemy;
						base.transform.parent = component.coreTransform;
						base.transform.position = component.coreTransform.position;
					}
					else
					{
						this.pingType = PingIndicator.PingType.Interactable;
					}
				}
			}
			string displayName = this.pingOwner.GetComponent<PlayerCharacterMasterController>().GetDisplayName();
			string text = (componentInParent != null) ? componentInParent.GetDisplayName() : "";
			this.pingText.enabled = true;
			this.pingText.text = displayName;
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
				Chat.AddMessage(string.Format(Language.GetString("PLAYER_PING_DEFAULT"), displayName));
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
							foreach (CharacterModel.RendererInfo rendererInfo in component2.rendererInfos)
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
					Chat.AddMessage(string.Format(Language.GetString("PLAYER_PING_ENEMY"), displayName, text));
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
				base.transform.parent.GetComponentInChildren<Renderer>();
				Renderer componentInChildren;
				if (modelLocator)
				{
					componentInChildren = modelLocator.modelTransform.GetComponentInChildren<Renderer>();
				}
				else
				{
					componentInChildren = base.transform.parent.GetComponentInChildren<Renderer>();
				}
				if (componentInChildren)
				{
					this.pingHighlight.highlightColor = Highlight.HighlightColor.interactive;
					this.pingHighlight.targetRenderer = componentInChildren;
					this.pingHighlight.strength = 1f;
					this.pingHighlight.isOn = true;
				}
				component3.sprite = sprite;
				Chat.AddMessage(string.Format(Language.GetString("PLAYER_PING_INTERACTABLE"), displayName, text));
				break;
			}
			}
			this.pingText.color = this.textBaseColor * this.pingColor;
			this.fixedTimer = this.pingDuration;
		}

		// Token: 0x06002343 RID: 9027 RVA: 0x000A63BC File Offset: 0x000A45BC
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

		// Token: 0x04002626 RID: 9766
		public PositionIndicator positionIndicator;

		// Token: 0x04002627 RID: 9767
		public TextMeshPro pingText;

		// Token: 0x04002628 RID: 9768
		public Highlight pingHighlight;

		// Token: 0x04002629 RID: 9769
		public ObjectScaleCurve pingObjectScaleCurve;

		// Token: 0x0400262A RID: 9770
		public GameObject positionIndicatorRoot;

		// Token: 0x0400262B RID: 9771
		public Color textBaseColor;

		// Token: 0x0400262C RID: 9772
		public GameObject[] defaultPingGameObjects;

		// Token: 0x0400262D RID: 9773
		public Color defaultPingColor;

		// Token: 0x0400262E RID: 9774
		public float defaultPingDuration;

		// Token: 0x0400262F RID: 9775
		public GameObject[] enemyPingGameObjects;

		// Token: 0x04002630 RID: 9776
		public Color enemyPingColor;

		// Token: 0x04002631 RID: 9777
		public float enemyPingDuration;

		// Token: 0x04002632 RID: 9778
		public GameObject[] interactablePingGameObjects;

		// Token: 0x04002633 RID: 9779
		public Color interactablePingColor;

		// Token: 0x04002634 RID: 9780
		public float interactablePingDuration;

		// Token: 0x04002635 RID: 9781
		public static List<PingIndicator> instancesList = new List<PingIndicator>();

		// Token: 0x04002636 RID: 9782
		private PingIndicator.PingType pingType;

		// Token: 0x04002637 RID: 9783
		private Color pingColor;

		// Token: 0x04002638 RID: 9784
		private float pingDuration;

		// Token: 0x04002639 RID: 9785
		private PurchaseInteraction pingTargetPurchaseInteraction;

		// Token: 0x0400263A RID: 9786
		[HideInInspector]
		public Vector3 pingOrigin;

		// Token: 0x0400263B RID: 9787
		[HideInInspector]
		public Vector3 pingNormal;

		// Token: 0x0400263C RID: 9788
		[HideInInspector]
		public GameObject pingOwner;

		// Token: 0x0400263D RID: 9789
		[HideInInspector]
		public GameObject pingTarget;

		// Token: 0x0400263E RID: 9790
		private float fixedTimer;

		// Token: 0x02000620 RID: 1568
		public enum PingType
		{
			// Token: 0x04002640 RID: 9792
			Default,
			// Token: 0x04002641 RID: 9793
			Enemy,
			// Token: 0x04002642 RID: 9794
			Interactable,
			// Token: 0x04002643 RID: 9795
			Count
		}
	}
}
