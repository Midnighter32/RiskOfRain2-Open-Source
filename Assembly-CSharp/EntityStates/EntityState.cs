using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates
{
	// Token: 0x020000AD RID: 173
	public class EntityState
	{
		// Token: 0x0600033C RID: 828 RVA: 0x0000D5E0 File Offset: 0x0000B7E0
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

		// Token: 0x0600033D RID: 829 RVA: 0x0000D624 File Offset: 0x0000B824
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

		// Token: 0x0600033E RID: 830 RVA: 0x0000D682 File Offset: 0x0000B882
		public static EntityState Instantiate(SerializableEntityStateType serializableStateType)
		{
			return EntityState.Instantiate(serializableStateType.stateType);
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x0600033F RID: 831 RVA: 0x0000D690 File Offset: 0x0000B890
		// (set) Token: 0x06000340 RID: 832 RVA: 0x0000D698 File Offset: 0x0000B898
		private protected float age { protected get; private set; }

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000341 RID: 833 RVA: 0x0000D6A1 File Offset: 0x0000B8A1
		// (set) Token: 0x06000342 RID: 834 RVA: 0x0000D6A9 File Offset: 0x0000B8A9
		private protected float fixedAge { protected get; private set; }

		// Token: 0x06000343 RID: 835 RVA: 0x0000D6B2 File Offset: 0x0000B8B2
		public EntityState()
		{
			EntityStateManager.InitializeStateFields(this);
		}

		// Token: 0x06000344 RID: 836 RVA: 0x00004507 File Offset: 0x00002707
		public virtual void OnEnter()
		{
		}

		// Token: 0x06000345 RID: 837 RVA: 0x00004507 File Offset: 0x00002707
		public virtual void OnExit()
		{
		}

		// Token: 0x06000346 RID: 838 RVA: 0x0000D6C0 File Offset: 0x0000B8C0
		public virtual void Update()
		{
			this.age += Time.deltaTime;
		}

		// Token: 0x06000347 RID: 839 RVA: 0x0000D6D4 File Offset: 0x0000B8D4
		public virtual void FixedUpdate()
		{
			this.fixedAge += Time.fixedDeltaTime;
		}

		// Token: 0x06000348 RID: 840 RVA: 0x00004507 File Offset: 0x00002707
		public virtual void OnSerialize(NetworkWriter writer)
		{
		}

		// Token: 0x06000349 RID: 841 RVA: 0x00004507 File Offset: 0x00002707
		public virtual void OnDeserialize(NetworkReader reader)
		{
		}

		// Token: 0x0600034A RID: 842 RVA: 0x0000A1ED File Offset: 0x000083ED
		public virtual InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Any;
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x0600034B RID: 843 RVA: 0x0000D6E8 File Offset: 0x0000B8E8
		protected GameObject gameObject
		{
			get
			{
				return this.outer.gameObject;
			}
		}

		// Token: 0x0600034C RID: 844 RVA: 0x0000D6F5 File Offset: 0x0000B8F5
		protected static void Destroy(UnityEngine.Object obj)
		{
			UnityEngine.Object.Destroy(obj);
		}

		// Token: 0x0600034D RID: 845 RVA: 0x0000D6FD File Offset: 0x0000B8FD
		protected T GetComponent<T>() where T : Component
		{
			return this.outer.GetComponent<T>();
		}

		// Token: 0x0600034E RID: 846 RVA: 0x0000D70A File Offset: 0x0000B90A
		protected Component GetComponent(Type type)
		{
			return this.outer.GetComponent(type);
		}

		// Token: 0x0600034F RID: 847 RVA: 0x0000D718 File Offset: 0x0000B918
		protected Component GetComponent(string type)
		{
			return this.outer.GetComponent(type);
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000350 RID: 848 RVA: 0x0000D726 File Offset: 0x0000B926
		protected bool isServer
		{
			get
			{
				return this.outer.networker && this.outer.networker.isServer;
			}
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000351 RID: 849 RVA: 0x0000D74C File Offset: 0x0000B94C
		protected bool isClient
		{
			get
			{
				return this.outer.networker && this.outer.networker.isClient;
			}
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000352 RID: 850 RVA: 0x0000D772 File Offset: 0x0000B972
		protected bool hasAuthority
		{
			get
			{
				return this.outer.networker && this.outer.networker.hasAuthority;
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000353 RID: 851 RVA: 0x0000D798 File Offset: 0x0000B998
		protected bool isLocalPlayer
		{
			get
			{
				return this.outer.networker && this.outer.networker.isLocalPlayer;
			}
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000354 RID: 852 RVA: 0x0000D7BE File Offset: 0x0000B9BE
		protected bool localPlayerAuthority
		{
			get
			{
				return this.outer.networker && this.outer.networker.localPlayerAuthority;
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000355 RID: 853 RVA: 0x0000D7E4 File Offset: 0x0000B9E4
		protected bool isAuthority
		{
			get
			{
				return Util.HasEffectiveAuthority(this.outer.networkIdentity);
			}
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000356 RID: 854 RVA: 0x0000D7F6 File Offset: 0x0000B9F6
		protected Transform transform
		{
			get
			{
				return this.outer.commonComponents.transform;
			}
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000357 RID: 855 RVA: 0x0000D808 File Offset: 0x0000BA08
		protected CharacterBody characterBody
		{
			get
			{
				return this.outer.commonComponents.characterBody;
			}
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000358 RID: 856 RVA: 0x0000D81A File Offset: 0x0000BA1A
		protected CharacterMotor characterMotor
		{
			get
			{
				return this.outer.commonComponents.characterMotor;
			}
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06000359 RID: 857 RVA: 0x0000D82C File Offset: 0x0000BA2C
		protected CharacterDirection characterDirection
		{
			get
			{
				return this.outer.commonComponents.characterDirection;
			}
		}

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x0600035A RID: 858 RVA: 0x0000D83E File Offset: 0x0000BA3E
		protected Rigidbody rigidbody
		{
			get
			{
				return this.outer.commonComponents.rigidbody;
			}
		}

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x0600035B RID: 859 RVA: 0x0000D850 File Offset: 0x0000BA50
		protected RigidbodyMotor rigidbodyMotor
		{
			get
			{
				return this.outer.commonComponents.rigidbodyMotor;
			}
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x0600035C RID: 860 RVA: 0x0000D862 File Offset: 0x0000BA62
		protected RigidbodyDirection rigidbodyDirection
		{
			get
			{
				return this.outer.commonComponents.rigidbodyDirection;
			}
		}

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x0600035D RID: 861 RVA: 0x0000D874 File Offset: 0x0000BA74
		protected RailMotor railMotor
		{
			get
			{
				return this.outer.commonComponents.railMotor;
			}
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x0600035E RID: 862 RVA: 0x0000D886 File Offset: 0x0000BA86
		protected ModelLocator modelLocator
		{
			get
			{
				return this.outer.commonComponents.modelLocator;
			}
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x0600035F RID: 863 RVA: 0x0000D898 File Offset: 0x0000BA98
		protected InputBankTest inputBank
		{
			get
			{
				return this.outer.commonComponents.inputBank;
			}
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06000360 RID: 864 RVA: 0x0000D8AA File Offset: 0x0000BAAA
		protected TeamComponent teamComponent
		{
			get
			{
				return this.outer.commonComponents.teamComponent;
			}
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x06000361 RID: 865 RVA: 0x0000D8BC File Offset: 0x0000BABC
		protected HealthComponent healthComponent
		{
			get
			{
				return this.outer.commonComponents.healthComponent;
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x06000362 RID: 866 RVA: 0x0000D8CE File Offset: 0x0000BACE
		protected SkillLocator skillLocator
		{
			get
			{
				return this.outer.commonComponents.skillLocator;
			}
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06000363 RID: 867 RVA: 0x0000D8E0 File Offset: 0x0000BAE0
		protected CharacterEmoteDefinitions characterEmoteDefinitions
		{
			get
			{
				return this.outer.commonComponents.characterEmoteDefinitions;
			}
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x06000364 RID: 868 RVA: 0x0000D8F2 File Offset: 0x0000BAF2
		protected CameraTargetParams cameraTargetParams
		{
			get
			{
				return this.outer.commonComponents.cameraTargetParams;
			}
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000365 RID: 869 RVA: 0x0000D904 File Offset: 0x0000BB04
		protected SfxLocator sfxLocator
		{
			get
			{
				return this.outer.commonComponents.sfxLocator;
			}
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06000366 RID: 870 RVA: 0x0000D916 File Offset: 0x0000BB16
		protected BodyAnimatorSmoothingParameters bodyAnimatorSmoothingParameters
		{
			get
			{
				return this.outer.commonComponents.bodyAnimatorSmoothingParameters;
			}
		}

		// Token: 0x06000367 RID: 871 RVA: 0x0000D928 File Offset: 0x0000BB28
		protected Transform GetModelBaseTransform()
		{
			if (!this.modelLocator)
			{
				return null;
			}
			return this.modelLocator.modelBaseTransform;
		}

		// Token: 0x06000368 RID: 872 RVA: 0x0000D944 File Offset: 0x0000BB44
		protected Transform GetModelTransform()
		{
			if (!this.modelLocator)
			{
				return null;
			}
			return this.modelLocator.modelTransform;
		}

		// Token: 0x06000369 RID: 873 RVA: 0x0000D960 File Offset: 0x0000BB60
		protected Animator GetModelAnimator()
		{
			if (!this.modelLocator)
			{
				return null;
			}
			return this.modelLocator.modelTransform.GetComponent<Animator>();
		}

		// Token: 0x0600036A RID: 874 RVA: 0x0000D981 File Offset: 0x0000BB81
		protected RootMotionAccumulator GetModelRootMotionAccumulator()
		{
			if (!this.modelLocator)
			{
				return null;
			}
			return this.modelLocator.modelTransform.GetComponent<RootMotionAccumulator>();
		}

		// Token: 0x0600036B RID: 875 RVA: 0x0000D9A4 File Offset: 0x0000BBA4
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
				int layerIndex = modelAnimator.GetLayerIndex(layerName);
				modelAnimator.SetFloat(playbackRateParam, 1f);
				modelAnimator.PlayInFixedTime(animationStateName, layerIndex, 0f);
				modelAnimator.Update(0f);
				float length = modelAnimator.GetCurrentAnimatorStateInfo(layerIndex).length;
				modelAnimator.SetFloat(playbackRateParam, length / duration);
			}
		}

		// Token: 0x0600036C RID: 876 RVA: 0x0000DA30 File Offset: 0x0000BC30
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

		// Token: 0x0600036D RID: 877 RVA: 0x0000DAB8 File Offset: 0x0000BCB8
		protected void PlayCrossfade(string layerName, string animationStateName, float crossfadeDuration)
		{
			Animator modelAnimator = this.GetModelAnimator();
			if (modelAnimator)
			{
				int layerIndex = modelAnimator.GetLayerIndex(layerName);
				modelAnimator.CrossFadeInFixedTime(animationStateName, crossfadeDuration, layerIndex);
			}
		}

		// Token: 0x0600036E RID: 878 RVA: 0x0000DAE8 File Offset: 0x0000BCE8
		protected void PlayAnimation(string layerName, string animationStateName)
		{
			Animator modelAnimator = this.GetModelAnimator();
			if (modelAnimator)
			{
				int layerIndex = modelAnimator.GetLayerIndex(layerName);
				modelAnimator.PlayInFixedTime(animationStateName, layerIndex, 0f);
			}
		}

		// Token: 0x0600036F RID: 879 RVA: 0x0000DB19 File Offset: 0x0000BD19
		protected void GetBodyAnimatorSmoothingParameters(out BodyAnimatorSmoothingParameters.SmoothingParameters smoothingParameters)
		{
			if (this.bodyAnimatorSmoothingParameters)
			{
				smoothingParameters = this.bodyAnimatorSmoothingParameters.smoothingParameters;
				return;
			}
			smoothingParameters = BodyAnimatorSmoothingParameters.defaultParameters;
		}

		// Token: 0x04000326 RID: 806
		public EntityStateMachine outer;
	}
}
