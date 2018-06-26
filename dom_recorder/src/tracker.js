let config = require('./config/entry.config.json');
let guid = require('./guid.js');

function attach(beacon) {
    if (!beacon.getAttribute(config.identifierName)) {
        dontTrackChange(beacon, function() { beacon.setAttribute(config.identifierName, guid.create()); });
    }
    return beacon;
}

function dontTrackChange(element, action) {
    //element.setAttribute(config.ignoreAttribute, true);
    action(element);
    //element.removeAttribute(config.ignoreAttribute);
}

function isObserved(element) {
    let type = element.tagName || element.nodeName;
    return config.observableItems.includes(type.toLowerCase());
}

function attributeFilter() {
    return config.observableAttributes;
}

function identity(beacon) {
    return beacon.getAttribute(config.identifierName);
}

module.exports = { attach, identity, dontTrackChange, isObserved, attributeFilter };