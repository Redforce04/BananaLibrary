yo yo yo welcome to my wacky plugin

# Banana Library
An all-in-one Library for SCP-SL Community Server Plugins.

### Some helpful features:
- Banana Servers - Define "servers" and enable automatic feature rollout to specific servers.
- Banana Roles - Define "roles" which allow you to quickly control access to commands \[wip\].
- Banana Feature - Quick and simple features can be individually configured, will automatically subscribe and unsubscribe to BananaEvents when the feature is enabled/disabled, and can even contain configurable properties.
- Banana Events - A quick, clean, and simple solution to subscribing to events when features are enabled.
- Banana Commands - A super clean one-method solution to creating fully capable parent and normal commands. The annoying permissions checks are no longer a hassle. \[WIP\]
- BPLogger - An advanced logger that shows the feature & methods which logs stem from. (And it contains the colorful look from NW Plugin API).



### BananaServer Intro:
Use classes to outline each server. These are auto-instantiated and registered per plugin by the BananaLibrary.
```csharp
/// <summary>
/// The testing server
/// </summary>
public sealed class TestNet : BananaServer
{
    /// <inheritdoc />
    public override ushort ServerPort => 7777;

    /// <inheritdoc />
    public override string ServerName => "Testing Net";

    /// <inheritdoc />
    public override string ServerId => "TestNet";
}
```

### BananaRoles
Still a wip. Being reworked so it is not just an additional overly-complicated SL-Role manager, but rather a powerful yet simple Permissions manager. 

### BananaFeature

```csharp
/// <summary>
/// Makes the radio battery infinite.
/// </summary>
[DisableOnServer<Net1>] // Disable features on a server with one simple attribute.
[DisableOnServer<Net2>]
internal sealed class AdjustableRadioBatteryDrainFeature : BananaFeature
{
    /// <summary>
    /// Gets or sets a value indicating the multiplier to apply to radio power drain. If set to 0 radios will have no drain.
    /// </summary>
    // Configs simplified. BananaManager automatically creates a BananaFeature config files with all BananaConfigs. Configs are loaded with the features.
    // It has never been so easy to specify config defaults for servers.
    [BananaConfig]
    [BananaConfigDefault<Net4>(.3f)]
    [BananaConfigDefault<Net5>(.75f)]
    [Description("Indicates the multiplier to apply to radio power drain. If set to 0 radios will have no drain.")]
    public float RadioDrainMultiplier { get; set; } = 0f;

    /// <inheritdoc/>
    public override string Name => nameof(AdjustableRadioBatteryDrainFeature);

    /// <inheritdoc/>
    protected override void Enable()
    {
    }

    /// <inheritdoc/>
    protected override void Disable()
    {
    }

    // Auto-Subscribes after Enabled() is called, and Auto-Unsubscribes after Disabled() is called.
    // Automatically detects EventArgs or has manual overrides for specifying which LabApiEventHandler to subscribe to.
    [BananaEvent]
    private void OnUsingRadio(PlayerUsingRadioEventArgs ev)
    {
        ev.Drain *= RadioDrainMultiplier;
    }
}
```
