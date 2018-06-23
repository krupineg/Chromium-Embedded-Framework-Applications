"use strict";

function observeMutation(domObserver, config) {
    const attributesMutationType = 'attributes';
    let beaconEvent = require("./beaconEvent.js");
    let walk = require("./walk.js");
    let guid = require("./guid.js");
    
    function isObserved(element) {
        function isIgnored(element) {
            element.getAttribute(config.ignoreAttribute);
        }
        let type = element.tagName || element.nodeName;
        return config.observableItems.includes(type.toLowerCase()) && !isIgnored(element);
    }
   
    const mutationObserver = new MutationObserver(
        function (mutations) {
            mutations.forEach(function (mutation) {
                if (mutation.type === attributesMutationType && isObserved(mutation.target)) {
                    beaconChanged(mutation.target);
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
        if (isObserved(beacon)) {
            let attached = guid.attach(beacon, config.identifierName);
            let event = beaconEvent.lost(attached, guid.identity(attached, config.identifierName));
            domObserver.raise(event.type, event);
        }
    }

    function beaconChanged(beacon) {
        if (isObserved(beacon)) {
            let attached = guid.attach(beacon, config.identifierName);
            let event = beaconEvent.changed(attached, guid.identity(attached, config.identifierName));
            domObserver.raise(event.type, event);
        }
    }

    function beaconAppears(beacon) {
        if (isObserved(beacon)) {
            let attached = guid.attach(beacon, config.identifierName);
            let event = beaconEvent.detected(attached, guid.identity(attached, config.identifierName));
            domObserver.raise(event.type, event);
        }
    }

    mutationObserver.observe(document, { childList: true, subtree: true, attributes: true });
    walk.dom(document.body, beaconAppears);
}

module.exports = { observeMutation };