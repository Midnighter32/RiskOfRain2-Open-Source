using System;
using System.Collections.Generic;
using EntityStates;
using JetBrains.Annotations;
using RoR2.CharacterAI;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020001F4 RID: 500
	public class EntityStateMachine : MonoBehaviour
	{
		// Token: 0x17000150 RID: 336
		// (get) Token: 0x06000A6C RID: 2668 RVA: 0x0002DB4B File Offset: 0x0002BD4B
		// (set) Token: 0x06000A6D RID: 2669 RVA: 0x0002DB53 File Offset: 0x0002BD53
		public EntityState state { get; private set; }

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x06000A6E RID: 2670 RVA: 0x0002DB5C File Offset: 0x0002BD5C
		// (set) Token: 0x06000A6F RID: 2671 RVA: 0x0002DB64 File Offset: 0x0002BD64
		public NetworkStateMachine networker { get; private set; }

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x06000A70 RID: 2672 RVA: 0x0002DB6D File Offset: 0x0002BD6D
		// (set) Token: 0x06000A71 RID: 2673 RVA: 0x0002DB75 File Offset: 0x0002BD75
		public NetworkIdentity networkIdentity { get; private set; }

		// Token: 0x06000A72 RID: 2674 RVA: 0x0002DB7E File Offset: 0x0002BD7E
		public void SetNextState(EntityState newNextState)
		{
			this.nextState = newNextState;
		}

		// Token: 0x06000A73 RID: 2675 RVA: 0x0002DB87 File Offset: 0x0002BD87
		public void SetNextStateToMain()
		{
			this.nextState = EntityState.Instantiate(this.mainStateType);
		}

		// Token: 0x06000A74 RID: 2676 RVA: 0x0002DB9A File Offset: 0x0002BD9A
		public bool CanInterruptState(InterruptPriority interruptPriority)
		{
			return (this.nextState ?? this.state).GetMinimumInterruptPriority() <= interruptPriority;
		}

		// Token: 0x06000A75 RID: 2677 RVA: 0x0002DBB7 File Offset: 0x0002BDB7
		public bool SetInterruptState(EntityState newNextState, InterruptPriority interruptPriority)
		{
			if (this.CanInterruptState(interruptPriority))
			{
				this.nextState = newNextState;
				return true;
			}
			return false;
		}

		// Token: 0x06000A76 RID: 2678 RVA: 0x0002DBCC File Offset: 0x0002BDCC
		public bool HasPendingState()
		{
			return this.nextState != null;
		}

		// Token: 0x06000A77 RID: 2679 RVA: 0x0002DBD8 File Offset: 0x0002BDD8
		public void SetState([NotNull] EntityState newState)
		{
			this.nextState = null;
			newState.outer = this;
			if (this.state == null)
			{
				Debug.LogErrorFormat("State machine {0} on object {1} does not have a state!", new object[]
				{
					this.customName,
					base.gameObject
				});
			}
			this.state.OnExit();
			this.state = newState;
			this.state.OnEnter();
			if (this.networkIndex != -1)
			{
				if (!this.networker)
				{
					Debug.LogErrorFormat("State machine {0} on object {1} does not have a networker assigned!", new object[]
					{
						this.customName,
						base.gameObject
					});
				}
				this.networker.SendSetEntityState(this.networkIndex);
			}
		}

		// Token: 0x06000A78 RID: 2680 RVA: 0x0002DC84 File Offset: 0x0002BE84
		private void Awake()
		{
			this.networker = base.GetComponent<NetworkStateMachine>();
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
			this.commonComponents = new EntityStateMachine.CommonComponentCache(base.gameObject);
			this.state = new Uninitialized();
			this.state.outer = this;
		}

		// Token: 0x06000A79 RID: 2681 RVA: 0x0002DCD4 File Offset: 0x0002BED4
		private void Start()
		{
			if (this.nextState != null && this.networker && !this.networker.hasAuthority)
			{
				this.SetState(this.nextState);
				return;
			}
			Type stateType = this.initialStateType.stateType;
			if (this.state is Uninitialized && stateType != null && stateType.IsSubclassOf(typeof(EntityState)))
			{
				this.SetState(EntityState.Instantiate(stateType));
			}
		}

		// Token: 0x06000A7A RID: 2682 RVA: 0x0002DD50 File Offset: 0x0002BF50
		public void Update()
		{
			this.state.Update();
		}

		// Token: 0x06000A7B RID: 2683 RVA: 0x0002DD5D File Offset: 0x0002BF5D
		public void FixedUpdate()
		{
			if (this.nextState != null)
			{
				this.SetState(this.nextState);
			}
			this.state.FixedUpdate();
		}

		// Token: 0x17000153 RID: 339
		// (get) Token: 0x06000A7C RID: 2684 RVA: 0x0002DD7E File Offset: 0x0002BF7E
		// (set) Token: 0x06000A7D RID: 2685 RVA: 0x0002DD86 File Offset: 0x0002BF86
		public bool destroying { get; private set; }

		// Token: 0x06000A7E RID: 2686 RVA: 0x0002DD8F File Offset: 0x0002BF8F
		private void OnDestroy()
		{
			this.destroying = true;
			if (this.state != null)
			{
				this.state.OnExit();
				this.state = null;
			}
		}

		// Token: 0x06000A7F RID: 2687 RVA: 0x0002DDB4 File Offset: 0x0002BFB4
		private void OnValidate()
		{
			if (this.mainStateType.stateType == null)
			{
				if (this.customName == "Body")
				{
					if (base.GetComponent<CharacterMotor>())
					{
						this.mainStateType = new SerializableEntityStateType(typeof(GenericCharacterMain));
						return;
					}
					if (base.GetComponent<RigidbodyMotor>())
					{
						this.mainStateType = new SerializableEntityStateType(typeof(FlyState));
						return;
					}
				}
				else
				{
					if (this.customName == "Weapon")
					{
						this.mainStateType = new SerializableEntityStateType(typeof(Idle));
						return;
					}
					if (this.customName == "AI")
					{
						BaseAI component = base.GetComponent<BaseAI>();
						if (component)
						{
							this.mainStateType = component.scanState;
						}
					}
				}
			}
		}

		// Token: 0x06000A80 RID: 2688 RVA: 0x0002DE84 File Offset: 0x0002C084
		public static EntityStateMachine FindByCustomName(GameObject gameObject, string customName)
		{
			List<EntityStateMachine> gameObjectComponents = GetComponentsCache<EntityStateMachine>.GetGameObjectComponents(gameObject);
			EntityStateMachine result = null;
			int i = 0;
			int count = gameObjectComponents.Count;
			while (i < count)
			{
				if (string.CompareOrdinal(customName, gameObjectComponents[i].customName) == 0)
				{
					result = gameObjectComponents[i];
				}
				i++;
			}
			GetComponentsCache<EntityStateMachine>.ReturnBuffer(gameObjectComponents);
			return result;
		}

		// Token: 0x04000AD1 RID: 2769
		private EntityState nextState;

		// Token: 0x04000AD2 RID: 2770
		[Tooltip("The name of this state machine.")]
		public string customName;

		// Token: 0x04000AD3 RID: 2771
		[Tooltip("The type of the state to enter when this component is first activated.")]
		public SerializableEntityStateType initialStateType = new SerializableEntityStateType(typeof(TestState1));

		// Token: 0x04000AD4 RID: 2772
		[Tooltip("The preferred main state of this state machine.")]
		public SerializableEntityStateType mainStateType;

		// Token: 0x04000AD7 RID: 2775
		public EntityStateMachine.CommonComponentCache commonComponents;

		// Token: 0x04000AD8 RID: 2776
		[NonSerialized]
		public int networkIndex = -1;

		// Token: 0x020001F5 RID: 501
		public struct CommonComponentCache
		{
			// Token: 0x06000A82 RID: 2690 RVA: 0x0002DEF4 File Offset: 0x0002C0F4
			public CommonComponentCache(GameObject gameObject)
			{
				this.transform = gameObject.transform;
				this.characterBody = gameObject.GetComponent<CharacterBody>();
				this.characterMotor = gameObject.GetComponent<CharacterMotor>();
				this.characterDirection = gameObject.GetComponent<CharacterDirection>();
				this.rigidbody = gameObject.GetComponent<Rigidbody>();
				this.rigidbodyMotor = gameObject.GetComponent<RigidbodyMotor>();
				this.rigidbodyDirection = gameObject.GetComponent<RigidbodyDirection>();
				this.railMotor = gameObject.GetComponent<RailMotor>();
				this.modelLocator = gameObject.GetComponent<ModelLocator>();
				this.inputBank = gameObject.GetComponent<InputBankTest>();
				this.teamComponent = gameObject.GetComponent<TeamComponent>();
				this.healthComponent = gameObject.GetComponent<HealthComponent>();
				this.skillLocator = gameObject.GetComponent<SkillLocator>();
				this.characterEmoteDefinitions = gameObject.GetComponent<CharacterEmoteDefinitions>();
				this.cameraTargetParams = gameObject.GetComponent<CameraTargetParams>();
				this.sfxLocator = gameObject.GetComponent<SfxLocator>();
				this.bodyAnimatorSmoothingParameters = gameObject.GetComponent<BodyAnimatorSmoothingParameters>();
				this.projectileController = gameObject.GetComponent<ProjectileController>();
			}

			// Token: 0x04000ADA RID: 2778
			public readonly Transform transform;

			// Token: 0x04000ADB RID: 2779
			public readonly CharacterBody characterBody;

			// Token: 0x04000ADC RID: 2780
			public readonly CharacterMotor characterMotor;

			// Token: 0x04000ADD RID: 2781
			public readonly CharacterDirection characterDirection;

			// Token: 0x04000ADE RID: 2782
			public readonly Rigidbody rigidbody;

			// Token: 0x04000ADF RID: 2783
			public readonly RigidbodyMotor rigidbodyMotor;

			// Token: 0x04000AE0 RID: 2784
			public readonly RigidbodyDirection rigidbodyDirection;

			// Token: 0x04000AE1 RID: 2785
			public readonly RailMotor railMotor;

			// Token: 0x04000AE2 RID: 2786
			public readonly ModelLocator modelLocator;

			// Token: 0x04000AE3 RID: 2787
			public readonly InputBankTest inputBank;

			// Token: 0x04000AE4 RID: 2788
			public readonly TeamComponent teamComponent;

			// Token: 0x04000AE5 RID: 2789
			public readonly HealthComponent healthComponent;

			// Token: 0x04000AE6 RID: 2790
			public readonly SkillLocator skillLocator;

			// Token: 0x04000AE7 RID: 2791
			public readonly CharacterEmoteDefinitions characterEmoteDefinitions;

			// Token: 0x04000AE8 RID: 2792
			public readonly CameraTargetParams cameraTargetParams;

			// Token: 0x04000AE9 RID: 2793
			public readonly SfxLocator sfxLocator;

			// Token: 0x04000AEA RID: 2794
			public readonly BodyAnimatorSmoothingParameters bodyAnimatorSmoothingParameters;

			// Token: 0x04000AEB RID: 2795
			public readonly ProjectileController projectileController;
		}
	}
}
