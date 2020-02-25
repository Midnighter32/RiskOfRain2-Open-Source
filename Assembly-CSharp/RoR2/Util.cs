using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Rewired;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200047B RID: 1147
	public static class Util
	{
		// Token: 0x06001BD3 RID: 7123 RVA: 0x00076A74 File Offset: 0x00074C74
		public static WeightedSelection<DirectorCard> CreateReasonableDirectorCardSpawnList(float availableCredit, int maximumNumberToSpawnBeforeSkipping, int minimumToSpawn)
		{
			WeightedSelection<DirectorCard> monsterSelection = ClassicStageInfo.instance.monsterSelection;
			WeightedSelection<DirectorCard> weightedSelection = new WeightedSelection<DirectorCard>(8);
			float highestEliteCostMultiplier = CombatDirector.highestEliteCostMultiplier;
			for (int i = 0; i < monsterSelection.Count; i++)
			{
				DirectorCard value = monsterSelection.choices[i].value;
				float num = (float)(value.cost * maximumNumberToSpawnBeforeSkipping) * ((value.spawnCard as CharacterSpawnCard).noElites ? 1f : highestEliteCostMultiplier);
				if (value.CardIsValid() && (float)value.cost * (float)minimumToSpawn <= availableCredit && num / 2f > availableCredit)
				{
					weightedSelection.AddChoice(value, monsterSelection.choices[i].weight);
				}
			}
			return weightedSelection;
		}

		// Token: 0x06001BD4 RID: 7124 RVA: 0x00076B24 File Offset: 0x00074D24
		public static CharacterBody HurtBoxColliderToBody(Collider collider)
		{
			HurtBox component = collider.GetComponent<HurtBox>();
			if (component && component.healthComponent)
			{
				return component.healthComponent.body;
			}
			return null;
		}

		// Token: 0x06001BD5 RID: 7125 RVA: 0x00076B5A File Offset: 0x00074D5A
		public static float ConvertAmplificationPercentageIntoReductionPercentage(float amplificationPercentage)
		{
			return (1f - 100f / (100f + amplificationPercentage)) * 100f;
		}

		// Token: 0x06001BD6 RID: 7126 RVA: 0x00076B78 File Offset: 0x00074D78
		public static Vector3 ClosestPointOnLine(Vector3 vA, Vector3 vB, Vector3 vPoint)
		{
			Vector3 rhs = vPoint - vA;
			Vector3 normalized = (vB - vA).normalized;
			float num = Vector3.Distance(vA, vB);
			float num2 = Vector3.Dot(normalized, rhs);
			if (num2 <= 0f)
			{
				return vA;
			}
			if (num2 >= num)
			{
				return vB;
			}
			Vector3 b = normalized * num2;
			return vA + b;
		}

		// Token: 0x06001BD7 RID: 7127 RVA: 0x00076BD0 File Offset: 0x00074DD0
		public static CharacterBody TryToCreateGhost(CharacterBody targetBody, CharacterBody ownerBody, int duration)
		{
			Util.<>c__DisplayClass4_0 CS$<>8__locals1 = new Util.<>c__DisplayClass4_0();
			CS$<>8__locals1.targetBody = targetBody;
			CS$<>8__locals1.ownerBody = ownerBody;
			CS$<>8__locals1.duration = duration;
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'RoR2.CharacterBody RoR2.Util::TryToCreateGhost(RoR2.CharacterBody, RoR2.CharacterBody, int)' called on client");
				return null;
			}
			if (!CS$<>8__locals1.targetBody)
			{
				return null;
			}
			CS$<>8__locals1.bodyPrefab = BodyCatalog.FindBodyPrefab(CS$<>8__locals1.targetBody);
			if (!CS$<>8__locals1.bodyPrefab)
			{
				return null;
			}
			CharacterMaster characterMaster = MasterCatalog.allAiMasters.FirstOrDefault((CharacterMaster master) => master.bodyPrefab == CS$<>8__locals1.bodyPrefab);
			if (!characterMaster)
			{
				return null;
			}
			MasterSummon masterSummon = new MasterSummon();
			masterSummon.masterPrefab = characterMaster.gameObject;
			masterSummon.ignoreTeamMemberLimit = false;
			masterSummon.position = CS$<>8__locals1.targetBody.footPosition;
			CharacterDirection component = CS$<>8__locals1.targetBody.GetComponent<CharacterDirection>();
			masterSummon.rotation = (component ? Quaternion.Euler(0f, component.yaw, 0f) : CS$<>8__locals1.targetBody.transform.rotation);
			masterSummon.summonerBodyObject = (CS$<>8__locals1.ownerBody ? CS$<>8__locals1.ownerBody.gameObject : null);
			masterSummon.preSpawnSetupCallback = new Action<CharacterMaster>(CS$<>8__locals1.<TryToCreateGhost>g__PreSpawnSetup|1);
			CharacterMaster characterMaster2 = masterSummon.Perform();
			if (!characterMaster2)
			{
				return null;
			}
			CharacterBody body = characterMaster2.GetBody();
			if (body)
			{
				foreach (EntityStateMachine entityStateMachine in body.GetComponents<EntityStateMachine>())
				{
					entityStateMachine.initialStateType = entityStateMachine.mainStateType;
				}
			}
			return body;
		}

		// Token: 0x06001BD8 RID: 7128 RVA: 0x00076D48 File Offset: 0x00074F48
		public static float OnHitProcDamage(float damageThatProccedIt, float damageStat, float damageCoefficient)
		{
			float b = damageThatProccedIt * damageCoefficient;
			return Mathf.Max(1f, b);
		}

		// Token: 0x06001BD9 RID: 7129 RVA: 0x00076D64 File Offset: 0x00074F64
		public static float OnKillProcDamage(float baseDamage, float damageCoefficient)
		{
			return baseDamage * damageCoefficient;
		}

		// Token: 0x06001BDA RID: 7130 RVA: 0x00076D6C File Offset: 0x00074F6C
		public static Quaternion QuaternionSafeLookRotation(Vector3 forward)
		{
			Quaternion result = Quaternion.identity;
			if (forward.sqrMagnitude > Mathf.Epsilon)
			{
				result = Quaternion.LookRotation(forward);
			}
			return result;
		}

		// Token: 0x06001BDB RID: 7131 RVA: 0x00076D98 File Offset: 0x00074F98
		public static Quaternion QuaternionSafeLookRotation(Vector3 forward, Vector3 upwards)
		{
			Quaternion result = Quaternion.identity;
			if (forward.sqrMagnitude > Mathf.Epsilon)
			{
				result = Quaternion.LookRotation(forward, upwards);
			}
			return result;
		}

		// Token: 0x06001BDC RID: 7132 RVA: 0x00076DC4 File Offset: 0x00074FC4
		public static bool HasParameterOfType(Animator animator, string name, AnimatorControllerParameterType type)
		{
			foreach (AnimatorControllerParameter animatorControllerParameter in animator.parameters)
			{
				if (animatorControllerParameter.type == type && animatorControllerParameter.name == name)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001BDD RID: 7133 RVA: 0x00076E04 File Offset: 0x00075004
		public static uint PlaySound(string soundString, GameObject gameObject)
		{
			if (string.IsNullOrEmpty(soundString))
			{
				return 0U;
			}
			return AkSoundEngine.PostEvent(soundString, gameObject);
		}

		// Token: 0x06001BDE RID: 7134 RVA: 0x00076E18 File Offset: 0x00075018
		public static uint PlaySound(string soundString, GameObject gameObject, string RTPCstring, float RTPCvalue)
		{
			uint num = Util.PlaySound(soundString, gameObject);
			if (num != 0U)
			{
				AkSoundEngine.SetRTPCValueByPlayingID(RTPCstring, RTPCvalue, num);
			}
			return num;
		}

		// Token: 0x06001BDF RID: 7135 RVA: 0x00076E3C File Offset: 0x0007503C
		public static uint PlayScaledSound(string soundString, GameObject gameObject, float playbackRate)
		{
			uint num = Util.PlaySound(soundString, gameObject);
			if (num != 0U)
			{
				float num2 = Mathf.Log(playbackRate, 2f);
				float in_value = 1200f * num2 / 96f + 50f;
				AkSoundEngine.SetRTPCValueByPlayingID("attackSpeed", in_value, num);
			}
			return num;
		}

		// Token: 0x06001BE0 RID: 7136 RVA: 0x00076E84 File Offset: 0x00075084
		public static void RotateAwayFromWalls(float raycastLength, int raycastCount, Vector3 raycastOrigin, Transform referenceTransform)
		{
			float num = 360f / (float)raycastCount;
			float angle = 0f;
			float num2 = 0f;
			for (int i = 0; i < raycastCount; i++)
			{
				Vector3 direction = Quaternion.Euler(0f, num * (float)i, 0f) * Vector3.forward;
				float num3 = raycastLength;
				RaycastHit raycastHit;
				if (Physics.Raycast(raycastOrigin, direction, out raycastHit, raycastLength, LayerIndex.world.mask))
				{
					num3 = raycastHit.distance;
				}
				if (raycastHit.distance > num2)
				{
					angle = num * (float)i;
					num2 = num3;
				}
			}
			referenceTransform.Rotate(Vector3.up, angle, Space.Self);
		}

		// Token: 0x06001BE1 RID: 7137 RVA: 0x00076F1C File Offset: 0x0007511C
		public static string GetActionDisplayString(ActionElementMap actionElementMap)
		{
			if (actionElementMap == null)
			{
				return "";
			}
			string elementIdentifierName = actionElementMap.elementIdentifierName;
			if (elementIdentifierName == "Left Mouse Button")
			{
				return "M1";
			}
			if (elementIdentifierName == "Right Mouse Button")
			{
				return "M2";
			}
			if (!(elementIdentifierName == "Left Shift"))
			{
				return actionElementMap.elementIdentifierName;
			}
			return "Shift";
		}

		// Token: 0x06001BE2 RID: 7138 RVA: 0x00076F7A File Offset: 0x0007517A
		public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
		{
			return Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * 57.29578f;
		}

		// Token: 0x06001BE3 RID: 7139 RVA: 0x00017777 File Offset: 0x00015977
		public static float Remap(float value, float inMin, float inMax, float outMin, float outMax)
		{
			return outMin + (value - inMin) / (inMax - inMin) * (outMax - outMin);
		}

		// Token: 0x06001BE4 RID: 7140 RVA: 0x00076F9C File Offset: 0x0007519C
		public static bool HasAnimationParameter(string paramName, Animator animator)
		{
			AnimatorControllerParameter[] parameters = animator.parameters;
			for (int i = 0; i < parameters.Length; i++)
			{
				if (parameters[i].name == paramName)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001BE5 RID: 7141 RVA: 0x00076FD4 File Offset: 0x000751D4
		public static bool HasAnimationParameter(int paramHash, Animator animator)
		{
			int i = 0;
			int parameterCount = animator.parameterCount;
			while (i < parameterCount)
			{
				if (animator.GetParameter(i).nameHash == paramHash)
				{
					return true;
				}
				i++;
			}
			return false;
		}

		// Token: 0x06001BE6 RID: 7142 RVA: 0x00077008 File Offset: 0x00075208
		public static bool CheckRoll(float percentChance, float luck = 0f, CharacterMaster effectOriginMaster = null)
		{
			if (percentChance <= 0f)
			{
				return false;
			}
			int num = Mathf.CeilToInt(Mathf.Abs(luck));
			float num2 = UnityEngine.Random.Range(0f, 100f);
			float num3 = num2;
			for (int i = 0; i < num; i++)
			{
				float b = UnityEngine.Random.Range(0f, 100f);
				num2 = ((luck > 0f) ? Mathf.Min(num2, b) : Mathf.Max(num2, b));
			}
			if (num2 <= percentChance)
			{
				if (num3 > percentChance && effectOriginMaster)
				{
					GameObject bodyObject = effectOriginMaster.GetBodyObject();
					if (bodyObject)
					{
						CharacterBody component = bodyObject.GetComponent<CharacterBody>();
						if (component)
						{
							component.wasLucky = true;
						}
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x06001BE7 RID: 7143 RVA: 0x000770B3 File Offset: 0x000752B3
		public static bool CheckRoll(float percentChance, CharacterMaster master)
		{
			return Util.CheckRoll(percentChance, master ? master.luck : 0f, master);
		}

		// Token: 0x06001BE8 RID: 7144 RVA: 0x000770D4 File Offset: 0x000752D4
		public static float EstimateSurfaceDistance(Collider a, Collider b)
		{
			Vector3 center = a.bounds.center;
			Vector3 center2 = b.bounds.center;
			RaycastHit raycastHit;
			Vector3 a2;
			if (b.Raycast(new Ray(center, center2 - center), out raycastHit, float.PositiveInfinity))
			{
				a2 = raycastHit.point;
			}
			else
			{
				a2 = b.ClosestPointOnBounds(center);
			}
			Vector3 b2;
			if (a.Raycast(new Ray(center2, center - center2), out raycastHit, float.PositiveInfinity))
			{
				b2 = raycastHit.point;
			}
			else
			{
				b2 = a.ClosestPointOnBounds(center2);
			}
			return Vector3.Distance(a2, b2);
		}

		// Token: 0x06001BE9 RID: 7145 RVA: 0x00077167 File Offset: 0x00075367
		public static bool HasEffectiveAuthority(GameObject gameObject)
		{
			return gameObject && Util.HasEffectiveAuthority(gameObject.GetComponent<NetworkIdentity>());
		}

		// Token: 0x06001BEA RID: 7146 RVA: 0x0007717E File Offset: 0x0007537E
		public static bool HasEffectiveAuthority(NetworkIdentity networkIdentity)
		{
			return networkIdentity && (networkIdentity.hasAuthority || (NetworkServer.active && networkIdentity.clientAuthorityOwner == null));
		}

		// Token: 0x06001BEB RID: 7147 RVA: 0x000771A6 File Offset: 0x000753A6
		public static float CalculateSphereVolume(float radius)
		{
			return 4.1887903f * radius * radius * radius;
		}

		// Token: 0x06001BEC RID: 7148 RVA: 0x000771B3 File Offset: 0x000753B3
		public static float CalculateCylinderVolume(float radius, float height)
		{
			return 3.1415927f * radius * radius * height;
		}

		// Token: 0x06001BED RID: 7149 RVA: 0x000771C0 File Offset: 0x000753C0
		public static float CalculateColliderVolume(Collider collider)
		{
			Vector3 lossyScale = collider.transform.lossyScale;
			float num = lossyScale.x * lossyScale.y * lossyScale.z;
			float num2 = 0f;
			if (collider is BoxCollider)
			{
				Vector3 size = ((BoxCollider)collider).size;
				num2 = size.x * size.y * size.z;
			}
			else if (collider is SphereCollider)
			{
				num2 = Util.CalculateSphereVolume(((SphereCollider)collider).radius);
			}
			else if (collider is CapsuleCollider)
			{
				CapsuleCollider capsuleCollider = (CapsuleCollider)collider;
				float radius = capsuleCollider.radius;
				float num3 = Util.CalculateSphereVolume(radius);
				float num4 = Mathf.Max(capsuleCollider.height - num3, 0f);
				float num5 = 3.1415927f * radius * radius * num4;
				num2 = num3 + num5;
			}
			else if (collider is CharacterController)
			{
				CharacterController characterController = (CharacterController)collider;
				float radius2 = characterController.radius;
				float num6 = Util.CalculateSphereVolume(radius2);
				float num7 = Mathf.Max(characterController.height - num6, 0f);
				float num8 = 3.1415927f * radius2 * radius2 * num7;
				num2 = num6 + num8;
			}
			return num2 * num;
		}

		// Token: 0x06001BEE RID: 7150 RVA: 0x000772D8 File Offset: 0x000754D8
		public static Vector3 RandomColliderVolumePoint(Collider collider)
		{
			Transform transform = collider.transform;
			Vector3 vector = Vector3.zero;
			if (collider is BoxCollider)
			{
				BoxCollider boxCollider = (BoxCollider)collider;
				Vector3 size = boxCollider.size;
				Vector3 center = boxCollider.center;
				vector = new Vector3(center.x + UnityEngine.Random.Range(size.x * -0.5f, size.x * 0.5f), center.y + UnityEngine.Random.Range(size.y * -0.5f, size.y * 0.5f), center.z + UnityEngine.Random.Range(size.z * -0.5f, size.z * 0.5f));
			}
			else if (collider is SphereCollider)
			{
				SphereCollider sphereCollider = (SphereCollider)collider;
				vector = sphereCollider.center + UnityEngine.Random.insideUnitSphere * sphereCollider.radius;
			}
			else if (collider is CapsuleCollider)
			{
				CapsuleCollider capsuleCollider = (CapsuleCollider)collider;
				float radius = capsuleCollider.radius;
				float num = Mathf.Max(capsuleCollider.height - radius, 0f);
				float num2 = Util.CalculateSphereVolume(radius);
				float num3 = Util.CalculateCylinderVolume(radius, num);
				float max = num2 + num3;
				if (UnityEngine.Random.Range(0f, max) <= num2)
				{
					vector = UnityEngine.Random.insideUnitSphere * radius;
					float num4 = ((float)UnityEngine.Random.Range(0, 2) * 2f - 1f) * num * 0.5f;
					switch (capsuleCollider.direction)
					{
					case 0:
						vector.x += num4;
						break;
					case 1:
						vector.y += num4;
						break;
					case 2:
						vector.z += num4;
						break;
					}
				}
				else
				{
					Vector2 vector2 = UnityEngine.Random.insideUnitCircle * radius;
					float num5 = UnityEngine.Random.Range(num * -0.5f, num * 0.5f);
					switch (capsuleCollider.direction)
					{
					case 0:
						vector = new Vector3(num5, vector2.x, vector2.y);
						break;
					case 1:
						vector = new Vector3(vector2.x, num5, vector2.y);
						break;
					case 2:
						vector = new Vector3(vector2.x, vector2.y, num5);
						break;
					}
				}
				vector += capsuleCollider.center;
			}
			else if (collider is CharacterController)
			{
				CharacterController characterController = (CharacterController)collider;
				float radius2 = characterController.radius;
				float num6 = Mathf.Max(characterController.height - radius2, 0f);
				float num7 = Util.CalculateSphereVolume(radius2);
				float num8 = Util.CalculateCylinderVolume(radius2, num6);
				float max2 = num7 + num8;
				if (UnityEngine.Random.Range(0f, max2) <= num7)
				{
					vector = UnityEngine.Random.insideUnitSphere * radius2;
					float num9 = ((float)UnityEngine.Random.Range(0, 2) * 2f - 1f) * num6 * 0.5f;
					vector.y += num9;
				}
				else
				{
					Vector2 vector3 = UnityEngine.Random.insideUnitCircle * radius2;
					float y = UnityEngine.Random.Range(num6 * -0.5f, num6 * 0.5f);
					vector = new Vector3(vector3.x, y, vector3.y);
				}
				vector += characterController.center;
			}
			return transform.TransformPoint(vector);
		}

		// Token: 0x06001BEF RID: 7151 RVA: 0x00077628 File Offset: 0x00075828
		public static CharacterBody GetFriendlyEasyTarget(CharacterBody casterBody, Ray aimRay, float maxDistance, float maxDeviation = 20f)
		{
			TeamIndex teamIndex = TeamIndex.Neutral;
			TeamComponent component = casterBody.GetComponent<TeamComponent>();
			if (component)
			{
				teamIndex = component.teamIndex;
			}
			ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(teamIndex);
			Vector3 origin = aimRay.origin;
			Vector3 direction = aimRay.direction;
			List<Util.EasyTargetCandidate> candidatesList = new List<Util.EasyTargetCandidate>(teamMembers.Count);
			List<int> list = new List<int>(teamMembers.Count);
			float num = Mathf.Cos(maxDeviation * 0.017453292f);
			for (int i = 0; i < teamMembers.Count; i++)
			{
				Transform transform = teamMembers[i].transform;
				Vector3 a2 = transform.position - origin;
				float magnitude = a2.magnitude;
				float num2 = Vector3.Dot(a2 * (1f / magnitude), direction);
				CharacterBody component2 = transform.GetComponent<CharacterBody>();
				if (num2 >= num && component2 != casterBody)
				{
					float num3 = 1f / magnitude;
					float score = num2 + num3;
					candidatesList.Add(new Util.EasyTargetCandidate
					{
						transform = transform,
						score = score,
						distance = magnitude
					});
					list.Add(list.Count);
				}
			}
			list.Sort(delegate(int a, int b)
			{
				float score2 = candidatesList[a].score;
				float score3 = candidatesList[b].score;
				if (score2 == score3)
				{
					return 0;
				}
				if (score2 <= score3)
				{
					return 1;
				}
				return -1;
			});
			for (int j = 0; j < list.Count; j++)
			{
				int index = list[j];
				CharacterBody component3 = candidatesList[index].transform.GetComponent<CharacterBody>();
				if (component3 && component3 != casterBody)
				{
					return component3;
				}
			}
			return null;
		}

		// Token: 0x06001BF0 RID: 7152 RVA: 0x000777C0 File Offset: 0x000759C0
		public static CharacterBody GetEnemyEasyTarget(CharacterBody casterBody, Ray aimRay, float maxDistance, float maxDeviation = 20f)
		{
			TeamIndex teamIndex = TeamIndex.Neutral;
			TeamComponent component = casterBody.GetComponent<TeamComponent>();
			if (component)
			{
				teamIndex = component.teamIndex;
			}
			List<TeamComponent> list = new List<TeamComponent>();
			for (TeamIndex teamIndex2 = TeamIndex.Neutral; teamIndex2 < TeamIndex.Count; teamIndex2 += 1)
			{
				if (teamIndex2 != teamIndex)
				{
					list.AddRange(TeamComponent.GetTeamMembers(teamIndex2));
				}
			}
			Vector3 origin = aimRay.origin;
			Vector3 direction = aimRay.direction;
			List<Util.EasyTargetCandidate> candidatesList = new List<Util.EasyTargetCandidate>(list.Count);
			List<int> list2 = new List<int>(list.Count);
			float num = Mathf.Cos(maxDeviation * 0.017453292f);
			for (int i = 0; i < list.Count; i++)
			{
				Transform transform = list[i].transform;
				Vector3 a2 = transform.position - origin;
				float magnitude = a2.magnitude;
				float num2 = Vector3.Dot(a2 * (1f / magnitude), direction);
				CharacterBody component2 = transform.GetComponent<CharacterBody>();
				if (num2 >= num && component2 != casterBody && magnitude < maxDistance)
				{
					float num3 = 1f / magnitude;
					float score = num2 + num3;
					candidatesList.Add(new Util.EasyTargetCandidate
					{
						transform = transform,
						score = score,
						distance = magnitude
					});
					list2.Add(list2.Count);
				}
			}
			list2.Sort(delegate(int a, int b)
			{
				float score2 = candidatesList[a].score;
				float score3 = candidatesList[b].score;
				if (score2 == score3)
				{
					return 0;
				}
				if (score2 <= score3)
				{
					return 1;
				}
				return -1;
			});
			for (int j = 0; j < list2.Count; j++)
			{
				int index = list2[j];
				CharacterBody component3 = candidatesList[index].transform.GetComponent<CharacterBody>();
				if (component3 && component3 != casterBody)
				{
					return component3;
				}
			}
			return null;
		}

		// Token: 0x06001BF1 RID: 7153 RVA: 0x00077980 File Offset: 0x00075B80
		public static float GetBodyPrefabFootOffset(GameObject prefab)
		{
			CapsuleCollider component = prefab.GetComponent<CapsuleCollider>();
			if (component)
			{
				return component.height * 0.5f - component.center.y;
			}
			return 0f;
		}

		// Token: 0x06001BF2 RID: 7154 RVA: 0x000779BC File Offset: 0x00075BBC
		public static void ShuffleList<T>(List<T> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				int index = UnityEngine.Random.Range(0, list.Count);
				T value = list[i];
				list[i] = list[index];
				list[index] = value;
			}
		}

		// Token: 0x06001BF3 RID: 7155 RVA: 0x00077A08 File Offset: 0x00075C08
		public static void ShuffleList<T>(List<T> list, Xoroshiro128Plus rng)
		{
			for (int i = 0; i < list.Count; i++)
			{
				int index = rng.RangeInt(0, list.Count);
				T value = list[i];
				list[i] = list[index];
				list[index] = value;
			}
		}

		// Token: 0x06001BF4 RID: 7156 RVA: 0x00077A54 File Offset: 0x00075C54
		public static void ShuffleArray<T>(T[] array)
		{
			for (int i = 0; i < array.Length; i++)
			{
				int num = UnityEngine.Random.Range(0, array.Length);
				T t = array[i];
				array[i] = array[num];
				array[num] = t;
			}
		}

		// Token: 0x06001BF5 RID: 7157 RVA: 0x00077A98 File Offset: 0x00075C98
		public static void ShuffleArray<T>(T[] array, Xoroshiro128Plus rng)
		{
			for (int i = 0; i < array.Length; i++)
			{
				int num = rng.RangeInt(0, array.Length);
				T t = array[i];
				array[i] = array[num];
				array[num] = t;
			}
		}

		// Token: 0x06001BF6 RID: 7158 RVA: 0x00077ADC File Offset: 0x00075CDC
		public static Transform FindNearest(Vector3 position, List<Transform> transformsList, float range = float.PositiveInfinity)
		{
			Transform result = null;
			float num = range * range;
			for (int i = 0; i < transformsList.Count; i++)
			{
				float sqrMagnitude = (transformsList[i].position - position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					result = transformsList[i];
				}
			}
			return result;
		}

		// Token: 0x06001BF7 RID: 7159 RVA: 0x00077B2C File Offset: 0x00075D2C
		public static Vector3 ApplySpread(Vector3 aimDirection, float minSpread, float maxSpread, float spreadYawScale, float spreadPitchScale, float bonusYaw = 0f, float bonusPitch = 0f)
		{
			Vector3 up = Vector3.up;
			Vector3 axis = Vector3.Cross(up, aimDirection);
			float x = UnityEngine.Random.Range(minSpread, maxSpread);
			float z = UnityEngine.Random.Range(0f, 360f);
			Vector3 vector = Quaternion.Euler(0f, 0f, z) * (Quaternion.Euler(x, 0f, 0f) * Vector3.forward);
			float y = vector.y;
			vector.y = 0f;
			float angle = (Mathf.Atan2(vector.z, vector.x) * 57.29578f - 90f + bonusYaw) * spreadYawScale;
			float angle2 = (Mathf.Atan2(y, vector.magnitude) * 57.29578f + bonusPitch) * spreadPitchScale;
			return Quaternion.AngleAxis(angle, up) * (Quaternion.AngleAxis(angle2, axis) * aimDirection);
		}

		// Token: 0x06001BF8 RID: 7160 RVA: 0x00077C00 File Offset: 0x00075E00
		public static string GenerateColoredString(string str, Color32 color)
		{
			return string.Format(CultureInfo.InvariantCulture, "<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", new object[]
			{
				color.r,
				color.g,
				color.b,
				str
			});
		}

		// Token: 0x06001BF9 RID: 7161 RVA: 0x00077C50 File Offset: 0x00075E50
		public static bool GuessRenderBounds(GameObject gameObject, out Bounds bounds)
		{
			Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
			if (componentsInChildren.Length != 0)
			{
				bounds = componentsInChildren[0].bounds;
				for (int i = 1; i < componentsInChildren.Length; i++)
				{
					bounds.Encapsulate(componentsInChildren[i].bounds);
				}
				return true;
			}
			bounds = new Bounds(gameObject.transform.position, Vector3.zero);
			return false;
		}

		// Token: 0x06001BFA RID: 7162 RVA: 0x00077CB0 File Offset: 0x00075EB0
		public static bool GuessRenderBoundsMeshOnly(GameObject gameObject, out Bounds bounds)
		{
			Renderer[] array = (from renderer in gameObject.GetComponentsInChildren<Renderer>()
			where renderer is MeshRenderer || renderer is SkinnedMeshRenderer
			select renderer).ToArray<Renderer>();
			if (array.Length != 0)
			{
				bounds = array[0].bounds;
				for (int i = 1; i < array.Length; i++)
				{
					bounds.Encapsulate(array[i].bounds);
				}
				return true;
			}
			bounds = new Bounds(gameObject.transform.position, Vector3.zero);
			return false;
		}

		// Token: 0x06001BFB RID: 7163 RVA: 0x00077D39 File Offset: 0x00075F39
		public static GameObject FindNetworkObject(NetworkInstanceId networkInstanceId)
		{
			if (NetworkServer.active)
			{
				return NetworkServer.FindLocalObject(networkInstanceId);
			}
			return ClientScene.FindLocalObject(networkInstanceId);
		}

		// Token: 0x06001BFC RID: 7164 RVA: 0x00077D50 File Offset: 0x00075F50
		public static string GetGameObjectHierarchyName(GameObject gameObject)
		{
			int num = 0;
			Transform transform = gameObject.transform;
			while (transform)
			{
				num++;
				transform = transform.parent;
			}
			string[] array = new string[num];
			Transform transform2 = gameObject.transform;
			while (transform2)
			{
				array[--num] = transform2.gameObject.name;
				transform2 = transform2.parent;
			}
			return string.Join("/", array);
		}

		// Token: 0x06001BFD RID: 7165 RVA: 0x00077DB8 File Offset: 0x00075FB8
		public static string GetBestBodyName(GameObject bodyObject)
		{
			CharacterBody characterBody = null;
			string text = "???";
			if (bodyObject)
			{
				characterBody = bodyObject.GetComponent<CharacterBody>();
			}
			if (characterBody)
			{
				text = characterBody.GetUserName();
			}
			else
			{
				IDisplayNameProvider component = bodyObject.GetComponent<IDisplayNameProvider>();
				if (component != null)
				{
					text = component.GetDisplayName();
				}
			}
			string text2 = text;
			if (characterBody && characterBody.isElite)
			{
				foreach (BuffIndex buffIndex in BuffCatalog.eliteBuffIndices)
				{
					if (characterBody.HasBuff(buffIndex))
					{
						text2 = Language.GetStringFormatted(EliteCatalog.GetEliteDef(BuffCatalog.GetBuffDef(buffIndex).eliteIndex).modifierToken, new object[]
						{
							text2
						});
					}
				}
			}
			return text2;
		}

		// Token: 0x06001BFE RID: 7166 RVA: 0x00077E64 File Offset: 0x00076064
		public static string GetBestBodyNameColored(GameObject bodyObject)
		{
			if (bodyObject)
			{
				CharacterBody component = bodyObject.GetComponent<CharacterBody>();
				if (component)
				{
					CharacterMaster master = component.master;
					if (master)
					{
						PlayerCharacterMasterController component2 = master.GetComponent<PlayerCharacterMasterController>();
						if (component2)
						{
							GameObject networkUserObject = component2.networkUserObject;
							if (networkUserObject)
							{
								NetworkUser component3 = networkUserObject.GetComponent<NetworkUser>();
								if (component3)
								{
									return Util.GenerateColoredString(component3.userName, component3.userColor);
								}
							}
						}
					}
				}
				IDisplayNameProvider component4 = bodyObject.GetComponent<IDisplayNameProvider>();
				if (component4 != null)
				{
					return component4.GetDisplayName();
				}
			}
			return "???";
		}

		// Token: 0x06001BFF RID: 7167 RVA: 0x00077EF4 File Offset: 0x000760F4
		public static string GetBestMasterName(CharacterMaster characterMaster)
		{
			if (characterMaster)
			{
				PlayerCharacterMasterController playerCharacterMasterController = characterMaster.playerCharacterMasterController;
				string text;
				if (playerCharacterMasterController == null)
				{
					text = null;
				}
				else
				{
					NetworkUser networkUser = playerCharacterMasterController.networkUser;
					text = ((networkUser != null) ? networkUser.userName : null);
				}
				string text2 = text;
				if (text2 == null)
				{
					GameObject bodyPrefab = characterMaster.bodyPrefab;
					text2 = Language.GetString((bodyPrefab != null) ? bodyPrefab.GetComponent<CharacterBody>().baseNameToken : null);
				}
				return text2;
			}
			return "Null Master";
		}

		// Token: 0x06001C00 RID: 7168 RVA: 0x00077F4F File Offset: 0x0007614F
		public static NetworkUser LookUpBodyNetworkUser(GameObject bodyObject)
		{
			if (bodyObject)
			{
				return Util.LookUpBodyNetworkUser(bodyObject.GetComponent<CharacterBody>());
			}
			return null;
		}

		// Token: 0x06001C01 RID: 7169 RVA: 0x00077F68 File Offset: 0x00076168
		public static NetworkUser LookUpBodyNetworkUser(CharacterBody characterBody)
		{
			if (characterBody)
			{
				CharacterMaster master = characterBody.master;
				if (master)
				{
					PlayerCharacterMasterController component = master.GetComponent<PlayerCharacterMasterController>();
					if (component)
					{
						GameObject networkUserObject = component.networkUserObject;
						if (networkUserObject)
						{
							NetworkUser component2 = networkUserObject.GetComponent<NetworkUser>();
							if (component2)
							{
								return component2;
							}
						}
					}
				}
			}
			return null;
		}

		// Token: 0x06001C02 RID: 7170 RVA: 0x00077FBC File Offset: 0x000761BC
		private static bool HandleCharacterPhysicsCastResults(GameObject bodyObject, RaycastHit[] hits, out RaycastHit hitInfo)
		{
			int num = -1;
			float num2 = float.PositiveInfinity;
			for (int i = 0; i < hits.Length; i++)
			{
				float distance = hits[i].distance;
				if (distance < num2)
				{
					HurtBox component = hits[i].collider.GetComponent<HurtBox>();
					if (component)
					{
						HealthComponent healthComponent = component.healthComponent;
						if (healthComponent && healthComponent.gameObject == bodyObject)
						{
							goto IL_5E;
						}
					}
					num = i;
					num2 = distance;
				}
				IL_5E:;
			}
			if (num == -1)
			{
				hitInfo = default(RaycastHit);
				return false;
			}
			hitInfo = hits[num];
			return true;
		}

		// Token: 0x06001C03 RID: 7171 RVA: 0x0007804C File Offset: 0x0007624C
		public static bool CharacterRaycast(GameObject bodyObject, Ray ray, out RaycastHit hitInfo, float maxDistance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction)
		{
			RaycastHit[] hits = Physics.RaycastAll(ray, maxDistance, layerMask, queryTriggerInteraction);
			return Util.HandleCharacterPhysicsCastResults(bodyObject, hits, out hitInfo);
		}

		// Token: 0x06001C04 RID: 7172 RVA: 0x00078074 File Offset: 0x00076274
		public static bool CharacterSpherecast(GameObject bodyObject, Ray ray, float radius, out RaycastHit hitInfo, float maxDistance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction)
		{
			RaycastHit[] hits = Physics.SphereCastAll(ray, radius, maxDistance, layerMask, queryTriggerInteraction);
			return Util.HandleCharacterPhysicsCastResults(bodyObject, hits, out hitInfo);
		}

		// Token: 0x06001C05 RID: 7173 RVA: 0x0007809C File Offset: 0x0007629C
		public static bool ConnectionIsLocal([NotNull] NetworkConnection conn)
		{
			return !(conn is SteamNetworkConnection) && conn.GetType() != typeof(NetworkConnection);
		}

		// Token: 0x06001C06 RID: 7174 RVA: 0x000780C0 File Offset: 0x000762C0
		public static string EscapeRichTextForTextMeshPro(string rtString)
		{
			string str = rtString.Replace("<", "</noparse><noparse><</noparse><noparse>");
			return "<noparse>" + str + "</noparse>";
		}

		// Token: 0x06001C07 RID: 7175 RVA: 0x000780F0 File Offset: 0x000762F0
		public static string EscapeQuotes(string str)
		{
			str = Util.backlashSearch.Replace(str, Util.strBackslashBackslash);
			str = Util.quoteSearch.Replace(str, Util.strBackslashQuote);
			return str;
		}

		// Token: 0x06001C08 RID: 7176 RVA: 0x00078117 File Offset: 0x00076317
		public static string RGBToHex(Color32 rgb)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0:X2}{1:X2}{2:X2}", rgb.r, rgb.g, rgb.b);
		}

		// Token: 0x06001C09 RID: 7177 RVA: 0x00078149 File Offset: 0x00076349
		public static Vector2 Vector3XZToVector2XY(Vector3 vector3)
		{
			return new Vector2(vector3.x, vector3.z);
		}

		// Token: 0x06001C0A RID: 7178 RVA: 0x00078149 File Offset: 0x00076349
		public static Vector2 Vector3XZToVector2XY(ref Vector3 vector3)
		{
			return new Vector2(vector3.x, vector3.z);
		}

		// Token: 0x06001C0B RID: 7179 RVA: 0x0007815C File Offset: 0x0007635C
		public static void Vector3XZToVector2XY(Vector3 vector3, out Vector2 vector2)
		{
			vector2.x = vector3.x;
			vector2.y = vector3.z;
		}

		// Token: 0x06001C0C RID: 7180 RVA: 0x0007815C File Offset: 0x0007635C
		public static void Vector3XZToVector2XY(ref Vector3 vector3, out Vector2 vector2)
		{
			vector2.x = vector3.x;
			vector2.y = vector3.z;
		}

		// Token: 0x06001C0D RID: 7181 RVA: 0x00078178 File Offset: 0x00076378
		public static Vector2 RotateVector2(Vector2 vector2, float degrees)
		{
			float num = Mathf.Sin(degrees * 0.017453292f);
			float num2 = Mathf.Cos(degrees * 0.017453292f);
			return new Vector2(num2 * vector2.x - num * vector2.y, num * vector2.x + num2 * vector2.y);
		}

		// Token: 0x06001C0E RID: 7182 RVA: 0x000781C8 File Offset: 0x000763C8
		public static Quaternion SmoothDampQuaternion(Quaternion current, Quaternion target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
		{
			float num = Quaternion.Angle(current, target);
			num = Mathf.SmoothDamp(0f, num, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
			return Quaternion.RotateTowards(current, target, num);
		}

		// Token: 0x06001C0F RID: 7183 RVA: 0x000781F8 File Offset: 0x000763F8
		public static Quaternion SmoothDampQuaternion(Quaternion current, Quaternion target, ref float currentVelocity, float smoothTime)
		{
			float num = Quaternion.Angle(current, target);
			num = Mathf.SmoothDamp(0f, num, ref currentVelocity, smoothTime);
			return Quaternion.RotateTowards(current, target, num);
		}

		// Token: 0x06001C10 RID: 7184 RVA: 0x00078223 File Offset: 0x00076423
		public static HurtBox FindBodyMainHurtBox(CharacterBody characterBody)
		{
			return characterBody.mainHurtBox;
		}

		// Token: 0x06001C11 RID: 7185 RVA: 0x0007822C File Offset: 0x0007642C
		public static HurtBox FindBodyMainHurtBox(GameObject bodyObject)
		{
			CharacterBody component = bodyObject.GetComponent<CharacterBody>();
			if (component)
			{
				return Util.FindBodyMainHurtBox(component);
			}
			return null;
		}

		// Token: 0x06001C12 RID: 7186 RVA: 0x00078250 File Offset: 0x00076450
		public static Vector3 GetCorePosition(CharacterBody characterBody)
		{
			return characterBody.corePosition;
		}

		// Token: 0x06001C13 RID: 7187 RVA: 0x00078258 File Offset: 0x00076458
		public static Vector3 GetCorePosition(GameObject bodyObject)
		{
			CharacterBody component = bodyObject.GetComponent<CharacterBody>();
			if (component)
			{
				return Util.GetCorePosition(component);
			}
			return bodyObject.transform.position;
		}

		// Token: 0x06001C14 RID: 7188 RVA: 0x00078288 File Offset: 0x00076488
		public static Transform GetCoreTransform(GameObject bodyObject)
		{
			CharacterBody component = bodyObject.GetComponent<CharacterBody>();
			if (component)
			{
				return component.coreTransform;
			}
			return bodyObject.transform;
		}

		// Token: 0x06001C15 RID: 7189 RVA: 0x000782B1 File Offset: 0x000764B1
		public static float SphereRadiusToVolume(float radius)
		{
			return 4.1887903f * (radius * radius * radius);
		}

		// Token: 0x06001C16 RID: 7190 RVA: 0x000782BE File Offset: 0x000764BE
		public static float SphereVolumeToRadius(float volume)
		{
			return Mathf.Pow(3f * volume / 12.566371f, 0.33333334f);
		}

		// Token: 0x06001C17 RID: 7191 RVA: 0x000782D8 File Offset: 0x000764D8
		public static void CopyList<T>(List<T> src, List<T> dest)
		{
			dest.Clear();
			foreach (T item in src)
			{
				dest.Add(item);
			}
		}

		// Token: 0x06001C18 RID: 7192 RVA: 0x0007832C File Offset: 0x0007652C
		public static float ScanCharacterAnimationClipForMomentOfRootMotionStop(GameObject characterPrefab, string clipName, string rootBoneNameInChildLocator)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(characterPrefab);
			Transform modelTransform = gameObject.GetComponent<ModelLocator>().modelTransform;
			Transform transform = modelTransform.GetComponent<ChildLocator>().FindChild(rootBoneNameInChildLocator);
			AnimationClip animationClip = modelTransform.GetComponent<Animator>().runtimeAnimatorController.animationClips.FirstOrDefault((AnimationClip c) => c.name == clipName);
			float result = 1f;
			animationClip.SampleAnimation(gameObject, 0f);
			Vector3 b = transform.position;
			for (float num = 0.1f; num < 1f; num += 0.1f)
			{
				animationClip.SampleAnimation(gameObject, num);
				Vector3 position = transform.position;
				if ((position - b).magnitude == 0f)
				{
					result = num;
					break;
				}
				b = position;
			}
			UnityEngine.Object.Destroy(gameObject);
			return result;
		}

		// Token: 0x06001C19 RID: 7193 RVA: 0x000783FC File Offset: 0x000765FC
		public static void DebugCross(Vector3 position, float radius, Color color, float duration)
		{
			Debug.DrawLine(position - Vector3.right * radius, position + Vector3.right * radius, color, duration);
			Debug.DrawLine(position - Vector3.up * radius, position + Vector3.up * radius, color, duration);
			Debug.DrawLine(position - Vector3.forward * radius, position + Vector3.forward * radius, color, duration);
		}

		// Token: 0x06001C1A RID: 7194 RVA: 0x00078484 File Offset: 0x00076684
		public static bool PositionIsValid(Vector3 value)
		{
			float f = value.x + value.y + value.z;
			return !float.IsInfinity(f) && !float.IsNaN(f);
		}

		// Token: 0x06001C1B RID: 7195 RVA: 0x000784BC File Offset: 0x000766BC
		public static void Swap<T>(ref T a, ref T b)
		{
			T t = a;
			a = b;
			b = t;
		}

		// Token: 0x06001C1C RID: 7196 RVA: 0x000784E4 File Offset: 0x000766E4
		public static DateTime UnixTimeStampToDateTimeUtc(uint unixTimeStamp)
		{
			return Util.dateZero.AddSeconds(unixTimeStamp).ToUniversalTime();
		}

		// Token: 0x06001C1D RID: 7197 RVA: 0x0007850B File Offset: 0x0007670B
		public static bool IsValid(UnityEngine.Object o)
		{
			return o;
		}

		// Token: 0x06001C1E RID: 7198 RVA: 0x00078514 File Offset: 0x00076714
		public static string BuildPrefabTransformPath(Transform root, Transform transform, bool includeClone)
		{
			string name = transform.gameObject.name;
			if (includeClone)
			{
				Util.sharedStringStack.Push(Util.cloneString);
			}
			Util.sharedStringStack.Push(name);
			Transform parent = transform.parent;
			while (parent != root)
			{
				string name2 = parent.name;
				Util.sharedStringStack.Push("/");
				Util.sharedStringStack.Push(name2);
				parent = parent.parent;
			}
			while (Util.sharedStringStack.Count > 0)
			{
				Util.sharedStringBuilder.Append(Util.sharedStringStack.Pop());
			}
			return Util.sharedStringBuilder.Take();
		}

		// Token: 0x06001C1F RID: 7199 RVA: 0x000785B4 File Offset: 0x000767B4
		public static int GetItemCountForTeam(TeamIndex teamIndex, ItemIndex itemIndex, bool requiresAlive, bool requiresConnected = true)
		{
			int num = 0;
			ReadOnlyCollection<CharacterMaster> readOnlyInstancesList = CharacterMaster.readOnlyInstancesList;
			int i = 0;
			int count = readOnlyInstancesList.Count;
			while (i < count)
			{
				CharacterMaster characterMaster = readOnlyInstancesList[i];
				if (characterMaster.teamIndex == teamIndex && (!requiresAlive || characterMaster.alive) && (!requiresConnected || !characterMaster.playerCharacterMasterController || characterMaster.playerCharacterMasterController.isConnected))
				{
					num += characterMaster.inventory.GetItemCount(itemIndex);
				}
				i++;
			}
			return num;
		}

		// Token: 0x06001C20 RID: 7200 RVA: 0x0007862B File Offset: 0x0007682B
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void NullifyIfInvalid<T>(ref T objRef) where T : UnityEngine.Object
		{
			if (objRef == null)
			{
				return;
			}
			if (!objRef)
			{
				objRef = default(T);
			}
		}

		// Token: 0x06001C21 RID: 7201 RVA: 0x00078654 File Offset: 0x00076854
		public static bool IsPrefab(GameObject gameObject)
		{
			return gameObject && !gameObject.scene.IsValid();
		}

		// Token: 0x06001C22 RID: 7202 RVA: 0x0007867C File Offset: 0x0007687C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint IntToUintPlusOne(int value)
		{
			return (uint)(value + 1);
		}

		// Token: 0x06001C23 RID: 7203 RVA: 0x00078681 File Offset: 0x00076881
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int UintToIntMinusOne(uint value)
		{
			return (int)(value - 1U);
		}

		// Token: 0x06001C24 RID: 7204 RVA: 0x00078686 File Offset: 0x00076886
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong LongToUlongPlusOne(long value)
		{
			return (ulong)(value + 1L);
		}

		// Token: 0x06001C25 RID: 7205 RVA: 0x0007868C File Offset: 0x0007688C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long UlongToLongMinusOne(ulong value)
		{
			return (long)(value - 1UL);
		}

		// Token: 0x04001907 RID: 6407
		private static readonly string strBackslash = "\\";

		// Token: 0x04001908 RID: 6408
		private static readonly string strBackslashBackslash = Util.strBackslash + Util.strBackslash;

		// Token: 0x04001909 RID: 6409
		private static readonly string strQuote = "\"";

		// Token: 0x0400190A RID: 6410
		private static readonly string strBackslashQuote = Util.strBackslash + Util.strQuote;

		// Token: 0x0400190B RID: 6411
		private static readonly Regex backlashSearch = new Regex(Util.strBackslashBackslash);

		// Token: 0x0400190C RID: 6412
		private static readonly Regex quoteSearch = new Regex(Util.strQuote);

		// Token: 0x0400190D RID: 6413
		public static readonly DateTime dateZero = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		// Token: 0x0400190E RID: 6414
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();

		// Token: 0x0400190F RID: 6415
		private static readonly Stack<string> sharedStringStack = new Stack<string>();

		// Token: 0x04001910 RID: 6416
		private static readonly string cloneString = "(Clone)";

		// Token: 0x0200047C RID: 1148
		private struct EasyTargetCandidate
		{
			// Token: 0x04001911 RID: 6417
			public Transform transform;

			// Token: 0x04001912 RID: 6418
			public float score;

			// Token: 0x04001913 RID: 6419
			public float distance;
		}
	}
}
