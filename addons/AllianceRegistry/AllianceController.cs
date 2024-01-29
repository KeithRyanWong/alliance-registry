using Godot;
using Godot.Collections;

// Controls the registration and checking of alliances
// Will be a singleton
// Is this different from the registry? not realllllyy,
// it's just now meant to be accessed by a system, but maybe it doesn't need to be autoloaded, so I can attach it to the main trree
[Tool]
public partial class AllianceController : Node
{
    
    public static AllianceController Instance { get; private set; }
    [Signal]
    public delegate void AllyIdAddedEventHandler();
    
    
    public Dictionary<string, int> AllyIds
    {
        get => _allyIds.Duplicate();
        private set => _allyIds = value;
    }
    public Dictionary<string, Array<string>> Alliances
    {
        get => _alliances.Duplicate();
        private set => _alliances = value;
    }

    private Dictionary<string, int> _allyIds;
    private Dictionary<string, Array<string>> _alliances;

    public override void _EnterTree()
    {
        base._EnterTree();
        Instance = this;
        AllyIds = new Dictionary<string, int>();
        Alliances = new Dictionary<string, Array<string>>();
    }

    // Lets hope since this is a singleton that this persists between sessions. It won't sigh, because we unload it ourselves
    // public void InitializeTeams()
    // {
    //     
    // }
    // register a team
    // Register an alliance
    // destroy an alliance
    public void RegisterAllyId(string allyId) 
    {
        if(AllyIds.ContainsKey(allyId.ToLower())) return;
        _allyIds.Add(allyId.ToLower(), AllyIds.Count);
        GD.Print($"AllyIds contains {allyId}: ", AllyIds.ContainsKey(allyId.ToLower()));
        EmitSignal(SignalName.AllyIdAdded);
    }

    public void RegisterAlliance(string requesterId, string allyId)
    {
        if (requesterId == allyId) return;
        if (!AllyIds.ContainsKey(requesterId) || !AllyIds.ContainsKey(allyId)) return;
        
        if (!Alliances.ContainsKey(requesterId)) _alliances.Add(requesterId, new Array<string>());
        if (!Alliances.ContainsKey(allyId)) _alliances.Add(allyId, new Array<string>());
        
        if(!Alliances[requesterId].Contains(allyId)) _alliances[requesterId].Add(allyId);
        if(!Alliances[allyId].Contains(requesterId)) _alliances[allyId].Add(requesterId);
    }
    public void DeregisterAlliance(string requesterId, string allyId)
    {
        if (!AllyIds.ContainsKey(requesterId) || !AllyIds.ContainsKey(allyId)) return;
        
        if (!Alliances.ContainsKey(requesterId)) _alliances.Add(requesterId, new Array<string>());
        if (!Alliances.ContainsKey(allyId)) _alliances.Add(allyId, new Array<string>());
        
        if(Alliances[requesterId].Contains(allyId)) _alliances[requesterId].Remove(allyId);
        if(Alliances[allyId].Contains(requesterId)) _alliances[allyId].Remove(requesterId);
    }
    
    // ALlow teams to check for ally
    public bool AreAllied(AllianceComponent requester, GodotObject obj)
    {
        if (obj is Node node && node.HasNode("AllianceComponent"))
        {
            AllianceComponent ally2 = node.GetNode<AllianceComponent>("AllianceComponent");
            return requester.AllyId == ally2.AllyId || Alliances[requester.AllyId].Contains(ally2.AllyId);
        }

        return false;
    }


    public void Debug()
    {
        foreach (var alliance in Alliances)
        {
            var alliances = new string[alliance.Value.Count];
            for (var i = 0; i < alliances.Length; i++)
            {
                alliances[i] = alliance.Value[i];
            }
            
            var line = $"{alliance.Key} is allied with {alliances.Join(", ")}.";
            GD.Print(line);
        }
    }
}
