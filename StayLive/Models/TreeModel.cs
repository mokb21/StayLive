using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StayLive.Models
{
    
    public class TreeModel
    {
        public string id { get; set; }
        public string text { get; set; }
        public NodeState state { get; set; }
        public string icon { get; set; }
        public string parent { get; set; }
    }

    public class NodeState
    {
        public bool opened { get; set; }
        public bool disabled { get; set; }
        public bool selected { get; set; }
        public NodeState(bool opened, bool disabled, bool selected)
        {
            this.opened = opened;
            this.disabled = disabled;
            this.selected = selected;
        }

        public NodeState()
        {
            this.opened = true;
            this.disabled = false;
            this.selected = false;
        }
    }
}