"use strict";
function guid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
            .toString(16)
            .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
}

function attach(beacon, propertyName) {
    if (!beacon.getAttribute(propertyName)) {
        beacon.setAttribute(propertyName, guid());
    }
    return beacon;
}

function identity(beacon, propertyName) {
    return beacon.getAttribute(propertyName);
}

module.exports = { identity, attach };