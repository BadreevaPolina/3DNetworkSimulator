using GNS.JsonObjects;
using GNS.ProjectHandling.Project;
using GNS3.JsonObjects;

namespace GNS.ProjectHandling.Node
{
    public class GnsSSwitchNode : GnsNode
    {
        private GnsJsSwitchNode _jNode;

        public GnsSSwitchNode(GnsProject project, string name)
        {
            Init(name, project);
            InitializeNode();
        }

        private void InitializeNode()
        {
            void AssignNode(GnsJsSwitchNode jNode)
            {
                _jNode = jNode;
                ID = _jNode.node_id;
                IsReady = true;
            }

            Project.CreateNode<GnsJsSwitchNode>(Name, "ethernet_switch", AssignNode, this);
        }
    }
}