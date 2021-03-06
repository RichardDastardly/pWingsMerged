﻿namespace ProceduralWings
{
    public class WingProperty :IConfigNode
    {
        public string ID { get; }
        public string name;
        public double defaultValue;
        public int decPlaces;
        public double min;
        public double max;
        public string tooltip;

        double value;
        public double Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                if (UI.WindowManager.Window != null)
                    UI.WindowManager.Window.SetLastModifiedProperty(this);
            }
        }

        public WingProperty(string Name, string id, double DefaultV = 0, int DecPlaces = 2, double Min = 0, double Max = 0, string Tooltip = "")
        {
            ID = id;
            name = Name;
            value = defaultValue = DefaultV;
            decPlaces = DecPlaces;
            min = Min;
            max = Max;
            tooltip = Tooltip;
        }

        public WingProperty(WingProperty prop)
        {
            ID = prop.ID;
            Update(prop);
        }

        public void Update(WingProperty prop)
        {
            name = prop.name;
            value = prop.Value;
            defaultValue = prop.defaultValue;
            decPlaces = prop.decPlaces;
            min = prop.min;
            max = prop.max;
            tooltip = prop.tooltip;
        }

        public void Load(ConfigNode node)
        {
            node.TryGetValue(nameof(value), ref value);
        }

        public void Save(ConfigNode node)
        {
            ConfigNode pNode = new ConfigNode("WING_PROPERTY");
            pNode.AddValue(nameof(ID), ID);
            pNode.AddValue(nameof(value), value);
            node.AddNode(pNode);
        }
    }
}
