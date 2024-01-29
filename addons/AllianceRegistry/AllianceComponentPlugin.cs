#if TOOLS
using Godot;

[Tool]
public partial class AllianceComponentPlugin : EditorInspectorPlugin
{
    public override bool _CanHandle(GodotObject @object)
    {
        return @object is AllianceComponent;
    }

    public override void _ParseBegin(GodotObject @object)
    {
        base._ParseBegin(@object);
        
        var allianceComponent = (AllianceComponent)@object;
        
        if (!string.IsNullOrWhiteSpace(allianceComponent.AllyId))
        {
            allianceComponent.AllyId = allianceComponent.AllyId.ToLower();
        }

        var panelScene = GD.Load<PackedScene>("res://addons/AllianceRegistry/AllianceComponentPanel.tscn");
        var panelInstance = (AllianceComponentPanel)panelScene.Instantiate();
        panelInstance.AllianceComponent = allianceComponent;
        AddCustomControl(panelInstance);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}
#endif