using Godot;
using Godot.Collections;

// Controls the registration and checking of alliances
// Will be a singleton
// Is this different from the registry? not realllllyy,
// it's just now meant to be accessed by a system, but maybe it doesn't need to be autoloaded, so I can attach it to the main trree
[Tool]
public partial class AllianceController : Node
{
    private const string _savefilePath = "res://addons/AllianceRegistry/AllianceRegistrySave.tres";
    public static AllianceController Instance { get; private set; }
    [Signal]
    public delegate void AllyIdAddedEventHandler();
    
    
    public Dictionary<string, int> AllyIds => _allyIds.Duplicate();

    public Dictionary<string, Array<string>> Alliances => _alliances.Duplicate();

    private Dictionary<string, int> _allyIds;
    private Dictionary<string, Array<string>> _alliances;

    public override void _EnterTree()
    {
        base._EnterTree();
        Instance = this;

        LoadData();
        _allyIds ??= new Dictionary<string, int>();
        _alliances ??= new Dictionary<string, Array<string>>();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        SaveData();
    }

    private void SaveData()
    {
        var newSaveFile = new AllianceRegistrySave();
        newSaveFile.Alliances = _alliances ?? new Dictionary<string, Array<string>>();
        newSaveFile.AllyIds = _allyIds ?? new Dictionary<string, int>();
        ResourceSaver.Save(newSaveFile, _savefilePath);
    }

    private void LoadData()
    {
        if (!ResourceLoader.Exists(_savefilePath))
        {
            var newSaveFile = new AllianceRegistrySave();
            ResourceSaver.Save(newSaveFile, _savefilePath);
        }

        var saveData = ResourceLoader.Load<AllianceRegistrySave>(_savefilePath);
        if (saveData is not null)
        {
            _allyIds = saveData.AllyIds ?? new Dictionary<string, int>();
            _alliances = saveData.Alliances ?? new Dictionary<string, Array<string>>();
        }
        else
        {
            GD.PrintErr("Failed to load AllianceRegistrySave from: ", _savefilePath);
            _allyIds = new Dictionary<string, int>();
            _alliances = new Dictionary<string, Array<string>>();
        }
    }
    
    
    public void RegisterAllyId(string allyId)
    {
        string sanitizedId = allyId.ToLower().Split(' ').Join("");
        if(AllyIds.ContainsKey(sanitizedId)) return;
        _allyIds.Add(sanitizedId, AllyIds.Count);
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
