using System;
using GNS.ProjectHandling.Node;
using GNS.ProjectHandling.Project;
using GNS3.GNSConsole;
using Objects.Devices.Common;
using Objects.Devices.Common.ConsoleDevice;
using Objects.Parts.Wire;
using UI.Console;
using UI.Terminal;
using UnityEngine;

namespace Objects.Devices.PC.Laptop
{
    public class Laptop : AConsoleDevice
    {
        public AWire ethernetPort;

        [SerializeField] private GameObject uiTerminalPrefab;
        [SerializeField] private Canvas parentCanvas;
        [SerializeField] private Canvas screenCanvas;

        private IEventConsole _console;
        private TerminalManagerLaptop _uiTerminal;

        public void Start()
        {
            parentCanvas = GameObject.FindWithTag("UI").GetComponent<Canvas>();

            _uiTerminal = Instantiate(uiTerminalPrefab, parentCanvas.transform).GetComponent<TerminalManagerLaptop>();
            _uiTerminal.Initialize(screenCanvas);

            ethernetPort.SingleConnectEvent += Connect;
            ethernetPort.SingleDisconnectEvent += Disconnect;
        }

        private void Connect(AWire other)
        {
            Node.ConnectTo(
                other.GetParent().GetComponent<ADevice>().Node,
                ethernetPort.GetPortNumber(),
                other.GetPortNumber(),
                ethernetPort.GetAdapterNumber(),
                other.GetAdapterNumber()
            );
        }

        private void Disconnect(AWire other)
        {
            Node.DisconnectFrom(
                other.GetParent().GetComponent<ADevice>().Node,
                ethernetPort.GetPortNumber(),
                other.GetPortNumber(),
                ethernetPort.GetAdapterNumber(),
                other.GetAdapterNumber()
            );
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
                _uiTerminal.SetPre("VPCS>");
            }

            _uiTerminal.Show();
        }


        public override void CreateNode(GnsProject parent)
        {
            Node = new GnsVpcsNode(parent, "Laptop");
            Node.Start();
        }

        public override AWire GetWire(int adapterNumber, int portNumber)
        {
            if (adapterNumber != 0 || portNumber != 0)
                throw new ArgumentException("Laptop has only 1 port");

            return ethernetPort;
        }
    }
}