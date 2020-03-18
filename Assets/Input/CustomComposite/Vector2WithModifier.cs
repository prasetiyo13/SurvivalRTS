using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[InitializeOnLoad]
#endif

[DisplayStringFormat("{modifier}+{vector2}")]
public class Vector2WithModifier : InputBindingComposite<Vector2>
{
#if UNITY_EDITOR
    static Vector2WithModifier() => Initialize();
#endif

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize() => InputSystem.RegisterBindingComposite<Vector2WithModifier>("2DVectorWithModifier");

    [InputControl(layout = "Button")]
    public int modifier;

    [InputControl(layout = "Vector2")]
    public int vector2;

    public override Vector2 ReadValue(ref InputBindingCompositeContext context)
    {
        if (context.ReadValueAsButton(modifier))
        {
            return context.ReadValue<Vector2, Vector2MagnitudeComparer>(vector2);
        }
        return default;
    }
}
