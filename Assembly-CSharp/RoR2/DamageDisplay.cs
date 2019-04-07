using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002BF RID: 703
	public class DamageDisplay : MonoBehaviour
	{
		// Token: 0x06000E41 RID: 3649 RVA: 0x00046279 File Offset: 0x00044479
		static DamageDisplay()
		{
			UICamera.onUICameraPreCull += DamageDisplay.OnUICameraPreCull;
			RoR2Application.onUpdate += DamageDisplay.UpdateAll;
		}

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x06000E42 RID: 3650 RVA: 0x000462B6 File Offset: 0x000444B6
		public static ReadOnlyCollection<DamageDisplay> readOnlyInstancesList
		{
			get
			{
				return DamageDisplay._readOnlyInstancesList;
			}
		}

		// Token: 0x06000E43 RID: 3651 RVA: 0x000462C0 File Offset: 0x000444C0
		private void Start()
		{
			this.velocity = Vector3.Normalize(Vector3.up + new Vector3(UnityEngine.Random.Range(-this.offset, this.offset), 0f, UnityEngine.Random.Range(-this.offset, this.offset))) * this.magnitude;
			DamageDisplay.instancesList.Add(this);
			this.internalPosition = base.transform.position;
		}

		// Token: 0x06000E44 RID: 3652 RVA: 0x00046337 File Offset: 0x00044537
		private void OnDestroy()
		{
			DamageDisplay.instancesList.Remove(this);
		}

		// Token: 0x06000E45 RID: 3653 RVA: 0x00046348 File Offset: 0x00044548
		public void SetValues(GameObject victim, GameObject attacker, float damage, bool crit, DamageColorIndex damageColorIndex)
		{
			this.victimTeam = TeamIndex.Neutral;
			this.attackerTeam = TeamIndex.Neutral;
			this.scale = 1f;
			this.victim = victim;
			this.attacker = attacker;
			this.crit = crit;
			this.baseColor = DamageColor.FindColor(damageColorIndex);
			string text = Mathf.CeilToInt(Mathf.Abs(damage)).ToString();
			this.heal = (damage < 0f);
			if (this.heal)
			{
				damage = -damage;
				base.transform.parent = victim.transform;
				text = "+" + text;
				this.baseColor = DamageColor.FindColor(DamageColorIndex.Heal);
				this.baseOutlineColor = this.baseColor * Color.gray;
			}
			if (victim)
			{
				TeamComponent component = victim.GetComponent<TeamComponent>();
				if (component)
				{
					this.victimTeam = component.teamIndex;
				}
			}
			if (attacker)
			{
				TeamComponent component2 = attacker.GetComponent<TeamComponent>();
				if (component2)
				{
					this.attackerTeam = component2.teamIndex;
				}
			}
			if (crit)
			{
				text += "!";
				this.baseOutlineColor = Color.red;
			}
			this.textMeshComponent.text = text;
			this.UpdateMagnitude();
		}

		// Token: 0x06000E46 RID: 3654 RVA: 0x00046474 File Offset: 0x00044674
		private void UpdateMagnitude()
		{
			float fontSize = this.magnitudeCurve.Evaluate(this.life / this.maxLife) * this.textMagnitude * this.scale;
			this.textMeshComponent.fontSize = fontSize;
		}

		// Token: 0x06000E47 RID: 3655 RVA: 0x000464B4 File Offset: 0x000446B4
		private static void UpdateAll()
		{
			for (int i = DamageDisplay.instancesList.Count - 1; i >= 0; i--)
			{
				DamageDisplay.instancesList[i].DoUpdate();
			}
		}

		// Token: 0x06000E48 RID: 3656 RVA: 0x000464E8 File Offset: 0x000446E8
		private void DoUpdate()
		{
			this.UpdateMagnitude();
			this.life += Time.deltaTime;
			if (this.life >= this.maxLife)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			this.velocity += this.gravity * Vector3.down * Time.deltaTime;
			this.internalPosition += this.velocity * Time.deltaTime;
		}

		// Token: 0x06000E49 RID: 3657 RVA: 0x00046574 File Offset: 0x00044774
		private static void OnUICameraPreCull(UICamera uiCamera)
		{
			Camera camera = uiCamera.camera;
			Camera sceneCam = uiCamera.cameraRigController.sceneCam;
			GameObject target = uiCamera.cameraRigController.target;
			TeamIndex targetTeamIndex = uiCamera.cameraRigController.targetTeamIndex;
			for (int i = 0; i < DamageDisplay.instancesList.Count; i++)
			{
				DamageDisplay damageDisplay = DamageDisplay.instancesList[i];
				Color b = Color.white;
				if (!damageDisplay.heal)
				{
					if (targetTeamIndex == damageDisplay.victimTeam)
					{
						b = new Color(0.5568628f, 0.29411766f, 0.6039216f);
					}
					else if (targetTeamIndex == damageDisplay.attackerTeam && target != damageDisplay.attacker)
					{
						b = Color.gray;
					}
				}
				damageDisplay.textMeshComponent.color = Color.Lerp(Color.white, damageDisplay.baseColor * b, damageDisplay.life / 0.2f);
				damageDisplay.textMeshComponent.outlineColor = Color.Lerp(Color.white, damageDisplay.baseOutlineColor * b, damageDisplay.life / 0.2f);
				Vector3 position = damageDisplay.internalPosition;
				Vector3 vector = sceneCam.WorldToScreenPoint(position);
				vector.z = ((vector.z > 0f) ? 1f : -1f);
				Vector3 position2 = camera.ScreenToWorldPoint(vector);
				damageDisplay.transform.position = position2;
			}
		}

		// Token: 0x04001225 RID: 4645
		private static List<DamageDisplay> instancesList = new List<DamageDisplay>();

		// Token: 0x04001226 RID: 4646
		private static ReadOnlyCollection<DamageDisplay> _readOnlyInstancesList = new ReadOnlyCollection<DamageDisplay>(DamageDisplay.instancesList);

		// Token: 0x04001227 RID: 4647
		public TextMeshPro textMeshComponent;

		// Token: 0x04001228 RID: 4648
		public AnimationCurve magnitudeCurve;

		// Token: 0x04001229 RID: 4649
		public float maxLife = 3f;

		// Token: 0x0400122A RID: 4650
		public float gravity = 9.81f;

		// Token: 0x0400122B RID: 4651
		public float magnitude = 3f;

		// Token: 0x0400122C RID: 4652
		public float offset = 20f;

		// Token: 0x0400122D RID: 4653
		private Vector3 velocity;

		// Token: 0x0400122E RID: 4654
		public float textMagnitude = 0.01f;

		// Token: 0x0400122F RID: 4655
		private float vel;

		// Token: 0x04001230 RID: 4656
		private float life;

		// Token: 0x04001231 RID: 4657
		private float scale = 1f;

		// Token: 0x04001232 RID: 4658
		[HideInInspector]
		public Color baseColor = Color.white;

		// Token: 0x04001233 RID: 4659
		[HideInInspector]
		public Color baseOutlineColor = Color.gray;

		// Token: 0x04001234 RID: 4660
		private GameObject victim;

		// Token: 0x04001235 RID: 4661
		private GameObject attacker;

		// Token: 0x04001236 RID: 4662
		private TeamIndex victimTeam;

		// Token: 0x04001237 RID: 4663
		private TeamIndex attackerTeam;

		// Token: 0x04001238 RID: 4664
		private bool crit;

		// Token: 0x04001239 RID: 4665
		private bool heal;

		// Token: 0x0400123A RID: 4666
		private Vector3 internalPosition;
	}
}
