window.StateIndicators = {
    StateMemory: {},

    SetupElementsIn: function (element) {
        StateIndicators.SetupElementFromOptions($(element));

        // Setup all children
        $(element).find("[data-state-options]:not([data-state-ready])").each(function (i, e) {
            StateIndicators.SetupElementFromOptions(e);
        });
    },

    SetupAllElements: function () {
        $("[data-state-options]").each(function (i, e) {
            StateIndicators.SetupElementFromOptions(e);
        });
    },

    SetupElementFromOptions: function (element) {
        // Already setup?
        if (["true", "waitingforgroup"].includes($(element).attr("data-state-ready"))) return;

        // Fetch options object
        let optionsJson = $(element).attr("data-state-options");

        // Return if no options specified
        if (!optionsJson) return;

        let options = JSON.parse(optionsJson);

        // Setup element's group roots first
        if (options.role != "group") {
            let groupInfo = StateIndicators.FindElementGroupDomElement(element);

            if (groupInfo == null) {
                return;
            }

            options.groupId = groupInfo.groupId;
        }

        if (options.role == "element") {
            StateIndicators.RegisterElement(element, options);
        } else if (options.role == "group") {
            if (!options.groupId) {
                options.groupId = StateIndicators.GenerateId();
            }

            StateIndicators.RegisterGroup(element, options.groupId);
        }
    },

    ClearUnusedKeysFromMemory: function () {
        let keys = Object.keys(StateIndicators.StateMemory);
        let unusedKeys = keys.filter(uid => $("[data-state-uid='" + uid + "']").length == 0);

        unusedKeys.forEach(uid => {
            if ($("[data-state-uid='" + uid + "']").length == 0) {
                delete StateIndicators.StateMemory[uid];
            }
        });
    },

    FindElementGroupDomElement: function (element) {
        // Fetch options object
        let optionsJson = $(element).attr("data-state-options");

        // Return if no options specified
        if (!optionsJson) return null;

        let options = JSON.parse(optionsJson);

        // Only run for elements
        if (options.role == "group") return null;

        let parentGroup = null;

        if (!options.groupId) {
            parentGroup = $(element)
                .parents("[data-state-options]")
                .filter(function (i, e) {
                    let o = JSON.parse($(e).attr("data-state-options"));

                    return o && o.role == "group";
                })
                .first();

            if (parentGroup.length == 0) {
                throw "Cannot find elements group because no id has been specified and no group could be found looking upwards in the DOM tree.";
                return null;
            }
        } else {
            parentGroup = $("[data-state-options]")
                .filter(function (i, e) {
                    let o = JSON.parse($(e).attr("data-state-options"));

                    return o && o.role == "group" && o.groupId == options.groupId;
                })
                .first();

            if (parentGroup.length == 0) {
                // The group does not exist yet, but it might be added to the DOM later.
                // Mark this as unfinished and hope for the best later on.
                let uniqueId = StateIndicators.GenerateId();

                $(element)
                    .attr("data-state-uid", uniqueId)
                    .attr("data-state-ready", "waitingforgroup")
                    .attr("data-state-role", "element")
                    .attr("data-state-groupid", options.groupId);

                return null;
            }
        }

        if (parentGroup.attr("data-state-groupid") === undefined) {
            StateIndicators.SetupElementFromOptions(parentGroup.get(0));
        }

        return {
            "groupId": parentGroup.attr("data-state-groupid"),
            "groupElement": parentGroup.get(0)
        };
    },

    RegisterElement: function (element, options) {
        let uniqueId = StateIndicators.GenerateId();

        StateIndicators.StateMemory[uniqueId] = 0;

        $(element)
            .attr("data-state-uid", uniqueId)
            .attr("data-state-ready", "true")
            .attr("data-state-role", "element")
            .attr("data-state-groupid", options.groupId)
            .on("state-group-busy", StateIndicators.OnElementEnable)
            .on("state-group-idle", StateIndicators.OnElementDisable);

        // Remove all old keys
        StateIndicators.ClearUnusedKeysFromMemory();

        // Attach requested handlers to element
        if (options.runTriggerOnEvents) {
            $(element).on(options.runTriggerOnEvents.join(" "), function () {
                StateIndicators.EnableGroup(element, $(element).attr("data-state-groupid"));
            });
        }

        // Since this control might be added during the group being busy, we should check the groups status and run the handlers accordingly
        let parentGroup = $("[data-state-role='group'][data-state-groupid='" + options.groupId + "']")
            .first();

        if (parentGroup.length == 0) {
            throw "An element cannot be setup because the group control with id " + options.groupId + " cannot be found.";
        }

        if (parentGroup.attr("data-state-group-status") === "true") {
            $(element).trigger("state-group-busy", [null, options.groupId]);
        } else {
            $(element).trigger("state-group-idle", [options.groupId]);
        }
    },

    RegisterGroup: function (element, groupId) {
        let uniqueId = StateIndicators.GenerateId();
        StateIndicators.StateMemory[uniqueId] = 0;

        $(element)
            .attr("data-state-uid", uniqueId)
            .attr("data-state-ready", "true")
            .attr("data-state-role", "group")
            .attr("data-state-groupid", groupId)
            .attr("data-state-group-status", false)
            .on("state-group-busy", StateIndicators.OnElementEnable)
            .on("state-group-idle", StateIndicators.OnElementDisable);

        // Find elements waiting for this group and initialize them
        $("[data-state-ready='waitingforgroup'][data-state-groupid='" + groupId + "']").each((i, e) => {
            StateIndicators.SetupElementFromOptions(e);
        });

        let options = JSON.parse($(element).attr("data-state-options"));

        // Check if we should inherit and directly do a check whether we need to enable/disable this new group
        if (options.inheritGroupStateFromParent) {
            let parent = null;

            if (options.parentGroupId) {
                $(element).attr("data-state-parentgroupid", options.parentGroupId);

                // Check if we can find the parent with the ID
                parent = $("[data-state-role='group'][data-state-groupid='" + options.parentGroupId + "']").first();
            } else {
                parent = $(element).parents("[data-state-role='group']").first();
            }

            if (parent.length > 0 && parent.attr("data-state-group-status") == "true") {
                StateIndicators.EnableGroup(null, groupId);
            } else {
                StateIndicators.DisableGroup(groupId);
            }
        }
    },

    EnableGroup: function (sender, groupId) {
        $("[data-state-groupid='" + groupId + "']").trigger("state-group-busy", [sender, groupId]);
    },

    DisableGroup: function (groupId) {
        $("[data-state-groupid='" + groupId + "']").trigger("state-group-idle", [groupId]);
    },

    OnElementEnable: function (e, sender, groupId) {
        e.stopPropagation();

        let self = $(this);
        let options = JSON.parse(self.attr("data-state-options"));
        let uid = self.attr("data-state-uid");

        // Group specific code on activation
        if (self.attr("data-state-role") == "group") {
            if (StateIndicators.StateMemory[uid] == 0) {
                $("[data-state-groupid='" + groupId + "'][data-state-role='group']")
                    .attr("data-state-group-status", "true");
            }

            StateIndicators.StateMemory[uid] = StateIndicators.StateMemory[uid] + 1;

            // Find possible child groups to triggered if required
            let childGroup = self.find("[data-state-role='group'][data-state-options]").first();
            let otherGroups = $("[data-state-role='group'][data-state-options][data-state-parentgroupid='" + groupId + "']");

            childGroup = childGroup.add(otherGroups);

            childGroup.each((i, e) => {
                let childOptions = JSON.parse($(e).attr("data-state-options"));

                if (!childOptions.inheritGroupStateFromParent) return;

                StateIndicators.EnableGroup(null, childOptions.groupId);
            });

            return;
        }

        // Increase number in memory
        StateIndicators.StateMemory[uid] = StateIndicators.StateMemory[uid] + 1;

        // Check if should be triggered
        if (options.exclusiveToTriggerIds.length > 0) {
            if (sender == null) {
                return;
            }

            let senderOptions = JSON.parse($(sender).attr("data-state-options"));

            if (!options.exclusiveToTriggerIds.includes(senderOptions.id)) {
                return;
            }
        }

        if (StateIndicators.StateMemory[uid] > 1) {
            return;
        }

        if (options.idleCssClasses.length > 0) {
            options.idleCssClasses.forEach(c => self.removeClass(c));
        }

        if (options.busyCssClasses.length > 0) {
            options.busyCssClasses.forEach(c => self.addClass(c));
        }

        if (options.idleDomAttributes.length > 0) {
            options.idleDomAttributes.forEach(a => self.removeAttr(a.key));
        }

        if (options.busyDomAttributes.length > 0) {
            options.busyDomAttributes.forEach(a => self.attr(a.key, a.value));
        }
    },

    OnElementDisable: function (e, groupId) {
        e.stopPropagation();

        let self = $(this);
        let options = JSON.parse(self.attr("data-state-options"));
        let uid = self.attr("data-state-uid");

        if (self.attr("data-state-role") == "group") {
            if (StateIndicators.StateMemory[uid] <= 0) return;

            StateIndicators.StateMemory[uid] = StateIndicators.StateMemory[uid] - 1;

            if (StateIndicators.StateMemory[uid] == 0) {
                $("[data-state-groupid='" + groupId + "'][data-state-role='group']")
                    .attr("data-state-group-status", "false");
            }

            // Find possible child groups to un-trigger if required
            let childGroup = self.find("[data-state-role='group'][data-state-options]").first();
            let otherGroups = $("[data-state-role='group'][data-state-options][data-state-parentgroupid='" + groupId + "']");

            childGroup = childGroup.add(otherGroups);

            childGroup.each((i, e) => {
                let childOptions = JSON.parse($(e).attr("data-state-options"));

                if (!childOptions.inheritGroupStateFromParent) return;

                StateIndicators.DisableGroup(childOptions.groupId);
            });

            return;
        }

        // Decrease number in memory
        if (StateIndicators.StateMemory[uid] > 0) {
            StateIndicators.StateMemory[uid] = StateIndicators.StateMemory[uid] - 1;
        }

        if (StateIndicators.StateMemory[uid] > 0) {
            return;
        }

        if (options.idleCssClasses.length > 0) {
            options.idleCssClasses.forEach(c => self.addClass(c));
        }

        if (options.busyCssClasses.length > 0) {
            options.busyCssClasses.forEach(c => self.removeClass(c));
        }

        if (options.idleDomAttributes.length > 0) {
            options.idleDomAttributes.forEach(a => self.attr(a.key, a.value));
        }

        if (options.busyDomAttributes.length > 0) {
            options.busyDomAttributes.forEach(a => self.removeAttr(a.key));
        }
    },

    GenerateId: function () {
        var result = '';
        var characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';

        for (var i = 0; i < 30; i++) {
            result += characters.charAt(Math.floor(Math.random() * characters.length));
        }

        return result;
    },

    //InitObserver: function () {
    //    // Setup a mutation observer to track changed DOM elements
    //    var target = $("body")[0];

    //    var observer = new MutationObserver(function (mutations) {
    //        mutations.forEach(function (mutation) {
    //            var nodes = mutation.addedNodes;

    //            if (nodes !== null) {
    //                $([...nodes].filter(x => {
    //                    if (x.nodeType != 1) {
    //                        return false;
    //                    }

    //                    return x.hasAttribute("data-state-options");
    //                })).each(function () {
    //                    console.log("stategroups.js added elements");

    //                    StateIndicators.SetupElementFromOptions($(this).get(0))
    //                });
    //            }
    //        });
    //    });

    //    observer.observe(target, {
    //        attributes: true,
    //        subtree: true
    //    });
    //}
}

// Setup all elements intially available
StateIndicators.SetupAllElements();