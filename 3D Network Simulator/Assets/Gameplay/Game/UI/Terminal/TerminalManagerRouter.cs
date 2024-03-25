using System.Collections.Generic;
using System.Text;
using System.Linq;
using GNS3.GNSConsole;
using Interfaces.TextTransformer;
using Objects.Player.Scripts;
using Text;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace UI.Terminal
{
    /*
    * Creates the process and uses it's input and output with unity
    */
    [RequireComponent(typeof(CanvasGroup))]
    public class TerminalManagerRouter : MonoBehaviour
    {
        private const int MessageHeight = 25;
        [SerializeField] private GameObject directoryLine;
        [SerializeField] private GameObject responseLine;
        [SerializeField] private TMP_InputField terminalInput;
        [SerializeField] private GameObject userInputLine;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private GameObject messageList;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI preText;
        [SerializeField] private Button closeButton;
        private readonly Queue<string> _messages = new();
        private Canvas _baseCanvas;
        private RectTransform _baseCanvasRectTransform;
        private Vector3 _cachedPosition;
        private Vector3 _cachedScale;
        private CanvasGroup _canvasGroup;

        private IEventConsole _console;
        private string _lastInput = "";
        private string currentDisplayMessageLine = "";
        private PlayerMovement _playerMovement;
        private ITextTransformer _textTransformer;
        private Canvas ScreenCanvas { get; set; }

        private void OnGUI()
        {
            InstantiateMessages();
            
            if (terminalInput.isFocused && terminalInput.text != "" && Input.GetKeyDown(KeyCode.Return))
            {
                var userInput = terminalInput.text;
                ClearInputField();
                AddDirectoryLine(userInput);
                Send(userInput);
                _lastInput = userInput;
                userInputLine.transform.SetAsLastSibling();
                terminalInput.ActivateInputField();
                terminalInput.Select();
                userInputLine.SetActive(false);
                UpdateMessagesHeight();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
                Hide();

            userInputLine.transform.SetAsLastSibling();
        }

        public void SetTitle(string title)
        {
            titleText.text = title;
        }

        public void SetPre(string pre)
        {
            preText.text = pre;
        }


        public void Initialize(Canvas screenCanvas)
        {
            ScreenCanvas = screenCanvas;
            _canvasGroup = gameObject.GetComponent<CanvasGroup>();
            _baseCanvas = gameObject.transform.parent.gameObject.GetComponent<Canvas>();
            _baseCanvasRectTransform = gameObject.GetComponent<RectTransform>();
            _cachedPosition = _baseCanvasRectTransform.localPosition;
            _cachedScale = _baseCanvasRectTransform.localScale;
            closeButton.onClick.AddListener(Hide);
            _playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
            _textTransformer = new EscapeSequenceRemover();

            Hide();
            LayoutRebuilder.ForceRebuildLayoutImmediate(messageList.GetComponent<RectTransform>());
        }

        private void Hide()
        {
            SetVisible(false);

            // Unfreeze the player
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _playerMovement.InControl = true;
        }

        private void UpdateMessagesHeight()
        {
            var msgListSize = messageList.GetComponent<RectTransform>().sizeDelta;
            messageList.GetComponent<RectTransform>().sizeDelta =
                new Vector2(msgListSize.x, messageList.transform.childCount * MessageHeight);
            scrollRect.content.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
            scrollRect.verticalNormalizedPosition = 0;
        }

        private void InstantiateMessages()
        {
            if (_messages.Count == 0) return;

            while (_messages.Count > 0)
            {
                var stringText = _messages.Dequeue();

                if (stringText.Contains("Router"))
                {
                    userInputLine.GetComponentsInChildren<TextMeshProUGUI>()[0].text = stringText;
                    userInputLine.SetActive(true);
                    continue;
                }
                else
                {
                    if (stringText.Trim().EndsWith("OFF")){
                        userInputLine.SetActive(true);
                        _console.SendMessage("\r");
                        continue;
                    }
                    if (stringText.Contains("--More--")){
                        stringText = "";
                        _console.SendMessage("  ");
                    }
                }

                var msg = Instantiate(responseLine, messageList.transform);
                msg.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = stringText;
                msg.transform.SetAsLastSibling();
            }

            scrollRect.content.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
            scrollRect.verticalNormalizedPosition = 0;

            UpdateMessagesHeight();
            LayoutRebuilder.ForceRebuildLayoutImmediate(messageList.GetComponent<RectTransform>());
        }

        private void ClearInputField()
        {
            terminalInput.text = "";
        }

        private void AddDirectoryLine(string userInput)
        {
            var msg = Instantiate(directoryLine, messageList.transform);
            msg.transform.SetSiblingIndex(messageList.transform.childCount - 1);
            msg.GetComponentsInChildren<TextMeshProUGUI>()[0].text = preText.text;
            msg.GetComponentsInChildren<TextMeshProUGUI>()[1].text = userInput;
        }

        public void LinkTo(IEventConsole console)
        {
            _console = console;
            Debug.Log("Linking WS!");
            console.AddOnMessageListener(DisplayMessage);
            console.Connect();
        }

        [Preserve]
        private void DisplayMessage(byte[] text)
        {
            var stringText = Encoding.ASCII.GetString(text);

            foreach (var character in stringText)
            {
                currentDisplayMessageLine += character;
 
                if ((currentDisplayMessageLine.Length > 85 && character == ' ') || character =='\n')
                {
                    if (character == ' ') currentDisplayMessageLine += "\n";
                    DisplayMessageLine();
                }
            }

            if (stringText.EndsWith(": ") && (currentDisplayMessageLine != string.Empty))
            {
                currentDisplayMessageLine += "\r\n";
                DisplayMessageLine();
            }
            if ((currentDisplayMessageLine.Contains("Router") &&
             (new[] { ">", ")", "#" }.Any(currentDisplayMessageLine.Contains))) || 
             currentDisplayMessageLine.Contains("--More--"))
            {
                DisplayMessageLine();
            }
        }

        private void DisplayMessageLine()
        {
            if (ValidateLine(currentDisplayMessageLine))
            {
                _messages.Enqueue(ProcessText(currentDisplayMessageLine.Trim()));
            }
            
            currentDisplayMessageLine = string.Empty;
        }

        private bool ValidateLine(string line)
        {
            if (line.Trim() == _lastInput.Trim()) return false;
            if (line.Contains("??????")) return false;

            return true;
        }

        public void Show()
        {
            SetVisible(true);

            terminalInput.ActivateInputField();
            terminalInput.Select();

            // Freeze the player
            _playerMovement.InControl = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void SetVisible(bool state)
        {
            _canvasGroup.interactable = state;
            ChangeCanvas(state);
            _canvasGroup.blocksRaycasts = state;
        }

        private void ChangeCanvas(bool active)
        {
            if (active)
            {
                gameObject.transform.SetParent(_baseCanvas.transform);

                Transform transform1;
                (transform1 = transform).SetLocalPositionAndRotation(_cachedPosition, Quaternion.Euler(Vector3.zero));
                transform1.localScale = _cachedScale;

                _baseCanvasRectTransform.localPosition = _cachedPosition;
                _baseCanvasRectTransform.localScale = _cachedScale;
            }
            else
            {
                gameObject.transform.SetParent(ScreenCanvas.transform);

                var transform1 = transform;
                transform1.localScale = Vector3.one * 1.5f;
                transform1.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.Euler(Vector3.zero);
            }
        }

        private void Send(string msg)
        {
            _console.SendMessage(msg);
            _console.SendMessage("\r\n");
        }

        private string ProcessText(string response)
        {
            return _textTransformer.Process(response);
        }
    }
}