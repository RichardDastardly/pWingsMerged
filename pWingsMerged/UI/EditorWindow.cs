﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace ProceduralWings.UI
{
    /// <summary>
    /// Window is an instance of the MainEditorPanelPrefab. It has a header block for title and generic info, a set of property groups, and finally the fuel switch options
    /// </summary>
    public class EditorWindow
    {
        bool visible;
        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
                if (!visible)
                {
                    wing = null;
                }
                else
                {
                    windowPosition.position = StaticWingGlobals.uiRectWindowEditor.position;
                }
                canvas.enabled = visible;
            }
        }


        public Base_ProceduralWing wing;

        /// <summary>
        /// window position
        /// </summary>
        RectTransform windowPosition;

        /// <summary>
        /// the canvas
        /// </summary>
        Canvas canvas;

        /// <summary>
        /// The window backing all other elements
        /// </summary>
        GameObject mainPanel;

        /// <summary>
        /// The green header label that displays the type of the selected panel
        /// </summary>
        Text wingType;

        /// <summary>
        /// display for last modified property label
        /// </summary>
        Text lastModifiedProperty;

        /// <summary>
        /// and a tooltip with a bit of explanation about that property
        /// </summary>
        Text lastModifiedPropertyTooltip;

        /// <summary>
        /// it hides the window...
        /// </summary>
        Button closeButton;

        /// <summary>
        /// a list of the propertyGroups that this window is using
        /// </summary>
        List<PropertyGroup> propertyGroupList = new List<PropertyGroup>();

        // can do lookups by property ID into this when the edited property changes
        Dictionary<string, PropertySlider> propertiesDict = new Dictionary<string, PropertySlider>();
        // store the last edited property for quick reference when continuously updating
        PropertySlider lastEditedPropertyRef;

        /// <summary>
        /// Constructor
        /// </summary>
        public EditorWindow()
        {
            // get references to all useful components
            canvas = UnityEngine.Object.Instantiate(StaticWingGlobals.UI_WindowPrefab.GetComponent<Canvas>());
            canvas.enabled = false;
            
            mainPanel = canvas.gameObject.GetChild("MainPanel");
            windowPosition = mainPanel.GetComponent<RectTransform>();
            GameObject headerPanel = mainPanel.GetChild("HeaderPanel");
            wingType = headerPanel.GetChild("WingType").GetComponent<Text>();
            lastModifiedProperty = headerPanel.GetChild("LastModifiedProperty").GetComponent<Text>();
            lastModifiedPropertyTooltip = headerPanel.GetChild("LastModifiedPropertyToolTip").GetComponent<Text>();
            closeButton = headerPanel.GetChild("CloseButton").GetComponent<Button>();
            
            // window position drag event
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Drag;
            entry.callback = new EventTrigger.TriggerEvent();
            entry.callback.AddListener((x) => windowDrag(x));
            mainPanel.GetComponent<EventTrigger>().triggers.Add(entry);

            // close button click hides the window
            closeButton.onClick.AddListener(closeWindow);
        }

        public void AddFuelPanel()
        {
            GameObject fuelPanel = UnityEngine.Object.Instantiate(StaticWingGlobals.UI_FuelPanel);
            fuelPanel.transform.SetParent(mainPanel.transform, false); // parented onto the window

            Dropdown drop = fuelPanel.GetChild("Dropdown").GetComponent<Dropdown>();

            for (int i = 0; i < StaticWingGlobals.wingTankConfigurations.Count; ++i)
            {
                drop.options.Add(new Dropdown.OptionData(StaticWingGlobals.wingTankConfigurations[i].ConfigurationName));
            }
            drop.value = 1;
            drop.value = 0; // refresh stuff?
            drop.onValueChanged.AddListener(fuelSelectedChanged);
        }

        #region Event callbacks

        public void windowDrag(UnityEngine.EventSystems.BaseEventData data)
        {
            windowPosition.position += new Vector3(((PointerEventData)data).delta.x, ((PointerEventData)data).delta.y);
            StaticWingGlobals.uiRectWindowEditor.position = windowPosition.position;
        }

        public void closeWindow()
        {
            canvas.enabled = false;
        }

        public void fuelSelectedChanged(int index)
        {
            wing.fuelSelectedTankSetup = index;
            wing.FuelTankTypeChanged();
        }

        #endregion
        public PropertyGroup AddPropertyGroup(string name, Color groupColour)
        {
            PropertyGroup newGroup = FindPropertyGroup(name);
            if (newGroup == null)
            {
                newGroup = new PropertyGroup(name, groupColour, this);
                propertyGroupList.Add(newGroup);
                newGroup.groupInstance.transform.SetParent(mainPanel.transform, false);
            }
            return newGroup;
        }

        public PropertyGroup FindPropertyGroup(string groupName)
        {
            return propertyGroupList.Find(g => g.Name == groupName);
        }

        public void ResetGroups()
        {
            foreach (PropertyGroup group in propertyGroupList)
            {
                group.groupInstance.SetActive(false);
            }
        }

        public void UpdateProperty(WingProperty wp)
        {
            if (lastEditedPropertyRef.propertyRef.ID == wp.ID)
            {
                lastEditedPropertyRef.Refresh(wp);
            }
            else
            {
                PropertySlider slider;
                if (propertiesDict.TryGetValue(wp.ID, out slider))
                {
                    lastEditedPropertyRef = slider;
                    lastEditedPropertyRef.Refresh(wp);
                    SetLastModifiedProperty(wp);
                }
            }
        }

        public void SetLastModifiedProperty(WingProperty wp)
        {
            lastModifiedPropertyTooltip.text = wp.tooltip;
            lastModifiedProperty.text = wp.name;
        }

        public void GroupAddProperty(PropertySlider slider)
        {
            if (slider?.propertyRef?.ID != null && !propertiesDict.ContainsKey(slider.propertyRef.ID))
            {
                propertiesDict.Add(slider.propertyRef.ID, slider);
            }
        }

        #region Properties
        public string WindowTitle
        {
            get
            {
                return wingType.text;
            }
            set
            {
                wingType.text = value;
            }
        }

        #endregion
    }
}
