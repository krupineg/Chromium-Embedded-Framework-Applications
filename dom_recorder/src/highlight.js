"use strict";
const defaultColor = '#f00';

function highlight(beacon, color, thickness) {
    var highlightColor = /(^#[0-9A-F]{6}$)|(^#[0-9A-F]{3}$)/i.test('color') ? color : defaultColor;
    beacon.style.outline = `${highlightColor} solid ${thickness}px`;
    
}

function highlightAt(x, y, color, ignoreAttribute) {
    let element = document.elementFromPoint(x, y);
    element.setAttribute(ignoreAttribute, true);
    highlight(element, color, 2);
    element.removeAttribute(ignoreAttribute);
}

function extinguishAt(x, y, ignoreAttribute) {
    let element = document.elementFromPoint(x, y);
    element.setAttribute(ignoreAttribute, true);
    element.style.outline = `transparent solid 0px`;
    element.ignoremutation = false;
}

module.exports = { highlight, highlightAt, extinguishAt };