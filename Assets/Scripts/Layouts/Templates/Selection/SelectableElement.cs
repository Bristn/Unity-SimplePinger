using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SelectableElement 
{
    public bool Selected { get; set; }

    public bool ShowIndicator { get; set; }
}
