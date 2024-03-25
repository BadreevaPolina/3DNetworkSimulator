using System;
using GNS.ProjectHandling.Node;
using GNS.ProjectHandling.Project;
using GNS3.ProjectHandling.Project;
using GNS3.GNSConsole;
using Objects.Devices.Common;
using Objects.Devices.Common.ConsoleDevice;
using Objects.Parts.Controllers.Scripts;
using Objects.Parts.Wire;
using UI.Console;
using UI.Terminal;
using UnityEngine;

namespace Objects.Devices.Router.MiniRouter
{
    public class MiniRouter : AConsoleDevice
    {
        [SerializeField] private AWire[] ethernetCables = new AWire[4];
        [SerializeField] private Switchable powerIndicator;
        [SerializeField] private AWire powerPort;


        [SerializeField] private GameObject uiTerminalPrefab;
        [SerializeField] private Canvas parentCanvas;
        [SerializeField] private Canvas screenCanvas;

        private IEventConsole _console;
        private TerminalManagerRouter _uiTerminal;

        public void Start()
        {
            parentCanvas = GameObject.FindWithTag("UI").GetComponent<Canvas>();

            _uiTerminal = Instantiate(uiTerminalPrefab, parentCanvas.transform).GetComponent<TerminalManagerRouter>();
            _uiTerminal.Initialize(screenCanvas);

            powerPort.ConnectEvent += Enable;
            powerPort.DisconnectEvent += Disable;

            foreach (var en in ethernetCables)
            {
                en.SingleConnectEvent += other =>
                    Node.ConnectTo(
                        other.GetParent().GetComponent<ADevice>().Node,
                        en.GetPortNumber(),
                        other.GetPortNumber(),
                        en.GetAdapterNumber(),
                        other.GetAdapterNumber()
                    );
                en.SingleDisconnectEvent += other =>
                    Node.DisconnectFrom(
                        other.GetParent().GetComponent<ADevice>().Node,
                        en.GetPortNumber(),
                        other.GetPortNumber(),
                        en.GetAdapterNumber(),
                        other.GetAdapterNumber()
                    );
            }
        }

        public override void OpenConsole()
        {
            if (!Node.IsReady)
            {
                GlobalNotificationManager.AddMessage("[<color=red>FL</color>] Can't connect to " + Node.Name +
                                                     " as it is not loaded");
                return;
            }

            if (!Node.IsStarted)
            {
                GlobalNotificationManager.AddMessage("[<color=red>FL</color>] Can't connect to " + Node.Name +
                                                     " as it is not started");
                return;
            }

            if (_console is null)
            {
                _console = Node.GetTerminal();
                _uiTerminal.LinkTo(_console);
                _uiTerminal.SetTitle(Node.Name);
                _uiTerminal.SetPre("Router>");
            }

            _uiTerminal.Show();
        }

        private void Enable()
        {
            powerIndicator.SwitchOn();
            Node.Start();
        }

        private void Disable()
        {
            powerIndicator.SwitchOff();
            Node.Stop();
        }


        public override void CreateNode(GnsProject parent)
        {
            Node = new GnsSRouterNode(parent, "Mini router");
            Node.Start();
        }

        public override AWire GetWire(int adapterNumber, int portNumber)
        {
            if (adapterNumber > 3 || portNumber > 3)
                throw new ArgumentException("Mini router has only 4 ports");

            return ethernetCables[portNumber];
        }

    }
}
