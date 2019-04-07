using System;
using Rewired;
using Rewired.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace RoR2.UI
{
	// Token: 0x020005A8 RID: 1448
	public class MPInput : BaseInput, IMouseInputSource
	{
		// Token: 0x06002067 RID: 8295 RVA: 0x00098D9E File Offset: 0x00096F9E
		protected override void Awake()
		{
			base.Awake();
			this.eventSystem = base.GetComponent<MPEventSystem>();
		}

		// Token: 0x06002068 RID: 8296 RVA: 0x00098DB2 File Offset: 0x00096FB2
		private static int MouseButtonToAction(int button)
		{
			switch (button)
			{
			case 0:
				return 20;
			case 1:
				return 21;
			case 2:
				return 22;
			default:
				return -1;
			}
		}

		// Token: 0x06002069 RID: 8297 RVA: 0x00098DD2 File Offset: 0x00096FD2
		public override bool GetMouseButtonDown(int button)
		{
			return this.player.GetButtonDown(MPInput.MouseButtonToAction(button));
		}

		// Token: 0x0600206A RID: 8298 RVA: 0x00098DE5 File Offset: 0x00096FE5
		public override bool GetMouseButtonUp(int button)
		{
			return this.player.GetButtonUp(MPInput.MouseButtonToAction(button));
		}

		// Token: 0x0600206B RID: 8299 RVA: 0x00098DF8 File Offset: 0x00096FF8
		public override bool GetMouseButton(int button)
		{
			return this.player.GetButton(MPInput.MouseButtonToAction(button));
		}

		// Token: 0x0600206C RID: 8300 RVA: 0x00098E0B File Offset: 0x0009700B
		public void CenterCursor()
		{
			this.internalMousePosition = new Vector2((float)Screen.width * 0.5f, (float)Screen.height * 0.5f);
		}

		// Token: 0x0600206D RID: 8301 RVA: 0x00098E30 File Offset: 0x00097030
		public void Update()
		{
			if (!this.eventSystem.isCursorVisible)
			{
				return;
			}
			float num = (float)Screen.width;
			float num2 = (float)Screen.height;
			float num3 = Mathf.Min(num / 1920f, num2 / 1080f);
			this.internalScreenPositionDelta = Vector2.zero;
			if (this.eventSystem.currentInputSource == MPEventSystem.InputSource.Keyboard)
			{
				if (Application.isFocused)
				{
					this.internalMousePosition = Input.mousePosition;
				}
			}
			else
			{
				Vector2 a = new Vector2(this.player.GetAxis(23), this.player.GetAxis(24));
				float magnitude = a.magnitude;
				this.stickMagnitude = Mathf.Min(Mathf.MoveTowards(this.stickMagnitude, magnitude, this.cursorAcceleration * Time.unscaledDeltaTime), magnitude);
				float num4 = this.stickMagnitude;
				if (this.eventSystem.isHovering)
				{
					num4 *= this.cursorStickyModifier;
				}
				Vector2 a2 = (magnitude == 0f) ? Vector2.zero : (a * (num4 / magnitude));
				float d = 1920f * this.cursorScreenSpeed * num3;
				this.internalScreenPositionDelta = a2 * Time.unscaledDeltaTime * d;
				this.internalMousePosition += this.internalScreenPositionDelta;
			}
			this.internalMousePosition.x = Mathf.Clamp(this.internalMousePosition.x, 0f, num);
			this.internalMousePosition.y = Mathf.Clamp(this.internalMousePosition.y, 0f, num2);
			this._scrollDelta = new Vector2(0f, this.player.GetAxis(26));
		}

		// Token: 0x170002D8 RID: 728
		// (get) Token: 0x0600206E RID: 8302 RVA: 0x00098FD0 File Offset: 0x000971D0
		public override Vector2 mousePosition
		{
			get
			{
				return this.internalMousePosition;
			}
		}

		// Token: 0x170002D9 RID: 729
		// (get) Token: 0x0600206F RID: 8303 RVA: 0x00098FD8 File Offset: 0x000971D8
		public override Vector2 mouseScrollDelta
		{
			get
			{
				return this._scrollDelta;
			}
		}

		// Token: 0x06002070 RID: 8304 RVA: 0x00098FE0 File Offset: 0x000971E0
		public bool GetButtonDown(int button)
		{
			return this.GetMouseButtonDown(button);
		}

		// Token: 0x06002071 RID: 8305 RVA: 0x00098FE9 File Offset: 0x000971E9
		public bool GetButtonUp(int button)
		{
			return this.GetMouseButtonUp(button);
		}

		// Token: 0x06002072 RID: 8306 RVA: 0x00098FF2 File Offset: 0x000971F2
		public bool GetButton(int button)
		{
			return this.GetMouseButton(button);
		}

		// Token: 0x170002DA RID: 730
		// (get) Token: 0x06002073 RID: 8307 RVA: 0x00098FFB File Offset: 0x000971FB
		public int playerId
		{
			get
			{
				return this.player.id;
			}
		}

		// Token: 0x170002DB RID: 731
		// (get) Token: 0x06002074 RID: 8308 RVA: 0x00099008 File Offset: 0x00097208
		public bool locked
		{
			get
			{
				return !this.eventSystem.isCursorVisible;
			}
		}

		// Token: 0x170002DC RID: 732
		// (get) Token: 0x06002075 RID: 8309 RVA: 0x0000BB2B File Offset: 0x00009D2B
		public int buttonCount
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x170002DD RID: 733
		// (get) Token: 0x06002076 RID: 8310 RVA: 0x00098FD0 File Offset: 0x000971D0
		public Vector2 screenPosition
		{
			get
			{
				return this.internalMousePosition;
			}
		}

		// Token: 0x170002DE RID: 734
		// (get) Token: 0x06002077 RID: 8311 RVA: 0x00099018 File Offset: 0x00097218
		public Vector2 screenPositionDelta
		{
			get
			{
				return this.internalScreenPositionDelta;
			}
		}

		// Token: 0x170002DF RID: 735
		// (get) Token: 0x06002078 RID: 8312 RVA: 0x00098FD8 File Offset: 0x000971D8
		public Vector2 wheelDelta
		{
			get
			{
				return this._scrollDelta;
			}
		}

		// Token: 0x0600207A RID: 8314 RVA: 0x00099050 File Offset: 0x00097250
		bool IMouseInputSource.get_enabled()
		{
			return base.enabled;
		}

		// Token: 0x04002301 RID: 8961
		public Player player;

		// Token: 0x04002302 RID: 8962
		private MPEventSystem eventSystem;

		// Token: 0x04002303 RID: 8963
		[FormerlySerializedAs("useAcceleration")]
		public bool useCursorAcceleration = true;

		// Token: 0x04002304 RID: 8964
		[FormerlySerializedAs("acceleration")]
		public float cursorAcceleration = 8f;

		// Token: 0x04002305 RID: 8965
		public float cursorStickyModifier = 0.33333334f;

		// Token: 0x04002306 RID: 8966
		public float cursorScreenSpeed = 0.75f;

		// Token: 0x04002307 RID: 8967
		private float stickMagnitude;

		// Token: 0x04002308 RID: 8968
		private Vector2 _scrollDelta;

		// Token: 0x04002309 RID: 8969
		private Vector2 internalScreenPositionDelta;

		// Token: 0x0400230A RID: 8970
		public Vector2 internalMousePosition;
	}
}
