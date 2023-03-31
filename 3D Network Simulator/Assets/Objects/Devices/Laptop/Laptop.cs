
using UnityEngine;
using Wire;
using GNSHandling;
using System.Diagnostics;

namespace Device
{
    public class Laptop : AConsoleDevice
    {
        public AWire ethernetPort;
        public void Start()
        {
            Node = new GNSVPCSNode(GlobalGNSParameters.GetProject(), "VPCS" + GlobalGNSParameters.GetNextFreeID());
            ethernetPort.SingleConnectEvent += Connect;
            Node.Start();
        }

        private void Connect(AWire other)
        {
            Node.ConnectTo(other.GetParent().GetComponent<ADevice>().Node, ethernetPort.GetPortNumber(), other.GetPortNumber());
        }

        public override void OpenTelnet()
        {
            Process.Start("telnet", ((GNSVPCSNode)Node).JNode.console_host + " " + ((GNSVPCSNode)Node).JNode.console);
        }
    }
}
