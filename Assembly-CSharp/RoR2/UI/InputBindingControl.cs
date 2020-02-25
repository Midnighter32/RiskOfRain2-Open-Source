using System;
using System.Collections.Generic;
using System.Linq;
using Rewired;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005D3 RID: 1491
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class InputBindingControl : MonoBehaviour
	{
		// Token: 0x06002345 RID: 9029 RVA: 0x0009A214 File Offset: 0x00098414
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

		// Token: 0x170003AF RID: 943
		// (get) Token: 0x06002346 RID: 9030 RVA: 0x0009A29E File Offset: 0x0009849E
		private bool isListening
		{
			get
			{
				return this.inputMapperHelper.isListening;
			}
		}

		// Token: 0x06002347 RID: 9031 RVA: 0x0009A2AB File Offset: 0x000984AB
		public void ToggleListening()
		{
			if (!this.isListening)
			{
				this.StartListening();
				return;
			}
			this.StopListening();
		}

		// Token: 0x06002348 RID: 9032 RVA: 0x0009A2C4 File Offset: 0x000984C4
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

		// Token: 0x06002349 RID: 9033 RVA: 0x0009A39C File Offset: 0x0009859C
		private void StopListening()
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			this.currentPlayer = null;
			this.inputMapperHelper.Stop();
		}

		// Token: 0x0600234A RID: 9034 RVA: 0x0009A3B9 File Offset: 0x000985B9
		private void OnEnable()
		{
			if (!this.eventSystemLocator.eventSystem)
			{
				base.enabled = false;
				return;
			}
			this.inputMapperHelper = this.eventSystemLocator.eventSystem.inputMapperHelper;
		}

		// Token: 0x0600234B RID: 9035 RVA: 0x0009A3EB File Offset: 0x000985EB
		private void OnDisable()
		{
			this.StopListening();
		}

		// Token: 0x0600234C RID: 9036 RVA: 0x0009A3F4 File Offset: 0x000985F4
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

		// Token: 0x04002129 RID: 8489
		public string actionName;

		// Token: 0x0400212A RID: 8490
		public AxisRange axisRange;

		// Token: 0x0400212B RID: 8491
		public LanguageTextMeshController nameLabel;

		// Token: 0x0400212C RID: 8492
		public InputBindingDisplayController bindingDisplay;

		// Token: 0x0400212D RID: 8493
		public MPEventSystem.InputSource inputSource;

		// Token: 0x0400212E RID: 8494
		public MPButton button;

		// Token: 0x0400212F RID: 8495
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04002130 RID: 8496
		private InputAction action;

		// Token: 0x04002131 RID: 8497
		private InputMapperHelper inputMapperHelper;

		// Token: 0x04002132 RID: 8498
		private Player currentPlayer;

		// Token: 0x04002133 RID: 8499
		private float buttonReactivationTime;
	}
}
