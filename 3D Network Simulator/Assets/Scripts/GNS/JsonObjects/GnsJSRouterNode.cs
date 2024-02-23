using System.Collections.Generic;
using GNS3.JsonObjects.Basic;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace GNS.JsonObjects
{
    [Preserve]
    public class PropertiesRouter
    {
        [Preserve] public int dynamips_id { get; set; }
        [Preserve] public bool auto_delete_disks { get; set; }
        [Preserve] public int clock_divisor { get; set; }
        [Preserve] public int disk0 { get; set; }
        [Preserve] public int disk1 { get; set; }
        [Preserve] public int exec_area { get; set; }
        [Preserve] public int idlemax { get; set; }
        [Preserve] public int idlesleep { get; set; }
        [Preserve] public string image { get; set; }
        [Preserve] public string mac_addr { get; set; }
        [Preserve] public string midplane { get; set; }
        [Preserve] public bool mmap { get; set; }
        [Preserve] public string npe { get; set; }
        [Preserve] public int nvram { get; set; }
        [Preserve] public string platform { get; set; }
        [Preserve] public int[] power_supplies { get; set; }
        [Preserve] public int ram { get; set; } 
        [Preserve] public int[] sensors { get; set; }
        [Preserve] public string slot0 { get; set; }
        [Preserve] public bool sparsemem { get; set; }
        [Preserve] public string system_id { get; set; }
        
        [JsonConstructor] public PropertiesRouter() { }
    }

    [Preserve]
    public class GnsJsRouterNode : GnsJNode
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
        [Preserve] public PropertiesRouter properties { get; set; }
        [Preserve] public string status { get; set; }
        [Preserve] public string symbol { get; set; }
        [Preserve] public object template_id { get; set; }
        [Preserve] public int width { get; set; }
        [Preserve] public int x { get; set; }
        [Preserve] public int y { get; set; }
        [Preserve] public int z { get; set; }

        [JsonConstructor] public GnsJsRouterNode() { }
    }
}
