using UnityEngine;

public class MobileInput : MonoBehaviour
{
    // trzymane przez przyciski / auto-drive
    public static bool GasHeld = false;
    public static bool BrakeHeld = false;

    // -1 = lewo, 0 = prosto, 1 = prawo
    public static float SteerAxis = 0f;

    // Jeœli kiedyœ bêdziesz u¿ywaæ osobnych przycisków gaz/hamulec:
    public void GasDown() => GasHeld = true;
    public void GasUp() => GasHeld = false;
    public void BrakeDown() => BrakeHeld = true;
    public void BrakeUp() => BrakeHeld = false;
}
