# ConsolePilot Message Bus Adapters

ConsolePilot runtime code depends only on `IConsoleEventDispatcher`.

For a generic bus that implements `IGenericMessageBus`, use:

```csharp
var dispatcher = new GenericMessageBusConsoleDispatcher(myBus);
consolePilot.SetEventDispatcher(dispatcher);
```

For a BitBox-style bus, copy `BitBoxMessageBusConsoleDispatcher.cs.txt` into your game assembly, rename it to `.cs`, and add the correct BitBox `using` statement or namespace reference for your project. It is intentionally a text template so the package does not depend on BitBox game code.
