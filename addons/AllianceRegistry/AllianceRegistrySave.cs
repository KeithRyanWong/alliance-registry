using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class AllianceRegistrySave : Resource
{
    [Export] public Dictionary<string, int> AllyIds { get; set; }
    [Export] public Dictionary<string, Array<string>> Alliances { get; set; }

    public AllianceRegistrySave()
    {
        AllyIds = new Dictionary<string, int>();
        Alliances = new Dictionary<string, Array<string>>();
    }
}