using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200025A RID: 602
	[RequireComponent(typeof(CharacterBody))]
	public class InputBankTest : MonoBehaviour
	{
		// Token: 0x170001AD RID: 429
		// (get) Token: 0x06000D28 RID: 3368 RVA: 0x0003B32B File Offset: 0x0003952B
		// (set) Token: 0x06000D29 RID: 3369 RVA: 0x0003B351 File Offset: 0x00039551
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

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x06000D2A RID: 3370 RVA: 0x0003B360 File Offset: 0x00039560
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

		// Token: 0x06000D2B RID: 3371 RVA: 0x0003B390 File Offset: 0x00039590
		public Ray GetAimRay()
		{
			return new Ray(this.aimOrigin, this.aimDirection);
		}

		// Token: 0x06000D2C RID: 3372 RVA: 0x0003B3A4 File Offset: 0x000395A4
		public bool GetAimRaycast(float maxDistance, out RaycastHit hitInfo)
		{
			float time = Time.time;
			float fixedTime = Time.fixedTime;
			if (!this.cachedRaycast.time.Equals(time) || !this.cachedRaycast.fixedTime.Equals(fixedTime) || (this.cachedRaycast.maxDistance < maxDistance && !this.cachedRaycast.didHit))
			{
				float num = 0f;
				Ray ray = CameraRigController.ModifyAimRayIfApplicable(this.GetAimRay(), base.gameObject, out num);
				this.cachedRaycast = InputBankTest.CachedRaycastInfo.empty;
				this.cachedRaycast.time = time;
				this.cachedRaycast.fixedTime = fixedTime;
				this.cachedRaycast.maxDistance = maxDistance;
				GameObject gameObject = base.gameObject;
				Ray ray2 = ray;
				float maxDistance2 = maxDistance + num;
				LayerMask layerMask = LayerIndex.world.mask | LayerIndex.entityPrecise.mask;
				this.cachedRaycast.didHit = Util.CharacterRaycast(gameObject, ray2, out this.cachedRaycast.hitInfo, maxDistance2, layerMask, QueryTriggerInteraction.Ignore);
			}
			bool flag = this.cachedRaycast.didHit;
			hitInfo = this.cachedRaycast.hitInfo;
			if (flag && hitInfo.distance > maxDistance)
			{
				flag = false;
				hitInfo = default(RaycastHit);
			}
			return flag;
		}

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x06000D2D RID: 3373 RVA: 0x0003B4DF File Offset: 0x000396DF
		// (set) Token: 0x06000D2E RID: 3374 RVA: 0x0003B4E7 File Offset: 0x000396E7
		public int emoteRequest { get; set; } = -1;

		// Token: 0x06000D2F RID: 3375 RVA: 0x0003B4F0 File Offset: 0x000396F0
		public bool CheckAnyButtonDown()
		{
			return this.skill1.down || this.skill2.down || this.skill3.down || this.skill4.down || this.interact.down || this.jump.down || this.sprint.down || this.activateEquipment.down || this.ping.down;
		}

		// Token: 0x06000D30 RID: 3376 RVA: 0x0003B572 File Offset: 0x00039772
		private void Awake()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
		}

		// Token: 0x04000D56 RID: 3414
		private CharacterBody characterBody;

		// Token: 0x04000D57 RID: 3415
		private Vector3 _aimDirection;

		// Token: 0x04000D58 RID: 3416
		private float lastRaycastTime = float.NegativeInfinity;

		// Token: 0x04000D59 RID: 3417
		private float lastFixedRaycastTime = float.NegativeInfinity;

		// Token: 0x04000D5A RID: 3418
		private bool didLastRaycastHit;

		// Token: 0x04000D5B RID: 3419
		private RaycastHit lastHitInfo;

		// Token: 0x04000D5C RID: 3420
		private float lastMaxDistance;

		// Token: 0x04000D5D RID: 3421
		private InputBankTest.CachedRaycastInfo cachedRaycast = InputBankTest.CachedRaycastInfo.empty;

		// Token: 0x04000D5E RID: 3422
		public Vector3 moveVector;

		// Token: 0x04000D5F RID: 3423
		public InputBankTest.ButtonState skill1;

		// Token: 0x04000D60 RID: 3424
		public InputBankTest.ButtonState skill2;

		// Token: 0x04000D61 RID: 3425
		public InputBankTest.ButtonState skill3;

		// Token: 0x04000D62 RID: 3426
		public InputBankTest.ButtonState skill4;

		// Token: 0x04000D63 RID: 3427
		public InputBankTest.ButtonState interact;

		// Token: 0x04000D64 RID: 3428
		public InputBankTest.ButtonState jump;

		// Token: 0x04000D65 RID: 3429
		public InputBankTest.ButtonState sprint;

		// Token: 0x04000D66 RID: 3430
		public InputBankTest.ButtonState activateEquipment;

		// Token: 0x04000D67 RID: 3431
		public InputBankTest.ButtonState ping;

		// Token: 0x0200025B RID: 603
		private struct CachedRaycastInfo
		{
			// Token: 0x04000D69 RID: 3433
			public float time;

			// Token: 0x04000D6A RID: 3434
			public float fixedTime;

			// Token: 0x04000D6B RID: 3435
			public bool didHit;

			// Token: 0x04000D6C RID: 3436
			public RaycastHit hitInfo;

			// Token: 0x04000D6D RID: 3437
			public float maxDistance;

			// Token: 0x04000D6E RID: 3438
			public static readonly InputBankTest.CachedRaycastInfo empty = new InputBankTest.CachedRaycastInfo
			{
				time = float.NegativeInfinity,
				fixedTime = float.NegativeInfinity,
				didHit = false,
				maxDistance = 0f
			};
		}

		// Token: 0x0200025C RID: 604
		public struct ButtonState
		{
			// Token: 0x170001B0 RID: 432
			// (get) Token: 0x06000D33 RID: 3379 RVA: 0x0003B5F7 File Offset: 0x000397F7
			public bool justReleased
			{
				get
				{
					return !this.down && this.wasDown;
				}
			}

			// Token: 0x170001B1 RID: 433
			// (get) Token: 0x06000D34 RID: 3380 RVA: 0x0003B609 File Offset: 0x00039809
			public bool justPressed
			{
				get
				{
					return this.down && !this.wasDown;
				}
			}

			// Token: 0x06000D35 RID: 3381 RVA: 0x0003B61E File Offset: 0x0003981E
			public void PushState(bool newState)
			{
				this.wasDown = this.down;
				this.down = newState;
			}

			// Token: 0x04000D6F RID: 3439
			public bool down;

			// Token: 0x04000D70 RID: 3440
			public bool wasDown;
		}
	}
}
