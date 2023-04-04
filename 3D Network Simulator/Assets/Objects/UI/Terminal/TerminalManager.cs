using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Player;

namespace UI.Terminal
{
    /*
    * Creates the process and uses it's input and output with unity
    */
    public class TerminalManager : MonoBehaviour
    {
        [SerializeField] private GameObject DirectoryLine;
        [SerializeField] private GameObject ResponseLine;
        [SerializeField] private TMP_InputField TerminalInput;
        [SerializeField] private GameObject UserInputLine;
        [SerializeField] private ScrollRect ScrollRect;
        [SerializeField] private GameObject MessageList;
        [SerializeField] private TextMeshProUGUI TitleText;
        [SerializeField] private Button CloseButton;

        private GNSConsole.GNSConsole console;
        private readonly Queue<string> messages = new();
        private const int messageHeight = 25;
        private string lastInput = "";
        private PlayerMovement playerMovement;

        public void SetTitle(string title)
        {
            TitleText.text = title;
        }

        private void Start()
        {
            gameObject.GetComponent<CanvasGroup>().alpha = 0;

            LayoutRebuilder.ForceRebuildLayoutImmediate(MessageList.GetComponent<RectTransform>());

            CloseButton.onClick.AddListener(Hide);

            playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        }

        private void OnGUI()
        {
            InstantiateMessages();

            if (TerminalInput.isFocused && TerminalInput.text != "" && Input.GetKeyDown(KeyCode.Return))
            {
                string userInput = TerminalInput.text;
                ClearInputField();
                AddDirectoryLine(userInput);
                Send(userInput);
                lastInput = userInput;
                UserInputLine.transform.SetAsLastSibling();
                TerminalInput.ActivateInputField();
                TerminalInput.Select();
                UserInputLine.SetActive(false);
                UpdateMessagesHeight();
            }

            if (Input.GetKeyDown(KeyCode.T))
                Hide();

            /*if (updated)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                    Send("[A");
                if (Input.GetKeyDown(KeyCode.DownArrow))
                    Send("[B");
                updated = false;
            }*/

            UserInputLine.transform.SetAsLastSibling();
        }

        private void Hide()
        {
            gameObject.GetComponent<CanvasGroup>().alpha = 0;
        }

        private void UpdateMessagesHeight()
        {
            Vector2 msgListSize = MessageList.GetComponent<RectTransform>().sizeDelta;
            MessageList.GetComponent<RectTransform>().sizeDelta = new Vector2(msgListSize.x, MessageList.transform.childCount * messageHeight);
            ScrollRect.content.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
            ScrollRect.verticalNormalizedPosition = 0;
        }

        private void InstantiateMessages()
        {
            if (messages.Count == 0) return;

            while (messages.Count > 0)
            {
                var stringText = messages.Dequeue();

                if (stringText.Trim().EndsWith(">"))
                {
                    UserInputLine.GetComponentsInChildren<TextMeshProUGUI>()[0].text = stringText;
                    UserInputLine.SetActive(true);
                    continue;
                }

                var msg = Instantiate(ResponseLine, MessageList.transform);
                msg.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = stringText;
                msg.transform.SetAsLastSibling();
            }
            ScrollRect.content.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
            ScrollRect.verticalNormalizedPosition = 0;

            UpdateMessagesHeight();
            LayoutRebuilder.ForceRebuildLayoutImmediate(MessageList.GetComponent<RectTransform>());
        }

        private void ClearInputField()
        {
            TerminalInput.text = "";
        }

        private void AddDirectoryLine(string userInput)
        {
            var msg = Instantiate(DirectoryLine, MessageList.transform);
            msg.transform.SetSiblingIndex(MessageList.transform.childCount - 1);
            msg.GetComponentsInChildren<TextMeshProUGUI>()[1].text = userInput;
        }

        public void LinkTo(GNSConsole.GNSConsole console)
        {
            this.console = console;
            console.AddOnMessageListener(DisplayMessage);
        }

        private void DisplayMessage(byte[] text)
        {
            var stringText = Encoding.ASCII.GetString(text);

            foreach (var line in stringText.Split('\n'))
            {
                if (ValidateLine(line))
                {
                    messages.Enqueue(ReplaceCVTS(line));
                }
            }
        }

        private bool ValidateLine(string line)
        {
            if (line.Trim() == lastInput.Trim()) return false;
            if (line.Contains("??????")) return false;

            return true;
        }

        public void Show()
        {
            gameObject.GetComponent<CanvasGroup>().alpha = 1;
            TerminalInput.ActivateInputField();
            TerminalInput.Select();

            // Freeze the player
            //playerMovement.InControl = false;
        }

        private void Send(string msg)
        {
            console.SendMessage(msg);
            console.SendMessage("\n");
        }

        private string ReplaceCVTS(string response)
        {
            return Regex.Replace(response, "\\[\\dm", "");
        }
    }
}