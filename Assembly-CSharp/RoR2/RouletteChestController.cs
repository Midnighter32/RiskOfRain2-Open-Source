using System;
using EntityStates;
using JetBrains.Annotations;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002FC RID: 764
	[RequireComponent(typeof(EntityStateMachine))]
	[RequireComponent(typeof(PurchaseInteraction))]
	public class RouletteChestController : NetworkBehaviour
	{
		// Token: 0x06001178 RID: 4472 RVA: 0x0004C618 File Offset: 0x0004A818
		private float CalcEntryDuration(int i)
		{
			float time = (float)i / (float)this.maxEntries;
			return this.bonusTimeDecay.Evaluate(time) * this.bonusTime + RouletteChestController.minTime;
		}

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x06001179 RID: 4473 RVA: 0x0004C649 File Offset: 0x0004A849
		private bool isCycling
		{
			get
			{
				return !this.activationTime.isPositiveInfinity;
			}
		}

		// Token: 0x0600117A RID: 4474 RVA: 0x0004C65C File Offset: 0x0004A85C
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint syncVarDirtyBits = base.syncVarDirtyBits;
			if (initialState)
			{
				syncVarDirtyBits = RouletteChestController.allDirtyBitsMask;
			}
			writer.WritePackedUInt32(syncVarDirtyBits);
			if ((syncVarDirtyBits & RouletteChestController.activationTimeDirtyBit) != 0U)
			{
				writer.Write(this.activationTime);
			}
			if ((syncVarDirtyBits & RouletteChestController.entriesDirtyBit) != 0U)
			{
				writer.WritePackedUInt32((uint)this.entries.Length);
				for (int i = 0; i < this.entries.Length; i++)
				{
					writer.Write(this.entries[i].pickupIndex);
				}
			}
			return syncVarDirtyBits > 0U;
		}

		// Token: 0x0600117B RID: 4475 RVA: 0x0004C6DC File Offset: 0x0004A8DC
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			uint num = reader.ReadPackedUInt32();
			if ((num & RouletteChestController.activationTimeDirtyBit) != 0U)
			{
				this.activationTime = reader.ReadFixedTimeStamp();
			}
			if ((num & RouletteChestController.entriesDirtyBit) != 0U)
			{
				Array.Resize<RouletteChestController.Entry>(ref this.entries, (int)reader.ReadPackedUInt32());
				Run.FixedTimeStamp endTime = this.activationTime;
				for (int i = 0; i < this.entries.Length; i++)
				{
					RouletteChestController.Entry[] array = this.entries;
					int num2 = i;
					array[num2].pickupIndex = reader.ReadPickupIndex();
					array[num2].endTime = endTime + this.CalcEntryDuration(i);
					endTime = array[num2].endTime;
				}
			}
		}

		// Token: 0x0600117C RID: 4476 RVA: 0x0004C768 File Offset: 0x0004A968
		private void Awake()
		{
			this.stateMachine = base.GetComponent<EntityStateMachine>();
			this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
		}

		// Token: 0x0600117D RID: 4477 RVA: 0x0004C782 File Offset: 0x0004A982
		private void Start()
		{
			if (NetworkServer.active)
			{
				this.rng = new Xoroshiro128Plus(Run.instance.treasureRng.nextUlong);
			}
		}

		// Token: 0x0600117E RID: 4478 RVA: 0x0004C7A5 File Offset: 0x0004A9A5
		private void OnEnable()
		{
			base.SetDirtyBit(RouletteChestController.enabledDirtyBit);
			if (this.pickupDisplay)
			{
				this.pickupDisplay.enabled = true;
			}
		}

		// Token: 0x0600117F RID: 4479 RVA: 0x0004C7CB File Offset: 0x0004A9CB
		private void OnDisable()
		{
			if (this.pickupDisplay)
			{
				this.pickupDisplay.SetPickupIndex(PickupIndex.none, false);
				this.pickupDisplay.enabled = false;
			}
			base.SetDirtyBit(RouletteChestController.enabledDirtyBit);
		}

		// Token: 0x06001180 RID: 4480 RVA: 0x0004C804 File Offset: 0x0004AA04
		[Server]
		private void GenerateEntriesServer(Run.FixedTimeStamp startTime)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.RouletteChestController::GenerateEntriesServer(RoR2.Run/FixedTimeStamp)' called on client");
				return;
			}
			Array.Resize<RouletteChestController.Entry>(ref this.entries, this.maxEntries);
			for (int i = 0; i < this.entries.Length; i++)
			{
				RouletteChestController.Entry[] array = this.entries;
				int num = i;
				array[num].endTime = startTime + this.CalcEntryDuration(i);
				startTime = array[num].endTime;
			}
			PickupIndex b = PickupIndex.none;
			for (int j = 0; j < this.entries.Length; j++)
			{
				RouletteChestController.Entry[] array2 = this.entries;
				int num2 = j;
				PickupIndex pickupIndex = this.dropTable.GenerateDrop(this.rng);
				if (pickupIndex == b)
				{
					pickupIndex = this.dropTable.GenerateDrop(this.rng);
				}
				array2[num2].pickupIndex = pickupIndex;
				b = pickupIndex;
			}
			base.SetDirtyBit(RouletteChestController.entriesDirtyBit);
		}

		// Token: 0x06001181 RID: 4481 RVA: 0x0004C8D5 File Offset: 0x0004AAD5
		[Server]
		public void HandleInteractionServer(Interactor activator)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.RouletteChestController::HandleInteractionServer(RoR2.Interactor)' called on client");
				return;
			}
			((RouletteChestController.RouletteChestControllerBaseState)this.stateMachine.state).HandleInteractionServer(activator);
		}

		// Token: 0x06001182 RID: 4482 RVA: 0x0004C904 File Offset: 0x0004AB04
		[Server]
		private void BeginCycleServer()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.RouletteChestController::BeginCycleServer()' called on client");
				return;
			}
			this.activationTime = Run.FixedTimeStamp.now;
			base.SetDirtyBit(RouletteChestController.activationTimeDirtyBit);
			this.GenerateEntriesServer(this.activationTime);
			UnityEvent unityEvent = this.onCycleBeginServer;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		// Token: 0x06001183 RID: 4483 RVA: 0x0004C958 File Offset: 0x0004AB58
		[Server]
		private void EndCycleServer([CanBeNull] Interactor activator)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.RouletteChestController::EndCycleServer(RoR2.Interactor)' called on client");
				return;
			}
			float b = 0f;
			NetworkUser networkUser;
			if (activator && (networkUser = Util.LookUpBodyNetworkUser(activator.gameObject)) != null)
			{
				b = RttManager.GetConnectionRTT(networkUser.connectionToClient);
			}
			Run.FixedTimeStamp time = Run.FixedTimeStamp.now - b - RouletteChestController.rewindTime;
			PickupIndex pickupIndexForTime = this.GetPickupIndexForTime(time);
			this.EjectPickupServer(pickupIndexForTime);
			this.activationTime = Run.FixedTimeStamp.positiveInfinity;
			this.onCycleCompletedServer.Invoke();
		}

		// Token: 0x06001184 RID: 4484 RVA: 0x0004C9E0 File Offset: 0x0004ABE0
		private void FixedUpdate()
		{
			if (this.pickupDisplay)
			{
				this.pickupDisplay.SetPickupIndex(this.isCycling ? this.GetPickupIndexForTime(Run.FixedTimeStamp.now) : PickupIndex.none, false);
			}
			if (NetworkClient.active)
			{
				int entryIndexForTime = this.GetEntryIndexForTime(Run.FixedTimeStamp.now);
				if (entryIndexForTime != this.previousEntryIndexClient)
				{
					this.previousEntryIndexClient = entryIndexForTime;
					this.onChangedEntryClient.Invoke();
				}
			}
			if (NetworkServer.active && this.isCycling && this.entries.Length != 0)
			{
				Run.FixedTimeStamp endTime = this.entries[this.entries.Length - 1].endTime;
				if (endTime.hasPassed)
				{
					this.EndCycleServer(null);
				}
			}
		}

		// Token: 0x06001185 RID: 4485 RVA: 0x0004CA94 File Offset: 0x0004AC94
		private int GetEntryIndexForTime(Run.FixedTimeStamp time)
		{
			for (int i = 0; i < this.entries.Length; i++)
			{
				if (time < this.entries[i].endTime)
				{
					return i;
				}
			}
			if (this.entries.Length != 0)
			{
				return this.entries.Length - 1;
			}
			return -1;
		}

		// Token: 0x06001186 RID: 4486 RVA: 0x0004CAE4 File Offset: 0x0004ACE4
		private PickupIndex GetPickupIndexForTime(Run.FixedTimeStamp time)
		{
			int entryIndexForTime = this.GetEntryIndexForTime(time);
			if (entryIndexForTime != -1)
			{
				return this.entries[entryIndexForTime].pickupIndex;
			}
			return PickupIndex.none;
		}

		// Token: 0x06001187 RID: 4487 RVA: 0x0004CB14 File Offset: 0x0004AD14
		private void EjectPickupServer(PickupIndex pickupIndex)
		{
			if (pickupIndex == PickupIndex.none)
			{
				return;
			}
			PickupDropletController.CreatePickupDroplet(pickupIndex, this.ejectionTransform.position, this.ejectionTransform.rotation * this.localEjectionVelocity);
		}

		// Token: 0x0600118A RID: 4490 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x040010D9 RID: 4313
		public int maxEntries = 1;

		// Token: 0x040010DA RID: 4314
		public float bonusTime;

		// Token: 0x040010DB RID: 4315
		public AnimationCurve bonusTimeDecay;

		// Token: 0x040010DC RID: 4316
		public PickupDropTable dropTable;

		// Token: 0x040010DD RID: 4317
		public Transform ejectionTransform;

		// Token: 0x040010DE RID: 4318
		public Vector3 localEjectionVelocity;

		// Token: 0x040010DF RID: 4319
		public Animator modelAnimator;

		// Token: 0x040010E0 RID: 4320
		public PickupDisplay pickupDisplay;

		// Token: 0x040010E1 RID: 4321
		private EntityStateMachine stateMachine;

		// Token: 0x040010E2 RID: 4322
		private PurchaseInteraction purchaseInteraction;

		// Token: 0x040010E3 RID: 4323
		private static readonly float averageHgReactionTime = 0.20366667f;

		// Token: 0x040010E4 RID: 4324
		private static readonly float lowestHgReactionTime = 0.15f;

		// Token: 0x040010E5 RID: 4325
		private static readonly float recognitionWindow = 0.15f;

		// Token: 0x040010E6 RID: 4326
		private static readonly float minTime = RouletteChestController.lowestHgReactionTime + RouletteChestController.recognitionWindow;

		// Token: 0x040010E7 RID: 4327
		private static readonly float rewindTime = 0.05f;

		// Token: 0x040010E8 RID: 4328
		private Run.FixedTimeStamp activationTime = Run.FixedTimeStamp.positiveInfinity;

		// Token: 0x040010E9 RID: 4329
		private RouletteChestController.Entry[] entries = Array.Empty<RouletteChestController.Entry>();

		// Token: 0x040010EA RID: 4330
		private Xoroshiro128Plus rng;

		// Token: 0x040010EB RID: 4331
		private static readonly uint activationTimeDirtyBit = 1U;

		// Token: 0x040010EC RID: 4332
		private static readonly uint entriesDirtyBit = 2U;

		// Token: 0x040010ED RID: 4333
		private static readonly uint enabledDirtyBit = 4U;

		// Token: 0x040010EE RID: 4334
		private static readonly uint allDirtyBitsMask = RouletteChestController.activationTimeDirtyBit | RouletteChestController.entriesDirtyBit;

		// Token: 0x040010EF RID: 4335
		private int previousEntryIndexClient = -1;

		// Token: 0x040010F0 RID: 4336
		public UnityEvent onCycleBeginServer;

		// Token: 0x040010F1 RID: 4337
		public UnityEvent onCycleCompletedServer;

		// Token: 0x040010F2 RID: 4338
		public UnityEvent onChangedEntryClient;

		// Token: 0x020002FD RID: 765
		public struct Entry
		{
			// Token: 0x040010F3 RID: 4339
			public PickupIndex pickupIndex;

			// Token: 0x040010F4 RID: 4340
			public Run.FixedTimeStamp endTime;
		}

		// Token: 0x020002FE RID: 766
		private class RouletteChestControllerBaseState : EntityState
		{
			// Token: 0x1700021D RID: 541
			// (get) Token: 0x0600118B RID: 4491 RVA: 0x0004CBDF File Offset: 0x0004ADDF
			// (set) Token: 0x0600118C RID: 4492 RVA: 0x0004CBE7 File Offset: 0x0004ADE7
			private protected RouletteChestController rouletteChestController { protected get; private set; }

			// Token: 0x0600118D RID: 4493 RVA: 0x0004CBF0 File Offset: 0x0004ADF0
			public override void OnEnter()
			{
				base.OnEnter();
				this.rouletteChestController = base.GetComponent<RouletteChestController>();
			}

			// Token: 0x0600118E RID: 4494 RVA: 0x0000409B File Offset: 0x0000229B
			public virtual void HandleInteractionServer(Interactor activator)
			{
			}
		}

		// Token: 0x020002FF RID: 767
		private class Idle : RouletteChestController.RouletteChestControllerBaseState
		{
			// Token: 0x06001190 RID: 4496 RVA: 0x0004CC04 File Offset: 0x0004AE04
			public override void OnEnter()
			{
				base.OnEnter();
				base.PlayAnimation("Body", "Idle");
				base.rouletteChestController.purchaseInteraction.Networkavailable = true;
			}

			// Token: 0x06001191 RID: 4497 RVA: 0x0004CC2D File Offset: 0x0004AE2D
			public override void HandleInteractionServer(Interactor activator)
			{
				base.HandleInteractionServer(activator);
				this.outer.SetNextState(new RouletteChestController.Startup());
			}
		}

		// Token: 0x02000300 RID: 768
		private class Startup : RouletteChestController.RouletteChestControllerBaseState
		{
			// Token: 0x06001193 RID: 4499 RVA: 0x0004CC50 File Offset: 0x0004AE50
			public override void OnEnter()
			{
				base.OnEnter();
				base.PlayAnimation("Body", "IdleToActive");
				base.rouletteChestController.purchaseInteraction.Networkavailable = false;
				base.rouletteChestController.purchaseInteraction.costType = CostTypeIndex.None;
				Util.PlaySound(RouletteChestController.Startup.soundEntryEvent, base.gameObject);
			}

			// Token: 0x06001194 RID: 4500 RVA: 0x0004CCA6 File Offset: 0x0004AEA6
			public override void FixedUpdate()
			{
				base.FixedUpdate();
				if (NetworkServer.active && base.fixedAge > RouletteChestController.Startup.baseDuration)
				{
					this.outer.SetNextState(new RouletteChestController.Cycling());
				}
			}

			// Token: 0x040010F6 RID: 4342
			public static float baseDuration;

			// Token: 0x040010F7 RID: 4343
			public static string soundEntryEvent;
		}

		// Token: 0x02000301 RID: 769
		private class Cycling : RouletteChestController.RouletteChestControllerBaseState
		{
			// Token: 0x06001196 RID: 4502 RVA: 0x0004CCD4 File Offset: 0x0004AED4
			public override void OnEnter()
			{
				base.OnEnter();
				base.rouletteChestController.onChangedEntryClient.AddListener(new UnityAction(this.OnChangedEntryClient));
				if (NetworkServer.active)
				{
					base.rouletteChestController.BeginCycleServer();
					base.rouletteChestController.onCycleCompletedServer.AddListener(new UnityAction(this.OnCycleCompleted));
				}
				base.rouletteChestController.purchaseInteraction.Networkavailable = true;
				base.rouletteChestController.purchaseInteraction.costType = CostTypeIndex.None;
			}

			// Token: 0x06001197 RID: 4503 RVA: 0x0004CD53 File Offset: 0x0004AF53
			private void OnCycleCompleted()
			{
				this.outer.SetNextState(new RouletteChestController.Opening());
			}

			// Token: 0x06001198 RID: 4504 RVA: 0x0004CD65 File Offset: 0x0004AF65
			public override void OnExit()
			{
				base.rouletteChestController.onCycleCompletedServer.RemoveListener(new UnityAction(this.OnCycleCompleted));
				base.rouletteChestController.onChangedEntryClient.RemoveListener(new UnityAction(this.OnChangedEntryClient));
				base.OnExit();
			}

			// Token: 0x06001199 RID: 4505 RVA: 0x0004CDA8 File Offset: 0x0004AFA8
			private void OnChangedEntryClient()
			{
				int entryIndexForTime = base.rouletteChestController.GetEntryIndexForTime(Run.FixedTimeStamp.now);
				float num = base.rouletteChestController.CalcEntryDuration(entryIndexForTime);
				base.PlayAnimation("Body", "ActiveLoop", "ActiveLoop.playbackRate", num);
				float num2 = Util.Remap(num, RouletteChestController.minTime, RouletteChestController.minTime + base.rouletteChestController.bonusTime, 1f, 0f);
				Util.PlaySound(RouletteChestController.Cycling.soundCycleEvent, base.gameObject, RouletteChestController.Cycling.soundCycleSpeedRtpc, num2 * RouletteChestController.Cycling.soundCycleSpeedRtpcScale);
			}

			// Token: 0x0600119A RID: 4506 RVA: 0x0004CE2D File Offset: 0x0004B02D
			public override void HandleInteractionServer(Interactor activator)
			{
				base.HandleInteractionServer(activator);
				base.rouletteChestController.EndCycleServer(activator);
			}

			// Token: 0x040010F8 RID: 4344
			public static string soundCycleEvent;

			// Token: 0x040010F9 RID: 4345
			public static string soundCycleSpeedRtpc;

			// Token: 0x040010FA RID: 4346
			public static float soundCycleSpeedRtpcScale;
		}

		// Token: 0x02000302 RID: 770
		private class Opening : RouletteChestController.RouletteChestControllerBaseState
		{
			// Token: 0x0600119C RID: 4508 RVA: 0x0004CE44 File Offset: 0x0004B044
			public override void OnEnter()
			{
				base.OnEnter();
				base.PlayAnimation("Body", "ActiveToOpening");
				base.rouletteChestController.purchaseInteraction.Networkavailable = false;
				base.rouletteChestController.purchaseInteraction.costType = CostTypeIndex.None;
				Util.PlaySound(RouletteChestController.Opening.soundEntryEvent, base.gameObject);
			}

			// Token: 0x0600119D RID: 4509 RVA: 0x0004CE9A File Offset: 0x0004B09A
			public override void FixedUpdate()
			{
				base.FixedUpdate();
				if (NetworkServer.active && base.fixedAge > RouletteChestController.Opening.baseDuration)
				{
					this.outer.SetNextState(new RouletteChestController.Opened());
				}
			}

			// Token: 0x040010FB RID: 4347
			public static float baseDuration;

			// Token: 0x040010FC RID: 4348
			public static string soundEntryEvent;
		}

		// Token: 0x02000303 RID: 771
		private class Opened : RouletteChestController.RouletteChestControllerBaseState
		{
			// Token: 0x0600119F RID: 4511 RVA: 0x0004CEC6 File Offset: 0x0004B0C6
			public override void OnEnter()
			{
				base.OnEnter();
				base.PlayAnimation("Body", "Opened");
				base.rouletteChestController.purchaseInteraction.Networkavailable = false;
				base.rouletteChestController.purchaseInteraction.costType = CostTypeIndex.None;
			}
		}
	}
}
