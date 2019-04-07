using System;
using System.Collections.Generic;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002EB RID: 747
	public class ExperienceManager : MonoBehaviour
	{
		// Token: 0x17000148 RID: 328
		// (get) Token: 0x06000F18 RID: 3864 RVA: 0x0004A9CD File Offset: 0x00048BCD
		// (set) Token: 0x06000F19 RID: 3865 RVA: 0x0004A9D4 File Offset: 0x00048BD4
		public static ExperienceManager instance { get; private set; }

		// Token: 0x06000F1A RID: 3866 RVA: 0x0004A9DC File Offset: 0x00048BDC
		private static float CalcOrbTravelTime(float timeOffset)
		{
			return 0.5f + 1.5f * timeOffset;
		}

		// Token: 0x06000F1B RID: 3867 RVA: 0x0004A9EB File Offset: 0x00048BEB
		private void OnEnable()
		{
			if (ExperienceManager.instance && ExperienceManager.instance != this)
			{
				Debug.LogError("Only one ExperienceManager can exist at a time.");
				return;
			}
			ExperienceManager.instance = this;
		}

		// Token: 0x06000F1C RID: 3868 RVA: 0x0004AA17 File Offset: 0x00048C17
		private void OnDisable()
		{
			if (ExperienceManager.instance == this)
			{
				ExperienceManager.instance = null;
			}
		}

		// Token: 0x06000F1D RID: 3869 RVA: 0x0004AA2C File Offset: 0x00048C2C
		private void Start()
		{
			this.localTime = 0f;
			this.nextAward = float.PositiveInfinity;
		}

		// Token: 0x06000F1E RID: 3870 RVA: 0x0004AA44 File Offset: 0x00048C44
		private void FixedUpdate()
		{
			this.localTime += Time.fixedDeltaTime;
			if (this.pendingAwards.Count > 0 && this.nextAward <= this.localTime)
			{
				this.nextAward = float.PositiveInfinity;
				for (int i = this.pendingAwards.Count - 1; i >= 0; i--)
				{
					if (this.pendingAwards[i].awardTime <= this.localTime)
					{
						if (TeamManager.instance)
						{
							TeamManager.instance.GiveTeamExperience(this.pendingAwards[i].recipient, this.pendingAwards[i].awardAmount);
						}
						this.pendingAwards.RemoveAt(i);
					}
					else if (this.pendingAwards[i].awardTime < this.nextAward)
					{
						this.nextAward = this.pendingAwards[i].awardTime;
					}
				}
			}
		}

		// Token: 0x06000F1F RID: 3871 RVA: 0x0004AB40 File Offset: 0x00048D40
		public void AwardExperience(Vector3 origin, CharacterBody body, ulong amount)
		{
			CharacterMaster master = body.master;
			if (!master)
			{
				return;
			}
			TeamIndex teamIndex = master.teamIndex;
			List<ulong> list = this.CalculateDenominations(amount);
			uint num = 0u;
			for (int i = 0; i < list.Count; i++)
			{
				this.AddPendingAward(this.localTime + 0.5f + 1.5f * ExperienceManager.orbTimeOffsetSequence[(int)num], teamIndex, list[i]);
				num += 1u;
				if ((ulong)num >= (ulong)((long)ExperienceManager.orbTimeOffsetSequence.Length))
				{
					num = 0u;
				}
			}
			ExperienceManager.currentOutgoingCreateExpEffectMessage.awardAmount = amount;
			ExperienceManager.currentOutgoingCreateExpEffectMessage.origin = origin;
			ExperienceManager.currentOutgoingCreateExpEffectMessage.targetBody = body.gameObject;
			NetworkServer.SendToAll(55, ExperienceManager.currentOutgoingCreateExpEffectMessage);
		}

		// Token: 0x06000F20 RID: 3872 RVA: 0x0004ABF4 File Offset: 0x00048DF4
		private void AddPendingAward(float awardTime, TeamIndex recipient, ulong awardAmount)
		{
			this.pendingAwards.Add(new ExperienceManager.TimedExpAward
			{
				awardTime = awardTime,
				recipient = recipient,
				awardAmount = awardAmount
			});
			if (this.nextAward > awardTime)
			{
				this.nextAward = awardTime;
			}
		}

		// Token: 0x06000F21 RID: 3873 RVA: 0x0004AC40 File Offset: 0x00048E40
		public List<ulong> CalculateDenominations(ulong total)
		{
			List<ulong> list = new List<ulong>();
			while (total > 0UL)
			{
				ulong num = (ulong)Math.Pow(6.0, (double)Mathf.Floor(Mathf.Log(total, 6f)));
				total = Math.Max(total - num, 0UL);
				list.Add(num);
			}
			return list;
		}

		// Token: 0x06000F22 RID: 3874 RVA: 0x0004AC91 File Offset: 0x00048E91
		[NetworkMessageHandler(msgType = 55, client = true)]
		private static void HandleCreateExpEffect(NetworkMessage netMsg)
		{
			if (ExperienceManager.instance)
			{
				ExperienceManager.instance.HandleCreateExpEffectInternal(netMsg);
			}
		}

		// Token: 0x06000F23 RID: 3875 RVA: 0x0004ACAC File Offset: 0x00048EAC
		private void HandleCreateExpEffectInternal(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<ExperienceManager.CreateExpEffectMessage>(ExperienceManager.currentIncomingCreateExpEffectMessage);
			GameObject targetBody = ExperienceManager.currentIncomingCreateExpEffectMessage.targetBody;
			if (!targetBody)
			{
				return;
			}
			HurtBox hurtBox = Util.FindBodyMainHurtBox(targetBody);
			Transform targetTransform = ((hurtBox != null) ? hurtBox.transform : null) ?? targetBody.transform;
			List<ulong> list = this.CalculateDenominations(ExperienceManager.currentIncomingCreateExpEffectMessage.awardAmount);
			uint num = 0u;
			for (int i = 0; i < list.Count; i++)
			{
				ExperienceOrbBehavior component = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/ExpOrb"), ExperienceManager.currentIncomingCreateExpEffectMessage.origin, Quaternion.identity).GetComponent<ExperienceOrbBehavior>();
				component.targetTransform = targetTransform;
				component.travelTime = ExperienceManager.CalcOrbTravelTime(ExperienceManager.orbTimeOffsetSequence[(int)num]);
				component.exp = list[i];
				num += 1u;
				if ((ulong)num >= (ulong)((long)ExperienceManager.orbTimeOffsetSequence.Length))
				{
					num = 0u;
				}
			}
		}

		// Token: 0x0400132B RID: 4907
		private float localTime;

		// Token: 0x0400132C RID: 4908
		private List<ExperienceManager.TimedExpAward> pendingAwards = new List<ExperienceManager.TimedExpAward>();

		// Token: 0x0400132D RID: 4909
		private float nextAward;

		// Token: 0x0400132E RID: 4910
		private const float minOrbTravelTime = 0.5f;

		// Token: 0x0400132F RID: 4911
		public const float maxOrbTravelTime = 2f;

		// Token: 0x04001330 RID: 4912
		public static readonly float[] orbTimeOffsetSequence = new float[]
		{
			0.841f,
			0.394f,
			0.783f,
			0.799f,
			0.912f,
			0.197f,
			0.335f,
			0.768f,
			0.278f,
			0.554f,
			0.477f,
			0.629f,
			0.365f,
			0.513f,
			0.953f,
			0.917f
		};

		// Token: 0x04001331 RID: 4913
		private static ExperienceManager.CreateExpEffectMessage currentOutgoingCreateExpEffectMessage = new ExperienceManager.CreateExpEffectMessage();

		// Token: 0x04001332 RID: 4914
		private static ExperienceManager.CreateExpEffectMessage currentIncomingCreateExpEffectMessage = new ExperienceManager.CreateExpEffectMessage();

		// Token: 0x020002EC RID: 748
		[Serializable]
		private struct TimedExpAward
		{
			// Token: 0x04001333 RID: 4915
			public float awardTime;

			// Token: 0x04001334 RID: 4916
			public ulong awardAmount;

			// Token: 0x04001335 RID: 4917
			public TeamIndex recipient;
		}

		// Token: 0x020002ED RID: 749
		private class CreateExpEffectMessage : MessageBase
		{
			// Token: 0x06000F27 RID: 3879 RVA: 0x0004ADB8 File Offset: 0x00048FB8
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.origin);
				writer.Write(this.targetBody);
				writer.WritePackedUInt64(this.awardAmount);
			}

			// Token: 0x06000F28 RID: 3880 RVA: 0x0004ADDE File Offset: 0x00048FDE
			public override void Deserialize(NetworkReader reader)
			{
				this.origin = reader.ReadVector3();
				this.targetBody = reader.ReadGameObject();
				this.awardAmount = reader.ReadPackedUInt64();
			}

			// Token: 0x04001336 RID: 4918
			public Vector3 origin;

			// Token: 0x04001337 RID: 4919
			public GameObject targetBody;

			// Token: 0x04001338 RID: 4920
			public ulong awardAmount;
		}
	}
}
