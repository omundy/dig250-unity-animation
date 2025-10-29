using UnityEngine;

public class Info : MonoBehaviour
{
    [TextArea(20, 40)] // Min 3 lines, Max 10 lines visible
    public string info;
}
