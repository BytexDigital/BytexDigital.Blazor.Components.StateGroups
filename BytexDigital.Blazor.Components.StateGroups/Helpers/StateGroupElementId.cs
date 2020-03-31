namespace BytexDigital.Blazor.Components.StateGroups
{
    public struct StateGroupElementId
    {
        public string Id { get; }

        public StateGroupElementId(string id)
        {
            Id = id;
        }

        public static implicit operator StateGroupElementId(string id) => new StateGroupElementId(id);
        public static implicit operator StateGroupElementId(int id) => new StateGroupElementId(id.ToString());
    }
}
