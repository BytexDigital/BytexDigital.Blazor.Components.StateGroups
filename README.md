![Nuget](https://img.shields.io/nuget/vpre/BytexDigital.Blazor.Components.StateGroups.svg?style=flat-square)

# BytexDigital.Blazor.Components.StateGroups

This library provides functionality to group elements together and modify them on events to indicate a busy UI when performing work.

The library was specifically designed for server side Blazor and triggers directly on the client when an event occurs
so the UI is adjusted (e.g. disabled) instantly without latency to the server. This allows you to e.g. disable parts of the UI when the user
clicks a button and enable it again remotely from the server once work has been completed.

## Download
[nuget package](https://www.nuget.org/packages/BytexDigital.Blazor.Components.StateGroups/)

## Installation

- Requires jQuery
- \_Host.cshtml

```html
<!-- Place in header -->
<link href="_content/BytexDigital.Blazor.Components.StateGroups/main.css" rel="stylesheet" />

<!-- Place below blazor.server.js -->
<script src="_content/BytexDigital.Blazor.Components.StateGroups/main.js"></script>
```

## Examples with explanations

### Button that disables itself

In this example, a button is placed inside a group. It is configured to act as a group trigger and disable itself when the group is busy.

```cshtml
<StateGroup @ref="_stateGroup">
    <button @onclick="OnClick"
            class="btn btn-primary"
            data-state-options="@StateElement.Conf().TriggerOnClick().DisableWhenBusy().Compile()">
        Click me
    </button>
</StateGroup>

@code {
    private StateGroup _stateGroup;

    public async Task OnClick()
    {
        await Task.Delay(1000);
        await _stateGroup.SetIdleAsync();
    }
}
```

![](https://static.bytex.digital/github/BytexDigital.Blazor.Components.StateGroups/24fd3bd65c.gif)

### Button that disables itself and shows a loading spinner

In this example, a button is placed inside a group whereas the button also displays a loading spinner when busy.

```cshtml
<StateGroup @ref="_stateGroup">
    <button @onclick="OnClick"
            class="btn btn-primary"
            data-state-options="@StateElement.Conf().TriggerOnClick().DisableWhenBusy().Compile()">

        <span
              class="spinner-border spinner-border-sm"
              role="status"
              aria-hidden="true"
              data-state-options="@StateElement.Conf().ShowWhenBusy().Compile()"></span>

        Click me
    </button>
</StateGroup>

@code {
    private StateGroup _stateGroup;

    public async Task OnClick()
    {
        await Task.Delay(1000);
        await _stateGroup.SetIdleAsync();
    }
}
```

![](https://static.bytex.digital/github/BytexDigital.Blazor.Components.StateGroups/b42795e0be.gif)

### Nested groups

Groups can be nested and optionally inherit their state from their parent group.

**Nested groups don't actually have to be nested within the DOM tree**, but if they are, they will automatically connect.
Nested groups that aren't actually nested in the DOM tree need to be manually linked, see `<StateGroup Id="myid">` and `<StateGroup ParentGroupId="myid">`.

```cshtml
<StateGroup @ref="_stateGroup">
    <button @onclick="OnClick"
            class="btn btn-primary"
            data-state-options="@StateElement.Conf().TriggerOnClick().DisableWhenBusy().Compile()">

        <span class="spinner-border spinner-border-sm"
              role="status"
              aria-hidden="true"
              data-state-options="@StateElement.Conf().ShowWhenBusy().Compile()"></span>

        Click me
    </button>

    <StateGroup @ref="_stateGroup2" InheritState="true">
        <p class="mt-4"
           data-state-options="@StateElement.Conf().ShowWhenBusy().Compile()">
            This sub-group is also busy...
        </p>
    </StateGroup>
</StateGroup>

@code {
    private StateGroup _stateGroup;
    private StateGroup _stateGroup2;

    public async Task OnClick()
    {
        await Task.Delay(1000);
        await _stateGroup.SetIdleAsync();
    }
}
```

![](https://static.bytex.digital/github/BytexDigital.Blazor.Components.StateGroups/2d9dba87b9.gif)

### Dynamically added elements

This example shows that also dynamically added elements/groups will display as expected.

```cshtml
<StateGroup @ref="_stateGroup">
    <button @onclick="OnClick"
            class="btn btn-primary"
            data-state-options="@StateElement.Conf().TriggerOnClick().DisableWhenBusy().Compile()">

        <span class="spinner-border spinner-border-sm"
              role="status"
              aria-hidden="true"
              data-state-options="@StateElement.Conf().ShowWhenBusy().Compile()"></span>

        Click me
    </button>

    @if (_cond)
    {
        <StateGroup @ref="_stateGroup2" InheritState="true">
            <p class="mt-4"
               data-state-options="@StateElement.Conf().ShowWhenBusy().Compile()">
                This sub-group appears and is instantly busy!
            </p>
        </StateGroup>
    }
</StateGroup>

@code {
    private StateGroup _stateGroup;
    private StateGroup _stateGroup2;

    private bool _cond = false;

    public async Task OnClick()
    {
        await Task.Delay(1000);

        _cond = true;
        StateHasChanged();

        await Task.Delay(1000);
        await _stateGroup.SetIdleAsync();
    }
}
```

![](https://static.bytex.digital/github/BytexDigital.Blazor.Components.StateGroups/c0c1cd706f.gif)