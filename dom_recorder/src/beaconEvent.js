"use strict";
function notifyAboutBeacon(beacon, type, id) {
    var rect = new DOMRect();
    if (beacon.getBoundingClientRect) {
        rect = beacon.getBoundingClientRect();
    }
    return {
        type: type,
        time: new Date().valueOf(),
        url: document.URL,
        beacon: {
            id: id,
            x: rect.left,
            y: rect.top,
            width: rect.width,
            height: rect.height,
            tag: beacon.tagName,
            tags: {
                'href': beacon.href,
                'src': beacon.src,
                'submit': beacon.submit,
                'tag': beacon.tagName,
                'html': beacon.innerHTML,
                'name': beacon.name,
                'id': beacon.id,
                'title': beacon.title,
                'value': beacon.value,
                'role': beacon.role,
                'innerText': beacon.innerText,
                'className': beacon.className,
                'jsAction': beacon.jsAction,
                'scrollTop': beacon.scrollTop,
                'scrollLeft': beacon.scrollLeft,
                'style': beacon.getAttribute("style")
            }
        }
    };
}

function detected(beacon, id) {
    let beaconEvent = notifyAboutBeacon(beacon, 'beacon-detected', id);
    return beaconEvent;
}

function changed(beacon, id) {
    let beaconEvent = notifyAboutBeacon(beacon, 'beacon-changed', id);
    return beaconEvent;
}

function lost(beacon, id) {
    let beaconEvent = notifyAboutBeacon(beacon, 'beacon-removed', id);
    return beaconEvent;
}

module.exports = { detected, lost, changed };