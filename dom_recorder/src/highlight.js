"use strict";
let tracker = require('./tracker.js');
var CurrentElement = null;

function highlight(beacon, color, thickness) {
    beacon.style.outline = `${color} solid ${thickness}px`;
}

function highlightAt(x, y, color) {
    let element = document.elementFromPoint(x, y);
    if (CurrentElement === element) {
        return;
    } else {
        extinguishAt();
        CurrentElement = element;

        console.log(CurrentElement);
        tracker.dontTrackChange(CurrentElement, function () {
            highlight(element, color, 2);
        });
    }
  
}

function extinguishAt() {
    if (CurrentElement) {
        tracker.dontTrackChange(CurrentElement, function () {
            CurrentElement.style.outline = `transparent solid 0px`;
        });
    }
}

module.exports = { highlight, highlightAt, extinguishAt };