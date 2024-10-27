using System;
using UnityEngine;

public delegate void DrawDelegate();

public class Drawer : MonoBehaviour
{
    public static Drawer Instance { get; private set; }

    private void Awake()
    {
		if (Instance != null)
		{
            throw new Exception($"Another instance of {nameof(Drawer)} already exists.");
		}
        
        Instance = this;
    }

    public event DrawDelegate OnDraw;
    
    public event DrawDelegate OnDrawEditMode;

    private void OnDrawGizmos()
    {
	    if (Application.isPlaying)
	    {
		    OnDraw?.Invoke();
	    }
	    
	    OnDrawEditMode?.Invoke();
    }

    private void OnDestroy()
    {
	    RemoveListeners(OnDraw);
	    
	    RemoveListeners(OnDrawEditMode);
    }

    private void RemoveListeners(DrawDelegate drawEvent)
    {
	    if (drawEvent != null)
	    {
		    foreach (Delegate drawDelegate in drawEvent.GetInvocationList())
		    {
			    drawEvent -= (DrawDelegate) drawDelegate;
		    }
	    }
    }
}
