using System;
using System.Collections.Generic;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020001F9 RID: 505
	public class ExperienceManager : MonoBehaviour
	{
		// Token: 0x1700015A RID: 346
		// (get) Token: 0x06000AC0 RID: 2752 RVA: 0x0002F8FD File Offset: 0x0002DAFD
		// (set) Token: 0x06000AC1 RID: 2753 RVA: 0x0002F904 File Offset: 0x0002DB04
		public static ExperienceManager instance { get; private set; }

		// Token: 0x06000AC2 RID: 2754 RVA: 0x0002F90C File Offset: 0x0002DB0C
		private static float CalcOrbTravelTime(float timeOffset)
		{
			return 0.5f + 1.5f * timeOffset;
		}

		// Token: 0x06000AC3 RID: 2755 RVA: 0x0002F91B File Offset: 0x0002DB1B
		private void OnEnable()
		{
			if (ExperienceManager.instance && ExperienceManager.instance != this)
			{
				Debug.LogError("Only one ExperienceManager can exist at a time.");
				return;
			}
			ExperienceManager.instance = this;
		}

		// Token: 0x06000AC4 RID: 2756 RVA: 0x0002F947 File Offset: 0x0002DB47
		private void OnDisable()
		{
			if (ExperienceManager.instance == this)
			{
				ExperienceManager.instance = null;
			}
		}

		// Token: 0x06000AC5 RID: 2757 RVA: 0x0002F95C File Offset: 0x0002DB5C
		private void Start()
		{
			this.localTime = 0f;
			this.nextAward = float.PositiveInfinity;
		}

		// Token: 0x06000AC6 RID: 2758 RVA: 0x0002F974 File Offset: 0x0002DB74
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

		// Token: 0x06000AC7 RID: 2759 RVA: 0x0002FA70 File Offset: 0x0002DC70
		public void AwardExperience(Vector3 origin, CharacterBody body, ulong amount)
		{
			CharacterMaster master = body.master;
			if (!master)
			{
				return;
			}
			TeamIndex teamIndex = master.teamIndex;
			List<ulong> list = this.CalculateDenominations(amount);
			uint num = 0U;
			for (int i = 0; i < list.Count; i++)
			{
				this.AddPendingAward(this.localTime + 0.5f + 1.5f * ExperienceManager.orbTimeOffsetSequence[(int)num], teamIndex, list[i]);
				num += 1U;
				if ((ulong)num >= (ulong)((long)ExperienceManager.orbTimeOffsetSequence.Length))
				{
					num = 0U;
				}
			}
			ExperienceManager.currentOutgoingCreateExpEffectMessage.awardAmount = amount;
			ExperienceManager.currentOutgoingCreateExpEffectMessage.origin = origin;
			ExperienceManager.currentOutgoingCreateExpEffectMessage.targetBody = body.gameObject;
			NetworkServer.SendToAll(55, ExperienceManager.currentOutgoingCreateExpEffectMessage);
		}

		// Token: 0x06000AC8 RID: 2760 RVA: 0x0002FB24 File Offset: 0x0002DD24
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

		// Token: 0x06000AC9 RID: 2761 RVA: 0x0002FB70 File Offset: 0x0002DD70
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

		// Token: 0x06000ACA RID: 2762 RVA: 0x0002FBC1 File Offset: 0x0002DDC1
		[NetworkMessageHandler(msgType = 55, client = true)]
		private static void HandleCreateExpEffect(NetworkMessage netMsg)
		{
			if (ExperienceManager.instance)
			{
				ExperienceManager.instance.HandleCreateExpEffectInternal(netMsg);
			}
		}

		// Token: 0x06000ACB RID: 2763 RVA: 0x0002FBDC File Offset: 0x0002DDDC
		private void HandleCreateExpEffectInternal(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<ExperienceManager.CreateExpEffectMessage>(ExperienceManager.currentIncomingCreateExpEffectMessage);
			if (!SettingsConVars.cvExpAndMoneyEffects.value)
			{
				return;
			}
			GameObject targetBody = ExperienceManager.currentIncomingCreateExpEffectMessage.targetBody;
			if (!targetBody)
			{
				return;
			}
			HurtBox hurtBox = Util.FindBodyMainHurtBox(targetBody);
			Transform targetTransform = ((hurtBox != null) ? hurtBox.transform : null) ?? targetBody.transform;
			List<ulong> list = this.CalculateDenominations(ExperienceManager.currentIncomingCreateExpEffectMessage.awardAmount);
			uint num = 0U;
			for (int i = 0; i < list.Count; i++)
			{
				ExperienceOrbBehavior component = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/ExpOrb"), ExperienceManager.currentIncomingCreateExpEffectMessage.origin, Quaternion.identity).GetComponent<ExperienceOrbBehavior>();
				component.targetTransform = targetTransform;
				component.travelTime = ExperienceManager.CalcOrbTravelTime(ExperienceManager.orbTimeOffsetSequence[(int)num]);
				component.exp = list[i];
				num += 1U;
				if ((ulong)num >= (ulong)((long)ExperienceManager.orbTimeOffsetSequence.Length))
				{
					num = 0U;
				}
			}
		}

		// Token: 0x04000B0D RID: 2829
		private float localTime;

		// Token: 0x04000B0E RID: 2830
		private List<ExperienceManager.TimedExpAward> pendingAwards = new List<ExperienceManager.TimedExpAward>();

		// Token: 0x04000B0F RID: 2831
		private float nextAward;

		// Token: 0x04000B10 RID: 2832
		private const float minOrbTravelTime = 0.5f;

		// Token: 0x04000B11 RID: 2833
		public const float maxOrbTravelTime = 2f;

		// Token: 0x04000B12 RID: 2834
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

		// Token: 0x04000B13 RID: 2835
		private static ExperienceManager.CreateExpEffectMessage currentOutgoingCreateExpEffectMessage = new ExperienceManager.CreateExpEffectMessage();

		// Token: 0x04000B14 RID: 2836
		private static ExperienceManager.CreateExpEffectMessage currentIncomingCreateExpEffectMessage = new ExperienceManager.CreateExpEffectMessage();

		// Token: 0x020001FA RID: 506
		[Serializable]
		private struct TimedExpAward
		{
			// Token: 0x04000B15 RID: 2837
			public float awardTime;

			// Token: 0x04000B16 RID: 2838
			public ulong awardAmount;

			// Token: 0x04000B17 RID: 2839
			public TeamIndex recipient;
		}

		// Token: 0x020001FB RID: 507
		private class CreateExpEffectMessage : MessageBase
		{
			// Token: 0x06000ACF RID: 2767 RVA: 0x0002FCF5 File Offset: 0x0002DEF5
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.origin);
				writer.Write(this.targetBody);
				writer.WritePackedUInt64(this.awardAmount);
			}

			// Token: 0x06000AD0 RID: 2768 RVA: 0x0002FD1B File Offset: 0x0002DF1B
			public override void Deserialize(NetworkReader reader)
			{
				this.origin = reader.ReadVector3();
				this.targetBody = reader.ReadGameObject();
				this.awardAmount = reader.ReadPackedUInt64();
			}

			// Token: 0x04000B18 RID: 2840
			public Vector3 origin;

			// Token: 0x04000B19 RID: 2841
			public GameObject targetBody;

			// Token: 0x04000B1A RID: 2842
			public ulong awardAmount;
		}
	}
}
