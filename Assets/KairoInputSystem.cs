// GENERATED AUTOMATICALLY FROM 'Assets/KairoInputSystem.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @KairoInputSystem : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @KairoInputSystem()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""KairoInputSystem"",
    ""maps"": [
        {
            ""name"": ""Mouse"",
            ""id"": ""8e284373-3ec2-4d7a-b54a-c1cd536e3c9e"",
            ""actions"": [
                {
                    ""name"": ""New action"",
                    ""type"": ""Button"",
                    ""id"": ""57a14d24-3576-4896-9be6-cba4ae4c0840"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""d6c9e248-c25c-402e-97d3-fa318fbf8c6b"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Touch"",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""KeyBoard"",
            ""id"": ""6420e473-057f-4f99-9f94-c5ff1ce05083"",
            ""actions"": [
                {
                    ""name"": ""New action"",
                    ""type"": ""Button"",
                    ""id"": ""a7ed5dec-c780-4b5e-8242-f3bc59b6407b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""e29b4f5b-7468-43bf-9648-62a60ae479e7"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse"",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Touch"",
            ""id"": ""41293204-1089-4a39-821b-8cfd4639dac4"",
            ""actions"": [
                {
                    ""name"": ""New action"",
                    ""type"": ""Button"",
                    ""id"": ""567c8dc4-dbaf-4cd6-8cc6-d1fa1033f667"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""861af0a6-294c-4c36-b1d1-161a6a991fac"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse"",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Mouse"",
            ""bindingGroup"": ""Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""KeyBoard"",
            ""bindingGroup"": ""KeyBoard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Touch"",
            ""bindingGroup"": ""Touch"",
            ""devices"": [
                {
                    ""devicePath"": ""<Touchscreen>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Mouse
        m_Mouse = asset.FindActionMap("Mouse", throwIfNotFound: true);
        m_Mouse_Newaction = m_Mouse.FindAction("New action", throwIfNotFound: true);
        // KeyBoard
        m_KeyBoard = asset.FindActionMap("KeyBoard", throwIfNotFound: true);
        m_KeyBoard_Newaction = m_KeyBoard.FindAction("New action", throwIfNotFound: true);
        // Touch
        m_Touch = asset.FindActionMap("Touch", throwIfNotFound: true);
        m_Touch_Newaction = m_Touch.FindAction("New action", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Mouse
    private readonly InputActionMap m_Mouse;
    private IMouseActions m_MouseActionsCallbackInterface;
    private readonly InputAction m_Mouse_Newaction;
    public struct MouseActions
    {
        private @KairoInputSystem m_Wrapper;
        public MouseActions(@KairoInputSystem wrapper) { m_Wrapper = wrapper; }
        public InputAction @Newaction => m_Wrapper.m_Mouse_Newaction;
        public InputActionMap Get() { return m_Wrapper.m_Mouse; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MouseActions set) { return set.Get(); }
        public void SetCallbacks(IMouseActions instance)
        {
            if (m_Wrapper.m_MouseActionsCallbackInterface != null)
            {
                @Newaction.started -= m_Wrapper.m_MouseActionsCallbackInterface.OnNewaction;
                @Newaction.performed -= m_Wrapper.m_MouseActionsCallbackInterface.OnNewaction;
                @Newaction.canceled -= m_Wrapper.m_MouseActionsCallbackInterface.OnNewaction;
            }
            m_Wrapper.m_MouseActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Newaction.started += instance.OnNewaction;
                @Newaction.performed += instance.OnNewaction;
                @Newaction.canceled += instance.OnNewaction;
            }
        }
    }
    public MouseActions @Mouse => new MouseActions(this);

    // KeyBoard
    private readonly InputActionMap m_KeyBoard;
    private IKeyBoardActions m_KeyBoardActionsCallbackInterface;
    private readonly InputAction m_KeyBoard_Newaction;
    public struct KeyBoardActions
    {
        private @KairoInputSystem m_Wrapper;
        public KeyBoardActions(@KairoInputSystem wrapper) { m_Wrapper = wrapper; }
        public InputAction @Newaction => m_Wrapper.m_KeyBoard_Newaction;
        public InputActionMap Get() { return m_Wrapper.m_KeyBoard; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(KeyBoardActions set) { return set.Get(); }
        public void SetCallbacks(IKeyBoardActions instance)
        {
            if (m_Wrapper.m_KeyBoardActionsCallbackInterface != null)
            {
                @Newaction.started -= m_Wrapper.m_KeyBoardActionsCallbackInterface.OnNewaction;
                @Newaction.performed -= m_Wrapper.m_KeyBoardActionsCallbackInterface.OnNewaction;
                @Newaction.canceled -= m_Wrapper.m_KeyBoardActionsCallbackInterface.OnNewaction;
            }
            m_Wrapper.m_KeyBoardActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Newaction.started += instance.OnNewaction;
                @Newaction.performed += instance.OnNewaction;
                @Newaction.canceled += instance.OnNewaction;
            }
        }
    }
    public KeyBoardActions @KeyBoard => new KeyBoardActions(this);

    // Touch
    private readonly InputActionMap m_Touch;
    private ITouchActions m_TouchActionsCallbackInterface;
    private readonly InputAction m_Touch_Newaction;
    public struct TouchActions
    {
        private @KairoInputSystem m_Wrapper;
        public TouchActions(@KairoInputSystem wrapper) { m_Wrapper = wrapper; }
        public InputAction @Newaction => m_Wrapper.m_Touch_Newaction;
        public InputActionMap Get() { return m_Wrapper.m_Touch; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(TouchActions set) { return set.Get(); }
        public void SetCallbacks(ITouchActions instance)
        {
            if (m_Wrapper.m_TouchActionsCallbackInterface != null)
            {
                @Newaction.started -= m_Wrapper.m_TouchActionsCallbackInterface.OnNewaction;
                @Newaction.performed -= m_Wrapper.m_TouchActionsCallbackInterface.OnNewaction;
                @Newaction.canceled -= m_Wrapper.m_TouchActionsCallbackInterface.OnNewaction;
            }
            m_Wrapper.m_TouchActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Newaction.started += instance.OnNewaction;
                @Newaction.performed += instance.OnNewaction;
                @Newaction.canceled += instance.OnNewaction;
            }
        }
    }
    public TouchActions @Touch => new TouchActions(this);
    private int m_MouseSchemeIndex = -1;
    public InputControlScheme MouseScheme
    {
        get
        {
            if (m_MouseSchemeIndex == -1) m_MouseSchemeIndex = asset.FindControlSchemeIndex("Mouse");
            return asset.controlSchemes[m_MouseSchemeIndex];
        }
    }
    private int m_KeyBoardSchemeIndex = -1;
    public InputControlScheme KeyBoardScheme
    {
        get
        {
            if (m_KeyBoardSchemeIndex == -1) m_KeyBoardSchemeIndex = asset.FindControlSchemeIndex("KeyBoard");
            return asset.controlSchemes[m_KeyBoardSchemeIndex];
        }
    }
    private int m_TouchSchemeIndex = -1;
    public InputControlScheme TouchScheme
    {
        get
        {
            if (m_TouchSchemeIndex == -1) m_TouchSchemeIndex = asset.FindControlSchemeIndex("Touch");
            return asset.controlSchemes[m_TouchSchemeIndex];
        }
    }
    public interface IMouseActions
    {
        void OnNewaction(InputAction.CallbackContext context);
    }
    public interface IKeyBoardActions
    {
        void OnNewaction(InputAction.CallbackContext context);
    }
    public interface ITouchActions
    {
        void OnNewaction(InputAction.CallbackContext context);
    }
}
