using System.Collections.Generic;
using GNS3.JsonObjects.Basic;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace GNS.JsonObjects
{
    [Preserve]
    public class DataLinkTypes
    {
        [Preserve] public string Ethernet { get; set; }

        [JsonConstructor] public DataLinkTypes() { }
    }

    [Preserve]
    public class Label
    {
        [Preserve] public int rotation { get; set; }
        [Preserve] public object style { get; set; }
        [Preserve] public string text { get; set; }
        [Preserve] public object x { get; set; }
        [Preserve] public int y { get; set; }

        [JsonConstructor] public Label() { }
    }

    [Preserve]
    public class Port
    {
        [Preserve] public int adapter_number { get; set; }
        [Preserve] public DataLinkTypes data_link_types { get; set; }
        [Preserve] public string link_type { get; set; }
        [Preserve] public string name { get; set; }
        [Preserve] public int port_number { get; set; }
        [Preserve] public string short_name { get; set; }

        [JsonConstructor] public Port() { }
    }

}