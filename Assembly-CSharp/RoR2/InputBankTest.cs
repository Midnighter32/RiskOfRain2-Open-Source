using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000337 RID: 823
	[RequireComponent(typeof(CharacterBody))]
	public class InputBankTest : MonoBehaviour
	{
		// Token: 0x17000172 RID: 370
		// (get) Token: 0x060010D9 RID: 4313 RVA: 0x00054302 File Offset: 0x00052502
		// (set) Token: 0x060010DA RID: 4314 RVA: 0x00054328 File Offset: 0x00052528
		public Vector3 aimDirection
		{
			get
			{
				if (!(this._aimDirection != Vector3.zero))
				{
					return base.transform.forward;
				}
				return this._aimDirection;
			}
			set
			{
				this._aimDirection = value.normalized;
			}
		}

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x060010DB RID: 4315 RVA: 0x00054337 File Offset: 0x00052537
		public Vector3 aimOrigin
		{
			get
			{
				if (!this.characterBody.aimOriginTransform)
				{
					return base.transform.position;
				}
				return this.characterBody.aimOriginTransform.position;
			}
		}

		// Token: 0x060010DC RID: 4316 RVA: 0x00054367 File Offset: 0x00052567
		private void Awake()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
		}

		// Token: 0x04001511 RID: 5393
		private CharacterBody characterBody;

		// Token: 0x04001512 RID: 5394
		public float lookPitch;

		// Token: 0x04001513 RID: 5395
		public float lookYaw;

		// Token: 0x04001514 RID: 5396
		private Vector3 _aimDirection;

		// Token: 0x04001515 RID: 5397
		public Vector3 moveVector;

		// Token: 0x04001516 RID: 5398
		public InputBankTest.ButtonState skill1;

		// Token: 0x04001517 RID: 5399
		public InputBankTest.ButtonState skill2;

		// Token: 0x04001518 RID: 5400
		public InputBankTest.ButtonState skill3;

		// Token: 0x04001519 RID: 5401
		public InputBankTest.ButtonState skill4;

		// Token: 0x0400151A RID: 5402
		public InputBankTest.ButtonState interact;

		// Token: 0x0400151B RID: 5403
		public InputBankTest.ButtonState jump;

		// Token: 0x0400151C RID: 5404
		public InputBankTest.ButtonState sprint;

		// Token: 0x0400151D RID: 5405
		public InputBankTest.ButtonState activateEquipment;

		// Token: 0x0400151E RID: 5406
		public InputBankTest.ButtonState ping;

		// Token: 0x0400151F RID: 5407
		[NonSerialized]
		public int emoteRequest = -1;

		// Token: 0x02000338 RID: 824
		public struct ButtonState
		{
			// Token: 0x17000174 RID: 372
			// (get) Token: 0x060010DE RID: 4318 RVA: 0x00054384 File Offset: 0x00052584
			public bool justReleased
			{
				get
				{
					return !this.down && this.wasDown;
				}
			}

			// Token: 0x17000175 RID: 373
			// (get) Token: 0x060010DF RID: 4319 RVA: 0x00054396 File Offset: 0x00052596
			public bool justPressed
			{
				get
				{
					return this.down && !this.wasDown;
				}
			}

			// Token: 0x060010E0 RID: 4320 RVA: 0x000543AB File Offset: 0x000525AB
			public void PushState(bool newState)
			{
				this.wasDown = this.down;
				this.down = newState;
			}

			// Token: 0x04001520 RID: 5408
			public bool down;

			// Token: 0x04001521 RID: 5409
			public bool wasDown;
		}
	}
}
