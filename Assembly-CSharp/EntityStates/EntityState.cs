using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates
{
	// Token: 0x02000704 RID: 1796
	public class EntityState
	{
		// Token: 0x060029B6 RID: 10678 RVA: 0x000AFB18 File Offset: 0x000ADD18
		public static EntityState Instantiate(short stateTypeIndex)
		{
			Type type = StateIndexTable.IndexToType(stateTypeIndex);
			if (type != null)
			{
				return Activator.CreateInstance(type) as EntityState;
			}
			Debug.LogFormat("Bad stateTypeIndex {0}", new object[]
			{
				stateTypeIndex
			});
			return null;
		}

		// Token: 0x060029B7 RID: 10679 RVA: 0x000AFB5C File Offset: 0x000ADD5C
		public static EntityState Instantiate(Type stateType)
		{
			if (stateType != null && stateType.IsSubclassOf(typeof(EntityState)))
			{
				return Activator.CreateInstance(stateType) as EntityState;
			}
			Debug.LogFormat("Bad stateType {0}", new object[]
			{
				(stateType == null) ? "null" : stateType.FullName
			});
			return null;
		}

		// Token: 0x060029B8 RID: 10680 RVA: 0x000AFBBA File Offset: 0x000ADDBA
		public static EntityState Instantiate(SerializableEntityStateType serializableStateType)
		{
			return EntityState.Instantiate(serializableStateType.stateType);
		}

		// Token: 0x17000403 RID: 1027
		// (get) Token: 0x060029B9 RID: 10681 RVA: 0x000AFBC8 File Offset: 0x000ADDC8
		// (set) Token: 0x060029BA RID: 10682 RVA: 0x000AFBD0 File Offset: 0x000ADDD0
		protected float age { get; set; }

		// Token: 0x17000404 RID: 1028
		// (get) Token: 0x060029BB RID: 10683 RVA: 0x000AFBD9 File Offset: 0x000ADDD9
		// (set) Token: 0x060029BC RID: 10684 RVA: 0x000AFBE1 File Offset: 0x000ADDE1
		protected float fixedAge { get; set; }

		// Token: 0x060029BD RID: 10685 RVA: 0x000AFBEA File Offset: 0x000ADDEA
		public EntityState()
		{
			EntityStateManager.InitializeStateFields(this);
		}

		// Token: 0x060029BE RID: 10686 RVA: 0x0000409B File Offset: 0x0000229B
		public virtual void OnEnter()
		{
		}

		// Token: 0x060029BF RID: 10687 RVA: 0x0000409B File Offset: 0x0000229B
		public virtual void OnExit()
		{
		}

		// Token: 0x060029C0 RID: 10688 RVA: 0x000AFBF8 File Offset: 0x000ADDF8
		public virtual void Update()
		{
			this.age += Time.deltaTime;
		}

		// Token: 0x060029C1 RID: 10689 RVA: 0x000AFC0C File Offset: 0x000ADE0C
		public virtual void FixedUpdate()
		{
			this.fixedAge += Time.fixedDeltaTime;
		}

		// Token: 0x060029C2 RID: 10690 RVA: 0x0000409B File Offset: 0x0000229B
		public virtual void OnSerialize(NetworkWriter writer)
		{
		}

		// Token: 0x060029C3 RID: 10691 RVA: 0x0000409B File Offset: 0x0000229B
		public virtual void OnDeserialize(NetworkReader reader)
		{
		}

		// Token: 0x060029C4 RID: 10692 RVA: 0x0000AC89 File Offset: 0x00008E89
		public virtual InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Any;
		}

		// Token: 0x17000405 RID: 1029
		// (get) Token: 0x060029C5 RID: 10693 RVA: 0x000AFC20 File Offset: 0x000ADE20
		protected GameObject gameObject
		{
			get
			{
				return this.outer.gameObject;
			}
		}

		// Token: 0x060029C6 RID: 10694 RVA: 0x000AFC2D File Offset: 0x000ADE2D
		protected static void Destroy(UnityEngine.Object obj)
		{
			UnityEngine.Object.Destroy(obj);
		}

		// Token: 0x060029C7 RID: 10695 RVA: 0x000AFC35 File Offset: 0x000ADE35
		protected T GetComponent<T>() where T : Component
		{
			return this.outer.GetComponent<T>();
		}

		// Token: 0x060029C8 RID: 10696 RVA: 0x000AFC42 File Offset: 0x000ADE42
		protected Component GetComponent(Type type)
		{
			return this.outer.GetComponent(type);
		}

		// Token: 0x060029C9 RID: 10697 RVA: 0x000AFC50 File Offset: 0x000ADE50
		protected Component GetComponent(string type)
		{
			return this.outer.GetComponent(type);
		}

		// Token: 0x17000406 RID: 1030
		// (get) Token: 0x060029CA RID: 10698 RVA: 0x000AFC5E File Offset: 0x000ADE5E
		protected bool isLocalPlayer
		{
			get
			{
				return this.outer.networker && this.outer.networker.isLocalPlayer;
			}
		}

		// Token: 0x17000407 RID: 1031
		// (get) Token: 0x060029CB RID: 10699 RVA: 0x000AFC84 File Offset: 0x000ADE84
		protected bool localPlayerAuthority
		{
			get
			{
				return this.outer.networker && this.outer.networker.localPlayerAuthority;
			}
		}

		// Token: 0x17000408 RID: 1032
		// (get) Token: 0x060029CC RID: 10700 RVA: 0x000AFCAA File Offset: 0x000ADEAA
		protected bool isAuthority
		{
			get
			{
				return Util.HasEffectiveAuthority(this.outer.networkIdentity);
			}
		}

		// Token: 0x17000409 RID: 1033
		// (get) Token: 0x060029CD RID: 10701 RVA: 0x000AFCBC File Offset: 0x000ADEBC
		protected Transform transform
		{
			get
			{
				return this.outer.commonComponents.transform;
			}
		}

		// Token: 0x1700040A RID: 1034
		// (get) Token: 0x060029CE RID: 10702 RVA: 0x000AFCCE File Offset: 0x000ADECE
		protected CharacterBody characterBody
		{
			get
			{
				return this.outer.commonComponents.characterBody;
			}
		}

		// Token: 0x1700040B RID: 1035
		// (get) Token: 0x060029CF RID: 10703 RVA: 0x000AFCE0 File Offset: 0x000ADEE0
		protected CharacterMotor characterMotor
		{
			get
			{
				return this.outer.commonComponents.characterMotor;
			}
		}

		// Token: 0x1700040C RID: 1036
		// (get) Token: 0x060029D0 RID: 10704 RVA: 0x000AFCF2 File Offset: 0x000ADEF2
		protected CharacterDirection characterDirection
		{
			get
			{
				return this.outer.commonComponents.characterDirection;
			}
		}

		// Token: 0x1700040D RID: 1037
		// (get) Token: 0x060029D1 RID: 10705 RVA: 0x000AFD04 File Offset: 0x000ADF04
		protected Rigidbody rigidbody
		{
			get
			{
				return this.outer.commonComponents.rigidbody;
			}
		}

		// Token: 0x1700040E RID: 1038
		// (get) Token: 0x060029D2 RID: 10706 RVA: 0x000AFD16 File Offset: 0x000ADF16
		protected RigidbodyMotor rigidbodyMotor
		{
			get
			{
				return this.outer.commonComponents.rigidbodyMotor;
			}
		}

		// Token: 0x1700040F RID: 1039
		// (get) Token: 0x060029D3 RID: 10707 RVA: 0x000AFD28 File Offset: 0x000ADF28
		protected RigidbodyDirection rigidbodyDirection
		{
			get
			{
				return this.outer.commonComponents.rigidbodyDirection;
			}
		}

		// Token: 0x17000410 RID: 1040
		// (get) Token: 0x060029D4 RID: 10708 RVA: 0x000AFD3A File Offset: 0x000ADF3A
		protected RailMotor railMotor
		{
			get
			{
				return this.outer.commonComponents.railMotor;
			}
		}

		// Token: 0x17000411 RID: 1041
		// (get) Token: 0x060029D5 RID: 10709 RVA: 0x000AFD4C File Offset: 0x000ADF4C
		protected ModelLocator modelLocator
		{
			get
			{
				return this.outer.commonComponents.modelLocator;
			}
		}

		// Token: 0x17000412 RID: 1042
		// (get) Token: 0x060029D6 RID: 10710 RVA: 0x000AFD5E File Offset: 0x000ADF5E
		protected InputBankTest inputBank
		{
			get
			{
				return this.outer.commonComponents.inputBank;
			}
		}

		// Token: 0x17000413 RID: 1043
		// (get) Token: 0x060029D7 RID: 10711 RVA: 0x000AFD70 File Offset: 0x000ADF70
		protected TeamComponent teamComponent
		{
			get
			{
				return this.outer.commonComponents.teamComponent;
			}
		}

		// Token: 0x17000414 RID: 1044
		// (get) Token: 0x060029D8 RID: 10712 RVA: 0x000AFD82 File Offset: 0x000ADF82
		protected HealthComponent healthComponent
		{
			get
			{
				return this.outer.commonComponents.healthComponent;
			}
		}

		// Token: 0x17000415 RID: 1045
		// (get) Token: 0x060029D9 RID: 10713 RVA: 0x000AFD94 File Offset: 0x000ADF94
		protected SkillLocator skillLocator
		{
			get
			{
				return this.outer.commonComponents.skillLocator;
			}
		}

		// Token: 0x17000416 RID: 1046
		// (get) Token: 0x060029DA RID: 10714 RVA: 0x000AFDA6 File Offset: 0x000ADFA6
		protected CharacterEmoteDefinitions characterEmoteDefinitions
		{
			get
			{
				return this.outer.commonComponents.characterEmoteDefinitions;
			}
		}

		// Token: 0x17000417 RID: 1047
		// (get) Token: 0x060029DB RID: 10715 RVA: 0x000AFDB8 File Offset: 0x000ADFB8
		protected CameraTargetParams cameraTargetParams
		{
			get
			{
				return this.outer.commonComponents.cameraTargetParams;
			}
		}

		// Token: 0x17000418 RID: 1048
		// (get) Token: 0x060029DC RID: 10716 RVA: 0x000AFDCA File Offset: 0x000ADFCA
		protected SfxLocator sfxLocator
		{
			get
			{
				return this.outer.commonComponents.sfxLocator;
			}
		}

		// Token: 0x17000419 RID: 1049
		// (get) Token: 0x060029DD RID: 10717 RVA: 0x000AFDDC File Offset: 0x000ADFDC
		protected BodyAnimatorSmoothingParameters bodyAnimatorSmoothingParameters
		{
			get
			{
				return this.outer.commonComponents.bodyAnimatorSmoothingParameters;
			}
		}

		// Token: 0x1700041A RID: 1050
		// (get) Token: 0x060029DE RID: 10718 RVA: 0x000AFDEE File Offset: 0x000ADFEE
		protected ProjectileController projectileController
		{
			get
			{
				return this.outer.commonComponents.projectileController;
			}
		}

		// Token: 0x060029DF RID: 10719 RVA: 0x000AFE00 File Offset: 0x000AE000
		protected Transform GetModelBaseTransform()
		{
			if (!this.modelLocator)
			{
				return null;
			}
			return this.modelLocator.modelBaseTransform;
		}

		// Token: 0x060029E0 RID: 10720 RVA: 0x000AFE1C File Offset: 0x000AE01C
		protected Transform GetModelTransform()
		{
			if (!this.modelLocator)
			{
				return null;
			}
			return this.modelLocator.modelTransform;
		}

		// Token: 0x060029E1 RID: 10721 RVA: 0x000AFE38 File Offset: 0x000AE038
		protected Animator GetModelAnimator()
		{
			if (this.modelLocator && this.modelLocator.modelTransform)
			{
				return this.modelLocator.modelTransform.GetComponent<Animator>();
			}
			return null;
		}

		// Token: 0x060029E2 RID: 10722 RVA: 0x000AFE6B File Offset: 0x000AE06B
		protected RootMotionAccumulator GetModelRootMotionAccumulator()
		{
			if (this.modelLocator && this.modelLocator.modelTransform)
			{
				return this.modelLocator.modelTransform.GetComponent<RootMotionAccumulator>();
			}
			return null;
		}

		// Token: 0x060029E3 RID: 10723 RVA: 0x000AFEA0 File Offset: 0x000AE0A0
		protected void PlayAnimation(string layerName, string animationStateName, string playbackRateParam, float duration)
		{
			if (duration <= 0f)
			{
				Debug.LogWarningFormat("EntityState.PlayAnimation: Zero duration is not allowed. type={0}", new object[]
				{
					base.GetType().Name
				});
				return;
			}
			Animator modelAnimator = this.GetModelAnimator();
			if (modelAnimator)
			{
				EntityState.PlayAnimationOnAnimator(modelAnimator, layerName, animationStateName, playbackRateParam, duration);
			}
		}

		// Token: 0x060029E4 RID: 10724 RVA: 0x000AFEF0 File Offset: 0x000AE0F0
		protected static void PlayAnimationOnAnimator(Animator modelAnimator, string layerName, string animationStateName, string playbackRateParam, float duration)
		{
			int layerIndex = modelAnimator.GetLayerIndex(layerName);
			modelAnimator.SetFloat(playbackRateParam, 1f);
			modelAnimator.PlayInFixedTime(animationStateName, layerIndex, 0f);
			modelAnimator.Update(0f);
			float length = modelAnimator.GetCurrentAnimatorStateInfo(layerIndex).length;
			modelAnimator.SetFloat(playbackRateParam, length / duration);
		}

		// Token: 0x060029E5 RID: 10725 RVA: 0x000AFF44 File Offset: 0x000AE144
		protected void PlayCrossfade(string layerName, string animationStateName, string playbackRateParam, float duration, float crossfadeDuration)
		{
			if (duration <= 0f)
			{
				Debug.LogWarningFormat("EntityState.PlayCrossfade: Zero duration is not allowed. type={0}", new object[]
				{
					base.GetType().Name
				});
				return;
			}
			Animator modelAnimator = this.GetModelAnimator();
			if (modelAnimator)
			{
				int layerIndex = modelAnimator.GetLayerIndex(layerName);
				modelAnimator.SetFloat(playbackRateParam, 1f);
				modelAnimator.CrossFadeInFixedTime(animationStateName, crossfadeDuration, layerIndex);
				modelAnimator.Update(0f);
				float length = modelAnimator.GetNextAnimatorStateInfo(layerIndex).length;
				modelAnimator.SetFloat(playbackRateParam, length / duration);
			}
		}

		// Token: 0x060029E6 RID: 10726 RVA: 0x000AFFCC File Offset: 0x000AE1CC
		protected void PlayCrossfade(string layerName, string animationStateName, float crossfadeDuration)
		{
			Animator modelAnimator = this.GetModelAnimator();
			if (modelAnimator)
			{
				int layerIndex = modelAnimator.GetLayerIndex(layerName);
				modelAnimator.CrossFadeInFixedTime(animationStateName, crossfadeDuration, layerIndex);
			}
		}

		// Token: 0x060029E7 RID: 10727 RVA: 0x000AFFFC File Offset: 0x000AE1FC
		protected void PlayAnimation(string layerName, string animationStateName)
		{
			Animator modelAnimator = this.GetModelAnimator();
			if (modelAnimator)
			{
				EntityState.PlayAnimationOnAnimator(modelAnimator, layerName, animationStateName);
			}
		}

		// Token: 0x060029E8 RID: 10728 RVA: 0x000B0020 File Offset: 0x000AE220
		protected static void PlayAnimationOnAnimator(Animator modelAnimator, string layerName, string animationStateName)
		{
			int layerIndex = modelAnimator.GetLayerIndex(layerName);
			modelAnimator.PlayInFixedTime(animationStateName, layerIndex, 0f);
		}

		// Token: 0x060029E9 RID: 10729 RVA: 0x000B0042 File Offset: 0x000AE242
		protected void GetBodyAnimatorSmoothingParameters(out BodyAnimatorSmoothingParameters.SmoothingParameters smoothingParameters)
		{
			if (this.bodyAnimatorSmoothingParameters)
			{
				smoothingParameters = this.bodyAnimatorSmoothingParameters.smoothingParameters;
				return;
			}
			smoothingParameters = BodyAnimatorSmoothingParameters.defaultParameters;
		}

		// Token: 0x040025B9 RID: 9657
		public EntityStateMachine outer;
	}
}
