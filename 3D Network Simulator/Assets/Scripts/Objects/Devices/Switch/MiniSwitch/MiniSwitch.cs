using System;
using GNS.ProjectHandling.Node;
using GNS.ProjectHandling.Project;
using GNS3.ProjectHandling.Project;
using Objects.Devices.Common;
using Objects.Parts.Controllers.Scripts;
using Objects.Parts.Wire;
using UnityEngine;

namespace Objects.Devices.Switch.MiniSwitch
{
    public class MiniSwitch : ADevice
    {
        [SerializeField] private AWire[] ethernetCables = new AWire[8];
        [SerializeField] private Switchable powerIndicator;
        [SerializeField] private AWire powerPort;

        public void Start()
        {
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
            Node = new GnsSSwitchNode(parent, "Mini switch");
        }
        
        public override AWire GetWire(int adapterNumber, int portNumber)
        {
            if (adapterNumber != 0 || portNumber > 7)
                throw new ArgumentException("Mini switch has only 8 ports");
            
            return ethernetCables[portNumber];
        }

    }
}
