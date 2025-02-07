using DCL.Interface;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DCL.Huds.QuestsPanel
{
    public class QuestsPanelTask_Single : MonoBehaviour, IQuestsPanelTask
    {
        [SerializeField] internal TextMeshProUGUI taskName;
        [SerializeField] internal Toggle status;
        [SerializeField] internal Button jumpInButton;

        internal TaskPayload_Single payload;

        private Action jumpInDelegate;

        public void Awake()
        {
            jumpInButton.onClick.AddListener(() => { jumpInDelegate?.Invoke();});
        }

        public void Populate(QuestTask task)
        {
            payload = JsonUtility.FromJson<TaskPayload_Single>(task.payload);

            jumpInButton.gameObject.SetActive(task.progress < 1 && !string.IsNullOrEmpty(task.coordinates));
            jumpInDelegate = () => WebInterface.SendChatMessage(new ChatMessage
            {
                messageType = ChatMessage.Type.NONE,
                recipient = string.Empty,
                body = $"/goto {task.coordinates}",
            });

            taskName.text = task.name;
            status.isOn = payload.isDone;
        }
    }
}