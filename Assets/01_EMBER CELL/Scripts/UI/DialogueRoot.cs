using UnityEngine;

namespace TEC
{
    public class DialogueRoot : UIBase
    {
        public static DialogueRoot Instance { get; private set; }

        public DialogueUI DialogueUI => dialogueUI;
        public QuestAcceptUI QuestAcceptUI => questAcceptUI;

        [SerializeField] private DialogueUI dialogueUI;
        [SerializeField] private QuestAcceptUI questAcceptUI;

        private void Awake()
        {
            Instance = this;
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}