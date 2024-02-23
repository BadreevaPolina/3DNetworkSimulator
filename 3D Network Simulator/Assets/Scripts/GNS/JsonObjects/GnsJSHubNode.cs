using System.Collections.Generic;
using GNS3.JsonObjects.Basic;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace GNS.JsonObjects
{
    [Preserve]
    public class PortsMappingHub
    {
        [Preserve] public string name { get; set; }
        [Preserve] public int port_number { get; set; }

        [JsonConstructor] public PortsMappingHub() { }
    }

    [Preserve]
    public class PropertiesHub
    {
        [Preserve] public List<PortsMappingHub> ports_mapping { get; set; }

        [JsonConstructor] public PropertiesHub() { }
    }

    [Preserve]
    public class GnsJsHubNode : GnsJNode
    {
        [Preserve] public object command_line { get; set; }
        [Preserve] public string compute_id { get; set; }
        [Preserve] public object console { get; set; }
        [Preserve] public bool console_auto_start { get; set; }
        [Preserve] public string console_host { get; set; }
        [Preserve] public object console_type { get; set; }
        [Preserve] public List<object> custom_adapters { get; set; }
        [Preserve] public object first_port_name { get; set; }
        [Preserve] public int height { get; set; }
        [Preserve] public Label label { get; set; }
        [Preserve] public bool locked { get; set; }
        [Preserve] public object node_directory { get; set; }
        [Preserve] public string port_name_format { get; set; }
        [Preserve] public int port_segment_size { get; set; }
        [Preserve] public List<Port> ports { get; set; }
        [Preserve] public PropertiesHub properties { get; set; }
        [Preserve] public string status { get; set; }
        [Preserve] public string symbol { get; set; }
        [Preserve] public object template_id { get; set; }
        [Preserve] public int width { get; set; }
        [Preserve] public int x { get; set; }
        [Preserve] public int y { get; set; }
        [Preserve] public int z { get; set; }

        [JsonConstructor] public GnsJsHubNode() { }
    }
}