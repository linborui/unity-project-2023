using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GrassFlowPreprocessor {




#if GRASSFLOW_SRP
    public readonly static bool isSRP = true;
#else
    public readonly static bool isSRP = false;
#endif







}
