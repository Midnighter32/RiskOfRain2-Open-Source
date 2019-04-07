using System;
using System.Collections.ObjectModel;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RoR2
{
	// Token: 0x02000363 RID: 867
	public class MusicController : MonoBehaviour
	{
		// Token: 0x060011D5 RID: 4565 RVA: 0x000583AE File Offset: 0x000565AE
		private void RefreshStageInfo(Scene a, Scene b)
		{
			this.stageInfo = default(MusicController.StageInfo);
		}

		// Token: 0x060011D6 RID: 4566 RVA: 0x000583BC File Offset: 0x000565BC
		private void Start()
		{
			this.enemyInfoBuffer = new NativeArray<MusicController.EnemyInfo>(64, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			SceneManager.activeSceneChanged += this.RefreshStageInfo;
			if (this.enableMusicSystem)
			{
				AkSoundEngine.PostEvent("Play_Music_System", base.gameObject);
			}
		}

		// Token: 0x060011D7 RID: 4567 RVA: 0x000583F8 File Offset: 0x000565F8
		private void Update()
		{
			this.UpdateState();
			this.targetCamera = ((CameraRigController.readOnlyInstancesList.Count > 0) ? CameraRigController.readOnlyInstancesList[0] : null);
			this.target = (this.targetCamera ? this.targetCamera.target : null);
			this.ScheduleIntensityCalculation(this.target);
		}

		// Token: 0x060011D8 RID: 4568 RVA: 0x0005845C File Offset: 0x0005665C
		private void RecalculateHealth(GameObject playerObject)
		{
			this.rtpcPlayerHealthValue = 1f;
			if (this.target)
			{
				HealthComponent component = this.target.GetComponent<HealthComponent>();
				if (component)
				{
					this.rtpcPlayerHealthValue = component.combinedHealthFraction;
				}
			}
		}

		// Token: 0x060011D9 RID: 4569 RVA: 0x000584A4 File Offset: 0x000566A4
		private void UpdateTeleporterParameters(TeleporterInteraction teleporter)
		{
			float num = 0.5f;
			this.rtpcTeleporterProximityValue = float.PositiveInfinity;
			this.rtpcTeleporterDirectionValue = 0f;
			this.rtpcTeleporterCharged = 100f;
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
					this.rtpcTeleporterProximityValue = vector.magnitude;
					this.rtpcTeleporterDirectionValue = num2;
				}
				this.rtpcTeleporterProximityValue /= num;
				this.rtpcTeleporterCharged = teleporter.chargeFraction * 90f / num;
			}
		}

		// Token: 0x060011DA RID: 4570 RVA: 0x0005859C File Offset: 0x0005679C
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
			Mathf.Clamp(1f - this.rtpcPlayerHealthValue, 0.25f, 0.75f);
			float num4 = (num * 0.75f + num2 * 0.25f) * num3;
			this.rtpcEnemyValue = num4;
			this.UpdateRTPCValues();
		}

		// Token: 0x060011DB RID: 4571 RVA: 0x00058658 File Offset: 0x00056858
		private void UpdateRTPCValues()
		{
			AkSoundEngine.SetRTPCValue("playerHealth", this.rtpcPlayerHealthValue * 100f);
			AkSoundEngine.SetRTPCValue("teleporterProximity", this.rtpcTeleporterProximityValue);
			AkSoundEngine.SetRTPCValue("teleporterDirection", this.rtpcTeleporterDirectionValue);
			AkSoundEngine.SetRTPCValue("teleporterPlayerStatus", this.rtpcTeleporterPlayerStatus);
			AkSoundEngine.SetRTPCValue("enemyValue", this.rtpcEnemyValue);
		}

		// Token: 0x060011DC RID: 4572 RVA: 0x000586C0 File Offset: 0x000568C0
		private void UpdateState()
		{
			string in_pszState = "None";
			string in_pszState2 = "None";
			string in_pszState3 = "None";
			string in_pszState4 = "None";
			this.rtpcTeleporterPlayerStatus = 1f;
			SceneDef mostRecentSceneDef = SceneCatalog.mostRecentSceneDef;
			if (mostRecentSceneDef)
			{
				string sceneName = mostRecentSceneDef.sceneName;
				if (!(sceneName == "title"))
				{
					if (!(sceneName == "lobby"))
					{
						if (!(sceneName == "logbook"))
						{
							if (!(sceneName == "crystalworld"))
							{
								if (!(sceneName == "bazaar"))
								{
									in_pszState2 = "Gameplay";
									if (mostRecentSceneDef)
									{
										in_pszState = mostRecentSceneDef.songName;
									}
									if (TeleporterInteraction.instance && !TeleporterInteraction.instance.isIdle)
									{
										in_pszState = mostRecentSceneDef.bossSongName;
										in_pszState4 = "alive";
										in_pszState2 = "Bossfight";
										if (TeleporterInteraction.instance.isIdleToCharging || TeleporterInteraction.instance.isCharging)
										{
											if (this.target)
											{
												this.rtpcTeleporterPlayerStatus = 0f;
												if (TeleporterInteraction.instance.IsInChargingRange(this.target))
												{
													this.rtpcTeleporterPlayerStatus = 1f;
												}
											}
										}
										else if (TeleporterInteraction.instance.isCharged)
										{
											in_pszState4 = "dead";
										}
									}
								}
								else
								{
									in_pszState2 = "SecretLevel";
								}
							}
							else
							{
								in_pszState2 = "Menu";
								in_pszState3 = "Logbook";
							}
						}
						else
						{
							in_pszState2 = "Menu";
							in_pszState3 = "Logbook";
						}
					}
					else
					{
						in_pszState2 = "Menu";
						in_pszState3 = "Main";
					}
				}
				else
				{
					in_pszState2 = "Menu";
					in_pszState3 = "Main";
				}
			}
			AkSoundEngine.SetState("Music_system", in_pszState2);
			AkSoundEngine.SetState("gameplaySongChoice", in_pszState);
			AkSoundEngine.SetState("Music_menu", in_pszState3);
			AkSoundEngine.SetState("bossStatus", in_pszState4);
		}

		// Token: 0x060011DD RID: 4573 RVA: 0x0005887A File Offset: 0x00056A7A
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

		// Token: 0x060011DE RID: 4574 RVA: 0x000588B0 File Offset: 0x00056AB0
		private void OnDestroy()
		{
			SceneManager.activeSceneChanged -= this.RefreshStageInfo;
			this.enemyInfoBuffer.Dispose();
		}

		// Token: 0x060011DF RID: 4575 RVA: 0x000588D0 File Offset: 0x00056AD0
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

		// Token: 0x060011E0 RID: 4576 RVA: 0x000589F0 File Offset: 0x00056BF0
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			RoR2Application.onLoad = (Action)Delegate.Combine(RoR2Application.onLoad, new Action(delegate()
			{
				UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/MusicController"), RoR2Application.instance.transform);
			}));
		}

		// Token: 0x040015EB RID: 5611
		public GameObject target;

		// Token: 0x040015EC RID: 5612
		public bool enableMusicSystem = true;

		// Token: 0x040015ED RID: 5613
		private CameraRigController targetCamera;

		// Token: 0x040015EE RID: 5614
		private float rtpcPlayerHealthValue;

		// Token: 0x040015EF RID: 5615
		private float rtpcEnemyValue;

		// Token: 0x040015F0 RID: 5616
		private float rtpcTeleporterProximityValue;

		// Token: 0x040015F1 RID: 5617
		private float rtpcTeleporterDirectionValue;

		// Token: 0x040015F2 RID: 5618
		private float rtpcTeleporterCharged;

		// Token: 0x040015F3 RID: 5619
		private float rtpcTeleporterPlayerStatus;

		// Token: 0x040015F4 RID: 5620
		private MusicController.StageInfo stageInfo;

		// Token: 0x040015F5 RID: 5621
		private bool wasPaused;

		// Token: 0x040015F6 RID: 5622
		private NativeArray<MusicController.EnemyInfo> enemyInfoBuffer;

		// Token: 0x040015F7 RID: 5623
		private MusicController.CalculateIntensityJob calculateIntensityJob;

		// Token: 0x040015F8 RID: 5624
		private JobHandle calculateIntensityJobHandle;

		// Token: 0x02000364 RID: 868
		private struct StageInfo
		{
			// Token: 0x040015F9 RID: 5625
			public bool inAction;

			// Token: 0x040015FA RID: 5626
			public bool inIntro;
		}

		// Token: 0x02000365 RID: 869
		private struct EnemyInfo
		{
			// Token: 0x040015FB RID: 5627
			public Ray aimRay;

			// Token: 0x040015FC RID: 5628
			public float lookScore;

			// Token: 0x040015FD RID: 5629
			public float proximityScore;

			// Token: 0x040015FE RID: 5630
			public float threatScore;
		}

		// Token: 0x02000366 RID: 870
		private struct CalculateIntensityJob : IJobParallelFor
		{
			// Token: 0x060011E2 RID: 4578 RVA: 0x00058A34 File Offset: 0x00056C34
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

			// Token: 0x060011E3 RID: 4579 RVA: 0x00058AD4 File Offset: 0x00056CD4
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

			// Token: 0x040015FF RID: 5631
			[ReadOnly]
			public Vector3 targetPosition;

			// Token: 0x04001600 RID: 5632
			[ReadOnly]
			public int elementCount;

			// Token: 0x04001601 RID: 5633
			public NativeArray<MusicController.EnemyInfo> enemyInfoBuffer;

			// Token: 0x04001602 RID: 5634
			[ReadOnly]
			public float nearDistance;

			// Token: 0x04001603 RID: 5635
			[ReadOnly]
			public float farDistance;
		}
	}
}
