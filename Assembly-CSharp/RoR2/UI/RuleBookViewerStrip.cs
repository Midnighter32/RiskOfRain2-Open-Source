using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200061B RID: 1563
	public class RuleBookViewerStrip : MonoBehaviour
	{
		// Token: 0x060024F0 RID: 9456 RVA: 0x000A106A File Offset: 0x0009F26A
		private RuleChoiceController CreateChoice()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.choicePrefab, this.choiceContainer);
			gameObject.SetActive(true);
			RuleChoiceController component = gameObject.GetComponent<RuleChoiceController>();
			component.strip = this;
			return component;
		}

		// Token: 0x060024F1 RID: 9457 RVA: 0x000A1090 File Offset: 0x0009F290
		private void DestroyChoice(RuleChoiceController choiceController)
		{
			UnityEngine.Object.Destroy(choiceController.gameObject);
		}

		// Token: 0x060024F2 RID: 9458 RVA: 0x000A10A0 File Offset: 0x0009F2A0
		public void SetData(List<RuleChoiceDef> newChoices, int choiceIndex)
		{
			this.AllocateChoices(newChoices.Count);
			int num = this.currentDisplayChoiceIndex;
			int count = newChoices.Count;
			bool canVote = count > 1;
			for (int i = 0; i < count; i++)
			{
				this.choiceControllers[i].canVote = canVote;
				this.choiceControllers[i].SetChoice(newChoices[i]);
				if (newChoices[i].localIndex == choiceIndex)
				{
					num = i;
				}
			}
			this.currentDisplayChoiceIndex = num;
		}

		// Token: 0x060024F3 RID: 9459 RVA: 0x000A111C File Offset: 0x0009F31C
		private void AllocateChoices(int desiredCount)
		{
			while (this.choiceControllers.Count > desiredCount)
			{
				int index = this.choiceControllers.Count - 1;
				this.DestroyChoice(this.choiceControllers[index]);
				this.choiceControllers.RemoveAt(index);
			}
			while (this.choiceControllers.Count < desiredCount)
			{
				this.choiceControllers.Add(this.CreateChoice());
			}
		}

		// Token: 0x060024F4 RID: 9460 RVA: 0x000A1188 File Offset: 0x0009F388
		public void Update()
		{
			if (this.choiceControllers.Count == 0)
			{
				return;
			}
			if (this.currentDisplayChoiceIndex >= this.choiceControllers.Count)
			{
				this.currentDisplayChoiceIndex = this.choiceControllers.Count - 1;
			}
			Vector3 localPosition = this.choiceControllers[this.currentDisplayChoiceIndex].transform.localPosition;
			float target = 0f;
			RectTransform.Axis axis = this.movementAxis;
			if (axis != RectTransform.Axis.Horizontal)
			{
				if (axis == RectTransform.Axis.Vertical)
				{
					target = -localPosition.y;
				}
			}
			else
			{
				target = -localPosition.x;
			}
			this.currentPosition = Mathf.SmoothDamp(this.currentPosition, target, ref this.velocity, this.movementDuration);
			this.UpdatePosition();
		}

		// Token: 0x060024F5 RID: 9461 RVA: 0x000A1231 File Offset: 0x0009F431
		private void OnEnable()
		{
			this.UpdatePosition();
		}

		// Token: 0x060024F6 RID: 9462 RVA: 0x000A123C File Offset: 0x0009F43C
		private void UpdatePosition()
		{
			Vector3 localPosition = this.choiceContainer.localPosition;
			RectTransform.Axis axis = this.movementAxis;
			if (axis != RectTransform.Axis.Horizontal)
			{
				if (axis == RectTransform.Axis.Vertical)
				{
					localPosition.y = this.currentPosition;
				}
			}
			else
			{
				localPosition.x = this.currentPosition;
			}
			this.choiceContainer.localPosition = localPosition;
		}

		// Token: 0x040022AF RID: 8879
		public GameObject choicePrefab;

		// Token: 0x040022B0 RID: 8880
		public RectTransform choiceContainer;

		// Token: 0x040022B1 RID: 8881
		public RectTransform.Axis movementAxis = RectTransform.Axis.Vertical;

		// Token: 0x040022B2 RID: 8882
		public float movementDuration = 0.1f;

		// Token: 0x040022B3 RID: 8883
		private RuleDef currentRuleDef;

		// Token: 0x040022B4 RID: 8884
		public readonly List<RuleChoiceController> choiceControllers = new List<RuleChoiceController>();

		// Token: 0x040022B5 RID: 8885
		public int currentDisplayChoiceIndex;

		// Token: 0x040022B6 RID: 8886
		private float velocity;

		// Token: 0x040022B7 RID: 8887
		private float currentPosition;
	}
}
