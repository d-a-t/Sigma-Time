using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletBar : ScreenGUI
{
    // VARIABLES ------------
    public Slider slider;

    // sets max vaalue for the slider and makes sure health starts at full
    public void setMaxAmmo(int ammo)
    {
        slider.maxValue = ammo;
        slider.value = ammo;
    }

    public void setAmmo(int ammo)
    {
        slider.value = ammo;
    }
}
