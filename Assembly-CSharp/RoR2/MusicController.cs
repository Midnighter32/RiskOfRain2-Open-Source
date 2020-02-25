using System;
using System.Collections.ObjectModel;
using RoR2.WwiseUtils;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RoR2
{
	// Token: 0x02000294 RID: 660
	public class MusicController : MonoBehaviour
	{
		// Token: 0x06000EAF RID: 3759 RVA: 0x000411E0 File Offset: 0x0003F3E0
		private void InitializeEngineDependentValues()
		{
			this.rtpcPlayerHealthValue = new RtpcSetter("playerHealth", null);
			this.rtpcEnemyValue = new RtpcSetter("enemyValue", null);
			this.rtpcTeleporterProximityValue = new RtpcSetter("teleporterProximity", null);
			this.rtpcTeleporterDirectionValue = new RtpcSetter("teleporterDirection", null);
			this.rtpcTeleporterCharged = new RtpcSetter("", null);
			this.rtpcTeleporterPlayerStatus = new RtpcSetter("teleporterPlayerStatus", null);
			this.stMusicSystem = new StateSetter("Music_system");
			this.stGameplaySongChoice = new StateSetter("gameplaySongChoice");
			this.stMusicMenu = new StateSetter("Music_menu");
			this.stBossStatus = new StateSetter("bossStatus");
		}

		// Token: 0x06000EB0 RID: 3760 RVA: 0x00041293 File Offset: 0x0003F493
		private void RefreshStageInfo(Scene a, Scene b)
		{
			this.stageInfo = default(MusicController.StageInfo);
		}

		// Token: 0x06000EB1 RID: 3761 RVA: 0x000412A4 File Offset: 0x0003F4A4
		private void Start()
		{
			this.enemyInfoBuffer = new NativeArray<MusicController.EnemyInfo>(64, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			SceneManager.activeSceneChanged += this.RefreshStageInfo;
			this.InitializeEngineDependentValues();
			if (this.enableMusicSystem)
			{
				AkSoundEngine.PostEvent("Play_Music_System", base.gameObject);
			}
		}

		// Token: 0x06000EB2 RID: 3762 RVA: 0x000412F0 File Offset: 0x0003F4F0
		private void Update()
		{
			this.UpdateState();
			this.targetCamera = ((CameraRigController.readOnlyInstancesList.Count > 0) ? CameraRigController.readOnlyInstancesList[0] : null);
			this.target = (this.targetCamera ? this.targetCamera.target : null);
			this.ScheduleIntensityCalculation(this.target);
		}

		// Token: 0x06000EB3 RID: 3763 RVA: 0x00041354 File Offset: 0x0003F554
		private void RecalculateHealth(GameObject playerObject)
		{
			this.rtpcPlayerHealthValue.value = 100f;
			if (this.target)
			{
				CharacterBody component = this.target.GetComponent<CharacterBody>();
				if (component)
				{
					if (component.HasBuff(BuffIndex.Deafened))
					{
						this.rtpcPlayerHealthValue.value = -100f;
						return;
					}
					HealthComponent healthComponent = component.healthComponent;
					if (healthComponent)
					{
						this.rtpcPlayerHealthValue.value = healthComponent.combinedHealthFraction * 100f;
					}
				}
			}
		}

		// Token: 0x06000EB4 RID: 3764 RVA: 0x000413D4 File Offset: 0x0003F5D4
		private void UpdateTeleporterParameters(TeleporterInteraction teleporter)
		{
			float num = 0.5f;
			this.rtpcTeleporterProximityValue.value = float.PositiveInfinity;
			this.rtpcTeleporterDirectionValue.value = 0f;
			this.rtpcTeleporterCharged.value = 100f;
			if (teleporter)
			{
				if (this.targetCamera)
				{
					Vector3 position = this.targetCamera.transform.position;
					Vector3 forward = this.targetCamera.transform.forward;
					Vector3 vector = teleporter.transform.position - position;
					float num2 = Vector2.SignedAngle(new Vector2(vector.x, vector.z), new Vector2(forward.x, forward.z));
					if (num2 < 0f)
					{
						num2 += 360f;
					}
					this.rtpcTeleporterProximityValue.value = vector.magnitude;
					this.rtpcTeleporterDirectionValue.value = num2;
				}
				this.rtpcTeleporterProximityValue.value = Mathf.Clamp(this.rtpcTeleporterProximityValue.value, 20f, 250f);
				this.rtpcTeleporterProximityValue.value = Util.Remap(this.rtpcTeleporterProximityValue.value, 20f, 250f, 0f, 10000f);
				this.rtpcTeleporterCharged.value = teleporter.chargeFraction * 90f / num;
			}
		}

		// Token: 0x06000EB5 RID: 3765 RVA: 0x00041530 File Offset: 0x0003F730
		private void LateUpdate()
		{
			bool flag = Time.timeScale == 0f;
			if (this.wasPaused != flag)
			{
				AkSoundEngine.PostEvent(flag ? "Pause_Music" : "Unpause_Music", base.gameObject);
				this.wasPaused = flag;
			}
			this.RecalculateHealth(this.target);
			this.UpdateTeleporterParameters(TeleporterInteraction.instance);
			this.calculateIntensityJobHandle.Complete();
			float num;
			float num2;
			this.calculateIntensityJob.CalculateSum(out num, out num2);
			float num3 = 0.025f;
			Mathf.Clamp(1f - this.rtpcPlayerHealthValue.value * 0.01f, 0.25f, 0.75f);
			float value = (num * 0.75f + num2 * 0.25f) * num3;
			this.rtpcEnemyValue.value = value;
			this.FlushValuesToEngine();
		}

		// Token: 0x06000EB6 RID: 3766 RVA: 0x000415FC File Offset: 0x0003F7FC
		private void FlushValuesToEngine()
		{
			this.stMusicSystem.FlushIfChanged();
			this.stGameplaySongChoice.FlushIfChanged();
			this.stMusicMenu.FlushIfChanged();
			this.stBossStatus.FlushIfChanged();
			this.rtpcPlayerHealthValue.FlushIfChanged();
			this.rtpcTeleporterProximityValue.FlushIfChanged();
			this.rtpcTeleporterDirectionValue.FlushIfChanged();
			this.rtpcTeleporterPlayerStatus.FlushIfChanged();
			this.rtpcEnemyValue.FlushIfChanged();
		}

		// Token: 0x06000EB7 RID: 3767 RVA: 0x0004166C File Offset: 0x0003F86C
		private void UpdateState()
		{
			this.stGameplaySongChoice.valueId = CommonWwiseIds.none;
			this.stMusicSystem.valueId = CommonWwiseIds.none;
			this.stMusicMenu.valueId = CommonWwiseIds.none;
			this.stBossStatus.valueId = CommonWwiseIds.none;
			this.rtpcTeleporterPlayerStatus.value = 1f;
			SceneDef mostRecentSceneDef = SceneCatalog.mostRecentSceneDef;
			if (mostRecentSceneDef)
			{
				string baseSceneName = mostRecentSceneDef.baseSceneName;
				if (baseSceneName == "title")
				{
					this.stMusicSystem.valueId = CommonWwiseIds.menu;
					this.stMusicMenu.valueId = CommonWwiseIds.main;
					return;
				}
				if (baseSceneName == "lobby")
				{
					this.stMusicSystem.valueId = CommonWwiseIds.menu;
					this.stMusicMenu.valueId = CommonWwiseIds.main;
					return;
				}
				if (baseSceneName == "logbook")
				{
					this.stMusicSystem.valueId = CommonWwiseIds.menu;
					this.stMusicMenu.valueId = CommonWwiseIds.logbook;
					return;
				}
				if (baseSceneName == "crystalworld")
				{
					this.stMusicSystem.valueId = CommonWwiseIds.menu;
					this.stMusicMenu.valueId = CommonWwiseIds.logbook;
					return;
				}
				if (baseSceneName == "bazaar")
				{
					this.stMusicSystem.valueId = CommonWwiseIds.secretLevel;
					return;
				}
				this.stMusicSystem.valueId = CommonWwiseIds.gameplay;
				if (mostRecentSceneDef)
				{
					this.stGameplaySongChoice.valueId = AkSoundEngine.GetIDFromString(mostRecentSceneDef.songName);
				}
				if (TeleporterInteraction.instance && !TeleporterInteraction.instance.isIdle)
				{
					this.stGameplaySongChoice.valueId = AkSoundEngine.GetIDFromString(mostRecentSceneDef.bossSongName);
					this.stBossStatus.valueId = CommonWwiseIds.alive;
					this.stMusicSystem.valueId = CommonWwiseIds.bossfight;
					if (TeleporterInteraction.instance.isIdleToCharging || TeleporterInteraction.instance.isCharging)
					{
						if (this.target)
						{
							this.rtpcTeleporterPlayerStatus.value = 0f;
							if (TeleporterInteraction.instance.IsInChargingRange(this.target))
							{
								this.rtpcTeleporterPlayerStatus.value = 1f;
								return;
							}
						}
					}
					else if (TeleporterInteraction.instance.isCharged)
					{
						this.stBossStatus.valueId = CommonWwiseIds.dead;
					}
				}
			}
		}

		// Token: 0x06000EB8 RID: 3768 RVA: 0x000418B5 File Offset: 0x0003FAB5
		private void EnsureEnemyBufferSize(int requiredSize)
		{
			if (this.enemyInfoBuffer.Length < requiredSize)
			{
				if (this.enemyInfoBuffer.Length != 0)
				{
					this.enemyInfoBuffer.Dispose();
				}
				this.enemyInfoBuffer = new NativeArray<MusicController.EnemyInfo>(requiredSize, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			}
		}

		// Token: 0x06000EB9 RID: 3769 RVA: 0x000418EB File Offset: 0x0003FAEB
		private void OnDestroy()
		{
			SceneManager.activeSceneChanged -= this.RefreshStageInfo;
			this.enemyInfoBuffer.Dispose();
		}

		// Token: 0x06000EBA RID: 3770 RVA: 0x0004190C File Offset: 0x0003FB0C
		private void ScheduleIntensityCalculation(GameObject targetBodyObject)
		{
			if (!targetBodyObject)
			{
				return;
			}
			ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Monster);
			int count = teamMembers.Count;
			this.EnsureEnemyBufferSize(count);
			int num = 0;
			int i = 0;
			int num2 = count;
			while (i < num2)
			{
				TeamComponent teamComponent = teamMembers[i];
				InputBankTest component = teamComponent.GetComponent<InputBankTest>();
				CharacterBody component2 = teamComponent.GetComponent<CharacterBody>();
				if (component)
				{
					this.enemyInfoBuffer[num++] = new MusicController.EnemyInfo
					{
						aimRay = new Ray(component.aimOrigin, component.aimDirection),
						threatScore = (component2.master ? component2.GetNormalizedThreatValue() : 0f)
					};
				}
				i++;
			}
			this.calculateIntensityJob = new MusicController.CalculateIntensityJob
			{
				enemyInfoBuffer = this.enemyInfoBuffer,
				elementCount = num,
				targetPosition = targetBodyObject.transform.position,
				nearDistance = 20f,
				farDistance = 75f
			};
			this.calculateIntensityJobHandle = this.calculateIntensityJob.Schedule(num, 32, default(JobHandle));
		}

		// Token: 0x06000EBB RID: 3771 RVA: 0x00041A2C File Offset: 0x0003FC2C
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			RoR2Application.onLoad = (Action)Delegate.Combine(RoR2Application.onLoad, new Action(delegate()
			{
				UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/MusicController"), RoR2Application.instance.transform);
			}));
		}

		// Token: 0x04000E93 RID: 3731
		public GameObject target;

		// Token: 0x04000E94 RID: 3732
		public bool enableMusicSystem = true;

		// Token: 0x04000E95 RID: 3733
		private CameraRigController targetCamera;

		// Token: 0x04000E96 RID: 3734
		private RtpcSetter rtpcPlayerHealthValue;

		// Token: 0x04000E97 RID: 3735
		private RtpcSetter rtpcEnemyValue;

		// Token: 0x04000E98 RID: 3736
		private RtpcSetter rtpcTeleporterProximityValue;

		// Token: 0x04000E99 RID: 3737
		private RtpcSetter rtpcTeleporterDirectionValue;

		// Token: 0x04000E9A RID: 3738
		private RtpcSetter rtpcTeleporterCharged;

		// Token: 0x04000E9B RID: 3739
		private RtpcSetter rtpcTeleporterPlayerStatus;

		// Token: 0x04000E9C RID: 3740
		private StateSetter stMusicSystem;

		// Token: 0x04000E9D RID: 3741
		private StateSetter stGameplaySongChoice;

		// Token: 0x04000E9E RID: 3742
		private StateSetter stMusicMenu;

		// Token: 0x04000E9F RID: 3743
		private StateSetter stBossStatus;

		// Token: 0x04000EA0 RID: 3744
		private MusicController.StageInfo stageInfo;

		// Token: 0x04000EA1 RID: 3745
		private bool wasPaused;

		// Token: 0x04000EA2 RID: 3746
		private NativeArray<MusicController.EnemyInfo> enemyInfoBuffer;

		// Token: 0x04000EA3 RID: 3747
		private MusicController.CalculateIntensityJob calculateIntensityJob;

		// Token: 0x04000EA4 RID: 3748
		private JobHandle calculateIntensityJobHandle;

		// Token: 0x02000295 RID: 661
		private struct StageInfo
		{
			// Token: 0x04000EA5 RID: 3749
			public bool inAction;

			// Token: 0x04000EA6 RID: 3750
			public bool inIntro;
		}

		// Token: 0x02000296 RID: 662
		private struct EnemyInfo
		{
			// Token: 0x04000EA7 RID: 3751
			public Ray aimRay;

			// Token: 0x04000EA8 RID: 3752
			public float lookScore;

			// Token: 0x04000EA9 RID: 3753
			public float proximityScore;

			// Token: 0x04000EAA RID: 3754
			public float threatScore;
		}

		// Token: 0x02000297 RID: 663
		private struct CalculateIntensityJob : IJobParallelFor
		{
			// Token: 0x06000EBD RID: 3773 RVA: 0x00041A70 File Offset: 0x0003FC70
			public void Execute(int i)
			{
				MusicController.EnemyInfo enemyInfo = this.enemyInfoBuffer[i];
				Vector3 a = this.targetPosition - enemyInfo.aimRay.origin;
				float magnitude = a.magnitude;
				float num = Mathf.Clamp01(Vector3.Dot(a / magnitude, enemyInfo.aimRay.direction));
				float num2 = Mathf.Clamp01(Mathf.InverseLerp(this.farDistance, this.nearDistance, magnitude));
				enemyInfo.lookScore = num * enemyInfo.threatScore;
				enemyInfo.proximityScore = num2 * enemyInfo.threatScore;
				this.enemyInfoBuffer[i] = enemyInfo;
			}

			// Token: 0x06000EBE RID: 3774 RVA: 0x00041B10 File Offset: 0x0003FD10
			public void CalculateSum(out float proximityScore, out float lookScore)
			{
				proximityScore = 0f;
				lookScore = 0f;
				for (int i = 0; i < this.elementCount; i++)
				{
					proximityScore += this.enemyInfoBuffer[i].proximityScore;
					lookScore += this.enemyInfoBuffer[i].lookScore;
				}
			}

			// Token: 0x04000EAB RID: 3755
			[ReadOnly]
			public Vector3 targetPosition;

			// Token: 0x04000EAC RID: 3756
			[ReadOnly]
			public int elementCount;

			// Token: 0x04000EAD RID: 3757
			public NativeArray<MusicController.EnemyInfo> enemyInfoBuffer;

			// Token: 0x04000EAE RID: 3758
			[ReadOnly]
			public float nearDistance;

			// Token: 0x04000EAF RID: 3759
			[ReadOnly]
			public float farDistance;
		}
	}
}
