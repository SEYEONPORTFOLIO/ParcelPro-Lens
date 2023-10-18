// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Pnc.Model.Hands;
using UnityEngine;

namespace Pnc.UI.Button
{
    ///<summary>
    /// This class exists to route <see cref="Microsoft.MixedReality.Toolkit.UI.PressableButton"/> events through to <see cref="Microsoft.MixedReality.Toolkit.UI.Interactable"/>.
    /// The result is being able to have physical touch call Interactable.OnPointerClicked.
    ///</summary>
    [AddComponentMenu("MetaCore/Scripts/UI/Button/PhysicalPressEventRouter")]
    public class PhysicalPressEventRouter : MonoBehaviour
    {
        [Tooltip("Interactable to which the press events are being routed. Defaults to the object of the component.")]
        public Interactable routingTarget;

        public GameObject frontPlate;
        public GameObject outline;

        /// Enum specifying which button event causes a Click to be raised.
        public enum PhysicalPressEventBehavior
        {
            EventOnClickCompletion = 0,
            EventOnPress,
            EventOnTouch
        }
        public PhysicalPressEventBehavior InteractableOnClick = PhysicalPressEventBehavior.EventOnClickCompletion;

        private void Awake()
        {
            Debug.Log($"PhysicalPressEventRouter Awake");
            if (routingTarget == null)
            {
                Debug.Log($"PhysicalPressEventRouter Awake GetComponent<Interactable>");
                routingTarget = GetComponent<Interactable>();
            }
            
            if(frontPlate == null)
            {
                frontPlate = this.gameObject.transform.Find("FrontPlate").gameObject;
            }
            if(outline == null)
            {
                outline = frontPlate.transform.Find("Outline").gameObject;
            }
        }

        private bool CanRouteInput()
        {
            if(routingTarget == null)
            {
                Debug.Log($"PhysicalPressEventRouter CanRouteInput routingTarget is null");
            }
            else
            {
                Debug.Log($"PhysicalPressEventRouter CanRouteInput routingTarget.IsEnabled={routingTarget.IsEnabled}");                
            }
            return routingTarget != null && routingTarget.IsEnabled;
        }

        /// <summary>
        /// Gets called when the TouchBegin event is invoked within the default PressableButton and 
        /// PressableButtonHoloLens2 components. When the physical touch with a 
        /// hand has begun, set physical touch state within Interactable. 
        /// </summary>
        public void OnHandPressTouched()
        {
            Debug.Log($"PhysicalPressEventRouter OnHandPressTouched Start");

            // Material matTouched = Resources.Load<Material>("Materials/WireFrame");
            // frontPlate.GetComponent<Renderer>().material = matTouched;

            SetOutline(true);

            // if (CanRouteInput())
            // {
            //     Debug.Log($"PhysicalPressEventRouter OnHandPressTouched routingTarget.HasPhysicalTouch={routingTarget.HasPhysicalTouch}");
            //     routingTarget.HasPhysicalTouch = true;
            //     if (InteractableOnClick == PhysicalPressEventBehavior.EventOnTouch)
            //     {
            //         routingTarget.HasPress = true;
            //         routingTarget.TriggerOnClick();
            //         routingTarget.HasPress = false;
            //         Debug.Log($"PhysicalPressEventRouter OnHandPressTouched TriggerOnClick");
            //     }
            // }
        }

        /// <summary>
        /// Gets called when the TouchEnd event is invoked within the default PressableButton and 
        /// PressableButtonHoloLens2 components. Once the physical touch with a hand is removed, set
        /// the physical touch and possibly press state within Interactable.
        /// </summary>
        public void OnHandPressUntouched()
        {
            Debug.Log($"PhysicalPressEventRouter OnHandPressUntouched Start");
            
            // Material matUntouched = Resources.Load<Material>("Materials/PlateTransparent");
            // frontPlate.GetComponent<Renderer>().material = matUntouched;
            SetOutline(false);

            // if (CanRouteInput())
            // {
            //     Debug.Log($"PhysicalPressEventRouter OnHandPressUntouched routingTarget.HasPhysicalTouch={routingTarget.HasPhysicalTouch}");
            //     routingTarget.HasPhysicalTouch = false;
            //     if (InteractableOnClick == PhysicalPressEventBehavior.EventOnTouch)
            //     {
            //     Debug.Log($"PhysicalPressEventRouter OnHandPressUntouched routingTarget.HasPress={routingTarget.HasPress}");
            //         routingTarget.HasPress = true;
            //     }
            // }
        }

        /// <summary>
        /// Gets called when the ButtonPressed event is invoked within the default PressableButton and 
        /// PressableButtonHoloLens2 components. When the physical press with a hand is triggered, set 
        /// the physical touch and press state within Interactable. 
        /// </summary>
        public void OnHandPressTriggered()
        {
            Debug.Log($"PhysicalPressEventRouter OnHandPressTriggered");
            if (CanRouteInput())
            {
                routingTarget.HasPhysicalTouch = true;
                routingTarget.HasPress = true;
                if (InteractableOnClick == PhysicalPressEventBehavior.EventOnPress)
                {
                    routingTarget.TriggerOnClick();
                }
            }
        }

        /// <summary>
        /// Gets called when the ButtonReleased event is invoked within the default PressableButton and 
        /// PressableButtonHoloLens2 components.  Once the physical press with a hand is completed, set
        /// the press and physical touch states within Interactable
        /// </summary>
        public void OnHandPressCompleted()
        {
            Debug.Log($"PhysicalPressEventRouter OnHandPressCompleted");
            if (CanRouteInput())
            {
                routingTarget.HasPhysicalTouch = true;
                routingTarget.HasPress = true;
                if (InteractableOnClick == PhysicalPressEventBehavior.EventOnClickCompletion)
                {
                    routingTarget.TriggerOnClick();
                }
                routingTarget.HasPress = false;
                routingTarget.HasPhysicalTouch = false;
            }
        }

        void SetOutline(bool isOn=true)
        {
            string strLineName = "";
            Material mat;
            
            Debug.Log($"PhysicalPressEventRouter SetOutline isOn:{isOn}");

            if(isOn)
                mat = Resources.Load<Material>("Materials/Outline");
            else
                mat = Resources.Load<Material>("Materials/PlateTransparent");

            for(int i=1; i<13; i++)
            {
                strLineName = "line" + i.ToString();
                GameObject goLine = outline.transform.Find(strLineName).gameObject;

                if(!goLine)
                {
                    Debug.Log($"PhysicalPressEventRouter SetOutline goLine is null LineName:{strLineName}");
                    continue;
                }

                goLine.GetComponent<MeshRenderer>().material = mat;
            }
        }
    }
}