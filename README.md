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

### Elements that are aware of what element triggered the UI to become busy

Sometimes you only want to display an element when a specific other element was the cause of the UI going busy.
An example is 2 buttons next to each other, both being able to do something, for example a "Save" and "Delete" button, but
instead of displaying the spinner in them when any button is pressed, you only want the spinner to show when the button is pressed in which the spinner is.

This can be done by utilizing `UseId(..)` and `ListenToId(..)`. 

```cshtml
<StateGroup @ref="_stateGroup">
    <button @onclick="OnClick"
            class="btn btn-success"
            data-state-options="@StateElement.Conf().TriggerOnClick().DisableWhenBusy().UseId(1).Compile()">

        <span class="spinner-border spinner-border-sm"
              role="status"
              aria-hidden="true"
              data-state-options="@StateElement.Conf().ShowWhenBusy().ListenToId(1).Compile()"></span>

        Save
    </button>

    <button @onclick="OnClick"
            class="btn btn-danger"
            data-state-options="@StateElement.Conf().TriggerOnClick().DisableWhenBusy().UseId(2).Compile()">

        <span class="spinner-border spinner-border-sm"
              role="status"
              aria-hidden="true"
              data-state-options="@StateElement.Conf().ShowWhenBusy().ListenToId(2).Compile()"></span>

        Delete
    </button>
</StateGroup>

@code {
    private StateGroup _stateGroup;

    private bool _cond = false;

    public async Task OnClick()
    {
        await Task.Delay(1000);
        await _stateGroup.SetIdleAsync();
    }
}
```

![](https://static.bytex.digital/github/BytexDigital.Blazor.Components.StateGroups/9f8dffd927.gif)

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