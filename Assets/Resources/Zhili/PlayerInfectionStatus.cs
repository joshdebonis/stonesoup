using UnityEngine;

public class PlayerInfectionStatus : MonoBehaviour
{
    public enum Infection { None, Red, Green, Purple }

    [SerializeField] private Infection _state = Infection.None;

    public Infection State => _state;

    public void Apply(Infection incoming)
    {
        if (incoming == Infection.None) return;

        // 已经紫了就不用再变
        if (_state == Infection.Purple) return;

        if (_state == Infection.None) _state = incoming;
        else if (_state != incoming) _state = Infection.Purple;
    }

    public void Clear()
    {
        _state = Infection.None;
    }

    public static Color ToColor(Infection s)
    {
        switch (s)
        {
            case Infection.Red: return Color.red;
            case Infection.Green: return Color.green;
            case Infection.Purple: return new Color(0.5f, 0f, 0.5f, 1f);
            default: return Color.white;
        }
    }
}