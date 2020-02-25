using System;
using Rewired;
using Rewired.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace RoR2.UI
{
	// Token: 0x0200057B RID: 1403
	public class MPInput : BaseInput, IMouseInputSource
	{
		// Token: 0x06002157 RID: 8535 RVA: 0x00090956 File Offset: 0x0008EB56
		protected override void Awake()
		{
			base.Awake();
			this.eventSystem = base.GetComponent<MPEventSystem>();
		}

		// Token: 0x06002158 RID: 8536 RVA: 0x0009096A File Offset: 0x0008EB6A
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

		// Token: 0x06002159 RID: 8537 RVA: 0x0009098A File Offset: 0x0008EB8A
		public override bool GetMouseButtonDown(int button)
		{
			return this.player.GetButtonDown(MPInput.MouseButtonToAction(button));
		}

		// Token: 0x0600215A RID: 8538 RVA: 0x0009099D File Offset: 0x0008EB9D
		public override bool GetMouseButtonUp(int button)
		{
			return this.player.GetButtonUp(MPInput.MouseButtonToAction(button));
		}

		// Token: 0x0600215B RID: 8539 RVA: 0x000909B0 File Offset: 0x0008EBB0
		public override bool GetMouseButton(int button)
		{
			return this.player.GetButton(MPInput.MouseButtonToAction(button));
		}

		// Token: 0x0600215C RID: 8540 RVA: 0x000909C3 File Offset: 0x0008EBC3
		public void CenterCursor()
		{
			this.internalMousePosition = new Vector2((float)Screen.width * 0.5f, (float)Screen.height * 0.5f);
		}

		// Token: 0x0600215D RID: 8541 RVA: 0x000909E8 File Offset: 0x0008EBE8
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

		// Token: 0x17000385 RID: 901
		// (get) Token: 0x0600215E RID: 8542 RVA: 0x00090B88 File Offset: 0x0008ED88
		public override Vector2 mousePosition
		{
			get
			{
				return this.internalMousePosition;
			}
		}

		// Token: 0x17000386 RID: 902
		// (get) Token: 0x0600215F RID: 8543 RVA: 0x00090B90 File Offset: 0x0008ED90
		public override Vector2 mouseScrollDelta
		{
			get
			{
				return this._scrollDelta;
			}
		}

		// Token: 0x06002160 RID: 8544 RVA: 0x00090B98 File Offset: 0x0008ED98
		public bool GetButtonDown(int button)
		{
			return this.GetMouseButtonDown(button);
		}

		// Token: 0x06002161 RID: 8545 RVA: 0x00090BA1 File Offset: 0x0008EDA1
		public bool GetButtonUp(int button)
		{
			return this.GetMouseButtonUp(button);
		}

		// Token: 0x06002162 RID: 8546 RVA: 0x00090BAA File Offset: 0x0008EDAA
		public bool GetButton(int button)
		{
			return this.GetMouseButton(button);
		}

		// Token: 0x17000387 RID: 903
		// (get) Token: 0x06002163 RID: 8547 RVA: 0x00090BB3 File Offset: 0x0008EDB3
		public int playerId
		{
			get
			{
				return this.player.id;
			}
		}

		// Token: 0x17000388 RID: 904
		// (get) Token: 0x06002164 RID: 8548 RVA: 0x00090BC0 File Offset: 0x0008EDC0
		public bool locked
		{
			get
			{
				return !this.eventSystem.isCursorVisible;
			}
		}

		// Token: 0x17000389 RID: 905
		// (get) Token: 0x06002165 RID: 8549 RVA: 0x0000C5D3 File Offset: 0x0000A7D3
		public int buttonCount
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x1700038A RID: 906
		// (get) Token: 0x06002166 RID: 8550 RVA: 0x00090B88 File Offset: 0x0008ED88
		public Vector2 screenPosition
		{
			get
			{
				return this.internalMousePosition;
			}
		}

		// Token: 0x1700038B RID: 907
		// (get) Token: 0x06002167 RID: 8551 RVA: 0x00090BD0 File Offset: 0x0008EDD0
		public Vector2 screenPositionDelta
		{
			get
			{
				return this.internalScreenPositionDelta;
			}
		}

		// Token: 0x1700038C RID: 908
		// (get) Token: 0x06002168 RID: 8552 RVA: 0x00090B90 File Offset: 0x0008ED90
		public Vector2 wheelDelta
		{
			get
			{
				return this._scrollDelta;
			}
		}

		// Token: 0x0600216A RID: 8554 RVA: 0x00090C08 File Offset: 0x0008EE08
		bool IMouseInputSource.get_enabled()
		{
			return base.enabled;
		}

		// Token: 0x04001ED4 RID: 7892
		public Player player;

		// Token: 0x04001ED5 RID: 7893
		private MPEventSystem eventSystem;

		// Token: 0x04001ED6 RID: 7894
		[FormerlySerializedAs("useAcceleration")]
		public bool useCursorAcceleration = true;

		// Token: 0x04001ED7 RID: 7895
		[FormerlySerializedAs("acceleration")]
		public float cursorAcceleration = 8f;

		// Token: 0x04001ED8 RID: 7896
		public float cursorStickyModifier = 0.33333334f;

		// Token: 0x04001ED9 RID: 7897
		public float cursorScreenSpeed = 0.75f;

		// Token: 0x04001EDA RID: 7898
		private float stickMagnitude;

		// Token: 0x04001EDB RID: 7899
		private Vector2 _scrollDelta;

		// Token: 0x04001EDC RID: 7900
		private Vector2 internalScreenPositionDelta;

		// Token: 0x04001EDD RID: 7901
		public Vector2 internalMousePosition;
	}
}
