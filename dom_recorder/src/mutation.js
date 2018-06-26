"use strict";

function observeMutation(domObserver, additionalAttributesToObserve) {
    additionalAttributesToObserve = additionalAttributesToObserve || [];
    const attributesMutationType = 'attributes';
    let beaconEvent = require("./beaconEvent.js");
    let tracker = require("./tracker.js");
    let walk = require("./walk.js");
   
    const mutationObserver = new MutationObserver(
        function (mutations) {
            mutations.forEach(function (mutation) {
                if (mutation.type === attributesMutationType) {
                    beaconChanged(mutation.target, mutation.attributeName);
                }
                mutation.addedNodes.forEach(function (addedNode) {
                    walk.dom(addedNode, beaconAppears);
                });
                mutation.removedNodes.forEach(function (removedNode) {
                    walk.dom(removedNode, beaconDisappears);
                });
            });
        });
    
    function beaconDisappears(beacon) {
        if (tracker.isObserved(beacon)) {
            let attached = tracker.attach(beacon);
            let event = beaconEvent.lost(attached, tracker.identity(attached));
            domObserver.raise(event.type, event);
        }
    }

    function beaconChanged(beacon, changed) {
        if (tracker.isObserved(beacon) && tracker) {
            let attached = tracker.attach(beacon);
            let event = beaconEvent.changed(attached, tracker.identity(attached), changed);
            domObserver.raise(event.type, event);
        }
    }

    function beaconAppears(beacon) {
        if (tracker.isObserved(beacon)) {
            let attached = tracker.attach(beacon);
            let event = beaconEvent.detected(attached, tracker.identity(attached));
            domObserver.raise(event.type, event);
        }
    }

    mutationObserver.observe(document, { childList: true, subtree: true, attributes: true, attributeFilter: additionalAttributesToObserve.concat(tracker.attributeFilter()) });
    walk.dom(document.body, beaconAppears);
}

module.exports = { observeMutation };