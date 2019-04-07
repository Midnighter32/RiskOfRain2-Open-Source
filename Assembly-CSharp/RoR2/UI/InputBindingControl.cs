using System;
using System.Collections.Generic;
using System.Linq;
using Rewired;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005EE RID: 1518
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class InputBindingControl : MonoBehaviour
	{
		// Token: 0x06002202 RID: 8706 RVA: 0x000A0DBC File Offset: 0x0009EFBC
		public void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			this.bindingDisplay.actionName = this.actionName;
			this.bindingDisplay.useExplicitInputSource = true;
			this.bindingDisplay.explicitInputSource = this.inputSource;
			this.bindingDisplay.axisRange = this.axisRange;
			this.nameLabel.token = InputCatalog.GetActionNameToken(this.actionName, this.axisRange);
			this.action = ReInput.mapping.GetAction(this.actionName);
		}

		// Token: 0x170002FA RID: 762
		// (get) Token: 0x06002203 RID: 8707 RVA: 0x000A0E46 File Offset: 0x0009F046
		private bool isListening
		{
			get
			{
				return this.inputMapperHelper.isListening;
			}
		}

		// Token: 0x06002204 RID: 8708 RVA: 0x000A0E53 File Offset: 0x0009F053
		public void ToggleListening()
		{
			if (!this.isListening)
			{
				this.StartListening();
				return;
			}
			this.StopListening();
		}

		// Token: 0x06002205 RID: 8709 RVA: 0x000A0E6C File Offset: 0x0009F06C
		public void StartListening()
		{
			this.inputMapperHelper.Stop();
			MPEventSystem eventSystem = this.eventSystemLocator.eventSystem;
			Player player;
			if (eventSystem == null)
			{
				player = null;
			}
			else
			{
				LocalUser localUser = eventSystem.localUser;
				player = ((localUser != null) ? localUser.inputPlayer : null);
			}
			this.currentPlayer = player;
			if (this.currentPlayer == null)
			{
				return;
			}
			IList<Controller> controllers = null;
			MPEventSystem.InputSource inputSource = this.inputSource;
			if (inputSource != MPEventSystem.InputSource.Keyboard)
			{
				if (inputSource == MPEventSystem.InputSource.Gamepad)
				{
					controllers = this.currentPlayer.controllers.Joysticks.ToArray<Joystick>();
				}
			}
			else
			{
				controllers = new Controller[]
				{
					this.currentPlayer.controllers.Keyboard,
					this.currentPlayer.controllers.Mouse
				};
			}
			this.inputMapperHelper.Start(this.currentPlayer, controllers, this.action, this.axisRange);
			if (this.button)
			{
				this.button.interactable = false;
			}
		}

		// Token: 0x06002206 RID: 8710 RVA: 0x000A0F44 File Offset: 0x0009F144
		private void StopListening()
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			this.currentPlayer = null;
			this.inputMapperHelper.Stop();
		}

		// Token: 0x06002207 RID: 8711 RVA: 0x000A0F61 File Offset: 0x0009F161
		private void OnEnable()
		{
			if (!this.eventSystemLocator.eventSystem)
			{
				base.enabled = false;
				return;
			}
			this.inputMapperHelper = this.eventSystemLocator.eventSystem.inputMapperHelper;
		}

		// Token: 0x06002208 RID: 8712 RVA: 0x000A0F93 File Offset: 0x0009F193
		private void OnDisable()
		{
			this.StopListening();
		}

		// Token: 0x06002209 RID: 8713 RVA: 0x000A0F9C File Offset: 0x0009F19C
		private void Update()
		{
			if (this.button)
			{
				bool flag = !this.eventSystemLocator.eventSystem.inputMapperHelper.isListening;
				if (!flag)
				{
					this.buttonReactivationTime = Time.unscaledTime + 0.25f;
				}
				this.button.interactable = (flag && this.buttonReactivationTime <= Time.unscaledTime);
			}
		}

		// Token: 0x04002507 RID: 9479
		public string actionName;

		// Token: 0x04002508 RID: 9480
		public AxisRange axisRange;

		// Token: 0x04002509 RID: 9481
		public LanguageTextMeshController nameLabel;

		// Token: 0x0400250A RID: 9482
		public InputBindingDisplayController bindingDisplay;

		// Token: 0x0400250B RID: 9483
		public MPEventSystem.InputSource inputSource;

		// Token: 0x0400250C RID: 9484
		public MPButton button;

		// Token: 0x0400250D RID: 9485
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x0400250E RID: 9486
		private InputAction action;

		// Token: 0x0400250F RID: 9487
		private InputMapperHelper inputMapperHelper;

		// Token: 0x04002510 RID: 9488
		private Player currentPlayer;

		// Token: 0x04002511 RID: 9489
		private float buttonReactivationTime;
	}
}
