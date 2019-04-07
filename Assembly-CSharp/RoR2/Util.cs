using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Rewired;
using RoR2.CharacterAI;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020004E0 RID: 1248
	public static class Util
	{
		// Token: 0x06001C1F RID: 7199 RVA: 0x000832A8 File Offset: 0x000814A8
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

		// Token: 0x06001C20 RID: 7200 RVA: 0x00083300 File Offset: 0x00081500
		public static CharacterBody TryToCreateGhost(CharacterBody targetBody, CharacterBody ownerBody, int duration)
		{
			if (!targetBody || !NetworkServer.active)
			{
				return null;
			}
			if (TeamComponent.GetTeamMembers(ownerBody.teamComponent.teamIndex).Count >= 40)
			{
				return null;
			}
			int num = BodyCatalog.FindBodyIndex(targetBody.gameObject);
			if (num < 0)
			{
				return null;
			}
			GameObject bodyPrefab = BodyCatalog.GetBodyPrefab(num);
			if (!bodyPrefab)
			{
				return null;
			}
			CharacterMaster characterMaster = MasterCatalog.allAiMasters.FirstOrDefault((CharacterMaster master) => master.bodyPrefab == bodyPrefab);
			if (!characterMaster)
			{
				return null;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(characterMaster.gameObject);
			CharacterMaster component = gameObject.GetComponent<CharacterMaster>();
			component.teamIndex = ownerBody.teamComponent.teamIndex;
			component.GetComponent<BaseAI>().leader.gameObject = ownerBody.gameObject;
			Inventory inventory = targetBody.inventory;
			if (inventory)
			{
				component.inventory.CopyItemsFrom(inventory);
				component.inventory.CopyEquipmentFrom(inventory);
			}
			component.inventory.GiveItem(ItemIndex.Ghost, 1);
			component.inventory.GiveItem(ItemIndex.HealthDecay, duration);
			component.inventory.GiveItem(ItemIndex.BoostDamage, 30);
			NetworkServer.Spawn(gameObject);
			CharacterBody characterBody = component.Respawn(targetBody.footPosition, targetBody.transform.rotation);
			if (characterBody)
			{
				foreach (EntityStateMachine entityStateMachine in characterBody.GetComponents<EntityStateMachine>())
				{
					entityStateMachine.initialStateType = entityStateMachine.mainStateType;
				}
			}
			return characterBody;
		}

		// Token: 0x06001C21 RID: 7201 RVA: 0x0008347C File Offset: 0x0008167C
		public static float OnHitProcDamage(float damageThatProccedIt, float baseDamage, float damageCoefficient)
		{
			float a = damageThatProccedIt + (damageCoefficient - 1f) * baseDamage;
			float b = damageThatProccedIt * damageCoefficient;
			return Mathf.Max(1f, Mathf.Min(a, b));
		}

		// Token: 0x06001C22 RID: 7202 RVA: 0x000834AA File Offset: 0x000816AA
		public static float OnKillProcDamage(float baseDamage, float damageCoefficient)
		{
			return baseDamage * damageCoefficient;
		}

		// Token: 0x06001C23 RID: 7203 RVA: 0x000834B0 File Offset: 0x000816B0
		public static Quaternion QuaternionSafeLookRotation(Vector3 forward)
		{
			Quaternion result = Quaternion.identity;
			if (forward.sqrMagnitude > Mathf.Epsilon)
			{
				result = Quaternion.LookRotation(forward);
			}
			return result;
		}

		// Token: 0x06001C24 RID: 7204 RVA: 0x000834DC File Offset: 0x000816DC
		public static Quaternion QuaternionSafeLookRotation(Vector3 forward, Vector3 upwards)
		{
			Quaternion result = Quaternion.identity;
			if (forward.sqrMagnitude > Mathf.Epsilon)
			{
				result = Quaternion.LookRotation(forward, upwards);
			}
			return result;
		}

		// Token: 0x06001C25 RID: 7205 RVA: 0x00083508 File Offset: 0x00081708
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

		// Token: 0x06001C26 RID: 7206 RVA: 0x00083548 File Offset: 0x00081748
		public static uint PlaySound(string soundString, GameObject gameObject)
		{
			if (string.IsNullOrEmpty(soundString))
			{
				return 0u;
			}
			return AkSoundEngine.PostEvent(soundString, gameObject);
		}

		// Token: 0x06001C27 RID: 7207 RVA: 0x0008355C File Offset: 0x0008175C
		public static uint PlaySound(string soundString, GameObject gameObject, string RTPCstring, float RTPCvalue)
		{
			uint num = Util.PlaySound(soundString, gameObject);
			if (num != 0u)
			{
				AkSoundEngine.SetRTPCValueByPlayingID(RTPCstring, RTPCvalue, num);
			}
			return num;
		}

		// Token: 0x06001C28 RID: 7208 RVA: 0x00083580 File Offset: 0x00081780
		public static uint PlayScaledSound(string soundString, GameObject gameObject, float playbackRate)
		{
			uint num = Util.PlaySound(soundString, gameObject);
			if (num != 0u)
			{
				float num2 = Mathf.Log(playbackRate, 2f);
				float in_value = 1200f * num2 / 96f + 50f;
				AkSoundEngine.SetRTPCValueByPlayingID("attackSpeed", in_value, num);
			}
			return num;
		}

		// Token: 0x06001C29 RID: 7209 RVA: 0x000835C8 File Offset: 0x000817C8
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

		// Token: 0x06001C2A RID: 7210 RVA: 0x00083660 File Offset: 0x00081860
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

		// Token: 0x06001C2B RID: 7211 RVA: 0x000836BE File Offset: 0x000818BE
		public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
		{
			return Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * 57.29578f;
		}

		// Token: 0x06001C2C RID: 7212 RVA: 0x00036C4B File Offset: 0x00034E4B
		public static float Remap(float value, float inMin, float inMax, float outMin, float outMax)
		{
			return outMin + (value - inMin) / (inMax - inMin) * (outMax - outMin);
		}

		// Token: 0x06001C2D RID: 7213 RVA: 0x000836E0 File Offset: 0x000818E0
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

		// Token: 0x06001C2E RID: 7214 RVA: 0x00083718 File Offset: 0x00081918
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

		// Token: 0x06001C2F RID: 7215 RVA: 0x0008374C File Offset: 0x0008194C
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

		// Token: 0x06001C30 RID: 7216 RVA: 0x000837F7 File Offset: 0x000819F7
		public static bool CheckRoll(float percentChance, CharacterMaster master)
		{
			return Util.CheckRoll(percentChance, master ? master.luck : 0f, master);
		}

		// Token: 0x06001C31 RID: 7217 RVA: 0x00083818 File Offset: 0x00081A18
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

		// Token: 0x06001C32 RID: 7218 RVA: 0x000838AB File Offset: 0x00081AAB
		public static bool HasEffectiveAuthority(GameObject gameObject)
		{
			return gameObject && Util.HasEffectiveAuthority(gameObject.GetComponent<NetworkIdentity>());
		}

		// Token: 0x06001C33 RID: 7219 RVA: 0x000838C2 File Offset: 0x00081AC2
		public static bool HasEffectiveAuthority(NetworkIdentity networkIdentity)
		{
			return networkIdentity && (networkIdentity.hasAuthority || (NetworkServer.active && networkIdentity.clientAuthorityOwner == null));
		}

		// Token: 0x06001C34 RID: 7220 RVA: 0x000838EA File Offset: 0x00081AEA
		public static float CalculateSphereVolume(float radius)
		{
			return 4.1887903f * radius * radius * radius;
		}

		// Token: 0x06001C35 RID: 7221 RVA: 0x000838F7 File Offset: 0x00081AF7
		public static float CalculateCylinderVolume(float radius, float height)
		{
			return 3.1415927f * radius * radius * height;
		}

		// Token: 0x06001C36 RID: 7222 RVA: 0x00083904 File Offset: 0x00081B04
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

		// Token: 0x06001C37 RID: 7223 RVA: 0x00083A1C File Offset: 0x00081C1C
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

		// Token: 0x06001C38 RID: 7224 RVA: 0x00083D6C File Offset: 0x00081F6C
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

		// Token: 0x06001C39 RID: 7225 RVA: 0x00083F04 File Offset: 0x00082104
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

		// Token: 0x06001C3A RID: 7226 RVA: 0x000840C4 File Offset: 0x000822C4
		public static float GetBodyPrefabFootOffset(GameObject prefab)
		{
			CapsuleCollider component = prefab.GetComponent<CapsuleCollider>();
			if (component)
			{
				return component.height * 0.5f - component.center.y;
			}
			return 0f;
		}

		// Token: 0x06001C3B RID: 7227 RVA: 0x00084100 File Offset: 0x00082300
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

		// Token: 0x06001C3C RID: 7228 RVA: 0x0008414C File Offset: 0x0008234C
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

		// Token: 0x06001C3D RID: 7229 RVA: 0x00084198 File Offset: 0x00082398
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

		// Token: 0x06001C3E RID: 7230 RVA: 0x000841DC File Offset: 0x000823DC
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

		// Token: 0x06001C3F RID: 7231 RVA: 0x00084220 File Offset: 0x00082420
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

		// Token: 0x06001C40 RID: 7232 RVA: 0x00084270 File Offset: 0x00082470
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

		// Token: 0x06001C41 RID: 7233 RVA: 0x00084344 File Offset: 0x00082544
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

		// Token: 0x06001C42 RID: 7234 RVA: 0x00084394 File Offset: 0x00082594
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

		// Token: 0x06001C43 RID: 7235 RVA: 0x000843F4 File Offset: 0x000825F4
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

		// Token: 0x06001C44 RID: 7236 RVA: 0x0008447D File Offset: 0x0008267D
		public static GameObject FindNetworkObject(NetworkInstanceId networkInstanceId)
		{
			if (NetworkServer.active)
			{
				return NetworkServer.FindLocalObject(networkInstanceId);
			}
			return ClientScene.FindLocalObject(networkInstanceId);
		}

		// Token: 0x06001C45 RID: 7237 RVA: 0x00084494 File Offset: 0x00082694
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

		// Token: 0x06001C46 RID: 7238 RVA: 0x000844FC File Offset: 0x000826FC
		public static string GetBestBodyName(GameObject bodyObject)
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
									return component3.userName;
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

		// Token: 0x06001C47 RID: 7239 RVA: 0x00084580 File Offset: 0x00082780
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

		// Token: 0x06001C48 RID: 7240 RVA: 0x00084610 File Offset: 0x00082810
		public static string GetBestMasterName(CharacterMaster characterMaster)
		{
			if (characterMaster)
			{
				PlayerCharacterMasterController component = characterMaster.GetComponent<PlayerCharacterMasterController>();
				if (component)
				{
					GameObject networkUserObject = component.networkUserObject;
					if (networkUserObject)
					{
						NetworkUser component2 = networkUserObject.GetComponent<NetworkUser>();
						if (component2)
						{
							return component2.userName;
						}
					}
				}
				return characterMaster.name;
			}
			return "Null Master";
		}

		// Token: 0x06001C49 RID: 7241 RVA: 0x00084665 File Offset: 0x00082865
		public static NetworkUser LookUpBodyNetworkUser(GameObject bodyObject)
		{
			if (bodyObject)
			{
				return Util.LookUpBodyNetworkUser(bodyObject.GetComponent<CharacterBody>());
			}
			return null;
		}

		// Token: 0x06001C4A RID: 7242 RVA: 0x0008467C File Offset: 0x0008287C
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

		// Token: 0x06001C4B RID: 7243 RVA: 0x000846D0 File Offset: 0x000828D0
		public static bool CharacterRaycast(GameObject bodyObject, Ray ray, out RaycastHit hitInfo, float maxDistance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction)
		{
			RaycastHit[] array = Physics.RaycastAll(ray, maxDistance, layerMask, queryTriggerInteraction);
			int num = -1;
			float num2 = float.PositiveInfinity;
			for (int i = 0; i < array.Length; i++)
			{
				float distance = array[i].distance;
				if (distance < num2)
				{
					HurtBox component = array[i].collider.GetComponent<HurtBox>();
					if (component)
					{
						HealthComponent healthComponent = component.healthComponent;
						if (healthComponent && healthComponent.gameObject == bodyObject)
						{
							goto IL_72;
						}
					}
					num = i;
					num2 = distance;
				}
				IL_72:;
			}
			if (num == -1)
			{
				hitInfo = default(RaycastHit);
				return false;
			}
			hitInfo = array[num];
			return true;
		}

		// Token: 0x06001C4C RID: 7244 RVA: 0x00084774 File Offset: 0x00082974
		public static bool ConnectionIsLocal([NotNull] NetworkConnection conn)
		{
			return !(conn is SteamNetworkConnection) && conn.GetType() != typeof(NetworkConnection);
		}

		// Token: 0x06001C4D RID: 7245 RVA: 0x00084798 File Offset: 0x00082998
		public static string EscapeRichTextForTextMeshPro(string rtString)
		{
			string str = rtString.Replace("<", "</noparse><noparse><</noparse><noparse>");
			return "<noparse>" + str + "</noparse>";
		}

		// Token: 0x06001C4E RID: 7246 RVA: 0x000847C8 File Offset: 0x000829C8
		public static string EscapeQuotes(string str)
		{
			return new Regex("\"").Replace(str, "\\\"");
		}

		// Token: 0x06001C4F RID: 7247 RVA: 0x000847DF File Offset: 0x000829DF
		public static string RGBToHex(Color32 rgb)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0:X2}{1:X2}{2:X2}", rgb.r, rgb.g, rgb.b);
		}

		// Token: 0x06001C50 RID: 7248 RVA: 0x00084811 File Offset: 0x00082A11
		public static Vector2 Vector3XZToVector2XY(Vector3 vector3)
		{
			return new Vector2(vector3.x, vector3.z);
		}

		// Token: 0x06001C51 RID: 7249 RVA: 0x00084811 File Offset: 0x00082A11
		public static Vector2 Vector3XZToVector2XY(ref Vector3 vector3)
		{
			return new Vector2(vector3.x, vector3.z);
		}

		// Token: 0x06001C52 RID: 7250 RVA: 0x00084824 File Offset: 0x00082A24
		public static void Vector3XZToVector2XY(Vector3 vector3, out Vector2 vector2)
		{
			vector2.x = vector3.x;
			vector2.y = vector3.z;
		}

		// Token: 0x06001C53 RID: 7251 RVA: 0x00084824 File Offset: 0x00082A24
		public static void Vector3XZToVector2XY(ref Vector3 vector3, out Vector2 vector2)
		{
			vector2.x = vector3.x;
			vector2.y = vector3.z;
		}

		// Token: 0x06001C54 RID: 7252 RVA: 0x00084840 File Offset: 0x00082A40
		public static Vector2 RotateVector2(Vector2 vector2, float degrees)
		{
			float num = Mathf.Sin(degrees * 0.017453292f);
			float num2 = Mathf.Cos(degrees * 0.017453292f);
			return new Vector2(num2 * vector2.x - num * vector2.y, num * vector2.x + num2 * vector2.y);
		}

		// Token: 0x06001C55 RID: 7253 RVA: 0x00084890 File Offset: 0x00082A90
		public static Quaternion SmoothDampQuaternion(Quaternion current, Quaternion target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
		{
			float num = Quaternion.Angle(current, target);
			num = Mathf.SmoothDamp(0f, num, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
			return Quaternion.RotateTowards(current, target, num);
		}

		// Token: 0x06001C56 RID: 7254 RVA: 0x000848C0 File Offset: 0x00082AC0
		public static Quaternion SmoothDampQuaternion(Quaternion current, Quaternion target, ref float currentVelocity, float smoothTime)
		{
			float num = Quaternion.Angle(current, target);
			num = Mathf.SmoothDamp(0f, num, ref currentVelocity, smoothTime);
			return Quaternion.RotateTowards(current, target, num);
		}

		// Token: 0x06001C57 RID: 7255 RVA: 0x000848EB File Offset: 0x00082AEB
		public static HurtBox FindBodyMainHurtBox(CharacterBody characterBody)
		{
			return characterBody.mainHurtBox;
		}

		// Token: 0x06001C58 RID: 7256 RVA: 0x000848F4 File Offset: 0x00082AF4
		public static HurtBox FindBodyMainHurtBox(GameObject bodyObject)
		{
			CharacterBody component = bodyObject.GetComponent<CharacterBody>();
			if (component)
			{
				return Util.FindBodyMainHurtBox(component);
			}
			return null;
		}

		// Token: 0x06001C59 RID: 7257 RVA: 0x00084918 File Offset: 0x00082B18
		public static Vector3 GetCorePosition(CharacterBody characterBody)
		{
			return characterBody.corePosition;
		}

		// Token: 0x06001C5A RID: 7258 RVA: 0x00084920 File Offset: 0x00082B20
		public static Vector3 GetCorePosition(GameObject bodyObject)
		{
			CharacterBody component = bodyObject.GetComponent<CharacterBody>();
			if (component)
			{
				return Util.GetCorePosition(component);
			}
			return bodyObject.transform.position;
		}

		// Token: 0x06001C5B RID: 7259 RVA: 0x00084950 File Offset: 0x00082B50
		public static Transform GetCoreTransform(GameObject bodyObject)
		{
			CharacterBody component = bodyObject.GetComponent<CharacterBody>();
			if (component)
			{
				return component.coreTransform;
			}
			return bodyObject.transform;
		}

		// Token: 0x06001C5C RID: 7260 RVA: 0x00084979 File Offset: 0x00082B79
		public static float SphereRadiusToVolume(float radius)
		{
			return 4.1887903f * (radius * radius * radius);
		}

		// Token: 0x06001C5D RID: 7261 RVA: 0x00084986 File Offset: 0x00082B86
		public static float SphereVolumeToRadius(float volume)
		{
			return Mathf.Pow(3f * volume / 12.566371f, 0.33333334f);
		}

		// Token: 0x06001C5E RID: 7262 RVA: 0x000849A0 File Offset: 0x00082BA0
		public static void CopyList<T>(List<T> src, List<T> dest)
		{
			dest.Clear();
			foreach (T item in src)
			{
				dest.Add(item);
			}
		}

		// Token: 0x06001C5F RID: 7263 RVA: 0x000849F4 File Offset: 0x00082BF4
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

		// Token: 0x06001C60 RID: 7264 RVA: 0x00084AC4 File Offset: 0x00082CC4
		public static void DebugCross(Vector3 position, float radius, Color color, float duration)
		{
			Debug.DrawLine(position - Vector3.right * radius, position + Vector3.right * radius, color, duration);
			Debug.DrawLine(position - Vector3.up * radius, position + Vector3.up * radius, color, duration);
			Debug.DrawLine(position - Vector3.forward * radius, position + Vector3.forward * radius, color, duration);
		}

		// Token: 0x06001C61 RID: 7265 RVA: 0x00084B4C File Offset: 0x00082D4C
		public static bool PositionIsValid(Vector3 value)
		{
			float f = value.x + value.y + value.z;
			return !float.IsInfinity(f) && !float.IsNaN(f);
		}

		// Token: 0x06001C62 RID: 7266 RVA: 0x00084B84 File Offset: 0x00082D84
		public static void Swap<T>(ref T a, ref T b)
		{
			T t = a;
			a = b;
			b = t;
		}

		// Token: 0x06001C63 RID: 7267 RVA: 0x00084BAC File Offset: 0x00082DAC
		public static DateTime UnixTimeStampToDateTimeUtc(uint unixTimeStamp)
		{
			return Util.dateZero.AddSeconds(unixTimeStamp).ToUniversalTime();
		}

		// Token: 0x04001E77 RID: 7799
		public static readonly DateTime dateZero = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		// Token: 0x020004E1 RID: 1249
		private struct EasyTargetCandidate
		{
			// Token: 0x04001E78 RID: 7800
			public Transform transform;

			// Token: 0x04001E79 RID: 7801
			public float score;

			// Token: 0x04001E7A RID: 7802
			public float distance;
		}
	}
}
