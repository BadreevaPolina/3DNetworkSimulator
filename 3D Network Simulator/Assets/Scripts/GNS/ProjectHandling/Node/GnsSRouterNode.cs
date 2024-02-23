using GNS.JsonObjects;
using GNS.ProjectHandling.Project;
using GNS3.JsonObjects;

namespace GNS.ProjectHandling.Node
{
    public class GnsSRouterNode : GnsNode
    {
        private GnsJsRouterNode _jNode;

        public GnsSRouterNode(GnsProject project, string name)
        {
            Init(name, project);
            InitializeNode();
        }

        private void InitializeNode()
        {
            void AssignNode(GnsJsRouterNode jNode)
            {
                _jNode = jNode;
                ID = _jNode.node_id;
                IsReady = true;
            }

            Project.CreateNode<GnsJsRouterNode>(Name, "dynamips", AssignNode, this);
        }
    }
}