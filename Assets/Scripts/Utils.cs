using System.Linq;
using UnityEngine;

public static class Utils
{
    public static float BiDirectionalNormalizedClamp(this float value)
    {
        return Mathf.Clamp(value, - 1f, 1f);
    }
    
    public static float NormalizedClamp(this float value)
    {
        return Mathf.Clamp(value, 0, 1f);
    }
    
    public static bool IsFrontWheel(this WheelCollider wheel, Transform centerOfMass)
    {
        Vector3 directionToWheel = wheel.transform.position - centerOfMass.position;
        
        return Vector3.Angle(centerOfMass.forward, directionToWheel) < 90f;
    }
    
    public static bool IsRightWheel(this WheelCollider wheel, Transform centerOfMass)
    {
        Vector3 directionToWheel = wheel.transform.position - centerOfMass.position;
        
        return Vector3.SignedAngle(centerOfMass.forward, directionToWheel, centerOfMass.up) < 0f;
    }
    
    public static WheelCollider[] GetFrontWheels(this WheelCollider[] wheels, Transform centerOfMass)
    {
        return wheels.Where(wheel => wheel.IsFrontWheel(centerOfMass)).ToArray();
    }
    
    public static WheelCollider[] GetBackWheels(this WheelCollider[] wheels, Transform centerOfMass)
    {
        return wheels.Where(wheel => !wheel.IsFrontWheel(centerOfMass)).ToArray();
    }
}
