using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace BytexDigital.Blazor.Components.StateGroups
{
    public class StateElement
    {
        public static StateIndicatorOptionsBuilder Conf()
        {
            return new StateIndicatorOptionsBuilder();
        }

        public class StateIndicatorOptionsBuilder
        {
            public string GroupId { get; private set; }
            public List<string> BusyCssClasses { get; private set; } = new List<string>();
            public List<string> IdleCssClasses { get; private set; } = new List<string>();
            public List<DomAttribute> BusyDomAttributes { get; private set; } = new List<DomAttribute>();
            public List<DomAttribute> IdleDomAttributes { get; private set; } = new List<DomAttribute>();
            public string Role { get; private set; } = "element";
            public bool InheritGroupStateFromParent { get; private set; }
            public string ParentGroupId { get; set; }
            public string Id { get; set; }
            public List<string> RunTriggerOnEvents { get; private set; } = new List<string>();
            public List<string> ExclusiveToTriggerIds { get; private set; } = new List<string>();

            /// <summary>
            /// Serializes the setup data about this element and returns a JSON string representing the settings.
            /// </summary>
            /// <returns></returns>
            public string Compile() => JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            public override string ToString() => Compile();

            /// <summary>
            /// Explicitly sets which group id this element belongs to.
            /// <para>If this method is not called, the element will belong to the nearest state group upwards in the DOM.</para>
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public StateIndicatorOptionsBuilder UseGroupId(string id)
            {
                GroupId = id;

                return this;
            }

            /// <summary>
            /// Marks this element as the root of a state group.
            /// </summary>
            /// <returns></returns>
            public StateIndicatorOptionsBuilder IsGroupRoot()
            {
                Role = "group";

                return this;
            }

            /// <summary>
            /// Enables that this group root element inherits it's status from it's parent group.
            /// <para>If <see cref="UseParentGroupId(string)"/> is not called, the parent group will be the nearest state group upwards in the DOM.</para>
            /// </summary>
            /// <returns></returns>
            public StateIndicatorOptionsBuilder InheritGroupState()
            {
                InheritGroupStateFromParent = true;

                return this;
            }

            /// <summary>
            /// Explicitly sets which state group is the parent group identified by its <paramref name="parentGroupId"/>.
            /// </summary>
            /// <param name="parentGroupId"></param>
            /// <returns></returns>
            public StateIndicatorOptionsBuilder UseParentGroupId(string parentGroupId)
            {
                ParentGroupId = parentGroupId;

                return this;
            }

            /// <summary>
            /// Configures this element to act as a trigger for it's group and fire on the given <paramref name="domEvents"/> .
            /// </summary>
            /// <param name="domEvents">DOM events this trigger will run on.</param>
            /// <returns></returns>
            public StateIndicatorOptionsBuilder TriggerOn(params string[] domEvents)
            {
                RunTriggerOnEvents.AddRange(domEvents);
                Role = "element";

                return this;
            }

            /// <summary>
            /// Assigns an id to this element.
            /// <para>An id does not have to be specified.</para>
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public StateIndicatorOptionsBuilder UseId(StateGroupElementId id)
            {
                Id = id.Id;

                return this;
            }

            /// <summary>
            /// Configures this element to only change when the trigger's <see cref="Id"/> matches any of the given <paramref name="triggerIds"/>.
            /// </summary>
            /// <param name="triggerIds">IDs which this element listens to.</param>
            /// <returns></returns>
            public StateIndicatorOptionsBuilder ListenToId(params StateGroupElementId[] triggerIds)
            {
                ExclusiveToTriggerIds.AddRange(triggerIds.Select(x => x.Id));

                return this;
            }

            /// <summary>
            /// Configures this element to act as a trigger for its group. Will fire on the "click" event.
            /// <para>Shorthand for <see cref="TriggerOn(string[])"/> with parameter "click".</para>
            /// </summary>
            /// <returns></returns>
            public StateIndicatorOptionsBuilder TriggerOnClick()
            {
                return TriggerOn("click");
            }

            /// <summary>
            /// Configures this element to act as a trigger for its group. Will fire on the "change" event.
            /// <para>Shorthand for <see cref="TriggerOn(string[])"/> with parameter "change".</para>
            /// </summary>
            /// <returns></returns>
            public StateIndicatorOptionsBuilder TriggerOnChange()
            {
                return TriggerOn("click");
            }

            /// <summary>
            /// Configures this element to apply the following HTML attributes and values to the DOM element when the group goes into a busy state.
            /// <para>When the group becomes idle/not busy, these attributes will be removed from the DOM element.</para>
            /// </summary>
            /// <param name="attributes">Attributes with values to apply.</param>
            /// <returns></returns>
            public StateIndicatorOptionsBuilder UseWhenBusyAttributes(params DomAttribute[] attributes)
            {
                BusyDomAttributes.AddRange(attributes);

                return this;
            }

            /// <summary>
            /// Configures this element to apply the following HTML attributes and values to the DOM element when the group goes into an idle/not busy state.
            /// <para>When the group becomes busy, these attributes will be removed from the DOM element.</para>
            /// </summary>
            /// <param name="attributes">Attributes with values to apply.</param>
            /// <returns></returns>
            public StateIndicatorOptionsBuilder UseWhenNotBusyAttributes(params DomAttribute[] attributes)
            {
                IdleDomAttributes.AddRange(attributes);

                return this;
            }

            /// <summary>
            /// Configures this element to apply the following CSS classes to the DOM element when the group goes into a busy state.
            /// <para>When the group becomes idle/not busy, these CSS classes will be removed from the DOM element.</para>
            /// </summary>
            /// <param name="classes">CSS classes to apply.</param>
            /// <returns></returns>
            public StateIndicatorOptionsBuilder UseWhenBusyCss(params string[] classes)
            {
                BusyCssClasses.AddRange(classes);

                return this;
            }

            /// <summary>
            /// Configures this element to apply the following CSS classes to the DOM element when the group goes into an idle/not busy state.
            /// <para>When the group becomes busy, these CSS classes will be removed from the DOM element.</para>
            /// </summary>
            /// <param name="attributes">Attributes with values to apply.</param>
            /// <returns></returns>
            public StateIndicatorOptionsBuilder UseWhenNotBusyCss(params string[] classes)
            {
                IdleCssClasses.AddRange(classes);

                return this;
            }

            /// <summary>
            /// Configures this element to apply a CSS class when busy which will hide it from the DOM.
            /// <para>Shorthand for <see cref="UseWhenNotBusyCss(string[])"/> with parameter "stateindicator-hidden".</para>
            /// </summary>
            /// <returns></returns>
            public StateIndicatorOptionsBuilder ShowWhenBusy()
            {
                return UseWhenNotBusyCss("stateindicator-hidden");
            }

            /// <summary>
            /// Configures this element to apply a CSS class when idle/not busy which will hide it from the DOM.
            /// <para>Shorthand for <see cref="UseWhenBusyCss(string[])"/> with parameter "stateindicator-hidden".</para>
            /// </summary>
            /// <returns></returns>
            public StateIndicatorOptionsBuilder ShowWhenNotBusy()
            {
                return UseWhenBusyCss("stateindicator-hidden");
            }

            /// <summary>
            /// Configures this element to apply a CSS class which will disable it when busy. If <paramref name="addDisabledAttribute"/> is set, will also add the "disabled" HTML attribute.
            /// <para>Classes and attributes are removed when not busy/idle.</para>
            /// </summary>
            /// <param name="addDisabledAttribute">Enable usage of the "disabled" HTMl attribute.</param>
            /// <returns></returns>
            public StateIndicatorOptionsBuilder DisableWhenBusy(bool addDisabledAttribute = false)
            {
                if (!addDisabledAttribute)
                {
                    return UseWhenBusyCss("stateindicator-disabled disabled");
                }
                else
                {
                    return UseWhenBusyAttributes(("disabled", "disabled")).UseWhenBusyCss("stateindicator-disabled disabled");
                }
            }

            /// <summary>
            /// Configures this element to apply a CSS class which will disable it when idle/not busy. If <paramref name="addDisabledAttribute"/> is set, will also add the "disabled" HTML attribute.
            /// <para>Classes and attributes are removed when busy.</para>
            /// </summary>
            /// <param name="addDisabledAttribute">Enable usage of the "disabled" HTMl attribute.</param>
            /// <returns></returns>
            public StateIndicatorOptionsBuilder DisableWhenIdle(bool addDisabledAttribute = false)
            {
                if (!addDisabledAttribute)
                {
                    return UseWhenNotBusyCss("stateindicator-disabled disabled");
                }
                else
                {
                    return UseWhenNotBusyAttributes(("disabled", "disabled")).UseWhenNotBusyCss("stateindicator-disabled disabled");
                }
            }

            public class DomAttribute
            {
                public string Key { get; }
                public string Value { get; }

                public DomAttribute(string key, string value)
                {
                    Key = key;
                    Value = value;
                }

                public static implicit operator DomAttribute((string key, string value) x) => new DomAttribute(x.key, x.value);
            }
        }
    }
}
